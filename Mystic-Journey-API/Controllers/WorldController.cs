using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Security.Claims;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/world")]
    [ApiController]
    [Authorize]
    public class WorldController : ControllerBase
    {
        private readonly IWorldService _worldService;

        public WorldController(IWorldService worldService)
        {
            _worldService = worldService;
        }

        private int GetPlayerProfileId()
        {
            var claim = User.FindFirstValue("playerProfileId");
            if (!int.TryParse(claim, out var id))
                throw new UnauthorizedAccessException("PlayerProfileId is missing from token. Please login again.");
            return id;
        }

        [HttpGet("state")]
        public async Task<IActionResult> GetState()
        {
            var profileId = GetPlayerProfileId();
            var result = await _worldService.GetWorldState(profileId);
            return Ok(new ApiResponse<WorldStateResponseDto> { Success = true, Data = result });
        }

        [HttpPut("position")]
        public async Task<IActionResult> UpdatePosition([FromBody] UpdateWorldPositionRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var profileId = GetPlayerProfileId();
            var result = await _worldService.UpdatePosition(profileId, request);
            return Ok(new ApiResponse<PlayerWorldPositionDto> { Success = true, Data = result });
        }

        [HttpPost("npc/talk")]
        public async Task<IActionResult> TalkToNpc([FromBody] TalkToNpcRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var profileId = GetPlayerProfileId();
            var result = await _worldService.TalkToNpc(profileId, request);
            return Ok(new ApiResponse<TalkToNpcResponseDto> { Success = true, Data = result });
        }

        [HttpPost("npc/turn-in")]
        public async Task<IActionResult> TurnInQuestItem([FromBody] TurnInQuestItemRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var profileId = GetPlayerProfileId();
            var result = await _worldService.TurnInQuestItem(profileId, request);
            return Ok(new ApiResponse<TurnInQuestItemResponseDto> { Success = true, Data = result });
        }

        [HttpPost("chests/open")]
        public async Task<IActionResult> OpenChest([FromBody] OpenWorldChestRequestDto request)
        {
            var profileId = GetPlayerProfileId();
            var result = await _worldService.OpenChest(profileId, request);
            return Ok(new ApiResponse<OpenChestResponseDto> { Success = true, Data = result });
        }

        [HttpPost("interactions")]
        public async Task<IActionResult> InteractWithObject([FromBody] InteractObjectRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var profileId = GetPlayerProfileId();
            var result = await _worldService.InteractWithObject(profileId, request);
            return Ok(new ApiResponse<InteractObjectResponseDto> { Success = true, Data = result });
        }

        [HttpPost("daily-login/claim")]
        public async Task<IActionResult> ClaimDailyLoginReward()
        {
            var profileId = GetPlayerProfileId();
            var result = await _worldService.ClaimDailyLoginReward(profileId);
            return Ok(new ApiResponse<ClaimDailyRewardResponseDto> { Success = true, Data = result });
        }

        [HttpPost("daily-login/retro-claim")]
        public async Task<IActionResult> RetroactiveClaimDailyLoginReward([FromBody] RetroClaimRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var profileId = GetPlayerProfileId();
            var result = await _worldService.RetroactiveClaimDailyLoginReward(profileId, request.DayNumber);
            return Ok(new ApiResponse<ClaimDailyRewardResponseDto> { Success = true, Data = result });
        }
    }
}
