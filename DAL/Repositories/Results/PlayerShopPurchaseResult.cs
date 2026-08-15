using DAL.Models;

namespace DAL.Repositories.Results
{
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

    public class PlayerShopPurchaseResult
    {
        public PurchaseShopItemStatus Status { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }
        public ShopItem? ShopItem { get; set; }
        public PurchaseHistory? PurchaseHistory { get; set; }
        public PlayerCurrencyLog? CurrencyLog { get; set; }
        public decimal BalanceBefore { get; set; }
        public decimal BalanceAfter { get; set; }
        public int InventoryQuantity { get; set; }
        public int PurchasedTodayAfter { get; set; }
        public int PurchasedThisWeekAfter { get; set; }
    }

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

    public class PlayerSkinShopResult
    {
        public PlayerProfile? PlayerProfile { get; set; }
        public List<Skin> Skins { get; set; } = new();
        public HashSet<int> OwnedSkinIds { get; set; } = new();
    }

    public class PlayerShopSkinPurchaseResult
    {
        public PurchaseShopSkinStatus Status { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }
        public Skin? Skin { get; set; }
        public PlayerSkin? PlayerSkin { get; set; }
        public PlayerCurrencyLog? CurrencyLog { get; set; }
        public decimal BalanceBefore { get; set; }
        public decimal BalanceAfter { get; set; }
    }
}
