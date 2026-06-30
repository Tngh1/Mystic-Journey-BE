using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    // Quản lý skills của người chơi trong game.
    // Cho phép xem, nâng cấp, trang bị, mở khóa và phá skill.
    [Route("api/player-skills")]
    [ApiController]
    public class PlayerSkillsController : ControllerBase
    {
        private readonly ISkillService _skillService;

        public PlayerSkillsController(ISkillService skillService)
        {
            _skillService = skillService;
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

        // ── GET /api/player-skills/me ───────────────────────────────────────
        // Lấy danh sách skills của player đang đăng nhập.
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMySkills()
        {
            var playerProfileId = GetCurrentPlayerProfileId();
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.Unauthorized });

            var result = await _skillService.GetMeSkills(playerProfileId);
            return Ok(new ApiResponse<PlayerMeSkillsResponseDto> { Success = true, Data = result });
        }

        // ── POST /api/player-skills/upgrade ───────────────────────────────────
        // Nâng cấp skill của player.
        [Authorize]
        [HttpPost("upgrade")]
        public async Task<IActionResult> Upgrade([FromBody] UpgradePlayerSkillRequestDto request)
        {
            var playerProfileId = GetCurrentPlayerProfileId();
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.Unauthorized });

            var updated = await _skillService.UpgradePlayerSkill(playerProfileId, request);
            return Ok(new ApiResponse<PlayerSkillResponseDto> { Success = true, Data = updated });
        }

        // ── POST /api/player-skills/equip ─────────────────────────────────────
        // Trang bị skill vào slot.
        [Authorize]
        [HttpPost("equip")]
        public async Task<IActionResult> EquipSkill([FromBody] EquipSkillRequestDto request)
        {
            var playerProfileId = GetCurrentPlayerProfileId();
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.Unauthorized });

            var updated = await _skillService.EquipPlayerSkill(playerProfileId, request);
            return Ok(new ApiResponse<PlayerSkillResponseDto> { Success = true, Data = updated });
        }

        // ── POST /api/player-skills/unlock ────────────────────────────────────
        // Mở khóa skill mới cho player.
        [Authorize]
        [HttpPost("unlock")]
        public async Task<IActionResult> Unlock([FromBody] UnlockPlayerSkillRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var playerProfileId = GetCurrentPlayerProfileId();
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.Unauthorized });

            var created = await _skillService.UnlockPlayerSkill(playerProfileId, request);
            return Ok(new ApiResponse<PlayerSkillResponseDto> { Success = true, Data = created });
        }

        // ── POST /api/player-skills/dismantle ─────────────────────────────────
        // Phá skill để lấy nguyên liệu.
        [Authorize]
        [HttpPost("dismantle")]
        public async Task<IActionResult> Dismantle([FromBody] DismantlePlayerSkillRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var playerProfileId = GetCurrentPlayerProfileId();
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.Unauthorized });

            var updated = await _skillService.DismantlePlayerSkill(playerProfileId, request);
            return Ok(new ApiResponse<PlayerSkillResponseDto> { Success = true, Data = updated });
        }
    }
}
