namespace DAL.Models
{
    public class PlayerQuest
    {
        public Guid Id { get; set; }

        public Guid PlayerProfileId { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }

        public Guid QuestId { get; set; }
        public Quest? Quest { get; set; }

        // Statuses: NotStarted, InProgress, Completed, Claimed, Failed
        public string Status { get; set; } = "NotStarted";
        public int Progress { get; set; } = 0;
        public int TargetValue { get; set; } = 1;

        public DateTime AcceptedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public DateTime? ClaimedAt { get; set; }
    }
}