using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    // Initializes a new default instance of the GuildChatMessage class.
    public class GuildChatMessage
    {
        // Executes guild chat message id operation.
        public int GuildChatMessageId { get; set; }

        // Executes guild id operation.
        public int GuildId { get; set; }
        // Executes guild operation.
        public Guild? Guild { get; set; }

        // Executes sender id operation.
        public int SenderId { get; set; }
        // Executes sender operation.
        public PlayerProfile? Sender { get; set; }

        // Executes content operation.
        [Required, MaxLength(500)]
        public string Content { get; set; } = string.Empty;

        // Executes message type operation.
        public GuildMessageType MessageType { get; set; } = GuildMessageType.Text;

        // Executes sender role operation.
        public GuildRole SenderRole { get; set; } = GuildRole.Member;

        // Executes sent at operation.
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
