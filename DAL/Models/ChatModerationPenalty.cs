using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class ChatModerationPenalty
    {
        public int ChatModerationPenaltyId { get; set; }

        public int PlayerProfileId { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }

        public int? ReporterId { get; set; }
        public PlayerProfile? Reporter { get; set; }

        public int? ChatMessageId { get; set; }
        public ChatMessage? ChatMessage { get; set; }

        public int? WorldChatMessageId { get; set; }
        public WorldChatMessage? WorldChatMessage { get; set; }

        [Required, MaxLength(30)]
        public string Channel { get; set; } = "World";

        [Required, MaxLength(500)]
        public string ContentSnapshot { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? ReportReason { get; set; }

        [MaxLength(500)]
        public string? MatchedTerms { get; set; }

        public int ViolationCount { get; set; }
        public int LockLevel { get; set; }
        public DateTime LockedUntil { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}