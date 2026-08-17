using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Initializes a new default instance of the IDungeonSessionRepository class.
    public interface IDungeonSessionRepository
    {

        Task<DungeonSession?> GetById(int sessionId);

        Task<List<DungeonSession>> GetByPlayerProfileId(int playerProfileId);

        Task<DungeonSession?> GetActiveSession(int playerProfileId, int? dungeonConfigId = null);

        Task<int> FailActiveSessions(int playerProfileId);

        Task<DungeonSession> Create(DungeonSession session);

        Task<DungeonSession> Update(DungeonSession session);
    }
}
