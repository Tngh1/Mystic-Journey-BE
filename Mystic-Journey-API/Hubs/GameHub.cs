using BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Mystic_Journey_API.Hubs
{
    [Authorize]
    public class GameHub : Hub
    {

        public static string SessionGroup(int accountId, string? clientType)
            => AuthService.ActiveSessionKey(accountId, clientType);

        public override async Task OnConnectedAsync()
        {

            var accountIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var clientType = Context.User?.FindFirst(AuthService.ClientTypeClaim)?.Value;

            if (int.TryParse(accountIdClaim, out var accountId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, SessionGroup(accountId, clientType));
            }

            await base.OnConnectedAsync();
        }

    }
}
