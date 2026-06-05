using DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IDungeonConfigRepository
    {
        Task<DungeonConfig?> GetDungeonConfigById(int id);
        Task<List<DungeonConfig>> GetAllDungeonConfigs();
        Task<List<DungeonConfig>> GetActiveDungeonConfigs();
        Task<DungeonConfig> CreateDungeonConfig(DungeonConfig dungeon);
        Task<DungeonConfig> UpdateDungeonConfig(DungeonConfig dungeon);
        Task DeleteDungeonConfig(int id);
        IQueryable<DungeonConfig> GetDungeonConfigsQueryable();
    }
}
