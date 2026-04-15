namespace DAL.Models
{
    public class InventoryItem
    {
        public Guid Id { get; set; }

        public Guid PlayerProfileId { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }

        public Guid ItemId { get; set; }
        public Item? Item { get; set; }

        public int Quantity { get; set; } = 1;
        public bool IsEquipped { get; set; } = false;
        public int EnhancementLevel { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}