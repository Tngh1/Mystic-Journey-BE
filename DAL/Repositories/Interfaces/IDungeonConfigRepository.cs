using DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IDungeonConfigRepository
    {
        Task<DungeonConfig?> GetDungeonConfigById(int id);
        Task<DungeonConfig> CreateDungeonConfig(DungeonConfig dungeon);
        Task<DungeonConfig> UpdateDungeonConfig(DungeonConfig dungeon);
        Task<(int TotalCount, List<DungeonConfig> Items)> GetDungeonsPaged(int page, int pageSize, string? search, string? type, bool? isActive);
    }
}
