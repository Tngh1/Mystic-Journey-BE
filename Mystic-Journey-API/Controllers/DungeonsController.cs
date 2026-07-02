using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    // Quản lý dungeons (danh sach và phó bản) và dungeon session (phiên chơi).
    // Game APIs: Vào dungeon, cập nhật tiến trình, hoàn thành, nhận thưởng.
    // Admin APIs: Tạo, cập nhật, xem danh sách dungeons.
    [Route("api/[controller]")]
    [ApiController]
    public class DungeonsController : ControllerBase
    {
        private readonly IDungeonConfigService _dungeonConfigService;
        private readonly IDungeonSessionService _dungeonSessionService;

        public DungeonsController(
            IDungeonConfigService dungeonConfigService,
            IDungeonSessionService dungeonSessionService)
        {
            _dungeonConfigService = dungeonConfigService;
            _dungeonSessionService = dungeonSessionService;
        }

        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // ── GET /api/dungeons/{id} ───────────────────────────────────────────
        // Lấy thông tin dungeon theo ID.
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var dungeon = await _dungeonConfigService.GetDungeonById(id);
            if (dungeon == null)
            {
                // Fallback: nếu id=1 không tìm thấy, lấy dungeon đầu tiên đang active.
                if (id == 1)
                {
                    var fallback = await _dungeonConfigService.GetDungeonsPaged(1, 1, null, null, true);
                    if (fallback.Items != null && fallback.Items.Any())
                    {
                        return Ok(new ApiResponse<DungeonConfigResponseDto> { Success = true, Data = fallback.Items.First() });
                    }
                }
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Dungeon with id {id} not found.", ErrorCode = ErrorCodes.NotFound });
            }

            return Ok(new ApiResponse<DungeonConfigResponseDto> { Success = true, Data = dungeon });
        }

        // ── GET /api/dungeons ───────────────────────────────────────────────
        // Lấy danh sách tất cả dungeons có phân trang và lọc.
        // Query: page, pageSize, search, type, isActive.
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? type = null,
            [FromQuery] bool? isActive = null)
        {
            var result = await _dungeonConfigService.GetDungeonsPaged(page, pageSize, search, type, isActive);
            return Ok(new ApiResponse<PagedResultDto<DungeonConfigResponseDto>> { Success = true, Data = result });
        }

        // ── POST /api/dungeons/{dungeonId}/enter ──────────────────────────────
        // Vào dungeon. Kiểm tra player và dungeon, tạo phiên chơi mới.
        // Chưa trừ energy - sẽ trừ khi nhận thưởng.
        [Authorize]
        [HttpPost("{dungeonId}/enter")]
        public async Task<IActionResult> Enter(int dungeonId, [FromBody] EnterDungeonRequestDto? request = null)
        {
            try
            {
                var profileId = GetPlayerProfileId();
                var result = await _dungeonSessionService.EnterDungeon(profileId, dungeonId, request?.PartyMembers);
                return Ok(new ApiResponse<EnterDungeonResponseDto>
                {
                    Success = true,
                    Message = "Entered dungeon successfully. Energy will be consumed when you claim your reward.",
                    Data = result
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object> { Success = false, ErrorCode = "NOT_FOUND", Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorCode = "INVALID_OPERATION", Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse<object> { Success = false, ErrorCode = "UNAUTHORIZED", Message = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new ApiResponse<object> { Success = false, ErrorCode = "INTERNAL_ERROR", Message = ex.Message });
            }
        }

        // ── POST /api/dungeons/session/{sessionId}/progress ──────────────────
        // Cập nhật tiến trình chiến đấu (quái đã giết, boss, % hoàn thành).
        [Authorize]
        [HttpPost("session/{sessionId}/progress")]
        public async Task<IActionResult> Progress(int sessionId, [FromBody] UpdateDungeonProgressRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var profileId = GetPlayerProfileId();
                var result = await _dungeonSessionService.UpdateProgress(sessionId, profileId, request);
                return Ok(new ApiResponse<DungeonProgressResponseDto>
                {
                    Success = true,
                    Message = "Progress updated.",
                    Data = result
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object> { Success = false, ErrorCode = "NOT_FOUND", Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorCode = "INVALID_OPERATION", Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse<object> { Success = false, ErrorCode = "UNAUTHORIZED", Message = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new ApiResponse<object> { Success = false, ErrorCode = "INTERNAL_ERROR", Message = ex.Message });
            }
        }

        // ── POST /api/dungeons/session/{sessionId}/complete ──────────────────
        // Hoàn thành dungeon. Kiểm tra boss đã bị đánh bại, trả về preview rương.
        // Chưa cấp thưởng - phải gọi claim-reward sau.
        [Authorize]
        [HttpPost("session/{sessionId}/complete")]
        public async Task<IActionResult> Complete(int sessionId)
        {
            try
            {
                var profileId = GetPlayerProfileId();
                var result = await _dungeonSessionService.CompleteSession(sessionId, profileId);
                return Ok(new ApiResponse<CompleteDungeonResponseDto>
                {
                    Success = true,
                    Message = result.Message,
                    Data = result
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object> { Success = false, ErrorCode = "NOT_FOUND", Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorCode = "INVALID_OPERATION", Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse<object> { Success = false, ErrorCode = "UNAUTHORIZED", Message = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new ApiResponse<object> { Success = false, ErrorCode = "INTERNAL_ERROR", Message = ex.Message });
            }
        }

        // ── POST /api/dungeons/session/{sessionId}/claim-reward ───────────────
        // Nhận thưởng dungeon. Kiểm tra session đã hoàn thành và chưa nhận.
        // Trừ energy, tạo thưởng, lưu inventory (transactional - rollback nếu lỗi).
        [Authorize]
        [HttpPost("session/{sessionId}/claim-reward")]
        public async Task<IActionResult> ClaimReward(int sessionId)
        {
            try
            {
                var profileId = GetPlayerProfileId();
                var result = await _dungeonSessionService.ClaimReward(sessionId, profileId);
                return Ok(new ApiResponse<ClaimDungeonRewardResponseDto>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = result
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object> { Success = false, ErrorCode = "NOT_FOUND", Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.StartsWith("CONFLICT:"))
                {
                    return Conflict(new ApiResponse<object> { Success = false, ErrorCode = "CONFLICT", Message = ex.Message.Replace("CONFLICT: ", "") });
                }
                return BadRequest(new ApiResponse<object> { Success = false, ErrorCode = "INVALID_OPERATION", Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse<object> { Success = false, ErrorCode = "UNAUTHORIZED", Message = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new ApiResponse<object> { Success = false, ErrorCode = "INTERNAL_ERROR", Message = ex.Message });
            }
        }

        // ── POST /api/dungeons/session/{sessionId}/abandon ────────────────────
        // Hủy dungeon session.
        [Authorize]
        [HttpPost("session/{sessionId}/abandon")]
        public async Task<IActionResult> Abandon(int sessionId)
        {
            try
            {
                var profileId = GetPlayerProfileId();
                await _dungeonSessionService.AbandonSession(sessionId, profileId);
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Dungeon session abandoned.",
                    Data = null
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object> { Success = false, ErrorCode = "NOT_FOUND", Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorCode = "INVALID_OPERATION", Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse<object> { Success = false, ErrorCode = "UNAUTHORIZED", Message = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new ApiResponse<object> { Success = false, ErrorCode = "INTERNAL_ERROR", Message = ex.Message });
            }
        }

        // ── GET /api/dungeons/session/active ──────────────────────────────────
        // Lấy dungeon session đang active (Resume).
        [Authorize]
        [HttpGet("session/active")]
        public async Task<IActionResult> GetActiveSession()
        {
            try
            {
                var profileId = GetPlayerProfileId();
                var result = await _dungeonSessionService.GetActiveSession(profileId);
                if (result == null)
                    return Ok(new ApiResponse<object> { Success = true, Message = "No active session found.", Data = null });

                return Ok(new ApiResponse<EnterDungeonResponseDto>
                {
                    Success = true,
                    Message = "Active session retrieved.",
                    Data = result
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new ApiResponse<object> { Success = false, ErrorCode = "INTERNAL_ERROR", Message = ex.Message });
            }
        }

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // ── POST /api/dungeons ───────────────────────────────────────────────
        // Tạo dungeon mới.
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDungeonConfigRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var dungeon = await _dungeonConfigService.CreateDungeon(request);
            return Ok(new ApiResponse<DungeonConfigResponseDto> { Success = true, Data = dungeon });
        }

        // ── PUT /api/dungeons/{id} ───────────────────────────────────────────
        // Cập nhật dungeon hiện có.
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDungeonConfigRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var dungeon = await _dungeonConfigService.UpdateDungeon(id, request);
            return Ok(new ApiResponse<DungeonConfigResponseDto> { Success = true, Data = dungeon });
        }

        // ── Helper ────────────────────────────────────────────────────────────
        // Đọc playerProfileId từ JWT token.
        private int GetPlayerProfileId()
        {
            var claim = User.FindFirstValue("playerProfileId");
            if (!int.TryParse(claim, out var id))
                throw new UnauthorizedAccessException("PlayerProfileId is missing from token. Please login again.");
            return id;
        }

        // ── Lấy lịch sử dungeon ───────────────────────────────────────────────────────
        [Authorize]
        [HttpGet("history")]
        public async Task<IActionResult> GetHistory()
        {
            try
            {
                var profileId = GetPlayerProfileId();
                var result = await _dungeonSessionService.GetHistory(profileId);
                return Ok(new ApiResponse<List<DungeonHistoryResponseDto>>
                {
                    Success = true,
                    Message = "Dungeon history retrieved successfully.",
                    Data = result
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new ApiResponse<object> { Success = false, ErrorCode = "INTERNAL_ERROR", Message = ex.Message });
            }
        }
    }
}
