using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class QuestRewardItem
    {
        public int QuestRewardItemId { get; set; }

        public int QuestId { get; set; }
        public Quest? Quest { get; set; }

        public int ItemId { get; set; }
        public Item? Item { get; set; }

        [Range(1, 10000)]
        public int Quantity { get; set; } = 1;
    }
}