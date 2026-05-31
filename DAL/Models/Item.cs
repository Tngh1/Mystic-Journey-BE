using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Item
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        // Types: Weapon, Armor, Consumable, Material, Accessory, QuestItem
        public string Type { get; set; } = "Weapon";
        // Rarities: Common, Uncommon, Rare, Epic, Legendary, Mythic
        public string Rarity { get; set; } = "Common";
        // Slots: None, Weapon, Helmet, Armor, Gloves, Boots, Ring, Necklace
        public string Slot { get; set; } = "None";

        public decimal BaseValue { get; set; } = 0;
        public int MaxStack { get; set; } = 1;
        public bool IsTradable { get; set; } = true;
        public bool IsActive { get; set; } = true;

        public string? IconUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public EquipmentStats? EquipmentStats { get; set; }

        public ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();
        public ICollection<GachaBannerItem> GachaBannerItems { get; set; } = new List<GachaBannerItem>();
        public ICollection<ShopItem> ShopItems { get; set; } = new List<ShopItem>();
        public ICollection<MonsterDrop> MonsterDrops { get; set; } = new List<MonsterDrop>();
    }
}