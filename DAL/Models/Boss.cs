using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Boss
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        public int Level { get; set; } = 1;
        public int Health { get; set; } = 1000;
        public int Attack { get; set; } = 50;
        public int Defense { get; set; } = 20;

        public string? SpecialSkillDescription { get; set; }

        public bool IsFinalBoss { get; set; } = false;
    }
}