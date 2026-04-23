using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BLL.DTOs
{
    public class InventoryItemResponseDto
    {
        public Guid InventoryItemId { get; set; }
        public Guid PlayerProfileId { get; set; }
        public Guid ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string ItemType { get; set; } = string.Empty;
        public string ItemRarity { get; set; } = string.Empty;
        public string? IconUrl { get; set; }
        public int Quantity { get; set; }
        public bool IsEquipped { get; set; }
        public int EnhancementLevel { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class InventoryItemDetailResponseDto : InventoryItemResponseDto
    {
        public string? Description { get; set; }
        public decimal BaseValue { get; set; }
        public int MaxStack { get; set; }
        public bool IsTradable { get; set; }
        public EquipmentStatsDto? EquipmentStats { get; set; }
    }

    public class AddItemToInventoryRequestDto
    {
        public Guid ItemId { get; set; }
        public int Quantity { get; set; } = 1;
    }

    public class RemoveItemFromInventoryRequestDto
    {
        public Guid InventoryItemId { get; set; }
        public int Quantity { get; set; } = 1;
    }

    public class EquipItemRequestDto
    {
        public Guid InventoryItemId { get; set; }
    }

    public class UnequipItemRequestDto
    {
        public Guid InventoryItemId { get; set; }
    }

    public class EnhanceItemRequestDto
    {
        public Guid InventoryItemId { get; set; }
    }

    public class InventoryResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<InventoryItemResponseDto>? Items { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public InventoryItemDetailResponseDto? Item { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class InventoryApiResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public InventoryItemResponseDto? Item { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public InventoryItemDetailResponseDto? Detail { get; set; }
    }
}
