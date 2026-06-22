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
    public class MonstersController : ControllerBase
    {
        private readonly IMonsterService _monsterService;

        public MonstersController(IMonsterService monsterService)
        {
            _monsterService = monsterService;
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var monster = await _monsterService.GetMonsterById(id);
            if (monster == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Monster with id {id} not found.", ErrorCode = ErrorCodes.NotFound });

            return Ok(new ApiResponse<MonsterDetailResponseDto> { Success = true, Data = monster });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMonsterRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var monster = await _monsterService.CreateMonster(request);
            return Ok(new ApiResponse<MonsterResponseDto> { Success = true, Data = monster });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMonsterRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var monster = await _monsterService.UpdateMonster(id, request);
            return Ok(new ApiResponse<MonsterResponseDto> { Success = true, Data = monster });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost("{id}/drops")]
        public async Task<IActionResult> AddDrop(int id, [FromBody] CreateMonsterDropRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var drop = await _monsterService.AddMonsterDrop(id, request);
            return Ok(new ApiResponse<MonsterDropResponseDto> { Success = true, Data = drop });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] string? type = null, [FromQuery] bool? isActive = null)
        {
            var result = await _monsterService.GetMonstersPaged(page, pageSize, search, type, isActive);
            return Ok(new ApiResponse<PagedResultDto<MonsterResponseDto>> { Success = true, Data = result });
        }

        [HttpGet("drops")]
        public async Task<IActionResult> GetAllDrops([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _monsterService.GetMonsterDropsPaged(page, pageSize);
            return Ok(new ApiResponse<PagedResultDto<MonsterDropResponseDto>> { Success = true, Data = result });
        }
    }
}
