using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class MonsterRepository : IMonsterRepository
    {
        private readonly MysticJourneyDbContext _context;

        public MonsterRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<Monster?> GetMonsterById(int id)
        {
            return await _context.Monsters
                .FirstOrDefaultAsync(m => m.MonsterId == id);
        }

        public async Task<Monster?> GetMonsterByIdWithDrops(int id)
        {
            return await _context.Monsters
                .Include(m => m.MonsterDrops)
                    .ThenInclude(d => d.Item)
                .FirstOrDefaultAsync(m => m.MonsterId == id);
        }

        public async Task<List<Monster>> GetAllMonsters()
        {
            return await _context.Monsters.ToListAsync();
        }

        public async Task<List<Monster>> GetActiveMonsters()
        {
            return await _context.Monsters
                .Where(m => m.IsActive)
                .ToListAsync();
        }

        public async Task<Monster> CreateMonster(Monster monster)
        {
            await _context.Monsters.AddAsync(monster);
            await _context.SaveChangesAsync();
            return monster;
        }

        public async Task<Monster> UpdateMonster(Monster monster)
        {
_context.Monsters.Update(monster);
            await _context.SaveChangesAsync();
            return monster;
        }

        public async Task DeleteMonster(int id)
        {
            var monster = await GetMonsterById(id);
            if (monster != null)
            {
                _context.Monsters.Remove(monster);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<MonsterDrop> CreateDrop(MonsterDrop drop)
        {
            await _context.MonsterDrops.AddAsync(drop);
            await _context.SaveChangesAsync();
            return drop;
        }

        public async Task<List<MonsterDrop>> GetDropsByMonsterId(int monsterId)
        {
            return await _context.MonsterDrops
                .Include(d => d.Item)
                .Where(d => d.MonsterId == monsterId)
                .ToListAsync();
        }

        public IQueryable<Monster> GetMonstersQueryable()
        {
            return _context.Monsters
                .Include(m => m.MonsterDrops)
                    .ThenInclude(d => d.Item)
                .AsNoTracking();
        }
    }
}
