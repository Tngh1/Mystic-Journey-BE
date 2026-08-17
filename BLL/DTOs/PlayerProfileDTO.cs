using System;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // Initializes a new default instance of the PlayerProfileResponseDto class.
    public class PlayerProfileResponseDto
    {
        // Executes player profile id operation.
        public int PlayerProfileId { get; set; }
        // Executes account id operation.
        public int AccountId { get; set; }
        // Executes account email operation.
        public string? AccountEmail { get; set; }
        // Executes display name operation.
        public string DisplayName { get; set; } = string.Empty;
        // Executes avatar url operation.
        public string? AvatarUrl { get; set; }
        // Executes player class operation.
        public string PlayerClass { get; set; } = string.Empty;
        // Executes level operation.
        public int Level { get; set; }
        // Executes experience points operation.
        public int ExperiencePoints { get; set; }
        // Executes available stat points operation.
        public int AvailableStatPoints { get; set; }
        // Executes gold operation.
        public decimal Gold { get; set; }
        // Executes gems operation.
        public decimal Gems { get; set; }
        // Executes energy operation.
        public int Energy { get; set; }
        // Executes max energy operation.
        public int MaxEnergy { get; set; }
        // Executes last energy update time operation.
        public DateTime LastEnergyUpdateTime { get; set; }
        // Executes corruption level operation.
        public float CorruptionLevel { get; set; }
        // Executes created at operation.
        public DateTime CreatedAt { get; set; }
        // Executes updated at operation.
        public DateTime? UpdatedAt { get; set; }
        // Executes last free gacha time operation.
        public DateTime? LastFreeGachaTime { get; set; }
        // Executes has changed name operation.
        public bool HasChangedName { get; set; }

        // Executes is banned operation.
        public bool IsBanned { get; set; }
    }

    // Initializes a new default instance of the PlayerProfileResponseDto class.
    public class PlayerProfileDetailResponseDto : PlayerProfileResponseDto
    {
        // Executes stats operation.
        public PlayerStatsResponseDto? Stats { get; set; }
    }

    // Executes player stats response dto operation.
    public class PlayerStatsResponseDto
    {
        // Executes current hp operation.
        public int CurrentHp { get; set; }
        // Executes max hp operation.
        public int MaxHp { get; set; }
        // Executes atk operation.
        public int Atk { get; set; }
        // Executes def operation.
        public int Def { get; set; }
        // Executes move speed operation.
        public int MoveSpeed { get; set; }
        // Executes attack speed operation.
        public int AttackSpeed { get; set; }
        // Executes crit rate operation.
        public int CritRate { get; set; }
        // Executes crit damage operation.
        public int CritDamage { get; set; }
        // Executes damage bonus operation.
        public int DamageBonus { get; set; }
        // Executes skill points operation.
        public int SkillPoints { get; set; }
        // Executes total wins operation.
        public int TotalWins { get; set; }
        // Executes total losses operation.
        public int TotalLosses { get; set; }
        // Executes total kills operation.
        public int TotalKills { get; set; }
        // Executes total deaths operation.
        public int TotalDeaths { get; set; }
        // Executes active buffs operation.
        public List<PlayerBuffDTO> ActiveBuffs { get; set; } = new List<PlayerBuffDTO>();
    }

    // Executes update player profile request dto operation.
    public class UpdatePlayerProfileRequestDto
    {
        // Executes display name operation.
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Display name must be between 3 and 100 characters.")]
        public string? DisplayName { get; set; }

        // Executes avatar url operation.
        public string? AvatarUrl { get; set; }

        [RegularExpression("^(Knight|Archer|Mage)$", ErrorMessage = "PlayerClass must be Knight, Archer, or Mage.")]
        // Executes player class operation.
        public string? PlayerClass { get; set; }

        // Executes level operation.
        [Range(1, 100, ErrorMessage = "Level must be between 1 and 100.")]
        public int? Level { get; set; }

        // Executes experience points operation.
        [Range(0, int.MaxValue, ErrorMessage = "ExperiencePoints cannot be negative.")]
        public int? ExperiencePoints { get; set; }

        // Executes gold operation.
        [Range(0, double.MaxValue, ErrorMessage = "Gold cannot be negative.")]
        public decimal? Gold { get; set; }

        // Executes gems operation.
        [Range(0, double.MaxValue, ErrorMessage = "Gems cannot be negative.")]
        public decimal? Gems { get; set; }

        // Executes energy operation.
        [Range(0, int.MaxValue, ErrorMessage = "Energy cannot be negative.")]
        public int? Energy { get; set; }

        // Executes max energy operation.
        [Range(1, int.MaxValue, ErrorMessage = "MaxEnergy must be at least 1.")]
        public int? MaxEnergy { get; set; }

        // Executes corruption level operation.
        [Range(0, 100, ErrorMessage = "CorruptionLevel must be between 0 and 100.")]
        public float? CorruptionLevel { get; set; }
    }

    // Executes change name request dto operation.
    public class ChangeNameRequestDto
    {
        // Executes new name operation.
        [Required(ErrorMessage = "New name is required.")]
        [StringLength(16, MinimumLength = 3, ErrorMessage = "Character name must be between 3 and 16 characters.")]
        public string NewName { get; set; } = string.Empty;
    }


    // Executes create character request dto operation.
    public class CreateCharacterRequestDto
    {
        // Executes character name operation.
        [Required(ErrorMessage = "Character name is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Character name must be between 2 and 50 characters.")]
        public string CharacterName { get; set; } = string.Empty;

        // Executes selected class operation.
        [Required(ErrorMessage = "Selected class is required.")]
        [RegularExpression("^(Knight|Archer|Mage)$", ErrorMessage = "SelectedClass must be Knight, Archer, or Mage.")]
        public string SelectedClass { get; set; } = string.Empty;
    }

    // Executes character response dto operation.
    public class CharacterResponseDto
    {
        // Executes player profile id operation.
        public int PlayerProfileId { get; set; }
        // Executes account id operation.
        public int AccountId { get; set; }
        // Executes character name operation.
        public string CharacterName { get; set; } = string.Empty;
        // Executes player class operation.
        public string PlayerClass { get; set; } = string.Empty;
        // Executes level operation.
        public int Level { get; set; }
        // Executes experience points operation.
        public int ExperiencePoints { get; set; }
        // Executes available stat points operation.
        public int AvailableStatPoints { get; set; }
        // Executes gold operation.
        public decimal Gold { get; set; }
        // Executes gems operation.
        public decimal Gems { get; set; }
        // Executes energy operation.
        public int Energy { get; set; }
        // Executes max energy operation.
        public int MaxEnergy { get; set; }
        // Executes last energy update time operation.
        public DateTime LastEnergyUpdateTime { get; set; }
        // Executes corruption level operation.
        public float CorruptionLevel { get; set; }
        // Executes created at operation.
        public DateTime CreatedAt { get; set; }
        // Executes stats operation.
        public PlayerStatsResponseDto Stats { get; set; } = new();
    }


    // Executes upgrade attribute request dto operation.
    public class UpgradeAttributeRequestDto
    {
        [Required(ErrorMessage = "AttributeName is required.")]
        [RegularExpression(
            "^(MaxHp|Atk|Def|MoveSpeed|AttackSpeed|CritRate|CritDamage|DamageBonus)$",
            ErrorMessage = "AttributeName must be one of: MaxHp, Atk, Def, MoveSpeed, AttackSpeed, CritRate, CritDamage, DamageBonus.")]
        // Executes attribute name operation.
        public string AttributeName { get; set; } = string.Empty;

        // Executes amount operation.
        [Range(1, 10, ErrorMessage = "Amount must be between 1 and 10.")]
        public int Amount { get; set; } = 1;
    }

    // Executes upgrade attribute response dto operation.
    public class UpgradeAttributeResponseDto
    {
        // Executes upgraded attribute operation.
        public string UpgradedAttribute { get; set; } = string.Empty;
        // Executes amount spent operation.
        public int AmountSpent { get; set; }
        // Executes remaining skill points operation.
        public int RemainingSkillPoints { get; set; }
        // Executes stats operation.
        public PlayerStatsResponseDto Stats { get; set; } = new();
    }

    // Executes allocate stat request dto operation.
    public class AllocateStatRequestDto
    {
        // Executes stat name operation.
        [Required(ErrorMessage = "StatName is required.")]
        public string StatName { get; set; } = string.Empty;
    }
}
