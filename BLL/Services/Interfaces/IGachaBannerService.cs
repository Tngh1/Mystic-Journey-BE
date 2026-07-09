using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Quản lý gacha banners (banner gacha/quay thưởng).
    // Game APIs: Xem danh sách, xem chi tiết banner.
    // Admin APIs: Tạo, cập nhật banner và thêm items.
    public interface IGachaBannerService
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy chi tiết gacha banner theo ID.
        Task<GachaBannerDetailResponseDto?> GetBannerById(int id);

        // Lấy danh sách tất cả gacha banners có phân trang và lọc.
        Task<PagedResultDto<GachaBannerResponseDto>> GetBannersPaged(int page, int pageSize, string? search, string? type, bool? isActive, string? sortBy = null, string? sortOrder = null);

        // Lấy danh sách banner items có phân trang.
        Task<PagedResultDto<GachaBannerItemResponseDto>> GetBannerItemsPaged(int page, int pageSize);

        // Quay gacha
        Task<MultiPullResultDto> Pull(int playerProfileId, int bannerId, GachaPullRequestDto request);

        // Lấy lịch sử quay
        Task<PagedResultDto<GachaPullHistoryResponseDto>> GetHistoryPaged(int playerProfileId, int page, int pageSize);

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // Cập nhật gacha banner hiện có.
        Task<GachaBannerResponseDto> UpdateBanner(int id, UpdateGachaBannerRequestDto request);

        // Thêm item vào banner.
        Task<GachaBannerItemResponseDto> AddBannerItem(int bannerId, CreateGachaBannerItemRequestDto request);
    }
}
