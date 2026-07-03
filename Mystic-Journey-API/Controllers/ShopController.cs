using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Security.Claims;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/shop")]
    [ApiController]
    public class ShopController : ControllerBase
    {
        private readonly IPlayerShopService _shopService;

        public ShopController(IPlayerShopService shopService)
        {
            _shopService = shopService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ViewShop([FromQuery] ViewShopQueryDto query)
            => await GetItems(query);

        [Authorize]
        [HttpGet("items")]
        public async Task<IActionResult> GetItems([FromQuery] ViewShopQueryDto query)
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
            var result = await _shopService.GetShop(playerProfileId, query);

            return Ok(new ApiResponse<PagedResultDto<ShopItemPublicResponseDto>>
            {
                Success = true,
                Data = result
            });
        }

        [Authorize]
        [HttpPost("purchase")]
        public async Task<IActionResult> PurchaseItem([FromBody] PurchaseShopItemRequestDto request)
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
            var result = await _shopService.PurchaseItem(playerProfileId, request);

            return Ok(new ApiResponse<PurchaseShopItemResponseDto>
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
