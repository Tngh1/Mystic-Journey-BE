using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Initializes a new default instance of the IWorldRepository class.
    public interface IWorldRepository
    {

        Task<List<NPC>> GetNpcsByMapName(string mapName, int take);

        Task<List<string>> GetAllNpcMapNames();

        Task<NPC?> GetNpcById(int npcId);

        Task<bool> IsQuestLinkedToNpc(int npcId, int questId);

        Task<Chest?> GetChestById(int chestId);

        Task<PlayerChest?> GetPlayerChest(int playerChestId, int playerProfileId);

        Task<PlayerChest> CreatePlayerChest(PlayerChest playerChest);

        Task<PlayerChest> UpdatePlayerChest(PlayerChest playerChest);

        Task<PlayerDailyLogin?> GetPlayerDailyLogin(int playerProfileId);

        Task<PlayerDailyLogin> CreatePlayerDailyLogin(PlayerDailyLogin login);

        Task<PlayerDailyLogin> UpdatePlayerDailyLogin(PlayerDailyLogin login);

        Task<DailyLoginReward?> GetDailyLoginReward(int dayNumber, int month, int year);
    }
}
