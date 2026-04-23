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
    public class FriendsController : ControllerBase
    {
        private readonly IFriendService _friendService;

        public FriendsController(IFriendService friendService)
        {
            _friendService = friendService;
        }

        private Guid GetAccountId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        [HttpGet]
        public async Task<IActionResult> GetFriends()
        {
            var accountId = GetAccountId();
            var result = await _friendService.GetFriendsAsync(accountId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingRequests()
        {
            var accountId = GetAccountId();
            var result = await _friendService.GetPendingRequestsAsync(accountId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("sent")]
        public async Task<IActionResult> GetSentRequests()
        {
            var accountId = GetAccountId();
            var result = await _friendService.GetSentRequestsAsync(accountId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendFriendRequest([FromBody] SendFriendRequestDto request)
        {
            var accountId = GetAccountId();
            var result = await _friendService.SendFriendRequestAsync(accountId, request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("respond")]
        public async Task<IActionResult> RespondToRequest([FromBody] RespondFriendRequestDto request)
        {
            var accountId = GetAccountId();
            var result = await _friendService.RespondToRequestAsync(accountId, request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpDelete("{friendId}")]
        public async Task<IActionResult> RemoveFriend(Guid friendId)
        {
            var accountId = GetAccountId();
            var result = await _friendService.RemoveFriendAsync(accountId, friendId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("block/{playerId}")]
        public async Task<IActionResult> BlockPlayer(Guid playerId)
        {
            var accountId = GetAccountId();
            var result = await _friendService.BlockPlayerAsync(accountId, playerId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
