namespace DAL.Models
{
    public class PurchaseHistory
    {
        public Guid Id { get; set; }

        public Guid PlayerProfileId { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }

        public Guid ShopItemId { get; set; }
        public ShopItem? ShopItem { get; set; }

        public int Quantity { get; set; } = 1;
        public decimal TotalPrice { get; set; } = 0;

        public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;
    }
}