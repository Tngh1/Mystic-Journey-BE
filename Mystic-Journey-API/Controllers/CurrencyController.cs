using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Security.Claims;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/currencies")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;

        public CurrencyController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        [Authorize]
        [HttpGet("me/balance")]
        public async Task<IActionResult> GetBalance()
        {
            var playerProfileId = GetCurrentPlayerProfileId();
            var result = await _currencyService.GetBalance(playerProfileId);

            return Ok(new ApiResponse<CurrencyBalanceResponseDto>
            {
                Success = true,
                Data = result
            });
        }

        [Authorize]
        [HttpPost("spend")]
        public async Task<IActionResult> SpendCurrency([FromBody] SpendCurrencyRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    ErrorCode = ErrorCodes.ValidationError
                });
            }

            var playerProfileId = GetCurrentPlayerProfileId();
            var result = await _currencyService.SpendCurrency(playerProfileId, request);

            return Ok(new ApiResponse<CurrencySpendResponseDto>
            {
                Success = true,
                Message = result.Message,
                Data = result
            });
        }

        private int GetCurrentPlayerProfileId()
        {
            var claim = User.FindFirstValue("playerProfileId");
            if (!int.TryParse(claim, out var profileId))
                throw new UnauthorizedAccessException("PlayerProfileId is missing from token. Please login again.");

            return profileId;
        }
    }
}
