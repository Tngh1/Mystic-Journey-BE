using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class DungeonConfig
    {
        public int DungeonConfigId { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        // Types: Normal, Elite, Boss
        public string Type { get; set; } = "Normal";

        public int LevelRequirement { get; set; } = 1;

        public int MaxMembers { get; set; } = 4;

        public int Difficulty { get; set; } = 1;

        /// <summary>Energy required to enter and claim reward from this dungeon.</summary>
        public int EnergyCost { get; set; } = 10;

        public int RecommendedPower { get; set; } = 0;

        public int? ChestId { get; set; }
        public Chest? Chest { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
