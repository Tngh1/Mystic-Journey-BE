using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    // Initializes a new default instance of the Monster class.
    public class Monster
    {
        // Executes monster id operation.
        public int MonsterId { get; set; }

        // Executes name operation.
        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        // Supported monster types: Normal, Elite, or Boss; the type controls presentation and encounter behavior.
        public string Type { get; set; } = "Normal";

        // Executes description operation.
        public string Description { get; set; } = string.Empty;

        // Executes level operation.
        public int Level { get; set; } = 1;

        // Executes max hp operation.
        public int MaxHp { get; set; }

        // Executes atk operation.
        public int Atk { get; set; }

        // Executes def operation.
        public int Def { get; set; }

        // Executes move speed operation.
        public int MoveSpeed { get; set; }

        // Executes attack speed operation.
        public int AttackSpeed { get; set; }

        // Executes crit rate operation.
        public int CritRate { get; set; }

        // Executes crit damage operation.
        public int CritDamage { get; set; }

        // Executes experience reward operation.
        public int ExperienceReward { get; set; } = 10;
        // Executes gold reward operation.
        public decimal GoldReward { get; set; } = 5;
        // Executes gem reward operation.
        public decimal GemReward { get; set; } = 0;

        // Executes image url operation.
        public string? ImageUrl { get; set; }
        // Executes is active operation.
        public bool IsActive { get; set; } = true;

        // Executes monster drops operation.
        public ICollection<MonsterDrop> MonsterDrops { get; set; } = new List<MonsterDrop>();

        // Executes created at operation.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
