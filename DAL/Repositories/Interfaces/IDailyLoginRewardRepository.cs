using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Quản lý phần thưởng đăng nhập hàng ngày.
    // Game APIs: Xem phần thưởng đăng nhập.
    // Admin APIs: Tạo, cập nhật phần thưởng.
    public interface IDailyLoginRewardRepository
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy phần thưởng đăng nhập theo mã.
        Task<DailyLoginReward?> GetDailyLoginRewardById(int id);

        // Lấy phần thưởng đăng nhập theo số ngày.
        Task<DailyLoginReward?> GetDailyLoginRewardByDayNumber(int dayNumber);

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy danh sách phần thưởng đăng nhập có phân trang.
        Task<(int TotalCount, List<DailyLoginReward> Items)> GetDailyLoginRewardsPaged(int page, int pageSize);

        // Tạo phần thưởng đăng nhập mới.
        Task<DailyLoginReward> CreateDailyLoginReward(DailyLoginReward reward);

        // Cập nhật phần thưởng đăng nhập.
        Task<DailyLoginReward> UpdateDailyLoginReward(DailyLoginReward reward);
    }
}
