using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/[controller]")]
    // Executes controller base operation.
    [ApiController]
    public class MonstersController : ControllerBase
    {
        private readonly IMonsterService _monsterService;

        // Initializes a new instance of MonstersController with dependencies: monsterService.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public MonstersController(IMonsterService monsterService)
        {
            _monsterService = monsterService;
        }


        // ─── Player APIs ───────────────────────────────────────────────────────
        [Authorize]
        [HttpGet("{id}")]
        // Retrieves base monster stats, elemental affinity, and lore description.
        public async Task<IActionResult> GetById(int id)
        {
            var monster = await _monsterService.GetMonsterById(id); // Look up monster definition
            if (monster == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Monster with id {id} not found.", ErrorCode = ErrorCodes.NotFound });

            return Ok(new ApiResponse<MonsterDetailResponseDto> { Success = true, Data = monster });
        }

        [Authorize]
        [HttpGet("{id}/me")]
        // Retrieves player-specific bestiary entry for this monster (kill count, discovery status, unlocked drop info).
        public async Task<IActionResult> GetByIdForPlayer(int id)
        {
            try
            {
                var profileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
                var monster = await _monsterService.GetMonsterForPlayer(id, profileId); // Query player bestiary record combined with monster data
                if (monster == null)
                    return NotFound(new ApiResponse<object> { Success = false, Message = $"Monster with id {id} not found.", ErrorCode = ErrorCodes.NotFound });

                return Ok(new ApiResponse<MonsterDetailResponseDto> { Success = true, Data = monster });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse<object> { Success = false, Message = ex.Message, ErrorCode = ErrorCodes.Unauthorized });
            }
        }

        // ─── Admin APIs ───────────────────────────────────────────────────────
        [Authorize(Roles = "Admin")]
        [HttpGet]
        // Retrieves paginated list of all monster entities for game masters/admin portal.
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? type = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = null)
        {
            var result = await _monsterService.GetMonstersPaged(page, pageSize, search, type, isActive, sortBy, sortOrder); // Query monsters database
            return Ok(new ApiResponse<PagedResultDto<MonsterResponseDto>> { Success = true, Data = result });
        }

        // ─── Player APIs ───────────────────────────────────────────────────────
        [Authorize]
        [HttpGet("me/catalog")]
        // Retrieves player's complete bestiary catalog with discovered and defeated progress badges.
        public async Task<IActionResult> GetCatalogForPlayer(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null,
            [FromQuery] string? type = null)
        {
            try
            {
                var profileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
                var result = await _monsterService.GetMonsterCatalogForPlayer(profileId, page, pageSize, search, type); // Query bestiary progress items
                return Ok(new ApiResponse<PagedResultDto<PlayerMonsterCatalogItemDto>> { Success = true, Data = result });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse<object> { Success = false, Message = ex.Message, ErrorCode = ErrorCodes.Unauthorized });
            }
        }

        [Authorize]
        [HttpGet("spawns")]
        // Retrieves configured monster spawn locations for a specific map or dungeon instance.
        public async Task<IActionResult> GetSpawnsForPlayer(
            [FromQuery] string mapName,
            [FromQuery] string? regionName = null,
            [FromQuery] int? dungeonId = null)
        {
            try
            {
                var profileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
                var spawns = await _monsterService.GetSpawnsForPlayer(profileId, mapName, regionName, dungeonId); // Query spawn points and respawn timers
                return Ok(new ApiResponse<List<MonsterSpawnResponseDto>> { Success = true, Data = spawns });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse<object> { Success = false, Message = ex.Message, ErrorCode = ErrorCodes.Unauthorized });
            }
        }

        [Authorize]
        [HttpPost("{id}/discover")]
        // Unlocks monster bestiary entry when player encounters the monster in the game world.
        public async Task<IActionResult> Discover(int id)
        {
            try
            {
                var profileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
                var result = await _monsterService.DiscoverMonster(profileId, id); // Mark monster as discovered in player bestiary
                return Ok(new ApiResponse<PlayerMonsterCatalogItemDto> { Success = true, Data = result, Message = "Monster discovered." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = ex.Message, ErrorCode = ErrorCodes.NotFound });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse<object> { Success = false, Message = ex.Message, ErrorCode = ErrorCodes.Unauthorized });
            }
        }

        [Authorize]
        [HttpPost("{id}/defeat")]
        // Records a monster kill, updates bestiary defeat counters, and rolls for drop rewards.
        public async Task<IActionResult> Defeat(int id, [FromBody] MonsterDefeatRequestDto? request)
        {
            try
            {
                var profileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
                var result = await _monsterService.DefeatMonster(profileId, id, request); // Increment defeat count and evaluate drop tables for inventory loot
                return Ok(new ApiResponse<MonsterDefeatResponseDto> { Success = true, Data = result, Message = "Monster defeated." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = ex.Message, ErrorCode = ErrorCodes.NotFound });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = ex.Message, ErrorCode = ErrorCodes.ValidationError });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse<object> { Success = false, Message = ex.Message, ErrorCode = ErrorCodes.Unauthorized });
            }
        }

        // ─── Admin APIs ───────────────────────────────────────────────────────
        [Authorize(Roles = "Admin")]
        [HttpGet("drops")]
        // Executes get all drops operation.
        public async Task<IActionResult> GetAllDrops([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _monsterService.GetMonsterDropsPaged(page, pageSize);
            return Ok(new ApiResponse<PagedResultDto<MonsterDropResponseDto>> { Success = true, Data = result });  // Return HTTP 200 with standard ApiResponse envelope
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        // Per-frame update loop for MonstersController.
        // Handles real-time input polling, smooth interpolations, cooldown timers, and UI updates.
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMonsterRequestDto request)
        {
            if (!ModelState.IsValid)  // Reject request immediately if any DTO validation annotation fails
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });  // Return HTTP 400 with validation error details

            var monster = await _monsterService.UpdateMonster(id, request);
            return Ok(new ApiResponse<MonsterResponseDto> { Success = true, Data = monster });  // Return HTTP 200 with standard ApiResponse envelope
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/drops")]
        // Executes add drop operation.
        public async Task<IActionResult> AddDrop(int id, [FromBody] CreateMonsterDropRequestDto request)
        {
            if (!ModelState.IsValid)  // Reject request immediately if any DTO validation annotation fails
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });  // Return HTTP 400 with validation error details

            var drop = await _monsterService.AddMonsterDrop(id, request);
            return Ok(new ApiResponse<MonsterDropResponseDto> { Success = true, Data = drop });  // Return HTTP 200 with standard ApiResponse envelope
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}/spawns")]
        // Executes get spawns by monster operation.
        public async Task<IActionResult> GetSpawnsByMonster(int id)
        {
            var spawns = await _monsterService.GetSpawnsByMonsterId(id);
            return Ok(new ApiResponse<List<MonsterSpawnResponseDto>> { Success = true, Data = spawns });  // Return HTTP 200 with standard ApiResponse envelope
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("spawns")]
        // Executes create spawn operation.
        public async Task<IActionResult> CreateSpawn([FromBody] CreateMonsterSpawnRequestDto request)
        {
            if (!ModelState.IsValid)  // Reject request immediately if any DTO validation annotation fails
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });  // Return HTTP 400 with validation error details

            try
            {
                var spawn = await _monsterService.CreateSpawn(request);
                return Ok(new ApiResponse<MonsterSpawnResponseDto> { Success = true, Data = spawn });  // Return HTTP 200 with standard ApiResponse envelope
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = ex.Message, ErrorCode = ErrorCodes.NotFound });  // Return HTTP 404 when the requested resource does not exist
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("spawns/{id}")]
        // Executes update spawn operation.
        public async Task<IActionResult> UpdateSpawn(int id, [FromBody] UpdateMonsterSpawnRequestDto request)
        {
            if (!ModelState.IsValid)  // Reject request immediately if any DTO validation annotation fails
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Invalid request.", Data = ModelState });  // Return HTTP 400 with validation error details

            try
            {
                var updated = await _monsterService.UpdateSpawn(id, request);
                return Ok(new ApiResponse<MonsterSpawnResponseDto> { Success = true, Data = updated, Message = "Spawn updated successfully." });  // Return HTTP 200 with standard ApiResponse envelope
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = ex.Message, ErrorCode = ErrorCodes.NotFound });  // Return HTTP 404 when the requested resource does not exist
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("spawns/{id}")]
        // Executes delete spawn operation.
        public async Task<IActionResult> DeleteSpawn(int id)
        {
            try
            {
                await _monsterService.DeleteSpawn(id);
                return Ok(new ApiResponse<object> { Success = true, Message = "Spawn deleted successfully." });  // Return HTTP 200 with standard ApiResponse envelope
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = ex.Message, ErrorCode = ErrorCodes.NotFound });  // Return HTTP 404 when the requested resource does not exist
            }
        }

        // Executes get player profile id operation.
        private int GetPlayerProfileId()
        {
            var claim = User.FindFirst("playerProfileId")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(claim, out var id))  // Claim value missing or non-integer — reject as unauthorized
                throw new UnauthorizedAccessException("PlayerProfileId is missing from token. Please login again.");  // Authentication token is invalid or expired
            return id;
        }
    }
}
