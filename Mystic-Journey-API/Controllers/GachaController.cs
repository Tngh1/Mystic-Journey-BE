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
    public class GachaController : ControllerBase
    {
        private readonly IGachaService _gachaService;

        public GachaController(IGachaService gachaService)
        {
            _gachaService = gachaService;
        }

        private Guid GetAccountId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        [HttpGet("banners")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllBanners()
        {
            var result = await _gachaService.GetAllBannersAsync();
            return Ok(result);
        }

        [HttpGet("banners/available")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailableBanners()
        {
            var result = await _gachaService.GetAvailableBannersAsync();
            return Ok(result);
        }

        [HttpGet("banners/{bannerId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBannerById(Guid bannerId)
        {
            var result = await _gachaService.GetBannerByIdAsync(bannerId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost("pull")]
        [Authorize]
        public async Task<IActionResult> PullGacha([FromBody] GachaPullRequestDto request)
        {
            var accountId = GetAccountId();
            var result = await _gachaService.PullGachaAsync(accountId, request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("history")]
        [Authorize]
        public async Task<IActionResult> GetPullHistory([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var accountId = GetAccountId();
            var result = await _gachaService.GetPullHistoryAsync(accountId, pageNumber, pageSize);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}
