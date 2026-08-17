using DAL.Models;

namespace DAL.Repositories.Results
{
    // Executes purchase shop item status operation.
    public enum PurchaseShopItemStatus
    {
        Success,
        PlayerNotFound,
        ShopItemNotFound,
        InvalidQuantity,
        ShopItemInactive,
        ItemInactive,
        NotYetAvailable,
        Expired,
        SoldOut,
        DailyLimitExceeded,
        WeeklyLimitExceeded,
        UnsupportedCurrency,
        InsufficientCurrency,
        DailyDealNotAvailable
    }

    // Initializes a new default instance of the PlayerShopPurchaseResult class.
    public class PlayerShopPurchaseResult
    {
        // Item purchase outcomes: Success, lookup failures, invalid quantity, inactive or unavailable items, sold-out or limit failures, and currency failures.
        public PurchaseShopItemStatus Status { get; set; }
        // Executes player profile operation.
        public PlayerProfile? PlayerProfile { get; set; }
        // Executes shop item operation.
        public ShopItem? ShopItem { get; set; }
        // Executes purchase history operation.
        public PurchaseHistory? PurchaseHistory { get; set; }
        // Executes currency log operation.
        public PlayerCurrencyLog? CurrencyLog { get; set; }
        // Executes balance before operation.
        public decimal BalanceBefore { get; set; }
        // Executes balance after operation.
        public decimal BalanceAfter { get; set; }
        // Executes inventory quantity operation.
        public int InventoryQuantity { get; set; }
        // Executes purchased today after operation.
        public int PurchasedTodayAfter { get; set; }
        // Executes purchased this week after operation.
        public int PurchasedThisWeekAfter { get; set; }
    }

    // Executes purchase shop skin status operation.
    public enum PurchaseShopSkinStatus
    {
        Success,
        PlayerNotFound,
        SkinNotFound,
        WrongClass,
        NotForSale,
        AlreadyOwned,
        UnsupportedCurrency,
        InsufficientCurrency
    }

    // Executes player skin shop result operation.
    public class PlayerSkinShopResult
    {
        // Executes player profile operation.
        public PlayerProfile? PlayerProfile { get; set; }
        // Executes skins operation.
        public List<Skin> Skins { get; set; } = new();
        // Executes owned skin ids operation.
        public HashSet<int> OwnedSkinIds { get; set; } = new();
    }

    // Executes player shop skin purchase result operation.
    public class PlayerShopSkinPurchaseResult
    {
        // Skin purchase outcomes: Success, PlayerNotFound, SkinNotFound, WrongClass, NotForSale, AlreadyOwned, UnsupportedCurrency, or InsufficientCurrency.
        public PurchaseShopSkinStatus Status { get; set; }
        // Executes player profile operation.
        public PlayerProfile? PlayerProfile { get; set; }
        // Executes skin operation.
        public Skin? Skin { get; set; }
        // Executes player skin operation.
        public PlayerSkin? PlayerSkin { get; set; }
        // Executes currency log operation.
        public PlayerCurrencyLog? CurrencyLog { get; set; }
        // Executes balance before operation.
        public decimal BalanceBefore { get; set; }
        // Executes balance after operation.
        public decimal BalanceAfter { get; set; }
    }
}
