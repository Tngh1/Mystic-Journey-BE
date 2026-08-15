using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Security.Claims;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/shop")]
    [ApiController]
    public class ShopController : ControllerBase
    {
        private readonly IPlayerShopService _shopService;

        public ShopController(IPlayerShopService shopService)
        {
            _shopService = shopService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ViewShop([FromQuery] ViewShopQueryDto query)
            => await GetFixedItems(query);

        [Authorize]
        [HttpGet("items")]
        public async Task<IActionResult> GetItems([FromQuery] ViewShopQueryDto query)
            => await GetFixedItems(query);

        [Authorize]
        [HttpGet("fixed")]
        public async Task<IActionResult> GetFixedItems([FromQuery] ViewShopQueryDto query)
        {
            if (!ModelState.IsValid)
                return ValidationError();

            var playerProfileId = GetCurrentPlayerProfileId();
            var result = await _shopService.GetShop(playerProfileId, query);

            return Ok(new ApiResponse<PagedResultDto<ShopItemPublicResponseDto>>
            {
                Success = true,
                Data = result
            });
        }

        [Authorize]
        [HttpGet("daily-deals")]
        public async Task<IActionResult> GetDailyDeals([FromQuery] ViewShopQueryDto query)
        {
            if (!ModelState.IsValid)
                return ValidationError();

            var playerProfileId = GetCurrentPlayerProfileId();
            var result = await _shopService.GetDailyDeals(playerProfileId, query);

            return Ok(new ApiResponse<PagedResultDto<ShopItemPublicResponseDto>>
            {
                Success = true,
                Data = result
            });
        }

        [Authorize]
        [HttpGet("refresh-status")]
        public async Task<IActionResult> GetRefreshStatus()
            => await GetDailyDealsRefreshStatus();

        [Authorize]
        [HttpGet("daily-deals/refresh-status")]
        public async Task<IActionResult> GetDailyDealsRefreshStatus()
        {
            var playerProfileId = GetCurrentPlayerProfileId();
            var result = await _shopService.GetRefreshStatus(playerProfileId);

            return Ok(new ApiResponse<ShopRefreshStatusDto>
            {
                Success = true,
                Data = result
            });
        }

        [Authorize]
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshShop([FromQuery] ViewShopQueryDto query)
            => await RefreshDailyDeals(query);

        [Authorize]
        [HttpPost("daily-deals/refresh")]
        public async Task<IActionResult> RefreshDailyDeals([FromQuery] ViewShopQueryDto query)
        {
            if (!ModelState.IsValid)
                return ValidationError();

            var playerProfileId = GetCurrentPlayerProfileId();
            var result = await _shopService.RefreshDailyDeals(playerProfileId, query);

            return Ok(new ApiResponse<ShopRefreshResponseDto>
            {
                Success = true,
                Message = result.Message,
                Data = result
            });
        }

        [Authorize]
        [HttpPost("purchase")]
        public async Task<IActionResult> PurchaseItem([FromBody] PurchaseShopItemRequestDto request)
        {
            if (!ModelState.IsValid)
                return ValidationError();

            var playerProfileId = GetCurrentPlayerProfileId();
            var result = await _shopService.PurchaseItem(playerProfileId, request);

            return Ok(new ApiResponse<PurchaseShopItemResponseDto>
            {
                Success = true,
                Message = result.Message,
                Data = result
            });
        }

        [Authorize]
        [HttpGet("skins")]
        public async Task<IActionResult> GetSkins()
        {
            var playerProfileId = GetCurrentPlayerProfileId();
            var result = await _shopService.GetSkinShop(playerProfileId);
            return Ok(new ApiResponse<IReadOnlyList<SkinShopItemResponseDto>>
            {
                Success = true,
                Data = result
            });
        }

        [Authorize]
        [HttpPost("skins/purchase")]
        public async Task<IActionResult> PurchaseSkin([FromBody] PurchaseShopSkinRequestDto request)
        {
            if (!ModelState.IsValid)
                return ValidationError();

            var playerProfileId = GetCurrentPlayerProfileId();
            var result = await _shopService.PurchaseSkin(playerProfileId, request);
            return Ok(new ApiResponse<PurchaseShopSkinResponseDto>
            {
                Success = true,
                Message = result.Message,
                Data = result
            });
        }

        private IActionResult ValidationError()
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Validation failed.",
                ErrorCode = ErrorCodes.ValidationError
            });
        }

        private int GetCurrentPlayerProfileId()
        {
            var claim = User.FindFirstValue("playerProfileId");
            if (!int.TryParse(claim, out var profileId))
                throw new UnauthorizedAccessException("PlayerProfileId is missing from token. Please login again.");

            return profileId;
        }
    }
}
