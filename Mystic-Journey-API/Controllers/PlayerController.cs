using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/player")]
    [ApiController]
    [Authorize]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerHeartbeatService _heartbeatService;

        public PlayerController(IPlayerHeartbeatService heartbeatService)
        {
            _heartbeatService = heartbeatService;
        }

        [HttpPost("heartbeat")]
        public async Task<IActionResult> Heartbeat()
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out int accountId))
            {
                return Unauthorized(new { message = "Invalid account ID" });
            }

            await _heartbeatService.UpdateLastSeenAsync(accountId);
            return Ok(new { success = true });
        }
    }
}
