namespace DAL.Models
{
    // Initializes a new default instance of the ShopItem class.
    public class ShopItem
    {
        // Executes shop item id operation.
        public int ShopItemId { get; set; }

        // Executes item id operation.
        public int ItemId { get; set; }
        // Executes item operation.
        public Item? Item { get; set; }

        // Executes shop section operation.
        public string ShopSection { get; set; } = ShopSections.Fixed;

        // Supported currencies: Gold or Gems; the selected currency determines which player balance is charged or credited.
        public string Currency { get; set; } = "Gold";
        // Executes price operation.
        public decimal Price { get; set; } = 0;

        // Executes stock operation.
        public int Stock { get; set; } = -1;
        // Executes daily purchase limit operation.
        public int DailyPurchaseLimit { get; set; } = 0;
        // Executes weekly purchase limit operation.
        public int WeeklyPurchaseLimit { get; set; } = 0;

        // Executes is active operation.
        public bool IsActive { get; set; } = true;

        // Executes available from operation.
        public DateTime? AvailableFrom { get; set; }
        // Executes available to operation.
        public DateTime? AvailableTo { get; set; }

        // Executes purchase histories operation.
        public ICollection<PurchaseHistory> PurchaseHistories { get; set; } = new List<PurchaseHistory>();
    }
}
