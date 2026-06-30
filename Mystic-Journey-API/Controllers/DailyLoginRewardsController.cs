using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;

namespace Mystic_Journey_API.Controllers
{
    // Quản lý thưởng đăng nhập hàng ngày (daily login rewards).
    // Game APIs: Xem danh sách rewards.
    // Admin APIs: Tạo reward mới.
    [Route("api/[controller]")]
    [ApiController]
    public class DailyLoginRewardsController : ControllerBase
    {
        private readonly IDailyLoginRewardService _dailyLoginRewardService;

        public DailyLoginRewardsController(IDailyLoginRewardService dailyLoginRewardService)
        {
            _dailyLoginRewardService = dailyLoginRewardService;
        }

        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // ── GET /api/dailyloginrewards ────────────────────────────────
        // Lấy danh sách tất cả daily login rewards có phân trang.
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _dailyLoginRewardService.GetDailyLoginRewardsPaged(page, pageSize);
            return Ok(new ApiResponse<PagedResultDto<DailyLoginRewardResponseDto>> { Success = true, Data = result });
        }

        // ── GET /api/dailyloginrewards/current-month ─────────────────
        // Lấy danh sách rewards cho tháng hiện tại.
        // Ngày chưa có reward sẽ có IsActive=false (placeholder).
        [HttpGet("current-month")]
        public async Task<IActionResult> GetCurrentMonth()
        {
            var result = await _dailyLoginRewardService.GetCurrentMonthRewards();
            return Ok(new ApiResponse<List<DailyLoginRewardResponseDto>> { Success = true, Data = result });
        }

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // ── POST /api/dailyloginrewards ──────────────────────────────
        // Tạo daily login reward mới.
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDailyLoginRewardRequestDto dto)
        {
            var result = await _dailyLoginRewardService.CreateDailyLoginReward(dto);
            return Ok(new ApiResponse<DailyLoginRewardResponseDto> { Success = true, Data = result });
        }
    }
}
