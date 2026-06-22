using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // ============ Quest ============
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
        public int? RewardSkillId { get; set; }
        public string? RewardSkillName { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateQuestRequestDto
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
        public int? RewardSkillId { get; set; }
        public bool IsActive { get; set; } = true;
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
        public int? RewardSkillId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
