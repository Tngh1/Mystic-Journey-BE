using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Collections.Generic;
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

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var mail = await _mailService.GetMailById(id);
            if (mail == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Mail with id {id} not found.", ErrorCode = ErrorCodes.NotFound });

            return Ok(new ApiResponse<MailResponseDto> { Success = true, Data = mail });
        }

        [AllowAnonymous]
        [HttpGet("player/{playerProfileId}")]
        public async Task<IActionResult> GetByPlayerId(int playerProfileId)
        {
            var mails = await _mailService.GetMailsByPlayerId(playerProfileId);
            return Ok(new ApiResponse<List<MailResponseDto>> { Success = true, Data = mails });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost("by-ids")]
        public async Task<IActionResult> SendMailByListId([FromBody] SendMailByListIdDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            await _mailService.SendMailByListId(request);
            return Ok(new ApiResponse<object> { Success = true, Message = "Mail sent successfully." });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost("broadcast")]
        public async Task<IActionResult> SendMailToAll([FromBody] SendMailToAllDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            await _mailService.SendMailToAll(request);
            return Ok(new ApiResponse<object> { Success = true, Message = "Mail sent to all players successfully." });
        }

        [Authorize]
        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var mail = await _mailService.MarkMailAsRead(id);
            return Ok(new ApiResponse<MailResponseDto> { Success = true, Data = mail });
        }

        [Authorize]
        [HttpPost("{id}/claim")]
        public async Task<IActionResult> ClaimReward(int id)
        {
            var mail = await _mailService.ClaimMailReward(id);
            return Ok(new ApiResponse<MailResponseDto> { Success = true, Data = mail });
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMail(int id, [FromQuery] int playerProfileId)
        {
            await _mailService.DeleteMail(id, playerProfileId);
            return Ok(new ApiResponse<object> { Success = true, Message = "Mail deleted successfully." });
        }

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
            return Ok(new ApiResponse<PagedResultDto<MailResponseDto>> { Success = true, Data = result });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyMails()
        {
            var playerProfileId = await GetCurrentPlayerProfileId();
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.Unauthorized });

            var result = await _mailService.GetMeMails(playerProfileId);
            return Ok(new ApiResponse<PlayerMeMailsResponseDto> { Success = true, Data = result });
        }
    }
}
