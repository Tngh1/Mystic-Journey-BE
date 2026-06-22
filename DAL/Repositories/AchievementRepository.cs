using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class AchievementRepository : IAchievementRepository
    {
        private readonly MysticJourneyDbContext _context;

        public AchievementRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<Achievement?> GetAchievementById(int id)
        {
            return await _context.Achievements
                .FirstOrDefaultAsync(a => a.AchievementId == id);
        }

        public async Task<Achievement?> GetAchievementByIdWithReward(int id)
        {
            return await _context.Achievements
                .Include(a => a.RewardItem)
                .FirstOrDefaultAsync(a => a.AchievementId == id);
        }

        public async Task<Achievement> CreateAchievement(Achievement achievement)
        {
            await _context.Achievements.AddAsync(achievement);
            await _context.SaveChangesAsync();
            return achievement;
        }

        public async Task<Achievement> UpdateAchievement(Achievement achievement)
        {
_context.Achievements.Update(achievement);
            await _context.SaveChangesAsync();
            return achievement;
        }


        public async Task<(int TotalCount, List<Achievement> Items)> GetAchievementsPaged(int page, int pageSize, string? search, string? type, bool? isActive)
        {
            var query = _context.Achievements
                .Include(a => a.RewardItem)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(a => a.Name.Contains(search));
            }
            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(a => a.Type == type);
            }
            if (isActive.HasValue)
            {
                query = query.Where(a => a.IsActive == isActive.Value);
            }

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (totalCount, items);
        }
    }
}
