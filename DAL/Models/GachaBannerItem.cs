namespace DAL.Models
{
    public class GachaBannerItem
    {
        public Guid Id { get; set; }

        public Guid GachaBannerId { get; set; }
        public GachaBanner? GachaBanner { get; set; }

        public Guid ItemId { get; set; }
        public Item? Item { get; set; }

        public decimal DropRate { get; set; } = 0;
        public bool IsFeatured { get; set; } = false;
    }
}