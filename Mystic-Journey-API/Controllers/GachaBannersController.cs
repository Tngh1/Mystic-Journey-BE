using BLL.DTOs;
using BLL.Services.Interfaces;
using System.Security.Claims;
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
        // Lấy chi tiết gacha banner theo ID. Yêu cầu đăng nhập: đây là dữ liệu
        // trong game, web wiki không hiển thị banner.
        [Authorize]
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
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? type = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = null)
        {
            var result = await _gachaBannerService.GetBannersPaged(page, pageSize, search, type, isActive, sortBy, sortOrder);
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

        private int GetPlayerProfileId()
        {
            var claim = User.FindFirstValue("playerProfileId");
            if (string.IsNullOrEmpty(claim) || !int.TryParse(claim, out var profileId))
            {
                return 0;
            }
            return profileId;
        }

        // ── POST /api/gachabanners/{id}/pull ───────────────────────
        // Thực hiện quay gacha
        [Authorize]
        [HttpPost("{id}/pull")]
        public async Task<IActionResult> Pull(int id, [FromBody] GachaPullRequestDto request)
        {
            try
            {
                var playerProfileId = GetPlayerProfileId();
                if (playerProfileId == 0)
                    return Unauthorized(new ApiResponse<object> { Success = false, Message = "Profile not found.", ErrorCode = ErrorCodes.Unauthorized });

                var result = await _gachaBannerService.Pull(playerProfileId, id, request);
                return Ok(new ApiResponse<MultiPullResultDto> { Success = true, Data = result });
            }
            catch (System.InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = ex.Message, ErrorCode = ErrorCodes.InvalidOperation });
            }
            catch (System.Collections.Generic.KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = ex.Message, ErrorCode = ErrorCodes.NotFound });
            }
        }

        // ── GET /api/gachabanners/history ──────────────────────────
        // Lấy lịch sử quay của người chơi hiện tại
        [Authorize]
        [HttpGet("history")]
        public async Task<IActionResult> GetHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var playerProfileId = GetPlayerProfileId();
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Profile not found.", ErrorCode = ErrorCodes.Unauthorized });

            var result = await _gachaBannerService.GetHistoryPaged(playerProfileId, page, pageSize);
            return Ok(new ApiResponse<PagedResultDto<GachaPullHistoryResponseDto>> { Success = true, Data = result });
        }

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════
        // NOTE: Create endpoint removed - managed via seeding.

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
        // ══ POST /api/gachabanners ══════════════════════════════
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

        // ══ DELETE /api/gachabanners/{bannerId}/items/{bannerItemId} ══
        // Xóa item khỏi banner.
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpDelete("{bannerId}/items/{bannerItemId}")]
        public async Task<IActionResult> RemoveBannerItem(int bannerId, int bannerItemId)
        {
            var removed = await _gachaBannerService.RemoveBannerItem(bannerId, bannerItemId);
            if (!removed)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Banner item not found.", ErrorCode = ErrorCodes.NotFound });
            return Ok(new ApiResponse<object> { Success = true, Message = "Item removed from banner." });
        }

        // ══ GET /api/gachabanners/history/admin ═══════════════
        // Admin xem toàn bộ lịch sử quay.
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet("history/admin")]
        public async Task<IActionResult> GetAllHistory(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] int? bannerId = null,
            [FromQuery] string? rarity = null)
        {
            var result = await _gachaBannerService.GetAllHistoryPaged(page, pageSize, bannerId, rarity);
            return Ok(new ApiResponse<PagedResultDto<GachaPullHistoryResponseDto>> { Success = true, Data = result });
        }
        // ══ GET /api/gachabanners/history/admin/stats/{playerProfileId} ══
        // Admin xem thống kê gacha của người chơi.
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet("history/admin/stats/{playerProfileId}")]
        public async Task<IActionResult> GetPlayerGachaStats(int playerProfileId)
        {
            var stats = await _gachaBannerService.GetPlayerGachaStats(playerProfileId);
            if (stats == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.NotFound });
            return Ok(new ApiResponse<PlayerGachaStatsDto> { Success = true, Data = stats });
        }
    }
}
