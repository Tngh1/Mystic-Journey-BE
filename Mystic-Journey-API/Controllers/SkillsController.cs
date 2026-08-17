using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/[controller]")]
    // Executes controller base operation.
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class SkillsController : ControllerBase
    {
        private readonly ISkillService _skillService;

        // Initializes a new instance of SkillsController with dependencies: skillService.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public SkillsController(ISkillService skillService)
        {
            _skillService = skillService;
        }


        // ─── Admin APIs ───────────────────────────────────────────────────────
        [HttpGet("{id}")]
        // Executes get by id operation.
        public async Task<IActionResult> GetById(int id)
        {
            var skill = await _skillService.GetSkillById(id);
            if (skill == null)  // Entity not found — short-circuit with appropriate error result
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Skill with id {id} not found.", ErrorCode = ErrorCodes.NotFound });  // Return HTTP 404 when the requested resource does not exist

            return Ok(new ApiResponse<SkillResponseDto> { Success = true, Data = skill });  // Return HTTP 200 with standard ApiResponse envelope
        }

        [HttpGet]
        // Executes get all operation.
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] string? type = null, [FromQuery] bool? isActive = null)
        {
            var result = await _skillService.GetSkillsPaged(page, pageSize, search, type, isActive);
            return Ok(new ApiResponse<PagedResultDto<SkillResponseDto>> { Success = true, Data = result });  // Return HTTP 200 with standard ApiResponse envelope
        }

        [HttpPost]
        // Executes create operation.
        public async Task<IActionResult> Create([FromBody] CreateSkillRequestDto request)
        {
            if (!ModelState.IsValid)  // Reject request immediately if any DTO validation annotation fails
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });  // Return HTTP 400 with validation error details

            var skill = await _skillService.CreateSkill(request);
            return Ok(new ApiResponse<SkillResponseDto> { Success = true, Data = skill });  // Return HTTP 200 with standard ApiResponse envelope
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        // Per-frame update loop for SkillsController.
        // Handles real-time input polling, smooth interpolations, cooldown timers, and UI updates.
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSkillRequestDto request)
        {
            if (!ModelState.IsValid)  // Reject request immediately if any DTO validation annotation fails
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });  // Return HTTP 400 with validation error details

            var skill = await _skillService.UpdateSkill(id, request);
            return Ok(new ApiResponse<SkillResponseDto> { Success = true, Data = skill });  // Return HTTP 200 with standard ApiResponse envelope
        }
    }
}
