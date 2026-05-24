using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Monster
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        // Types: Normal, Elite, Boss
        public string Type { get; set; } = "Normal";

        public int Level { get; set; } = 1;
        public int Health { get; set; } = 100;
        public int Attack { get; set; } = 10;
        public int Defense { get; set; } = 5;
        public int ExperienceReward { get; set; } = 10;
        public decimal GoldReward { get; set; } = 5;

        public bool IsActive { get; set; } = true;
    }
}