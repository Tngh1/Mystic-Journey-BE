using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestsController : ControllerBase
    {
        private readonly IQuestService _questService;

        public QuestsController(IQuestService questService)
        {
            _questService = questService;
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

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateQuestRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var quest = await _questService.CreateQuest(request);
            return Ok(new ApiResponse<QuestResponseDto> { Success = true, Data = quest });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateQuestRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var quest = await _questService.UpdateQuest(id, request);
            return Ok(new ApiResponse<QuestResponseDto> { Success = true, Data = quest });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] string? type = null, [FromQuery] bool? isActive = null, [FromQuery] string? mapName = null)
        {
            var result = await _questService.GetQuestsPaged(page, pageSize, search, type, isActive, mapName);
            return Ok(new ApiResponse<PagedResultDto<QuestResponseDto>> { Success = true, Data = result });
        }
    }
}
