using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    // Executes core business logic for i character service.
    public class CharacterService : ICharacterService
    {
        public const int SkillPointsPerLevel = 3;

        private readonly IPlayerProfileRepository _profileRepository;
        private readonly IPlayerStatRepository _statRepository;
        private readonly IPlayerProfileService _playerProfileService;
        private readonly IClassConfigRepository _classConfigRepository;
        private readonly ITransactionManager _transactionManager;
        private readonly DAL.Data.MysticJourneyDbContext _context;

        // Initialize this instance from profile repository, stat repository, player profile service, and class config repository and store profile repository, stat repository, player profile service, class config repository, and transaction manager for later operations.
        public CharacterService(
            IPlayerProfileRepository profileRepository,
            IPlayerStatRepository statRepository,
            IPlayerProfileService playerProfileService,
            IClassConfigRepository classConfigRepository,
            ITransactionManager transactionManager,
            DAL.Data.MysticJourneyDbContext context)
        {
            _profileRepository = profileRepository;
            _statRepository = statRepository;
            _playerProfileService = playerProfileService;
            _classConfigRepository = classConfigRepository;
            _transactionManager = transactionManager;
            _context = context;
        }


        // Execute character creation inside one transaction so profile, stats, starter skill, and default skin are committed together or rolled back together.
        public Task<CharacterResponseDto> CreateCharacter(int playerProfileId, CreateCharacterRequestDto request)
            => _transactionManager.ExecuteInTransactionAsync(() => CreateCharacterCore(playerProfileId, request));

        // Load the profile, reject duplicate characters, validate the selected class, initialize class stats and progression, grant the starter skill and default skin, save all changes, and return the character response.
        private async Task<CharacterResponseDto> CreateCharacterCore(int playerProfileId, CreateCharacterRequestDto request)
        {
            var profile = await _profileRepository.GetPlayerProfileByIdWithStats(playerProfileId)
                ?? throw new KeyNotFoundException($"PlayerProfile {playerProfileId} not found.");

            if (profile.PlayerStats != null)
                throw new InvalidOperationException("Character has already been created for this account. Use the upgrade endpoint to improve attributes.");  // Unexpected runtime state — propagate to global error handler

            var template = await _classConfigRepository.GetByClassName(request.SelectedClass);
            if (template == null)  // Entity not found — short-circuit with appropriate error result
                throw new ArgumentException($"Unknown class '{request.SelectedClass}'.");

            profile.DisplayName = request.CharacterName.Trim();
            profile.Class = request.SelectedClass;
            profile.UpdatedAt = DateTime.UtcNow;
            _playerProfileService.RecalculateEnergy(profile);
            await _profileRepository.UpdatePlayerProfile(profile);

            var stat = new PlayerStat
            {
                PlayerProfileId = playerProfileId,
                MaxHp         = template.MaxHp,
                CurrentHp     = template.MaxHp,
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

            int starterSkillId = request.SelectedClass switch
            {
                "Archer" => 1,
                "Mage" => 5,
                "Knight" => 7,
                _ => throw new ArgumentException($"Unknown class '{request.SelectedClass}'.")
            };

            var starterSkill = await _context.Skills
                .SingleOrDefaultAsync(skill =>
                    skill.SkillId == starterSkillId &&
                    skill.IsActive &&
                    skill.ClassRequirement == request.SelectedClass)
                ?? throw new InvalidOperationException(  // Unexpected runtime state — propagate to global error handler
                    $"Starter skill {starterSkillId} is not configured for class '{request.SelectedClass}'.");

            int defaultSkinId = request.SelectedClass switch
            {
                "Knight" => 1,
                "Archer" => 2,
                "Mage" => 3,
                _ => throw new ArgumentException($"Unknown class '{request.SelectedClass}'.")
            };

            if (!await _context.Skins.AnyAsync(skin => skin.SkinId == defaultSkinId && skin.IsActive))  // Check existence without loading the full entity
                throw new InvalidOperationException($"Default skin {defaultSkinId} is not configured for class '{request.SelectedClass}'.");  // Unexpected runtime state — propagate to global error handler

            var unlockedAt = DateTime.UtcNow;

            _context.PlayerSkills.Add(new PlayerSkill
            {
                PlayerProfileId = playerProfileId,
                SkillId = starterSkill.SkillId,
                Level = 1,
                Experience = 0,
                EquippedSlot = null,
                UnlockedAt = unlockedAt
            });

            _context.PlayerSkins.Add(new PlayerSkin
            {
                PlayerProfileId = playerProfileId,
                SkinId = defaultSkinId,
                IsEquipped = true,
                UnlockedAt = unlockedAt
            });

            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database

            return BuildCharacterResponse(profile, stat);
        }


        // Load the player's base stats with buffs and achievements, apply the saved equipment snapshot and achievement bonuses, then return the effective stat response.
        public async Task<PlayerStatsResponseDto> GetStats(int playerProfileId)
        {
            var profile = await _context.PlayerProfiles
                .Include(p => p.PlayerStats)  // Eagerly load related navigation entities to avoid N+1 queries
                .Include(p => p.PlayerBuffs)  // Eagerly load related navigation entities to avoid N+1 queries
                .Include(p => p.PlayerAchievements)  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(pa => pa.Achievement)
                .FirstOrDefaultAsync(p => p.PlayerProfileId == playerProfileId)  // Fetch single matching record or null if not found
                ?? throw new KeyNotFoundException("Character stats not found. Please create a character first.");

            var stat = profile.PlayerStats
                ?? throw new KeyNotFoundException("Character stats not found. Please create a character first.");
            var dto = MapToStatsDto(stat, profile.PlayerBuffs);

            var snapshot = await _statRepository.GetSnapshotByPlayerProfileId(playerProfileId);
            if (snapshot != null)  // Entity exists — proceed with conditional branch
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

            if (profile.PlayerAchievements != null && profile.PlayerAchievements.Any())
            {
                var completedBuffs = profile.PlayerAchievements
                    .Where(pa => pa.IsCompleted && pa.Achievement != null && !string.IsNullOrEmpty(pa.Achievement.BuffDescription))  // Filter records matching the predicate
                    .Select(pa => pa.Achievement!.BuffDescription);

                var totals = BLL.Helpers.AchievementBuffCalculator.ParseMany(completedBuffs);

                dto.MaxHp = BLL.Helpers.AchievementBuffCalculator.CombineMaxHp(stat.MaxHp, snapshot?.MaxHp ?? 0, totals.MaxHpPercent);
                dto.Atk = BLL.Helpers.AchievementBuffCalculator.ApplyPercent(dto.Atk, totals.AtkPercent);
                dto.Def = BLL.Helpers.AchievementBuffCalculator.ApplyPercent(dto.Def, totals.DefPercent);
                dto.MoveSpeed = BLL.Helpers.AchievementBuffCalculator.ApplyPercent(dto.MoveSpeed, totals.MoveSpeedPercent);
                dto.AttackSpeed = BLL.Helpers.AchievementBuffCalculator.ApplyPercent(dto.AttackSpeed, totals.AttackSpeedPercent);

                dto.CritRate += (int)totals.CritRatePercent;
                dto.DamageBonus += (int)totals.DamageBonusPercent;
            }


            return dto;
        }

        // Replace the player's persisted buff rows with the supplied active buffs, save the new set, and return the recalculated effective stats.
        public async Task SyncBuffs(int playerProfileId, UpdatePlayerBuffsRequest request)
        {
            var existingBuffs = await _context.PlayerBuffs.Where(b => b.PlayerProfileId == playerProfileId).ToListAsync();  // Materialize the query into a list from the database
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

            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
        }


        // Executes core business logic for upgrade attribute.
        // Logic details: delegates data queries and updates to repository layer; throws InvalidOperationException, KeyNotFoundException on invalid state or rule violations.
        // Returns the computed UpgradeAttributeResponseDto result asynchronously.
        public async Task<UpgradeAttributeResponseDto> UpgradeAttribute(int playerProfileId, UpgradeAttributeRequestDto request)
        {
            var stat = await _statRepository.GetByPlayerProfileId(playerProfileId)
                ?? throw new KeyNotFoundException("Character stats not found. Please create a character first.");

            if (stat.SkillPoints < request.Amount)
                throw new InvalidOperationException(  // Unexpected runtime state — propagate to global error handler
                    $"Not enough skill points. You have {stat.SkillPoints} but need {request.Amount}.");

            ApplyAttributeUpgrade(stat, request.AttributeName, request.Amount);

            stat.SkillPoints -= request.Amount;

            await _statRepository.Update(stat);

            return new UpgradeAttributeResponseDto
            {
                UpgradedAttribute   = request.AttributeName,
                AmountSpent         = request.Amount,
                RemainingSkillPoints = stat.SkillPoints,
                Stats               = MapToStatsDto(stat, (await _context.PlayerProfiles.Include(p => p.PlayerBuffs).FirstAsync(p => p.PlayerProfileId == playerProfileId)).PlayerBuffs)  // Eagerly load related navigation entities to avoid N+1 queries
            };
        }


        // Executes core business logic for apply attribute upgrade.
        // Logic details: throws ArgumentException on invalid state or rule violations.
        private static void ApplyAttributeUpgrade(PlayerStat stat, string attributeName, int amount)
        {
            switch (attributeName.ToLowerInvariant())
            {
                case "maxhp":
                    stat.MaxHp     += amount;
                    stat.CurrentHp += amount;
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
                    throw new ArgumentException($"Unknown attribute '{attributeName}'.");
            }
        }

        // Executes core business logic for update hp.
        // Logic details: delegates data queries and updates to repository layer; throws KeyNotFoundException on invalid state or rule violations.
        // Completes asynchronously upon successful execution.
        public async Task UpdateHp(int playerProfileId, int currentHp)
        {
            var stat = await _statRepository.GetByPlayerProfileId(playerProfileId);

            if (stat == null)  // Entity not found — short-circuit with appropriate error result
            {
                throw new KeyNotFoundException("PlayerStats not found for the specified profile.");
            }

            int effectiveMaxHp = await ResolveEffectiveMaxHp(stat);

            // Clamp the calculated value to the minimum and maximum accepted by this domain rule.
            stat.CurrentHp = Math.Clamp(currentHp, 0, effectiveMaxHp);
            stat.UpdatedAt = DateTime.UtcNow;

            await _statRepository.Update(stat);
        }

        // Queries the database to retrieve get effective max hp records.
        // Returns the computed numeric count or database ID result.
        public async Task<int> GetEffectiveMaxHp(int playerProfileId)
        {
            var stat = await _statRepository.GetByPlayerProfileId(playerProfileId);
            return stat == null ? 0 : await ResolveEffectiveMaxHp(stat);
        }

        // Queries the database to retrieve resolve effective max hp records.
        // Returns the computed numeric count or database ID result.
        private async Task<int> ResolveEffectiveMaxHp(PlayerStat stat)
        {
            var snapshot = await _statRepository.GetSnapshotByPlayerProfileId(stat.PlayerProfileId);

            var buffDescriptions = await _context.PlayerAchievements
                .Where(pa => pa.PlayerProfileId == stat.PlayerProfileId  // Filter records matching the predicate
                          && pa.IsCompleted
                          && pa.Achievement != null
                          && pa.Achievement.BuffDescription != null)
                .Select(pa => pa.Achievement!.BuffDescription)
                .ToListAsync();  // Materialize the query into a list from the database

            var totals = BLL.Helpers.AchievementBuffCalculator.ParseMany(buffDescriptions);
            return BLL.Helpers.AchievementBuffCalculator.CombineMaxHp(
                stat.MaxHp, snapshot?.MaxHp ?? 0, totals.MaxHpPercent);
        }

        // Executes core business logic for map to stats dto.
        private static PlayerStatsResponseDto MapToStatsDto(PlayerStat stat, ICollection<PlayerBuff>? buffs = null)
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

        // Executes core business logic for build character response.
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

        // Queries the database to retrieve get level up options records.
        // Returns the matching List<string entity result or default if not found.
        public async Task<List<string>> GetLevelUpOptions(int playerProfileId)
        {
            var profile = await _context.PlayerProfiles.FirstOrDefaultAsync(p => p.PlayerProfileId == playerProfileId)  // Fetch single matching record or null if not found
                ?? throw new KeyNotFoundException("PlayerProfile not found.");

            if (profile.AvailableStatPoints <= 0)
                throw new InvalidOperationException("No stat points available.");  // Unexpected runtime state — propagate to global error handler

            if (!string.IsNullOrEmpty(profile.CachedStatRolls))
            {
                return profile.CachedStatRolls.Split(',').ToList();
            }

            var allStats = new List<string> { "MaxHp", "Atk", "Def", "MoveSpeed", "AttackSpeed", "CritRate", "CritDamage", "DamageBonus" };
            var random = new Random();
            var rolledStats = allStats.OrderBy(x => random.Next()).Take(5).ToList();  // Apply pagination limit — cap result set size

            profile.CachedStatRolls = string.Join(",", rolledStats);
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database

            return rolledStats;
        }

        // Queries the database to retrieve allocate stat records.
        // Query details: eagerly loads related entity navigation properties.
        // Returns the matching PlayerStatsResponseDto entity result or default if not found.
        public async Task<PlayerStatsResponseDto> AllocateStat(int playerProfileId, string statName)
        {
            var profile = await _context.PlayerProfiles
                .Include(p => p.PlayerStats)  // Eagerly load related navigation entities to avoid N+1 queries
                .Include(p => p.PlayerBuffs)  // Eagerly load related navigation entities to avoid N+1 queries
                .FirstOrDefaultAsync(p => p.PlayerProfileId == playerProfileId)  // Fetch single matching record or null if not found
                ?? throw new KeyNotFoundException("PlayerProfile not found.");

            if (profile.AvailableStatPoints <= 0)
                throw new InvalidOperationException("No stat points available.");  // Unexpected runtime state — propagate to global error handler

            if (string.IsNullOrEmpty(profile.CachedStatRolls))  // Mandatory string argument is null or empty — fail fast
                throw new InvalidOperationException("No cached stats to allocate. Please request level up options first.");  // Unexpected runtime state — propagate to global error handler

            var availableOptions = profile.CachedStatRolls.Split(',').Select(s => s.ToLowerInvariant()).ToList();
            if (!availableOptions.Contains(statName.ToLowerInvariant()))
                throw new InvalidOperationException($"Stat '{statName}' is not a valid option for this roll.");  // Unexpected runtime state — propagate to global error handler

            if (profile.PlayerStats == null)
                throw new InvalidOperationException("Character stats not found.");  // Unexpected runtime state — propagate to global error handler

            int amount = GetStatIncrementAmount(statName);
            ApplyAttributeUpgrade(profile.PlayerStats, statName, amount);

            profile.AvailableStatPoints--;
            profile.CachedStatRolls = string.Empty;

            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database

            return MapToStatsDto(profile.PlayerStats, profile.PlayerBuffs);
        }

        // Executes core business logic for get stat increment amount.
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
    }
}
