using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopItemsController : ControllerBase
    {
        private readonly IShopItemService _shopItemService;

        public ShopItemsController(IShopItemService shopItemService)
        {
            _shopItemService = shopItemService;
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _shopItemService.GetShopItemById(id);
            if (item == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Shop item with id {id} not found.", ErrorCode = ErrorCodes.NotFound });

            return Ok(new ApiResponse<ShopItemResponseDto> { Success = true, Data = item });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateShopItemRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var item = await _shopItemService.CreateShopItem(request);
            return Ok(new ApiResponse<ShopItemResponseDto> { Success = true, Data = item });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateShopItemRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var item = await _shopItemService.UpdateShopItem(id, request);
            return Ok(new ApiResponse<ShopItemResponseDto> { Success = true, Data = item });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] string? currency = null, [FromQuery] bool? isActive = null)
        {
            var result = await _shopItemService.GetShopItemsPaged(page, pageSize, search, currency, isActive);
            return Ok(new ApiResponse<PagedResultDto<ShopItemResponseDto>> { Success = true, Data = result });
        }
    }
}
