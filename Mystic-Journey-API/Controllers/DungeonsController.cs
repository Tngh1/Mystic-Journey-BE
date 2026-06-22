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
    public class DungeonsController : ControllerBase
    {
        private readonly IDungeonConfigService _dungeonConfigService;

        public DungeonsController(IDungeonConfigService dungeonConfigService)
        {
            _dungeonConfigService = dungeonConfigService;
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var dungeon = await _dungeonConfigService.GetDungeonById(id);
            if (dungeon == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Dungeon with id {id} not found.", ErrorCode = ErrorCodes.NotFound });

            return Ok(new ApiResponse<DungeonConfigResponseDto> { Success = true, Data = dungeon });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDungeonConfigRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var dungeon = await _dungeonConfigService.CreateDungeon(request);
            return Ok(new ApiResponse<DungeonConfigResponseDto> { Success = true, Data = dungeon });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDungeonConfigRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var dungeon = await _dungeonConfigService.UpdateDungeon(id, request);
            return Ok(new ApiResponse<DungeonConfigResponseDto> { Success = true, Data = dungeon });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] string? type = null, [FromQuery] bool? isActive = null)
        {
            var result = await _dungeonConfigService.GetDungeonsPaged(page, pageSize, search, type, isActive);
            return Ok(new ApiResponse<PagedResultDto<DungeonConfigResponseDto>> { Success = true, Data = result });
        }
    }
}
