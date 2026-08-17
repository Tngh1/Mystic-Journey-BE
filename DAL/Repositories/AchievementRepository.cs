using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    // Queries the database to retrieve i achievement repository records.
    public class AchievementRepository : IAchievementRepository
    {
        private readonly MysticJourneyDbContext _context;

        // Initializes a new instance of AchievementRepository with dependencies: context.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public AchievementRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }


        // Queries the database to retrieve get achievement by id records.
        // Query details: eagerly loads related entity navigation properties.
        // Returns the matching Achievement? entity result or default if not found.
        public async Task<Achievement?> GetAchievementById(int id)
        {
            return await _context.Achievements
                .FirstOrDefaultAsync(a => a.AchievementId == id);  // Fetch single matching record or null if not found
        }

        // Queries the database to retrieve get achievement by id with reward records.
        // Query details: eagerly loads related entity navigation properties.
        // Returns the matching Achievement? entity result or default if not found.
        public async Task<Achievement?> GetAchievementByIdWithReward(int id)
        {
            return await _context.Achievements
                .Include(a => a.RewardItem)  // Eagerly load related navigation entities to avoid N+1 queries
                .FirstOrDefaultAsync(a => a.AchievementId == id);  // Fetch single matching record or null if not found
        }

        // Load all active achievements; it filters the eligible records and materializes the query results.
        public async Task<List<Achievement>> GetAllActiveAchievements()
        {
            return await _context.Achievements
                .Where(a => a.IsActive)  // Filter records matching the predicate
                .ToListAsync();  // Materialize the query into a list from the database
        }


        // Performs database query and transactional persistence workflow for update achievement.
        // Query details: uses AsNoTracking() for read-only query optimization; eagerly loads related entity navigation properties; commits entity state changes via EF Core SaveChangesAsync; sorts records according to business ordering rules; applies pagination offset and limit parameters.
        // Returns the matching Achievement entity result or default if not found.
        public async Task<Achievement> UpdateAchievement(Achievement achievement)
        {
            _context.Achievements.Update(achievement);
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return achievement;
        }


        // Queries the database to retrieve get achievements paged records.
        // Query details: uses AsNoTracking() for read-only query optimization; eagerly loads related entity navigation properties; sorts records according to business ordering rules; applies pagination offset and limit parameters.
        public async Task<(int TotalCount, List<Achievement> Items)> GetAchievementsPaged(int page, int pageSize, string? search, string? type, bool? isActive, string? sortBy = null, string? sortOrder = null)
        {
            var query = _context.Achievements
                .Include(a => a.RewardItem)  // Eagerly load related navigation entities to avoid N+1 queries
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking();  // Disable EF Core change tracking for this read-only query

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(a => a.Name.Contains(search));  // Filter records matching the predicate
            }
            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(a => a.Type == type);  // Filter records matching the predicate
            }
            if (isActive.HasValue)
            {
                query = query.Where(a => a.IsActive == isActive.Value);  // Filter records matching the predicate
            }

            bool desc = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);
            query = (sortBy?.ToLowerInvariant()) switch
            {
                "name" => desc ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),  // Sort results newest/highest first
                "type" => desc ? query.OrderByDescending(x => x.Type) : query.OrderBy(x => x.Type),  // Sort results newest/highest first
                "requiredvalue" => desc ? query.OrderByDescending(x => x.RequiredValue) : query.OrderBy(x => x.RequiredValue),  // Sort results newest/highest first
                "rewardgold" => desc ? query.OrderByDescending(x => x.RewardGold) : query.OrderBy(x => x.RewardGold),  // Sort results newest/highest first
                "rewardgems" => desc ? query.OrderByDescending(x => x.RewardGem) : query.OrderBy(x => x.RewardGem),  // Sort results newest/highest first
                "isactive" => desc ? query.OrderByDescending(x => x.IsActive) : query.OrderBy(x => x.IsActive),  // Sort results newest/highest first
                _ => desc ? query.OrderByDescending(x => x.AchievementId) : query.OrderBy(x => x.AchievementId),  // Sort results newest/highest first
            };

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();  // Materialize the query into a list from the database

            return (totalCount, items);
        }
    }
}
