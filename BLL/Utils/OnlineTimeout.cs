using System;

namespace BLL.Utils
{

    // Initializes a new default instance of the OnlineTimeout class.
    public static class OnlineTimeout
    {

        public const int HeartbeatSeconds = 30;
        public static readonly TimeSpan Presence = TimeSpan.FromMinutes(5);
        public static readonly TimeSpan Dashboard = TimeSpan.FromMinutes(1);
        // Executes is within operation.
        public static bool IsWithin(DateTime? lastSeen, TimeSpan window)
            => lastSeen.HasValue && lastSeen.Value >= DateTime.UtcNow - window;
    }
}
