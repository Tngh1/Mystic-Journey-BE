namespace DAL.Models
{
    public class DungeonRunMember
    {
        public int Id { get; set; }

        public int DungeonRunId { get; set; }
        public DungeonRun? DungeonRun { get; set; }

        public int PlayerProfileId { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }

        public bool IsLeader { get; set; } = false;
        public bool IsReady { get; set; } = false;

        public int CurrentStage { get; set; } = 1;
        public bool IsCompleted { get; set; } = false;
        public bool IsDefeated { get; set; } = false;

        // Reward tracking
        public int RewardExperience { get; set; } = 0;
        public decimal RewardGold { get; set; } = 0;
        public bool IsRewardClaimed { get; set; } = false;

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public DateTime? DefeatedAt { get; set; }
    }
}
