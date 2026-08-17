using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // Initializes a new default instance of the SkillResponseDto class.
    public class SkillResponseDto
    {
        // Executes skill id operation.
        public int SkillId { get; set; }
        // Executes name operation.
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
        public int CooldownSeconds { get; set; }
        // Executes base damage operation.
        public double BaseDamage { get; set; }
        // Executes damage per level operation.
        public double DamagePerLevel { get; set; }
        // Executes damage growth percent operation.
        public double DamageGrowthPercent { get; set; }
        // Executes unlock level operation.
        public int UnlockLevel { get; set; }
        // Executes corruption cost operation.
        public float CorruptionCost { get; set; }
        // Executes is active operation.
        public bool IsActive { get; set; }
    }

    // Executes create skill request dto operation.
    public class CreateSkillRequestDto
    {
        // Executes name operation.
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(150, ErrorMessage = "Name must not exceed 150 characters.")]
        public string Name { get; set; } = string.Empty;

        // Executes description operation.
        [StringLength(500)]
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
        [Range(0, int.MaxValue)]
        public int CooldownSeconds { get; set; }

        // Executes base damage operation.
        [Range(0, double.MaxValue)]
        public double BaseDamage { get; set; }
        // Executes damage per level operation.
        [Range(0, double.MaxValue)]
        public double DamagePerLevel { get; set; }
        // Executes damage growth percent operation.
        [Range(0, 1000)]
        public double DamageGrowthPercent { get; set; }

        // Executes unlock level operation.
        [Range(1, 100)]
        public int UnlockLevel { get; set; } = 1;
        // Executes corruption cost operation.
        public float CorruptionCost { get; set; } = 0;

        // Executes is active operation.
        public bool IsActive { get; set; } = true;
    }

    // Executes update skill request dto operation.
    public class UpdateSkillRequestDto
    {
        // Executes name operation.
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        // Executes description operation.
        [StringLength(500)]
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
        [Range(0, int.MaxValue)]
        public int CooldownSeconds { get; set; }

        // Executes base damage operation.
        [Range(0, double.MaxValue)]
        public double BaseDamage { get; set; }
        // Executes damage per level operation.
        [Range(0, double.MaxValue)]
        public double DamagePerLevel { get; set; }
        // Executes damage growth percent operation.
        [Range(0, 1000)]
        public double DamageGrowthPercent { get; set; }

        // Executes unlock level operation.
        [Range(1, 100)]
        public int UnlockLevel { get; set; } = 1;
        // Executes corruption cost operation.
        public float CorruptionCost { get; set; } = 0;

        // Executes is active operation.
        public bool IsActive { get; set; } = true;
    }

    // Executes player skill response dto operation.
    public class PlayerSkillResponseDto
    {
        // Executes player skill id operation.
        public int PlayerSkillId { get; set; }
        // Executes player profile id operation.
        public int PlayerProfileId { get; set; }
        // Executes skill id operation.
        public int SkillId { get; set; }
        // Executes skill name operation.
        public string SkillName { get; set; } = string.Empty;
        // Executes skill description operation.
        public string? SkillDescription { get; set; }
        // Supported skill types: Active, Passive, Buff, or Debuff; the type controls activation and effect presentation.
        public string SkillType { get; set; } = string.Empty;
        // Supported damage types: Physical, Magical, or TrueDamage; the value selects how skill damage is categorized and resolved.
        public string DamageType { get; set; } = string.Empty;
        // Supported target types: SingleTarget, Area, Self, or Ally; the value determines who can receive the skill effect.
        public string TargetType { get; set; } = string.Empty;
        // Executes level operation.
        public int Level { get; set; }
        // Executes experience operation.
        public int Experience { get; set; }
        // Executes is equipped operation.
        public bool IsEquipped { get; set; }
        // Executes equipped slot operation.
        public int? EquippedSlot { get; set; }
        // Executes effective damage operation.
        public double EffectiveDamage { get; set; }
        // Executes cooldown seconds operation.
        public int CooldownSeconds { get; set; }
        // Executes base damage operation.
        public double BaseDamage { get; set; }
        // Executes unlock level operation.
        public int UnlockLevel { get; set; }
        // Executes corruption cost operation.
        public float CorruptionCost { get; set; }
        // Executes unlocked at operation.
        public DateTime UnlockedAt { get; set; }
        // Executes next available time operation.
        public DateTime? NextAvailableTime { get; set; }
    }

    // Executes upgrade player skill request dto operation.
    public class UpgradePlayerSkillRequestDto
    {
        // Executes player skill id operation.
        [Required]
        public int PlayerSkillId { get; set; }
    }

    // Executes equip skill request dto operation.
    public class EquipSkillRequestDto
    {
        // Executes player skill id operation.
        [Required]
        public int PlayerSkillId { get; set; }

        // Executes is equipped operation.
        public bool IsEquipped { get; set; }

        // Executes slot index operation.
        public int? SlotIndex { get; set; }
    }

    // Executes dismantle player skill request dto operation.
    public class DismantlePlayerSkillRequestDto
    {
        // Executes player skill id operation.
        [Required]
        public int PlayerSkillId { get; set; }

        // Executes target player skill id operation.
        public int? TargetPlayerSkillId { get; set; }
    }

    // Executes player me skills response dto operation.
    public class PlayerMeSkillsResponseDto
    {
        // Executes player profile id operation.
        public int PlayerProfileId { get; set; }
        // Executes skills operation.
        public List<PlayerSkillResponseDto> Skills { get; set; } = new();
        // Executes total count operation.
        public int TotalCount { get; set; }
    }
}
