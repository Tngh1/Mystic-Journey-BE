namespace DAL.Models
{
    /// <summary>
    /// Represents a single dungeon run initiated by a player.
    /// Lifecycle: Active → Completed → RewardClaimed
    /// </summary>
    public class DungeonSession
    {
        public int DungeonSessionId { get; set; }

        public int PlayerProfileId { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }

        public int DungeonConfigId { get; set; }
        public DungeonConfig? DungeonConfig { get; set; }

        /// <summary>UTC timestamp when the player entered the dungeon.</summary>
        public DateTime EnterTime { get; set; } = DateTime.UtcNow;

        /// <summary>UTC timestamp when the dungeon was completed (boss defeated).</summary>
        public DateTime? CompletedTime { get; set; }

        /// <summary>UTC timestamp when the rewards were claimed.</summary>
        public DateTime? ClaimedAt { get; set; }

        /// <summary>
        /// Session state machine.
        /// Values: Active | Completed | Abandoned | RewardClaimed
        /// </summary>
        public string Status { get; set; } = "Active";

        /// <summary>
        /// Guards against duplicate reward claims regardless of Status.
        /// Set to true atomically inside the claim-reward transaction.
        /// </summary>
        public bool IsRewardClaimed { get; set; } = false;

        public string? PartyMembers { get; set; }

        /// <summary>
        /// Comma-separated list of profile IDs who have successfully claimed rewards for this session.
        /// Used to prevent duplicate claims by the same party member.
        /// </summary>
        public string? ClaimedByMembers { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public DungeonProgress? Progress { get; set; }
    }
}
