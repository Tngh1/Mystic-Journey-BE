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
    public class PlayerProfilesController : ControllerBase
    {
        private readonly IPlayerProfileService _playerProfileService;
        private readonly IMailService _mailService;
        private readonly IAuthRepository _authRepository;

        public PlayerProfilesController(
            IPlayerProfileService playerProfileService,
            IMailService mailService,
            IAuthRepository authRepository)
        {
            _playerProfileService = playerProfileService;
            _mailService = mailService;
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _playerProfileService.GetProfileById(id);
            return Ok(new ApiResponse<PlayerProfileDetailResponseDto> { Success = true, Data = result });
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePlayerProfileRequestDto dto)
        {
            var result = await _playerProfileService.UpdateProfile(id, dto);
            return Ok(new ApiResponse<PlayerProfileResponseDto> { Success = true, Data = result });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] int? level = null)
        {
            var result = await _playerProfileService.GetProfilesPaged(page, pageSize, search, level);
            return Ok(new ApiResponse<PagedResultDto<PlayerProfileResponseDto>> { Success = true, Data = result });
        }

        [Authorize]
        [HttpGet("me/inventory")]
        public async Task<IActionResult> GetMyInventory()
        {
            var playerProfileId = await GetCurrentPlayerProfileId();
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.Unauthorized });

            var result = await _playerProfileService.GetMeInventory(playerProfileId);
            return Ok(new ApiResponse<PlayerMeInventoryResponseDto> { Success = true, Data = result });
        }

        [Authorize]
        [HttpGet("me/skills")]
        public async Task<IActionResult> GetMySkills()
        {
            var playerProfileId = await GetCurrentPlayerProfileId();
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.Unauthorized });

            var result = await _playerProfileService.GetMeSkills(playerProfileId);
            return Ok(new ApiResponse<PlayerMeSkillsResponseDto> { Success = true, Data = result });
        }

        [Authorize]
        [HttpGet("me/quests")]
        public async Task<IActionResult> GetMyQuests()
        {
            var playerProfileId = await GetCurrentPlayerProfileId();
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.Unauthorized });

            var result = await _playerProfileService.GetMeQuests(playerProfileId);
            return Ok(new ApiResponse<PlayerMeQuestsResponseDto> { Success = true, Data = result });
        }

        [Authorize]
        [HttpGet("me/achievements")]
        public async Task<IActionResult> GetMyAchievements()
        {
            var playerProfileId = await GetCurrentPlayerProfileId();
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.Unauthorized });

            var result = await _playerProfileService.GetMeAchievements(playerProfileId);
            return Ok(new ApiResponse<PlayerMeAchievementsResponseDto> { Success = true, Data = result });
        }

        [Authorize]
        [HttpGet("me/friends")]
        public async Task<IActionResult> GetMyFriends()
        {
            var playerProfileId = await GetCurrentPlayerProfileId();
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.Unauthorized });

            var result = await _playerProfileService.GetFriends(playerProfileId);
            return Ok(new ApiResponse<List<PlayerProfileResponseDto>> { Success = true, Data = result });
        }

        [Authorize]
        [HttpGet("me/mails")]
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
