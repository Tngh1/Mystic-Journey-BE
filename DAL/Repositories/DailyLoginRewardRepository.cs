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

        // ── Base query helper ────────────────────────────────────────────────
        private IQueryable<DailyLoginReward> BaseQuery() =>
            _context.DailyLoginRewards.Include(r => r.RewardItem).AsNoTracking();

        // ── GAME APIs ────────────────────────────────────────────────────────

        public async Task<DailyLoginReward?> GetDailyLoginRewardById(int id)
        {
            return await BaseQuery().FirstOrDefaultAsync(r => r.DailyLoginRewardId == id);
        }

        // Lấy override cho ngày + tháng/năm cụ thể (không fallback).
        public async Task<DailyLoginReward?> GetByDayAndMonth(int dayNumber, int month, int year)
        {
            return await BaseQuery().FirstOrDefaultAsync(r =>
                r.DayNumber == dayNumber &&
                r.Month == month &&
                r.Year == year &&
                r.IsActive);
        }

        // Lấy default cho ngày (Month=null, Year=null).
        public async Task<DailyLoginReward?> GetDefaultByDayNumber(int dayNumber)
        {
            return await BaseQuery().FirstOrDefaultAsync(r =>
                r.DayNumber == dayNumber &&
                r.Month == null &&
                r.Year == null &&
                r.IsActive);
        }

        // Lấy tất cả overrides của một tháng/năm cụ thể.
        public async Task<List<DailyLoginReward>> GetOverridesByMonth(int month, int year)
        {
            return await BaseQuery()
                .Where(r => r.Month == month && r.Year == year && r.IsActive)
                .OrderBy(r => r.DayNumber)
                .ToListAsync();
        }

        // Lấy tất cả default (Month=null, Year=null).
        public async Task<List<DailyLoginReward>> GetAllDefaults()
        {
            return await BaseQuery()
                .Where(r => r.Month == null && r.Year == null && r.IsActive)
                .OrderBy(r => r.DayNumber)
                .ToListAsync();
        }

        // ── ADMIN APIs ───────────────────────────────────────────────────────

        // Phân trang: month=null → lấy defaults; month có giá trị → lấy overrides tháng đó.
        public async Task<(int TotalCount, List<DailyLoginReward> Items)> GetDailyLoginRewardsPaged(
            int page, int pageSize, int? month = null, int? year = null)
        {
            IQueryable<DailyLoginReward> query;

            if (month == null || year == null)
            {
                // Lấy default records (Month IS NULL AND Year IS NULL)
                query = BaseQuery().Where(r => r.Month == null && r.Year == null);
            }
            else
            {
                // Lấy override records của tháng/năm cụ thể
                query = BaseQuery().Where(r => r.Month == month && r.Year == year);
            }

            int totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(r => r.DayNumber)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (totalCount, items);
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

        public async Task DeleteDailyLoginReward(int id)
        {
            var reward = await _context.DailyLoginRewards.FindAsync(id);
            if (reward != null)
            {
                reward.IsActive = false;
                _context.DailyLoginRewards.Update(reward);
                await _context.SaveChangesAsync();
            }
        }
    }
}
