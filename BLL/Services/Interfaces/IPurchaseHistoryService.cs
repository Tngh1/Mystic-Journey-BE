using BLL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Quản lý lịch sử mua hàng (purchase histories).
    // Game APIs: Xem lịch sử mua của player.
    // Admin APIs: Xem tất cả lịch sử mua.
    public interface IPurchaseHistoryService
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy lịch sử mua của player.
        Task<List<PurchaseHistoryResponseDto>> GetPurchasesByPlayerId(int playerProfileId);

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy tất cả lịch sử mua có phân trang và lọc.
        Task<PagedResultDto<PurchaseHistoryResponseDto>> GetPurchaseHistoriesPaged(int page, int pageSize, string? search = null, string? sortBy = null, string? sortOrder = null);

        // Lấy tất cả lịch sử mua.
        Task<List<PurchaseHistoryResponseDto>> GetAllPurchaseHistories();
    }
}
