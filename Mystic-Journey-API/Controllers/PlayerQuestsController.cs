using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Security.Claims;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/playerquests")]
    [ApiController]
    [Authorize]
    public class PlayerQuestsController : ControllerBase
    {
        private readonly IPlayerQuestService _service;

        public PlayerQuestsController(IPlayerQuestService service)
        {
            _service = service;
        }

        private int GetPlayerProfileId()
        {
            var claim = User.FindFirstValue("playerProfileId");
            if (!int.TryParse(claim, out var id))
                throw new UnauthorizedAccessException("PlayerProfileId is missing from token. Please login again.");
            return id;
        }

        [HttpGet("{questId:int}")]
        public async Task<IActionResult> GetMyQuestDetail(int questId)
        {
            var profileId = GetPlayerProfileId();
            var result = await _service.GetMyQuestDetail(profileId, questId);
            if (result == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Quest {questId} not found on current map.", ErrorCode = ErrorCodes.NotFound });

            return Ok(new ApiResponse<PlayerQuestResponseDto> { Success = true, Data = result });
        }

        [HttpPost("accept")]
        public async Task<IActionResult> AcceptQuest([FromBody] AcceptQuestRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var profileId = GetPlayerProfileId();
            var result = await _service.AcceptQuest(profileId, request);
            return Ok(new ApiResponse<PlayerQuestResponseDto> { Success = true, Data = result });
        }

        [HttpPut("batch-progress")]
        public async Task<IActionResult> BatchUpdateProgress([FromBody] BatchProgressRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var profileId = GetPlayerProfileId();
            var result = await _service.BatchUpdateProgress(profileId, request);
            return Ok(new ApiResponse<List<PlayerQuestResponseDto>> { Success = true, Data = result });
        }

        [HttpPost("complete")]
        public async Task<IActionResult> CompleteQuest([FromBody] CompleteQuestRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var profileId = GetPlayerProfileId();
            var result = await _service.CompleteQuest(profileId, request);
            return Ok(new ApiResponse<PlayerQuestResponseDto> { Success = true, Data = result });
        }

        [HttpPost("claim")]
        public async Task<IActionResult> ClaimReward([FromBody] ClaimQuestRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var profileId = GetPlayerProfileId();
            var result = await _service.ClaimReward(profileId, request);
            return Ok(new ApiResponse<PlayerQuestResponseDto> { Success = true, Data = result });
        }
    }
}
