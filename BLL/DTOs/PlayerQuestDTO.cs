using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // Initializes a new default instance of the PlayerQuestResponseDto class.
    public class PlayerQuestResponseDto
    {
        // Executes player quest id operation.
        public int PlayerQuestId { get; set; }
        // Executes player profile id operation.
        public int PlayerProfileId { get; set; }
        // Executes quest id operation.
        public int QuestId { get; set; }
        // Executes quest title operation.
        public string QuestTitle { get; set; } = string.Empty;
        // Executes quest description operation.
        public string? QuestDescription { get; set; }
        // Executes quest type operation.
        public string QuestType { get; set; } = string.Empty;
        // Executes map name operation.
        public string MapName { get; set; } = "ElfForest";
        // Executes region name operation.
        public string? RegionName { get; set; }
        // Executes objective type operation.
        public string ObjectiveType { get; set; } = "Explore";
        // Executes objective target operation.
        public string? ObjectiveTarget { get; set; }
        // Executes objective location operation.
        public string? ObjectiveLocation { get; set; }
        // Executes quest giver name operation.
        public string? QuestGiverName { get; set; }
        // Executes status operation.
        public string Status { get; set; } = "NotStarted";
        // Executes progress operation.
        public int Progress { get; set; }
        // Executes target value operation.
        public int TargetValue { get; set; }
        // Executes target amount operation.
        public int TargetAmount { get; set; }
        // Executes required level operation.
        public int RequiredLevel { get; set; }
        // Executes reward experience operation.
        public int RewardExperience { get; set; }
        // Executes reward gold operation.
        public decimal RewardGold { get; set; }
        // Executes reward gems operation.
        public decimal RewardGems { get; set; }
        // Executes reward item id operation.
        public int? RewardItemId { get; set; }
        // Executes reward item name operation.
        public string? RewardItemName { get; set; }
        // Executes reward items operation.
        public List<QuestRewardItemDto> RewardItems { get; set; } = new();
        // Executes reward skill id operation.
        public int? RewardSkillId { get; set; }
        // Executes reward skill name operation.
        public string? RewardSkillName { get; set; }
        // Executes reward skills operation.
        public List<QuestRewardSkillDto> RewardSkills { get; set; } = new();
        // Executes accepted at operation.
        public DateTime AcceptedAt { get; set; }
        // Executes completed at operation.
        public DateTime? CompletedAt { get; set; }
        // Executes claimed at operation.
        public DateTime? ClaimedAt { get; set; }
    }

    // Executes accept quest request dto operation.
    public class AcceptQuestRequestDto
    {
        // Executes quest id operation.
        [Required]
        public int QuestId { get; set; }
    }

    // Executes claim quest request dto operation.
    public class ClaimQuestRequestDto
    {
        // Executes quest id operation.
        [Required]
        public int QuestId { get; set; }
    }

    // Executes complete quest request dto operation.
    public class CompleteQuestRequestDto
    {
        // Executes quest id operation.
        [Required]
        public int QuestId { get; set; }
    }

    // Executes quest progress item dto operation.
    public class QuestProgressItemDto
    {
        // Executes quest id operation.
        [Required]
        public int QuestId { get; set; }

        // Executes progress operation.
        [Range(0, int.MaxValue)]
        public int Progress { get; set; }
    }

    // Executes batch progress request dto operation.
    public class BatchProgressRequestDto
    {
        // Executes updates operation.
        [Required]
        public List<QuestProgressItemDto> Updates { get; set; } = new();
    }

}
