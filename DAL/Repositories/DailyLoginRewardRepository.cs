using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    // Queries the database to retrieve i daily login reward repository records.
    public class DailyLoginRewardRepository : IDailyLoginRewardRepository
    {
        private readonly MysticJourneyDbContext _context;

        // Initializes a new instance of DailyLoginRewardRepository with dependencies: context.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public DailyLoginRewardRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        // Queries the database to retrieve base query records.
        // Query details: uses AsNoTracking() for read-only query optimization; eagerly loads related entity navigation properties.
        private IQueryable<DailyLoginReward> BaseQuery() =>
            // Execute this query without change tracking because the returned entities are read-only.
            _context.DailyLoginRewards.Include(r => r.RewardItem).AsNoTracking();  // Disable EF Core change tracking for this read-only query


        // Queries the database to retrieve get daily login reward by id records.
        // Returns the matching DailyLoginReward? entity result or default if not found.
        public async Task<DailyLoginReward?> GetDailyLoginRewardById(int id)
        {
            return await BaseQuery().FirstOrDefaultAsync(r => r.DailyLoginRewardId == id);  // Fetch single matching record or null if not found
        }

        // Queries the database to retrieve get by day and month records.
        // Returns the matching DailyLoginReward? entity result or default if not found.
        public async Task<DailyLoginReward?> GetByDayAndMonth(int dayNumber, int month, int year)
        {
            return await BaseQuery().FirstOrDefaultAsync(r =>  // Fetch single matching record or null if not found
                r.DayNumber == dayNumber &&
                r.Month == month &&
                r.Year == year &&
                r.IsActive);
        }

        // Queries the database to retrieve get default by day number records.
        // Query details: sorts records according to business ordering rules.
        // Returns the matching DailyLoginReward? entity result or default if not found.
        public async Task<DailyLoginReward?> GetDefaultByDayNumber(int dayNumber)
        {
            return await BaseQuery().FirstOrDefaultAsync(r =>  // Fetch single matching record or null if not found
                r.DayNumber == dayNumber &&
                r.Month == null &&
                r.Year == null &&
                r.IsActive);
        }

        // Queries the database to retrieve get overrides by month records.
        // Query details: sorts records according to business ordering rules.
        // Returns the matching List<DailyLoginReward entity result or default if not found.
        public async Task<List<DailyLoginReward>> GetOverridesByMonth(int month, int year)
        {
            return await BaseQuery()
                .Where(r => r.Month == month && r.Year == year && r.IsActive)  // Filter records matching the predicate
                .OrderBy(r => r.DayNumber)  // Sort results oldest/lowest first
                .ToListAsync();  // Materialize the query into a list from the database
        }

        // Load all defaults; it filters the eligible records, orders the resulting records, and materializes the query results.
        public async Task<List<DailyLoginReward>> GetAllDefaults()
        {
            return await BaseQuery()
                .Where(r => r.Month == null && r.Year == null && r.IsActive)  // Filter records matching the predicate
                .OrderBy(r => r.DayNumber)  // Sort results oldest/lowest first
                .ToListAsync();  // Materialize the query into a list from the database
        }


        // Load daily login rewards paged using total count, page, page size, and month; it filters the eligible records, orders the resulting records, and materializes the query results and guards invalid or unavailable states.
        public async Task<(int TotalCount, List<DailyLoginReward> Items)> GetDailyLoginRewardsPaged(
            int page, int pageSize, int? month = null, int? year = null)
        {
            IQueryable<DailyLoginReward> query;

            if (month == null || year == null)
            {
                query = BaseQuery().Where(r => r.Month == null && r.Year == null);  // Filter records matching the predicate
            }
            else
            {
                query = BaseQuery().Where(r => r.Month == month && r.Year == year);  // Filter records matching the predicate
            }

            int totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(r => r.DayNumber)  // Sort results oldest/lowest first
                .Skip((page - 1) * pageSize)  // Apply pagination offset — skip already-seen records
                .Take(pageSize)  // Apply pagination limit — cap result set size
                .ToListAsync();  // Materialize the query into a list from the database

            return (totalCount, items);
        }

        // Persists state modifications to the database for create daily login reward.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching DailyLoginReward entity result or default if not found.
        public async Task<DailyLoginReward> CreateDailyLoginReward(DailyLoginReward reward)
        {
            await _context.DailyLoginRewards.AddAsync(reward);  // Stage new entity for insertion in the next SaveChanges call
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return reward;
        }

        // Persists state modifications to the database for update daily login reward.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching DailyLoginReward entity result or default if not found.
        public async Task<DailyLoginReward> UpdateDailyLoginReward(DailyLoginReward reward)
        {
            _context.DailyLoginRewards.Update(reward);
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return reward;
        }

        // Persists state modifications to the database for delete daily login reward.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        public async Task DeleteDailyLoginReward(int id)
        {
            var reward = await _context.DailyLoginRewards.FindAsync(id);
            if (reward != null)  // Entity exists — proceed with conditional branch
            {
                reward.IsActive = false;
                _context.DailyLoginRewards.Update(reward);
                await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            }
        }
    }
}
