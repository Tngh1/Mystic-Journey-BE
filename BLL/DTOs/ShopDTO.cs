using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using static DAL.Models.ShopItem;

namespace BLL.DTOs
{
    public class ShopItemResponseDto
    {
        public Guid ShopItemId { get; set; }
        public Guid ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string? ItemDescription { get; set; }
        public string ItemType { get; set; } = string.Empty;
        public string ItemRarity { get; set; } = string.Empty;
        public string? IconUrl { get; set; }
        public string Currency { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int DailyPurchaseLimit { get; set; }
        public bool IsActive { get; set; }
        public DateTime? AvailableFrom { get; set; }
        public DateTime? AvailableTo { get; set; }
    }

    public class PurchaseRequestDto
    {
        public Guid ShopItemId { get; set; }
        public int Quantity { get; set; } = 1;
    }

    public class PurchaseHistoryResponseDto
    {
        public Guid PurchaseId { get; set; }
        public Guid PlayerProfileId { get; set; }
        public Guid ShopItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string Currency { get; set; } = string.Empty;
        public DateTime PurchasedAt { get; set; }
    }

    public class ShopListResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<ShopItemResponseDto>? Items { get; set; }
        public int TotalCount { get; set; }
    }

    public class ShopApiResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ShopItemResponseDto? Item { get; set; }
    }

    public class PurchaseApiResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PurchaseHistoryResponseDto? Purchase { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PlayerCurrencyResponseDto? Currency { get; set; }
    }

    public class PurchaseHistoryListResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<PurchaseHistoryResponseDto>? Purchases { get; set; }
        public int TotalCount { get; set; }
    }
}
