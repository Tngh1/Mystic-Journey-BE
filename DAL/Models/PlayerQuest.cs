namespace DAL.Models
{
    // Initializes a new default instance of the PlayerQuest class.
    public class PlayerQuest
    {
        // Executes player quest id operation.
        public int PlayerQuestId { get; set; }

        // Executes player profile id operation.
        public int PlayerProfileId { get; set; }
        // Executes player profile operation.
        public PlayerProfile? PlayerProfile { get; set; }

        // Executes quest id operation.
        public int QuestId { get; set; }
        // Executes quest operation.
        public Quest? Quest { get; set; }

        // Executes status operation.
        public string Status { get; set; } = "NotStarted";
        // Executes progress operation.
        public int Progress { get; set; } = 0;
        // Executes target value operation.
        public int TargetValue { get; set; } = 1;

        // Executes accepted at operation.
        public DateTime AcceptedAt { get; set; } = DateTime.UtcNow;
        // Executes completed at operation.
        public DateTime? CompletedAt { get; set; }
        // Executes claimed at operation.
        public DateTime? ClaimedAt { get; set; }
    }
}
