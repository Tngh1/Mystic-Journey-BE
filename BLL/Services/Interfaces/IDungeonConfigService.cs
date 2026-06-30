using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Quản lý cấu hình dungeons (danh sách phó bản).
    // Admin APIs: Tạo, cập nhật và xem danh sách dungeons.
    public interface IDungeonConfigService
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy thông tin dungeon theo ID.
        Task<DungeonConfigResponseDto?> GetDungeonById(int id);

        // Lấy danh sách tất cả dungeons có phân trang và lọc.
        Task<PagedResultDto<DungeonConfigResponseDto>> GetDungeonsPaged(int page, int pageSize, string? search, string? type, bool? isActive);

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // Tạo dungeon mới.
        Task<DungeonConfigResponseDto> CreateDungeon(CreateDungeonConfigRequestDto request);

        // Cập nhật dungeon hiện có.
        Task<DungeonConfigResponseDto> UpdateDungeon(int id, UpdateDungeonConfigRequestDto request);
    }
}
