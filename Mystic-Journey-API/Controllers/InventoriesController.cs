using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InventoriesController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoriesController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        private Guid GetAccountId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        [HttpGet]
        public async Task<IActionResult> GetInventory([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50)
        {
            var accountId = GetAccountId();
            var result = await _inventoryService.GetPlayerInventoryAsync(accountId, pageNumber, pageSize);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("equipped")]
        public async Task<IActionResult> GetEquippedItems()
        {
            var accountId = GetAccountId();
            var result = await _inventoryService.GetEquippedItemsAsync(accountId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("{inventoryItemId}")]
        public async Task<IActionResult> GetInventoryItemDetail(Guid inventoryItemId)
        {
            var accountId = GetAccountId();
            var result = await _inventoryService.GetInventoryItemDetailAsync(accountId, inventoryItemId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddItem([FromBody] AddItemToInventoryRequestDto request)
        {
            var accountId = GetAccountId();
            var result = await _inventoryService.AddItemToInventoryAsync(accountId, request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("remove")]
        public async Task<IActionResult> RemoveItem([FromBody] RemoveItemFromInventoryRequestDto request)
        {
            var accountId = GetAccountId();
            var result = await _inventoryService.RemoveItemFromInventoryAsync(accountId, request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("equip")]
        public async Task<IActionResult> EquipItem([FromBody] EquipItemRequestDto request)
        {
            var accountId = GetAccountId();
            var result = await _inventoryService.EquipItemAsync(accountId, request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("unequip")]
        public async Task<IActionResult> UnequipItem([FromBody] UnequipItemRequestDto request)
        {
            var accountId = GetAccountId();
            var result = await _inventoryService.UnequipItemAsync(accountId, request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("enhance")]
        public async Task<IActionResult> EnhanceItem([FromBody] EnhanceItemRequestDto request)
        {
            var accountId = GetAccountId();
            var result = await _inventoryService.EnhanceItemAsync(accountId, request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
