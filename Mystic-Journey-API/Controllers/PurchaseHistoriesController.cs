using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    // Quản lý lịch sử mua hàng (purchase histories).
    // Game APIs: Xem lịch sử mua của player.
    // Admin APIs: Xem tất cả lịch sử mua.
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseHistoriesController : ControllerBase
    {
        private readonly IPurchaseHistoryService _purchaseHistoryService;

        public PurchaseHistoriesController(IPurchaseHistoryService purchaseHistoryService)
        {
            _purchaseHistoryService = purchaseHistoryService;
        }

        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lịch sử giao dịch là dữ liệu riêng tư: [Authorize] chỉ chứng minh "có đăng nhập",
        // không chứng minh "là chủ của playerProfileId trong route". Thiếu bước đối chiếu này
        // thì bất kỳ người chơi nào cũng đọc được lịch sử mua của người khác bằng cách đổi id.
        private bool IsSelfOrAdmin(int playerProfileId)
        {
            if (User.IsInRole("Admin"))
                return true;

            var claim = User.FindFirstValue("playerProfileId");
            return int.TryParse(claim, out var self) && self == playerProfileId;
        }

        // ── GET /api/purchasehistories/player/{playerProfileId} ────────────
        // Lấy lịch sử mua của player.
        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpGet("player/{playerProfileId}")]
        public async Task<IActionResult> GetByPlayerId(int playerProfileId)
        {
            if (!IsSelfOrAdmin(playerProfileId))
                return StatusCode(403, new ApiResponse<object> { Success = false, Message = "You can only view your own purchase history.", ErrorCode = ErrorCodes.Forbidden });

            var result = await _purchaseHistoryService.GetPurchasesByPlayerId(playerProfileId);
            return Ok(new ApiResponse<List<PurchaseHistoryResponseDto>> { Success = true, Data = result });
        }

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // ── GET /api/purchasehistories ────────────────────────────────
        // Lấy tất cả lịch sử mua có phân trang và lọc.
        // Query: page, pageSize, search, sortBy, sortOrder.
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = null)
        {
            var result = await _purchaseHistoryService.GetPurchaseHistoriesPaged(page, pageSize, search, sortBy, sortOrder);
            return Ok(new ApiResponse<PagedResultDto<PurchaseHistoryResponseDto>> { Success = true, Data = result });
        }
    }
}
