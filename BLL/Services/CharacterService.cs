using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;

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

        public CharacterService(
            IPlayerProfileRepository profileRepository,
            IPlayerStatRepository statRepository,
            IPlayerProfileService playerProfileService,
            IClassConfigRepository classConfigRepository)
        {
            _profileRepository = profileRepository;
            _statRepository = statRepository;
            _playerProfileService = playerProfileService;
            _classConfigRepository = classConfigRepository;
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
                CurrentHp     = template.MaxHp, // Start with full HP
                MaxHp         = template.MaxHp,
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
            var stat = await _statRepository.GetByPlayerProfileId(playerProfileId)
                ?? throw new KeyNotFoundException("Character stats not found. Please create a character first.");

            var dto = MapToStatsDto(stat);

            // Tích hợp chỉ số từ trang bị (nếu có)
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
                
                // Đảm bảo CurrentHp luôn tăng theo MaxHp (tuỳ logic game, ở đây có thể cập nhật)
                // (Chưa cập nhật CurrentHp ở đây vì CurrentHp do logic hồi máu/chịu đòn quyết định)
            }

            return dto;
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
                Stats               = MapToStatsDto(stat)
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

        private static PlayerStatsResponseDto MapToStatsDto(PlayerStat stat)
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
                TotalDeaths = stat.TotalDeaths
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
                Stats           = MapToStatsDto(stat)
            };
        }
        // ── 5. Get Class Configs ───────────────────────────────────────────────────────

        public async Task<IEnumerable<ClassConfig>> GetAllClassConfigs()
        {
            return await _classConfigRepository.GetAll();
        }
    }
}
