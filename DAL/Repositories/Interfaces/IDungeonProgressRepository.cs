using DAL.Models;

namespace DAL.Repositories.Interfaces
{
    // Initializes a new default instance of the IDungeonProgressRepository class.
    public interface IDungeonProgressRepository
    {

        Task<DungeonProgress?> GetBySessionId(int sessionId);

        Task<DungeonProgress> Create(DungeonProgress progress);

        Task<DungeonProgress> Update(DungeonProgress progress);
    }
}
