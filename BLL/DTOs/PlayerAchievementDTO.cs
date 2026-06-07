namespace BLL.DTOs
{
    // ============ PlayerAchievement ============
    public class PlayerAchievementResponseDto
    {
        public int PlayerAchievementId { get; set; }
        public int PlayerProfileId { get; set; }
        public int AchievementId { get; set; }
        public string AchievementName { get; set; } = string.Empty;
        public string? AchievementDescription { get; set; }
        public string AchievementType { get; set; } = string.Empty;
        public string? IconUrl { get; set; }
        public int Progress { get; set; }
        public int RequiredValue { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime UnlockedAt { get; set; }
        public int? RewardItemId { get; set; }
        public string? RewardItemName { get; set; }
        public int RewardQuantity { get; set; }
        public decimal RewardGold { get; set; }
        public int RewardGem { get; set; }
    }

    public class ClaimAchievementRewardRequestDto
    {
        public int PlayerAchievementId { get; set; }
    }
}
