using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailsController : ControllerBase
    {
        private readonly IMailService _mailService;
        private readonly IPlayerProfileService _playerProfileService;
        private readonly IAuthRepository _authRepository;

        public MailsController(
            IMailService mailService,
            IPlayerProfileService playerProfileService,
            IAuthRepository authRepository)
        {
            _mailService = mailService;
            _playerProfileService = playerProfileService;
            _authRepository = authRepository;
        }

        // ========== SHARED: Helper Methods ==========

        private int GetCurrentAccountId()
        {
            var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }

        private async Task<int> GetCurrentPlayerProfileId()
        {
            var accountId = GetCurrentAccountId();
            var account = await _authRepository.GetAccountById(accountId);
            return account?.PlayerProfile?.PlayerProfileId ?? 0;
        }

        // ========== PLAYER: View Mail ==========
        // Dành cho người chơi - Xem chi tiết mail

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var mail = await _mailService.GetMailById(id);
                if (mail == null)
                    return NotFound(new { message = $"Mail with id {id} not found." });

                return Ok(mail);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // ========== PLAYER: View Player's Mails ==========
        // Dành cho người chơi - Xem danh sách mail của một player

        [AllowAnonymous]
        [HttpGet("player/{playerProfileId}")]
        public async Task<IActionResult> GetByPlayerId(int playerProfileId)
        {
            try
            {
                var mails = await _mailService.GetMailsByPlayerId(playerProfileId);
                return Ok(mails);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // ========== MANAGER: Send Mail (Dashboard) ==========
        // Dành cho Admin/Manager - Gửi mail cho người chơi từ dashboard

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost("by-ids")]
        public async Task<IActionResult> SendMailByListId([FromBody] SendMailByListIdDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _mailService.SendMailByListId(request);
                return Ok(new { message = "Mail sent successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost("broadcast")]
        public async Task<IActionResult> SendMailToAll([FromBody] SendMailToAllDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _mailService.SendMailToAll(request);
                return Ok(new { message = "Mail sent to all players successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // ========== PLAYER: Manage Own Mail ==========
        // Dành cho người chơi - Đánh dấu đã đọc, nhận thưởng, xóa mail

        [Authorize]
        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            try
            {
                var mail = await _mailService.MarkMailAsRead(id);
                return Ok(mail);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("{id}/claim")]
        public async Task<IActionResult> ClaimReward(int id)
        {
            try
            {
                var mail = await _mailService.ClaimMailReward(id);
                return Ok(mail);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMail(int id, [FromQuery] int playerProfileId)
        {
            try
            {
                var mail = await _mailService.DeleteMail(id, playerProfileId);
                return Ok(new { message = "Mail deleted successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // ========== MANAGER: Mail Management (Dashboard) ==========
        // Dành cho Admin/Manager - Quản lý danh sách mail trên dashboard

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
            return Ok(result);
        }

        // ========== PLAYER: Get Own Mails (/me endpoint) ==========
        // Dành cho người chơi - Lấy danh sách mail của chính mình

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyMails()
        {
            try
            {
                var playerProfileId = await GetCurrentPlayerProfileId();
                if (playerProfileId == 0)
                    return Unauthorized(new { message = "Player profile not found." });

                var result = await _mailService.GetMeMails(playerProfileId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
