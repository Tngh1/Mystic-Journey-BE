using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PlayerProfilesController : ControllerBase
    {
        private readonly IPlayerProfileService _playerProfileService;

        public PlayerProfilesController(IPlayerProfileService playerProfileService)
        {
            _playerProfileService = playerProfileService;
        }

        private Guid GetAccountId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateProfile([FromBody] CreatePlayerProfileRequestDto request)
        {
            var accountId = GetAccountId();
            var result = await _playerProfileService.CreateProfileAsync(accountId, request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var accountId = GetAccountId();
            var result = await _playerProfileService.GetProfileByAccountIdAsync(accountId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("details")]
        public async Task<IActionResult> GetProfileDetails()
        {
            var accountId = GetAccountId();
            var result = await _playerProfileService.GetProfileDetailByAccountIdAsync(accountId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdatePlayerProfileRequestDto request)
        {
            var accountId = GetAccountId();
            var result = await _playerProfileService.UpdateProfileAsync(accountId, request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var accountId = GetAccountId();
            var stats = await _playerProfileService.GetPlayerStatsAsync(accountId);

            if (stats == null)
            {
                return NotFound(new { Success = false, Message = "Profile or stats not found." });
            }

            return Ok(new { Success = true, Message = "Stats retrieved successfully.", Data = stats });
        }

        [HttpGet("currency")]
        public async Task<IActionResult> GetCurrency()
        {
            var accountId = GetAccountId();
            var result = await _playerProfileService.GetCurrencyAsync(accountId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost("currency/add")]
        public async Task<IActionResult> AddCurrency([FromBody] CurrencyUpdateDto request)
        {
            var accountId = GetAccountId();
            var result = await _playerProfileService.AddCurrencyAsync(accountId, request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("currency/spend")]
        public async Task<IActionResult> SpendCurrency([FromBody] CurrencyUpdateDto request)
        {
            var accountId = GetAccountId();
            var result = await _playerProfileService.SpendCurrencyAsync(accountId, request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("energy/{change}")]
        public async Task<IActionResult> UpdateEnergy(int change)
        {
            var accountId = GetAccountId();
            var result = await _playerProfileService.UpdateEnergyAsync(accountId, change);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("experience/{amount}")]
        public async Task<IActionResult> AddExperience(int amount)
        {
            var accountId = GetAccountId();
            var result = await _playerProfileService.AddExperienceAsync(accountId, amount);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("exists")]
        [AllowAnonymous]
        public async Task<IActionResult> HasProfile(Guid accountId)
        {
            var hasProfile = await _playerProfileService.HasProfileAsync(accountId);
            return Ok(new { Success = true, HasProfile = hasProfile });
        }
    }
}
