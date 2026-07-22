using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class CharacterService : ICharacterService
    {
        // Skill points awarded per level-up (used by other services that handle XP gain).
        public const int SkillPointsPerLevel = 3;

        private readonly IPlayerProfileRepository _profileRepository;
        private readonly IPlayerStatRepository _statRepository;
        private readonly IPlayerProfileService _playerProfileService;
        private readonly IClassConfigRepository _classConfigRepository;
        private readonly DAL.Data.MysticJourneyDbContext _context;

        public CharacterService(
            IPlayerProfileRepository profileRepository,
            IPlayerStatRepository statRepository,
            IPlayerProfileService playerProfileService,
            IClassConfigRepository classConfigRepository,
            DAL.Data.MysticJourneyDbContext context)
        {
            _profileRepository = profileRepository;
            _statRepository = statRepository;
            _playerProfileService = playerProfileService;
            _classConfigRepository = classConfigRepository;
            _context = context;
        }

        // ── 1. Create Character ────────────────────────────────────────────────────────

        public async Task<CharacterResponseDto> CreateCharacter(int playerProfileId, CreateCharacterRequestDto request)
        {
            // Load the existing PlayerProfile (created automatically on account registration).
            var profile = await _profileRepository.GetPlayerProfileByIdWithStats(playerProfileId)
                ?? throw new KeyNotFoundException($"PlayerProfile {playerProfileId} not found.");

            // Guard: a character can only be created once.
            if (profile.PlayerStats != null)
                throw new InvalidOperationException("Character has already been created for this account. Use the upgrade endpoint to improve attributes.");

            // Stamp the character name and chosen class on the profile.
            profile.DisplayName = request.CharacterName.Trim();
            profile.Class = request.SelectedClass;
            profile.UpdatedAt = DateTime.UtcNow;
            _playerProfileService.RecalculateEnergy(profile);
            await _profileRepository.UpdatePlayerProfile(profile);

            // Fetch class-appropriate base stats from DB.
            var template = await _classConfigRepository.GetByClassName(request.SelectedClass);
            if (template == null)
                throw new ArgumentException($"Unknown class '{request.SelectedClass}'.");

            var stat = new PlayerStat
            {
                PlayerProfileId = playerProfileId,
                MaxHp         = template.MaxHp, // Initialize MaxHp
                CurrentHp     = template.MaxHp, // Start with full HP
                Atk           = template.Atk,
                Def           = template.Def,
                MoveSpeed     = template.MoveSpeed,
                AttackSpeed   = template.AttackSpeed,
                CritRate      = template.CritRate,
                CritDamage    = template.CritDamage,
                DamageBonus   = template.DamageBonus,
                SkillPoints   = 0
            };

            await _statRepository.Create(stat);

            return BuildCharacterResponse(profile, stat);
        }

        // ── 2. View Attribute List ─────────────────────────────────────────────────────

        public async Task<PlayerStatsResponseDto> GetStats(int playerProfileId)
        {
            var profile = await _context.PlayerProfiles
                .Include(p => p.PlayerStats)
                .Include(p => p.PlayerBuffs)
                .Include(p => p.PlayerAchievements)
                    .ThenInclude(pa => pa.Achievement)
                .FirstOrDefaultAsync(p => p.PlayerProfileId == playerProfileId)
                ?? throw new KeyNotFoundException("Character stats not found. Please create a character first.");

            var stat = profile.PlayerStats;
            var dto = MapToStatsDto(stat, profile.PlayerBuffs);

            // 1. Tích hợp chỉ số từ trang bị (nếu có)
            var snapshot = await _statRepository.GetSnapshotByPlayerProfileId(playerProfileId);
            if (snapshot != null)
            {
                dto.MaxHp += snapshot.MaxHp;
                dto.Atk += snapshot.Atk;
                dto.Def += snapshot.Def;
                dto.MoveSpeed += snapshot.MoveSpeed;
                dto.AttackSpeed += snapshot.AttackSpeed;
                dto.CritRate += snapshot.CritRate;
                dto.CritDamage += snapshot.CritDamage;
                dto.DamageBonus += snapshot.DamageBonus;
            }

            // 2. Tích hợp chỉ số ẩn (Passive Buffs) từ danh hiệu
            if (profile.PlayerAchievements != null && profile.PlayerAchievements.Any())
            {
                var completedBuffs = profile.PlayerAchievements
                    .Where(pa => pa.IsCompleted && pa.Achievement != null && !string.IsNullOrEmpty(pa.Achievement.BuffDescription))
                    .Select(pa => pa.Achievement!.BuffDescription);

                var totals = BLL.Helpers.AchievementBuffCalculator.ParseMany(completedBuffs);

                // Áp dụng phần trăm tổng (ví dụ: 1 + 0.02 = 1.02)
                dto.MaxHp = (int)(dto.MaxHp * (1m + totals.MaxHpPercent));
                dto.Atk = (int)(dto.Atk * (1m + totals.AtkPercent));
                dto.Def = (int)(dto.Def * (1m + totals.DefPercent));
                dto.MoveSpeed = (int)(dto.MoveSpeed * (1m + totals.MoveSpeedPercent));
                dto.AttackSpeed = (int)(dto.AttackSpeed * (1m + totals.AttackSpeedPercent));
                
                // CritRate, DamageBonus là các chỉ số cộng thẳng % nên ta cộng trực tiếp
                dto.CritRate += (int)totals.CritRatePercent;
                dto.DamageBonus += (int)totals.DamageBonusPercent;
            }
                
                // Đảm bảo CurrentHp luôn tăng theo MaxHp (tuỳ logic game, ở đây có thể cập nhật)
                // (Chưa cập nhật CurrentHp ở đây vì CurrentHp do logic hồi máu/chịu đòn quyết định)

            return dto;
        }

        public async Task SyncBuffs(int playerProfileId, UpdatePlayerBuffsRequest request)
        {
            var existingBuffs = await _context.PlayerBuffs.Where(b => b.PlayerProfileId == playerProfileId).ToListAsync();
            _context.PlayerBuffs.RemoveRange(existingBuffs);

            if (request.Buffs != null && request.Buffs.Any())
            {
                var newBuffs = request.Buffs.Select(b => new PlayerBuff
                {
                    PlayerProfileId = playerProfileId,
                    BuffName = b.BuffName,
                    IconName = b.IconName,
                    DurationRemaining = b.DurationRemaining,
                    IsDebuff = b.IsDebuff
                });
                await _context.PlayerBuffs.AddRangeAsync(newBuffs);
            }

            await _context.SaveChangesAsync();
        }

        // ── 3. Upgrade Character ───────────────────────────────────────────────────────

        public async Task<UpgradeAttributeResponseDto> UpgradeAttribute(int playerProfileId, UpgradeAttributeRequestDto request)
        {
            var stat = await _statRepository.GetByPlayerProfileId(playerProfileId)
                ?? throw new KeyNotFoundException("Character stats not found. Please create a character first.");

            if (stat.SkillPoints < request.Amount)
                throw new InvalidOperationException(
                    $"Not enough skill points. You have {stat.SkillPoints} but need {request.Amount}.");

            // Apply the upgrade.
            ApplyAttributeUpgrade(stat, request.AttributeName, request.Amount);

            // Deduct spent skill points.
            stat.SkillPoints -= request.Amount;

            await _statRepository.Update(stat);

            return new UpgradeAttributeResponseDto
            {
                UpgradedAttribute   = request.AttributeName,
                AmountSpent         = request.Amount,
                RemainingSkillPoints = stat.SkillPoints,
                Stats               = MapToStatsDto(stat, (await _context.PlayerProfiles.Include(p => p.PlayerBuffs).FirstAsync(p => p.PlayerProfileId == playerProfileId)).PlayerBuffs)
            };
        }

        // ── Helpers ───────────────────────────────────────────────────────────────────

        /// <summary>
        /// Increments the chosen attribute by <paramref name="amount"/> in-place.
        /// For HP upgrades both MaxHp and CurrentHp are increased so the player
        /// does not immediately appear wounded after upgrading.
        /// </summary>
        private static void ApplyAttributeUpgrade(PlayerStat stat, string attributeName, int amount)
        {
            switch (attributeName.ToLowerInvariant())
            {
                case "maxhp":
                    stat.MaxHp     += amount;
                    stat.CurrentHp += amount;   // also restore the extra HP
                    break;
                case "atk":
                    stat.Atk += amount;
                    break;
                case "def":
                    stat.Def += amount;
                    break;
                case "movespeed":
                    stat.MoveSpeed += amount;
                    break;
                case "attackspeed":
                    stat.AttackSpeed += amount;
                    break;
                case "critrate":
                    stat.CritRate += amount;
                    break;
                case "critdamage":
                    stat.CritDamage += amount;
                    break;
                case "damagebonus":
                    stat.DamageBonus += amount;
                    break;
                default:
                    // This branch should never be reached because the DTO already validates
                    // AttributeName via [RegularExpression], but we keep it for safety.
                    throw new ArgumentException($"Unknown attribute '{attributeName}'.");
            }
        }

        public async Task UpdateHp(int playerProfileId, int currentHp)
        {
            var stat = await _statRepository.GetByPlayerProfileId(playerProfileId);

            if (stat == null)
            {
                throw new KeyNotFoundException("PlayerStats not found for the specified profile.");
            }

            // Ensure currentHp doesn't exceed maxHp
            stat.CurrentHp = Math.Min(currentHp, stat.MaxHp);
            stat.UpdatedAt = DateTime.UtcNow;

            await _statRepository.Update(stat);
        }

        private static PlayerStatsResponseDto MapToStatsDto(PlayerStat stat, ICollection<PlayerBuff> buffs = null)
        {
            return new PlayerStatsResponseDto
            {
                CurrentHp   = stat.CurrentHp,
                MaxHp       = stat.MaxHp,
                Atk         = stat.Atk,
                Def         = stat.Def,
                MoveSpeed   = stat.MoveSpeed,
                AttackSpeed = stat.AttackSpeed,
                CritRate    = stat.CritRate,
                CritDamage  = stat.CritDamage,
                DamageBonus = stat.DamageBonus,
                SkillPoints = stat.SkillPoints,
                TotalWins   = stat.TotalWins,
                TotalLosses = stat.TotalLosses,
                TotalKills  = stat.TotalKills,
                TotalDeaths = stat.TotalDeaths,
                ActiveBuffs = buffs != null ? buffs.Select(b => new PlayerBuffDTO
                {
                    BuffName = b.BuffName,
                    IconName = b.IconName,
                    DurationRemaining = b.DurationRemaining,
                    IsDebuff = b.IsDebuff
                }).ToList() : new List<PlayerBuffDTO>()
            };
        }

        private static CharacterResponseDto BuildCharacterResponse(PlayerProfile profile, PlayerStat stat)
        {
            return new CharacterResponseDto
            {
                PlayerProfileId = profile.PlayerProfileId,
                AccountId       = profile.AccountId,
                CharacterName   = profile.DisplayName,
                PlayerClass     = profile.Class,
                Level           = profile.Level,
                ExperiencePoints = profile.ExperiencePoints,
                Gold            = profile.Gold,
                Gems            = profile.Gems,
                Energy          = profile.CurrentEnergy,
                MaxEnergy       = profile.MaxEnergy,
                LastEnergyUpdateTime = profile.LastEnergyUpdateTime,
                CreatedAt       = profile.CreatedAt,
                Stats           = MapToStatsDto(stat, profile.PlayerBuffs)
            };
        }

        // ── 4. Level Up Stat Allocation ────────────────────────────────────────────────
        public async Task<List<string>> GetLevelUpOptions(int playerProfileId)
        {
            var profile = await _context.PlayerProfiles.FirstOrDefaultAsync(p => p.PlayerProfileId == playerProfileId)
                ?? throw new KeyNotFoundException("PlayerProfile not found.");

            if (profile.AvailableStatPoints <= 0)
                throw new InvalidOperationException("No stat points available.");

            if (!string.IsNullOrEmpty(profile.CachedStatRolls))
            {
                return profile.CachedStatRolls.Split(',').ToList();
            }

            // Roll 5 new stats out of 8
            var allStats = new List<string> { "MaxHp", "Atk", "Def", "MoveSpeed", "AttackSpeed", "CritRate", "CritDamage", "DamageBonus" };
            var random = new Random();
            var rolledStats = allStats.OrderBy(x => random.Next()).Take(5).ToList();
            
            profile.CachedStatRolls = string.Join(",", rolledStats);
            await _context.SaveChangesAsync();

            return rolledStats;
        }

        public async Task<PlayerStatsResponseDto> AllocateStat(int playerProfileId, string statName)
        {
            var profile = await _context.PlayerProfiles
                .Include(p => p.PlayerStats)
                .Include(p => p.PlayerBuffs)
                .FirstOrDefaultAsync(p => p.PlayerProfileId == playerProfileId)
                ?? throw new KeyNotFoundException("PlayerProfile not found.");

            if (profile.AvailableStatPoints <= 0)
                throw new InvalidOperationException("No stat points available.");

            if (string.IsNullOrEmpty(profile.CachedStatRolls))
                throw new InvalidOperationException("No cached stats to allocate. Please request level up options first.");

            var availableOptions = profile.CachedStatRolls.Split(',').Select(s => s.ToLowerInvariant()).ToList();
            if (!availableOptions.Contains(statName.ToLowerInvariant()))
                throw new InvalidOperationException($"Stat '{statName}' is not a valid option for this roll.");

            if (profile.PlayerStats == null)
                throw new InvalidOperationException("Character stats not found.");

            int amount = GetStatIncrementAmount(statName);
            ApplyAttributeUpgrade(profile.PlayerStats, statName, amount);

            profile.AvailableStatPoints--;
            profile.CachedStatRolls = string.Empty;

            await _context.SaveChangesAsync();

            return MapToStatsDto(profile.PlayerStats, profile.PlayerBuffs);
        }

        private static int GetStatIncrementAmount(string statName)
        {
            switch (statName.ToLowerInvariant())
            {
                case "maxhp": return 20;
                case "atk": return 3;
                case "def": return 2;
                case "movespeed": return 1;
                case "attackspeed": return 1;
                case "critrate": return 1;
                case "critdamage": return 2;
                case "damagebonus": return 1;
                default: return 1;
            }
        }

        // ── 5. Get Class Configs ───────────────────────────────────────────────────────

        public async Task<IEnumerable<ClassConfig>> GetAllClassConfigs()
        {
            return await _classConfigRepository.GetAll();
        }
    }
}
