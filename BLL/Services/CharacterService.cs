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

        // ── Base stats seeded on character creation, keyed by class name ─────────────
        private static readonly Dictionary<string, PlayerStat> BaseStats = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Knight"] = new PlayerStat
            {
                CurrentHp = 200, MaxHp = 200,
                Atk = 15, Def = 20,
                MoveSpeed = 100, AttackSpeed = 90,
                CritRate = 5, CritDamage = 150,
                DamageBonus = 0, SkillPoints = 0
            },
            ["Archer"] = new PlayerStat
            {
                CurrentHp = 140, MaxHp = 140,
                Atk = 20, Def = 10,
                MoveSpeed = 115, AttackSpeed = 120,
                CritRate = 12, CritDamage = 175,
                DamageBonus = 0, SkillPoints = 0
            },
            ["Mage"] = new PlayerStat
            {
                CurrentHp = 120, MaxHp = 120,
                Atk = 25, Def = 8,
                MoveSpeed = 105, AttackSpeed = 100,
                CritRate = 8, CritDamage = 160,
                DamageBonus = 0, SkillPoints = 0
            }
        };

        public CharacterService(
            IPlayerProfileRepository profileRepository,
            IPlayerStatRepository statRepository,
            IPlayerProfileService playerProfileService)
        {
            _profileRepository = profileRepository;
            _statRepository = statRepository;
            _playerProfileService = playerProfileService;
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

            // Seed class-appropriate base stats.
            if (!BaseStats.TryGetValue(request.SelectedClass, out var template))
                throw new ArgumentException($"Unknown class '{request.SelectedClass}'.");

            var stat = new PlayerStat
            {
                PlayerProfileId = playerProfileId,
                CurrentHp     = template.CurrentHp,
                MaxHp         = template.MaxHp,
                Atk           = template.Atk,
                Def           = template.Def,
                MoveSpeed     = template.MoveSpeed,
                AttackSpeed   = template.AttackSpeed,
                CritRate      = template.CritRate,
                CritDamage    = template.CritDamage,
                DamageBonus   = template.DamageBonus,
                SkillPoints   = template.SkillPoints
            };

            await _statRepository.Create(stat);

            return BuildCharacterResponse(profile, stat);
        }

        // ── 2. View Attribute List ─────────────────────────────────────────────────────

        public async Task<PlayerStatsResponseDto> GetStats(int playerProfileId)
        {
            var stat = await _statRepository.GetByPlayerProfileId(playerProfileId)
                ?? throw new KeyNotFoundException("Character stats not found. Please create a character first.");

            return MapToStatsDto(stat);
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
    }
}
