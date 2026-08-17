namespace BLL.Services.Interfaces
{
    // Initializes a new default instance of the IPlayerHeartbeatService class.
    public interface IPlayerHeartbeatService
    {
        Task UpdateLastSeenAsync(int accountId);
        bool IsOnline(DateTime? lastSeen);
    }
}
