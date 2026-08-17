using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    // Initializes a new default instance of the Item class.
    public class Item
    {
        // Executes item id operation.
        public int ItemId { get; set; }

        // Executes name operation.
        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        // Executes description operation.
        [MaxLength(1000)]
        public string? Description { get; set; }

        // Executes type operation.
        public string Type { get; set; } = "Weapon";
        // Supported rarity values: Common, Uncommon, Rare, Epic, Legendary, or Mythic; rarity controls quality, visuals, and sorting priority.
        public string Rarity { get; set; } = "Common";
        // Supported equipment slots: None, Weapon, Armor, Helmet, Gloves, Boots, Ring, Necklace, or Shield.
        public string Slot { get; set; } = "None";

        // Executes base value operation.
        public decimal BaseValue { get; set; } = 0;
        // Executes corruption reduction operation.
        public float CorruptionReduction { get; set; } = 0;
        // Executes max stack operation.
        public int MaxStack { get; set; } = 1;
        // Executes is active operation.
        public bool IsActive { get; set; } = true;

        // Executes icon url operation.
        public string? IconUrl { get; set; }

        // Executes created at operation.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Executes equipment stats operation.
        public EquipmentStats? EquipmentStats { get; set; }

        // Executes inventory items operation.
        public ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();
        // Executes gacha banner items operation.
        public ICollection<GachaBannerItem> GachaBannerItems { get; set; } = new List<GachaBannerItem>();
        // Executes shop items operation.
        public ICollection<ShopItem> ShopItems { get; set; } = new List<ShopItem>();
        // Executes monster drops operation.
        public ICollection<MonsterDrop> MonsterDrops { get; set; } = new List<MonsterDrop>();
    }
}
