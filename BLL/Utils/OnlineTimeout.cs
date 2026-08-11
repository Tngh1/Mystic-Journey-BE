using System;

namespace BLL.Utils
{

    public static class OnlineTimeout
    {

        public const int HeartbeatSeconds = 30;
        public static readonly TimeSpan Presence = TimeSpan.FromMinutes(5);
        public static readonly TimeSpan Dashboard = TimeSpan.FromMinutes(1);
        public static bool IsWithin(DateTime? lastSeen, TimeSpan window)
            => lastSeen.HasValue && lastSeen.Value >= DateTime.UtcNow - window;
    }
}
