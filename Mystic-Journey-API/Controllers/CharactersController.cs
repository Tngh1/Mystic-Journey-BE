using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace Mystic_Journey_API.Controllers
{
    // Quản lý nhân vật (character) của người chơi.
    // Cho phép tạo, xem chỉ số, cập nhật HP và nâng cấp thuộc tính.
    [Route("api/characters")]
    [ApiController]
    [Authorize]
    public class CharactersController : ControllerBase
    {
        private readonly ICharacterService _characterService;

        public CharactersController(ICharacterService characterService)
        {
            _characterService = characterService;
        }

        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // NOTE: GET /api/characters/class-configs đã chuyển sang
        // WikiController (/api/wiki/classes) — đó là dữ liệu codex công khai,
        // không phải nhân vật của người chơi, nên không thuộc controller này.

        // ── POST /api/characters ────────────────────────────────────
        // Tạo nhân vật mới cho tài khoản.
        // Thiết lập display name và class cho player mới đăng ký.
        // Chỉ được gọi một lần cho mỗi tài khoản.
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCharacterRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var profileId = GetPlayerProfileId();
                var result = await _characterService.CreateCharacter(profileId, request);

                return Ok(new ApiResponse<CharacterResponseDto>
                {
                    Success = true,
                    Message = "Character created successfully.",
                    Data = result
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorCode = "CHARACTER_ALREADY_CREATED", Message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object> { Success = false, ErrorCode = "PROFILE_NOT_FOUND", Message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorCode = "INVALID_CLASS", Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse<object> { Success = false, ErrorCode = "UNAUTHORIZED", Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object> { Success = false, ErrorCode = "INTERNAL_ERROR", Message = ex.Message });
            }
        }

        // ── GET /api/characters/stats ─────────────────────────────
        // Lấy danh sách chỉ số của nhân vật.
        // Bao gồm: CurrentHp, MaxHp, Atk, Def, MoveSpeed, AttackSpeed, CritRate, CritDamage,
        // DamageBonus, SkillPoints, TotalWins, TotalLosses, TotalKills, TotalDeaths.
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            try
            {
                var profileId = GetPlayerProfileId();
                var result = await _characterService.GetStats(profileId);

                return Ok(new ApiResponse<PlayerStatsResponseDto>
                {
                    Success = true,
                    Data = result
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object> { Success = false, ErrorCode = "STATS_NOT_FOUND", Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse<object> { Success = false, ErrorCode = "UNAUTHORIZED", Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object> { Success = false, ErrorCode = "INTERNAL_ERROR", Message = ex.Message });
            }
        }

        // ── PUT /api/characters/hp ────────────────────────────────
        // Cập nhật HP hiện tại của nhân vật (đồng bộ từ client).
        [HttpPut("hp")]
        public async Task<IActionResult> UpdateHp([FromBody] UpdateHpRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var profileId = GetPlayerProfileId();
                await _characterService.UpdateHp(profileId, request.CurrentHp);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "HP updated successfully."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object> { Success = false, ErrorCode = "PROFILE_NOT_FOUND", Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object> { Success = false, ErrorCode = "INTERNAL_ERROR", Message = ex.Message });
            }
        }

        // ── POST /api/characters/upgrade ──────────────────────────
        // Nâng cấp thuộc tính nhân vật bằng Skill Points.
        // Body: attributeName (Atk|Def|MaxHp|MoveSpeed|AttackSpeed|CritRate|CritDamage|DamageBonus), amount.
        // Skill Points được cấp tự động khi lên level (3 điểm mỗi level).
        [HttpPost("upgrade")]
        public async Task<IActionResult> Upgrade([FromBody] UpgradeAttributeRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var profileId = GetPlayerProfileId();
                var result = await _characterService.UpgradeAttribute(profileId, request);

                return Ok(new ApiResponse<UpgradeAttributeResponseDto>
                {
                    Success = true,
                    Message = $"{request.AttributeName} increased by {request.Amount}.",
                    Data = result
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorCode = "INSUFFICIENT_SKILL_POINTS", Message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object> { Success = false, ErrorCode = "STATS_NOT_FOUND", Message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorCode = "INVALID_ATTRIBUTE", Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse<object> { Success = false, ErrorCode = "UNAUTHORIZED", Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object> { Success = false, ErrorCode = "INTERNAL_ERROR", Message = ex.Message });
            }
        }

        // ── POST /api/characters/buffs ────────────────────────────
        [HttpPost("buffs")]
        [Authorize]
        public async Task<IActionResult> SyncBuffs([FromBody] UpdatePlayerBuffsRequest request)
        {
            try
            {
                var playerProfileId = GetPlayerProfileId();
                await _characterService.SyncBuffs(playerProfileId, request);
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Buffs synchronized successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object> { Success = false, ErrorCode = "INTERNAL_ERROR", Message = ex.Message });
            }
        }

        [HttpGet("level-up-options")]
        public async Task<IActionResult> GetLevelUpOptions()
        {
            try
            {
                var profileId = GetPlayerProfileId();
                var options = await _characterService.GetLevelUpOptions(profileId);
                return Ok(new ApiResponse<List<string>>
                {
                    Success = true,
                    Message = "Level up options generated.",
                    Data = options
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorCode = "NO_STAT_POINTS", Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object> { Success = false, ErrorCode = "INTERNAL_ERROR", Message = ex.Message });
            }
        }

        [HttpPost("allocate-stat")]
        public async Task<IActionResult> AllocateStat([FromBody] AllocateStatRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var profileId = GetPlayerProfileId();
                var result = await _characterService.AllocateStat(profileId, request.StatName);
                return Ok(new ApiResponse<PlayerStatsResponseDto>
                {
                    Success = true,
                    Message = "Stat allocated successfully.",
                    Data = result
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorCode = "INVALID_ALLOCATION", Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object> { Success = false, ErrorCode = "INTERNAL_ERROR", Message = ex.Message });
            }
        }

        // ── Helper ─────────────────────────────────────────────────
        // Đọc playerProfileId từ JWT token.
        private int GetPlayerProfileId()
        {
            var claim = User.FindFirstValue("playerProfileId");
            if (!int.TryParse(claim, out var id))
                throw new UnauthorizedAccessException("PlayerProfileId is missing from token. Please login again.");
            return id;
        }
    }
}
