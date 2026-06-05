using DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IQuestRepository
    {
        Task<Quest?> GetQuestById(int id);
        Task<Quest?> GetByIdWithReward(int id);
        Task<List<Quest>> GetAllQuests();
        Task<List<Quest>> GetActiveQuests();
        Task<Quest> CreateQuest(Quest quest);
        Task<Quest> UpdateQuest(Quest quest);
        Task DeleteQuest(int id);
        IQueryable<Quest> GetQuestsQueryable();
    }
}
