using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    // Queries the database to retrieve i dungeon session repository records.
    public class DungeonSessionRepository : IDungeonSessionRepository
    {
        private readonly MysticJourneyDbContext _context;

        // Initializes a new instance of DungeonSessionRepository with dependencies: context.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public DungeonSessionRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }


        // Queries the database to retrieve get by id records.
        // Query details: eagerly loads related entity navigation properties; sorts records according to business ordering rules.
        // Returns the matching DungeonSession? entity result or default if not found.
        public async Task<DungeonSession?> GetById(int sessionId)
        {
            return await _context.DungeonSessions
                .Include(s => s.DungeonConfig)  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(d => d!.Chest)
                        .ThenInclude(c => c!.ChestItems)
                            .ThenInclude(ci => ci.Item)
                .Include(s => s.Progress)  // Eagerly load related navigation entities to avoid N+1 queries
                .FirstOrDefaultAsync(s => s.DungeonSessionId == sessionId);  // Fetch single matching record or null if not found
        }

        // Queries the database to retrieve get by player profile id records.
        // Query details: eagerly loads related entity navigation properties; sorts records according to business ordering rules.
        public async Task<System.Collections.Generic.List<DungeonSession>> GetByPlayerProfileId(int playerProfileId)
        {
            return await _context.DungeonSessions
                .Include(s => s.DungeonConfig)  // Eagerly load related navigation entities to avoid N+1 queries
                .Include(s => s.Progress)  // Eagerly load related navigation entities to avoid N+1 queries
                .Where(s => s.PlayerProfileId == playerProfileId)  // Filter records matching the predicate
                .OrderByDescending(s => s.EnterTime)  // Sort results newest/highest first
                .ToListAsync();  // Materialize the query into a list from the database
        }

        // Queries the database to retrieve get active session records.
        // Query details: eagerly loads related entity navigation properties; sorts records according to business ordering rules.
        // Returns the matching DungeonSession? entity result or default if not found.
        public async Task<DungeonSession?> GetActiveSession(int playerProfileId, int? dungeonConfigId = null)
        {
            var query = _context.DungeonSessions.Where(s => s.PlayerProfileId == playerProfileId && s.Status == "Active");  // Filter records matching the predicate

            if (dungeonConfigId.HasValue)
            {
                query = query.Where(s => s.DungeonConfigId == dungeonConfigId.Value);  // Filter records matching the predicate
            }

            return await query
                .Include(s => s.DungeonConfig)  // Eagerly load related navigation entities to avoid N+1 queries
                .Include(s => s.Progress)  // Eagerly load related navigation entities to avoid N+1 queries
                .OrderByDescending(s => s.EnterTime)  // Sort results newest/highest first
                .FirstOrDefaultAsync();  // Fetch single matching record or null if not found
        }

        // Persists state modifications to the database for fail active sessions.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the computed numeric count or database ID result.
        public Task<int> FailActiveSessions(int playerProfileId)
        {
            return _context.DungeonSessions
                .Where(s => s.PlayerProfileId == playerProfileId && s.Status == "Active")  // Filter records matching the predicate
                // Apply this bulk change directly in the database without loading every affected entity.
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.Status, "Failed")
                    .SetProperty(x => x.UpdatedAt, DateTime.UtcNow));
        }


        // Persists state modifications to the database for create.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching DungeonSession entity result or default if not found.
        public async Task<DungeonSession> Create(DungeonSession session)
        {
            session.CreatedAt = DateTime.UtcNow;
            await _context.DungeonSessions.AddAsync(session);  // Stage new entity for insertion in the next SaveChanges call
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return session;
        }

        // Per-frame update loop for DungeonSessionRepository.
        // Handles real-time input polling, smooth interpolations, cooldown timers, and UI updates.
        public async Task<DungeonSession> Update(DungeonSession session)
        {
            session.UpdatedAt = DateTime.UtcNow;
            _context.DungeonSessions.Update(session);
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return session;
        }
    }
}
