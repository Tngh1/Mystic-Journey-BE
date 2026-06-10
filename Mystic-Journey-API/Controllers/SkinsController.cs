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
                    return Unauthorized(new ErrorResponse { Error = "UNAUTHORIZED", Message = "Invalid authentication token." });

                var profile = await _playerProfileRepository.GetByAccountId(accountId);
                if (profile == null)
                    return NotFound(new ErrorResponse { Error = "PROFILE_NOT_FOUND", Message = "Player profile not found." });

                var updated = await _inventoryService.EquipSkin(profile.PlayerProfileId, request);
                return Ok(new ApiResponse<PlayerSkinResponseDto> { Success = true, Message = "Skin updated.", Data = updated });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ErrorResponse { Error = "SKIN_NOT_FOUND", Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ErrorResponse { Error = "UNAUTHORIZED", Message = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Error = "INTERNAL_ERROR", Message = ex.Message });
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
                    return Unauthorized(new ErrorResponse { Error = "UNAUTHORIZED", Message = "Invalid authentication token." });

                var profile = await _playerProfileRepository.GetByAccountId(accountId);
                if (profile == null)
                    return NotFound(new ErrorResponse { Error = "PROFILE_NOT_FOUND", Message = "Player profile not found." });

                await _inventoryService.UnequipSkin(profile.PlayerProfileId, request);
                return Ok(new ApiResponse<object> { Success = true, Message = "Skin unequipped." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ErrorResponse { Error = "SKIN_NOT_FOUND", Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ErrorResponse { Error = "UNAUTHORIZED", Message = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Error = "INTERNAL_ERROR", Message = ex.Message });
            }
        }
    }
}
