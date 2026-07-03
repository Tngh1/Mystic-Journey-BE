using DAL.Models;
using DAL.Repositories.Results;

namespace DAL.Repositories.Interfaces
{
    public interface IPlayerShopRepository
    {
        Task<bool> PlayerExists(int playerProfileId);

        Task<(int TotalCount, List<ShopItem> Items)> GetShopItems(
            int page,
            int pageSize,
            string? currency,
            string? itemType,
            string? search,
            bool includeSoldOut,
            DateTime utcNow);

        Task<Dictionary<int, int>> GetPurchasedTodayCounts(
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
