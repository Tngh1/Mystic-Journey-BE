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
    public class MonstersController : ControllerBase
    {
        private readonly IMonsterService _monsterService;

        public MonstersController(IMonsterService monsterService)
        {
            _monsterService = monsterService;
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var monster = await _monsterService.GetMonsterById(id);
            if (monster == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Monster with id {id} not found.", ErrorCode = ErrorCodes.NotFound });

            return Ok(new ApiResponse<MonsterDetailResponseDto> { Success = true, Data = monster });
        }

        [Authorize]
        [HttpGet("{id}/me")]
        public async Task<IActionResult> GetByIdForPlayer(int id)
        {
            try
            {
                var profileId = GetPlayerProfileId();
                var monster = await _monsterService.GetMonsterForPlayer(id, profileId);
                if (monster == null)
                    return NotFound(new ApiResponse<object> { Success = false, Message = $"Monster with id {id} not found.", ErrorCode = ErrorCodes.NotFound });

                return Ok(new ApiResponse<MonsterDetailResponseDto> { Success = true, Data = monster });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse<object> { Success = false, Message = ex.Message, ErrorCode = ErrorCodes.Unauthorized });
            }
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMonsterRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var monster = await _monsterService.CreateMonster(request);
            return Ok(new ApiResponse<MonsterResponseDto> { Success = true, Data = monster });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMonsterRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var monster = await _monsterService.UpdateMonster(id, request);
            return Ok(new ApiResponse<MonsterResponseDto> { Success = true, Data = monster });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost("{id}/drops")]
        public async Task<IActionResult> AddDrop(int id, [FromBody] CreateMonsterDropRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var drop = await _monsterService.AddMonsterDrop(id, request);
            return Ok(new ApiResponse<MonsterDropResponseDto> { Success = true, Data = drop });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] string? type = null, [FromQuery] bool? isActive = null)
        {
            var result = await _monsterService.GetMonstersPaged(page, pageSize, search, type, isActive);
            return Ok(new ApiResponse<PagedResultDto<MonsterResponseDto>> { Success = true, Data = result });
        }

        [HttpGet("drops")]
        public async Task<IActionResult> GetAllDrops([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _monsterService.GetMonsterDropsPaged(page, pageSize);
            return Ok(new ApiResponse<PagedResultDto<MonsterDropResponseDto>> { Success = true, Data = result });
        }

        [Authorize]
        [HttpGet("me/catalog")]
        public async Task<IActionResult> GetCatalogForPlayer(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null,
            [FromQuery] string? type = null)
        {
            try
            {
                var profileId = GetPlayerProfileId();
                var result = await _monsterService.GetMonsterCatalogForPlayer(profileId, page, pageSize, search, type);
                return Ok(new ApiResponse<PagedResultDto<PlayerMonsterCatalogItemDto>> { Success = true, Data = result });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse<object> { Success = false, Message = ex.Message, ErrorCode = ErrorCodes.Unauthorized });
            }
        }

        [Authorize]
        [HttpGet("spawns")]
        public async Task<IActionResult> GetSpawnsForPlayer(
            [FromQuery] string mapName,
            [FromQuery] string? regionName = null,
            [FromQuery] int? dungeonId = null)
        {
            try
            {
                var profileId = GetPlayerProfileId();
                var spawns = await _monsterService.GetSpawnsForPlayer(profileId, mapName, regionName, dungeonId);
                return Ok(new ApiResponse<List<MonsterSpawnResponseDto>> { Success = true, Data = spawns });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse<object> { Success = false, Message = ex.Message, ErrorCode = ErrorCodes.Unauthorized });
            }
        }

        [Authorize]
        [HttpPost("{id}/discover")]
        public async Task<IActionResult> Discover(int id)
        {
            try
            {
                var profileId = GetPlayerProfileId();
                var result = await _monsterService.DiscoverMonster(profileId, id);
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
        public async Task<IActionResult> Defeat(int id, [FromBody] MonsterDefeatRequestDto? request)
        {
            try
            {
                var profileId = GetPlayerProfileId();
                var result = await _monsterService.DefeatMonster(profileId, id, request);
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

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet("{id}/spawns")]
        public async Task<IActionResult> GetSpawnsByMonster(int id)
        {
            var spawns = await _monsterService.GetSpawnsByMonsterId(id);
            return Ok(new ApiResponse<List<MonsterSpawnResponseDto>> { Success = true, Data = spawns });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost("spawns")]
        public async Task<IActionResult> CreateSpawn([FromBody] CreateMonsterSpawnRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            try
            {
                var spawn = await _monsterService.CreateSpawn(request);
                return Ok(new ApiResponse<MonsterSpawnResponseDto> { Success = true, Data = spawn });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = ex.Message, ErrorCode = ErrorCodes.NotFound });
            }
        }

        // Helper to read playerProfileId claim from JWT, mirrors other controllers.
        private int GetPlayerProfileId()
        {
            var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("playerProfileId")?.Value;
            if (!int.TryParse(claim, out var id))
                throw new UnauthorizedAccessException("PlayerProfileId is missing from token. Please login again.");
            return id;
        }
    }
}
