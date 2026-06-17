using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // ============ PlayerQuest ============
    public class PlayerQuestResponseDto
    {
        public int PlayerQuestId { get; set; }
        public int PlayerProfileId { get; set; }
        public int QuestId { get; set; }
        public string QuestTitle { get; set; } = string.Empty;
        public string? QuestDescription { get; set; }
        public string QuestType { get; set; } = string.Empty;
        public string MapName { get; set; } = "ElfForest";
        public string? RegionName { get; set; }
        public string ObjectiveType { get; set; } = "Explore";
        public string? ObjectiveTarget { get; set; }
        public string? ObjectiveLocation { get; set; }
        public string? QuestGiverName { get; set; }
        public string Status { get; set; } = "NotStarted";
        public int Progress { get; set; }
        public int TargetValue { get; set; }
        public int TargetAmount { get; set; }
        public int RequiredLevel { get; set; }
        public int RewardExperience { get; set; }
        public decimal RewardGold { get; set; }
        public decimal RewardGems { get; set; }
        public int? RewardItemId { get; set; }
        public string? RewardItemName { get; set; }
        public DateTime AcceptedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? ClaimedAt { get; set; }
    }

    public class AcceptQuestRequestDto
    {
        [Required]
        public int QuestId { get; set; }
    }

    public class ClaimQuestRequestDto
    {
        [Required]
        public int QuestId { get; set; }
    }

    public class CompleteQuestRequestDto
    {
        [Required]
        public int QuestId { get; set; }
    }

    public class QuestProgressItemDto
    {
        [Required]
        public int QuestId { get; set; }

        [Range(0, int.MaxValue)]
        public int Progress { get; set; }
    }

    public class BatchProgressRequestDto
    {
        [Required]
        public List<QuestProgressItemDto> Updates { get; set; } = new();
    }
}
