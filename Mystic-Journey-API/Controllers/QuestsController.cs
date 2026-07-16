using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    // Quản lý quests (nhiệm vụ) cho admin.
    // Admin APIs: Tạo, cập nhật và xem danh sách quests.
    [Route("api/[controller]")]
    [ApiController]
    public class QuestsController : ControllerBase
    {
        private readonly IQuestService _questService;

        public QuestsController(IQuestService questService)
        {
            _questService = questService;
        }

        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // ── GET /api/quests/{id} ───────────────────────────────────────
        // Lấy chi tiết quest theo ID.
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet("npc-options")]
        public async Task<IActionResult> GetNpcOptions([FromQuery] string? mapName = null)
        {
            var npcs = await _questService.GetQuestNpcOptions(mapName);
            return Ok(new ApiResponse<List<NPCResponseDto>> { Success = true, Data = npcs });
        }
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var quest = await _questService.GetQuestById(id);
            if (quest == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Quest with id {id} not found.", ErrorCode = ErrorCodes.NotFound });

            return Ok(new ApiResponse<QuestResponseDto> { Success = true, Data = quest });
        }

        // ── GET /api/quests ─────────────────────────────────────────────
        // Lấy danh sách tất cả quests có phân trang và lọc.
        // Query: page, pageSize, search, type, isActive, mapName, sortBy, sortOrder.
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? type = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] string? mapName = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = null)
        {
            var result = await _questService.GetQuestsPaged(page, pageSize, search, type, isActive, mapName, sortBy, sortOrder);
            return Ok(new ApiResponse<PagedResultDto<QuestResponseDto>> { Success = true, Data = result });
        }

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════
        // Tao quest moi.
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UpdateQuestRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var quest = await _questService.CreateQuest(request);
            return CreatedAtAction(nameof(GetById), new { id = quest.QuestId }, new ApiResponse<QuestResponseDto> { Success = true, Data = quest });
        }

        // ── PUT /api/quests/{id} ───────────────────────────────────────
        // Cập nhật quest hiện có.
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateQuestRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var quest = await _questService.UpdateQuest(id, request);
            return Ok(new ApiResponse<QuestResponseDto> { Success = true, Data = quest });
        }
    }
}
