using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/[controller]")]
    // Executes controller base operation.
    [ApiController]
    public class AchievementsController : ControllerBase
    {
        private readonly IAchievementService _achievementService;
        // Initializes a new instance of AchievementsController with dependencies: achievementService.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public AchievementsController(IAchievementService achievementService)
        {
            _achievementService = achievementService;
        }

        // Executes get player profile id operation.
        // Throws an exception if precondition validations fail.
        private int GetPlayerProfileId()
        {
            var claim = User.FindFirstValue("playerProfileId");
            if (!int.TryParse(claim, out var id))  // Claim value missing or non-integer — reject as unauthorized
                throw new UnauthorizedAccessException("PlayerProfileId is missing from token. Please login again.");  // Authentication token is invalid or expired
            return id;
        }


        // ─── Player APIs ───────────────────────────────────────────────────────
        [Authorize]
        [HttpGet("me")]
        // Retrieves achievement progression, unlocked milestones, and stat bonus multipliers for the player.
        public async Task<IActionResult> GetMyAchievements()
        {
            var profileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
            var result = await _achievementService.GetMeAchievements(profileId); // Load user achievement tracker rows and unlocked rewards
            return Ok(new ApiResponse<PlayerMeAchievementsResponseDto> { Success = true, Data = result });  // Return HTTP 200 with standard ApiResponse envelope
        }

        [Authorize]
        [HttpPost("me/{playerAchievementId}/unlock")]
        // Claims/unlocks rewards and applies permanent passive stat buffs for a satisfied achievement.
        public async Task<IActionResult> UnlockAchievement(int playerAchievementId)
        {
            var profileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
            var result = await _achievementService.UnlockAchievement(profileId, playerAchievementId); // Verify completion criteria, grant gem/title rewards, and recalculate permanent stat bonuses
            return Ok(new ApiResponse<PlayerAchievementResponseDto> { Success = true, Data = result });  // Return HTTP 200 with standard ApiResponse envelope
        }

        [Authorize]
        [HttpGet("{id}")]
        // Retrieves specific achievement configuration details.
        public async Task<IActionResult> GetById(int id)
        {
            var achievement = await _achievementService.GetAchievementById(id); // Look up achievement definition by ID
            if (achievement == null)  // Entity not found — short-circuit with appropriate error result
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Achievement with id {id} not found.", ErrorCode = ErrorCodes.NotFound });  // Return HTTP 404 when the requested resource does not exist

            return Ok(new ApiResponse<AchievementResponseDto> { Success = true, Data = achievement });  // Return HTTP 200 with standard ApiResponse envelope
        }

        [HttpGet]
        // Retrieves paginated catalog of all achievements in the game.
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? type = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = null)
        {
            var result = await _achievementService.GetAchievementsPaged(page, pageSize, search, type, isActive, sortBy, sortOrder); // Query achievements database with filters
            return Ok(new ApiResponse<PagedResultDto<AchievementResponseDto>> { Success = true, Data = result });  // Return HTTP 200 with standard ApiResponse envelope
        }


        // ─── Admin APIs ───────────────────────────────────────────────────────
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        // Updates achievement criteria, title, icon, and reward values.
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAchievementRequestDto request)
        {
            if (!ModelState.IsValid)  // Reject request immediately if any DTO validation annotation fails
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });  // Return HTTP 400 with validation error details

            var achievement = await _achievementService.UpdateAchievement(id, request); // Save updated achievement configuration
            return Ok(new ApiResponse<AchievementResponseDto> { Success = true, Data = achievement });  // Return HTTP 200 with standard ApiResponse envelope
        }
    }
}
