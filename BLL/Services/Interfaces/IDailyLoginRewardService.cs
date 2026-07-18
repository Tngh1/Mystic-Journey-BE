using BLL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Quản lý thưởng đăng nhập hàng ngày (daily login rewards).
    //
    // 2 loại record:
    //   Default  : Month=null, Year=null  — dùng khi tháng chưa có override
    //   Override : Month=1..12, Year=xxxx — quà riêng cho tháng/năm cụ thể
    //
    // Fallback priority: override(day,month,year) → default(day) → placeholder
    public interface IDailyLoginRewardService
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy danh sách tất cả daily login rewards có phân trang.
        Task<PagedResultDto<DailyLoginRewardResponseDto>> GetDailyLoginRewardsPaged(
            int page, int pageSize, int? month = null, int? year = null);

        // Lấy rewards tháng hiện tại (hoặc tháng cụ thể) với fallback logic.
        // Dùng cho game client: trả về đúng reward cho từng ngày trong tháng.
        Task<List<DailyLoginRewardResponseDto>> GetCurrentMonthRewards(int? month = null, int? year = null);

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy reward theo ID.
        Task<DailyLoginRewardResponseDto?> GetDailyLoginRewardById(int id);

        // Lấy full bộ rewards của một tháng (bao gồm fallback về default).
        // Dùng cho admin FE hiển thị calendar view theo tháng.
        Task<List<DailyLoginRewardResponseDto>> GetRewardsByMonth(int? month, int? year);

        // Tạo reward mới (default hoặc override tháng/năm).
        Task<DailyLoginRewardResponseDto> CreateDailyLoginReward(CreateDailyLoginRewardRequestDto request);

        // Cập nhật reward (không đổi tháng/năm).
        Task<DailyLoginRewardResponseDto> UpdateDailyLoginReward(int id, UpdateDailyLoginRewardRequestDto request);

        // Xóa reward (soft delete).
        Task DeleteDailyLoginReward(int id);
    }
}
