using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/[controller]")]
    // Executes controller base operation.
    [ApiController]
    public class PlayerProfilesController : ControllerBase
    {
        private readonly IPlayerProfileService _playerProfileService;
        // Initializes a new instance of PlayerProfilesController with dependencies: playerProfileService.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public PlayerProfilesController(IPlayerProfileService playerProfileService)
        {
            _playerProfileService = playerProfileService;
        }

        // Extracts and parses the integer account ID from the JWT NameIdentifier claim.
        // Throws UnauthorizedAccessException if the claim is missing, empty, or not a valid integer.
        private int GetCurrentAccountId()  // Extract authenticated caller's account ID from JWT
        {
            var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }

        // Executes get current player profile id operation.
        private int GetCurrentPlayerProfileId()
        {
            var claim = User.FindFirst("playerProfileId");
            if (claim != null && int.TryParse(claim.Value, out var profileId))
            {
                return profileId;
            }
            return 0;
        }


        // ─── Player APIs ───────────────────────────────────────────────────────
        [HttpGet("{id}")]
        // Executes get by id operation.
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _playerProfileService.GetProfileById(id); // Fetch full player profile details including stats, class, and currency
            return Ok(new ApiResponse<PlayerProfileDetailResponseDto> { Success = true, Data = result }); // Return HTTP 200 wrapped in standard API response envelope
        }

        [Authorize]
        [HttpPut("{id}")]
        // Updates player profile attributes (e.g. bio, preferences).
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePlayerProfileRequestDto dto)
        {
            var result = await _playerProfileService.UpdateProfile(id, dto); // Apply profile updates through domain validation layer
            return Ok(new ApiResponse<PlayerProfileResponseDto> { Success = true, Data = result }); // Return HTTP 200 with updated profile data
        }

        [Authorize]
        [HttpPost("change-name")]
        // Executes player display name change with duplicate check and cost validation.
        public async Task<IActionResult> ChangeName([FromBody] ChangeNameRequestDto request)
        {
            var accountId = GetCurrentAccountId(); // Extract calling user account ID from JWT token claims
            if (accountId == 0) // Guard against unauthenticated requests
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Not logged in.", ErrorCode = ErrorCodes.Unauthorized });

            var result = await _playerProfileService.ChangeName(accountId, request); // Deduct name change fee, validate uniqueness, and update display name
            return Ok(new ApiResponse<PlayerProfileDetailResponseDto> { Success = true, Data = result }); // Return HTTP 200 with updated profile details
        }

        [Authorize]
        [HttpGet("me/friends")]
        // Retrieves friend list for the authenticated player profile.
        public async Task<IActionResult> GetMyFriends()
        {
            var playerProfileId = GetCurrentPlayerProfileId(); // Read player profile ID from JWT claim
            if (playerProfileId == 0) // Profile claim missing or unassociated
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.Unauthorized });

            var result = await _playerProfileService.GetFriends(playerProfileId); // Load friends list and their online statuses
            return Ok(new ApiResponse<List<PlayerProfileResponseDto>> { Success = true, Data = result }); // Return HTTP 200 with friend list
        }


        // ─── Admin APIs ───────────────────────────────────────────────────────
        [Authorize(Roles = "Admin")]
        [HttpGet]
        // Load paginated list of player profiles for admin dashboard.
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] int? level = null)
        {
            var result = await _playerProfileService.GetProfilesPaged(page, pageSize, search, level); // Query database with pagination, keyword search, and level filters
            return Ok(new ApiResponse<PagedResultDto<PlayerProfileResponseDto>> { Success = true, Data = result }); // Return HTTP 200 with paginated result set
        }
    }
}
