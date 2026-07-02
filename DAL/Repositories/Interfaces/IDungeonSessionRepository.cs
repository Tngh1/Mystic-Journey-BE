using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Quản lý phiên chơi dungeon.
    // Game APIs: Xem, tạo, cập nhật phiên chơi dungeon.
    public interface IDungeonSessionRepository
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy phiên chơi dungeon theo mã, kèm cấu hình, tiến độ và rương.
        Task<DungeonSession?> GetById(int sessionId);

        // Lấy tất cả phiên chơi của người chơi (mới nhất trước).
        Task<List<DungeonSession>> GetByPlayerProfileId(int playerProfileId);

        // Lấy phiên chơi đang hoạt động của người chơi trong dungeon cụ thể.
        // Dùng để ngăn chặn chạy nhiều phiên cùng lúc.
        Task<DungeonSession?> GetActiveSession(int playerProfileId, int? dungeonConfigId = null);

        // Tạo phiên chơi dungeon mới.
        Task<DungeonSession> Create(DungeonSession session);

        // Cập nhật phiên chơi dungeon (trạng thái, thời gian...).
        Task<DungeonSession> Update(DungeonSession session);
    }
}
