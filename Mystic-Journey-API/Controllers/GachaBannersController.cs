using BLL.DTOs;
using BLL.Services.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/[controller]")]
    // Executes controller base operation.
    [ApiController]
    public class GachaBannersController : ControllerBase
    {
        private readonly IGachaBannerService _gachaBannerService;

        // Initializes a new instance of GachaBannersController with dependencies: gachaBannerService.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public GachaBannersController(IGachaBannerService gachaBannerService)
        {
            _gachaBannerService = gachaBannerService;
        }


        // ─── Player APIs ───────────────────────────────────────────────────────
        [Authorize]
        [HttpGet("{id}")]
        // Retrieves detailed gacha banner information, rate-up featured items, pity rules, and drop rates.
        public async Task<IActionResult> GetById(int id)
        {
            var banner = await _gachaBannerService.GetBannerById(id); // Load banner configuration and rate-up item pools
            if (banner == null)  // Entity not found — short-circuit with appropriate error result
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Gacha banner with id {id} not found.", ErrorCode = ErrorCodes.NotFound });  // Return HTTP 404 when the requested resource does not exist

            return Ok(new ApiResponse<GachaBannerDetailResponseDto> { Success = true, Data = banner });  // Return HTTP 200 with standard ApiResponse envelope
        }

        [HttpGet]
        // Retrieves list of currently active or all gacha banners with pagination and type filtering.
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? type = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = null)
        {
            var result = await _gachaBannerService.GetBannersPaged(page, pageSize, search, type, isActive, sortBy, sortOrder); // Query active gacha banners from database
            return Ok(new ApiResponse<PagedResultDto<GachaBannerResponseDto>> { Success = true, Data = result });  // Return HTTP 200 with standard ApiResponse envelope
        }

        [HttpGet("items-paged")]
        // Retrieves paginated list of all drop table items configured across gacha banners.
        public async Task<IActionResult> GetItemsPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _gachaBannerService.GetBannerItemsPaged(page, pageSize); // Query item drop tables and rarity weights
            return Ok(new ApiResponse<PagedResultDto<GachaBannerItemResponseDto>> { Success = true, Data = result });  // Return HTTP 200 with standard ApiResponse envelope
        }

        // Extracts caller's player profile ID integer from the JWT claim.
        // Validates input parameters against null or empty values.
        private int GetPlayerProfileId()
        {
            var claim = User.FindFirstValue("playerProfileId");
            if (string.IsNullOrEmpty(claim) || !int.TryParse(claim, out var profileId))  // Mandatory string argument is null or empty — fail fast
            {
                return 0; // Return 0 if claim is absent or invalid
            }
            return profileId;
        }

        [Authorize]
        [HttpPost("{id}/pull")]
        // Executes 1x or 10x gacha pull, calculates pity counters, rolls weighted random items, and deposits rewards.
        public async Task<IActionResult> Pull(int id, [FromBody] GachaPullRequestDto request)
        {
            try
            {
                var playerProfileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
                if (playerProfileId == 0)
                    return Unauthorized(new ApiResponse<object> { Success = false, Message = "Profile not found.", ErrorCode = ErrorCodes.Unauthorized });

                var result = await _gachaBannerService.Pull(playerProfileId, id, request); // Verify currency/tickets, execute weighted roll with pity logic, add to inventory, and log history
                return Ok(new ApiResponse<MultiPullResultDto> { Success = true, Data = result });  // Return HTTP 200 with standard ApiResponse envelope
            }
            catch (System.InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = ex.Message, ErrorCode = ErrorCodes.InvalidOperation });  // Return HTTP 400 with validation error details
            }
            catch (System.Collections.Generic.KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = ex.Message, ErrorCode = ErrorCodes.NotFound });  // Return HTTP 404 when the requested resource does not exist
            }
        }

        [Authorize]
        [HttpGet("history")]
        // Retrieves paginated pull history log for the authenticated player.
        public async Task<IActionResult> GetHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var playerProfileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Profile not found.", ErrorCode = ErrorCodes.Unauthorized });

            var result = await _gachaBannerService.GetHistoryPaged(playerProfileId, page, pageSize); // Load historical pull audit logs (items, rarities, timestamps)
            return Ok(new ApiResponse<PagedResultDto<GachaPullHistoryResponseDto>> { Success = true, Data = result });  // Return HTTP 200 with standard ApiResponse envelope
        }


        // ─── Admin APIs ───────────────────────────────────────────────────────
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        // Per-frame update loop for GachaBannersController.
        // Handles real-time input polling, smooth interpolations, cooldown timers, and UI updates.
        public async Task<IActionResult> Update(int id, [FromBody] UpdateGachaBannerRequestDto request)
        {
            if (!ModelState.IsValid)  // Reject request immediately if any DTO validation annotation fails
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });  // Return HTTP 400 with validation error details

            var banner = await _gachaBannerService.UpdateBanner(id, request);
            return Ok(new ApiResponse<GachaBannerResponseDto> { Success = true, Data = banner });  // Return HTTP 200 with standard ApiResponse envelope
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/items")]
        // Executes add banner item operation.
        public async Task<IActionResult> AddBannerItem(int id, [FromBody] CreateGachaBannerItemRequestDto request)
        {
            if (!ModelState.IsValid)  // Reject request immediately if any DTO validation annotation fails
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });  // Return HTTP 400 with validation error details

            var bannerItem = await _gachaBannerService.AddBannerItem(id, request);
            return Ok(new ApiResponse<GachaBannerItemResponseDto> { Success = true, Data = bannerItem });  // Return HTTP 200 with standard ApiResponse envelope
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        // Executes create operation.
        public async Task<IActionResult> Create([FromBody] CreateGachaBannerRequestDto request)
        {
            if (!ModelState.IsValid)  // Reject request immediately if any DTO validation annotation fails
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });  // Return HTTP 400 with validation error details

            var banner = await _gachaBannerService.CreateBanner(request);
            return Ok(new ApiResponse<GachaBannerResponseDto> { Success = true, Data = banner });  // Return HTTP 200 with standard ApiResponse envelope
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{bannerId}/items/{bannerItemId}")]
        // Executes remove banner item operation.
        public async Task<IActionResult> RemoveBannerItem(int bannerId, int bannerItemId)
        {
            var removed = await _gachaBannerService.RemoveBannerItem(bannerId, bannerItemId);
            if (!removed)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Banner item not found.", ErrorCode = ErrorCodes.NotFound });  // Return HTTP 404 when the requested resource does not exist
            return Ok(new ApiResponse<object> { Success = true, Message = "Item removed from banner." });  // Return HTTP 200 with standard ApiResponse envelope
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("history/admin")]
        // Load all history using page, page size, banner id, and rarity; it loads all history paged.
        public async Task<IActionResult> GetAllHistory(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] int? bannerId = null,
            [FromQuery] string? rarity = null)
        {
            var result = await _gachaBannerService.GetAllHistoryPaged(page, pageSize, bannerId, rarity);
            return Ok(new ApiResponse<PagedResultDto<GachaPullHistoryResponseDto>> { Success = true, Data = result });  // Return HTTP 200 with standard ApiResponse envelope
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("history/admin/stats/{playerProfileId}")]
        // Executes get player gacha stats operation.
        public async Task<IActionResult> GetPlayerGachaStats(int playerProfileId)
        {
            var stats = await _gachaBannerService.GetPlayerGachaStats(playerProfileId);
            if (stats == null)  // Entity not found — short-circuit with appropriate error result
                return NotFound(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.NotFound });  // Return HTTP 404 when the requested resource does not exist
            return Ok(new ApiResponse<PlayerGachaStatsDto> { Success = true, Data = stats });  // Return HTTP 200 with standard ApiResponse envelope
        }
    }
}
