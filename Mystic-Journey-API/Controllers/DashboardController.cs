using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    // Quản lý dashboard (bảng điều khiển) cho admin.
    // Admin APIs: Xem thống kê dashboard.
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // ── GET /api/dashboard/stats ────────────────────────────────
        // Lấy thống kê dashboard (tổng quan hệ thống).
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var result = await _dashboardService.GetDashboardStats();
            return Ok(new ApiResponse<DashboardStatsDto> { Success = true, Data = result });
        }
    }
}
