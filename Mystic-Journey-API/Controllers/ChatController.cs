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
    // Executes controller base operation.
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        // Initializes a new instance of ChatController with dependencies: chatService.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
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
        [Authorize]
        [HttpGet("world/messages")]
        // Retrieves recent world channel chat messages with pagination.
        public async Task<IActionResult> GetWorldMessages([FromQuery] WorldChatMessageListQueryDto query)
        {
            if (!ModelState.IsValid) // Guard against invalid query filters or pagination bounds
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    ErrorCode = ErrorCodes.ValidationError
                });

            var playerProfileId = GetCurrentPlayerProfileId(); // Extract caller's profile ID from JWT claim
            if (playerProfileId == 0) // Unauthorized if profile claim is absent
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Player profile not found.",
                    ErrorCode = ErrorCodes.Unauthorized
                });

            var result = await _chatService.GetWorldMessages(playerProfileId, query); // Query recent world messages and attach sender profile info
            return Ok(new ApiResponse<PagedResultDto<WorldChatMessageResponseDto>>
            {
                Success = true,
                Data = result
            });
        }

        [Authorize]
        [HttpGet("world/history")]
        // Retrieves world chat history (alias for world messages).
        public async Task<IActionResult> GetWorldHistory([FromQuery] WorldChatMessageListQueryDto query)
            => await GetWorldMessages(query);

        [Authorize]
        [HttpPost("world/send")]
        // Broadcasts a new world message after content safety moderation and rate-limit checks.
        public async Task<IActionResult> SendWorldMessage([FromBody] SendWorldChatMessageRequestDto request)
        {
            if (!ModelState.IsValid) // Validate message payload length and formatting
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    ErrorCode = ErrorCodes.ValidationError
                });

            var playerProfileId = GetCurrentPlayerProfileId(); // Extract caller's profile ID from JWT claim
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Player profile not found.",
                    ErrorCode = ErrorCodes.Unauthorized
                });

            try
            {
                var result = await _chatService.SendWorldMessage(playerProfileId, request); // Moderate content, apply rate limits, persist message, and broadcast via SignalR
                return Ok(new ApiResponse<WorldChatMessageResponseDto> { Success = true, Data = result });
            }
            catch (ChatLockedException ex)
            {
                Response.Headers["Retry-After"] = ex.RetryAfterSeconds.ToString(); // Set standard HTTP Retry-After header for chat mute duration
                return StatusCode(423, new ApiResponse<object> // HTTP 423 Locked: account temporarily muted due to toxic language
                {
                    Success = false,
                    Message = ex.Message,
                    ErrorCode = ErrorCodes.ChatLocked,
                    Data = new
                    {
                        lockedUntil = ex.LockedUntil,
                        lockLevel = ex.LockLevel,
                        retryAfterSeconds = ex.RetryAfterSeconds
                    }
                });
            }
            catch (ChatRateLimitException ex)
            {
                Response.Headers["Retry-After"] = ex.RetryAfterSeconds.ToString(); // Set cooldown retry timer header
                return StatusCode(StatusCodes.Status429TooManyRequests, new ApiResponse<object> // HTTP 429: spam throttle triggered
                {
                    Success = false,
                    Message = ex.Message,
                    ErrorCode = "RATE_LIMITED"
                });
            }
        }

        [Authorize]
        [HttpPost("world/report")]
        // Submits a player report on a world message and triggers automatic safety evaluation.
        public async Task<IActionResult> ReportWorldMessage([FromBody] ReportChatMessageRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    ErrorCode = ErrorCodes.ValidationError
                });

            var playerProfileId = GetCurrentPlayerProfileId(); // Extract reporter's profile ID from JWT claim
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Player profile not found.",
                    ErrorCode = ErrorCodes.Unauthorized
                });

            var result = await _chatService.ReportWorldMessage(playerProfileId, request); // Log report, trigger automated toxic check, and mute offender if threshold exceeded
            return Ok(new ApiResponse<ReportWorldChatMessageResponseDto>
            {
                Success = true,
                Message = result.Moderation.ChatLocked ? result.Moderation.WarningMessage : "Message reported.",
                Data = result
            });
        }

        [Authorize]
        [HttpPost("party/report")]
        public async Task<IActionResult> ReportPartyMessage([FromBody] ReportPartyChatMessageRequestDto request)
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

            var result = await _chatService.ReportPartyMessage(playerProfileId, request);
            return Ok(new ApiResponse<ChatModerationResultDto>
            {
                Success = true,
                Message = result.ChatLocked ? result.WarningMessage : "Party message reported.",
                Data = result
            });
        }

        [Authorize]
        [HttpGet("friend/messages")]
        // Retrieves private messages with a specific friend (alias for get messages).
        public async Task<IActionResult> GetFriendMessages([FromQuery] ChatMessageListQueryDto query)
            => await GetMessages(query);

        [Authorize]
        [HttpGet("friend/history")]
        // Retrieves friend chat history (alias).
        public async Task<IActionResult> GetFriendHistory([FromQuery] ChatMessageListQueryDto query)
            => await GetMessages(query);

        [Authorize]
        [HttpGet("messages")]
        // Retrieves direct 1-on-1 private chat conversation with another player.
        public async Task<IActionResult> GetMessages([FromQuery] ChatMessageListQueryDto query)
        {
            if (!ModelState.IsValid) // Validate target user ID and pagination parameters
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    ErrorCode = ErrorCodes.ValidationError
                });

            var playerProfileId = GetCurrentPlayerProfileId(); // Extract caller's profile ID from JWT claim
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Player profile not found.",
                    ErrorCode = ErrorCodes.Unauthorized
                });

            var result = await _chatService.GetMessages(playerProfileId, query); // Load chronological private messages between the two users
            return Ok(new ApiResponse<PagedResultDto<ChatMessageResponseDto>>
            {
                Success = true,
                Data = result
            });
        }

        [Authorize]
        [HttpGet("history")]
        // Retrieves private chat conversation history (alias).
        public async Task<IActionResult> GetHistory([FromQuery] ChatMessageListQueryDto query)
            => await GetMessages(query);

        [Authorize]
        [HttpPost("friend/send")]
        // Sends private message to a friend (alias).
        public async Task<IActionResult> SendFriendMessage([FromBody] SendChatMessageRequestDto request)
            => await SendMessage(request);

        [Authorize]
        [HttpPost("friend/report")]
        // Reports a friend's private message for moderation review (alias).
        public async Task<IActionResult> ReportFriendMessage([FromBody] ReportChatMessageRequestDto request)
            => await ReportMessage(request);

        [Authorize]
        [HttpPost("report")]
        // Submits a violation report on a direct message.
        public async Task<IActionResult> ReportMessage([FromBody] ReportChatMessageRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    ErrorCode = ErrorCodes.ValidationError
                });

            var playerProfileId = GetCurrentPlayerProfileId(); // Extract reporter's profile ID from JWT claim
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Player profile not found.",
                    ErrorCode = ErrorCodes.Unauthorized
                });

            var result = await _chatService.ReportMessage(playerProfileId, request); // Log moderation incident and trigger safety escalation
            return Ok(new ApiResponse<ReportChatMessageResponseDto>
            {
                Success = true,
                Message = result.Moderation.ChatLocked ? result.Moderation.WarningMessage : "Message reported.",
                Data = result
            });
        }

        [Authorize]
        [HttpPost("send")]
        // Sends a direct private message to another player with rate limiting and automated moderation.
        public async Task<IActionResult> SendMessage([FromBody] SendChatMessageRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    ErrorCode = ErrorCodes.ValidationError
                });

            var playerProfileId = GetCurrentPlayerProfileId(); // Extract sender's profile ID from JWT claim
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Player profile not found.",
                    ErrorCode = ErrorCodes.Unauthorized
                });

            try
            {
                var result = await _chatService.SendMessage(playerProfileId, request); // Verify friendship/block status, run content moderation, and deliver via SignalR
                return Ok(new ApiResponse<ChatMessageResponseDto> { Success = true, Data = result });
            }
            catch (ChatLockedException ex)
            {
                Response.Headers["Retry-After"] = ex.RetryAfterSeconds.ToString(); // Set standard HTTP Retry-After header
                return StatusCode(423, new ApiResponse<object> // Return HTTP 423 Locked if sender is muted
                {
                    Success = false,
                    Message = ex.Message,
                    ErrorCode = ErrorCodes.ChatLocked,
                    Data = new
                    {
                        lockedUntil = ex.LockedUntil,
                        lockLevel = ex.LockLevel,
                        retryAfterSeconds = ex.RetryAfterSeconds
                    }
                });
            }
            catch (ChatRateLimitException ex)
            {
                Response.Headers["Retry-After"] = ex.RetryAfterSeconds.ToString(); // Set retry cooldown timer header
                return StatusCode(StatusCodes.Status429TooManyRequests, new ApiResponse<object> // Return HTTP 429 Too Many Requests
                {
                    Success = false,
                    Message = ex.Message,
                    ErrorCode = "RATE_LIMITED"
                });
            }
        }
    }
}
