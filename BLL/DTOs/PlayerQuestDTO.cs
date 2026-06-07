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
        public string Status { get; set; } = "NotStarted";
        public int Progress { get; set; }
        public int TargetValue { get; set; }
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
        public int PlayerQuestId { get; set; }
    }

    public class QuestProgressUpdateDto
    {
        public int PlayerProfileId { get; set; }
        public string QuestType { get; set; } = string.Empty;
        public int IncrementBy { get; set; } = 1;
    }
}
