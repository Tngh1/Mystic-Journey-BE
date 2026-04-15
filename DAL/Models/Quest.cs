using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Quest
    {
        public Guid Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public QuestType Type { get; set; } = QuestType.Main;
        public QuestStatus DefaultStatus { get; set; } = QuestStatus.NotStarted;

        public int RequiredLevel { get; set; } = 1;
        public int RewardExperience { get; set; } = 0;
        public decimal RewardGold { get; set; } = 0;
        public decimal RewardGems { get; set; } = 0;

        public Guid? RewardItemId { get; set; }
        public Item? RewardItem { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<PlayerQuest> PlayerQuests { get; set; } = new List<PlayerQuest>();

        public enum QuestType
        {
            Main = 0,
            Side = 1,
            Daily = 2,
            Event = 3
        }

        public enum QuestStatus
        {
            NotStarted = 0,
            InProgress = 1,
            Completed = 2,
            Claimed = 3,
            Failed = 4
        }
    }
}