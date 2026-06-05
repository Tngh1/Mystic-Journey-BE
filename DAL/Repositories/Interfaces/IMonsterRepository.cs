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
        Task DeleteMonster(int id);
        Task<MonsterDrop> CreateDrop(MonsterDrop drop);
        Task<List<MonsterDrop>> GetDropsByMonsterId(int monsterId);
        IQueryable<Monster> GetMonstersQueryable();
    }
}
