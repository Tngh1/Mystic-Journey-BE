using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    // Initializes a new default instance of the Skill class.
    public class Skill
    {
        // Executes skill id operation.
        public int SkillId { get; set; }

        // Executes name operation.
        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        // Executes description operation.
        public string? Description { get; set; }

        // Executes image url operation.
        public string? ImageUrl { get; set; }

        // Supported skill types: Active, Passive, Buff, or Debuff; the type controls activation and effect presentation.
        public string Type { get; set; } = "Active";
        // Supported damage types: Physical, Magical, or TrueDamage; the value selects how skill damage is categorized and resolved.
        public string DamageType { get; set; } = "Physical";
        // Supported target types: SingleTarget, Area, Self, or Ally; the value determines who can receive the skill effect.
        public string TargetType { get; set; } = "SingleTarget";

        // Supported class requirements: Knight, Archer, Mage, or All; All allows every player class to use the skill or reward.
        public string ClassRequirement { get; set; } = "Knight";

        // Executes cooldown seconds operation.
        public int CooldownSeconds { get; set; } = 0;
        // Executes base damage operation.
        public double BaseDamage { get; set; } = 0.0;
        // Executes damage per level operation.
        public double DamagePerLevel { get; set; } = 0.0;
        // Executes damage growth percent operation.
        public double DamageGrowthPercent { get; set; } = 0.0;
        // Executes unlock level operation.
        public int UnlockLevel { get; set; } = 1;
        // Executes corruption cost operation.
        public float CorruptionCost { get; set; } = 0;

        // Executes is active operation.
        public bool IsActive { get; set; } = true;

        // Executes player skills operation.
        public ICollection<PlayerSkill> PlayerSkills { get; set; } = new List<PlayerSkill>();

    }
}
