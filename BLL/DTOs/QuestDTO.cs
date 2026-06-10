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
        public int RequiredLevel { get; set; }
        public int RewardExperience { get; set; }
        public decimal RewardGold { get; set; }
        public decimal RewardGems { get; set; }
        public int? RewardItemId { get; set; }
        public string? RewardItemName { get; set; }
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
        public int RequiredLevel { get; set; } = 1;
        public int RewardExperience { get; set; }
        public decimal RewardGold { get; set; }
        public decimal RewardGems { get; set; }
        public int? RewardItemId { get; set; }
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
        public int RequiredLevel { get; set; } = 1;
        public int RewardExperience { get; set; }
        public decimal RewardGold { get; set; }
        public decimal RewardGems { get; set; }
        public int? RewardItemId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
