using DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IQuestRepository
    {
        Task<Quest?> GetByIdAsync(Guid questId);
        Task<Quest?> GetByIdWithRewardAsync(Guid questId);
        Task<List<Quest>> GetAllActiveAsync();
        Task<List<Quest>> GetByTypeAsync(Quest.QuestType type);
        Task<List<Quest>> GetAvailableForLevelAsync(int playerLevel);
        Task<Quest> CreateAsync(Quest quest);
        Task<Quest> UpdateAsync(Quest quest);
    }
}
