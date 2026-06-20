using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("state")]
        public async Task<IActionResult> GetState()
        {
            try
            {
                var profileId = GetPlayerProfileId();
                var result = await _worldService.GetWorldState(profileId);
                return Ok(new ApiResponse<WorldStateResponseDto> { Success = true, Data = result });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ErrorResponse { Error = "UNAUTHORIZED", Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Error = "INTERNAL_ERROR", Message = ex.Message });
            }
        }

        [HttpPut("position")]
        public async Task<IActionResult> UpdatePosition([FromBody] UpdateWorldPositionRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var profileId = GetPlayerProfileId();
                var result = await _worldService.UpdatePosition(profileId, request);
                return Ok(new ApiResponse<PlayerWorldPositionDto> { Success = true, Data = result });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ErrorResponse { Error = "UNAUTHORIZED", Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Error = "INTERNAL_ERROR", Message = ex.Message });
            }
        }

        [HttpPost("npc/talk")]
        public async Task<IActionResult> TalkToNpc([FromBody] TalkToNpcRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var profileId = GetPlayerProfileId();
                var result = await _worldService.TalkToNpc(profileId, request);
                return Ok(new ApiResponse<TalkToNpcResponseDto> { Success = true, Data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ErrorResponse { Error = "NPC_NOT_FOUND", Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErrorResponse { Error = "INVALID_OPERATION", Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ErrorResponse { Error = "UNAUTHORIZED", Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Error = "INTERNAL_ERROR", Message = ex.Message });
            }
        }


        [HttpPost("npc/turn-in")]
        public async Task<IActionResult> TurnInQuestItem([FromBody] TurnInQuestItemRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var profileId = GetPlayerProfileId();
                var result = await _worldService.TurnInQuestItem(profileId, request);
                return Ok(new ApiResponse<TurnInQuestItemResponseDto> { Success = result.Success, Message = result.Message, Data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ErrorResponse { Error = "QUEST_OR_NPC_NOT_FOUND", Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErrorResponse { Error = "INVALID_OPERATION", Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ErrorResponse { Error = "UNAUTHORIZED", Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Error = "INTERNAL_ERROR", Message = ex.Message });
            }
        }
        [HttpPost("chests/open")]
        public async Task<IActionResult> OpenChest([FromBody] OpenWorldChestRequestDto request)
        {
            try
            {
                var profileId = GetPlayerProfileId();
                var result = await _worldService.OpenChest(profileId, request);
                return Ok(new ApiResponse<OpenChestResponseDto> { Success = true, Data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ErrorResponse { Error = "CHEST_NOT_FOUND", Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErrorResponse { Error = "INVALID_OPERATION", Message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorResponse { Error = "BAD_REQUEST", Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ErrorResponse { Error = "UNAUTHORIZED", Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Error = "INTERNAL_ERROR", Message = ex.Message });
            }
        }

        [HttpPost("interactions")]
        public async Task<IActionResult> InteractWithObject([FromBody] InteractObjectRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var profileId = GetPlayerProfileId();
                var result = await _worldService.InteractWithObject(profileId, request);
                return Ok(new ApiResponse<InteractObjectResponseDto> { Success = true, Data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ErrorResponse { Error = "OBJECT_NOT_FOUND", Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErrorResponse { Error = "INVALID_OPERATION", Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ErrorResponse { Error = "UNAUTHORIZED", Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Error = "INTERNAL_ERROR", Message = ex.Message });
            }
        }

        [HttpPost("daily-login/claim")]
        public async Task<IActionResult> ClaimDailyLoginReward()
        {
            try
            {
                var profileId = GetPlayerProfileId();
                var result = await _worldService.ClaimDailyLoginReward(profileId);
                return Ok(new ApiResponse<ClaimDailyRewardResponseDto> { Success = result.Success, Message = result.Message, Data = result });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ErrorResponse { Error = "UNAUTHORIZED", Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Error = "INTERNAL_ERROR", Message = ex.Message });
            }
        }

        private int GetPlayerProfileId()
        {
            var claim = User.FindFirstValue("playerProfileId");
            if (!int.TryParse(claim, out var id))
                throw new UnauthorizedAccessException("PlayerProfileId is missing from token. Please login again.");
            return id;
        }
    }
}
