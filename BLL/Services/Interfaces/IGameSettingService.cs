using BLL.DTOs;
using System;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Quản lý cài đặt game (game settings).
    // Game APIs: Xem danh sách, xem theo ID, xem theo key.
    // Admin APIs: Cập nhật setting.
    public interface IGameSettingService
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy game setting theo ID.
        Task<GameSettingResponseDto?> GetSettingById(int id);

        // Lấy game setting theo key.
        Task<GameSettingResponseDto?> GetSettingByKey(string key);

        // Lấy danh sách tất cả game settings có phân trang.
        Task<PagedResultDto<GameSettingResponseDto>> GetSettingsPaged(int page, int pageSize, string? search);

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // Cập nhật game setting theo key.
        Task<GameSettingResponseDto> UpdateSetting(string key, UpdateGameSettingRequestDto request, Guid? updatedByAccountId = null);
    }
}
