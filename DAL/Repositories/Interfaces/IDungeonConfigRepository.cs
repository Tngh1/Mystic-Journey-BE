using DAL.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Initializes a new default instance of the IDungeonConfigRepository class.
    public interface IDungeonConfigRepository
    {

        Task<DungeonConfig?> GetDungeonConfigById(int id);

        Task<DungeonConfig?> GetByIdWithChest(int id);

        Task<List<DungeonConfig>> GetAllDungeonConfigs();

        Task<List<DungeonConfig>> GetActiveDungeonConfigs();


        Task<DungeonConfig> UpdateDungeonConfig(DungeonConfig dungeon);

        Task<bool> DungeonExists(int dungeonId);

        Task<(int TotalCount, List<DungeonConfig> Items)> GetDungeonsPaged(int page, int pageSize, string? search, string? type, bool? isActive, string? sortBy = null, string? sortOrder = null);
    }
}
