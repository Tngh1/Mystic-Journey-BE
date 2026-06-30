using DAL.Models;

namespace DAL.Repositories.Interfaces
{
    // Quản lý tiến độ dungeon.
    // Game APIs: Xem, tạo, cập nhật tiến độ dungeon.
    public interface IDungeonProgressRepository
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy bản ghi tiến độ của một phiên chơi, trả về null nếu chưa tồn tại.
        Task<DungeonProgress?> GetBySessionId(int sessionId);

        // Tạo bản ghi tiến độ dungeon mới.
        Task<DungeonProgress> Create(DungeonProgress progress);

        // Cập nhật tiến độ dungeon.
        Task<DungeonProgress> Update(DungeonProgress progress);
    }
}
