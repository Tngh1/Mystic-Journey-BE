using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Security.Claims;

namespace Mystic_Journey_API.Controllers
{
    // Executes controller base operation.
    [Route("api/shop")]
    [ApiController]
    public class ShopController : ControllerBase
    {
        private readonly IPlayerShopService _shopService;

        // Initializes a new instance of ShopController with dependencies: shopService.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public ShopController(IPlayerShopService shopService)
        {
            _shopService = shopService;
        }

        // ─── Player APIs ───────────────────────────────────────────────────────
        [Authorize]
        [HttpGet]
        // View standard shop catalog items (alias for fixed items).
        public async Task<IActionResult> ViewShop([FromQuery] ViewShopQueryDto query)
            => await GetFixedItems(query);

        [Authorize]
        [HttpGet("items")]
        // Get paginated shop items (alias for fixed items).
        public async Task<IActionResult> GetItems([FromQuery] ViewShopQueryDto query)
            => await GetFixedItems(query);

        [Authorize]
        [HttpGet("fixed")]
        // Retrieves regular/fixed shop items with personal stock and purchase limits.
        public async Task<IActionResult> GetFixedItems([FromQuery] ViewShopQueryDto query)
        {
            if (!ModelState.IsValid) // Guard against invalid query parameters (e.g. negative page numbers)
                return ValidationError();

            var playerProfileId = GetCurrentPlayerProfileId(); // Extract caller's profile ID from JWT claim
            var result = await _shopService.GetShop(playerProfileId, query); // Load active catalog items with player-specific daily/weekly limits

            return Ok(new ApiResponse<PagedResultDto<ShopItemPublicResponseDto>> // Return HTTP 200 with paginated catalog list
            {
                Success = true,
                Data = result
            });
        }

        [Authorize]
        [HttpGet("daily-deals")]
        // Retrieves player's personalized daily deal item offers with discounted prices.
        public async Task<IActionResult> GetDailyDeals([FromQuery] ViewShopQueryDto query)
        {
            if (!ModelState.IsValid) // Validate incoming pagination query parameters
                return ValidationError();

            var playerProfileId = GetCurrentPlayerProfileId(); // Extract caller's profile ID from JWT claim
            var result = await _shopService.GetDailyDeals(playerProfileId, query); // Generate or fetch daily deal rotation for this player

            return Ok(new ApiResponse<PagedResultDto<ShopItemPublicResponseDto>> // Return HTTP 200 with daily deals list
            {
                Success = true,
                Data = result
            });
        }

        [Authorize]
        [HttpGet("refresh-status")]
        // Retrieves free refresh availability and countdown timer (alias).
        public async Task<IActionResult> GetRefreshStatus()
            => await GetDailyDealsRefreshStatus();

        [Authorize]
        [HttpGet("daily-deals/refresh-status")]
        // Returns remaining free refreshes and cost for paid manual shop reset.
        public async Task<IActionResult> GetDailyDealsRefreshStatus()
        {
            var playerProfileId = GetCurrentPlayerProfileId(); // Extract caller's profile ID from JWT claim
            var result = await _shopService.GetRefreshStatus(playerProfileId); // Calculate next free reset time and remaining daily refresh count

            return Ok(new ApiResponse<ShopRefreshStatusDto> // Return HTTP 200 with refresh status details
            {
                Success = true,
                Data = result
            });
        }

        [Authorize]
        [HttpPost("refresh")]
        // Trigger manual shop refresh (alias).
        public async Task<IActionResult> RefreshShop([FromQuery] ViewShopQueryDto query)
            => await RefreshDailyDeals(query);

        [Authorize]
        [HttpPost("daily-deals/refresh")]
        // Manually rolls a fresh set of daily deals, consuming a free reset or charging gold/gems.
        public async Task<IActionResult> RefreshDailyDeals([FromQuery] ViewShopQueryDto query)
        {
            if (!ModelState.IsValid) // Validate query parameters
                return ValidationError();

            var playerProfileId = GetCurrentPlayerProfileId(); // Extract caller's profile ID from JWT claim
            var result = await _shopService.RefreshDailyDeals(playerProfileId, query); // Validate refresh cooldown/cost and re-roll daily deal slots

            return Ok(new ApiResponse<ShopRefreshResponseDto> // Return HTTP 200 with new daily deals and updated refresh counters
            {
                Success = true,
                Message = result.Message,
                Data = result
            });
        }

        [Authorize]
        [HttpPost("purchase")]
        // Executes atomic shop item purchase within a transaction.
        // Validates player balance, daily/weekly limits, deducts currency, and adds items to inventory.
        public async Task<IActionResult> PurchaseItem([FromBody] PurchaseShopItemRequestDto request)
        {
            if (!ModelState.IsValid) // Reject if requested quantity is zero/negative or missing ItemId
                return ValidationError();

            var playerProfileId = GetCurrentPlayerProfileId(); // Extract caller's profile ID from JWT claim
            var result = await _shopService.PurchaseItem(playerProfileId, request); // Execute purchase transaction (balance deduction, stock decrement, inventory insert)

            return Ok(new ApiResponse<PurchaseShopItemResponseDto> // Return HTTP 200 with purchase receipt and updated balances
            {
                Success = true,
                Message = result.Message,
                Data = result
            });
        }

        [Authorize]
        [HttpGet("skins")]
        // Retrieves list of premium character skins available for purchase.
        public async Task<IActionResult> GetSkins()
        {
            var playerProfileId = GetCurrentPlayerProfileId(); // Extract caller's profile ID from JWT claim
            var result = await _shopService.GetSkinShop(playerProfileId); // Load available skins and check ownership flags
            return Ok(new ApiResponse<IReadOnlyList<SkinShopItemResponseDto>> // Return HTTP 200 with skin catalog
            {
                Success = true,
                Data = result
            });
        }

        [Authorize]
        [HttpPost("skins/purchase")]
        // Purchases a character skin for the player, deducting currency and recording skin ownership.
        public async Task<IActionResult> PurchaseSkin([FromBody] PurchaseShopSkinRequestDto request)
        {
            if (!ModelState.IsValid) // Validate skin ID parameter
                return ValidationError();

            var playerProfileId = GetCurrentPlayerProfileId(); // Extract caller's profile ID from JWT claim
            var result = await _shopService.PurchaseSkin(playerProfileId, request); // Verify unowned status, deduct gems, and unlock skin on player account
            return Ok(new ApiResponse<PurchaseShopSkinResponseDto> // Return HTTP 200 with skin purchase result
            {
                Success = true,
                Message = result.Message,
                Data = result
            });
        }

        // Executes validation error operation.
        private IActionResult ValidationError()
        {
            return BadRequest(new ApiResponse<object>  // Return HTTP 400 with validation error details
            {
                Success = false,
                Message = "Validation failed.",
                ErrorCode = ErrorCodes.ValidationError
            });
        }

        // Executes get current player profile id operation.
        private int GetCurrentPlayerProfileId()
        {
            var claim = User.FindFirstValue("playerProfileId");
            if (!int.TryParse(claim, out var profileId))  // Claim value missing or non-integer — reject as unauthorized
                throw new UnauthorizedAccessException("PlayerProfileId is missing from token. Please login again.");  // Authentication token is invalid or expired

            return profileId;
        }
    }
}
