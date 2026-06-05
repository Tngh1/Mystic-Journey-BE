using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

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

        [Authorize(Roles = "Admin")]
        [HttpGet("/odata/PurchaseHistories")]
        [EnableQuery]
        public IActionResult GetOData()
        {
            return Ok(_purchaseHistoryService.GetPurchaseHistoriesQueryable());
        }
    }
}
