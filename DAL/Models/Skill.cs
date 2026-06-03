using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Skill
    {
        public int SkillId { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        // Types: Active, Passive, Buff, Debuff
        public string Type { get; set; } = "Active";
        // DamageTypes: Physical, Magical, TrueDamage
        public string DamageType { get; set; } = "Physical";
        // TargetTypes: SingleTarget, Area, Self, Ally
        public string TargetType { get; set; } = "SingleTarget";

        // ClassRequirements: Knight, Archer, Mage
        public string ClassRequirement { get; set; } = "Knight";

        public int CooldownSeconds { get; set; } = 0;
        public int BaseDamage { get; set; } = 0;
        public int UnlockLevel { get; set; } = 1;

        public bool IsActive { get; set; } = true;

        public ICollection<PlayerSkill> PlayerSkills { get; set; } = new List<PlayerSkill>();

    }
}