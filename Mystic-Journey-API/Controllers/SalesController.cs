using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    // Quản lý lịch sử bán (sales).
    // Game APIs: Xem lịch sử bán của player.
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly ISaleService _saleService;

        public SalesController(ISaleService saleService)
        {
            _saleService = saleService;
        }

        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // ── GET /api/sales/player/{playerProfileId} ─────────────────
        // Lấy lịch sử bán của player.
        [Authorize]
        [HttpGet("player/{playerProfileId}")]
        public async Task<IActionResult> GetByPlayerId(int playerProfileId)
        {
            var result = await _saleService.GetSalesByPlayerId(playerProfileId);
            return Ok(new ApiResponse<List<PurchaseHistoryResponseDto>> { Success = true, Data = result });
        }
    }
}
