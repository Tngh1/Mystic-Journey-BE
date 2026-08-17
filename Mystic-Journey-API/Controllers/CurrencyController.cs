using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Security.Claims;

namespace Mystic_Journey_API.Controllers
{
    // Executes controller base operation.
    [Route("api/currencies")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;

        // Initializes a new instance of CurrencyController with dependencies: currencyService.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public CurrencyController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        // ─── Player APIs ───────────────────────────────────────────────────────
        [Authorize]
        [HttpGet("me/balance")]
        // Retrieves current player wallet balances (Gold, Gems, Stamina/Energy, Arena Tokens).
        public async Task<IActionResult> GetBalance()
        {
            var playerProfileId = GetCurrentPlayerProfileId(); // Extract caller's profile ID from JWT claim
            var result = await _currencyService.GetBalance(playerProfileId); // Query cached or database balance amounts

            return Ok(new ApiResponse<CurrencyBalanceResponseDto>
            {
                Success = true,
                Data = result
            });
        }

        [Authorize]
        [HttpPost("spend")]
        // Validates sufficient balance and deducts currencies for in-game purchases/actions.
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

            var playerProfileId = GetCurrentPlayerProfileId(); // Extract caller's profile ID from JWT claim
            var result = await _currencyService.SpendCurrency(playerProfileId, request); // Perform atomic balance deduction and record transaction log

            return Ok(new ApiResponse<CurrencySpendResponseDto>
            {
                Success = true,
                Message = result.Message,
                Data = result
            });
        }

        // Extracts caller's player profile ID integer from the JWT claim.
        private int GetCurrentPlayerProfileId()
        {
            var claim = User.FindFirstValue("playerProfileId");
            if (!int.TryParse(claim, out var profileId))
                throw new UnauthorizedAccessException("PlayerProfileId is missing from token. Please login again.");

            return profileId;
        }
    }
}
