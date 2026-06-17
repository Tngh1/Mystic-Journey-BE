using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/playerquests")]
    [ApiController]
    [Authorize]   // Tất cả endpoint đều cần auth
    public class PlayerQuestsController : ControllerBase
    {
        private readonly IPlayerQuestService _service;

        public PlayerQuestsController(IPlayerQuestService service)
        {
            _service = service;
        }

        // ── Helper: lấy PlayerProfileId từ JWT claim ─────────────────────────
        private int GetPlayerProfileId()
        {
            // Claim "playerProfileId" được set khi login-game
            var claim = User.FindFirstValue("playerProfileId");
            if (!int.TryParse(claim, out var id))
                throw new UnauthorizedAccessException("PlayerProfileId không hợp lệ trong token.");
            return id;
        }

        // ── GET /api/playerquests/me ─────────────────────────────────────────
        /// <summary>UC 25.1 – Lấy danh sách quest của player đang đăng nhập.</summary>
        [HttpGet("me")]
        public async Task<IActionResult> GetMyQuests()
        {
            try
            {
                var profileId = GetPlayerProfileId();
                var result = await _service.GetMyQuests(profileId);
                return Ok(new { success = true, data = result });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // ── POST /api/playerquests/accept ────────────────────────────────────
        /// <summary>UC 25.3 – Accept quest mới.</summary>
        [HttpGet("{questId:int}")]
        public async Task<IActionResult> GetMyQuestDetail(int questId)
        {
            try
            {
                var profileId = GetPlayerProfileId();
                var result = await _service.GetMyQuestDetail(profileId, questId);
                if (result == null)
                    return NotFound(new { success = false, message = $"Quest {questId} not found on current map." });

                return Ok(new { success = true, data = result });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("accept")]
        public async Task<IActionResult> AcceptQuest([FromBody] AcceptQuestRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var profileId = GetPlayerProfileId();
                var result = await _service.AcceptQuest(profileId, request);
                return Ok(new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // ── PUT /api/playerquests/batch-progress ────────────────────────────
        /// <summary>UC 25.4 – Batch cập nhật progress (gọi mỗi 1 giây từ QuestManager).</summary>
        [HttpPut("batch-progress")]
        public async Task<IActionResult> BatchUpdateProgress([FromBody] BatchProgressRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var profileId = GetPlayerProfileId();
                var result = await _service.BatchUpdateProgress(profileId, request);
                return Ok(new { success = true, data = result });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // ── POST /api/playerquests/claim ─────────────────────────────────────
        /// <summary>UC 25.5 – Nhận phần thưởng quest đã Completed.</summary>
        [HttpPost("complete")]
        public async Task<IActionResult> CompleteQuest([FromBody] CompleteQuestRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var profileId = GetPlayerProfileId();
                var result = await _service.CompleteQuest(profileId, request);
                return Ok(new { success = true, data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("claim")]
        public async Task<IActionResult> ClaimReward([FromBody] ClaimQuestRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var profileId = GetPlayerProfileId();
                var result = await _service.ClaimReward(profileId, request);
                return Ok(new { success = true, data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
