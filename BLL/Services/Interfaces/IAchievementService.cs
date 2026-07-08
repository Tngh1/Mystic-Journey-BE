using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Quản lý achievements (thành tựu) cho người chơi và admin.
    // Game APIs: Người chơi xem thành tựu của mình.
    // Admin APIs: Admin tạo, cập nhật thành tựu và xem danh sách.
    public interface IAchievementService
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy danh sách achievements của player đang đăng nhập (bao gồm tiến độ và trạng thái hoàn thành).
        Task<PlayerMeAchievementsResponseDto> GetMeAchievements(int playerProfileId);

        // Kích hoạt một achievement cho player đang đăng nhập.
        Task<PlayerAchievementResponseDto> UnlockAchievement(int playerProfileId, int playerAchievementId);

        // Lấy chi tiết một achievement theo ID.
        Task<AchievementResponseDto?> GetAchievementById(int id);

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy danh sách tất cả achievements có phân trang và lọc.
        Task<PagedResultDto<AchievementResponseDto>> GetAchievementsPaged(int page, int pageSize, string? search, string? type, bool? isActive);

        // Tạo achievement mới.
        Task<AchievementResponseDto> CreateAchievement(CreateAchievementRequestDto request);

        // Cập nhật achievement hiện có.
        Task<AchievementResponseDto> UpdateAchievement(int id, UpdateAchievementRequestDto request);
    }
}
