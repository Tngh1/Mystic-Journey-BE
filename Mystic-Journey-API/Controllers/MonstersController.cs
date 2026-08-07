using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    // Quản lý monsters (quái vật) và monster spawns (vị trí spawn).
    // Game APIs: Khám phá, đánh bại, xem catalog, xem spawns.
    // Admin APIs: Tạo, cập nhật monster và spawns.
    [Route("api/[controller]")]
    [ApiController]
    public class MonstersController : ControllerBase
    {
        private readonly IMonsterService _monsterService;

        public MonstersController(IMonsterService monsterService)
        {
            _monsterService = monsterService;
        }

        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // ── GET /api/monsters/{id} ─────────────────────────────────────────
        // Lấy thông tin monster theo ID. Yêu cầu đăng nhập: bản công khai cho
        // web wiki là /api/wiki/monsters/{id}, endpoint này thuộc luồng game.
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var monster = await _monsterService.GetMonsterById(id);
            if (monster == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Monster with id {id} not found.", ErrorCode = ErrorCodes.NotFound });

            return Ok(new ApiResponse<MonsterDetailResponseDto> { Success = true, Data = monster });
        }

        // ── GET /api/monsters/{id}/me ───────────────────────────────────────
        // Lấy thông tin monster cho player cụ thể (có trạng thái khám phá).
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

        // ── GET /api/monsters ───────────────────────────────────────────────
        // Lấy danh sách tất cả monsters có phân trang và lọc (Dashboard).
        // Query: page, pageSize, search, type, isActive.
        // Codex công khai: xem WikiController (/api/wiki/monsters).
        [Authorize(Roles = "Admin,SuperAdmin")]
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
            var result = await _monsterService.GetMonstersPaged(page, pageSize, search, type, isActive, sortBy, sortOrder);
            return Ok(new ApiResponse<PagedResultDto<MonsterResponseDto>> { Success = true, Data = result });
        }

        // ── GET /api/monsters/me/catalog ───────────────────────────────────
        // Lấy catalog monsters đã khám phá của player.
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

        // ── GET /api/monsters/spawns ────────────────────────────────────────
        // Lấy danh sách vị trí spawn monsters theo map.
        // Query: mapName, regionName, dungeonId.
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

        // ── POST /api/monsters/{id}/discover ────────────────────────────────
        // Khám phá monster (thêm vào catalog của player).
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

        // ── POST /api/monsters/{id}/defeat ─────────────────────────────────
        // Đánh bại monster, nhận XP và gold.
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

        // ── GET /api/monsters/drops ─────────────────────────────────────────
        // Lấy danh sách monster drops có phân trang (Dashboard).
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet("drops")]
        public async Task<IActionResult> GetAllDrops([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _monsterService.GetMonsterDropsPaged(page, pageSize);
            return Ok(new ApiResponse<PagedResultDto<MonsterDropResponseDto>> { Success = true, Data = result });
        }

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════
        // NOTE: Create endpoint removed - managed via seeding.

        // ── PUT /api/monsters/{id} ─────────────────────────────────────────
        // Cập nhật monster hiện có.
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMonsterRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var monster = await _monsterService.UpdateMonster(id, request);
            return Ok(new ApiResponse<MonsterResponseDto> { Success = true, Data = monster });
        }

        // ── POST /api/monsters/{id}/drops ────────────────────────────────────
        // Thêm drop cho monster.
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost("{id}/drops")]
        public async Task<IActionResult> AddDrop(int id, [FromBody] CreateMonsterDropRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var drop = await _monsterService.AddMonsterDrop(id, request);
            return Ok(new ApiResponse<MonsterDropResponseDto> { Success = true, Data = drop });
        }

        // ── GET /api/monsters/{id}/spawns ───────────────────────────────────
        // Lấy danh sách spawns của một monster (Admin).
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet("{id}/spawns")]
        public async Task<IActionResult> GetSpawnsByMonster(int id)
        {
            var spawns = await _monsterService.GetSpawnsByMonsterId(id);
            return Ok(new ApiResponse<List<MonsterSpawnResponseDto>> { Success = true, Data = spawns });
        }

        // ── POST /api/monsters/spawns ────────────────────────────────────────
        // Tạo spawn mới cho monster.
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

        // ── Helper ──────────────────────────────────────────────────────────
        // Đọc playerProfileId từ JWT token.
        private int GetPlayerProfileId()
        {
            var claim = User.FindFirst("playerProfileId")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(claim, out var id))
                throw new UnauthorizedAccessException("PlayerProfileId is missing from token. Please login again.");
            return id;
        }
    }
}
