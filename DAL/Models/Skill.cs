using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Skill
    {
        public Guid Id { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public SkillCategory Type { get; set; } = SkillCategory.Active;
        public SkillDamageType DamageType { get; set; } = SkillDamageType.Physical;
        public SkillTargetType TargetType { get; set; } = SkillTargetType.SingleTarget;

        public PlayerProfile.CharacterClass ClassRequirement { get; set; } = PlayerProfile.CharacterClass.Knight;

        public int ManaCost { get; set; } = 0;
        public int CooldownSeconds { get; set; } = 0;
        public int BaseDamage { get; set; } = 0;
        public int UnlockLevel { get; set; } = 1;

        public bool IsActive { get; set; } = true;

        public ICollection<PlayerSkill> PlayerSkills { get; set; } = new List<PlayerSkill>();

        public enum SkillCategory
        {
            Active = 0,
            Passive = 1,
            Buff = 2,
            Debuff = 3
        }

        public enum SkillDamageType
        {
            Physical = 0,
            Magical = 1,
            TrueDamage = 2
        }

        public enum SkillTargetType
        {
            SingleTarget = 0,
            Area = 1,
            Self = 2,
            Ally = 3
        }
    }
}