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
    public class ShopItemsController : ControllerBase
    {
        private readonly IShopItemService _shopItemService;

        // Initializes a new instance of ShopItemsController with dependencies: shopItemService.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public ShopItemsController(IShopItemService shopItemService)
        {
            _shopItemService = shopItemService;
        }


        // ─── Player APIs ───────────────────────────────────────────────────────
        [Authorize]
        [HttpGet("{id}")]
        // Retrieves shop item listing details, price, purchase limits, and currency type by ID.
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _shopItemService.GetShopItemById(id); // Query shop item catalog
            if (item == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Shop item with id {id} not found.", ErrorCode = ErrorCodes.NotFound });

            return Ok(new ApiResponse<ShopItemResponseDto> { Success = true, Data = item });
        }

        // ─── Admin APIs ───────────────────────────────────────────────────────
        [HttpGet]
        // Retrieves paginated list of catalog items with section and currency filters.
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? currency = null,
            [FromQuery] string? shopSection = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = null)
        {
            var result = await _shopItemService.GetShopItemsPaged(page, pageSize, search, currency, shopSection, isActive, sortBy, sortOrder); // Filter shop items table
            return Ok(new ApiResponse<PagedResultDto<ShopItemResponseDto>> { Success = true, Data = result });
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        // Adds a new product listing to the game shop.
        public async Task<IActionResult> Create([FromBody] CreateShopItemRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var item = await _shopItemService.CreateShopItem(request); // Insert shop listing record
            return Ok(new ApiResponse<ShopItemResponseDto> { Success = true, Data = item });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        // Updates pricing, discount percentage, daily cap limits, or active status of a shop listing.
        public async Task<IActionResult> Update(int id, [FromBody] UpdateShopItemRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var item = await _shopItemService.UpdateShopItem(id, request); // Save updated shop listing
            return Ok(new ApiResponse<ShopItemResponseDto> { Success = true, Data = item });
        }
    }
}
