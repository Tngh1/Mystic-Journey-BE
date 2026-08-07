using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Collections.Generic;
using System.Security.Claims;
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

        // Lịch sử giao dịch là dữ liệu riêng tư: [Authorize] chỉ chứng minh "có đăng nhập",
        // không chứng minh "là chủ của playerProfileId trong route". Thiếu bước đối chiếu này
        // thì bất kỳ người chơi nào cũng đọc được lịch sử bán của người khác bằng cách đổi id.
        private bool IsSelfOrAdmin(int playerProfileId)
        {
            if (User.IsInRole("Admin") || User.IsInRole("SuperAdmin"))
                return true;

            var claim = User.FindFirstValue("playerProfileId");
            return int.TryParse(claim, out var self) && self == playerProfileId;
        }

        // ── GET /api/sales/player/{playerProfileId} ─────────────────
        // Lấy lịch sử bán của player.
        [Authorize]
        [HttpGet("player/{playerProfileId}")]
        public async Task<IActionResult> GetByPlayerId(int playerProfileId)
        {
            if (!IsSelfOrAdmin(playerProfileId))
                return StatusCode(403, new ApiResponse<object> { Success = false, Message = "You can only view your own sales history.", ErrorCode = ErrorCodes.Forbidden });

            var result = await _saleService.GetSalesByPlayerId(playerProfileId);
            return Ok(new ApiResponse<List<PurchaseHistoryResponseDto>> { Success = true, Data = result });
        }
    }
}
