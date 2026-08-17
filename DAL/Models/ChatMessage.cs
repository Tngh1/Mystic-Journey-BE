using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    // Initializes a new default instance of the ChatMessage class.
    public class ChatMessage
    {
        // Executes chat message id operation.
        public int ChatMessageId { get; set; }

        // Executes sender id operation.
        public int SenderId { get; set; }
        // Executes sender operation.
        public PlayerProfile? Sender { get; set; }

        // Executes recipient id operation.
        public int RecipientId { get; set; }
        // Executes recipient operation.
        public PlayerProfile? Recipient { get; set; }

        // Executes content operation.
        [Required, MaxLength(500)]
        public string Content { get; set; } = string.Empty;

        // Executes is reported operation.
        public bool IsReported { get; set; } = false;
        // Executes is hidden operation.
        public bool IsHidden { get; set; } = false;

        // Executes reported by id operation.
        public int? ReportedById { get; set; }
        // Executes reported by operation.
        public PlayerProfile? ReportedBy { get; set; }

        // Executes report reason operation.
        [MaxLength(500)]
        public string? ReportReason { get; set; }

        // Executes reported at operation.
        public DateTime? ReportedAt { get; set; }

        // Executes sent at operation.
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
