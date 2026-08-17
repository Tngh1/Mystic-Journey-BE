using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    // Initializes a new default instance of the MonsterSpawn class.
    public class MonsterSpawn
    {
        // Executes monster spawn id operation.
        public int MonsterSpawnId { get; set; }

        // Executes monster id operation.
        public int MonsterId { get; set; }
        // Executes monster operation.
        public Monster? Monster { get; set; }

        // Executes map name operation.
        [MaxLength(100)]
        public string MapName { get; set; } = string.Empty;

        // Executes region name operation.
        [MaxLength(100)]
        public string? RegionName { get; set; }

        // Executes location operation.
        public string? Location { get; set; }

        // Executes spawn count operation.
        public int SpawnCount { get; set; } = 1;

        // Executes respawn seconds operation.
        public int RespawnSeconds { get; set; } = 60;

        // Executes dungeon id operation.
        public int? DungeonId { get; set; }
        // Executes dungeon operation.
        public Dungeon? Dungeon { get; set; }

        // Executes is active operation.
        public bool IsActive { get; set; } = true;
    }
}
