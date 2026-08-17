using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/[controller]")]
    // Executes controller base operation.
    [ApiController]
    public class QuestsController : ControllerBase
    {
        private readonly IQuestService _questService;

        // Initializes a new instance of QuestsController with dependencies: questService.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public QuestsController(IQuestService questService)
        {
            _questService = questService;
        }


        // ─── Admin APIs ───────────────────────────────────────────────────────
        [Authorize(Roles = "Admin")]
        [HttpGet("npc-options")]
        // Retrieves NPC dialogue nodes and quest assignment options for map zones.
        public async Task<IActionResult> GetNpcOptions([FromQuery] string? mapName = null)
        {
            var npcs = await _questService.GetQuestNpcOptions(mapName); // Query quest-giver NPCs filtered by map
            return Ok(new ApiResponse<List<NPCResponseDto>> { Success = true, Data = npcs });
        }
        // ─── Player APIs ───────────────────────────────────────────────────────
        [Authorize]
        [HttpGet("{id}")]
        // Retrieves full quest definition and objective specifications by ID.
        public async Task<IActionResult> GetById(int id)
        {
            var quest = await _questService.GetQuestById(id); // Look up quest row
            if (quest == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Quest with id {id} not found.", ErrorCode = ErrorCodes.NotFound });

            return Ok(new ApiResponse<QuestResponseDto> { Success = true, Data = quest });
        }

        // ─── Admin APIs ───────────────────────────────────────────────────────
        [HttpGet]
        // Retrieves paginated list of all quests with map and type filters.
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
            var result = await _questService.GetQuestsPaged(page, pageSize, search, type, isActive, mapName, sortBy, sortOrder); // Query quests database table
            return Ok(new ApiResponse<PagedResultDto<QuestResponseDto>> { Success = true, Data = result });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        // Creates a new quest template and associated objectives.
        public async Task<IActionResult> Create([FromBody] UpdateQuestRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var quest = await _questService.CreateQuest(request); // Insert quest record and objective dependencies
            return CreatedAtAction(nameof(GetById), new { id = quest.QuestId }, new ApiResponse<QuestResponseDto> { Success = true, Data = quest });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        // Updates quest narrative text, targets, prerequisites, or reward yields.
        public async Task<IActionResult> Update(int id, [FromBody] UpdateQuestRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var quest = await _questService.UpdateQuest(id, request); // Save updated quest configuration
            return Ok(new ApiResponse<QuestResponseDto> { Success = true, Data = quest });
        }
    }
}
