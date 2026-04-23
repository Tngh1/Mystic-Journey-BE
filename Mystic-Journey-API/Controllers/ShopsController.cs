using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ShopsController : ControllerBase
    {
        private readonly IShopService _shopService;

        public ShopsController(IShopService shopService)
        {
            _shopService = shopService;
        }

        private Guid GetAccountId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllShopItems()
        {
            var result = await _shopService.GetAllShopItemsAsync();
            return Ok(result);
        }

        [HttpGet("available")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailableItems()
        {
            var result = await _shopService.GetAvailableItemsAsync();
            return Ok(result);
        }

        [HttpGet("{shopItemId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetShopItemById(Guid shopItemId)
        {
            var result = await _shopService.GetShopItemByIdAsync(shopItemId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost("purchase")]
        public async Task<IActionResult> PurchaseItem([FromBody] PurchaseRequestDto request)
        {
            var accountId = GetAccountId();
            var result = await _shopService.PurchaseItemAsync(accountId, request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetPurchaseHistory([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var accountId = GetAccountId();
            var result = await _shopService.GetPurchaseHistoryAsync(accountId, pageNumber, pageSize);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}
