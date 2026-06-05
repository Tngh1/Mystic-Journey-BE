using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class DungeonConfigRepository : IDungeonConfigRepository
    {
        private readonly MysticJourneyDbContext _context;

        public DungeonConfigRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<DungeonConfig?> GetDungeonConfigById(int id)
        {
            return await _context.DungeonConfigs
                .FirstOrDefaultAsync(d => d.DungeonConfigId == id);
        }

        public async Task<List<DungeonConfig>> GetAllDungeonConfigs()
        {
            return await _context.DungeonConfigs.ToListAsync();
        }

        public async Task<List<DungeonConfig>> GetActiveDungeonConfigs()
        {
            return await _context.DungeonConfigs
                .Where(d => d.IsActive)
                .ToListAsync();
        }

        public async Task<DungeonConfig> CreateDungeonConfig(DungeonConfig dungeon)
        {
            await _context.DungeonConfigs.AddAsync(dungeon);
            await _context.SaveChangesAsync();
            return dungeon;
        }

        public async Task<DungeonConfig> UpdateDungeonConfig(DungeonConfig dungeon)
        {
_context.DungeonConfigs.Update(dungeon);
            await _context.SaveChangesAsync();
            return dungeon;
        }

        public async Task DeleteDungeonConfig(int id)
        {
            var dungeon = await GetDungeonConfigById(id);
            if (dungeon != null)
            {
                _context.DungeonConfigs.Remove(dungeon);
                await _context.SaveChangesAsync();
            }
        }

        public IQueryable<DungeonConfig> GetDungeonConfigsQueryable()
        {
            return _context.DungeonConfigs.AsNoTracking();
        }
    }
}
