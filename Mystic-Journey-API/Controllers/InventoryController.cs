using System;
using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Security.Claims;
using DAL.Repositories.Interfaces;
using BLL.Utils;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly IPlayerProfileRepository _playerProfileRepository;

        public InventoryController(IInventoryService inventoryService, IPlayerProfileRepository playerProfileRepository)
        {
            _inventoryService = inventoryService;
            _playerProfileRepository = playerProfileRepository;
        }

        [Authorize]
        [HttpPost("equip-item")]
        public async Task<IActionResult> EquipItem([FromBody] EquipItemRequestDto request)
        {
            try
            {
                var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(claim, out var accountId))
                    return Unauthorized(new { message = "Invalid authentication token." });

                var profile = await _playerProfileRepository.GetByAccountId(accountId);
                if (profile == null)
                    return NotFound(new { message = "Player profile not found." });

                var updated = await _inventoryService.EquipItem(profile.PlayerProfileId, request);

                var snapshot = await _playerProfileRepository.GetSnapshotByPlayerProfileId(profile.PlayerProfileId);
                PlayerStatsResponseDto? playerStats = null;
                if (snapshot != null)
                {
                    playerStats = new PlayerStatsResponseDto
                    {
                        CurrentHp = snapshot.MaxHp,
                        MaxHp = snapshot.MaxHp,
                        Atk = snapshot.Atk,
                        Def = snapshot.Def,
                        MoveSpeed = (int)StatHelper.FromScaled(snapshot.MoveSpeed, StatScale.MoveSpeed),
                        AttackSpeed = (int)StatHelper.FromScaled(snapshot.AttackSpeed, StatScale.AttackSpeed),
                        CritRate = (int)StatHelper.FromScaled(snapshot.CritRate, StatScale.CritRate),
                        CritDamage = (int)StatHelper.FromScaled(snapshot.CritDamage, StatScale.CritRate),
                        DamageBonus = (int)StatHelper.FromScaled(snapshot.DamageBonus, StatScale.DamageBonus),
                        SkillPoints = 0,
                        TotalWins = 0,
                        TotalLosses = 0,
                        TotalKills = 0,
                        TotalDeaths = 0
                    };
                }

                var actionResult = new InventoryActionResultDto { Item = updated, PlayerStats = playerStats };
                return Ok(new ApiResponse<InventoryActionResultDto> { Data = actionResult });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("unequip-item")]
        public async Task<IActionResult> UnequipItem([FromBody] UnequipItemRequestDto request)
        {
            try
            {
                var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(claim, out var accountId))
                    return Unauthorized(new { message = "Invalid authentication token." });

                var profile = await _playerProfileRepository.GetByAccountId(accountId);
                if (profile == null)
                    return NotFound(new { message = "Player profile not found." });

                var updated = await _inventoryService.UnequipItem(profile.PlayerProfileId, request);

                var snapshot = await _playerProfileRepository.GetSnapshotByPlayerProfileId(profile.PlayerProfileId);
                PlayerStatsResponseDto? playerStats = null;
                if (snapshot != null)
                {
                    playerStats = new PlayerStatsResponseDto
                    {
                        CurrentHp = snapshot.MaxHp,
                        MaxHp = snapshot.MaxHp,
                        Atk = snapshot.Atk,
                        Def = snapshot.Def,
                        MoveSpeed = (int)StatHelper.FromScaled(snapshot.MoveSpeed, StatScale.MoveSpeed),
                        AttackSpeed = (int)StatHelper.FromScaled(snapshot.AttackSpeed, StatScale.AttackSpeed),
                        CritRate = (int)StatHelper.FromScaled(snapshot.CritRate, StatScale.CritRate),
                        CritDamage = (int)StatHelper.FromScaled(snapshot.CritDamage, StatScale.CritRate),
                        DamageBonus = (int)StatHelper.FromScaled(snapshot.DamageBonus, StatScale.DamageBonus),
                        SkillPoints = 0,
                        TotalWins = 0,
                        TotalLosses = 0,
                        TotalKills = 0,
                        TotalDeaths = 0
                    };
                }

                var actionResult = new InventoryActionResultDto { Item = updated, PlayerStats = playerStats };
                return Ok(new ApiResponse<InventoryActionResultDto> { Data = actionResult });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("consume-item")]
        public async Task<IActionResult> ConsumeItem([FromBody] ConsumeItemRequestDto request)
        {
            try
            {
                var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(claim, out var accountId))
                    return Unauthorized(new { message = "Invalid authentication token." });

                var profile = await _playerProfileRepository.GetByAccountId(accountId);
                if (profile == null)
                    return NotFound(new { message = "Player profile not found." });

                await _inventoryService.ConsumeItem(profile.PlayerProfileId, request);
                return Ok(new ApiResponse<object> { Data = null });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
