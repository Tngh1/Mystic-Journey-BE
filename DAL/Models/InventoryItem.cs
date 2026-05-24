namespace DAL.Models
{
    public class InventoryItem
    {
        public int Id { get; set; }

        public int PlayerProfileId { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }

        public int ItemId { get; set; }
        public Item? Item { get; set; }

        public int Quantity { get; set; } = 1;
        public bool IsEquipped { get; set; } = false;
        public bool IsSkin { get; set; } = false;

        // Slots: Weapon, Helmet, Armor, Gloves, Boots, Ring, Necklace
        public string? EquippedSlot { get; set; }

        public int EnhancementLevel { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}