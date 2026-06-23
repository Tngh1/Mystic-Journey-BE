using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AchievementsController : ControllerBase
    {
        private readonly IAchievementService _achievementService;

        public AchievementsController(IAchievementService achievementService)
        {
            _achievementService = achievementService;
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var achievement = await _achievementService.GetAchievementById(id);
            if (achievement == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Achievement with id {id} not found.", ErrorCode = ErrorCodes.NotFound });

            return Ok(new ApiResponse<AchievementResponseDto> { Success = true, Data = achievement });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAchievementRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var achievement = await _achievementService.CreateAchievement(request);
            return Ok(new ApiResponse<AchievementResponseDto> { Success = true, Data = achievement });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAchievementRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var achievement = await _achievementService.UpdateAchievement(id, request);
            return Ok(new ApiResponse<AchievementResponseDto> { Success = true, Data = achievement });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] string? type = null, [FromQuery] bool? isActive = null)
        {
            var result = await _achievementService.GetAchievementsPaged(page, pageSize, search, type, isActive);
            return Ok(new ApiResponse<PagedResultDto<AchievementResponseDto>> { Success = true, Data = result });
        }
    }
}
