using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
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
            try
            {
                var dungeon = await _dungeonConfigService.GetDungeonById(id);
                if (dungeon == null)
                    return NotFound(new { message = $"Dungeon with id {id} not found." });

                return Ok(dungeon);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDungeonConfigRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var dungeon = await _dungeonConfigService.CreateDungeon(request);
                return Ok(dungeon);
            }
            catch (System.ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDungeonConfigRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var dungeon = await _dungeonConfigService.UpdateDungeon(id, request);
                return Ok(dungeon);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (System.ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("/odata/Dungeons")]
        [EnableQuery]
        public IActionResult GetOData()
        {
            return Ok(_dungeonConfigService.GetDungeonsQueryable());
        }
    }
}
