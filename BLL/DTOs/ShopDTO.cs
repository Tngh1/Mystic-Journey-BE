using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // Initializes a new default instance of the ShopItemResponseDto class.
    public class ShopItemResponseDto
    {
        // Executes shop item id operation.
        public int ShopItemId { get; set; }
        // Executes item id operation.
        public int ItemId { get; set; }
        // Executes item name operation.
        public string? ItemName { get; set; }
        // Executes item icon url operation.
        public string? ItemIconUrl { get; set; }
        // Executes item type operation.
        public string? ItemType { get; set; }
        // Executes shop section operation.
        public string ShopSection { get; set; } = "Fixed";
        // Supported currencies: Gold or Gems; the selected currency determines which player balance is charged or credited.
        public string Currency { get; set; } = "Gold";
        // Executes price operation.
        public decimal Price { get; set; }
        // Executes stock operation.
        public int Stock { get; set; }
        // Executes daily purchase limit operation.
        public int DailyPurchaseLimit { get; set; }
        // Executes weekly purchase limit operation.
        public int WeeklyPurchaseLimit { get; set; }
        // Executes is active operation.
        public bool IsActive { get; set; }
        // Executes available from operation.
        public DateTime? AvailableFrom { get; set; }
        // Executes available to operation.
        public DateTime? AvailableTo { get; set; }
    }

    // Executes create shop item request dto operation.
    public class CreateShopItemRequestDto
    {
        // Executes item id operation.
        [Required]
        public int ItemId { get; set; }

        // Executes shop section operation.
        [RegularExpression("^(Fixed|DailyDeal)$", ErrorMessage = "Shop section must be Fixed or DailyDeal.")]
        public string ShopSection { get; set; } = "Fixed";

        [RegularExpression("^(Gold|Gems)$", ErrorMessage = "Currency must be Gold or Gems.")]
        // Supported currencies: Gold or Gems; the selected currency determines which player balance is charged or credited.
        public string Currency { get; set; } = "Gold";

        // Executes price operation.
        [Range(0, double.MaxValue, ErrorMessage = "Price cannot be negative.")]
        public decimal Price { get; set; }

        // Executes stock operation.
        [Range(-1, int.MaxValue, ErrorMessage = "Stock must be -1 (unlimited) or a non-negative number.")]
        public int Stock { get; set; } = -1;

        // Executes daily purchase limit operation.
        [Range(0, int.MaxValue, ErrorMessage = "DailyPurchaseLimit cannot be negative.")]
        public int DailyPurchaseLimit { get; set; }

        // Executes weekly purchase limit operation.
        [Range(0, int.MaxValue, ErrorMessage = "WeeklyPurchaseLimit cannot be negative.")]
        public int WeeklyPurchaseLimit { get; set; }
        // Executes is active operation.
        public bool IsActive { get; set; } = true;
        // Executes available from operation.
        public DateTime? AvailableFrom { get; set; }
        // Executes available to operation.
        public DateTime? AvailableTo { get; set; }
    }

    // Executes update shop item request dto operation.
    public class UpdateShopItemRequestDto
    {
        // Executes item id operation.
        [Required]
        public int ItemId { get; set; }

        // Executes shop section operation.
        [RegularExpression("^(Fixed|DailyDeal)$", ErrorMessage = "Shop section must be Fixed or DailyDeal.")]
        public string ShopSection { get; set; } = "Fixed";

        [RegularExpression("^(Gold|Gems)$", ErrorMessage = "Currency must be Gold or Gems.")]
        // Supported currencies: Gold or Gems; the selected currency determines which player balance is charged or credited.
        public string Currency { get; set; } = "Gold";

        // Executes price operation.
        [Range(0, double.MaxValue, ErrorMessage = "Price cannot be negative.")]
        public decimal Price { get; set; }

        // Executes stock operation.
        [Range(-1, int.MaxValue, ErrorMessage = "Stock must be -1 (unlimited) or a non-negative number.")]
        public int Stock { get; set; } = -1;

        // Executes daily purchase limit operation.
        [Range(0, int.MaxValue, ErrorMessage = "DailyPurchaseLimit cannot be negative.")]
        public int DailyPurchaseLimit { get; set; }

        // Executes weekly purchase limit operation.
        [Range(0, int.MaxValue, ErrorMessage = "WeeklyPurchaseLimit cannot be negative.")]
        public int WeeklyPurchaseLimit { get; set; }
        // Executes is active operation.
        public bool IsActive { get; set; } = true;
        // Executes available from operation.
        public DateTime? AvailableFrom { get; set; }
        // Executes available to operation.
        public DateTime? AvailableTo { get; set; }
    }

    // Executes purchase history response dto operation.
    public class PurchaseHistoryResponseDto
    {
        // Executes purchase history id operation.
        public int PurchaseHistoryId { get; set; }
        // Executes player profile id operation.
        public int PlayerProfileId { get; set; }
        // Executes player name operation.
        public string? PlayerName { get; set; }
        // Executes shop item id operation.
        public int ShopItemId { get; set; }
        // Executes item name operation.
        public string? ItemName { get; set; }
        // Executes item icon url operation.
        public string? ItemIconUrl { get; set; }
        // Executes quantity operation.
        public int Quantity { get; set; }
        // Executes total price operation.
        public decimal TotalPrice { get; set; }
        // Supported currencies: Gold or Gems; the selected currency determines which player balance is charged or credited.
        public string Currency { get; set; } = string.Empty;
        // Executes purchased at operation.
        public DateTime PurchasedAt { get; set; }
    }
}
