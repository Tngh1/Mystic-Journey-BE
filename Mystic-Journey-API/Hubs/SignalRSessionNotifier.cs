using BLL.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Mystic_Journey_API.Hubs
{
    public class SignalRSessionNotifier : ISessionNotifier
    {
        private readonly IHubContext<GameHub> _hub;
        private readonly ILogger<SignalRSessionNotifier> _logger;

        public SignalRSessionNotifier(IHubContext<GameHub> hub, ILogger<SignalRSessionNotifier> logger)
        {
            _hub = hub;
            _logger = logger;
        }

        public async Task SessionOverridden(int accountId, string clientType, string newSessionId)
        {
            try
            {
                await _hub.Clients
                    .Group(GameHub.SessionGroup(accountId, clientType))
                    .SendAsync("SessionOverridden", new { accountId, clientType, newSessionId });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "SessionOverridden notify failed for account {AccountId} ({ClientType})", accountId, clientType);
            }
        }
    }
}
