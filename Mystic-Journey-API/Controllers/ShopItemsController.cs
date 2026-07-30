using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    // Quản lý shop items (vật phẩm trong cửa hàng).
    // Game APIs: Xem danh sách, xem chi tiết item.
    // Admin APIs: Tạo, cập nhật shop item.
    [Route("api/[controller]")]
    [ApiController]
    public class ShopItemsController : ControllerBase
    {
        private readonly IShopItemService _shopItemService;

        public ShopItemsController(IShopItemService shopItemService)
        {
            _shopItemService = shopItemService;
        }

        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // ── GET /api/shopitems/{id} ─────────────────────────────────
        // Lấy chi tiết shop item theo ID. Yêu cầu đăng nhập: đây là dữ liệu
        // trong game, web wiki không hiển thị shop.
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _shopItemService.GetShopItemById(id);
            if (item == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Shop item with id {id} not found.", ErrorCode = ErrorCodes.NotFound });

            return Ok(new ApiResponse<ShopItemResponseDto> { Success = true, Data = item });
        }

        // ── GET /api/shopitems ──────────────────────────────────────
        // Lấy danh sách tất cả shop items có phân trang và lọc.
        // Query: page, pageSize, search, currency, isActive, sortBy, sortOrder.
        [HttpGet]
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
            var result = await _shopItemService.GetShopItemsPaged(page, pageSize, search, currency, shopSection, isActive, sortBy, sortOrder);
            return Ok(new ApiResponse<PagedResultDto<ShopItemResponseDto>> { Success = true, Data = result });
        }

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // ── POST /api/shopitems ─────────────────────────────────────
        // Tạo shop item mới.
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateShopItemRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var item = await _shopItemService.CreateShopItem(request);
            return Ok(new ApiResponse<ShopItemResponseDto> { Success = true, Data = item });
        }

        // ── PUT /api/shopitems/{id} ─────────────────────────────────
        // Cập nhật shop item hiện có.
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateShopItemRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var item = await _shopItemService.UpdateShopItem(id, request);
            return Ok(new ApiResponse<ShopItemResponseDto> { Success = true, Data = item });
        }
    }
}
