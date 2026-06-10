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
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsBanned { get; set; }
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
        public float MoveSpeed { get; set; }
        public float AttackSpeed { get; set; }
        public float CritRate { get; set; }
        public float CritDamage { get; set; }
        public float DamageBonus { get; set; }
        public int SkillPoints { get; set; }
        public int TotalWins { get; set; }
        public int TotalLosses { get; set; }
        public int TotalKills { get; set; }
        public int TotalDeaths { get; set; }
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
        public bool? IsBanned { get; set; }
    }

    // ============ PlayerProfile API Response ============
    public class PlayerProfileApiResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Data { get; set; }
    }
}
