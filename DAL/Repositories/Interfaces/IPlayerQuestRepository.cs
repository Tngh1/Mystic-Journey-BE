using DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IPlayerQuestRepository
    {
        Task<PlayerQuest?> GetByIdAsync(Guid playerQuestId);
        Task<PlayerQuest?> GetByIdWithDetailsAsync(Guid playerQuestId);
        Task<List<PlayerQuest>> GetByPlayerProfileIdAsync(Guid playerProfileId);
        Task<List<PlayerQuest>> GetActiveQuestsAsync(Guid playerProfileId);
        Task<List<PlayerQuest>> GetCompletedQuestsAsync(Guid playerProfileId);
        Task<PlayerQuest?> GetByPlayerAndQuestAsync(Guid playerProfileId, Guid questId);
        Task<PlayerQuest> CreateAsync(PlayerQuest playerQuest);
        Task<PlayerQuest> UpdateAsync(PlayerQuest playerQuest);
        Task<bool> HasQuestAsync(Guid playerProfileId, Guid questId);
    }
}
