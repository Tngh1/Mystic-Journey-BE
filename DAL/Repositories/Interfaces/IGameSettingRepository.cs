using DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Quản lý cấu hình game (game settings).
    // Game APIs: Xem cấu hình game.
    // Admin APIs: Cập nhật cấu hình game.
    public interface IGameSettingRepository
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy cấu hình game theo tên.
        Task<GameSetting?> GetByName(string name);

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy cấu hình game theo mã.
        Task<GameSetting?> GetGameSettingById(int id);

        // Cập nhật cấu hình game.
        Task<GameSetting> UpdateGameSetting(GameSetting setting);

        // Lấy danh sách cấu hình có phân trang, lọc theo tìm kiếm.
        Task<(int TotalCount, List<GameSetting> Items)> GetSettingsPaged(int page, int pageSize, string? search);
    }
}
