namespace DAL.Models
{
    // Initializes a new default instance of the DungeonSession class.
    public class DungeonSession
    {
        // Executes dungeon session id operation.
        public int DungeonSessionId { get; set; }

        // Executes player profile id operation.
        public int PlayerProfileId { get; set; }
        // Executes player profile operation.
        public PlayerProfile? PlayerProfile { get; set; }

        // Executes dungeon config id operation.
        public int DungeonConfigId { get; set; }
        // Executes dungeon config operation.
        public DungeonConfig? DungeonConfig { get; set; }

        // Executes enter time operation.
        public DateTime EnterTime { get; set; } = DateTime.UtcNow;

        // Executes completed time operation.
        public DateTime? CompletedTime { get; set; }

        // Executes claimed at operation.
        public DateTime? ClaimedAt { get; set; }

        // Supported dungeon session states: Active, Completed, Abandoned, Failed, Expired, or RewardClaimed; transitions control progress and reward eligibility.
        public string Status { get; set; } = "Active";

        // Executes is reward claimed operation.
        public bool IsRewardClaimed { get; set; } = false;

        // Executes party members operation.
        public string? PartyMembers { get; set; }

        // Executes claimed by members operation.
        public string? ClaimedByMembers { get; set; }

        // Executes created at operation.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // Executes updated at operation.
        public DateTime? UpdatedAt { get; set; }

        // Executes progress operation.
        public DungeonProgress? Progress { get; set; }
    }
}
