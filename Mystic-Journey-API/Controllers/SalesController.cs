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
    [Route("api/[controller]")]
    // Executes controller base operation.
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly ISaleService _saleService;

        // Initializes a new instance of SalesController with dependencies: saleService.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public SalesController(ISaleService saleService)
        {
            _saleService = saleService;
        }


        // Checks caller permissions (self profile match or Admin role).
        private bool IsSelfOrAdmin(int playerProfileId)
        {
            if (User.IsInRole("Admin"))
                return true; // Admins have global audit access

            var claim = User.FindFirstValue("playerProfileId");
            return int.TryParse(claim, out var self) && self == playerProfileId; // Match token profile
        }

        // ─── Player APIs ───────────────────────────────────────────────────────
        [Authorize]
        [HttpGet("player/{playerProfileId}")]
        // Retrieves item and gear sell-back transactions for a player profile.
        public async Task<IActionResult> GetByPlayerId(int playerProfileId)
        {
            if (!IsSelfOrAdmin(playerProfileId))
                return StatusCode(403, new ApiResponse<object> { Success = false, Message = "You can only view your own sales history.", ErrorCode = ErrorCodes.Forbidden });

            var result = await _saleService.GetSalesByPlayerId(playerProfileId); // Query sales ledger
            return Ok(new ApiResponse<List<PurchaseHistoryResponseDto>> { Success = true, Data = result });
        }
    }
}
