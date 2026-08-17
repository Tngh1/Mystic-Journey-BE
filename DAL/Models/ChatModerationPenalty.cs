using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    // Initializes a new default instance of the ChatModerationPenalty class.
    public class ChatModerationPenalty
    {
        // Executes chat moderation penalty id operation.
        public int ChatModerationPenaltyId { get; set; }

        // Executes player profile id operation.
        public int PlayerProfileId { get; set; }
        // Executes player profile operation.
        public PlayerProfile? PlayerProfile { get; set; }

        // Executes reporter id operation.
        public int? ReporterId { get; set; }
        // Executes reporter operation.
        public PlayerProfile? Reporter { get; set; }

        // Executes chat message id operation.
        public int? ChatMessageId { get; set; }
        // Executes chat message operation.
        public ChatMessage? ChatMessage { get; set; }

        // Executes world chat message id operation.
        public int? WorldChatMessageId { get; set; }
        // Executes world chat message operation.
        public WorldChatMessage? WorldChatMessage { get; set; }

        // Executes channel operation.
        [Required, MaxLength(30)]
        public string Channel { get; set; } = "World";

        // Executes content snapshot operation.
        [Required, MaxLength(500)]
        public string ContentSnapshot { get; set; } = string.Empty;

        // Executes report reason operation.
        [MaxLength(500)]
        public string? ReportReason { get; set; }

        // Executes matched terms operation.
        [MaxLength(500)]
        public string? MatchedTerms { get; set; }

        // Executes violation count operation.
        public int ViolationCount { get; set; }
        // Executes lock level operation.
        public int LockLevel { get; set; }
        // Executes locked until operation.
        public DateTime LockedUntil { get; set; }
        // Executes created at operation.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
