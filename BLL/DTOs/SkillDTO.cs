using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using static DAL.Models.Skill;

namespace BLL.DTOs
{
    public class SkillResponseDto
    {
        public Guid SkillId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = string.Empty;
        public string DamageType { get; set; } = string.Empty;
        public string TargetType { get; set; } = string.Empty;
        public string ClassRequirement { get; set; } = string.Empty;
        public int ManaCost { get; set; }
        public int CooldownSeconds { get; set; }
        public int BaseDamage { get; set; }
        public int UnlockLevel { get; set; }
    }

    public class PlayerSkillResponseDto
    {
        public Guid PlayerSkillId { get; set; }
        public Guid PlayerProfileId { get; set; }
        public Guid SkillId { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public string? SkillDescription { get; set; }
        public string Category { get; set; } = string.Empty;
        public string DamageType { get; set; } = string.Empty;
        public string TargetType { get; set; } = string.Empty;
        public int Level { get; set; }
        public int Experience { get; set; }
        public bool IsEquipped { get; set; }
        public int ManaCost { get; set; }
        public int CooldownSeconds { get; set; }
        public int BaseDamage { get; set; }
        public DateTime UnlockedAt { get; set; }
    }

    public class UnlockSkillRequestDto
    {
        public Guid SkillId { get; set; }
    }

    public class UpgradeSkillRequestDto
    {
        public Guid PlayerSkillId { get; set; }
    }

    public class EquipSkillRequestDto
    {
        public Guid PlayerSkillId { get; set; }
    }

    public class SkillListResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<SkillResponseDto>? Skills { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<PlayerSkillResponseDto>? PlayerSkills { get; set; }
        public int TotalCount { get; set; }
    }

    public class SkillApiResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public SkillResponseDto? Skill { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PlayerSkillResponseDto? PlayerSkill { get; set; }
    }
}
