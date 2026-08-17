namespace BLL.Services.Interfaces
{
    // Initializes a new default instance of the ISessionNotifier class.
    public interface ISessionNotifier
    {
        Task SessionOverridden(int accountId, string clientType, string newSessionId);
    }
}
