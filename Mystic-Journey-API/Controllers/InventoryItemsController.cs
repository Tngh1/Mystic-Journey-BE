using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InventoryItemsController : ControllerBase
    {
        private readonly IInventoryItemService _inventoryItemService;

        public InventoryItemsController(IInventoryItemService inventoryItemService)
        {
            _inventoryItemService = inventoryItemService;
        }

        [HttpGet("player/{playerProfileId}")]
        public async Task<IActionResult> GetPlayerInventoryAsync(int playerProfileId)
        {
            var result = await _inventoryItemService.GetPlayerInventoryAsync(playerProfileId);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddItemToInventoryAsync([FromBody] AddInventoryItemRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new InventoryApiResponseDto
                {
                    Success = false,
                    Message = GetError()
                });
            }

            var result = await _inventoryItemService.AddItemToInventoryAsync(request);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInventoryItemAsync(int id, [FromBody] UpdateInventoryItemRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new InventoryApiResponseDto
                {
                    Success = false,
                    Message = GetError()
                });
            }

            var result = await _inventoryItemService.UpdateInventoryItemAsync(id, request);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveItemFromInventoryAsync(int id)
        {
            var result = await _inventoryItemService.RemoveItemFromInventoryAsync(id);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        private string GetError()
        {
            return ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .FirstOrDefault() ?? "Invalid request.";
        }
    }
}
