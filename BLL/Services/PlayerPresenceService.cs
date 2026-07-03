using BLL.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace BLL.Services
{
    public class PlayerPresenceService : IPlayerPresenceService
    {
        private readonly IMemoryCache _cache;
        
        // Timeout for heartbeat. If client pings every 30s, we give them 60s before marked offline.
        private static readonly TimeSpan HeartbeatTimeout = TimeSpan.FromSeconds(60);

        public PlayerPresenceService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void UpdatePresence(int playerId)
        {
            var cacheKey = $"Presence_Player_{playerId}";
            _cache.Set(cacheKey, DateTime.UtcNow, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = HeartbeatTimeout
            });
        }

        public bool IsOnline(int playerId)
        {
            var cacheKey = $"Presence_Player_{playerId}";
            // If the key exists in cache, they are online
            return _cache.TryGetValue(cacheKey, out _);
        }
    }
}
