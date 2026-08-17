using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    // Executes controller base operation.
    [Route("api/player-skills")]
    [ApiController]
    public class PlayerSkillsController : ControllerBase
    {
        private readonly ISkillService _skillService;

        // Initializes a new instance of PlayerSkillsController with dependencies: skillService.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public PlayerSkillsController(ISkillService skillService)
        {
            _skillService = skillService;
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
        // Retrieves unlocked skills, skill shards, equipped loadouts, and levels for the player.
        public async Task<IActionResult> GetMySkills()
        {
            var playerProfileId = GetCurrentPlayerProfileId(); // Extract caller's profile ID from JWT claim
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.Unauthorized });

            var result = await _skillService.GetMeSkills(playerProfileId); // Query player's skill collection and equipped active slots
            return Ok(new ApiResponse<PlayerMeSkillsResponseDto> { Success = true, Data = result });
        }

        [Authorize]
        [HttpPost("upgrade")]
        // Upgrades skill level using skill books/shards and Gold.
        public async Task<IActionResult> Upgrade([FromBody] UpgradePlayerSkillRequestDto request)
        {
            var playerProfileId = GetCurrentPlayerProfileId(); // Extract caller's profile ID from JWT claim
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.Unauthorized });

            var updated = await _skillService.UpgradePlayerSkill(playerProfileId, request); // Verify required shards/gold, increase skill level, and recalculate damage scaling
            return Ok(new ApiResponse<PlayerSkillResponseDto> { Success = true, Data = updated });
        }

        [Authorize]
        [HttpPost("equip")]
        // Assigns an unlocked active skill to an action bar slot (slot 1, 2, or 3).
        public async Task<IActionResult> EquipSkill([FromBody] EquipSkillRequestDto request)
        {
            var playerProfileId = GetCurrentPlayerProfileId(); // Extract caller's profile ID from JWT claim
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.Unauthorized });

            var updated = await _skillService.EquipPlayerSkill(playerProfileId, request); // Update slot binding, swap out any previous skill on that slot, and save loadout
            return Ok(new ApiResponse<PlayerSkillResponseDto> { Success = true, Data = updated });
        }


        [Authorize]
        [HttpPost("dismantle")]
        // Dismantles duplicate skill fragments into universal upgrade dust.
        public async Task<IActionResult> Dismantle([FromBody] DismantlePlayerSkillRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var playerProfileId = GetCurrentPlayerProfileId(); // Extract caller's profile ID from JWT claim
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.Unauthorized });

            var updated = await _skillService.DismantlePlayerSkill(playerProfileId, request); // Remove selected shards and credit universal skill essence
            return Ok(new ApiResponse<PlayerSkillResponseDto> { Success = true, Data = updated });
        }

        [HttpPost("record-cast/{id}")]
        // Synchronizes a server-side cooldown timestamp when a skill is cast in gameplay.
        public async Task<IActionResult> RecordSkillCast(int id)
        {
            var playerProfileId = GetCurrentPlayerProfileId(); // Extract caller's profile ID from JWT claim
            if (playerProfileId == 0)
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.Unauthorized });

            try
            {
                var updated = await _skillService.RecordSkillCast(playerProfileId, id); // Calculate and persist NextAvailableTime cooldown timestamp in database
                return Ok(new ApiResponse<PlayerSkillResponseDto> { Success = true, Data = updated });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = ex.Message, ErrorCode = ErrorCodes.NotFound });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new ApiResponse<object> { Success = false, Message = ex.Message, ErrorCode = ErrorCodes.Unauthorized });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = ex.Message, ErrorCode = ErrorCodes.InvalidOperation });
            }
        }
    }
}
