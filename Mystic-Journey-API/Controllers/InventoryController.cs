using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Security.Claims;
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
        public async Task<IActionResult> GetMyInventory()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(claim, out var accountId))
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Invalid authentication token.", ErrorCode = ErrorCodes.Unauthorized });

            var profile = await _playerProfileRepository.GetByAccountId(accountId);
            if (profile == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.NotFound });

            var inventory = await _inventoryService.GetInventory(profile.PlayerProfileId);
            return Ok(new ApiResponse<InventorySummaryDto> { Success = true, Data = inventory });
        }
        [Authorize]
        [HttpPost("equip-item")]
        public async Task<IActionResult> EquipItem([FromBody] EquipItemRequestDto request)
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(claim, out var accountId))
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Invalid authentication token.", ErrorCode = ErrorCodes.Unauthorized });

            var profile = await _playerProfileRepository.GetByAccountId(accountId);
            if (profile == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.NotFound });

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
            return Ok(new ApiResponse<InventoryActionResultDto> { Success = true, Data = actionResult });
        }

        [Authorize]
        [HttpPost("unequip-item")]
        public async Task<IActionResult> UnequipItem([FromBody] UnequipItemRequestDto request)
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(claim, out var accountId))
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Invalid authentication token.", ErrorCode = ErrorCodes.Unauthorized });

            var profile = await _playerProfileRepository.GetByAccountId(accountId);
            if (profile == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.NotFound });

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
            return Ok(new ApiResponse<InventoryActionResultDto> { Success = true, Data = actionResult });
        }

        [Authorize]
        [HttpPost("consume-item")]
        public async Task<IActionResult> ConsumeItem([FromBody] ConsumeItemRequestDto request)
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(claim, out var accountId))
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "Invalid authentication token.", ErrorCode = ErrorCodes.Unauthorized });

            var profile = await _playerProfileRepository.GetByAccountId(accountId);
            if (profile == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Player profile not found.", ErrorCode = ErrorCodes.NotFound });

            await _inventoryService.ConsumeItem(profile.PlayerProfileId, request);
            return Ok(new ApiResponse<object> { Success = true, Message = "Item consumed successfully." });
        }
    }
}
