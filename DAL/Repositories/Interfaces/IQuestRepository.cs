using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Initializes a new default instance of the IQuestRepository class.
    public interface IQuestRepository
    {

        Task<Quest?> GetQuestById(int id);

        Task<Quest?> GetByIdWithReward(int id);

        Task<List<Quest>> GetActiveQuests();


        Task<Quest> AddQuest(Quest quest);

        Task<Quest> UpdateQuest(Quest quest);

        Task<NPCDialogue?> GetQuestDialogueByQuestId(int questId);

        Task<NPC?> GetNpcByNameAndMap(string? npcName, string mapName);

        Task<List<NPC>> GetQuestNpcOptions(string? mapName);

        void AddQuestDialogue(NPCDialogue dialogue);

        Task<(int TotalCount, List<Quest> Items)> GetQuestsPaged(int page, int pageSize, string? search, string? type, bool? isActive, string? mapName, string? sortBy = null, string? sortOrder = null);
    }
}
