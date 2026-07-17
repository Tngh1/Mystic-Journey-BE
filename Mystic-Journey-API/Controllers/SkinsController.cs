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
    [ApiController]
    public class SkinsController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly IPlayerProfileService _playerProfileService;

        public SkinsController(IInventoryService inventoryService, IPlayerProfileService playerProfileService)
        {
            _inventoryService = inventoryService;
            _playerProfileService = playerProfileService;
        }

        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        [Authorize]
        [HttpPost("equip")]
        public async Task<IActionResult> EquipSkin([FromBody] EquipSkinRequestDto request)
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(claim, out var accountId))
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Invalid authentication token.", ErrorCode = ErrorCodes.Unauthorized });

            var profile = await _playerProfileService.GetByAccountIdAsync(accountId);
            if (profile == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.NotFound });

            var updated = await _inventoryService.EquipSkin(profile.PlayerProfileId, request);
            return Ok(new ApiResponse<PlayerSkinResponseDto> { Success = true, Data = updated });
        }

        [Authorize]
        [HttpPost("unequip")]
        public async Task<IActionResult> UnequipSkin([FromBody] UnequipSkinRequestDto request)
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(claim, out var accountId))
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Invalid authentication token.", ErrorCode = ErrorCodes.Unauthorized });

            var profile = await _playerProfileService.GetByAccountIdAsync(accountId);
            if (profile == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.NotFound });

            await _inventoryService.UnequipSkin(profile.PlayerProfileId, request);
            return Ok(new ApiResponse<object> { Success = true, Message = "Skin unequipped successfully." });
        }
    }
}
