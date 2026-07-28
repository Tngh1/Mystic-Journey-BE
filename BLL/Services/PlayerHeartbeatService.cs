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
            // Không cần đọc entity trước: nếu accountId không tồn tại thì UPDATE khớp 0 hàng,
            // đúng bằng hành vi "account == null" của đường cũ.
            await _authRepository.TouchLastSeen(accountId, DateTime.UtcNow);
        }

        public bool IsOnline(DateTime? lastSeen)
        {
            if (!lastSeen.HasValue)
                return false;

            return lastSeen.Value >= DateTime.UtcNow.AddMinutes(-5);
        }
    }
}
