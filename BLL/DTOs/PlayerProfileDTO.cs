using System;
using System.Text.Json.Serialization;
using static DAL.Models.PlayerProfile;

namespace BLL.DTOs
{
    public class PlayerProfileResponseDto
    {
        public Guid ProfileId { get; set; }
        public Guid AccountId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public int Level { get; set; }
        public int ExperiencePoints { get; set; }
        public int ExperienceToNextLevel { get; set; }
        public decimal Gold { get; set; }
        public decimal Gems { get; set; }
        public int Energy { get; set; }
        public int MaxEnergy { get; set; } = 100;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreatePlayerProfileRequestDto
    {
        public string DisplayName { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public int Class { get; set; } = (int)CharacterClass.Knight;
    }

    public class UpdatePlayerProfileRequestDto
    {
        public string? DisplayName { get; set; }
        public string? AvatarUrl { get; set; }
        public int? Class { get; set; }
    }

    public class PlayerStatsResponseDto
    {
        public Guid StatsId { get; set; }
        public int Health { get; set; }
        public int Mana { get; set; }
        public int Strength { get; set; }
        public int Defense { get; set; }
        public int Agility { get; set; }
        public int Intelligence { get; set; }
        public int Endurance { get; set; }
        public int Luck { get; set; }
        public int CriticalRate { get; set; }
        public int CriticalDamage { get; set; }
        public int ArmorPenetration { get; set; }
        public int SkillPoints { get; set; }
        public int TotalWins { get; set; }
        public int TotalLosses { get; set; }
        public int TotalKills { get; set; }
        public int TotalDeaths { get; set; }
    }

    public class PlayerProfileDetailResponseDto : PlayerProfileResponseDto
    {
        public PlayerStatsResponseDto? Stats { get; set; }
    }

    public class CurrencyUpdateDto
    {
        public int CurrencyType { get; set; }
        public decimal Amount { get; set; }
    }

    public class PlayerCurrencyResponseDto
    {
        public decimal Gold { get; set; }
        public decimal Gems { get; set; }
        public int Energy { get; set; }
        public int MaxEnergy { get; set; }
    }

    public class PlayerProfileApiResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PlayerProfileResponseDto? Data { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PlayerProfileDetailResponseDto? Detail { get; set; }
    }

    public class PlayerCurrencyApiResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PlayerCurrencyResponseDto? Data { get; set; }
    }
}
