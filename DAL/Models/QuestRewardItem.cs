using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    // Initializes a new default instance of the QuestRewardItem class.
    public class QuestRewardItem
    {
        // Executes quest reward item id operation.
        public int QuestRewardItemId { get; set; }

        // Executes quest id operation.
        public int QuestId { get; set; }
        // Executes quest operation.
        public Quest? Quest { get; set; }

        // Executes item id operation.
        public int ItemId { get; set; }
        // Executes item operation.
        public Item? Item { get; set; }

        // Executes quantity operation.
        [Range(1, 10000)]
        public int Quantity { get; set; } = 1;
    }
}
