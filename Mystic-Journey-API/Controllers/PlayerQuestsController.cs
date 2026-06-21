using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

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
            try
            {
                var profileId = GetPlayerProfileId();
                var result = await _service.GetMyQuestDetail(profileId, questId);
                if (result == null)
                    return NotFound(new { message = $"Quest {questId} not found on current map." });

                return Ok(new ApiResponse<object> { Data = result });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("accept")]
        public async Task<IActionResult> AcceptQuest([FromBody] AcceptQuestRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var profileId = GetPlayerProfileId();
                var result = await _service.AcceptQuest(profileId, request);
                return Ok(new ApiResponse<object> { Data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("batch-progress")]
        public async Task<IActionResult> BatchUpdateProgress([FromBody] BatchProgressRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var profileId = GetPlayerProfileId();
                var result = await _service.BatchUpdateProgress(profileId, request);
                return Ok(new ApiResponse<object> { Data = result });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("complete")]
        public async Task<IActionResult> CompleteQuest([FromBody] CompleteQuestRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var profileId = GetPlayerProfileId();
                var result = await _service.CompleteQuest(profileId, request);
                return Ok(new ApiResponse<object> { Data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("claim")]
        public async Task<IActionResult> ClaimReward([FromBody] ClaimQuestRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var profileId = GetPlayerProfileId();
                var result = await _service.ClaimReward(profileId, request);
                return Ok(new ApiResponse<object> { Data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
