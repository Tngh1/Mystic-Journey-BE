using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Mystic_Journey_API.Extensions;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
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

        // ── Existing Admin / Read Endpoints ─────────────────────────────────────────

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var dungeon = await _dungeonConfigService.GetDungeonById(id);
            if (dungeon == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Dungeon with id {id} not found.", ErrorCode = ErrorCodes.NotFound });

            return Ok(new ApiResponse<DungeonConfigResponseDto> { Success = true, Data = dungeon });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDungeonConfigRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var dungeon = await _dungeonConfigService.CreateDungeon(request);
            return Ok(new ApiResponse<DungeonConfigResponseDto> { Success = true, Data = dungeon });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDungeonConfigRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var dungeon = await _dungeonConfigService.UpdateDungeon(id, request);
            return Ok(new ApiResponse<DungeonConfigResponseDto> { Success = true, Data = dungeon });
        }

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

        // ── Dungeon Session Endpoints ─────────────────────────────────────────────────

        /// <summary>
        /// POST /api/dungeons/{dungeonId}/enter
        /// Validates player + dungeon, checks energy (does NOT consume it), creates a session.
        /// BR-01 to BR-05.
        /// </summary>
        [Authorize]
        [HttpPost("{dungeonId}/enter")]
        public async Task<IActionResult> Enter(int dungeonId)
        {
            try
            {
                var profileId = GetPlayerProfileId();
                var result = await _dungeonSessionService.EnterDungeon(profileId, dungeonId);
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

        /// <summary>
        /// POST /api/dungeons/session/{sessionId}/progress
        /// Updates combat progress (monsters killed, boss killed, completion %).
        /// BR-06, BR-07.
        /// </summary>
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

        /// <summary>
        /// POST /api/dungeons/session/{sessionId}/complete
        /// Validates boss is defeated, marks session Completed, returns chest preview.
        /// Energy and rewards are NOT granted yet (BR-08, BR-09).
        /// </summary>
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

        /// <summary>
        /// POST /api/dungeons/session/{sessionId}/claim-reward
        /// Validates session is completed + unclaimed + player still has energy.
        /// Transactionally: consumes energy, rolls + saves rewards, marks session RewardClaimed.
        /// BR-10. Full rollback on any failure.
        /// </summary>
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

        // ── Helper ────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Reads the playerProfileId custom claim embedded in the JWT by AccountService.
        /// Throws UnauthorizedAccessException if the claim is missing or invalid.
        /// </summary>
        private int GetPlayerProfileId()
        {
            var claim = User.FindFirstValue("playerProfileId");
            if (!int.TryParse(claim, out var id))
                throw new UnauthorizedAccessException("PlayerProfileId is missing from token. Please login again.");
            return id;
        }
    }
}
