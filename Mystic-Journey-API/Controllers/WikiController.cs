using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{

    // Executes controller base operation.
    [Route("api/wiki")]
    [ApiController]
    [AllowAnonymous]
    public class WikiController : ControllerBase
    {
        private readonly IWikiService _wikiService;

        // Initializes a new instance of WikiController with dependencies: wikiService.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public WikiController(IWikiService wikiService)
        {
            _wikiService = wikiService;
        }


        // ─── Guest APIs ───────────────────────────────────────────────────────
        [HttpGet("classes")]
        // Retrieves public information about available character classes (Knight, Archer, Mage) and their stat growth.
        public async Task<IActionResult> GetClasses()
        {
            var configs = await _wikiService.GetClasses(); // Query class baseline statistics and sprite assets
            return Ok(new ApiResponse<IEnumerable<ClassConfigResponseDto>> { Success = true, Data = configs });  // Return HTTP 200 with standard ApiResponse envelope
        }


        [HttpGet("monsters")]
        // Retrieves public game encyclopedia monster list with pagination, search, and type filter.
        public async Task<IActionResult> GetMonsters(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null,
            [FromQuery] string? type = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = null)
        {
            var result = await _wikiService.GetMonsters(page, pageSize, search, type, sortBy, sortOrder); // Query public monster wiki database
            return Ok(new ApiResponse<PagedResultDto<MonsterResponseDto>> { Success = true, Data = result });  // Return HTTP 200 with standard ApiResponse envelope
        }

        [HttpGet("monsters/{id:int}")]
        // Retrieves detailed wiki profile for a monster (hp, attack, defense, movesets, locations).
        public async Task<IActionResult> GetMonsterDetail(int id)
        {
            var monster = await _wikiService.GetMonsterById(id); // Look up monster wiki entry
            if (monster == null)  // Entity not found — short-circuit with appropriate error result
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Monster with id {id} not found.", ErrorCode = ErrorCodes.NotFound });  // Return HTTP 404 when the requested resource does not exist

            return Ok(new ApiResponse<MonsterDetailResponseDto> { Success = true, Data = monster });  // Return HTTP 200 with standard ApiResponse envelope
        }


        [HttpGet("items")]
        // Retrieves public item catalog with rarity and category filters.
        public async Task<IActionResult> GetItems(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null,
            [FromQuery] string? type = null,
            [FromQuery] string? rarity = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = null)
        {
            var result = await _wikiService.GetItems(page, pageSize, search, type, rarity, sortBy, sortOrder); // Query public item wiki database
            return Ok(new ApiResponse<PagedResultDto<ItemResponseDto>> { Success = true, Data = result });  // Return HTTP 200 with standard ApiResponse envelope
        }

        [HttpGet("items/{id:int}")]
        // Retrieves item details, stat modifiers, craft requirements, and lore description.
        public async Task<IActionResult> GetItemDetail(int id)
        {
            var item = await _wikiService.GetItemById(id); // Look up item wiki entry
            if (item == null)  // Entity not found — short-circuit with appropriate error result
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Item with id {id} not found.", ErrorCode = ErrorCodes.NotFound });  // Return HTTP 404 when the requested resource does not exist

            return Ok(new ApiResponse<ItemResponseDto> { Success = true, Data = item });  // Return HTTP 200 with standard ApiResponse envelope
        }


        [HttpGet("skills")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        // Retrieves public skill directory with damage scalings and element categories.
        public async Task<IActionResult> GetSkills(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null,
            [FromQuery] string? type = null)
        {
            var result = await _wikiService.GetSkills(page, pageSize, search, type); // Query public skills wiki database
            return Ok(new ApiResponse<PagedResultDto<SkillResponseDto>> { Success = true, Data = result });  // Return HTTP 200 with standard ApiResponse envelope
        }

        [HttpGet("skills/{id:int}")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        // Retrieves skill details, cooldown formula, damage multipliers, and area effects.
        public async Task<IActionResult> GetSkillDetail(int id)
        {
            var skill = await _wikiService.GetSkillById(id); // Look up skill wiki entry
            if (skill == null)  // Entity not found — short-circuit with appropriate error result
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Skill with id {id} not found.", ErrorCode = ErrorCodes.NotFound });  // Return HTTP 404 when the requested resource does not exist

            return Ok(new ApiResponse<SkillResponseDto> { Success = true, Data = skill });  // Return HTTP 200 with standard ApiResponse envelope
        }
    }
}
