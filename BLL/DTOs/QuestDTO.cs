using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // Initializes a new default instance of the QuestRewardItemDto class.
    public class QuestRewardItemDto
    {
        // Executes quest reward item id operation.
        public int QuestRewardItemId { get; set; }
        // Executes item id operation.
        public int ItemId { get; set; }
        // Executes item name operation.
        public string? ItemName { get; set; }
        // Executes icon url operation.
        public string? IconUrl { get; set; }
        // Executes quantity operation.
        public int Quantity { get; set; } = 1;
    }

    // Executes update quest reward item dto operation.
    public class UpdateQuestRewardItemDto
    {
        // Executes item id operation.
        [Required]
        public int ItemId { get; set; }

        // Executes quantity operation.
        [Range(1, 10000, ErrorMessage = "Quantity must be between 1 and 10000.")]
        public int Quantity { get; set; } = 1;
    }

    // Executes quest reward skill dto operation.
    public class QuestRewardSkillDto
    {
        // Executes quest reward skill id operation.
        public int QuestRewardSkillId { get; set; }
        // Executes skill id operation.
        public int SkillId { get; set; }
        // Executes skill name operation.
        public string? SkillName { get; set; }
        // Supported class requirements: Knight, Archer, Mage, or All; All allows every player class to use the skill or reward.
        public string? ClassRequirement { get; set; }
        // Supported skill types: Active, Passive, Buff, or Debuff; the type controls activation and effect presentation.
        public string? Type { get; set; }
        // Supported damage types: Physical, Magical, or TrueDamage; the value selects how skill damage is categorized and resolved.
        public string? DamageType { get; set; }
    }

    // Executes update quest reward skill dto operation.
    public class UpdateQuestRewardSkillDto
    {
        // Executes skill id operation.
        [Required]
        public int SkillId { get; set; }
    }
    // Executes quest response dto operation.
    public class QuestResponseDto
    {
        // Executes quest id operation.
        public int QuestId { get; set; }
        // Executes title operation.
        public string Title { get; set; } = string.Empty;
        // Executes description operation.
        public string? Description { get; set; }
        // Executes type operation.
        public string Type { get; set; } = "Main";
        // Executes default status operation.
        public string DefaultStatus { get; set; } = "NotStarted";
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
        // Executes required level operation.
        public int RequiredLevel { get; set; }
        // Executes target amount operation.
        public int TargetAmount { get; set; }
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
        // Executes dialogue id operation.
        public int? DialogueId { get; set; }
        // Executes dialogue npc id operation.
        public int? DialogueNpcId { get; set; }
        // Executes dialogue npc name operation.
        public string? DialogueNpcName { get; set; }
        // Executes dialogue content operation.
        public string? DialogueContent { get; set; }
        // Executes dialogue display order operation.
        public int? DialogueDisplayOrder { get; set; }
        // Executes dialogue is active operation.
        public bool? DialogueIsActive { get; set; }
        // Executes is active operation.
        public bool IsActive { get; set; }
    }

    // Executes update quest request dto operation.
    public class UpdateQuestRequestDto
    {
        // Executes title operation.
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        // Executes description operation.
        public string? Description { get; set; }
        // Executes type operation.
        public string Type { get; set; } = "Main";
        // Executes default status operation.
        public string DefaultStatus { get; set; } = "NotStarted";
        // Executes map name operation.
        [StringLength(100)]
        public string MapName { get; set; } = "ElfForest";
        // Executes region name operation.
        [StringLength(100)]
        public string? RegionName { get; set; }
        // Executes objective type operation.
        public string ObjectiveType { get; set; } = "Explore";
        // Executes objective target operation.
        public string? ObjectiveTarget { get; set; }
        // Executes objective location operation.
        public string? ObjectiveLocation { get; set; }
        // Executes quest giver name operation.
        public string? QuestGiverName { get; set; }
        // Executes required level operation.
        public int RequiredLevel { get; set; } = 1;
        // Executes target amount operation.
        [Range(1, 10000, ErrorMessage = "TargetAmount must be between 1 and 10000.")]
        public int TargetAmount { get; set; } = 1;
        // Executes reward experience operation.
        public int RewardExperience { get; set; }
        // Executes reward gold operation.
        public decimal RewardGold { get; set; }
        // Executes reward gems operation.
        public decimal RewardGems { get; set; }
        // Executes reward item id operation.
        public int? RewardItemId { get; set; }
        // Executes reward items operation.
        public List<UpdateQuestRewardItemDto> RewardItems { get; set; } = new();
        // Executes reward skill id operation.
        public int? RewardSkillId { get; set; }
        // Executes reward skills operation.
        public List<UpdateQuestRewardSkillDto> RewardSkills { get; set; } = new();
        // Executes sync dialogue operation.
        public bool SyncDialogue { get; set; } = false;
        // Executes dialogue content operation.
        public string? DialogueContent { get; set; }
        // Executes dialogue display order operation.
        public int? DialogueDisplayOrder { get; set; }
        // Executes dialogue is active operation.
        public bool? DialogueIsActive { get; set; }
        // Executes is active operation.
        public bool IsActive { get; set; } = true;
    }
}
