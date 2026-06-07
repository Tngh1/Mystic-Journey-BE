namespace DAL.Models
{
    public class GachaBannerItem
    {
        public int GachaBannerItemId { get; set; }

        public int GachaBannerId { get; set; }
        public GachaBanner? GachaBanner { get; set; }

        public int ItemId { get; set; }
        public Item? Item { get; set; }

        public decimal DropRate { get; set; } = 0;
        public bool IsFeatured { get; set; } = false;
    }
}