using DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IMonsterRepository
    {
        Task<Monster?> GetMonsterById(int id);
        Task<Monster?> GetMonsterByIdWithDrops(int id);
        Task<List<Monster>> GetAllMonsters();
        Task<List<Monster>> GetActiveMonsters();
        Task<Monster> CreateMonster(Monster monster);
        Task<Monster> UpdateMonster(Monster monster);
        Task<MonsterDrop> CreateDrop(MonsterDrop drop);
        Task<(int TotalCount, List<Monster> Items)> GetMonstersPaged(int page, int pageSize, string? search, string? type, bool? isActive);
        Task<(int TotalCount, List<MonsterDrop> Items)> GetMonsterDropsPaged(int page, int pageSize);
    }
}
