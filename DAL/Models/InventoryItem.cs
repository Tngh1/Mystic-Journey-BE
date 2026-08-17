namespace DAL.Models
{
    // Initializes a new default instance of the InventoryItem class.
    public class InventoryItem
    {
        // Executes inventory item id operation.
        public int InventoryItemId { get; set; }

        // Executes player profile id operation.
        public int PlayerProfileId { get; set; }
        // Executes player profile operation.
        public PlayerProfile? PlayerProfile { get; set; }

        // Executes item id operation.
        public int ItemId { get; set; }
        // Executes item operation.
        public Item? Item { get; set; }

        // Executes quantity operation.
        public int Quantity { get; set; } = 1;
        // Executes is equipped operation.
        public bool IsEquipped { get; set; } = false;
        // Executes is skin operation.
        public bool IsSkin { get; set; } = false;

        // Executes equipped slot operation.
        public string? EquippedSlot { get; set; }

        // Executes enhancement level operation.
        public int EnhancementLevel { get; set; } = 0;

        // Executes created at operation.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
