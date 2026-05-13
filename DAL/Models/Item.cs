using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Item
    {
        public Guid Id { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public ItemType Type { get; set; }
        public ItemRarity Rarity { get; set; }
        public EquipmentSlot Slot { get; set; } = EquipmentSlot.None;

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

        public enum ItemType
        {
            Weapon = 0,
            Armor = 1,
            Consumable = 2,
            Material = 3,
            Accessory = 4,
            QuestItem = 5
        }

        public enum ItemRarity
        {
            Common = 0,
            Uncommon = 1,
            Rare = 2,
            Epic = 3,
            Legendary = 4,
            Mythic = 5
        }

        public enum EquipmentSlot
        {
            None = 0,
            Weapon = 1,
            Helmet = 2,
            Armor = 3,
            Gloves = 4,
            Boots = 5,
            Ring = 6,
            Necklace = 7
        }
    }
}