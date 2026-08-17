using BLL.DTOs;
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
    public class MailboxesController : ControllerBase
    {
        private readonly IMailboxService _mailboxService;
        // Initializes a new instance of MailboxesController with dependencies: mailboxService.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public MailboxesController(IMailboxService mailboxService)
        {
            _mailboxService = mailboxService;
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
        [Authorize]
        [HttpGet("me")]
        // Retrieves inbox messages and unclaimed gift mails for the authenticated player.
        public async Task<IActionResult> GetMyMailboxes(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var playerProfileId = GetCurrentPlayerProfileId(); // Extract caller's profile ID from JWT claim
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.Unauthorized });

            var result = await _mailboxService.GetMyMailboxes(playerProfileId, page, pageSize); // Load player's inbox with read/unread statuses and reward attachments
            return Ok(new ApiResponse<MailboxListPagedDto> { Success = true, Data = result });
        }

        [Authorize]
        [HttpGet("{id}")]
        // Retrieves full content and attachment details for a specific mail message.
        public async Task<IActionResult> GetById(int id)
        {
            var playerProfileId = GetCurrentPlayerProfileId(); // Extract caller's profile ID from JWT claim
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.Unauthorized });

            var mailbox = await _mailboxService.GetMailboxById(id); // Look up mail message by ID
            if (mailbox == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Mailbox with id {id} not found.", ErrorCode = ErrorCodes.NotFound });

            if (mailbox.PlayerProfileId != playerProfileId) // Reject access if mail belongs to another player
                return Forbid();

            return Ok(new ApiResponse<MailboxDetailDto> { Success = true, Data = mailbox });
        }

        [Authorize]
        [HttpPost("{id}/read")]
        // Marks a mail message as opened/read.
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var playerProfileId = GetCurrentPlayerProfileId(); // Extract caller's profile ID from JWT claim
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.Unauthorized });

            var mailbox = await _mailboxService.GetMailboxById(id); // Verify existence before update
            if (mailbox == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Mailbox with id {id} not found.", ErrorCode = ErrorCodes.NotFound });

            if (mailbox.PlayerProfileId != playerProfileId) // Verify ownership
                return Forbid();

            var updated = await _mailboxService.MarkMailboxAsRead(id); // Update IsRead flag to true
            return Ok(new ApiResponse<MailboxDetailDto> { Success = true, Data = updated });
        }

        [Authorize]
        [HttpPost("{id}/claim")]
        // Claims attached currencies or items from the mail and transfers them to player inventory/wallet.
        public async Task<IActionResult> ClaimReward(int id)
        {
            var playerProfileId = GetCurrentPlayerProfileId(); // Extract caller's profile ID from JWT claim
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.Unauthorized });

            var mailbox = await _mailboxService.GetMailboxById(id); // Verify existence before claiming
            if (mailbox == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Mailbox with id {id} not found.", ErrorCode = ErrorCodes.NotFound });

            if (mailbox.PlayerProfileId != playerProfileId) // Verify ownership
                return Forbid();

            var updated = await _mailboxService.ClaimMailboxReward(id); // Deposit rewards, mark IsClaimed = true, and return updated mail
            return Ok(new ApiResponse<MailboxDetailDto> { Success = true, Data = updated });
        }

        [Authorize]
        [HttpDelete("{id}")]
        // Deletes a mail message from the player's inbox.
        public async Task<IActionResult> DeleteMailbox(int id)
        {
            var playerProfileId = GetCurrentPlayerProfileId(); // Extract caller's profile ID from JWT claim
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.Unauthorized });

            await _mailboxService.DeleteMailbox(id, playerProfileId); // Ensure ownership and delete mail record
            return Ok(new ApiResponse<object> { Success = true, Message = "Mailbox deleted successfully." });
        }


        // ─── Admin APIs ───────────────────────────────────────────────────────
        [Authorize(Roles = "Admin")]
        [HttpGet]
        // Load all using page, page size, search, and is read; it loads mailboxes paged.
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] bool? isRead = null,
            [FromQuery] bool? isClaimed = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = null)
        {
            var result = await _mailboxService.GetMailboxesPaged(page, pageSize, search, isRead, isClaimed, sortBy, sortOrder);
            return Ok(new ApiResponse<PagedResultDto<MailboxDetailDto>> { Success = true, Data = result });  // Return HTTP 200 with standard ApiResponse envelope
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("by-ids")]
        // Executes send mailbox by list id operation.
        public async Task<IActionResult> SendMailboxByListId([FromBody] SendMailboxByListIdDto request)
        {
            if (!ModelState.IsValid)  // Reject request immediately if any DTO validation annotation fails
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });  // Return HTTP 400 with validation error details

            var mailboxes = await _mailboxService.SendMailboxByListId(request);
            return Ok(new ApiResponse<List<MailboxDetailDto>> { Success = true, Data = mailboxes });  // Return HTTP 200 with standard ApiResponse envelope
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("broadcast")]
        // Executes send mailbox to all operation.
        public async Task<IActionResult> SendMailboxToAll([FromBody] SendMailboxToAllDto request)
        {
            if (!ModelState.IsValid)  // Reject request immediately if any DTO validation annotation fails
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });  // Return HTTP 400 with validation error details

            var mailboxes = await _mailboxService.SendMailboxToAll(request);
            return Ok(new ApiResponse<List<MailboxDetailDto>> { Success = true, Data = mailboxes });  // Return HTTP 200 with standard ApiResponse envelope
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("player/{playerProfileId:int}")]
        // Load by player id using player profile id, page, and page size; it loads my mailboxes.
        public async Task<IActionResult> GetByPlayerId(
            int playerProfileId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _mailboxService.GetMyMailboxes(playerProfileId, page, pageSize);
            return Ok(new ApiResponse<MailboxListPagedDto> { Success = true, Data = result });  // Return HTTP 200 with standard ApiResponse envelope
        }
    }
}
