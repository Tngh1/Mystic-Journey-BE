using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IItemService _itemService;

        public ItemsController(IItemService itemService)
        {
            _itemService = itemService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllItems([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _itemService.GetAllItemsAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("type/{type}")]
        public async Task<IActionResult> GetItemsByType(int type, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _itemService.GetItemsByTypeAsync((DAL.Models.Item.ItemType)type, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("rarity/{rarity}")]
        public async Task<IActionResult> GetItemsByRarity(int rarity, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _itemService.GetItemsByRarityAsync((DAL.Models.Item.ItemRarity)rarity, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchItems([FromQuery] string name, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest(new { Success = false, Message = "Search term is required." });
            }

            var result = await _itemService.SearchItemsAsync(name, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemById(Guid id)
        {
            var result = await _itemService.GetItemByIdAsync(id);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("{id}/detail")]
        public async Task<IActionResult> GetItemDetail(Guid id)
        {
            var result = await _itemService.GetItemDetailAsync(id);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateItem([FromBody] CreateItemRequestDto request)
        {
            var result = await _itemService.CreateItemAsync(request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetItemById), new { id = result.Item?.ItemId }, result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(Guid id, [FromBody] UpdateItemRequestDto request)
        {
            var result = await _itemService.UpdateItemAsync(id, request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            var result = await _itemService.DeleteItemAsync(id);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}
