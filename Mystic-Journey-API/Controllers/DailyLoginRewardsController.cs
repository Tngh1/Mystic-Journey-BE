using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    // Quản lý thưởng đăng nhập hàng ngày (daily login rewards).
    //
    // GAME APIs  : Xem danh sách rewards (không cần auth).
    // ADMIN APIs : Tạo, cập nhật, xóa reward (cần Admin/SuperAdmin).
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

        // ── GET /api/dailyloginrewards ───────────────────────────────────────
        // Lấy danh sách daily login rewards có phân trang.
        // Query: page, pageSize, month?, year?  (null → lấy defaults)
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 31,
            [FromQuery] int? month = null,
            [FromQuery] int? year = null)
        {
            var result = await _dailyLoginRewardService.GetDailyLoginRewardsPaged(page, pageSize, month, year);
            return Ok(new ApiResponse<PagedResultDto<DailyLoginRewardResponseDto>> { Success = true, Data = result });
        }

        // ── GET /api/dailyloginrewards/current-month ─────────────────────────
        // Lấy bộ rewards cho tháng hiện tại với fallback logic.
        // Dùng bởi Unity game client — KHÔNG thay đổi endpoint này.
        [HttpGet("current-month")]
        public async Task<IActionResult> GetCurrentMonth(
            [FromQuery] int? month = null,
            [FromQuery] int? year = null)
        {
            var result = await _dailyLoginRewardService.GetCurrentMonthRewards(month, year);
            return Ok(new ApiResponse<List<DailyLoginRewardResponseDto>> { Success = true, Data = result });
        }

        // ── GET /api/dailyloginrewards/by-month ──────────────────────────────
        // Lấy bộ rewards cho 1 tháng dùng cho admin FE (calendar view).
        // month=null / year=null → trả bộ Default (31 ngày).
        // month+year có giá trị → trả overrides + fallback default.
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet("by-month")]
        public async Task<IActionResult> GetByMonth(
            [FromQuery] int? month = null,
            [FromQuery] int? year = null)
        {
            var result = await _dailyLoginRewardService.GetRewardsByMonth(month, year);
            return Ok(new ApiResponse<List<DailyLoginRewardResponseDto>> { Success = true, Data = result });
        }

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // ── GET /api/dailyloginrewards/{id} ──────────────────────────────────
        // Lấy reward theo ID.
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _dailyLoginRewardService.GetDailyLoginRewardById(id);
            if (result == null)
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Daily login reward with ID {id} not found.",
                    ErrorCode = ErrorCodes.NotFound
                });

            return Ok(new ApiResponse<DailyLoginRewardResponseDto> { Success = true, Data = result });
        }

        // ── POST /api/dailyloginrewards ──────────────────────────────────────
        // Tạo daily login reward mới.
        // Nếu Month=null/Year=null → tạo default.
        // Nếu Month+Year có giá trị → tạo override tháng đó.
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDailyLoginRewardRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    ErrorCode = ErrorCodes.ValidationError
                });

            try
            {
                var result = await _dailyLoginRewardService.CreateDailyLoginReward(dto);
                return Ok(new ApiResponse<DailyLoginRewardResponseDto> { Success = true, Data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message,
                    ErrorCode = "DUPLICATE_REWARD"
                });
            }
        }

        // ── PUT /api/dailyloginrewards/{id} ──────────────────────────────────
        // Cập nhật reward (chỉ nội dung, không đổi DayNumber/Month/Year).
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDailyLoginRewardRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    ErrorCode = ErrorCodes.ValidationError
                });

            try
            {
                var result = await _dailyLoginRewardService.UpdateDailyLoginReward(id, dto);
                return Ok(new ApiResponse<DailyLoginRewardResponseDto> { Success = true, Data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message,
                    ErrorCode = ErrorCodes.NotFound
                });
            }
        }

        // ── DELETE /api/dailyloginrewards/{id} ───────────────────────────────
        // Xóa (soft delete) daily login reward.
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _dailyLoginRewardService.DeleteDailyLoginReward(id);
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Reward deleted successfully."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message,
                    ErrorCode = ErrorCodes.NotFound
                });
            }
        }
    }
}
