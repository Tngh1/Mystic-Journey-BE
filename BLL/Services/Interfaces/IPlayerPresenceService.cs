using System;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IPlayerPresenceService
    {
        void UpdatePresence(int playerId);
        bool IsOnline(int playerId);
    }
}
