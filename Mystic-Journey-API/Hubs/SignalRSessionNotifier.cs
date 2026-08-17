using BLL.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Mystic_Journey_API.Hubs
{
    // Executes i session notifier operation.
    public class SignalRSessionNotifier : ISessionNotifier
    {
        private readonly IHubContext<GameHub> _hub;
        private readonly ILogger<SignalRSessionNotifier> _logger;

        // Initializes a new instance of SignalRSessionNotifier with dependencies: hub, logger.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public SignalRSessionNotifier(IHubContext<GameHub> hub, ILogger<SignalRSessionNotifier> logger)
        {
            _hub = hub;
            _logger = logger;
        }

        // Executes session overridden operation.
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
