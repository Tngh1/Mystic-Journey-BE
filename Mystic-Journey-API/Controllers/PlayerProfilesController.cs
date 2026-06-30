using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    // Quản lý player profile (hồ sơ người chơi).
    // Game APIs: Xem, cập nhật profile và xem bạn bè.
    // Admin APIs: Xem danh sách tất cả player profiles.
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerProfilesController : ControllerBase
    {
        private readonly IPlayerProfileService _playerProfileService;
        public PlayerProfilesController(IPlayerProfileService playerProfileService)
        {
            _playerProfileService = playerProfileService;
        }

        private int GetCurrentAccountId()
        {
            var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }

        private int GetCurrentPlayerProfileId()
        {
            var claim = User.FindFirst("playerProfileId");
            if (claim != null && int.TryParse(claim.Value, out var profileId))
            {
                return profileId;
            }
            return 0;
        }

        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // ── GET /api/playerprofiles/{id} ─────────────────────────────────────
        // Lấy chi tiết player profile theo ID.
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _playerProfileService.GetProfileById(id);
            return Ok(new ApiResponse<PlayerProfileDetailResponseDto> { Success = true, Data = result });
        }

        // ── PUT /api/playerprofiles/{id} ──────────────────────────────────────
        // Cập nhật thông tin player profile (display name, avatar...).
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePlayerProfileRequestDto dto)
        {
            var result = await _playerProfileService.UpdateProfile(id, dto);
            return Ok(new ApiResponse<PlayerProfileResponseDto> { Success = true, Data = result });
        }

        // ── GET /api/playerprofiles/me/friends ────────────────────────────────
        // Lấy danh sách bạn bè của player đang đăng nhập.
        [Authorize]
        [HttpGet("me/friends")]
        public async Task<IActionResult> GetMyFriends()
        {
            var playerProfileId = GetCurrentPlayerProfileId();
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.Unauthorized });

            var result = await _playerProfileService.GetFriends(playerProfileId);
            return Ok(new ApiResponse<List<PlayerProfileResponseDto>> { Success = true, Data = result });
        }

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // ── GET /api/playerprofiles ───────────────────────────────────────────
        // Lấy danh sách tất cả player profiles có phân trang và lọc.
        // Query: page, pageSize, search, level.
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] int? level = null)
        {
            var result = await _playerProfileService.GetProfilesPaged(page, pageSize, search, level);
            return Ok(new ApiResponse<PagedResultDto<PlayerProfileResponseDto>> { Success = true, Data = result });
        }
    }
}
