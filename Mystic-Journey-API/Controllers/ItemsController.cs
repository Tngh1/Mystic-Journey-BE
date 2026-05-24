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
    public class ItemsController : ControllerBase
    {
        private readonly IItemService _itemService;

        public ItemsController(IItemService itemService)
        {
            _itemService = itemService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllItemsAsync()
        {
            var result = await _itemService.GetAllItemsAsync();

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemByIdAsync(int id)
        {
            var result = await _itemService.GetItemByIdAsync(id);

            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateItemAsync([FromBody] CreateItemRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ItemApiResponseDto
                {
                    Success = false,
                    Message = GetError()
                });
            }

            var result = await _itemService.CreateItemAsync(request);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItemAsync(int id, [FromBody] UpdateItemRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ItemApiResponseDto
                {
                    Success = false,
                    Message = GetError()
                });
            }

            var result = await _itemService.UpdateItemAsync(id, request);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItemAsync(int id)
        {
            var result = await _itemService.DeleteItemAsync(id);

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
