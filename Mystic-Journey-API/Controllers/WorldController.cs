using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Security.Claims;

namespace Mystic_Journey_API.Controllers
{
    // Quản lý world (thế giới game) của người chơi.
    // Cho phép xem trạng thái world, tương tác với NPC, rương, quest, và nhận thưởng đăng nhập hàng ngày.
    [Route("api/world")]
    [ApiController]
    [Authorize]
    public class WorldController : ControllerBase
    {
        private readonly IWorldService _worldService;

        public WorldController(IWorldService worldService)
        {
            _worldService = worldService;
        }

        private int GetPlayerProfileId()
        {
            var claim = User.FindFirstValue("playerProfileId");
            if (!int.TryParse(claim, out var id))
                throw new UnauthorizedAccessException("PlayerProfileId is missing from token. Please login again.");
            return id;
        }

        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // ── GET /api/world/state ─────────────────────────────────────────
        // Lấy trạng thái world của player (vị trí, quest đang thực hiện, NPCs...).
        [HttpGet("state")]
        public async Task<IActionResult> GetState()
        {
            var profileId = GetPlayerProfileId();
            var result = await _worldService.GetWorldState(profileId);
            return Ok(new ApiResponse<WorldStateResponseDto> { Success = true, Data = result });
        }

        // ── PUT /api/world/position ─────────────────────────────────────
        // Cập nhật vị trí của player trong world (map, tọa độ).
        [HttpPut("position")]
        public async Task<IActionResult> UpdatePosition([FromBody] UpdateWorldPositionRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var profileId = GetPlayerProfileId();
            var result = await _worldService.UpdatePosition(profileId, request);
            return Ok(new ApiResponse<PlayerWorldPositionDto> { Success = true, Data = result });
        }

        // ── POST /api/world/npc/talk ────────────────────────────────────
        // Nói chuyện với NPC, nhận dialogue và quest.
        [HttpPost("npc/talk")]
        public async Task<IActionResult> TalkToNpc([FromBody] TalkToNpcRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var profileId = GetPlayerProfileId();
            var result = await _worldService.TalkToNpc(profileId, request);
            return Ok(new ApiResponse<TalkToNpcResponseDto> { Success = true, Data = result });
        }

        // ── POST /api/world/npc/turn-in ─────────────────────────────────
        // Nộp item quest cho NPC.
        [HttpPost("npc/turn-in")]
        public async Task<IActionResult> TurnInQuestItem([FromBody] TurnInQuestItemRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var profileId = GetPlayerProfileId();
            var result = await _worldService.TurnInQuestItem(profileId, request);
            return Ok(new ApiResponse<TurnInQuestItemResponseDto> { Success = true, Data = result });
        }

        // ── POST /api/world/chests/open ─────────────────────────────────
        // Mở rương trong world.
        [HttpPost("chests/open")]
        public async Task<IActionResult> OpenChest([FromBody] OpenWorldChestRequestDto request)
        {
            var profileId = GetPlayerProfileId();
            var result = await _worldService.OpenChest(profileId, request);
            return Ok(new ApiResponse<OpenChestResponseDto> { Success = true, Data = result });
        }

        // ── POST /api/world/interactions ────────────────────────────────
        // Tương tác với object trong world (lever, button, v.v.).
        [HttpPost("interactions")]
        public async Task<IActionResult> InteractWithObject([FromBody] InteractObjectRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var profileId = GetPlayerProfileId();
            var result = await _worldService.InteractWithObject(profileId, request);
            return Ok(new ApiResponse<InteractObjectResponseDto> { Success = true, Data = result });
        }

        // ── POST /api/world/daily-login/claim ───────────────────────────
        // Nhận thưởng đăng nhập hàng ngày.
        [HttpPost("daily-login/claim")]
        public async Task<IActionResult> ClaimDailyLoginReward()
        {
            var profileId = GetPlayerProfileId();
            var result = await _worldService.ClaimDailyLoginReward(profileId);
            return Ok(new ApiResponse<ClaimDailyRewardResponseDto> { Success = true, Data = result });
        }

        // ── POST /api/world/daily-login/retro-claim ─────────────────────
        // Nhận thưởng bù ngày trước (retroactive claim).
        [HttpPost("daily-login/retro-claim")]
        public async Task<IActionResult> RetroactiveClaimDailyLoginReward([FromBody] RetroClaimRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var profileId = GetPlayerProfileId();
            var result = await _worldService.RetroactiveClaimDailyLoginReward(profileId, request.DayNumber);
            return Ok(new ApiResponse<ClaimDailyRewardResponseDto> { Success = true, Data = result });
        }

        // ── POST /api/world/claim-drop ──────────────────────────────────
        // Nhặt vật phẩm rơi ra map thế giới (World Drop Pickup)
        [HttpPost("claim-drop")]
        public async Task<IActionResult> ClaimDrop([FromBody] ClaimDropRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var profileId = GetPlayerProfileId();
            var result = await _worldService.ClaimDrop(profileId, request);
            return Ok(new ApiResponse<ClaimDropResponseDto> { Success = true, Data = result });
        }
    }
}
