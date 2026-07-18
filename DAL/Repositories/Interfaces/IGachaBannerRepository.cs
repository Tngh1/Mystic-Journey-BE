using DAL.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DAL.Repositories.Interfaces
{
    // Qu?n lý gacha banners (banner gacha/quay thu?ng).
    // Game APIs: Xem danh sách, xem chi ti?t banner.
    // Admin APIs: T?o, c?p nh?t banner và thêm items.
    public interface IGachaBannerRepository
    {
        // -----------------------------------------------------------------------
        // GAME APIs (Ngu?i choi)
        // -----------------------------------------------------------------------

        // L?y banner gacha theo mã d?nh danh.
        Task<GachaBanner?> GetGachaBannerById(int id);

        // L?y banner gacha kèm danh sách v?t ph?m có th? quay.
        Task<GachaBanner?> GetGachaBannerByIdWithItems(int id);

        // L?y danh sách v?t ph?m trong banner.
        Task<List<GachaBannerItem>> GetBannerItems(int bannerId);

        // L?y danh sách v?t ph?m trong banner có phân trang.
        Task<(int TotalCount, List<GachaBannerItem> Items)> GetBannerItemsPaged(int page, int pageSize);

        // -----------------------------------------------------------------------
        // ADMIN APIs
        // -----------------------------------------------------------------------

        // T?o banner gacha m?i.
        Task<GachaBanner> CreateGachaBanner(GachaBanner banner);

        // C?p nh?t banner gacha.
        Task<GachaBanner> UpdateGachaBanner(GachaBanner banner);

        // Xóa v?t ph?m kh?i banner.
        Task<bool> RemoveBannerItem(int bannerId, int bannerItemId);

        // T?o v?t ph?m trong banner gacha.
        Task<GachaBannerItem> CreateBannerItem(GachaBannerItem item);

        // L?y danh sách banner có phân trang, l?c theo tìm ki?m, lo?i và tr?ng thái.
        Task<(int TotalCount, List<GachaBanner> Items)> GetBannersPaged(int page, int pageSize, string? search, string? type, bool? isActive, string? sortBy = null, string? sortOrder = null);

        // Luu l?ch s? quay
        Task<GachaPullHistory> AddGachaPullHistory(GachaPullHistory history);

        // L?y l?ch s? quay theo player và banner d? tính pity
        Task<List<GachaPullHistory>> GetPullHistoryByPlayerAndBanner(int playerProfileId, int bannerId);

        // L?y danh sách l?ch s? quay c?a ngu?i choi
        Task<(int TotalCount, List<GachaPullHistory> Items)> GetGachaPullHistoryPaged(int playerProfileId, int page, int pageSize);

        // Admin: L?y toàn b? l?ch s? quay c?a t?t c? ngu?i choi
        Task<(int TotalCount, List<GachaPullHistory> Items)> GetAllGachaPullHistoryPaged(int page, int pageSize, int? bannerId, string? rarity);

        // L?y th?ng kê gacha c?a ngu?i choi
        Task<(int TotalPulls, decimal TotalCost, int LegendaryPulls, string PlayerName, int AccountId)?> GetPlayerGachaStatsAsync(int playerProfileId);
    }
}
