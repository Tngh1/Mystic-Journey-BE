using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Security.Claims;
using DAL.Repositories.Interfaces;

namespace Mystic_Journey_API.Controllers
{
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

        [Authorize]
        [HttpPost("equip")]
        public async Task<IActionResult> EquipSkin([FromBody] EquipSkinRequestDto request)
        {
            try
            {
                var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(claim, out var accountId))
                    return Unauthorized(new { message = "Invalid authentication token." });

                var profile = await _playerProfileRepository.GetByAccountId(accountId);
                if (profile == null)
                    return NotFound(new { message = "Player profile not found." });

                var updated = await _inventoryService.EquipSkin(profile.PlayerProfileId, request);
                return Ok(new ApiResponse<PlayerSkinResponseDto> { Data = updated });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("unequip")]
        public async Task<IActionResult> UnequipSkin([FromBody] UnequipSkinRequestDto request)
        {
            try
            {
                var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(claim, out var accountId))
                    return Unauthorized(new { message = "Invalid authentication token." });

                var profile = await _playerProfileRepository.GetByAccountId(accountId);
                if (profile == null)
                    return NotFound(new { message = "Player profile not found." });

                await _inventoryService.UnequipSkin(profile.PlayerProfileId, request);
                return Ok(new ApiResponse<object> { Data = null });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
