using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Quản lý quests (nhiệm vụ) cho admin.
    // Admin APIs: Tạo, cập nhật và xem danh sách quests.
    public interface IQuestService
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy chi tiết quest theo ID.
        Task<QuestResponseDto?> GetQuestById(int id);

        // Lấy danh sách tất cả quests có phân trang và lọc.
        Task<PagedResultDto<QuestResponseDto>> GetQuestsPaged(int page, int pageSize, string? search, string? type, bool? isActive, string? mapName, string? sortBy = null, string? sortOrder = null);

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // Cập nhật quest hiện có.
        Task<QuestResponseDto> UpdateQuest(int id, UpdateQuestRequestDto request);
    }
}
