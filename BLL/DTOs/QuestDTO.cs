using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // ============ Quest ============
    public class QuestRewardItemDto
    {
        public int QuestRewardItemId { get; set; }
        public int ItemId { get; set; }
        public string? ItemName { get; set; }
        public string? IconUrl { get; set; }
        public int Quantity { get; set; } = 1;
    }

    public class UpdateQuestRewardItemDto
    {
        [Required]
        public int ItemId { get; set; }

        [Range(1, 10000, ErrorMessage = "Quantity must be between 1 and 10000.")]
        public int Quantity { get; set; } = 1;
    }

    public class QuestRewardSkillDto
    {
        public int QuestRewardSkillId { get; set; }
        public int SkillId { get; set; }
        public string? SkillName { get; set; }
        public string? ClassRequirement { get; set; }
        public string? Type { get; set; }
        public string? DamageType { get; set; }
    }

    public class UpdateQuestRewardSkillDto
    {
        [Required]
        public int SkillId { get; set; }
    }
    public class QuestResponseDto
    {
        public int QuestId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Type { get; set; } = "Main";
        public string DefaultStatus { get; set; } = "NotStarted";
        public string MapName { get; set; } = "ElfForest";
        public string? RegionName { get; set; }
        public string ObjectiveType { get; set; } = "Explore";
        public string? ObjectiveTarget { get; set; }
        public string? ObjectiveLocation { get; set; }
        public string? QuestGiverName { get; set; }
        public int RequiredLevel { get; set; }
        public int TargetAmount { get; set; }
        public int RewardExperience { get; set; }
        public decimal RewardGold { get; set; }
        public decimal RewardGems { get; set; }
        public int? RewardItemId { get; set; }
        public string? RewardItemName { get; set; }
        public List<QuestRewardItemDto> RewardItems { get; set; } = new();
        public int? RewardSkillId { get; set; }
        public string? RewardSkillName { get; set; }
        public List<QuestRewardSkillDto> RewardSkills { get; set; } = new();
        public int? DialogueId { get; set; }
        public int? DialogueNpcId { get; set; }
        public string? DialogueNpcName { get; set; }
        public string? DialogueContent { get; set; }
        public int? DialogueDisplayOrder { get; set; }
        public bool? DialogueIsActive { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateQuestRequestDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }
        public string Type { get; set; } = "Main";
        public string DefaultStatus { get; set; } = "NotStarted";
        [StringLength(100)]
        public string MapName { get; set; } = "ElfForest";
        [StringLength(100)]
        public string? RegionName { get; set; }
        public string ObjectiveType { get; set; } = "Explore";
        public string? ObjectiveTarget { get; set; }
        public string? ObjectiveLocation { get; set; }
        public string? QuestGiverName { get; set; }
        public int RequiredLevel { get; set; } = 1;
        [Range(1, 10000, ErrorMessage = "TargetAmount must be between 1 and 10000.")]
        public int TargetAmount { get; set; } = 1;
        public int RewardExperience { get; set; }
        public decimal RewardGold { get; set; }
        public decimal RewardGems { get; set; }
        public int? RewardItemId { get; set; }
        public List<UpdateQuestRewardItemDto> RewardItems { get; set; } = new();
        public int? RewardSkillId { get; set; }
        public List<UpdateQuestRewardSkillDto> RewardSkills { get; set; } = new();
        public bool SyncDialogue { get; set; } = false;
        public string? DialogueContent { get; set; }
        public int? DialogueDisplayOrder { get; set; }
        public bool? DialogueIsActive { get; set; }
        public bool IsActive { get; set; } = true;
    }
}