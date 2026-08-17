using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // Initializes a new default instance of the InventoryItemResponseDto class.
    public class InventoryItemResponseDto
    {
        // Executes inventory item id operation.
        public int InventoryItemId { get; set; }
        // Executes player profile id operation.
        public int PlayerProfileId { get; set; }
        // Executes item id operation.
        public int ItemId { get; set; }
        // Executes item name operation.
        public string ItemName { get; set; } = string.Empty;
        // Executes item description operation.
        public string? ItemDescription { get; set; }
        // Executes item type operation.
        public string ItemType { get; set; } = string.Empty;
        // Supported rarity values: Common, Uncommon, Rare, Epic, Legendary, or Mythic; rarity controls quality, visuals, and sorting priority.
        public string ItemRarity { get; set; } = string.Empty;
        // Supported equipment slots: None, Weapon, Armor, Helmet, Gloves, Boots, Ring, Necklace, or Shield.
        public string ItemSlot { get; set; } = "None";
        // Executes icon url operation.
        public string? IconUrl { get; set; }
        // Executes corruption reduction operation.
        public float CorruptionReduction { get; set; }
        // Executes quantity operation.
        public int Quantity { get; set; }
        // Executes is equipped operation.
        public bool IsEquipped { get; set; }
        // Executes is skin operation.
        public bool IsSkin { get; set; }
        // Executes equipped slot operation.
        public string? EquippedSlot { get; set; }
        // Executes enhancement level operation.
        public int EnhancementLevel { get; set; }
        // Executes created at operation.
        public DateTime CreatedAt { get; set; }

        // Executes base hp operation.
        public int BaseHp { get; set; }
        // Executes base atk operation.
        public int BaseAtk { get; set; }
        // Executes base def operation.
        public int BaseDef { get; set; }

        // Executes bonus hp operation.
        public int BonusHp { get; set; }
        // Executes bonus atk operation.
        public int BonusAtk { get; set; }
        // Executes bonus def operation.
        public int BonusDef { get; set; }
        // Executes bonus crit rate operation.
        public float BonusCritRate { get; set; }
        // Executes bonus crit damage operation.
        public float BonusCritDamage { get; set; }
    }

    // Executes equip item request dto operation.
    public class EquipItemRequestDto
    {
        // Executes inventory item id operation.
        [Required]
        public int InventoryItemId { get; set; }
    }

    // Executes unequip skin request dto operation.
    public class UnequipSkinRequestDto
    {
        // Executes player skin id operation.
        [Required]
        public int PlayerSkinId { get; set; }
    }

    // Executes unequip item request dto operation.
    public class UnequipItemRequestDto
    {
        // Executes inventory item id operation.
        [Required]
        public int InventoryItemId { get; set; }
    }

    // Executes inventory summary dto operation.
    public class InventorySummaryDto
    {
        // Executes total items operation.
        public int TotalItems { get; set; }
        // Executes total skins operation.
        public int TotalSkins { get; set; }
        // Executes equipped items operation.
        public List<InventoryItemResponseDto> EquippedItems { get; set; } = new();
        // Executes bag items operation.
        public List<InventoryItemResponseDto> BagItems { get; set; } = new();
        // Executes player skins operation.
        public List<PlayerSkinResponseDto> PlayerSkins { get; set; } = new();
        // Executes bag capacity operation.
        public int BagCapacity { get; set; }
    }

    // Executes add inventory item request dto operation.
    public class AddInventoryItemRequestDto
    {
        // Executes player profile id operation.
        [Required(ErrorMessage = "PlayerProfileId is required.")]
        public int PlayerProfileId { get; set; }

        // Executes item id operation.
        [Required(ErrorMessage = "ItemId is required.")]
        public int ItemId { get; set; }

        // Executes quantity operation.
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; } = 1;

        // Executes is skin operation.
        public bool IsSkin { get; set; } = false;
    }

    // Executes update inventory item request dto operation.
    public class UpdateInventoryItemRequestDto
    {
        // Executes quantity operation.
        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative.")]
        public int Quantity { get; set; }

        // Executes is equipped operation.
        public bool IsEquipped { get; set; }

        // Executes equipped slot operation.
        public string? EquippedSlot { get; set; }

        // Executes enhancement level operation.
        [Range(0, int.MaxValue, ErrorMessage = "EnhancementLevel cannot be negative.")]
        public int EnhancementLevel { get; set; }
    }

    // Executes consume item request dto operation.
    public class ConsumeItemRequestDto
    {
        // Executes inventory item id operation.
        [Required]
        public int InventoryItemId { get; set; }

        // Executes quantity operation.
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; } = 1;
    }

    // Executes consume item result dto operation.
    public class ConsumeItemResultDto
    {
        // Executes item name operation.
        public string ItemName { get; set; } = string.Empty;
        // Executes effect type operation.
        public string EffectType { get; set; } = string.Empty;
        // Executes effect value operation.
        public int EffectValue { get; set; }
        // Executes current hp operation.
        public int? CurrentHp { get; set; }
        // Executes max hp operation.
        public int? MaxHp { get; set; }
        // Executes current energy operation.
        public int? CurrentEnergy { get; set; }
        // Executes max energy operation.
        public int? MaxEnergy { get; set; }
        // Executes corruption level operation.
        public float? CorruptionLevel { get; set; }
        // Executes remaining quantity operation.
        public int RemainingQuantity { get; set; }
    }

    // Executes inventory action result dto operation.
    public class InventoryActionResultDto
    {
        // Executes item operation.
        public InventoryItemResponseDto? Item { get; set; }
        // Executes player stats operation.
        public PlayerStatsResponseDto? PlayerStats { get; set; }
    }

    // Executes player me inventory response dto operation.
    public class PlayerMeInventoryResponseDto
    {
        // Executes player profile id operation.
        public int PlayerProfileId { get; set; }
        // Executes items operation.
        public List<InventoryItemResponseDto> Items { get; set; } = new();
        // Executes total count operation.
        public int TotalCount { get; set; }
    }
}
