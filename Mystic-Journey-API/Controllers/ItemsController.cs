using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    // Quản lý items (vật phẩm) trong game.
    // Game APIs: Xem danh sách, xem chi tiết item.
    // Admin APIs: Tạo, cập nhật item.
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IItemService _itemService;

        public ItemsController(IItemService itemService)
        {
            _itemService = itemService;
        }

        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // ── GET /api/items/{id} ─────────────────────────────────────
        // Lấy chi tiết item theo ID.
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _itemService.GetItemById(id);
            if (item == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Item with id {id} not found.", ErrorCode = ErrorCodes.NotFound });

            return Ok(new ApiResponse<ItemResponseDto> { Success = true, Data = item });
        }

        // ── GET /api/items ──────────────────────────────────────────
        // Lấy danh sách tất cả items có phân trang và lọc.
        // Query: page, pageSize, search, type, rarity, isActive, sortBy, sortOrder.
        [HttpGet]
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
            return Ok(new ApiResponse<PagedResultDto<ItemResponseDto>> { Success = true, Data = result });
        }

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // NOTE: Create endpoint removed - managed via seeding.

        // ── PUT /api/items/{id} ─────────────────────────────────────
        // Cập nhật item hiện có.
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateItemRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var item = await _itemService.UpdateItem(id, request);
            return Ok(new ApiResponse<ItemResponseDto> { Success = true, Data = item });
        }
    }
}
