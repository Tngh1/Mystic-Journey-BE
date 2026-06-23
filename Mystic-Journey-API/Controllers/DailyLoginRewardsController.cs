using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;

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

        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDailyLoginRewardRequestDto dto)
        {
            var result = await _dailyLoginRewardService.CreateDailyLoginReward(dto);
            return Ok(new ApiResponse<DailyLoginRewardResponseDto> { Success = true, Data = result });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _dailyLoginRewardService.GetDailyLoginRewardsPaged(page, pageSize);
            return Ok(new ApiResponse<PagedResultDto<DailyLoginRewardResponseDto>> { Success = true, Data = result });
        }
    }
}
