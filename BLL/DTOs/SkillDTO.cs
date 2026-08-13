using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // ============ Skill ============
    public class SkillResponseDto
    {
        public int SkillId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Type { get; set; } = "Active";
        public string DamageType { get; set; } = "Physical";
        public string TargetType { get; set; } = "SingleTarget";
        public string ClassRequirement { get; set; } = "Knight";
        public int CooldownSeconds { get; set; }
        public double BaseDamage { get; set; }
        public double DamagePerLevel { get; set; }
        public double DamageGrowthPercent { get; set; }
        public int UnlockLevel { get; set; }
        public float CorruptionCost { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateSkillRequestDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(150, ErrorMessage = "Name must not exceed 150 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public string Type { get; set; } = "Active";
        public string DamageType { get; set; } = "Physical";
        public string TargetType { get; set; } = "SingleTarget";
        public string ClassRequirement { get; set; } = "Knight";

        [Range(0, int.MaxValue)]
        public int CooldownSeconds { get; set; }

        [Range(0, double.MaxValue)]
        public double BaseDamage { get; set; }
        [Range(0, double.MaxValue)]
        public double DamagePerLevel { get; set; }
        [Range(0, 1000)]
        public double DamageGrowthPercent { get; set; }

        [Range(1, 100)]
        public int UnlockLevel { get; set; } = 1;
        public float CorruptionCost { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }

    public class UpdateSkillRequestDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public string Type { get; set; } = "Active";
        public string DamageType { get; set; } = "Physical";
        public string TargetType { get; set; } = "SingleTarget";
        public string ClassRequirement { get; set; } = "Knight";

        [Range(0, int.MaxValue)]
        public int CooldownSeconds { get; set; }

        [Range(0, double.MaxValue)]
        public double BaseDamage { get; set; }
        [Range(0, double.MaxValue)]
        public double DamagePerLevel { get; set; }
        [Range(0, 1000)]
        public double DamageGrowthPercent { get; set; }

        [Range(1, 100)]
        public int UnlockLevel { get; set; } = 1;
        public float CorruptionCost { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }

    // ============ PlayerSkill ============
    public class PlayerSkillResponseDto
    {
        public int PlayerSkillId { get; set; }
        public int PlayerProfileId { get; set; }
        public int SkillId { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public string? SkillDescription { get; set; }
        public string SkillType { get; set; } = string.Empty;
        public string DamageType { get; set; } = string.Empty;
        public string TargetType { get; set; } = string.Empty;
        public int Level { get; set; }
        public int Experience { get; set; }
        public bool IsEquipped { get; set; }
        public int? EquippedSlot { get; set; }
        // Calculated damage taking level and growth into account
        public double EffectiveDamage { get; set; }
        public int CooldownSeconds { get; set; }
        public double BaseDamage { get; set; }
        public int UnlockLevel { get; set; }
        public float CorruptionCost { get; set; }
        public DateTime UnlockedAt { get; set; }
        public DateTime? NextAvailableTime { get; set; }
    }

    public class UpgradePlayerSkillRequestDto
    {
        [Required]
        public int PlayerSkillId { get; set; }
    }

    public class EquipSkillRequestDto
    {
        [Required]
        public int PlayerSkillId { get; set; }

        public bool IsEquipped { get; set; }

        // Slot index 0..2 (null when not equipping)
        public int? SlotIndex { get; set; }
    }

    public class DismantlePlayerSkillRequestDto
    {
        [Required]
        public int PlayerSkillId { get; set; }

        // Optional: apply resulting XP to this owned PlayerSkill
        public int? TargetPlayerSkillId { get; set; }
    }

    // ============ Player Me Skills (GET /api/player-skills/me) ============
    public class PlayerMeSkillsResponseDto
    {
        public int PlayerProfileId { get; set; }
        public List<PlayerSkillResponseDto> Skills { get; set; } = new();
        public int TotalCount { get; set; }
    }
}
