using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    // ═══════════════════════════════════════════════════════════════════════
    // WIKI CONTROLLER - Codex công khai cho web wiki
    // ═══════════════════════════════════════════════════════════════════════

    [Route("api/wiki")]
    [ApiController]
    [AllowAnonymous]
    public class WikiController : ControllerBase
    {
        private readonly IWikiService _wikiService;

        public WikiController(IWikiService wikiService)
        {
            _wikiService = wikiService;
        }

        // ═══════════════════════════════════════════════════════════════════════
        // CLASSES - Chỉ số khởi điểm của các Class
        // ═══════════════════════════════════════════════════════════════════════

        // ── GET /api/wiki/classes ──────────────────────────────────
        // Không phân trang: chỉ có 3 class, và trang wiki tính trần chỉ số trên
        // toàn bộ tập nên luôn cần đủ.
        [HttpGet("classes")]
        public async Task<IActionResult> GetClasses()
        {
            var configs = await _wikiService.GetClasses();
            return Ok(new ApiResponse<object> { Success = true, Data = configs });
        }

        // ═══════════════════════════════════════════════════════════════════════
        // MONSTERS
        // ═══════════════════════════════════════════════════════════════════════

        // ── GET /api/wiki/monsters ─────────────────────────────────
        [HttpGet("monsters")]
        public async Task<IActionResult> GetMonsters(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null,
            [FromQuery] string? type = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = null)
        {
            var result = await _wikiService.GetMonsters(page, pageSize, search, type, sortBy, sortOrder);
            return Ok(new ApiResponse<PagedResultDto<MonsterResponseDto>> { Success = true, Data = result });
        }

        // ── GET /api/wiki/monsters/{id} ────────────────────────────
        [HttpGet("monsters/{id:int}")]
        public async Task<IActionResult> GetMonsterDetail(int id)
        {
            var monster = await _wikiService.GetMonsterById(id);
            if (monster == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Monster with id {id} not found.", ErrorCode = ErrorCodes.NotFound });

            return Ok(new ApiResponse<MonsterDetailResponseDto> { Success = true, Data = monster });
        }

        // ═══════════════════════════════════════════════════════════════════════
        // ITEMS
        // ═══════════════════════════════════════════════════════════════════════

        // ── GET /api/wiki/items ────────────────────────────────────
        [HttpGet("items")]
        public async Task<IActionResult> GetItems(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null,
            [FromQuery] string? type = null,
            [FromQuery] string? rarity = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = null)
        {
            var result = await _wikiService.GetItems(page, pageSize, search, type, rarity, sortBy, sortOrder);
            return Ok(new ApiResponse<PagedResultDto<ItemResponseDto>> { Success = true, Data = result });
        }

        // ── GET /api/wiki/items/{id} ───────────────────────────────
        [HttpGet("items/{id:int}")]
        public async Task<IActionResult> GetItemDetail(int id)
        {
            var item = await _wikiService.GetItemById(id);
            if (item == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Item with id {id} not found.", ErrorCode = ErrorCodes.NotFound });

            return Ok(new ApiResponse<ItemResponseDto> { Success = true, Data = item });
        }

        // ═══════════════════════════════════════════════════════════════════════
        // SKILLS
        // ═══════════════════════════════════════════════════════════════════════

        // ── GET /api/wiki/skills ───────────────────────────────────
        // Không có sortBy/sortOrder: WikiRepository luôn xếp theo UnlockLevel
        // (lộ trình mở khoá) — đó là thứ tự duy nhất codex cần.
        [HttpGet("skills")]
        public async Task<IActionResult> GetSkills(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null,
            [FromQuery] string? type = null)
        {
            var result = await _wikiService.GetSkills(page, pageSize, search, type);
            return Ok(new ApiResponse<PagedResultDto<SkillResponseDto>> { Success = true, Data = result });
        }

        // ── GET /api/wiki/skills/{id} ──────────────────────────────
        [HttpGet("skills/{id:int}")]
        public async Task<IActionResult> GetSkillDetail(int id)
        {
            var skill = await _wikiService.GetSkillById(id);
            if (skill == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Skill with id {id} not found.", ErrorCode = ErrorCodes.NotFound });

            return Ok(new ApiResponse<SkillResponseDto> { Success = true, Data = skill });
        }
    }
}
