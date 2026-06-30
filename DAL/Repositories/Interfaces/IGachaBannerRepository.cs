using DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Quản lý gacha banners (banner gacha/quay thưởng).
    // Game APIs: Xem danh sách, xem chi tiết banner.
    // Admin APIs: Tạo, cập nhật banner và thêm items.
    public interface IGachaBannerRepository
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy banner gacha theo mã định danh.
        Task<GachaBanner?> GetGachaBannerById(int id);

        // Lấy banner gacha kèm danh sách vật phẩm có thể quay.
        Task<GachaBanner?> GetGachaBannerByIdWithItems(int id);

        // Lấy danh sách vật phẩm trong banner.
        Task<List<GachaBannerItem>> GetBannerItems(int bannerId);

        // Lấy danh sách vật phẩm trong banner có phân trang.
        Task<(int TotalCount, List<GachaBannerItem> Items)> GetBannerItemsPaged(int page, int pageSize);

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // Tạo banner gacha mới.
        Task<GachaBanner> CreateGachaBanner(GachaBanner banner);

        // Cập nhật banner gacha.
        Task<GachaBanner> UpdateGachaBanner(GachaBanner banner);

        // Tạo vật phẩm trong banner gacha.
        Task<GachaBannerItem> CreateBannerItem(GachaBannerItem item);

        // Lấy danh sách banner có phân trang, lọc theo tìm kiếm, loại và trạng thái.
        Task<(int TotalCount, List<GachaBanner> Items)> GetBannersPaged(int page, int pageSize, string? search, string? type, bool? isActive);
    }
}
