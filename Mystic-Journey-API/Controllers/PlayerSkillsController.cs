using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/player-skills")]
    [ApiController]
    public class PlayerSkillsController : ControllerBase
    {
        private readonly ISkillService _skillService;
        private readonly IAuthRepository _authRepository;

        public PlayerSkillsController(ISkillService skillService, IAuthRepository authRepository)
        {
            _skillService = skillService;
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

        [Authorize]
        [HttpPost("upgrade")]
        public async Task<IActionResult> Upgrade([FromBody] UpgradePlayerSkillRequestDto request)
        {
            var playerProfileId = await GetCurrentPlayerProfileId();
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.Unauthorized });

            var updated = await _skillService.UpgradePlayerSkill(playerProfileId, request);
            return Ok(new ApiResponse<PlayerSkillResponseDto> { Success = true, Data = updated });
        }

        [Authorize]
        [HttpPost("equip")]
        public async Task<IActionResult> Equip([FromBody] EquipSkillRequestDto request)
        {
            var playerProfileId = await GetCurrentPlayerProfileId();
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.Unauthorized });

            var updated = await _skillService.EquipPlayerSkill(playerProfileId, request);
            return Ok(new ApiResponse<PlayerSkillResponseDto> { Success = true, Data = updated });
        }

        [Authorize]
        [HttpPost("unlock")]
        public async Task<IActionResult> Unlock([FromBody] UnlockPlayerSkillRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var playerProfileId = await GetCurrentPlayerProfileId();
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.Unauthorized });

            var created = await _skillService.UnlockPlayerSkill(playerProfileId, request);
            return Ok(new ApiResponse<PlayerSkillResponseDto> { Success = true, Data = created });
        }

        [Authorize]
        [HttpPost("dismantle")]
        public async Task<IActionResult> Dismantle([FromBody] DismantlePlayerSkillRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var playerProfileId = await GetCurrentPlayerProfileId();
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.Unauthorized });

            var updated = await _skillService.DismantlePlayerSkill(playerProfileId, request);
            return Ok(new ApiResponse<PlayerSkillResponseDto> { Success = true, Data = updated });
        }
    }
}
