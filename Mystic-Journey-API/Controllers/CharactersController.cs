using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Mystic_Journey_API.Controllers
{
    /// <summary>
    /// Handles character lifecycle: creation, stat viewing, and attribute upgrades.
    ///
    /// NOTE — Save Character Position is handled by a dedicated endpoint:
    ///   PUT /api/world/position  (see WorldController)
    /// That endpoint already persists MapName, PositionX, and PositionY on the
    /// PlayerProfile row and is the canonical way to save in-game position.
    /// </summary>
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

        // ── POST /api/characters ─────────────────────────────────────────────────────
        /// <summary>
        /// Create Character — sets the display name and class for a newly registered
        /// player and seeds their base stats. Can only be called once per account.
        ///
        /// Requires: JWT access token.
        /// Body: { "characterName": "...", "selectedClass": "Knight|Archer|Mage" }
        /// </summary>
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

        // ── GET /api/characters/stats ─────────────────────────────────────────────────
        /// <summary>
        /// View Attribute List — returns all player stats for the authenticated player:
        /// CurrentHp, MaxHp, Atk, Def, MoveSpeed, AttackSpeed, CritRate, CritDamage,
        /// DamageBonus, SkillPoints, TotalWins, TotalLosses, TotalKills, TotalDeaths.
        ///
        /// Requires: JWT access token.
        /// </summary>
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

        // ── PUT /api/characters/hp ────────────────────────────────────────────────────
        /// <summary>
        /// Update HP — syncs the player's current health point from the client.
        /// 
        /// Requires: JWT access token.
        /// </summary>
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

        // ── POST /api/characters/upgrade ─────────────────────────────────────────────
        /// <summary>
        /// Upgrade Character — spends Skill Points to permanently increase one attribute.
        ///
        /// Requires: JWT access token.
        /// Body: { "attributeName": "Atk|Def|MaxHp|MoveSpeed|AttackSpeed|CritRate|CritDamage|DamageBonus",
        ///         "amount": 1 }
        ///
        /// Skill Points are granted automatically on level-up (3 points per level).
        /// </summary>
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

        // ── Helper ───────────────────────────────────────────────────────────────────
        /// <summary>
        /// Reads the playerProfileId custom claim embedded in the JWT by AccountService.
        /// Throws UnauthorizedAccessException if the claim is missing or invalid.
        /// </summary>
        private int GetPlayerProfileId()
        {
            var claim = User.FindFirstValue("playerProfileId");
            if (!int.TryParse(claim, out var id))
                throw new UnauthorizedAccessException("PlayerProfileId is missing from token. Please login again.");
            return id;
        }
    }
}
