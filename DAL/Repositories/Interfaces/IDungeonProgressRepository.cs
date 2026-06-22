using DAL.Models;

namespace DAL.Repositories.Interfaces
{
    public interface IDungeonProgressRepository
    {
        /// <summary>Gets the progress record for a session, or null if not yet created.</summary>
        Task<DungeonProgress?> GetBySessionId(int sessionId);

        Task<DungeonProgress> Create(DungeonProgress progress);
        Task<DungeonProgress> Update(DungeonProgress progress);
    }
}
