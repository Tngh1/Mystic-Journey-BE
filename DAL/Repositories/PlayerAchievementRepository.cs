using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    // Queries the database to retrieve i player achievement repository records.
    public class PlayerAchievementRepository : IPlayerAchievementRepository
    {
        private readonly MysticJourneyDbContext _context;

        // Initializes a new instance of PlayerAchievementRepository with dependencies: context.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public PlayerAchievementRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        // Queries the database to retrieve get by player profile id records.
        // Query details: eagerly loads related entity navigation properties.
        // Returns the matching List<PlayerAchievement entity result or default if not found.
        public async Task<List<PlayerAchievement>> GetByPlayerProfileId(int playerProfileId)
        {
            return await _context.PlayerAchievements
                .Include(pa => pa.Achievement)  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(a => a!.RewardItem)
                .Where(pa => pa.PlayerProfileId == playerProfileId)  // Filter records matching the predicate
                .ToListAsync();  // Materialize the query into a list from the database
        }

        // Performs database query and transactional persistence workflow for get by id with achievement.
        // Query details: eagerly loads related entity navigation properties; commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching PlayerAchievement? entity result or default if not found.
        public async Task<PlayerAchievement?> GetByIdWithAchievement(int playerAchievementId)
        {
            return await _context.PlayerAchievements
                .Include(pa => pa.Achievement)  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(a => a!.RewardItem)
                .FirstOrDefaultAsync(pa => pa.PlayerAchievementId == playerAchievementId);  // Fetch single matching record or null if not found
        }

        // Per-frame update loop for PlayerAchievementRepository.
        // Handles real-time input polling, smooth interpolations, cooldown timers, and UI updates.
        public async Task<PlayerAchievement> Update(PlayerAchievement playerAchievement)
        {
            _context.PlayerAchievements.Update(playerAchievement);
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return playerAchievement;
        }

        // Persists state modifications to the database for update range.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        public async Task UpdateRange(IEnumerable<PlayerAchievement> achievements)
        {
            _context.PlayerAchievements.UpdateRange(achievements);
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
        }

        // Queries the database to retrieve add range records.
        public async Task AddRange(IEnumerable<PlayerAchievement> achievements)
        {
            await _context.PlayerAchievements.AddRangeAsync(achievements);
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
        }
    }
}
