using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/[controller]")]
    // Executes controller base operation.
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        // Initializes a new instance of DashboardController with dependencies: dashboardService.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }


        // ─── Admin APIs ───────────────────────────────────────────────────────
        [Authorize(Roles = "Admin")]
        [HttpGet("stats")]
        // Aggregates dashboard analytics: total registered players, active concurrent users, monthly revenue, and guild counts.
        public async Task<IActionResult> GetStats()
        {
            var result = await _dashboardService.GetDashboardStats(); // Query dashboard metrics aggregation
            return Ok(new ApiResponse<DashboardStatsDto> { Success = true, Data = result });
        }
    }
}
