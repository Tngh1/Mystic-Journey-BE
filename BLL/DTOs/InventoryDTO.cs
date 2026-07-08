using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // ============ InventoryItem ============
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

    public class EnhanceItemRequestDto
    {
        [Required]
        public int InventoryItemId { get; set; }
    }

    public class EnhanceResultDto
    {
        public bool Success { get; set; }
        public int NewEnhancementLevel { get; set; }
        public int BonusHpGained { get; set; }
        public int BonusAtkGained { get; set; }
        public int BonusDefGained { get; set; }
        public decimal Cost { get; set; }
        public string? Message { get; set; }
    }

    // ============ Inventory Summary ============
    public class InventorySummaryDto
    {
        public int TotalItems { get; set; }
        public int TotalSkins { get; set; }
        public List<InventoryItemResponseDto> EquippedItems { get; set; } = new();
        public List<InventoryItemResponseDto> BagItems { get; set; } = new();
        public List<PlayerSkinResponseDto> PlayerSkins { get; set; } = new();
        public int BagCapacity { get; set; }
    }

    // ============ Additional Inventory Requests ============
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

    public class InventoryActionResultDto
    {
        public InventoryItemResponseDto? Item { get; set; }
        public PlayerStatsResponseDto? PlayerStats { get; set; }
    }

    // ============ Player Me Inventory (GET /api/inventory/me/full) ============
    public class PlayerMeInventoryResponseDto
    {
        public int PlayerProfileId { get; set; }
        public List<InventoryItemResponseDto> Items { get; set; } = new();
        public int TotalCount { get; set; }
    }
}
