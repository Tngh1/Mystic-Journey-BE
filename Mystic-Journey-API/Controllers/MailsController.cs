using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MailsController : ControllerBase
    {
        private readonly IMailService _mailService;

        public MailsController(IMailService mailService)
        {
            _mailService = mailService;
        }

        private Guid GetAccountId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        [HttpGet]
        public async Task<IActionResult> GetMails([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var accountId = GetAccountId();
            var result = await _mailService.GetMailsAsync(accountId, pageNumber, pageSize);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("unread")]
        public async Task<IActionResult> GetUnreadMails()
        {
            var accountId = GetAccountId();
            var result = await _mailService.GetUnreadMailsAsync(accountId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("unread/count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var accountId = GetAccountId();
            var count = await _mailService.GetUnreadCountAsync(accountId);
            return Ok(new { Success = true, UnreadCount = count });
        }

        [HttpGet("{mailId}")]
        public async Task<IActionResult> GetMailById(Guid mailId)
        {
            var accountId = GetAccountId();
            var result = await _mailService.GetMailByIdAsync(accountId, mailId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost("{mailId}/read")]
        public async Task<IActionResult> MarkAsRead(Guid mailId)
        {
            var accountId = GetAccountId();
            var result = await _mailService.MarkAsReadAsync(accountId, mailId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("{mailId}/claim")]
        public async Task<IActionResult> ClaimMail(Guid mailId)
        {
            var accountId = GetAccountId();
            var result = await _mailService.ClaimMailAsync(accountId, mailId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("send")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SendMail([FromBody] SendMailRequestDto request)
        {
            var accountId = GetAccountId();
            var result = await _mailService.SendMailAsync(accountId, request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpDelete("{mailId}")]
        public async Task<IActionResult> DeleteMail(Guid mailId)
        {
            var accountId = GetAccountId();
            var result = await _mailService.DeleteMailAsync(accountId, mailId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
