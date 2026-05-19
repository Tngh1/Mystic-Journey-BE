namespace DAL.Models
{
    public class PlayerAchievement
    {
        public Guid Id { get; set; }

        public Guid PlayerProfileId { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }

        public Guid AchievementId { get; set; }
        public Achievement? Achievement { get; set; }

        public int Progress { get; set; } = 0;

        public bool IsCompleted { get; set; } = false;
        public DateTime? CompletedAt { get; set; }

        public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;
    }
}
