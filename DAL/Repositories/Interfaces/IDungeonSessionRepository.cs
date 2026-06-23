using DAL.Models;

namespace DAL.Repositories.Interfaces
{
    public interface IDungeonSessionRepository
    {
        /// <summary>Gets a session by its PK, including DungeonConfig, Progress, and Chest data.</summary>
        Task<DungeonSession?> GetById(int sessionId);

        /// <summary>Gets all sessions for a player profile (most recent first).</summary>
        Task<List<DungeonSession>> GetByPlayerProfileId(int playerProfileId);

        /// <summary>
        /// Returns an active session for this player+dungeon combination, if any exists.
        /// Used to prevent stacking multiple concurrent runs.
        /// </summary>
        Task<DungeonSession?> GetActiveSession(int playerProfileId, int dungeonConfigId);

        Task<DungeonSession> Create(DungeonSession session);
        Task<DungeonSession> Update(DungeonSession session);
    }
}
