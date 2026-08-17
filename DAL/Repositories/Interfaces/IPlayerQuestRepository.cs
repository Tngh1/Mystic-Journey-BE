using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Initializes a new default instance of the IPlayerQuestRepository class.
    public interface IPlayerQuestRepository
    {

        Task<List<PlayerQuest>> GetByPlayerId(int playerProfileId);

        Task<PlayerQuest?> GetByPlayerAndQuest(int playerProfileId, int questId);

        Task<List<PlayerQuest>> GetByPlayerAndQuestIds(int playerProfileId, List<int> questIds);

        Task<PlayerQuest> Create(PlayerQuest entity);

        Task<PlayerQuest> Update(PlayerQuest entity);

        Task UpdateRange(List<PlayerQuest> entities);
    }
}
