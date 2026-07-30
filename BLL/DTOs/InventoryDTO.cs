using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    public class InventoryItemResponseDto
    {
        public int InventoryItemId { get; set; }
        public int PlayerProfileId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string? ItemDescription { get; set; }
        public string ItemType { get; set; } = string.Empty;
        public string ItemRarity { get; set; } = string.Empty;
        public string ItemSlot { get; set; } = "None";
        public string? IconUrl { get; set; }
        public float CorruptionReduction { get; set; }
        public int Quantity { get; set; }
        public bool IsEquipped { get; set; }
        public bool IsSkin { get; set; }
        public string? EquippedSlot { get; set; }
        public int EnhancementLevel { get; set; }
        public DateTime CreatedAt { get; set; }

        // Chỉ số của trang bị, lấy từ Item.EquipmentStats. Client (UIItemDetailPopup) đọc đúng
        // các field này để hiện bảng chỉ số khi mở chi tiết vật phẩm; thiếu chúng thì popup
        // luôn hiện 0. Vật phẩm không phải trang bị (Consumable/Material) sẽ để 0.
        public int BaseHp { get; set; }
        public int BaseAtk { get; set; }
        public int BaseDef { get; set; }

        public int BonusHp { get; set; }
        public int BonusAtk { get; set; }
        public int BonusDef { get; set; }
        public float BonusCritRate { get; set; }
        public float BonusCritDamage { get; set; }
    }

    public class EquipItemRequestDto
    {
        [Required]
        public int InventoryItemId { get; set; }
    }

    public class UnequipSkinRequestDto
    {
        [Required]
        public int PlayerSkinId { get; set; }
    }

    public class UnequipItemRequestDto
    {
        [Required]
        public int InventoryItemId { get; set; }
    }

    public class InventorySummaryDto
    {
        public int TotalItems { get; set; }
        public int TotalSkins { get; set; }
        public List<InventoryItemResponseDto> EquippedItems { get; set; } = new();
        public List<InventoryItemResponseDto> BagItems { get; set; } = new();
        public List<PlayerSkinResponseDto> PlayerSkins { get; set; } = new();
        public int BagCapacity { get; set; }
    }

    public class AddInventoryItemRequestDto
    {
        [Required(ErrorMessage = "PlayerProfileId is required.")]
        public int PlayerProfileId { get; set; }

        [Required(ErrorMessage = "ItemId is required.")]
        public int ItemId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; } = 1;

        public bool IsSkin { get; set; } = false;
    }

    public class UpdateInventoryItemRequestDto
    {
        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative.")]
        public int Quantity { get; set; }

        public bool IsEquipped { get; set; }

        public string? EquippedSlot { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "EnhancementLevel cannot be negative.")]
        public int EnhancementLevel { get; set; }
    }

    public class ConsumeItemRequestDto
    {
        [Required]
        public int InventoryItemId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; } = 1;
    }

    public class ConsumeItemResultDto
    {
        public string ItemName { get; set; } = string.Empty;
        public string EffectType { get; set; } = string.Empty; // "Heal", "Energy", "None"
        public int EffectValue { get; set; }       // amount of HP or Energy restored
        public int? CurrentHp { get; set; }        // HP after consuming (if heal effect)
        public int? MaxHp { get; set; }            // max HP (if heal effect)
        public int? CurrentEnergy { get; set; }    // Energy after consuming (if energy effect)
        public int? MaxEnergy { get; set; }        // max Energy (if energy effect)
        public int RemainingQuantity { get; set; } // remaining stack count in bag
    }

    public class InventoryActionResultDto
    {
        public InventoryItemResponseDto? Item { get; set; }
        public PlayerStatsResponseDto? PlayerStats { get; set; }
    }

    public class PlayerMeInventoryResponseDto
    {
        public int PlayerProfileId { get; set; }
        public List<InventoryItemResponseDto> Items { get; set; } = new();
        public int TotalCount { get; set; }
    }
}
