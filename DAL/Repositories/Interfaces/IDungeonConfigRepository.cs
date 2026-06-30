using DAL.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Quản lý cấu hình dungeon.
    // Game APIs: Xem cấu hình dungeon.
    // Admin APIs: Tạo, cập nhật cấu hình dungeon.
    public interface IDungeonConfigRepository
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy cấu hình dungeon theo mã định danh.
        Task<DungeonConfig?> GetDungeonConfigById(int id);

        // Lấy cấu hình dungeon kèm rương và vật phẩm trong rương.
        // Dùng để kiểm tra năng lượng và xem trước phần thưởng.
        Task<DungeonConfig?> GetByIdWithChest(int id);

        // Lấy tất cả cấu hình dungeon.
        Task<List<DungeonConfig>> GetAllDungeonConfigs();

        // Lấy các dungeon đang hoạt động.
        Task<List<DungeonConfig>> GetActiveDungeonConfigs();

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // Tạo cấu hình dungeon mới.
        Task<DungeonConfig> CreateDungeonConfig(DungeonConfig dungeon);

        // Cập nhật cấu hình dungeon.
        Task<DungeonConfig> UpdateDungeonConfig(DungeonConfig dungeon);

        // Kiểm tra dungeon có tồn tại hay không.
        Task<bool> DungeonExists(int dungeonId);

        // Lấy danh sách dungeon có phân trang, lọc theo tìm kiếm, loại và trạng thái.
        Task<(int TotalCount, List<DungeonConfig> Items)> GetDungeonsPaged(int page, int pageSize, string? search, string? type, bool? isActive);
    }
}
