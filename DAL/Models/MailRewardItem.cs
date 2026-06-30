using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class MailRewardItem
    {
        public int MailRewardItemId { get; set; }

        public int MailId { get; set; }
        public Mail? Mail { get; set; }

        public int ItemId { get; set; }
        public Item? Item { get; set; }

        public int Quantity { get; set; } = 1;
    }
}
