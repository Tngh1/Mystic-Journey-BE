using BLL.DTOs;
using DAL.Models;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Quản lý player profile (hồ sơ người chơi).
    // Game APIs: Xem, cập nhật profile và xem bạn bè.
    // Admin APIs: Xem danh sách tất cả player profiles.
    public interface IPlayerProfileService
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy chi tiết player profile theo ID.
        Task<PlayerProfileDetailResponseDto?> GetProfileById(int id);

        // Cập nhật thông tin player profile (display name, avatar...).
        Task<PlayerProfileResponseDto> UpdateProfile(int id, UpdatePlayerProfileRequestDto request);

        // Lấy danh sách bạn bè của player đang đăng nhập.
        Task<List<PlayerProfileResponseDto>> GetFriends(int playerProfileId);

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy danh sách tất cả player profiles có phân trang và lọc.
        Task<PagedResultDto<PlayerProfileResponseDto>> GetProfilesPaged(int page, int pageSize, string? search, int? level);

        // Tính toán lại năng lượng hiện tại của player (dựa trên thời gian).
        bool RecalculateEnergy(PlayerProfile profile);
    }
}
