namespace DAL.Models
{
    public class ShopItem
    {
        public Guid Id { get; set; }

        public Guid ItemId { get; set; }
        public Item? Item { get; set; }

        // Currencies: Gold, Gems
        public string Currency { get; set; } = "Gold";
        public decimal Price { get; set; } = 0;

        public int Stock { get; set; } = -1;
        public int DailyPurchaseLimit { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public DateTime? AvailableFrom { get; set; }
        public DateTime? AvailableTo { get; set; }

        public ICollection<PurchaseHistory> PurchaseHistories { get; set; } = new List<PurchaseHistory>();
    }
}