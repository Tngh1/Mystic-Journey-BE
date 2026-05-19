namespace DAL.Models
{
    public class DungeonRunMember
    {
        public Guid Id { get; set; }

        public Guid DungeonRunId { get; set; }
        public DungeonRun? DungeonRun { get; set; }

        public Guid PlayerProfileId { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }

        public bool IsLeader { get; set; } = false;
        public bool IsReady { get; set; } = false;

        public int CurrentStage { get; set; } = 1;
        public bool IsCompleted { get; set; } = false;
        public bool IsDefeated { get; set; } = false;

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public DateTime? DefeatedAt { get; set; }
    }
}
