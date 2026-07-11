using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class GuildChatMessage
    {
        public int GuildChatMessageId { get; set; }

        public int GuildId { get; set; }
        public Guild? Guild { get; set; }

        public int SenderId { get; set; }
        public PlayerProfile? Sender { get; set; }

        [Required, MaxLength(500)]
        public string Content { get; set; } = string.Empty;

        public GuildMessageType MessageType { get; set; } = GuildMessageType.Text;

        // Snapshot of sender's role at time of message for rich chat display
        public GuildRole SenderRole { get; set; } = GuildRole.Member;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
