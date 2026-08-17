namespace DAL.Models
{
    // Initializes a new default instance of the PurchaseHistory class.
    public class PurchaseHistory
    {
        // Executes purchase history id operation.
        public int PurchaseHistoryId { get; set; }

        // Executes player profile id operation.
        public int PlayerProfileId { get; set; }
        // Executes player profile operation.
        public PlayerProfile? PlayerProfile { get; set; }

        // Executes shop item id operation.
        public int ShopItemId { get; set; }
        // Executes shop item operation.
        public ShopItem? ShopItem { get; set; }

        // Executes quantity operation.
        public int Quantity { get; set; } = 1;
        // Executes total price operation.
        public decimal TotalPrice { get; set; } = 0;

        // Executes purchased at operation.
        public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;
    }
}
