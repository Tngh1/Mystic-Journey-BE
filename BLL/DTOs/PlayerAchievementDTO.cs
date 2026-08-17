namespace BLL.DTOs
{
    // Initializes a new default instance of the PlayerAchievementResponseDto class.
    public class PlayerAchievementResponseDto
    {
        // Executes player achievement id operation.
        public int PlayerAchievementId { get; set; }
        // Executes player profile id operation.
        public int PlayerProfileId { get; set; }
        // Executes achievement id operation.
        public int AchievementId { get; set; }
        // Executes achievement name operation.
        public string AchievementName { get; set; } = string.Empty;
        // Executes achievement description operation.
        public string? AchievementDescription { get; set; }
        // Executes achievement type operation.
        public string AchievementType { get; set; } = string.Empty;
        // Executes icon url operation.
        public string? IconUrl { get; set; }
        // Executes progress operation.
        public int Progress { get; set; }
        // Executes required value operation.
        public int RequiredValue { get; set; }
        // Executes is completed operation.
        public bool IsCompleted { get; set; }
        // Executes completed at operation.
        public DateTime? CompletedAt { get; set; }
        // Executes unlocked at operation.
        public DateTime UnlockedAt { get; set; }
        // Executes reward item id operation.
        public int? RewardItemId { get; set; }
        // Executes reward item name operation.
        public string? RewardItemName { get; set; }
        // Executes reward quantity operation.
        public int RewardQuantity { get; set; }
        // Executes reward gold operation.
        public decimal RewardGold { get; set; }
        // Executes reward gem operation.
        public int RewardGem { get; set; }
    }

    // Executes player me achievements response dto operation.
    public class PlayerMeAchievementsResponseDto
    {
        // Executes player profile id operation.
        public int PlayerProfileId { get; set; }
        // Executes achievements operation.
        public List<PlayerAchievementResponseDto> Achievements { get; set; } = new();
        // Executes total count operation.
        public int TotalCount { get; set; }
        // Executes completed count operation.
        public int CompletedCount { get; set; }
    }
}
