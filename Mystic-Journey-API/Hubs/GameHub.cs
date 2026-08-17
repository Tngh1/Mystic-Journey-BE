using BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Mystic_Journey_API.Hubs
{
    // Executes hub operation.
    [Authorize]
    public class GameHub : Hub
    {

        // Executes session group operation.
        public static string SessionGroup(int accountId, string? clientType)
            => AuthService.ActiveSessionKey(accountId, clientType);

        // Executes on connected async operation.
        public override async Task OnConnectedAsync()
        {

            var accountIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // Supported client types: Web or Game; this value selects the independent refresh-token slot and session behavior.
            var clientType = Context.User?.FindFirst(AuthService.ClientTypeClaim)?.Value;

            if (int.TryParse(accountIdClaim, out var accountId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, SessionGroup(accountId, clientType));
            }

            await base.OnConnectedAsync();
        }

    }
}
