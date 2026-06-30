using BLL.DTOs;
using BLL.Services.Interfaces;
using BLL.Utils;
using DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    // Quản lý inventory (hành trang) của người chơi.
    // Cho phép xem, trang bị, gỡ trang bị, và sử dụng item.
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly IPlayerProfileRepository _playerProfileRepository;

        public InventoryController(
            IInventoryService inventoryService,
            IPlayerProfileRepository playerProfileRepository)
        {
            _inventoryService = inventoryService;
            _playerProfileRepository = playerProfileRepository;
        }

        private int GetPlayerProfileId()
        {
            var claim = User.FindFirstValue("playerProfileId");
            if (!int.TryParse(claim, out var id))
                throw new UnauthorizedAccessException("PlayerProfileId is missing from token. Please login again.");
            return id;
        }

        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // ── GET /api/inventory/me ─────────────────────────────────────────────
        // Lấy tóm tắt inventory của player (số lượng items, skins, dung lượng túi).
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyInventory()
        {
            var profileId = GetPlayerProfileId();
            var inventory = await _inventoryService.GetInventory(profileId);
            return Ok(new ApiResponse<InventorySummaryDto> { Success = true, Data = inventory });
        }

        // ── GET /api/inventory/me/full ─────────────────────────────────────────
        // Lấy inventory đầy đủ của player với chi tiết từng item (ItemId, tên, slot...).
        [Authorize]
        [HttpGet("me/full")]
        public async Task<IActionResult> GetMyInventoryFull()
        {
            var profileId = GetPlayerProfileId();
            var result = await _inventoryService.GetMeInventory(profileId);
            return Ok(new ApiResponse<PlayerMeInventoryResponseDto> { Success = true, Data = result });
        }

        // ── POST /api/inventory/equip-item ─────────────────────────────────────
        // Trang bị item từ inventory vào slot tương ứng.
        // Trả về thông tin item đã trang bị và stats hiện tại của player.
        [Authorize]
        [HttpPost("equip-item")]
        public async Task<IActionResult> EquipItem([FromBody] EquipItemRequestDto request)
        {
            var profileId = GetPlayerProfileId();
            var updated = await _inventoryService.EquipItem(profileId, request);

            // Lấy stats hiện tại của player sau khi trang bị.
            var snapshot = await _playerProfileRepository.GetSnapshotByPlayerProfileId(profileId);
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

        // ── POST /api/inventory/unequip-item ────────────────────────────────────
        // Gỡ item đã trang bị và trả về inventory.
        // Trả về thông tin item và stats hiện tại của player.
        [Authorize]
        [HttpPost("unequip-item")]
        public async Task<IActionResult> UnequipItem([FromBody] UnequipItemRequestDto request)
        {
            var profileId = GetPlayerProfileId();
            var updated = await _inventoryService.UnequipItem(profileId, request);

            var snapshot = await _playerProfileRepository.GetSnapshotByPlayerProfileId(profileId);
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

        // ── POST /api/inventory/consume-item ────────────────────────────────────
        // Sử dụng item có thể tiêu thụ (consumable).
        // Giảm số lượng item trong inventory.
        [Authorize]
        [HttpPost("consume-item")]
        public async Task<IActionResult> ConsumeItem([FromBody] ConsumeItemRequestDto request)
        {
            var profileId = GetPlayerProfileId();
            await _inventoryService.ConsumeItem(profileId, request);
            return Ok(new ApiResponse<object> { Success = true, Message = "Item consumed successfully." });
        }
    }
}
