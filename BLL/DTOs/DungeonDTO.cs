using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // ============ Dungeon ============
    public class DungeonConfigResponseDto
    {
        public int DungeonConfigId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string Type { get; set; } = "Normal";
        public int LevelRequirement { get; set; }
        public int MaxMembers { get; set; }
        public int Difficulty { get; set; }
        public int RecommendedPower { get; set; }
        public int? ChestId { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateDungeonConfigRequestDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(150, ErrorMessage = "Name must not exceed 150 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string Type { get; set; } = "Normal";
        public int LevelRequirement { get; set; } = 1;
        public int MaxMembers { get; set; } = 4;
        public int Difficulty { get; set; } = 1;
        public int RecommendedPower { get; set; } = 0;
        public int? ChestId { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateDungeonConfigRequestDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string Type { get; set; } = "Normal";
        public int LevelRequirement { get; set; } = 1;
        public int MaxMembers { get; set; } = 4;
        public int Difficulty { get; set; } = 1;
        public int RecommendedPower { get; set; } = 0;
        public int? ChestId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
