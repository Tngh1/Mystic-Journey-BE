using System;

namespace BLL.Services
{
    // Executes exception operation.
    public class ChatLockedException : Exception
    {
        // Executes locked until operation.
        public DateTime LockedUntil { get; }
        // Executes lock level operation.
        public int LockLevel { get; }
        // Executes retry after seconds operation.
        public int RetryAfterSeconds { get; }

        // Initializes a new instance of ChatLockedException with dependencies: lockedUntil, lockLevel.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public ChatLockedException(DateTime lockedUntil, int lockLevel)
            : base($"Chat is locked until {lockedUntil:yyyy-MM-dd HH:mm:ss} UTC.")
        {
            LockedUntil = lockedUntil;
            LockLevel = lockLevel;
            RetryAfterSeconds = lockedUntil > DateTime.UtcNow
                ? (int)Math.Ceiling((lockedUntil - DateTime.UtcNow).TotalSeconds)
                : 0;
        }
    }
}
