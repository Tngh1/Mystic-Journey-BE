using BLL.DTOs;
using BLL.Services.Interfaces;
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
    public class PurchaseHistoriesController : ControllerBase
    {
        private readonly IPurchaseHistoryService _purchaseHistoryService;

        // Initializes a new instance of PurchaseHistoriesController with dependencies: purchaseHistoryService.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public PurchaseHistoriesController(IPurchaseHistoryService purchaseHistoryService)
        {
            _purchaseHistoryService = purchaseHistoryService;
        }


        // Verifies whether caller is accessing their own records or has Admin privileges.
        private bool IsSelfOrAdmin(int playerProfileId)
        {
            if (User.IsInRole("Admin"))
                return true; // Admins can view any player's audit logs

            var claim = User.FindFirstValue("playerProfileId");
            return int.TryParse(claim, out var self) && self == playerProfileId; // Check token match
        }

        // ─── Player APIs ───────────────────────────────────────────────────────
        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpGet("player/{playerProfileId}")]
        // Retrieves chronological item purchases, currency costs, and transaction timestamps for a player.
        public async Task<IActionResult> GetByPlayerId(int playerProfileId)
        {
            if (!IsSelfOrAdmin(playerProfileId))
                return StatusCode(403, new ApiResponse<object> { Success = false, Message = "You can only view your own purchase history.", ErrorCode = ErrorCodes.Forbidden });

            var result = await _purchaseHistoryService.GetPurchasesByPlayerId(playerProfileId); // Query purchase ledger
            return Ok(new ApiResponse<List<PurchaseHistoryResponseDto>> { Success = true, Data = result });
        }


        // ─── Admin APIs ───────────────────────────────────────────────────────
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
        [HttpGet]
        // Retrieves paginated list of all server microtransactions for financial auditing.
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = null)
        {
            var result = await _purchaseHistoryService.GetPurchaseHistoriesPaged(page, pageSize, search, sortBy, sortOrder); // Paginated purchase query
            return Ok(new ApiResponse<PagedResultDto<PurchaseHistoryResponseDto>> { Success = true, Data = result });
        }
    }
}
