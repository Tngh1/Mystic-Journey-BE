using System;
using System.Linq;
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
        [HttpGet("me")]
        public async Task<IActionResult> GetInventory()
        {
            try
            {
                var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(claim, out var accountId))
                    return Unauthorized(new { error = "UNAUTHORIZED", message = "Invalid authentication token." });

                var profile = await _playerProfileRepository.GetByAccountId(accountId);
                if (profile == null)
                    return NotFound(new { error = "PROFILE_NOT_FOUND", message = "Player profile not found." });

                var result = await _inventoryService.GetInventory(profile.PlayerProfileId);
                return Ok(new ApiResponse<InventorySummaryDto> { Success = true, Data = result });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Error = "INTERNAL_ERROR", Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("equip-item")]
        public async Task<IActionResult> EquipItem([FromBody] EquipItemRequestDto request)
        {
            try
            {
                var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(claim, out var accountId))
                    return Unauthorized(new ErrorResponse { Error = "UNAUTHORIZED", Message = "Invalid authentication token." });

                var profile = await _playerProfileRepository.GetByAccountId(accountId);
                if (profile == null)
                    return NotFound(new ErrorResponse { Error = "PROFILE_NOT_FOUND", Message = "Player profile not found." });

                var updated = await _inventoryService.EquipItem(profile.PlayerProfileId, request);

                // fetch fresh profile with PlayerStats snapshot
                var snapshot = await _playerProfileRepository.GetSnapshotByPlayerProfileId(profile.PlayerProfileId);
                PlayerStatsResponseDto? playerStats = null;
                if (snapshot != null)
                {
                    playerStats = new PlayerStatsResponseDto
                    {
                        CurrentHp = snapshot.MaxHp, // snapshot does not store runtime current HP
                        MaxHp = snapshot.MaxHp,
                        Atk = snapshot.Atk,
                        Def = snapshot.Def,
                        MoveSpeed = StatHelper.FromScaled(snapshot.MoveSpeed, StatScale.MoveSpeed),
                        AttackSpeed = StatHelper.FromScaled(snapshot.AttackSpeed, StatScale.AttackSpeed),
                        CritRate = StatHelper.FromScaled(snapshot.CritRate, StatScale.CritRate),
                        CritDamage = StatHelper.FromScaled(snapshot.CritDamage, StatScale.CritRate),
                        DamageBonus = StatHelper.FromScaled(snapshot.DamageBonus, StatScale.DamageBonus),
                        SkillPoints = 0,
                        TotalWins = 0,
                        TotalLosses = 0,
                        TotalKills = 0,
                        TotalDeaths = 0
                    };
                }

                var actionResult = new InventoryActionResultDto { Item = updated, PlayerStats = playerStats };
                return Ok(new ApiResponse<InventoryActionResultDto> { Success = true, Message = "Item equipped.", Data = actionResult });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ErrorResponse { Error = "ITEM_NOT_FOUND", Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ErrorResponse { Error = "UNAUTHORIZED", Message = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Error = "INTERNAL_ERROR", Message = ex.Message });
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
                    return Unauthorized(new ErrorResponse { Error = "UNAUTHORIZED", Message = "Invalid authentication token." });

                var profile = await _playerProfileRepository.GetByAccountId(accountId);
                if (profile == null)
                    return NotFound(new ErrorResponse { Error = "PROFILE_NOT_FOUND", Message = "Player profile not found." });

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
                        MoveSpeed = StatHelper.FromScaled(snapshot.MoveSpeed, StatScale.MoveSpeed),
                        AttackSpeed = StatHelper.FromScaled(snapshot.AttackSpeed, StatScale.AttackSpeed),
                        CritRate = StatHelper.FromScaled(snapshot.CritRate, StatScale.CritRate),
                        CritDamage = StatHelper.FromScaled(snapshot.CritDamage, StatScale.CritRate),
                        DamageBonus = StatHelper.FromScaled(snapshot.DamageBonus, StatScale.DamageBonus),
                        SkillPoints = 0,
                        TotalWins = 0,
                        TotalLosses = 0,
                        TotalKills = 0,
                        TotalDeaths = 0
                    };
                }

                var actionResult = new InventoryActionResultDto { Item = updated, PlayerStats = playerStats };
                return Ok(new ApiResponse<InventoryActionResultDto> { Success = true, Message = "Item unequipped.", Data = actionResult });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ErrorResponse { Error = "ITEM_NOT_FOUND", Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ErrorResponse { Error = "UNAUTHORIZED", Message = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Error = "INTERNAL_ERROR", Message = ex.Message });
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
                    return Unauthorized(new ErrorResponse { Error = "UNAUTHORIZED", Message = "Invalid authentication token." });

                var profile = await _playerProfileRepository.GetByAccountId(accountId);
                if (profile == null)
                    return NotFound(new ErrorResponse { Error = "PROFILE_NOT_FOUND", Message = "Player profile not found." });

                await _inventoryService.ConsumeItem(profile.PlayerProfileId, request);
                return Ok(new ApiResponse<object> { Success = true, Message = "Item consumed." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ErrorResponse { Error = "ITEM_NOT_FOUND", Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ErrorResponse { Error = "UNAUTHORIZED", Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErrorResponse { Error = "INVALID_OPERATION", Message = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Error = "INTERNAL_ERROR", Message = ex.Message });
            }
        }
    }
}
