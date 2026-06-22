using DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IDungeonConfigRepository
    {
        Task<DungeonConfig?> GetDungeonConfigById(int id);

        /// <summary>
        /// Gets a DungeonConfig and eagerly loads its Chest with ChestItems and Items.
        /// Used by the session service for energy validation and reward preview.
        /// </summary>
        Task<DungeonConfig?> GetByIdWithChest(int id);
        Task<List<DungeonConfig>> GetAllDungeonConfigs();
        Task<List<DungeonConfig>> GetActiveDungeonConfigs();
        Task<DungeonConfig> CreateDungeonConfig(DungeonConfig dungeon);
        Task<DungeonConfig> UpdateDungeonConfig(DungeonConfig dungeon);
        Task<(int TotalCount, List<DungeonConfig> Items)> GetDungeonsPaged(int page, int pageSize, string? search, string? type, bool? isActive);
    }
}
