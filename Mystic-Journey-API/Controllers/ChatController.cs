using BLL.DTOs;
using BLL.Services;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        private int GetCurrentPlayerProfileId()
        {
            var claim = User.FindFirst("playerProfileId");
            if (claim != null && int.TryParse(claim.Value, out var profileId))
            {
                return profileId;
            }

            return 0;
        }

        [Authorize]
        [HttpGet("world/messages")]
        public async Task<IActionResult> GetWorldMessages([FromQuery] WorldChatMessageListQueryDto query)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    ErrorCode = ErrorCodes.ValidationError
                });

            var playerProfileId = GetCurrentPlayerProfileId();
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Player profile not found.",
                    ErrorCode = ErrorCodes.Unauthorized
                });

            var result = await _chatService.GetWorldMessages(playerProfileId, query);
            return Ok(new ApiResponse<PagedResultDto<WorldChatMessageResponseDto>>
            {
                Success = true,
                Data = result
            });
        }

        [Authorize]
        [HttpGet("world/history")]
        public async Task<IActionResult> GetWorldHistory([FromQuery] WorldChatMessageListQueryDto query)
            => await GetWorldMessages(query);

        [Authorize]
        [HttpPost("world/send")]
        public async Task<IActionResult> SendWorldMessage([FromBody] SendWorldChatMessageRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    ErrorCode = ErrorCodes.ValidationError
                });

            var playerProfileId = GetCurrentPlayerProfileId();
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Player profile not found.",
                    ErrorCode = ErrorCodes.Unauthorized
                });

            try
            {
                var result = await _chatService.SendWorldMessage(playerProfileId, request);
                return Ok(new ApiResponse<WorldChatMessageResponseDto> { Success = true, Data = result });
            }
            catch (ChatRateLimitException ex)
            {
                Response.Headers["Retry-After"] = ex.RetryAfterSeconds.ToString();
                return StatusCode(StatusCodes.Status429TooManyRequests, new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message,
                    ErrorCode = "RATE_LIMITED"
                });
            }
        }

        [Authorize]
        [HttpPost("world/report")]
        public async Task<IActionResult> ReportWorldMessage([FromBody] ReportChatMessageRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    ErrorCode = ErrorCodes.ValidationError
                });

            var playerProfileId = GetCurrentPlayerProfileId();
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Player profile not found.",
                    ErrorCode = ErrorCodes.Unauthorized
                });

            var result = await _chatService.ReportWorldMessage(playerProfileId, request);
            return Ok(new ApiResponse<WorldChatMessageResponseDto>
            {
                Success = true,
                Message = "Message reported.",
                Data = result
            });
        }

        [Authorize]
        [HttpGet("messages")]
        public async Task<IActionResult> GetMessages([FromQuery] ChatMessageListQueryDto query)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    ErrorCode = ErrorCodes.ValidationError
                });

            var playerProfileId = GetCurrentPlayerProfileId();
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Player profile not found.",
                    ErrorCode = ErrorCodes.Unauthorized
                });

            var result = await _chatService.GetMessages(playerProfileId, query);
            return Ok(new ApiResponse<PagedResultDto<ChatMessageResponseDto>>
            {
                Success = true,
                Data = result
            });
        }

        [Authorize]
        [HttpGet("history")]
        public async Task<IActionResult> GetHistory([FromQuery] ChatMessageListQueryDto query)
            => await GetMessages(query);

        [Authorize]
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendChatMessageRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    ErrorCode = ErrorCodes.ValidationError
                });

            var playerProfileId = GetCurrentPlayerProfileId();
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Player profile not found.",
                    ErrorCode = ErrorCodes.Unauthorized
                });

            try
            {
                var result = await _chatService.SendMessage(playerProfileId, request);
                return Ok(new ApiResponse<ChatMessageResponseDto> { Success = true, Data = result });
            }
            catch (ChatRateLimitException ex)
            {
                Response.Headers["Retry-After"] = ex.RetryAfterSeconds.ToString();
                return StatusCode(StatusCodes.Status429TooManyRequests, new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message,
                    ErrorCode = "RATE_LIMITED"
                });
            }
        }
    }
}
