using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class DungeonRun
    {
        public Guid Id { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        // Types: Normal, Elite, Boss
        public string Type { get; set; } = "Normal";

        public int LevelRequirement { get; set; } = 1;
        public int MaxMembers { get; set; } = 4;
        public int Difficulty { get; set; } = 1;

        public int TotalStages { get; set; } = 5;
        public int CurrentStage { get; set; } = 1;

        public bool IsCompleted { get; set; } = false;
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }

        public ICollection<DungeonRunMember> Members { get; set; } = new List<DungeonRunMember>();
    }
}
