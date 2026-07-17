using DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Quản lý thành tích (achievement) trong game.
    // Game APIs: Xem thành tích.
    // Admin APIs: Tạo, cập nhật thành tích.
    public interface IAchievementRepository
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy thành tích theo mã định danh.
        Task<Achievement?> GetAchievementById(int id);

        // Lấy thành tích kèm vật phẩm thưởng.
        Task<Achievement?> GetAchievementByIdWithReward(int id);

        // Lấy toàn bộ thành tích đang hoạt động
        Task<List<Achievement>> GetAllActiveAchievements();

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // Cập nhật thông tin thành tích.
        Task<Achievement> UpdateAchievement(Achievement achievement);

        // Lấy danh sách thành tích có phân trang, lọc theo tìm kiếm, loại và trạng thái.
        Task<(int TotalCount, List<Achievement> Items)> GetAchievementsPaged(int page, int pageSize, string? search, string? type, bool? isActive, string? sortBy = null, string? sortOrder = null);
    }
}
