using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    // Quản lý achievements (thành tựu) cho người chơi và admin.
    // Game APIs: Người chơi xem thành tựu của mình.
    // Admin APIs: Admin tạo, cập nhật thành tựu và xem danh sách.
    [Route("api/[controller]")]
    [ApiController]
    public class AchievementsController : ControllerBase
    {
        private readonly IAchievementService _achievementService;
        public AchievementsController(IAchievementService achievementService)
        {
            _achievementService = achievementService;
        }

        private int GetPlayerProfileId()
        {
            var claim = User.FindFirstValue("playerProfileId");
            if (!int.TryParse(claim, out var id))
                throw new UnauthorizedAccessException("PlayerProfileId is missing from token. Please login again.");
            return id;
        }

        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // ── GET /api/achievements/me ──────────────────────────────────────────
        // Lấy danh sách achievements của player đang đăng nhập.
        // Bao gồm tiến độ và trạng thái hoàn thành.
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyAchievements()
        {
            var profileId = GetPlayerProfileId();
            var result = await _achievementService.GetMeAchievements(profileId);
            return Ok(new ApiResponse<PlayerMeAchievementsResponseDto> { Success = true, Data = result });
        }

        // ── POST /api/achievements/me/{playerAchievementId}/unlock ───────────
        // Kích hoạt thành tích của người chơi đang đăng nhập.
        [Authorize]
        [HttpPost("me/{playerAchievementId}/unlock")]
        public async Task<IActionResult> UnlockAchievement(int playerAchievementId)
        {
            var profileId = GetPlayerProfileId();
            var result = await _achievementService.UnlockAchievement(profileId, playerAchievementId);
            return Ok(new ApiResponse<PlayerAchievementResponseDto> { Success = true, Data = result });
        }

        // ── GET /api/achievements/{id} ─────────────────────────────────────────
        // Lấy chi tiết một achievement theo ID.
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var achievement = await _achievementService.GetAchievementById(id);
            if (achievement == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Achievement with id {id} not found.", ErrorCode = ErrorCodes.NotFound });

            return Ok(new ApiResponse<AchievementResponseDto> { Success = true, Data = achievement });
        }

        // ── GET /api/achievements ──────────────────────────────────────────────
        // Lấy danh sách achievements có phân trang và lọc.
        // Query: page, pageSize, search, type, isActive, sortBy, sortOrder.
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? type = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = null)
        {
            var result = await _achievementService.GetAchievementsPaged(page, pageSize, search, type, isActive, sortBy, sortOrder);
            return Ok(new ApiResponse<PagedResultDto<AchievementResponseDto>> { Success = true, Data = result });
        }

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════
        // NOTE: Create endpoint removed - managed via seeding.

        // ── PUT /api/achievements/{id} ─────────────────────────────────────────
        // Cập nhật achievement hiện có.
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAchievementRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var achievement = await _achievementService.UpdateAchievement(id, request);
            return Ok(new ApiResponse<AchievementResponseDto> { Success = true, Data = achievement });
        }
    }
}
