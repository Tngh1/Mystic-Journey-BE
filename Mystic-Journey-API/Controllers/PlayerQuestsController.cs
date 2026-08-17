using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Security.Claims;

namespace Mystic_Journey_API.Controllers
{
    // Executes controller base operation.
    [Route("api/playerquests")]
    [ApiController]
    [Authorize]
    public class PlayerQuestsController : ControllerBase
    {
        private readonly IPlayerQuestService _service;

        // Initializes a new instance of PlayerQuestsController with dependencies: service.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public PlayerQuestsController(IPlayerQuestService service)
        {
            _service = service;
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
        [HttpGet("me")]
        // Retrieves list of active, available, and completed quests for the authenticated player.
        public async Task<IActionResult> GetMyQuests()
        {
            var profileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
            var result = await _service.GetMyQuests(profileId); // Fetch all quest records assigned to or available for this player
            return Ok(new ApiResponse<List<PlayerQuestResponseDto>> { Success = true, Data = result });  // Return HTTP 200 with standard ApiResponse envelope
        }

        [HttpGet("{questId:int}")]
        // Retrieves specific quest details, objectives, NPC locations, and reward preview.
        public async Task<IActionResult> GetMyQuestDetail(int questId)
        {
            var profileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
            var result = await _service.GetMyQuestDetail(profileId, questId); // Load quest objectives, prerequisite checks, and completion status
            if (result == null)  // Entity not found — short-circuit with appropriate error result
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Quest {questId} not found on current map.", ErrorCode = ErrorCodes.NotFound });  // Return HTTP 404 when the requested resource does not exist

            return Ok(new ApiResponse<PlayerQuestResponseDto> { Success = true, Data = result });  // Return HTTP 200 with standard ApiResponse envelope
        }

        [HttpPost("accept")]
        // Accepts a quest from an NPC or world trigger, adding it to the player's quest journal.
        public async Task<IActionResult> AcceptQuest([FromBody] AcceptQuestRequestDto request)
        {
            if (!ModelState.IsValid)  // Reject request immediately if any DTO validation annotation fails
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });  // Return HTTP 400 with validation error details

            var profileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
            var result = await _service.AcceptQuest(profileId, request); // Verify prerequisites/level requirement and create InProgress quest tracking record
            return Ok(new ApiResponse<PlayerQuestResponseDto> { Success = true, Data = result });  // Return HTTP 200 with standard ApiResponse envelope
        }

        [HttpPut("batch-progress")]
        // Updates objective progress counts (kills, collections, exploration) in batches from the client.
        public async Task<IActionResult> BatchUpdateProgress([FromBody] BatchProgressRequestDto request)
        {
            if (!ModelState.IsValid)  // Reject request immediately if any DTO validation annotation fails
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });  // Return HTTP 400 with validation error details

            var profileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
            var result = await _service.BatchUpdateProgress(profileId, request); // Increment objective counters and check if quest objectives are met
            return Ok(new ApiResponse<List<PlayerQuestResponseDto>> { Success = true, Data = result });  // Return HTTP 200 with standard ApiResponse envelope
        }

        [HttpPost("complete")]
        // Completes all objectives for a quest and marks it ready to claim rewards.
        public async Task<IActionResult> CompleteQuest([FromBody] CompleteQuestRequestDto request)
        {
            if (!ModelState.IsValid)  // Reject request immediately if any DTO validation annotation fails
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });  // Return HTTP 400 with validation error details

            var profileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
            var result = await _service.CompleteQuest(profileId, request); // Validate all objectives satisfied and transition status to Completed
            return Ok(new ApiResponse<PlayerQuestResponseDto> { Success = true, Data = result });  // Return HTTP 200 with standard ApiResponse envelope
        }

        [HttpPost("claim")]
        // Claims the rewards (EXP, Gold, items) for a completed quest and moves it to Claimed status.
        public async Task<IActionResult> ClaimReward([FromBody] ClaimQuestRequestDto request)
        {
            if (!ModelState.IsValid)  // Reject request immediately if any DTO validation annotation fails
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });  // Return HTTP 400 with validation error details

            var profileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
            var result = await _service.ClaimReward(profileId, request); // Deliver EXP, currencies, and item rewards to inventory and set Claimed
            return Ok(new ApiResponse<PlayerQuestResponseDto> { Success = true, Data = result });  // Return HTTP 200 with standard ApiResponse envelope
        }
    }
}
