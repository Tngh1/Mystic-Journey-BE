using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    // Quản lý cài đặt game (game settings).
    // Game APIs: Xem danh sách, xem theo ID, xem theo key.
    // Admin APIs: Cập nhật setting.
    [Route("api/[controller]")]
    [ApiController]
    public class GameSettingsController : ControllerBase
    {
        private readonly IGameSettingService _gameSettingService;

        public GameSettingsController(IGameSettingService gameSettingService)
        {
            _gameSettingService = gameSettingService;
        }

        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // ── GET /api/gamesettings ──────────────────────────────────
        // Lấy danh sách tất cả game settings có phân trang.
        // Query: page, pageSize, search.
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
        {
            var result = await _gameSettingService.GetSettingsPaged(page, pageSize, search);
            return Ok(new ApiResponse<PagedResultDto<GameSettingResponseDto>> { Success = true, Data = result });
        }

        // ── GET /api/gamesettings/{id} ────────────────────────────
        // Lấy game setting theo ID.
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var setting = await _gameSettingService.GetSettingById(id);
            if (setting == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Game setting with id {id} not found.", ErrorCode = ErrorCodes.NotFound });

            return Ok(new ApiResponse<GameSettingResponseDto> { Success = true, Data = setting });
        }

        // ── GET /api/gamesettings/key/{key} ───────────────────────
        // Lấy game setting theo key.
        [AllowAnonymous]
        [HttpGet("key/{key}")]
        public async Task<IActionResult> GetByKey(string key)
        {
            var setting = await _gameSettingService.GetSettingByKey(key);
            if (setting == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Game setting with key '{key}' not found.", ErrorCode = ErrorCodes.NotFound });

            return Ok(new ApiResponse<GameSettingResponseDto> { Success = true, Data = setting });
        }

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // ── PUT /api/gamesettings/key/{key} ───────────────────────
        // Cập nhật game setting theo key.
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("key/{key}")]
        public async Task<IActionResult> Update(string key, [FromBody] UpdateGameSettingRequestDto request)
        {
            var setting = await _gameSettingService.UpdateSetting(key, request);
            return Ok(new ApiResponse<GameSettingResponseDto> { Success = true, Data = setting });
        }
    }
}
