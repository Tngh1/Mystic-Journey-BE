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
    public class DungeonsController : ControllerBase
    {
        private readonly IDungeonConfigService _dungeonConfigService;
        private readonly IDungeonSessionService _dungeonSessionService;
        private readonly IMonsterService _monsterService;

        // Initialize this instance from dungeon config service, dungeon session service, and monster service and store dungeon config service, dungeon session service, and monster service for later operations.
        public DungeonsController(
            IDungeonConfigService dungeonConfigService,
            IDungeonSessionService dungeonSessionService,
            IMonsterService monsterService)
        {
            _dungeonConfigService = dungeonConfigService;
            _dungeonSessionService = dungeonSessionService;
            _monsterService = monsterService;
        }


        // ─── Player APIs ───────────────────────────────────────────────────────
        [Authorize]
        [HttpGet("{id}")]
        // Retrieves detailed dungeon configuration (wave counts, monsters, rewards, energy cost).
        public async Task<IActionResult> GetById(int id)
        {
            var dungeon = await _dungeonConfigService.GetDungeonById(id); // Look up dungeon config by primary key
            if (dungeon == null)
            {
                if (id == 1) // Fallback for initial tutorial/story dungeon if database ID sequence differs
                {
                    var fallback = await _dungeonConfigService.GetDungeonsPaged(1, 1, null, null, true);
                    if (fallback.Items != null && fallback.Items.Any())
                    {
                        return Ok(new ApiResponse<DungeonConfigResponseDto> { Success = true, Data = fallback.Items.First() }); // Return first active dungeon as fallback
                    }
                }
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Dungeon with id {id} not found.", ErrorCode = ErrorCodes.NotFound }); // Return HTTP 404 when dungeon does not exist
            }

            return Ok(new ApiResponse<DungeonConfigResponseDto> { Success = true, Data = dungeon }); // Return HTTP 200 with dungeon configuration
        }

        [HttpGet]
        // Retrieves paginated list of all configured dungeons with optional filtering by type and active status.
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? type = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = null)
        {
            var result = await _dungeonConfigService.GetDungeonsPaged(page, pageSize, search, type, isActive, sortBy, sortOrder); // Query dungeons database with pagination and filters
            return Ok(new ApiResponse<PagedResultDto<DungeonConfigResponseDto>> { Success = true, Data = result }); // Return HTTP 200 with paginated dungeon list
        }

        [Authorize]
        [HttpPost("{dungeonId}/enter")]
        // Starts a new dungeon session for solo or party play, creating an active session tracker.
        public async Task<IActionResult> Enter(int dungeonId, [FromBody] EnterDungeonRequestDto? request = null)
        {
            try
            {
                var profileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
                var result = await _dungeonSessionService.EnterDungeon(profileId, dungeonId, request?.PartyMembers); // Create active dungeon run session without deducting energy upfront
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

        [Authorize]
        [HttpPost("session/{sessionId}/progress")]
        // Reports live dungeon progress (monsters defeated, current wave number, elapsed time).
        public async Task<IActionResult> Progress(int sessionId, [FromBody] UpdateDungeonProgressRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var profileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
                var result = await _dungeonSessionService.UpdateProgress(sessionId, profileId, request); // Update wave/kill counts in active session record
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

        [Authorize]
        [HttpPost("session/{sessionId}/complete")]
        // Finalizes a successful dungeon clear, computes clear rank/score, and unlocks reward claim eligibility.
        public async Task<IActionResult> Complete(int sessionId)
        {
            try
            {
                var profileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
                var result = await _dungeonSessionService.CompleteSession(sessionId, profileId); // Mark session Completed, record clear time, and calculate reward tier
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

        [Authorize]
        [HttpPost("session/{sessionId}/claim-reward")]
        // Deducts energy, rolls dungeon loot drops/currency/EXP, and deposits rewards into player inventory.
        public async Task<IActionResult> ClaimReward(int sessionId)
        {
            try
            {
                var profileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
                var result = await _dungeonSessionService.ClaimReward(sessionId, profileId); // Atomic transaction: verify energy, deduct energy, grant items/gold/exp, mark claimed
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

        [Authorize]
        [HttpPost("session/{sessionId}/abandon")]
        // Abandons an in-progress dungeon session without consuming energy or granting rewards.
        public async Task<IActionResult> Abandon(int sessionId)
        {
            try
            {
                var profileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
                await _dungeonSessionService.AbandonSession(sessionId, profileId); // Mark session Abandoned to free up player's active session slot
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

        [Authorize]
        [HttpGet("session/active")]
        // Executes get active session operation.
        public async Task<IActionResult> GetActiveSession()
        {
            try
            {
                var profileId = GetPlayerProfileId();
                var result = await _dungeonSessionService.GetActiveSession(profileId);
                if (result == null)  // Entity not found — short-circuit with appropriate error result
                    return Ok(new ApiResponse<object> { Success = true, Message = "No active session found.", Data = null });  // Return HTTP 200 with standard ApiResponse envelope

                return Ok(new ApiResponse<EnterDungeonResponseDto>  // Return HTTP 200 with standard ApiResponse envelope
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


        // ─── Admin APIs ───────────────────────────────────────────────────────
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        // Per-frame update loop for DungeonsController.
        // Handles real-time input polling, smooth interpolations, cooldown timers, and UI updates.
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDungeonConfigRequestDto request)
        {
            if (!ModelState.IsValid)  // Reject request immediately if any DTO validation annotation fails
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });  // Return HTTP 400 with validation error details

            var dungeon = await _dungeonConfigService.UpdateDungeon(id, request);
            return Ok(new ApiResponse<DungeonConfigResponseDto> { Success = true, Data = dungeon });  // Return HTTP 200 with standard ApiResponse envelope
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}/spawns")]
        // Executes get spawns by dungeon operation.
        public async Task<IActionResult> GetSpawnsByDungeon(int id)
        {
            var spawns = await _monsterService.GetSpawnsByDungeonId(id);
            return Ok(new ApiResponse<List<MonsterSpawnResponseDto>> { Success = true, Data = spawns });  // Return HTTP 200 with standard ApiResponse envelope
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/chest-items")]
        // Executes add chest item operation.
        public async Task<IActionResult> AddChestItem(int id, [FromBody] CreateChestItemRequestDto request)
        {
            if (!ModelState.IsValid)  // Reject request immediately if any DTO validation annotation fails
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });  // Return HTTP 400 with validation error details
            try
            {
                var item = await _dungeonConfigService.AddChestItem(id, request);
                return Ok(new ApiResponse<ChestItemResponseDto> { Success = true, Data = item });  // Return HTTP 200 with standard ApiResponse envelope
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = ex.Message, ErrorCode = ErrorCodes.NotFound });  // Return HTTP 404 when the requested resource does not exist
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/chest-items/{chestItemId}")]
        // Executes update chest item operation.
        public async Task<IActionResult> UpdateChestItem(int id, int chestItemId, [FromBody] CreateChestItemRequestDto request)
        {
            if (!ModelState.IsValid)  // Reject request immediately if any DTO validation annotation fails
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });  // Return HTTP 400 with validation error details
            try
            {
                var item = await _dungeonConfigService.UpdateChestItem(id, chestItemId, request);
                return Ok(new ApiResponse<ChestItemResponseDto> { Success = true, Data = item });  // Return HTTP 200 with standard ApiResponse envelope
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = ex.Message, ErrorCode = ErrorCodes.NotFound });  // Return HTTP 404 when the requested resource does not exist
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}/chest-items/{chestItemId}")]
        // Executes remove chest item operation.
        public async Task<IActionResult> RemoveChestItem(int id, int chestItemId)
        {
            await _dungeonConfigService.RemoveChestItem(id, chestItemId);
            return Ok(new ApiResponse<object> { Success = true, Message = "Item removed from chest." });  // Return HTTP 200 with standard ApiResponse envelope
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
        [HttpGet("history")]
        // Executes get history operation.
        public async Task<IActionResult> GetHistory()
        {
            try
            {
                var profileId = GetPlayerProfileId();
                var result = await _dungeonSessionService.GetHistory(profileId);
                return Ok(new ApiResponse<List<DungeonHistoryResponseDto>>  // Return HTTP 200 with standard ApiResponse envelope
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
