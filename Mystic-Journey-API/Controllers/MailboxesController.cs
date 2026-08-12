using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    // Quản lý hệ thống thư (mailbox) cho người chơi và admin.
    // Game APIs: Người chơi xem, đọc, nhận thưởng, xóa thư của mình.
    // Admin APIs: Admin gửi thư, broadcast, và quản lý tất cả thư.
    [Route("api/[controller]")]
    [ApiController]
    public class MailboxesController : ControllerBase
    {
        private readonly IMailboxService _mailboxService;
        public MailboxesController(IMailboxService mailboxService)
        {
            _mailboxService = mailboxService;
        }

        private int GetCurrentAccountId()
        {
            var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
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

        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // ── GET /api/mailboxes/me ──────────────────────────────────────────────
        // Lấy danh sách thư của player đang đăng nhập, có phân trang.
        // Query: page (mặc định 1), pageSize (mặc định 20).
        // Response: TotalMailboxes, Items[], Page, PageSize, TotalPages.
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyMailboxes(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var playerProfileId = GetCurrentPlayerProfileId();
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.Unauthorized });

            var result = await _mailboxService.GetMyMailboxes(playerProfileId, page, pageSize);
            return Ok(new ApiResponse<MailboxListPagedDto> { Success = true, Data = result });
        }

        // ── GET /api/mailboxes/{id} ────────────────────────────────────────────
        // Lấy chi tiết thư theo MailboxId.
        // Response: MailboxId, Title, Content, Type, IsRead, IsClaimed, AttachedGold, AttachedGems, AttachedItem.
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var playerProfileId = GetCurrentPlayerProfileId();
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.Unauthorized });

            var mailbox = await _mailboxService.GetMailboxById(id);
            if (mailbox == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Mailbox with id {id} not found.", ErrorCode = ErrorCodes.NotFound });

            if (mailbox.PlayerProfileId != playerProfileId)
                return Forbid();

            return Ok(new ApiResponse<MailboxDetailDto> { Success = true, Data = mailbox });
        }

        // ── POST /api/mailboxes/{id}/read ──────────────────────────────────────
        // Đánh dấu thư đã đọc.
        [Authorize]
        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var playerProfileId = GetCurrentPlayerProfileId();
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.Unauthorized });

            var mailbox = await _mailboxService.GetMailboxById(id);
            if (mailbox == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Mailbox with id {id} not found.", ErrorCode = ErrorCodes.NotFound });

            if (mailbox.PlayerProfileId != playerProfileId)
                return Forbid();

            var updated = await _mailboxService.MarkMailboxAsRead(id);
            return Ok(new ApiResponse<MailboxDetailDto> { Success = true, Data = updated });
        }

        // ── POST /api/mailboxes/{id}/claim ─────────────────────────────────────
        // Nhận phần thưởng trong thư (gold, gems, item).
        [Authorize]
        [HttpPost("{id}/claim")]
        public async Task<IActionResult> ClaimReward(int id)
        {
            var playerProfileId = GetCurrentPlayerProfileId();
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.Unauthorized });

            var mailbox = await _mailboxService.GetMailboxById(id);
            if (mailbox == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Mailbox with id {id} not found.", ErrorCode = ErrorCodes.NotFound });

            if (mailbox.PlayerProfileId != playerProfileId)
                return Forbid();

            var updated = await _mailboxService.ClaimMailboxReward(id);
            return Ok(new ApiResponse<MailboxDetailDto> { Success = true, Data = updated });
        }

        // ── DELETE /api/mailboxes/{id} ─────────────────────────────────────────
        // Xóa thư của player đang đăng nhập.
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMailbox(int id)
        {
            var playerProfileId = GetCurrentPlayerProfileId();
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.Unauthorized });

            await _mailboxService.DeleteMailbox(id, playerProfileId);
            return Ok(new ApiResponse<object> { Success = true, Message = "Mailbox deleted successfully." });
        }

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // ── GET /api/mailboxes ─────────────────────────────────────────────────
        // Lấy tất cả thư có lọc và phân trang (Admin).
        // Query: page, pageSize, search, isRead, isClaimed.
        [Authorize(Roles = "Admin")]
        [HttpGet]
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
            return Ok(new ApiResponse<PagedResultDto<MailboxDetailDto>> { Success = true, Data = result });
        }

        // ── POST /api/mailboxes/by-ids ─────────────────────────────────────────
        // Gửi thư đến danh sách player theo ID.
        [Authorize(Roles = "Admin")]
        [HttpPost("by-ids")]
        public async Task<IActionResult> SendMailboxByListId([FromBody] SendMailboxByListIdDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var mailboxes = await _mailboxService.SendMailboxByListId(request);
            return Ok(new ApiResponse<List<MailboxDetailDto>> { Success = true, Data = mailboxes });
        }

        // ── POST /api/mailboxes/broadcast ──────────────────────────────────────
        // Broadcast thư đến tất cả player.
        [Authorize(Roles = "Admin")]
        [HttpPost("broadcast")]
        public async Task<IActionResult> SendMailboxToAll([FromBody] SendMailboxToAllDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var mailboxes = await _mailboxService.SendMailboxToAll(request);
            return Ok(new ApiResponse<List<MailboxDetailDto>> { Success = true, Data = mailboxes });
        }

        // ── GET /api/mailboxes/player/{playerProfileId} ────────────────────────
        // Lấy tất cả thư của một player cụ thể (Admin).
        [Authorize(Roles = "Admin")]
        [HttpGet("player/{playerProfileId:int}")]
        public async Task<IActionResult> GetByPlayerId(
            int playerProfileId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _mailboxService.GetMyMailboxes(playerProfileId, page, pageSize);
            return Ok(new ApiResponse<MailboxListPagedDto> { Success = true, Data = result });
        }
    }
}
