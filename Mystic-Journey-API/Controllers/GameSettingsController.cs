using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameSettingsController : ControllerBase
    {
        private readonly IGameSettingService _gameSettingService;

        public GameSettingsController(IGameSettingService gameSettingService)
        {
            _gameSettingService = gameSettingService;
        }

        // ========== PLAYER: View Game Setting ==========
        // Dành cho người chơi - Xem cài đặt game

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var setting = await _gameSettingService.GetSettingById(id);
                if (setting == null)
                    return NotFound(new { message = $"Game setting with id {id} not found." });

                return Ok(setting);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("key/{key}")]
        public async Task<IActionResult> GetByKey(string key)
        {
            try
            {
                var setting = await _gameSettingService.GetSettingByKey(key);
                if (setting == null)
                    return NotFound(new { message = $"Game setting with key '{key}' not found." });

                return Ok(setting);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // ========== MANAGER: Game Setting Management (Dashboard) ==========
        // Dành cho Admin/Manager - Quản lý cài đặt game trên dashboard

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("key/{key}")]
        public async Task<IActionResult> Update(string key, [FromBody] UpdateGameSettingRequestDto request)
        {
            try
            {
                var setting = await _gameSettingService.UpdateSetting(key, request);
                return Ok(setting);
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

        // ========== PLAYER: Browse Game Settings ==========
        // Dành cho người chơi - Xem danh sách cài đặt game

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
        {
            var result = await _gameSettingService.GetSettingsPaged(page, pageSize, search);
            return Ok(result);
        }
    }
}
