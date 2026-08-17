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
    public class SkinsController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly IPlayerProfileService _playerProfileService;

        // Initializes a new instance of SkinsController with dependencies: inventoryService, playerProfileService.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public SkinsController(IInventoryService inventoryService, IPlayerProfileService playerProfileService)
        {
            _inventoryService = inventoryService;
            _playerProfileService = playerProfileService;
        }


        // ─── Player APIs ───────────────────────────────────────────────────────
        [Authorize]
        [HttpPost("equip")]
        // Equips a cosmetic character skin, updating active avatar appearance and visual FX.
        public async Task<IActionResult> EquipSkin([FromBody] EquipSkinRequestDto request)
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier); // Read caller account ID from JWT claim
            if (!int.TryParse(claim, out var accountId))
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Invalid authentication token.", ErrorCode = ErrorCodes.Unauthorized });

            var profile = await _playerProfileService.GetByAccountIdAsync(accountId); // Resolve profile from account ID
            if (profile == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.NotFound });

            var updated = await _inventoryService.EquipSkin(profile.PlayerProfileId, request); // Verify ownership, set IsEquipped = true, and unequip other skins
            return Ok(new ApiResponse<PlayerSkinResponseDto> { Success = true, Data = updated });
        }

        [Authorize]
        [HttpPost("unequip")]
        // Unequips cosmetic skin and restores default character class mesh.
        public async Task<IActionResult> UnequipSkin([FromBody] UnequipSkinRequestDto request)
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier); // Read caller account ID from JWT claim
            if (!int.TryParse(claim, out var accountId))
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Invalid authentication token.", ErrorCode = ErrorCodes.Unauthorized });

            var profile = await _playerProfileService.GetByAccountIdAsync(accountId); // Resolve profile from account ID
            if (profile == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.NotFound });

            await _inventoryService.UnequipSkin(profile.PlayerProfileId, request); // Clear active skin attachment
            return Ok(new ApiResponse<object> { Success = true, Message = "Skin unequipped successfully." });
        }
    }
}
