using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // ============ Shop ============
    public class ShopItemResponseDto
    {
        public int ShopItemId { get; set; }
        public int ItemId { get; set; }
        public string? ItemName { get; set; }
        public string? ItemIconUrl { get; set; }
        public string? ItemType { get; set; }
        public string ShopSection { get; set; } = "Fixed";
        public string Currency { get; set; } = "Gold";
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int DailyPurchaseLimit { get; set; }
        public int WeeklyPurchaseLimit { get; set; }
        public bool IsActive { get; set; }
        public DateTime? AvailableFrom { get; set; }
        public DateTime? AvailableTo { get; set; }
    }

    public class CreateShopItemRequestDto
    {
        [Required]
        public int ItemId { get; set; }

        [RegularExpression("^(Fixed|DailyDeal)$", ErrorMessage = "Shop section must be Fixed or DailyDeal.")]
        public string ShopSection { get; set; } = "Fixed";

        [RegularExpression("^(Gold|Gems)$", ErrorMessage = "Currency must be Gold or Gems.")]
        public string Currency { get; set; } = "Gold";

        [Range(0, double.MaxValue, ErrorMessage = "Price cannot be negative.")]
        public decimal Price { get; set; }

        public int Stock { get; set; } = -1;
        public int DailyPurchaseLimit { get; set; }
        public int WeeklyPurchaseLimit { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? AvailableFrom { get; set; }
        public DateTime? AvailableTo { get; set; }
    }

    public class UpdateShopItemRequestDto
    {
        [Required]
        public int ItemId { get; set; }

        [RegularExpression("^(Fixed|DailyDeal)$", ErrorMessage = "Shop section must be Fixed or DailyDeal.")]
        public string ShopSection { get; set; } = "Fixed";

        [RegularExpression("^(Gold|Gems)$", ErrorMessage = "Currency must be Gold or Gems.")]
        public string Currency { get; set; } = "Gold";

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public int Stock { get; set; } = -1;
        public int DailyPurchaseLimit { get; set; }
        public int WeeklyPurchaseLimit { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? AvailableFrom { get; set; }
        public DateTime? AvailableTo { get; set; }
    }

    // ============ Purchase History ============
    public class PurchaseHistoryResponseDto
    {
        public int PurchaseHistoryId { get; set; }
        public int PlayerProfileId { get; set; }
        public string? PlayerName { get; set; }
        public int ShopItemId { get; set; }
        public string? ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string Currency { get; set; } = string.Empty;
        public DateTime PurchasedAt { get; set; }
    }
}