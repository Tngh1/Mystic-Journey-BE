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
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        // Initializes a new instance of InventoryController with dependencies: inventoryService.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        // Executes get player profile id operation.
        // Throws an exception if precondition validations fail.
        private int GetPlayerProfileId()
        {
            var claim = User.FindFirstValue("playerProfileId");
            if (!int.TryParse(claim, out var id))  // Claim value missing or non-integer — reject as unauthorized
                throw new UnauthorizedAccessException("PlayerProfileId is missing from token. Please login again.");  // Authentication token is invalid or expired
            return id;
        }


        // ─── Player APIs ───────────────────────────────────────────────────────
        [Authorize]
        [HttpGet("me")]
        // Returns basic summary of the player inventory (slot counts and top-level item list).
        public async Task<IActionResult> GetMyInventory()
        {
            var profileId = GetPlayerProfileId(); // Extract authenticated player's profile ID from JWT claim
            var inventory = await _inventoryService.GetInventory(profileId); // Load summarized inventory data
            return Ok(new ApiResponse<InventorySummaryDto> { Success = true, Data = inventory }); // Return HTTP 200 with inventory summary
        }

        [Authorize]
        [HttpGet("me/full")]
        // Returns full inventory details including equipped gear, bag slots, enhancement levels, and calculated stats.
        public async Task<IActionResult> GetMyInventoryFull()
        {
            var profileId = GetPlayerProfileId(); // Extract authenticated player's profile ID from JWT claim
            var result = await _inventoryService.GetMeInventory(profileId); // Load full inventory payload with equipment slots and base stats
            return Ok(new ApiResponse<PlayerMeInventoryResponseDto> { Success = true, Data = result }); // Return HTTP 200 with complete inventory response
        }

        [Authorize]
        [HttpPost("equip-item")]
        // Equips an inventory item to the appropriate equipment slot and updates player combat stats.
        public async Task<IActionResult> EquipItem([FromBody] EquipItemRequestDto request)
        {
            var profileId = GetPlayerProfileId(); // Extract authenticated player's profile ID from JWT claim
            var result = await _inventoryService.EquipItem(profileId, request); // Unequip existing slot item if present, equip new item, and recalculate stat snapshot
            return Ok(new ApiResponse<InventoryActionResultDto> { Success = true, Data = result }); // Return HTTP 200 with updated item and player stats
        }

        [Authorize]
        [HttpPost("unequip-item")]
        // Unequips an equipped item back into player inventory and recalculates player combat stats.
        public async Task<IActionResult> UnequipItem([FromBody] UnequipItemRequestDto request)
        {
            var profileId = GetPlayerProfileId(); // Extract authenticated player's profile ID from JWT claim
            var result = await _inventoryService.UnequipItem(profileId, request); // Clear equipped slot, move back to bag, and recalculate combat stats
            return Ok(new ApiResponse<InventoryActionResultDto> { Success = true, Data = result }); // Return HTTP 200 with updated item and player stats
        }

        [Authorize]
        [HttpPost("consume-item")]
        // Consumes a consumable item (e.g. potion, scroll) and applies its instant or duration effect.
        public async Task<IActionResult> ConsumeItem([FromBody] ConsumeItemRequestDto request)
        {
            var profileId = GetPlayerProfileId(); // Extract authenticated player's profile ID from JWT claim
            var result = await _inventoryService.ConsumeItem(profileId, request); // Deduct quantity, apply buff/heal/currency effect, and persist changes
            return Ok(new ApiResponse<ConsumeItemResultDto> // Return HTTP 200 with consumption outcome
            {
                Success = true,
                Message = $"Used {result.ItemName} successfully.",
                Data    = result
            });
        }



        // ─── Admin APIs ───────────────────────────────────────────────────────
        [Authorize(Roles = "Admin")]
        [HttpGet("{playerProfileId:int}")]
        // Admin endpoint: inspect full inventory of any specific player profile.
        public async Task<IActionResult> GetInventoryByProfileId(int playerProfileId)
        {
            var result = await _inventoryService.GetMeInventory(playerProfileId); // Load target player's full inventory and equipment
            return Ok(new ApiResponse<PlayerMeInventoryResponseDto> { Success = true, Data = result }); // Return HTTP 200 with target player's inventory
        }
    }
}
