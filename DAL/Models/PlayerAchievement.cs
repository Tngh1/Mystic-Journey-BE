namespace DAL.Models
{
    public class PlayerAchievement
    {
        public int Id { get; set; }

        public int PlayerProfileId { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }

        public int AchievementId { get; set; }
        public Achievement? Achievement { get; set; }

        public int Progress { get; set; } = 0;

        public bool IsCompleted { get; set; } = false;
        public DateTime? CompletedAt { get; set; }

        public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;
    }
}
