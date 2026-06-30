using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Quản lý dashboard (bảng điều khiển) cho admin.
    // Admin APIs: Xem thống kê dashboard.
    public interface IDashboardService
    {
        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy thống kê dashboard (tổng quan hệ thống).
        Task<DashboardStatsDto> GetDashboardStats();
    }
}
