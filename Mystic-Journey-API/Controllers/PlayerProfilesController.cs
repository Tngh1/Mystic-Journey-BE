using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProfileByIdAsync(int id)
        {
            var result = await _playerProfileService.GetProfileByIdAsync(id);

            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("account/{accountId}")]
        public async Task<IActionResult> GetProfileByAccountIdAsync(Guid accountId)
        {
            var result = await _playerProfileService.GetProfileByAccountIdAsync(accountId);

            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProfileAsync([FromBody] CreatePlayerProfileRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new PlayerProfileApiResponseDto
                {
                    Success = false,
                    Message = GetError()
                });
            }

            var result = await _playerProfileService.CreateProfileAsync(request);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProfileAsync(int id, [FromBody] UpdatePlayerProfileRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new PlayerProfileApiResponseDto
                {
                    Success = false,
                    Message = GetError()
                });
            }

            var result = await _playerProfileService.UpdateProfileAsync(id, request);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        private string GetError()
        {
            return ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .FirstOrDefault() ?? "Invalid request.";
        }
    }
}
