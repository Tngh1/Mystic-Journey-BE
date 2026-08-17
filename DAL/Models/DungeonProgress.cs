namespace DAL.Models
{
    // Initializes a new default instance of the DungeonProgress class.
    public class DungeonProgress
    {
        // Executes dungeon progress id operation.
        public int DungeonProgressId { get; set; }

        // Executes dungeon session id operation.
        public int DungeonSessionId { get; set; }
        // Executes dungeon session operation.
        public DungeonSession? DungeonSession { get; set; }

        // Executes monsters killed operation.
        public int MonstersKilled { get; set; } = 0;

        // Executes boss spawned operation.
        public bool BossSpawned { get; set; } = false;

        // Executes boss killed operation.
        public bool BossKilled { get; set; } = false;

        // Executes elapsed time operation.
        public int ElapsedTime { get; set; } = 0;

        // Executes completion percentage operation.
        public int CompletionPercentage { get; set; } = 0;

        // Executes extra data operation.
        public string? ExtraData { get; set; }

        // Executes created at operation.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // Executes updated at operation.
        public DateTime? UpdatedAt { get; set; }
    }
}
