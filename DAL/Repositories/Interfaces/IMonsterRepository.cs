using DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Initializes a new default instance of the IMonsterRepository class.
    public interface IMonsterRepository
    {

        Task<Monster?> GetMonsterById(int id);

        Task<Monster?> GetMonsterByIdWithDrops(int id);

        Task<PlayerMonsterDiscovery?> GetPlayerDiscovery(int playerProfileId, int monsterId);

        Task<Dictionary<int, PlayerMonsterDiscovery>> GetPlayerDiscoveriesDict(int playerProfileId);

        Task<HashSet<int>> GetCompletedQuestBossMonsterIds(int playerProfileId);

        Task<HashSet<int>> GetDiscoveredMonsterIds(int playerProfileId);


        Task<Monster> UpdateMonster(Monster monster);

        Task<MonsterDrop> CreateDrop(MonsterDrop drop);

        Task<PlayerMonsterDiscovery> CreateOrUpdatePlayerDiscovery(PlayerMonsterDiscovery discovery);

        Task<MonsterSpawn?> GetSpawnById(int spawnId);

        Task<MonsterSpawn> UpdateSpawn(MonsterSpawn spawn);

        Task DeleteSpawn(int spawnId);

        Task<List<MonsterSpawn>> GetSpawnsByMonsterId(int monsterId);

        Task<List<MonsterSpawn>> GetActiveSpawns(string mapName, string? regionName, int? dungeonId);

        Task<MonsterSpawn> CreateSpawn(MonsterSpawn spawn);

        Task<List<MonsterDrop>> GetDropsByMonsterId(int monsterId);

        Task<(int TotalCount, List<Monster> Items)> GetMonstersPaged(int page, int pageSize, string? search, string? type, bool? isActive, string? sortBy = null, string? sortOrder = null);

        Task<(int TotalCount, List<MonsterDrop> Items)> GetMonsterDropsPaged(int page, int pageSize);

        Task<int> GetTotalMonstersCount();
    }
}
