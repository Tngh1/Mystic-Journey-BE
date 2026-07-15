using DAL.Models;
using DAL.Repositories.Results;

namespace DAL.Repositories.Interfaces
{
    public interface IPlayerShopRepository
    {
        Task<bool> PlayerExists(int playerProfileId);

        Task<PlayerShopRefreshState> GetOrCreateRefreshState(int playerProfileId, DateTime utcNow);

        Task<PlayerShopRefreshState?> TryConsumeRefresh(int playerProfileId, DateTime utcNow, int maxDailyRefreshes);

        Task<(int TotalCount, List<ShopItem> Items)> GetShopItems(
            int page,
            int pageSize,
            string? currency,
            string? itemType,
            string? search,
            bool includeSoldOut,
            DateTime utcNow,
            string shopSection,
            int? rotationSeed);


        Task<Dictionary<string, decimal>> GetFixedOriginalPrices(
            IEnumerable<(int ItemId, string Currency)> itemCurrencyPairs,
            DateTime utcNow);
        Task<Dictionary<int, int>> GetPurchasedTodayCounts(
            int playerProfileId,
            IEnumerable<int> shopItemIds,
            DateTime utcNow);

        Task<Dictionary<int, int>> GetPurchasedThisWeekCounts(
            int playerProfileId,
            IEnumerable<int> shopItemIds,
            DateTime utcNow);

        Task<PlayerShopPurchaseResult> PurchaseItem(
            int playerProfileId,
            int shopItemId,
            int quantity,
            DateTime utcNow);
    }
}