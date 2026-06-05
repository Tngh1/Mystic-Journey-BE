using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class GameSettingRepository : IGameSettingRepository
    {
        private readonly MysticJourneyDbContext _context;

        public GameSettingRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<GameSetting?> GetGameSettingById(int id)
        {
            return await _context.GameSettings
                .FirstOrDefaultAsync(g => g.GameSettingId == id);
        }

        public async Task<GameSetting?> GetByName(string name)
        {
            return await _context.GameSettings
                .FirstOrDefaultAsync(g => g.Name.ToLower() == name.ToLower());
        }

        public async Task<List<GameSetting>> GetAllGameSettings()
        {
            return await _context.GameSettings.ToListAsync();
        }

        public async Task<GameSetting> CreateGameSetting(GameSetting setting)
        {
            await _context.GameSettings.AddAsync(setting);
            await _context.SaveChangesAsync();
            return setting;
        }

        public async Task<GameSetting> UpdateGameSetting(GameSetting setting)
        {
_context.GameSettings.Update(setting);
            await _context.SaveChangesAsync();
            return setting;
        }


        public async Task<(int TotalCount, List<GameSetting> Items)> GetSettingsPaged(int page, int pageSize, string? search)
        {
            var query = _context.GameSettings
                .Include(g => g.UpdatedByAccount)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Name.Contains(search) || (x.Value != null && x.Value.Contains(search)) || (x.Description != null && x.Description.Contains(search)));
            }

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (totalCount, items);
        }
    }
}
