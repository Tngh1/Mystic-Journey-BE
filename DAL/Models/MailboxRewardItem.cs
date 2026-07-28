using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    [Table("MailRewardItems")]
    public class MailboxRewardItem
    {
        [Column("MailRewardItemId")]
        public int MailboxRewardItemId { get; set; }

        [Column("MailId")]
        public int MailboxId { get; set; }
        public Mailbox? Mailbox { get; set; }

        public int ItemId { get; set; }
        public Item? Item { get; set; }

        public int Quantity { get; set; } = 1;
    }
}
