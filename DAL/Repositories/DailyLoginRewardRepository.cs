using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class DailyLoginRewardRepository : IDailyLoginRewardRepository
    {
        private readonly MysticJourneyDbContext _context;

        public DailyLoginRewardRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<(int TotalCount, List<DailyLoginReward> Items)> GetDailyLoginRewardsPaged(int page, int pageSize)
        {
            var query = _context.DailyLoginRewards
                .Include(r => r.RewardItem)
                .AsNoTracking();

            int totalCount = await query.CountAsync();
            var items = await query.OrderBy(r => r.DayNumber).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (totalCount, items);
        }

        public async Task<DailyLoginReward?> GetDailyLoginRewardById(int id)
        {
            return await _context.DailyLoginRewards.FirstOrDefaultAsync(r => r.DailyLoginRewardId == id);
        }

        public async Task<DailyLoginReward?> GetDailyLoginRewardByDayNumber(int dayNumber)
        {
            return await _context.DailyLoginRewards.FirstOrDefaultAsync(r => r.DayNumber == dayNumber);
        }

        public async Task<List<DailyLoginReward>> GetAllDailyLoginRewards()
        {
            return await _context.DailyLoginRewards.OrderBy(r => r.DayNumber).ToListAsync();
        }

        public async Task<DailyLoginReward> CreateDailyLoginReward(DailyLoginReward reward)
        {
            await _context.DailyLoginRewards.AddAsync(reward);
            await _context.SaveChangesAsync();
            return reward;
        }

        public async Task<DailyLoginReward> UpdateDailyLoginReward(DailyLoginReward reward)
        {
            _context.DailyLoginRewards.Update(reward);
            await _context.SaveChangesAsync();
            return reward;
        }

    }
}
