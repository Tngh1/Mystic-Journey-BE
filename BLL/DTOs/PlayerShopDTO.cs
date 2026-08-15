using System;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    public class ViewShopQueryDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0.")]
        public int Page { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100.")]
        public int PageSize { get; set; } = 20;

        [RegularExpression("^(Gold|Gems)?$", ErrorMessage = "Currency must be Gold or Gems.")]
        public string? Currency { get; set; }

        [StringLength(50, ErrorMessage = "Item type must not exceed 50 characters.")]
        public string? ItemType { get; set; }

        [StringLength(100, ErrorMessage = "Search keyword must not exceed 100 characters.")]
        public string? Search { get; set; }

        public bool IncludeSoldOut { get; set; } = false;
    }

    public class ShopItemPublicResponseDto
    {
        public int ShopItemId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ItemIconUrl { get; set; }
        public string ItemType { get; set; } = string.Empty;
        public string Rarity { get; set; } = string.Empty;
        public string Slot { get; set; } = string.Empty;
        public int MaxStack { get; set; }
        public string ShopSection { get; set; } = "Fixed";
        public string Currency { get; set; } = "Gold";
        public decimal? OriginalPrice { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public bool IsUnlimitedStock { get; set; }
        public int DailyPurchaseLimit { get; set; }
        public int WeeklyPurchaseLimit { get; set; }
        public int PurchasedToday { get; set; }
        public int PurchasedThisWeek { get; set; }
        public int? RemainingDailyPurchases { get; set; }
        public int? RemainingWeeklyPurchases { get; set; }
        public DateTime? AvailableFrom { get; set; }
        public DateTime? AvailableTo { get; set; }
        public bool CanPurchase { get; set; }
        public string? UnavailableReason { get; set; }
    }

    public class ShopRefreshStatusDto
    {
        public DateTime ShopDateUtc { get; set; }
        public DateTime NextResetUtc { get; set; }
        public int RefreshesUsedToday { get; set; }
        public int RefreshesRemainingToday { get; set; }
        public int MaxDailyRefreshes { get; set; }
        public bool CanRefresh { get; set; }
    }

    public class ShopRefreshResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public ShopRefreshStatusDto RefreshStatus { get; set; } = new();
        public PagedResultDto<ShopItemPublicResponseDto> Shop { get; set; } = new(0, Array.Empty<ShopItemPublicResponseDto>());
    }

    public class PurchaseShopItemRequestDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Shop item ID must be greater than 0.")]
        public int ShopItemId { get; set; }

        [Range(1, 999, ErrorMessage = "Quantity must be between 1 and 999.")]
        public int Quantity { get; set; } = 1;
    }

    public class PurchaseShopItemResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int PurchaseHistoryId { get; set; }
        public int ShopItemId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Currency { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal BalanceBefore { get; set; }
        public decimal BalanceAfter { get; set; }
        public int InventoryQuantity { get; set; }
        public CurrencyBalanceResponseDto Balance { get; set; } = new();
        public PlayerCurrencyLogResponseDto? Transaction { get; set; }
    }

    public class SkinShopItemResponseDto
    {
        public int SkinId { get; set; }
        public string SkinName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string SkinType { get; set; } = string.Empty;
        public string Rarity { get; set; } = string.Empty;
        public string? IconUrl { get; set; }
        public string? PreviewUrl { get; set; }
        public string Currency { get; set; } = "Gems";
        public decimal Price { get; set; }
        public bool IsOwned { get; set; }
        public bool CanPurchase { get; set; }
        public string? UnavailableReason { get; set; }
    }

    public class PurchaseShopSkinRequestDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Skin ID must be greater than 0.")]
        public int SkinId { get; set; }
    }

    public class PurchaseShopSkinResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int PlayerSkinId { get; set; }
        public int SkinId { get; set; }
        public string SkinName { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal BalanceBefore { get; set; }
        public decimal BalanceAfter { get; set; }
        public CurrencyBalanceResponseDto Balance { get; set; } = new();
        public PlayerCurrencyLogResponseDto? Transaction { get; set; }
    }
}
