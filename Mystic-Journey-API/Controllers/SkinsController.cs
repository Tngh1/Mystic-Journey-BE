using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Security.Claims;
using DAL.Repositories.Interfaces;

namespace Mystic_Journey_API.Controllers
{
    // Quản lý skins (áo) của người chơi.
    // Cho phép trang bị và gỡ skin.
    [Route("api/[controller]")]
    [ApiController]
    public class SkinsController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly IPlayerProfileRepository _playerProfileRepository;

        public SkinsController(IInventoryService inventoryService, IPlayerProfileRepository playerProfileRepository)
        {
            _inventoryService = inventoryService;
            _playerProfileRepository = playerProfileRepository;
        }

        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // ── POST /api/skins/equip ─────────────────────────────────────
        // Trang bị skin cho nhân vật.
        [Authorize]
        [HttpPost("equip")]
        public async Task<IActionResult> EquipSkin([FromBody] EquipSkinRequestDto request)
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(claim, out var accountId))
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Invalid authentication token.", ErrorCode = ErrorCodes.Unauthorized });

            var profile = await _playerProfileRepository.GetByAccountId(accountId);
            if (profile == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.NotFound });

            var updated = await _inventoryService.EquipSkin(profile.PlayerProfileId, request);
            return Ok(new ApiResponse<PlayerSkinResponseDto> { Success = true, Data = updated });
        }

        // ── POST /api/skins/unequip ────────────────────────────────────
        // Gỡ skin đang trang bị.
        [Authorize]
        [HttpPost("unequip")]
        public async Task<IActionResult> UnequipSkin([FromBody] UnequipSkinRequestDto request)
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(claim, out var accountId))
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Invalid authentication token.", ErrorCode = ErrorCodes.Unauthorized });

            var profile = await _playerProfileRepository.GetByAccountId(accountId);
            if (profile == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.NotFound });

            await _inventoryService.UnequipSkin(profile.PlayerProfileId, request);
            return Ok(new ApiResponse<object> { Success = true, Message = "Skin unequipped successfully." });
        }
    }
}
