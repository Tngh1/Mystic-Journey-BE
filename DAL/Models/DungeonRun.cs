namespace DAL.Models
{
    public class DungeonRun
    {
        public int DungeonRunId { get; set; }

        public int DungeonConfigId { get; set; }
        public DungeonConfig? DungeonConfig { get; set; }

        public int CurrentStage { get; set; } = 1;

        public bool IsCompleted { get; set; } = false;
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }

        public ICollection<DungeonRunMember> Members { get; set; } = new List<DungeonRunMember>();
    }
}
