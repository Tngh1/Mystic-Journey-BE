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
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        private int GetPlayerProfileId()
        {
            var claim = User.FindFirstValue("playerProfileId");
            if (!int.TryParse(claim, out var id))
                throw new UnauthorizedAccessException("PlayerProfileId is missing from token. Please login again.");
            return id;
        }

        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyInventory()
        {
            var profileId = GetPlayerProfileId();
            var inventory = await _inventoryService.GetInventory(profileId);
            return Ok(new ApiResponse<InventorySummaryDto> { Success = true, Data = inventory });
        }

        [Authorize]
        [HttpGet("me/full")]
        public async Task<IActionResult> GetMyInventoryFull()
        {
            var profileId = GetPlayerProfileId();
            var result = await _inventoryService.GetMeInventory(profileId);
            return Ok(new ApiResponse<PlayerMeInventoryResponseDto> { Success = true, Data = result });
        }

        [Authorize]
        [HttpPost("equip-item")]
        public async Task<IActionResult> EquipItem([FromBody] EquipItemRequestDto request)
        {
            var profileId = GetPlayerProfileId();
            var result = await _inventoryService.EquipItem(profileId, request);
            return Ok(new ApiResponse<InventoryActionResultDto> { Success = true, Data = result });
        }

        [Authorize]
        [HttpPost("unequip-item")]
        public async Task<IActionResult> UnequipItem([FromBody] UnequipItemRequestDto request)
        {
            var profileId = GetPlayerProfileId();
            var result = await _inventoryService.UnequipItem(profileId, request);
            return Ok(new ApiResponse<InventoryActionResultDto> { Success = true, Data = result });
        }

        [Authorize]
        [HttpPost("consume-item")]
        public async Task<IActionResult> ConsumeItem([FromBody] ConsumeItemRequestDto request)
        {
            var profileId = GetPlayerProfileId();
            var result = await _inventoryService.ConsumeItem(profileId, request);
            return Ok(new ApiResponse<ConsumeItemResultDto>
            {
                Success = true,
                Message = $"Used {result.ItemName} successfully.",
                Data    = result
            });
        }


        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        [Authorize(Roles = "Admin")]
        [HttpGet("{playerProfileId:int}")]
        public async Task<IActionResult> GetInventoryByProfileId(int playerProfileId)
        {
            var result = await _inventoryService.GetMeInventory(playerProfileId);
            return Ok(new ApiResponse<PlayerMeInventoryResponseDto> { Success = true, Data = result });
        }
    }
}
