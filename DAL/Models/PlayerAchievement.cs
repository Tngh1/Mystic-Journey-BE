namespace DAL.Models
{
    // Initializes a new default instance of the PlayerAchievement class.
    public class PlayerAchievement
    {
        // Executes player achievement id operation.
        public int PlayerAchievementId { get; set; }

        // Executes player profile id operation.
        public int PlayerProfileId { get; set; }
        // Executes player profile operation.
        public PlayerProfile? PlayerProfile { get; set; }

        // Executes achievement id operation.
        public int AchievementId { get; set; }
        // Executes achievement operation.
        public Achievement? Achievement { get; set; }

        // Executes progress operation.
        public int Progress { get; set; } = 0;

        // Executes is completed operation.
        public bool IsCompleted { get; set; } = false;
        // Executes completed at operation.
        public DateTime? CompletedAt { get; set; }

        // Executes unlocked at operation.
        public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;
    }
}
