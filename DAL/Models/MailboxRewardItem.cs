using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    // Initializes a new default instance of the MailboxRewardItem class.
    [Table("MailRewardItems")]
    public class MailboxRewardItem
    {
        // Executes mailbox reward item id operation.
        [Column("MailRewardItemId")]
        public int MailboxRewardItemId { get; set; }

        // Executes mailbox id operation.
        [Column("MailId")]
        public int MailboxId { get; set; }
        // Executes mailbox operation.
        public Mailbox? Mailbox { get; set; }

        // Executes item id operation.
        public int ItemId { get; set; }
        // Executes item operation.
        public Item? Item { get; set; }

        // Executes quantity operation.
        public int Quantity { get; set; } = 1;
    }
}
