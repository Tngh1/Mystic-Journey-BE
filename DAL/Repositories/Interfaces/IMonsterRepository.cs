using DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IMonsterRepository
    {
        Task<Monster?> GetMonsterById(int id);
        Task<Monster?> GetMonsterByIdWithDrops(int id);
        Task<Monster> CreateMonster(Monster monster);
        Task<Monster> UpdateMonster(Monster monster);
        Task<MonsterDrop> CreateDrop(MonsterDrop drop);
        Task<PlayerMonsterDiscovery?> GetPlayerDiscovery(int playerProfileId, int monsterId);
        Task<PlayerMonsterDiscovery> CreateOrUpdatePlayerDiscovery(PlayerMonsterDiscovery discovery);
        Task<HashSet<int>> GetCompletedQuestBossMonsterIds(int playerProfileId);
        Task<HashSet<int>> GetDiscoveredMonsterIds(int playerProfileId);
        Task<List<MonsterSpawn>> GetSpawnsByMonsterId(int monsterId);
        Task<List<MonsterSpawn>> GetActiveSpawns(string mapName, string? regionName, int? dungeonId);
        Task<MonsterSpawn> CreateSpawn(MonsterSpawn spawn);
        Task<List<MonsterDrop>> GetActiveDropsByMonsterId(int monsterId);
        Task<List<MonsterDrop>> GetDropsByMonsterId(int monsterId);
        Task<(int TotalCount, List<Monster> Items)> GetMonstersPaged(int page, int pageSize, string? search, string? type, bool? isActive);
        Task<(int TotalCount, List<MonsterDrop> Items)> GetMonsterDropsPaged(int page, int pageSize);
    }
}
