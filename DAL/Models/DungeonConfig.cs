using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    // Initializes a new default instance of the DungeonConfig class.
    public class DungeonConfig
    {
        // Executes dungeon config id operation.
        public int DungeonConfigId { get; set; }

        // Executes name operation.
        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        // Executes description operation.
        [MaxLength(500)]
        public string? Description { get; set; }

        // Dungeon type is a free-form category with Normal as the current default; the backend does not enforce a closed allowlist.
        public string Type { get; set; } = "Normal";

        // Executes level requirement operation.
        public int LevelRequirement { get; set; } = 1;

        // Executes max members operation.
        public int MaxMembers { get; set; } = 4;

        // Executes difficulty operation.
        public int Difficulty { get; set; } = 1;

        // Executes energy cost operation.
        public int EnergyCost { get; set; } = 10;

        // Executes recommended power operation.
        public int RecommendedPower { get; set; } = 0;

        // Executes chest id operation.
        public int? ChestId { get; set; }
        // Executes chest operation.
        public Chest? Chest { get; set; }

        // Executes is active operation.
        public bool IsActive { get; set; } = true;
    }
}
