using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    // Initializes a new default instance of the Quest class.
    public class Quest
    {
        // Executes quest id operation.
        public int QuestId { get; set; }

        // Executes title operation.
        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        // Executes description operation.
        public string? Description { get; set; }

        // Executes type operation.
        public string Type { get; set; } = "Main";
        // Executes default status operation.
        public string DefaultStatus { get; set; } = "NotStarted";

        // Executes map name operation.
        [MaxLength(100)]
        public string MapName { get; set; } = "ElfForest";

        // Executes region name operation.
        [MaxLength(100)]
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
        public int TargetAmount { get; set; } = 1;
        // Executes reward experience operation.
        public int RewardExperience { get; set; } = 0;
        // Executes reward gold operation.
        public decimal RewardGold { get; set; } = 0;
        // Executes reward gems operation.
        public decimal RewardGems { get; set; } = 0;

        // Executes reward item id operation.
        public int? RewardItemId { get; set; }
        // Executes reward item operation.
        public Item? RewardItem { get; set; }
        // Executes reward items operation.
        public ICollection<QuestRewardItem> RewardItems { get; set; } = new List<QuestRewardItem>();

        // Executes reward skill id operation.
        public int? RewardSkillId { get; set; }
        // Executes reward skill operation.
        public Skill? RewardSkill { get; set; }
        // Executes reward skills operation.
        public ICollection<QuestRewardSkill> RewardSkills { get; set; } = new List<QuestRewardSkill>();

        // Executes is active operation.
        public bool IsActive { get; set; } = true;

        // Executes boss monster id operation.
        public int? BossMonsterId { get; set; }
        // Executes boss monster operation.
        public Monster? BossMonster { get; set; }

        // Executes player quests operation.
        public ICollection<PlayerQuest> PlayerQuests { get; set; } = new List<PlayerQuest>();
    }
}
