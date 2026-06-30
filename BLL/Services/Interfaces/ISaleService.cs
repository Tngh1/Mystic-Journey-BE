using BLL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Quản lý lịch sử bán (sales).
    // Game APIs: Xem lịch sử bán của player.
    public interface ISaleService
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy lịch sử bán của player.
        Task<List<PurchaseHistoryResponseDto>> GetSalesByPlayerId(int playerProfileId);
    }
}
