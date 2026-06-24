using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class MonsterSpawn
    {
        public int MonsterSpawnId { get; set; }

        public int MonsterId { get; set; }
        public Monster? Monster { get; set; }

        [MaxLength(100)]
        public string MapName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? RegionName { get; set; }

        // Optional precise location data (could be coordinates or a named point)
        public string? Location { get; set; }

        // How many of this monster should spawn at this point
        public int SpawnCount { get; set; } = 1;

        // Respawn delay in seconds after all spawned instances are cleared
        public int RespawnSeconds { get; set; } = 60;

        // Whether this spawn belongs to a dungeon; if so, dungeon behavior (repeatable) is considered
        public int? DungeonId { get; set; }
        public Dungeon? Dungeon { get; set; }

        // If the monster is a boss tied to a quest, spawn system should check quest status
        public bool IsActive { get; set; } = true;
    }
}
