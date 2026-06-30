using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    // Quản lý hệ thống thư (mail) cho người chơi và admin.
    // Game APIs: Người chơi xem, đọc, nhận thưởng, xóa mail của mình.
    // Admin APIs: Admin gửi mail, broadcast, và quản lý tất cả mail.
    [Route("api/[controller]")]
    [ApiController]
    public class MailsController : ControllerBase
    {
        private readonly IMailService _mailService;
        public MailsController(IMailService mailService)
        {
            _mailService = mailService;
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

        // ── GET /api/mails/me ─────────────────────────────────────────────────
        // Lấy danh sách mail của player đang đăng nhập, có phân trang.
        // Query: page (mặc định 1), pageSize (mặc định 20).
        // Response: TotalMails, Items[], Page, PageSize, TotalPages.
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyMails(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var playerProfileId = GetCurrentPlayerProfileId();
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.Unauthorized });

            var result = await _mailService.GetMyMails(playerProfileId, page, pageSize);
            return Ok(new ApiResponse<MailListPagedDto> { Success = true, Data = result });
        }

        // ── GET /api/mails/{id} ────────────────────────────────────────────────
        // Lấy chi tiết mail theo MailId.
        // Response: MailId, Title, Content, Type, IsRead, IsClaimed, AttachedGold, AttachedGems, AttachedItem.
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var mail = await _mailService.GetMailById(id);
            if (mail == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Mail with id {id} not found.", ErrorCode = ErrorCodes.NotFound });

            return Ok(new ApiResponse<MailDetailDto> { Success = true, Data = mail });
        }

        // ── POST /api/mails/{id}/read ──────────────────────────────────────────
        // Đánh dấu mail đã đọc.
        [Authorize]
        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var mail = await _mailService.MarkMailAsRead(id);
            return Ok(new ApiResponse<MailDetailDto> { Success = true, Data = mail });
        }

        // ── POST /api/mails/{id}/claim ─────────────────────────────────────────
        // Nhận phần thưởng trong mail (gold, gems, item).
        [Authorize]
        [HttpPost("{id}/claim")]
        public async Task<IActionResult> ClaimReward(int id)
        {
            var mail = await _mailService.ClaimMailReward(id);
            return Ok(new ApiResponse<MailDetailDto> { Success = true, Data = mail });
        }

        // ── DELETE /api/mails/{id} ─────────────────────────────────────────────
        // Xóa mail của player đang đăng nhập.
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMail(int id)
        {
            var playerProfileId = GetCurrentPlayerProfileId();
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.Unauthorized });

            await _mailService.DeleteMail(id, playerProfileId);
            return Ok(new ApiResponse<object> { Success = true, Message = "Mail deleted successfully." });
        }

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // ── GET /api/mails ────────────────────────────────────────────────────
        // Lấy tất cả mail có lọc và phân trang (Admin).
        // Query: page, pageSize, search, isRead, isClaimed.
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] bool? isRead = null,
            [FromQuery] bool? isClaimed = null)
        {
            var result = await _mailService.GetMailsPaged(page, pageSize, search, isRead, isClaimed);
            return Ok(new ApiResponse<PagedResultDto<MailDetailDto>> { Success = true, Data = result });
        }

        // ── POST /api/mails/by-ids ─────────────────────────────────────────────
        // Gửi mail đến danh sách player theo ID.
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost("by-ids")]
        public async Task<IActionResult> SendMailByListId([FromBody] SendMailByListIdDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            await _mailService.SendMailByListId(request);
            return Ok(new ApiResponse<object> { Success = true, Message = "Mail sent successfully." });
        }

        // ── POST /api/mails/broadcast ──────────────────────────────────────────
        // Broadcast mail đến tất cả player.
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost("broadcast")]
        public async Task<IActionResult> SendMailToAll([FromBody] SendMailToAllDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            await _mailService.SendMailToAll(request);
            return Ok(new ApiResponse<object> { Success = true, Message = "Mail sent to all players successfully." });
        }
    }
}
