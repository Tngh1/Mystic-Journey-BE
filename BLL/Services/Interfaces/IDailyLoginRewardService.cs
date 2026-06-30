using BLL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Quản lý thưởng đăng nhập hàng ngày (daily login rewards).
    // Game APIs: Xem danh sách rewards.
    // Admin APIs: Tạo reward mới.
    public interface IDailyLoginRewardService
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy danh sách tất cả daily login rewards có phân trang.
        Task<PagedResultDto<DailyLoginRewardResponseDto>> GetDailyLoginRewardsPaged(int page, int pageSize);

        // Lấy danh sách rewards cho tháng hiện tại.
        // Ngày chưa có reward sẽ có IsActive=false (placeholder).
        Task<List<DailyLoginRewardResponseDto>> GetCurrentMonthRewards();

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // Tạo daily login reward mới.
        Task<DailyLoginRewardResponseDto> CreateDailyLoginReward(CreateDailyLoginRewardRequestDto request);
    }
}
