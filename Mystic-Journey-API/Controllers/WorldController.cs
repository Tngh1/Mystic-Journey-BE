using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Security.Claims;

namespace Mystic_Journey_API.Controllers
{
    // Executes controller base operation.
    [Route("api/world")]
    [ApiController]
    [Authorize]
    public class WorldController : ControllerBase
    {
        private readonly IWorldService _worldService;

        // Initializes a new instance of WorldController with dependencies: worldService.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public WorldController(IWorldService worldService)
        {
            _worldService = worldService;
        }

        // Executes get player profile id operation.
        // Throws an exception if precondition validations fail.
        private int GetPlayerProfileId()
        {
            var claim = User.FindFirstValue("playerProfileId");
            if (!int.TryParse(claim, out var id))  // Claim value missing or non-integer — reject as unauthorized
                throw new UnauthorizedAccessException("PlayerProfileId is missing from token. Please login again.");  // Authentication token is invalid or expired
            return id;
        }


        // ─── Player APIs ───────────────────────────────────────────────────────
        [HttpGet("state")]
        // Retrieves composite world state snapshot (position, active quests, daily login streak, chest cooldowns).
        public async Task<IActionResult> GetState()
        {
            var profileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
            var result = await _worldService.GetWorldState(profileId); // Assemble player snapshot data
            return Ok(new ApiResponse<WorldStateResponseDto> { Success = true, Data = result });
        }

        [HttpGet("position")]
        // Retrieves last saved map name and coordinate position for the player.
        public async Task<IActionResult> GetPosition()
        {
            var profileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
            var result = await _worldService.GetPosition(profileId); // Load persisted map coordinates
            return Ok(new ApiResponse<PlayerWorldPositionDto> { Success = true, Data = result });
        }

        [HttpPut("position")]
        // Persists updated player coordinates (X, Y, MapName) upon world traversal or scene transition.
        public async Task<IActionResult> UpdatePosition([FromBody] UpdateWorldPositionRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var profileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
            var result = await _worldService.UpdatePosition(profileId, request); // Save coordinates to Redis/DB cache
            return Ok(new ApiResponse<PlayerWorldPositionDto> { Success = true, Data = result });
        }

        [HttpPost("npc/talk")]
        // Processes dialogue interaction with an NPC and returns story text or quest prompts.
        public async Task<IActionResult> TalkToNpc([FromBody] TalkToNpcRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var profileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
            var result = await _worldService.TalkToNpc(profileId, request); // Evaluate player quest state against NPC dialogue trees
            return Ok(new ApiResponse<TalkToNpcResponseDto> { Success = true, Data = result });
        }

        [HttpPost("npc/turn-in")]
        // Turns in required quest items to an NPC to complete an objective.
        public async Task<IActionResult> TurnInQuestItem([FromBody] TurnInQuestItemRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var profileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
            var result = await _worldService.TurnInQuestItem(profileId, request); // Remove required items from inventory and progress quest status
            return Ok(new ApiResponse<TurnInQuestItemResponseDto> { Success = true, Data = result });
        }

        [HttpPost("chests/open")]
        // Opens an overworld treasure chest and distributes randomized loot.
        public async Task<IActionResult> OpenChest([FromBody] OpenWorldChestRequestDto request)
        {
            var profileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
            var result = await _worldService.OpenChest(profileId, request); // Verify chest cooldown, grant rewards, and record opened state
            return Ok(new ApiResponse<OpenChestResponseDto> { Success = true, Data = result });
        }

        [HttpPost("interactions")]
        // Triggers interactive world props (portals, switches, mining nodes, shrines).
        public async Task<IActionResult> InteractWithObject([FromBody] InteractObjectRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var profileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
            var result = await _worldService.InteractWithObject(profileId, request); // Execute world object script and apply player buffs/teleports
            return Ok(new ApiResponse<InteractObjectResponseDto> { Success = true, Data = result });
        }

        [HttpPost("daily-login/claim")]
        // Claims the login reward for today's active day on the calendar.
        public async Task<IActionResult> ClaimDailyLoginReward()
        {
            var profileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
            var result = await _worldService.ClaimDailyLoginReward(profileId); // Verify eligibility, mark today as claimed, and deposit calendar rewards
            return Ok(new ApiResponse<ClaimDailyRewardResponseDto> { Success = true, Data = result });
        }

        [HttpPost("daily-login/retro-claim")]
        // Spends Gems to retroactively claim a missed daily login day from earlier this month.
        public async Task<IActionResult> RetroactiveClaimDailyLoginReward([FromBody] RetroClaimRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var profileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
            var result = await _worldService.RetroactiveClaimDailyLoginReward(profileId, request.DayNumber); // Deduct gem makeup fee, mark day claimed, and grant missed items
            return Ok(new ApiResponse<ClaimDailyRewardResponseDto> { Success = true, Data = result });
        }
    }
}
