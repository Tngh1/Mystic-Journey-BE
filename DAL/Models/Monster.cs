using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Monster
    {
        public Guid Id { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        public MonsterType Type { get; set; } = MonsterType.Normal;

        public int Level { get; set; } = 1;
        public int Health { get; set; } = 100;
        public int Attack { get; set; } = 10;
        public int Defense { get; set; } = 5;
        public int ExperienceReward { get; set; } = 10;
        public decimal GoldReward { get; set; } = 5;

        public bool IsActive { get; set; } = true;

        public enum MonsterType
        {
            Normal = 0,
            Elite = 1,
            Boss = 2
        }
    }
}