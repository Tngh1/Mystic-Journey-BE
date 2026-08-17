using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/[controller]")]
    // Executes controller base operation.
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemService _itemService;

        // Initializes a new instance of ItemsController with dependencies: itemService.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public ItemsController(IItemService itemService)
        {
            _itemService = itemService;
        }


        // ─── Admin APIs ───────────────────────────────────────────────────────
        [HttpGet("{id}")]
        // Executes get by id operation.
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _itemService.GetItemById(id);
            if (item == null)  // Entity not found — short-circuit with appropriate error result
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Item with id {id} not found.", ErrorCode = ErrorCodes.NotFound });  // Return HTTP 404 when the requested resource does not exist

            return Ok(new ApiResponse<ItemResponseDto> { Success = true, Data = item });  // Return HTTP 200 with standard ApiResponse envelope
        }

        [HttpGet]
        // Load all using page, page size, search, and type; it loads items paged.
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? type = null,
            [FromQuery] string? rarity = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = null)
        {
            var result = await _itemService.GetItemsPaged(page, pageSize, search, type, rarity, isActive, sortBy, sortOrder);
            return Ok(new ApiResponse<PagedResultDto<ItemResponseDto>> { Success = true, Data = result });  // Return HTTP 200 with standard ApiResponse envelope
        }


        [HttpPut("{id}")]
        // Per-frame update loop for ItemsController.
        // Handles real-time input polling, smooth interpolations, cooldown timers, and UI updates.
        public async Task<IActionResult> Update(int id, [FromBody] UpdateItemRequestDto request)
        {
            if (!ModelState.IsValid)  // Reject request immediately if any DTO validation annotation fails
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });  // Return HTTP 400 with validation error details

            var item = await _itemService.UpdateItem(id, request);
            return Ok(new ApiResponse<ItemResponseDto> { Success = true, Data = item });  // Return HTTP 200 with standard ApiResponse envelope
        }
    }
}
