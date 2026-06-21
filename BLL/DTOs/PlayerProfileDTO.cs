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
    }

    // ============ PlayerProfile API Response ============
    public class PlayerProfileApiResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Data { get; set; }
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

        // Position
        public string LastMapName { get; set; } = string.Empty;
        public double PositionX { get; set; }
        public double PositionY { get; set; }

        // Stats
        public PlayerStatsResponseDto? Stats { get; set; }

        // Timestamps
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    // ============ Player Me Inventory ============
    public class PlayerMeInventoryResponseDto
    {
        public int PlayerProfileId { get; set; }
        public List<InventoryItemResponseDto> Items { get; set; } = new();
        public int TotalCount { get; set; }
    }

    // ============ Player Me Skills ============
    public class PlayerMeSkillsResponseDto
    {
        public int PlayerProfileId { get; set; }
        public List<PlayerSkillResponseDto> Skills { get; set; } = new();
        public int TotalCount { get; set; }
    }

    // ============ Player Me Quests ============
    public class PlayerMeQuestsResponseDto
    {
        public int PlayerProfileId { get; set; }
        public List<PlayerQuestResponseDto> Quests { get; set; } = new();
        public int TotalCount { get; set; }
    }

    // ============ Player Me Mails ============
    public class PlayerMeMailsResponseDto
    {
        public int PlayerProfileId { get; set; }
        public List<MailResponseDto> Mails { get; set; } = new();
        public int TotalCount { get; set; }
        public int UnreadCount { get; set; }
    }

    // ============ Player Me Achievements ============
    public class PlayerMeAchievementsResponseDto
    {
        public int PlayerProfileId { get; set; }
        public List<PlayerAchievementResponseDto> Achievements { get; set; } = new();
        public int TotalCount { get; set; }
        public int CompletedCount { get; set; }
    }
}
