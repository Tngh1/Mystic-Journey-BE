using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BLL.DTOs
{
    public class InventoryItemResponseDto
    {
        public int Id { get; set; }
        public int PlayerProfileId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public bool IsEquipped { get; set; }
        public bool IsSkin { get; set; }
        public string? EquippedSlot { get; set; }
        public int EnhancementLevel { get; set; }
        public DateTime CreatedAt { get; set; }
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

    public class InventoryApiResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Data { get; set; }
    }
}
