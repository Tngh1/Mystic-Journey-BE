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
    // Executes controller base operation.
    [Route("api/characters")]
    [ApiController]
    [Authorize]
    public class CharactersController : ControllerBase
    {
        private readonly ICharacterService _characterService;

        // Initializes a new instance of CharactersController with dependencies: characterService.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public CharactersController(ICharacterService characterService)
        {
            _characterService = characterService;
        }



        // ─── Player APIs ───────────────────────────────────────────────────────
        [HttpPost]
        // Creates the initial character (Knight, Archer, Mage) for a freshly registered player profile.
        public async Task<IActionResult> Create([FromBody] CreateCharacterRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid) // Validate character creation payload (class name, starter preferences)
                    return BadRequest(ModelState);

                var profileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
                var result = await _characterService.CreateCharacter(profileId, request); // Initialize base stats, class traits, and starter inventory

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

        [HttpGet("stats")]
        // Load the player's base stats with buffs and achievements, apply the saved equipment snapshot and achievement bonuses, then return the effective stat response.
        public async Task<IActionResult> GetStats()
        {
            try
            {
                var profileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
                var result = await _characterService.GetStats(profileId); // Compute effective stats (base + equipment snapshot + buffs + passive bonuses)

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

        [HttpPut("hp")]
        // Persists real-time current health from the game client to the database.
        public async Task<IActionResult> UpdateHp([FromBody] UpdateHpRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid) // Validate HP value bounds
                    return BadRequest(ModelState);

                var profileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
                await _characterService.UpdateHp(profileId, request.CurrentHp); // Clamp HP to [0, MaxHp] and persist to PlayerStats

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

        [HttpPost("upgrade")]
        // Allocates available skill/attribute points into character stats (e.g., ATK, DEF, HP).
        public async Task<IActionResult> Upgrade([FromBody] UpgradeAttributeRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid) // Validate attribute name and upgrade point amount
                    return BadRequest(ModelState);

                var profileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
                var result = await _characterService.UpgradeAttribute(profileId, request); // Verify unspent points, increment attribute, and deduct points

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

        [HttpPost("buffs")]
        [Authorize]
        // Replace the player's persisted buff rows with the supplied active buffs, save the new set, and return the recalculated effective stats.
        public async Task<IActionResult> SyncBuffs([FromBody] UpdatePlayerBuffsRequest request)
        {
            try
            {
                var playerProfileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
                await _characterService.SyncBuffs(playerProfileId, request); // Invalidate expired buffs and sync current active buff entries
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
        // Generates random bonus perk/stat upgrade choices upon player level up.
        public async Task<IActionResult> GetLevelUpOptions()
        {
            try
            {
                var profileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
                var options = await _characterService.GetLevelUpOptions(profileId); // Generate 3 random upgrade options for the player's class
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
        // Confirms player selection of a level-up stat choice and applies the bonus permanently.
        public async Task<IActionResult> AllocateStat([FromBody] AllocateStatRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid) // Validate stat allocation choice
                    return BadRequest(ModelState);

                var profileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
                var result = await _characterService.AllocateStat(profileId, request.StatName); // Apply stat increase, decrement pending level-up point count, and save
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

        // Executes get player profile id operation.
        private int GetPlayerProfileId()
        {
            var claim = User.FindFirstValue("playerProfileId");
            if (!int.TryParse(claim, out var id))  // Claim value missing or non-integer — reject as unauthorized
                throw new UnauthorizedAccessException("PlayerProfileId is missing from token. Please login again.");  // Authentication token is invalid or expired
            return id;
        }
    }
}
