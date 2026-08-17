using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Mystic_Journey_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // Executes controller base operation.
    [Authorize]
    public class FriendController : ControllerBase
    {
        private readonly IFriendService _friendService;

        // Initializes a new instance of FriendController with dependencies: friendService.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public FriendController(IFriendService friendService)
        {
            _friendService = friendService;
        }

        // Executes get player profile id operation.
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

        // ─── Player APIs ───────────────────────────────────────────────────────
        [HttpGet]
        // Retrieves full friend list with current online presence and avatar info.
        public async Task<IActionResult> GetFriendList()
        {
            var playerId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
            var friends = await _friendService.GetFriendList(playerId); // Load accepted friends and their active session statuses
            return Ok(friends);
        }

        [HttpGet("requests")]
        // Retrieves pending incoming and outgoing friend requests.
        public async Task<IActionResult> GetFriendRequests()
        {
            var playerId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
            var requests = await _friendService.GetFriendRequests(playerId); // Query pending friend invitation records
            return Ok(requests);
        }

        [HttpGet("blocks")]
        // Retrieves list of currently blocked player profiles.
        public async Task<IActionResult> GetFriendBlocks()
        {
            var playerId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
            var blocks = await _friendService.GetFriendBlocks(playerId); // Query blocked player entries
            return Ok(blocks);
        }

        [HttpGet("profile/{id}")]
        // Retrieves public character profile, gear showcase, and stats of another player.
        public async Task<IActionResult> GetFriendProfile(int id)
        {
            var profile = await _friendService.GetFriendProfile(id); // Fetch target player's public showcase card
            if (profile == null) return NotFound(new { message = "Profile not found" });
            return Ok(profile);
        }

        [HttpGet("search")]
        // Searches players by display name keyword with existing friendship status annotations.
        public async Task<IActionResult> SearchPlayers([FromQuery] string? keyword)
        {
            var playerId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
            var results = await _friendService.SearchPlayers(playerId, keyword); // Search player profiles matching keyword prefix
            return Ok(results);
        }

        public class FriendRequestPayload
        {
            public int TargetProfileId { get; set; }
        }

        [HttpPost("request")]
        // Sends a friend invitation to target player profile.
        public async Task<IActionResult> SendFriendRequest([FromBody] FriendRequestPayload payload)
        {
            try
            {
                var playerId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
                await _friendService.SendFriendRequest(playerId, payload.TargetProfileId); // Verify blocklist and create pending friend request
                return Ok(new { message = "Friend request sent" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("accept/{requesterId}")]
        // Accepts a pending incoming friend request and establishes bidirectional friendship.
        public async Task<IActionResult> AcceptFriendRequest(int requesterId)
        {
            try
            {
                var playerId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
                await _friendService.AcceptFriendRequest(playerId, requesterId); // Transition request status to Accepted and increment friend counters
                return Ok(new { message = "Friend request accepted" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("decline/{requesterId}")]
        // Declines an incoming friend request.
        public async Task<IActionResult> DeclineFriendRequest(int requesterId)
        {
            try
            {
                var playerId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
                await _friendService.DeclineFriendRequest(playerId, requesterId); // Remove or reject pending request entry
                return Ok(new { message = "Friend request declined" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{targetId}")]
        // Unfriends a player and removes the bidirectional connection.
        public async Task<IActionResult> RemoveFriend(int targetId)
        {
            var playerId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
            await _friendService.RemoveFriend(playerId, targetId); // Delete friend connection row from database
            return Ok(new { message = "Friend removed" });
        }

        [HttpPost("block")]
        // Blocks another player from sending messages, invitations, or friend requests.
        public async Task<IActionResult> BlockPlayer([FromBody] FriendRequestPayload payload)
        {
            try
            {
                var playerId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
                await _friendService.BlockPlayer(playerId, payload.TargetProfileId); // Sever any existing friendship and insert block entry
                return Ok(new { message = "Player blocked" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("block/{targetId}")]
        // Unblocks a previously blocked player profile.
        public async Task<IActionResult> UnblockPlayer(int targetId)
        {
            try
            {
                var playerId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
                await _friendService.UnblockPlayer(playerId, targetId); // Delete blocklist record
                return Ok(new { message = "Player unblocked" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
