namespace DAL.Models
{
    // Initializes a new default instance of the GachaBannerItem class.
    public class GachaBannerItem
    {
        // Executes gacha banner item id operation.
        public int GachaBannerItemId { get; set; }

        // Executes gacha banner id operation.
        public int GachaBannerId { get; set; }
        // Executes gacha banner operation.
        public GachaBanner? GachaBanner { get; set; }

        // Executes item id operation.
        public int ItemId { get; set; }
        // Executes item operation.
        public Item? Item { get; set; }

        // Executes drop rate operation.
        public decimal DropRate { get; set; } = 0;
        // Executes is featured operation.
        public bool IsFeatured { get; set; } = false;
    }
}
