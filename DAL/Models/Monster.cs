using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Monster
    {
        public int MonsterId { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        // Types: Normal, Elite, Boss
        public string Type { get; set; } = "Normal";

        public string Description { get; set; } = string.Empty;

        public int Level { get; set; } = 1;

        public int MaxHp { get; set; }

        public int Atk { get; set; }

        public int Def { get; set; }

        public int MoveSpeed { get; set; }

        public int AttackSpeed { get; set; }

        public int CritRate { get; set; }

        public int CritDamage { get; set; }

        public int ExperienceReward { get; set; } = 10;
        public decimal GoldReward { get; set; } = 5;
        public decimal GemReward { get; set; } = 0;

        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<MonsterDrop> MonsterDrops { get; set; } = new List<MonsterDrop>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}