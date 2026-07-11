namespace BLL.Services.Interfaces
{
    public interface IPlayerHeartbeatService
    {
        Task UpdateLastSeenAsync(int accountId);
        bool IsOnline(DateTime? lastSeen);
    }
}
