using System;

namespace BLL.Services
{
    public class ChatLockedException : Exception
    {
        public DateTime LockedUntil { get; }
        public int LockLevel { get; }
        public int RetryAfterSeconds { get; }

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