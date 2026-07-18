using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Mystic_Journey_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FriendController : ControllerBase
    {
        private readonly IFriendService _friendService;

        public FriendController(IFriendService friendService)
        {
            _friendService = friendService;
        }

        private int GetPlayerProfileId()
        {
            var profileClaim = User.FindFirst("playerProfileId")?.Value;
            if (int.TryParse(profileClaim, out var profileId) && profileId > 0)
            {
                return profileId;
            }

            var legacyClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(legacyClaim, out var legacyId) ? legacyId : 0;
        }

        [HttpGet]
        public async Task<IActionResult> GetFriendList()
        {
            var playerId = GetPlayerProfileId();
            var friends = await _friendService.GetFriendList(playerId);
            return Ok(friends);
        }

        [HttpGet("requests")]
        public async Task<IActionResult> GetFriendRequests()
        {
            var playerId = GetPlayerProfileId();
            var requests = await _friendService.GetFriendRequests(playerId);
            return Ok(requests);
        }

        [HttpGet("blocks")]
        public async Task<IActionResult> GetFriendBlocks()
        {
            var playerId = GetPlayerProfileId();
            var blocks = await _friendService.GetFriendBlocks(playerId);
            return Ok(blocks);
        }

        [HttpGet("profile/{id}")]
        public async Task<IActionResult> GetFriendProfile(int id)
        {
            var profile = await _friendService.GetFriendProfile(id);
            if (profile == null) return NotFound(new { message = "Profile not found" });
            return Ok(profile);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchPlayers([FromQuery] string? keyword)
        {
            var playerId = GetPlayerProfileId();
            var results = await _friendService.SearchPlayers(playerId, keyword);
            return Ok(results);
        }

        public class FriendRequestPayload
        {
            public int TargetProfileId { get; set; }
        }

        [HttpPost("request")]
        public async Task<IActionResult> SendFriendRequest([FromBody] FriendRequestPayload payload)
        {
            try
            {
                var playerId = GetPlayerProfileId();
                await _friendService.SendFriendRequest(playerId, payload.TargetProfileId);
                return Ok(new { message = "Friend request sent" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("accept/{requesterId}")]
        public async Task<IActionResult> AcceptFriendRequest(int requesterId)
        {
            try
            {
                var playerId = GetPlayerProfileId();
                await _friendService.AcceptFriendRequest(playerId, requesterId);
                return Ok(new { message = "Friend request accepted" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("decline/{requesterId}")]
        public async Task<IActionResult> DeclineFriendRequest(int requesterId)
        {
            try
            {
                var playerId = GetPlayerProfileId();
                await _friendService.DeclineFriendRequest(playerId, requesterId);
                return Ok(new { message = "Friend request declined" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{targetId}")]
        public async Task<IActionResult> RemoveFriend(int targetId)
        {
            var playerId = GetPlayerProfileId();
            await _friendService.RemoveFriend(playerId, targetId);
            return Ok(new { message = "Friend removed" });
        }

        [HttpPost("block")]
        public async Task<IActionResult> BlockPlayer([FromBody] FriendRequestPayload payload)
        {
            try
            {
                var playerId = GetPlayerProfileId();
                await _friendService.BlockPlayer(playerId, payload.TargetProfileId);
                return Ok(new { message = "Player blocked" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("block/{targetId}")]
        public async Task<IActionResult> UnblockPlayer(int targetId)
        {
            try
            {
                var playerId = GetPlayerProfileId();
                await _friendService.UnblockPlayer(playerId, targetId);
                return Ok(new { message = "Player unblocked" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
