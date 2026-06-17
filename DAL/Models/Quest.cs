using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Quest
    {
        public int QuestId { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        // Types: Main, Side, Daily, Event
        public string Type { get; set; } = "Main";
        // DefaultStatuses: NotStarted, InProgress, Completed, Claimed, Failed
        public string DefaultStatus { get; set; } = "NotStarted";

        [MaxLength(100)]
        public string MapName { get; set; } = "ElfForest";

        [MaxLength(100)]
        public string? RegionName { get; set; }

        // ObjectiveTypes: Explore, Defeat, Collect, Talk, OpenChest, Interact
        public string ObjectiveType { get; set; } = "Explore";
        public string? ObjectiveTarget { get; set; }
        public string? ObjectiveLocation { get; set; }
        public string? QuestGiverName { get; set; }

        public int RequiredLevel { get; set; } = 1;
        public int TargetAmount { get; set; } = 1;
        public int RewardExperience { get; set; } = 0;
        public decimal RewardGold { get; set; } = 0;
        public decimal RewardGems { get; set; } = 0;

        public int? RewardItemId { get; set; }
        public Item? RewardItem { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<PlayerQuest> PlayerQuests { get; set; } = new List<PlayerQuest>();
    }
}
