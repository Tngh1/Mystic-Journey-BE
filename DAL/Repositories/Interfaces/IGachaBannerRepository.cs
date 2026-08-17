using DAL.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DAL.Repositories.Interfaces
{
    // Initializes a new default instance of the IGachaBannerRepository class.
    public interface IGachaBannerRepository
    {

        Task<GachaBanner?> GetGachaBannerById(int id);

        Task<GachaBanner?> GetGachaBannerByIdWithItems(int id);

        Task<List<GachaBannerItem>> GetBannerItems(int bannerId);

        Task<(int TotalCount, List<GachaBannerItem> Items)> GetBannerItemsPaged(int page, int pageSize);


        Task<GachaBanner> CreateGachaBanner(GachaBanner banner);

        Task<GachaBanner> UpdateGachaBanner(GachaBanner banner);

        Task<bool> RemoveBannerItem(int bannerId, int bannerItemId);

        Task<GachaBannerItem> CreateBannerItem(GachaBannerItem item);

        Task<(int TotalCount, List<GachaBanner> Items)> GetBannersPaged(int page, int pageSize, string? search, string? type, bool? isActive, string? sortBy = null, string? sortOrder = null);

        Task<GachaPullHistory> AddGachaPullHistory(GachaPullHistory history);

        Task<List<GachaPullHistory>> GetPullHistoryByPlayerAndBanner(int playerProfileId, int bannerId);

        Task<(int TotalCount, List<GachaPullHistory> Items)> GetGachaPullHistoryPaged(int playerProfileId, int page, int pageSize);

        Task<(int TotalCount, List<GachaPullHistory> Items)> GetAllGachaPullHistoryPaged(int page, int pageSize, int? bannerId, string? rarity);

        Task<(int TotalPulls, decimal TotalCost, int LegendaryPulls, string PlayerName, int AccountId)?> GetPlayerGachaStatsAsync(int playerProfileId);
    }
}
