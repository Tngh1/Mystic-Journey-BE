using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // ========== MANAGER: Dashboard Statistics (Dashboard) ==========
        // Dành cho Admin/Manager - Xem thống kê dashboard

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            try
            {
                var result = await _dashboardService.GetDashboardStats();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
