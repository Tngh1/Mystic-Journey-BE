using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class ChatMessage
    {
        public Guid Id { get; set; }

        public Guid SenderId { get; set; }
        public PlayerProfile? Sender { get; set; }

        // ChatTypes: Private, Party, Guild, Global, System
        public string ChatType { get; set; } = "Global";

        public Guid? RecipientId { get; set; }
        public PlayerProfile? Recipient { get; set; }

        public Guid? PartyId { get; set; }
        public Party? Party { get; set; }

        public Guid? GuildId { get; set; }
        public Guild? Guild { get; set; }

        [Required, MaxLength(500)]
        public string Content { get; set; } = string.Empty;

        public bool IsReported { get; set; } = false;
        public bool IsHidden { get; set; } = false;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
