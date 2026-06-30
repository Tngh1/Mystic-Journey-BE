using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    // Quản lý gacha banners (banner gacha/quay thưởng).
    // Game APIs: Xem danh sách, xem chi tiết banner.
    // Admin APIs: Tạo, cập nhật banner và thêm items.
    [Route("api/[controller]")]
    [ApiController]
    public class GachaBannersController : ControllerBase
    {
        private readonly IGachaBannerService _gachaBannerService;

        public GachaBannersController(IGachaBannerService gachaBannerService)
        {
            _gachaBannerService = gachaBannerService;
        }

        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // ── GET /api/gachabanners/{id} ────────────────────────────
        // Lấy chi tiết gacha banner theo ID.
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var banner = await _gachaBannerService.GetBannerById(id);
            if (banner == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Gacha banner with id {id} not found.", ErrorCode = ErrorCodes.NotFound });

            return Ok(new ApiResponse<GachaBannerDetailResponseDto> { Success = true, Data = banner });
        }

        // ── GET /api/gachabanners ──────────────────────────────────
        // Lấy danh sách tất cả gacha banners có phân trang và lọc.
        // Query: page, pageSize, search, type, isActive.
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] string? type = null, [FromQuery] bool? isActive = null)
        {
            var result = await _gachaBannerService.GetBannersPaged(page, pageSize, search, type, isActive);
            return Ok(new ApiResponse<PagedResultDto<GachaBannerResponseDto>> { Success = true, Data = result });
        }

        // ── GET /api/gachabanners/items-paged ──────────────────────
        // Lấy danh sách banner items có phân trang.
        [HttpGet("items-paged")]
        public async Task<IActionResult> GetItemsPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _gachaBannerService.GetBannerItemsPaged(page, pageSize);
            return Ok(new ApiResponse<PagedResultDto<GachaBannerItemResponseDto>> { Success = true, Data = result });
        }

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // ── POST /api/gachabanners ────────────────────────────────
        // Tạo gacha banner mới.
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGachaBannerRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var banner = await _gachaBannerService.CreateBanner(request);
            return Ok(new ApiResponse<GachaBannerResponseDto> { Success = true, Data = banner });
        }

        // ── PUT /api/gachabanners/{id} ────────────────────────────
        // Cập nhật gacha banner hiện có.
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateGachaBannerRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var banner = await _gachaBannerService.UpdateBanner(id, request);
            return Ok(new ApiResponse<GachaBannerResponseDto> { Success = true, Data = banner });
        }

        // ── POST /api/gachabanners/{id}/items ─────────────────────
        // Thêm item vào banner.
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost("{id}/items")]
        public async Task<IActionResult> AddBannerItem(int id, [FromBody] CreateGachaBannerItemRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var bannerItem = await _gachaBannerService.AddBannerItem(id, request);
            return Ok(new ApiResponse<GachaBannerItemResponseDto> { Success = true, Data = bannerItem });
        }
    }
}
