using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerProfilesController : ControllerBase
    {
        private readonly IPlayerProfileService _playerProfileService;
        private readonly IAuthRepository _authRepository;

        public PlayerProfilesController(
            IPlayerProfileService playerProfileService,
            IAuthRepository authRepository)
        {
            _playerProfileService = playerProfileService;
            _authRepository = authRepository;
        }

        // ========== SHARED: Helper Methods ==========

        private int GetCurrentAccountId()
        {
            var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }

        private async Task<int> GetCurrentPlayerProfileId()
        {
            var accountId = GetCurrentAccountId();
            var account = await _authRepository.GetAccountById(accountId);
            return account?.PlayerProfile?.PlayerProfileId ?? 0;
        }

        // ========== PLAYER: View Profile ==========
        // Dành cho người chơi - Xem profile (của mình hoặc người khác)

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _playerProfileService.GetProfileById(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // ========== PLAYER: Update Own Profile ==========
        // Dành cho người chơi - Cập nhật profile của mình

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePlayerProfileRequestDto dto)
        {
            try
            {
                var result = await _playerProfileService.UpdateProfile(id, dto);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // ========== MANAGER: Player Profile Management (Dashboard) ==========
        // Dành cho Admin/Manager - Quản lý danh sách player profiles trên dashboard

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] int? level = null)
        {
            var result = await _playerProfileService.GetProfilesPaged(page, pageSize, search, level);
            return Ok(result);
        }

        // ========== PLAYER: Player's Own Data (/me endpoints) ==========
        // Dành cho người chơi - Lấy dữ liệu của chính mình

        [Authorize]
        [HttpGet("me/inventory")]
        public async Task<IActionResult> GetMyInventory()
        {
            try
            {
                var playerProfileId = await GetCurrentPlayerProfileId();
                if (playerProfileId == 0)
                    return Unauthorized(new { message = "Player profile not found." });

                var result = await _playerProfileService.GetMeInventory(playerProfileId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("me/skills")]
        public async Task<IActionResult> GetMySkills()
        {
            try
            {
                var playerProfileId = await GetCurrentPlayerProfileId();
                if (playerProfileId == 0)
                    return Unauthorized(new { message = "Player profile not found." });

                var result = await _playerProfileService.GetMeSkills(playerProfileId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("me/quests")]
        public async Task<IActionResult> GetMyQuests()
        {
            try
            {
                var playerProfileId = await GetCurrentPlayerProfileId();
                if (playerProfileId == 0)
                    return Unauthorized(new { message = "Player profile not found." });

                var result = await _playerProfileService.GetMeQuests(playerProfileId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("me/achievements")]
        public async Task<IActionResult> GetMyAchievements()
        {
            try
            {
                var playerProfileId = await GetCurrentPlayerProfileId();
                if (playerProfileId == 0)
                    return Unauthorized(new { message = "Player profile not found." });

                var result = await _playerProfileService.GetMeAchievements(playerProfileId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
