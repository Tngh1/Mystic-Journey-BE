using BLL.Services.Interfaces;
using BLL.Utils;
using DAL.Repositories.Interfaces;
using System;

namespace BLL.Services
{
    // Executes core business logic for i player heartbeat service.
    public class PlayerHeartbeatService : IPlayerHeartbeatService
    {
        private readonly IAuthRepository _authRepository;

        // Initializes a new instance of PlayerHeartbeatService with dependencies: authRepository.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public PlayerHeartbeatService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        // Executes core business logic for update last seen async.
        // Logic details: delegates data queries and updates to repository layer.
        // Completes asynchronously upon successful execution.
        public async Task UpdateLastSeenAsync(int accountId)
        {
            await _authRepository.TouchLastSeen(accountId, DateTime.UtcNow);
        }

        // Executes core business logic for is online.
        // Returns a boolean indicating operation success.
        public bool IsOnline(DateTime? lastSeen)
            => OnlineTimeout.IsWithin(lastSeen, OnlineTimeout.Presence);
    }
}
