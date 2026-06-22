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
    public class SkillsController : ControllerBase
    {
        private readonly ISkillService _skillService;

        public SkillsController(ISkillService skillService)
        {
            _skillService = skillService;
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var skill = await _skillService.GetSkillById(id);
            if (skill == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Skill with id {id} not found.", ErrorCode = ErrorCodes.NotFound });

            return Ok(new ApiResponse<SkillResponseDto> { Success = true, Data = skill });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSkillRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var skill = await _skillService.CreateSkill(request);
            return Ok(new ApiResponse<SkillResponseDto> { Success = true, Data = skill });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSkillRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var skill = await _skillService.UpdateSkill(id, request);
            return Ok(new ApiResponse<SkillResponseDto> { Success = true, Data = skill });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] string? type = null, [FromQuery] bool? isActive = null)
        {
            var result = await _skillService.GetSkillsPaged(page, pageSize, search, type, isActive);
            return Ok(new ApiResponse<PagedResultDto<SkillResponseDto>> { Success = true, Data = result });
        }
    }
}
