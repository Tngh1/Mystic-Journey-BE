using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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

        public async Task<DungeonConfig?> GetByIdWithChest(int id)
        {
            return await _context.DungeonConfigs
                .Include(d => d.Chest)
                    .ThenInclude(c => c!.ChestItems)
                        .ThenInclude(ci => ci.Item)
                .FirstOrDefaultAsync(d => d.DungeonConfigId == id && d.IsActive);
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


        public async Task<(int TotalCount, List<DungeonConfig> Items)> GetDungeonsPaged(int page, int pageSize, string? search, string? type, bool? isActive)
        {
            var query = _context.DungeonConfigs.AsNoTracking();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d => d.Name.Contains(search));
            }
            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(d => d.Type == type);
            }
            if (isActive.HasValue)
            {
                query = query.Where(d => d.IsActive == isActive.Value);
            }

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (totalCount, items);
        }
    }
}
