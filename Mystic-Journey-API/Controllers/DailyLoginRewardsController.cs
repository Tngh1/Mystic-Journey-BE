using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DailyLoginRewardsController : ControllerBase
    {
        private readonly IDailyLoginRewardService _dailyLoginRewardService;

        public DailyLoginRewardsController(IDailyLoginRewardService dailyLoginRewardService)
        {
            _dailyLoginRewardService = dailyLoginRewardService;
        }

        // ========== MANAGER: Daily Login Reward Management (Dashboard) ==========
        // Dành cho Admin/Manager - Quản lý phần thưởng đăng nhập hàng ngày trên dashboard

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDailyLoginRewardRequestDto dto)
        {
            try
            {
                var result = await _dailyLoginRewardService.CreateDailyLoginReward(dto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // ========== PLAYER: Browse Daily Login Rewards ==========
        // Dành cho người chơi - Xem danh sách phần thưởng đăng nhập hàng ngày

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _dailyLoginRewardService.GetDailyLoginRewardsPaged(page, pageSize);
            return Ok(result);
        }
    }
}
