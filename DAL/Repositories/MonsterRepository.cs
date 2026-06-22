using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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

        public async Task<(int TotalCount, List<Monster> Items)> GetMonstersPaged(int page, int pageSize, string? search, string? type, bool? isActive)
        {
            var query = _context.Monsters
                .Include(m => m.MonsterDrops)
                    .ThenInclude(d => d.Item)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Name.Contains(search));
            }
            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(x => x.Type == type);
            }
            if (isActive.HasValue)
            {
                query = query.Where(x => x.IsActive == isActive.Value);
            }

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (totalCount, items);
        }

        public async Task<(int TotalCount, List<MonsterDrop> Items)> GetMonsterDropsPaged(int page, int pageSize)
        {
            var query = _context.MonsterDrops
                .Include(d => d.Item)
                .Where(d => d.IsActive)
                .AsNoTracking();

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (totalCount, items);
        }
    }
}
