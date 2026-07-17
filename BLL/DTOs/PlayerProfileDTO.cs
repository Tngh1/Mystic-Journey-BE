using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BLL.DTOs
{
    // ============ PlayerProfile ============
    public class PlayerProfileResponseDto
    {
        public int PlayerProfileId { get; set; }
        public int AccountId { get; set; }
        public string? AccountEmail { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public string PlayerClass { get; set; } = string.Empty;
        public int Level { get; set; }
        public int ExperiencePoints { get; set; }
        public decimal Gold { get; set; }
        public decimal Gems { get; set; }
        public int Energy { get; set; }
        public int MaxEnergy { get; set; }
        public DateTime LastEnergyUpdateTime { get; set; }
        public float CorruptionLevel { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastFreeGachaTime { get; set; }
        public bool HasChangedName { get; set; }
    }

    public class PlayerProfileDetailResponseDto : PlayerProfileResponseDto
    {
        public PlayerStatsResponseDto? Stats { get; set; }
    }

    public class PlayerStatsResponseDto
    {
        public int CurrentHp { get; set; }
        public int MaxHp { get; set; }
        public int Atk { get; set; }
        public int Def { get; set; }
        public int MoveSpeed { get; set; }
        public int AttackSpeed { get; set; }
        public int CritRate { get; set; }
        public int CritDamage { get; set; }
        public int DamageBonus { get; set; }
        public int SkillPoints { get; set; }
        public int TotalWins { get; set; }
        public int TotalLosses { get; set; }
        public int TotalKills { get; set; }
        public int TotalDeaths { get; set; }
        public List<PlayerBuffDTO> ActiveBuffs { get; set; } = new List<PlayerBuffDTO>();
    }

    public class UpdatePlayerProfileRequestDto
    {
        public string? DisplayName { get; set; }
        public string? AvatarUrl { get; set; }
        public string? PlayerClass { get; set; }
        public int Level { get; set; }
        public int ExperiencePoints { get; set; }
        public decimal Gold { get; set; }
        public decimal Gems { get; set; }
        public int Energy { get; set; }
        public int MaxEnergy { get; set; }
        public float? CorruptionLevel { get; set; }
    }

    public class ChangeNameRequestDto
    {
        [Required(ErrorMessage = "New name is required.")]
        [StringLength(16, MinimumLength = 3, ErrorMessage = "Character name must be between 3 and 16 characters.")]
        public string NewName { get; set; } = string.Empty;
    }

    // ============ PlayerProfile API Response ============
    public class PlayerProfileApiResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Data { get; set; }
    }

    // ============ Character Creation ============

    /// <summary>
    /// Request DTO to set a character name and class after initial registration.
    /// </summary>
    public class CreateCharacterRequestDto
    {
        [Required(ErrorMessage = "Character name is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Character name must be between 2 and 50 characters.")]
        public string CharacterName { get; set; } = string.Empty;

        /// <summary>Must be one of: Knight, Archer, Mage.</summary>
        [Required(ErrorMessage = "Selected class is required.")]
        [RegularExpression("^(Knight|Archer|Mage)$", ErrorMessage = "SelectedClass must be Knight, Archer, or Mage.")]
        public string SelectedClass { get; set; } = string.Empty;
    }

    /// <summary>
    /// Full character summary returned after creation or on character info requests.
    /// </summary>
    public class CharacterResponseDto
    {
        public int PlayerProfileId { get; set; }
        public int AccountId { get; set; }
        public string CharacterName { get; set; } = string.Empty;
        public string PlayerClass { get; set; } = string.Empty;
        public int Level { get; set; }
        public int ExperiencePoints { get; set; }
        public decimal Gold { get; set; }
        public decimal Gems { get; set; }
        public int Energy { get; set; }
        public int MaxEnergy { get; set; }
        public DateTime LastEnergyUpdateTime { get; set; }
        public float CorruptionLevel { get; set; }
        public DateTime CreatedAt { get; set; }
        public PlayerStatsResponseDto Stats { get; set; } = new();
    }

    // ============ Character Upgrade ============

    /// <summary>
    /// Request DTO to spend Skill Points on a specific attribute.
    /// Supported attribute names (case-insensitive):
    ///   MaxHp, Atk, Def, MoveSpeed, AttackSpeed, CritRate, CritDamage, DamageBonus
    /// </summary>
    public class UpgradeAttributeRequestDto
    {
        [Required(ErrorMessage = "AttributeName is required.")]
        [RegularExpression(
            "^(MaxHp|Atk|Def|MoveSpeed|AttackSpeed|CritRate|CritDamage|DamageBonus)$",
            ErrorMessage = "AttributeName must be one of: MaxHp, Atk, Def, MoveSpeed, AttackSpeed, CritRate, CritDamage, DamageBonus.")]
        public string AttributeName { get; set; } = string.Empty;

        /// <summary>Number of Skill Points to spend. Defaults to 1.</summary>
        [Range(1, 10, ErrorMessage = "Amount must be between 1 and 10.")]
        public int Amount { get; set; } = 1;
    }

    /// <summary>
    /// Response DTO after a successful attribute upgrade.
    /// </summary>
    public class UpgradeAttributeResponseDto
    {
        public string UpgradedAttribute { get; set; } = string.Empty;
        public int AmountSpent { get; set; }
        public int RemainingSkillPoints { get; set; }
        public PlayerStatsResponseDto Stats { get; set; } = new();
    }

    // ============ Player /me Response (Lightweight) ============
    public class PlayerMeResponseDto
    {
        // Basic Info
        public int PlayerProfileId { get; set; }
        public int AccountId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public string PlayerClass { get; set; } = "Knight";
        public int Level { get; set; }
        public int ExperiencePoints { get; set; }

        // Currency
        public decimal Gold { get; set; }
        public decimal Gems { get; set; }
        public int Energy { get; set; }
        public int MaxEnergy { get; set; }
        public DateTime LastEnergyUpdateTime { get; set; }

        // Position
        public string LastMapName { get; set; } = string.Empty;
        public double PositionX { get; set; }
        public double PositionY { get; set; }
        public float CorruptionLevel { get; set; }

        // Stats
        public PlayerStatsResponseDto? Stats { get; set; }

        // Timestamps
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastFreeGachaTime { get; set; }
    }
}
