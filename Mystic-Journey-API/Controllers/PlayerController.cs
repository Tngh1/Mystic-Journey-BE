using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Mystic_Journey_API.Controllers
{
    // Executes controller base operation.
    [Route("api/player")]
    [ApiController]
    [Authorize]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerHeartbeatService _heartbeatService;

        // Initializes a new instance of PlayerController with dependencies: heartbeatService.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public PlayerController(IPlayerHeartbeatService heartbeatService)
        {
            _heartbeatService = heartbeatService;
        }

        // ─── Player APIs ───────────────────────────────────────────────────────
        [HttpPost("heartbeat")]
        // Executes heartbeat operation.
        // Validates input parameters against null or empty values.
        public async Task<IActionResult> Heartbeat()
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Extract caller account ID string from JWT NameIdentifier claim
            if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out int accountId)) // Reject if claim missing or malformed
            {
                return Unauthorized(new { message = "Invalid account ID" }); // Return HTTP 401 Unauthorized
            }

            await _heartbeatService.UpdateLastSeenAsync(accountId); // Update last-seen timestamp and sync online presence in cache/database
            return Ok(new { success = true }); // Acknowledge heartbeat with success status
        }
    }
}
