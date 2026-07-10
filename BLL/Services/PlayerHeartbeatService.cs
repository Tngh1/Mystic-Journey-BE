using BLL.Services.Interfaces;
using DAL.Repositories.Interfaces;
using System;

namespace BLL.Services
{
    public class PlayerHeartbeatService : IPlayerHeartbeatService
    {
        private readonly IAuthRepository _authRepository;

        public PlayerHeartbeatService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public async Task UpdateLastSeenAsync(int accountId)
        {
            var account = await _authRepository.GetAccountById(accountId);
            if (account != null)
            {
                account.LastSeen = DateTime.UtcNow;
                await _authRepository.UpdateAccount(account);
            }
        }

        public bool IsOnline(DateTime? lastSeen)
        {
            if (!lastSeen.HasValue)
                return false;

            return lastSeen.Value >= DateTime.UtcNow.AddMinutes(-1);
        }
    }
}
