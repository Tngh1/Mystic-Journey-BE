using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Collections.Generic;
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

        // ── GET /api/purchasehistories/player/{playerProfileId} ────────────
        // Lấy lịch sử mua của player.
        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpGet("player/{playerProfileId}")]
        public async Task<IActionResult> GetByPlayerId(int playerProfileId)
        {
            var result = await _purchaseHistoryService.GetPurchasesByPlayerId(playerProfileId);
            return Ok(new ApiResponse<List<PurchaseHistoryResponseDto>> { Success = true, Data = result });
        }

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // ── GET /api/purchasehistories ────────────────────────────────
        // Lấy tất cả lịch sử mua có phân trang và lọc.
        // Query: page, pageSize, search.
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
        {
            var result = await _purchaseHistoryService.GetPurchaseHistoriesPaged(page, pageSize, search);
            return Ok(new ApiResponse<PagedResultDto<PurchaseHistoryResponseDto>> { Success = true, Data = result });
        }
    }
}
