using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Quản lý phần thưởng đăng nhập hàng ngày.
    // Hỗ trợ 2 loại record:
    //   Default  : Month=null, Year=null  — áp dụng mọi tháng nếu chưa có override
    //   Override : Month=1..12, Year=xxxx — quà riêng tháng/năm cụ thể
    public interface IDailyLoginRewardRepository
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy phần thưởng theo mã.
        Task<DailyLoginReward?> GetDailyLoginRewardById(int id);

        // Lấy override cho ngày + tháng + năm cụ thể (không fallback).
        Task<DailyLoginReward?> GetByDayAndMonth(int dayNumber, int month, int year);

        // Lấy default cho ngày (Month=null, Year=null).
        Task<DailyLoginReward?> GetDefaultByDayNumber(int dayNumber);

        // Lấy tất cả override của một tháng/năm.
        Task<List<DailyLoginReward>> GetOverridesByMonth(int month, int year);

        // Lấy tất cả default (Month=null, Year=null).
        Task<List<DailyLoginReward>> GetAllDefaults();

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy danh sách có phân trang, lọc theo month/year (null = defaults).
        Task<(int TotalCount, List<DailyLoginReward> Items)> GetDailyLoginRewardsPaged(
            int page, int pageSize, int? month = null, int? year = null);

        // Tạo phần thưởng mới.
        Task<DailyLoginReward> CreateDailyLoginReward(DailyLoginReward reward);

        // Cập nhật phần thưởng.
        Task<DailyLoginReward> UpdateDailyLoginReward(DailyLoginReward reward);

        // Xóa phần thưởng (soft delete).
        Task DeleteDailyLoginReward(int id);
    }
}
