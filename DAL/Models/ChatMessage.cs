using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class ChatMessage
    {
        public int ChatMessageId { get; set; }

        public int SenderId { get; set; }
        public PlayerProfile? Sender { get; set; }

        public int RecipientId { get; set; }
        public PlayerProfile? Recipient { get; set; }

        [Required, MaxLength(500)]
        public string Content { get; set; } = string.Empty;

        public bool IsReported { get; set; } = false;
        public bool IsHidden { get; set; } = false;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
