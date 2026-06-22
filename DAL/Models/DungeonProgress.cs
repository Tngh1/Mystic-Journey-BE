namespace DAL.Models
{
    /// <summary>
    /// Stores in-dungeon combat progress for a DungeonSession.
    /// Kept in a separate table so the session row stays lean and
    /// this can be updated frequently without touching session metadata.
    /// </summary>
    public class DungeonProgress
    {
        public int DungeonProgressId { get; set; }

        public int DungeonSessionId { get; set; }
        public DungeonSession? DungeonSession { get; set; }

        /// <summary>Total number of regular monsters killed in this run.</summary>
        public int MonstersKilled { get; set; } = 0;

        /// <summary>True once the final boss of the dungeon is defeated.</summary>
        public bool BossKilled { get; set; } = false;

        /// <summary>Completion percentage (0–100). Drives the UI progress bar.</summary>
        public int CompletionPercentage { get; set; } = 0;

        /// <summary>
        /// JSON blob for future extensibility: floor checkpoints, traps triggered, buffs active, etc.
        /// Stored as a raw string; deserialized in the service layer when needed.
        /// Example: {"FloorsCleared":3,"TrapsTriggered":1}
        /// </summary>
        public string? ExtraData { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
