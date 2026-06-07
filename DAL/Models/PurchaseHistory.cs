namespace DAL.Models
{
    public class PurchaseHistory
    {
        public int PurchaseHistoryId { get; set; }

        public int PlayerProfileId { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }

        public int ShopItemId { get; set; }
        public ShopItem? ShopItem { get; set; }

        public int Quantity { get; set; } = 1;
        public decimal TotalPrice { get; set; } = 0;

        public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;
    }
}