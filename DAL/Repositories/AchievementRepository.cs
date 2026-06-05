using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
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

        public async Task<List<Achievement>> GetAllAchievements()
        {
            return await _context.Achievements.ToListAsync();
        }

        public async Task<List<Achievement>> GetActiveAchievements()
        {
            return await _context.Achievements
                .Include(a => a.RewardItem)
                .Where(a => a.IsActive)
                .ToListAsync();
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

        public async Task DeleteAchievement(int id)
        {
            var achievement = await GetAchievementById(id);
            if (achievement != null)
            {
                _context.Achievements.Remove(achievement);
                await _context.SaveChangesAsync();
            }
        }

        public IQueryable<Achievement> GetAchievementsQueryable()
        {
            return _context.Achievements
                .Include(a => a.RewardItem)
                .AsNoTracking();
        }
    }
}
