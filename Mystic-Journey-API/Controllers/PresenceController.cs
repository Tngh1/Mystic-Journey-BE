using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PresenceController : ControllerBase
    {
        private readonly IPlayerPresenceService _presenceService;

        public PresenceController(IPlayerPresenceService presenceService)
        {
            _presenceService = presenceService;
        }

        [HttpPost("heartbeat")]
        public IActionResult Heartbeat()
        {
            var playerIdClaim = User.FindFirst("PlayerProfileId")?.Value;
            if (string.IsNullOrEmpty(playerIdClaim) || !int.TryParse(playerIdClaim, out int playerId))
            {
                return Unauthorized(new { message = "Invalid player profile ID" });
            }

            _presenceService.UpdatePresence(playerId);
            return Ok(new { success = true });
        }
    }
}
