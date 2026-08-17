using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    // Initializes a new default instance of the Mailbox class.
    [Table("Mails")]
    public class Mailbox
    {
        // Executes mailbox id operation.
        [Column("MailId")]
        public int MailboxId { get; set; }

        // Executes player profile id operation.
        public int PlayerProfileId { get; set; }
        // Executes player profile operation.
        public PlayerProfile? PlayerProfile { get; set; }

        // Executes title operation.
        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        // Executes content operation.
        public string Content { get; set; } = string.Empty;

        // Mailbox type is a free-form category with System as the current default; the backend does not enforce a closed allowlist.
        public string Type { get; set; } = "System";

        // Executes attached gold operation.
        public decimal AttachedGold { get; set; } = 0;
        // Executes attached gems operation.
        public decimal AttachedGems { get; set; } = 0;

        // Executes attached items operation.
        public List<MailboxRewardItem> AttachedItems { get; set; } = new();

        // Executes is read operation.
        public bool IsRead { get; set; } = false;
        // Executes is claimed operation.
        public bool IsClaimed { get; set; } = false;

        // Executes sent at operation.
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        // Executes expired at operation.
        public DateTime? ExpiredAt { get; set; }

        // Executes is deleted operation.
        public bool IsDeleted { get; set; } = false;
        // Executes deleted at operation.
        public DateTime? DeletedAt { get; set; }
    }
}
