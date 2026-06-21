using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseHistoriesController : ControllerBase
    {
        private readonly IPurchaseHistoryService _purchaseHistoryService;

        public PurchaseHistoriesController(IPurchaseHistoryService purchaseHistoryService)
        {
            _purchaseHistoryService = purchaseHistoryService;
        }

        // ========== PLAYER: View Own Purchase History ==========
        // Dành cho người chơi - Xem lịch sử mua hàng của mình

        [Authorize]
        [HttpGet("player/{playerProfileId}")]
        public async Task<IActionResult> GetByPlayerId(int playerProfileId)
        {
            try
            {
                var result = await _purchaseHistoryService.GetPurchasesByPlayerId(playerProfileId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // ========== MANAGER: Purchase History Management (Dashboard) ==========
        // Dành cho Admin/Manager - Quản lý lịch sử mua hàng trên dashboard

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
        {
            var result = await _purchaseHistoryService.GetPurchaseHistoriesPaged(page, pageSize, search);
            return Ok(result);
        }
    }
}
