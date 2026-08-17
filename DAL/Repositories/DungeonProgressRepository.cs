using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    // Queries the database to retrieve i dungeon progress repository records.
    public class DungeonProgressRepository : IDungeonProgressRepository
    {
        private readonly MysticJourneyDbContext _context;

        // Initializes a new instance of DungeonProgressRepository with dependencies: context.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public DungeonProgressRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        // Performs database query and transactional persistence workflow for get by session id.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching DungeonProgress? entity result or default if not found.
        public async Task<DungeonProgress?> GetBySessionId(int sessionId)
        {
            return await _context.DungeonProgresses
                .FirstOrDefaultAsync(p => p.DungeonSessionId == sessionId);  // Fetch single matching record or null if not found
        }

        // Persists state modifications to the database for create.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching DungeonProgress entity result or default if not found.
        public async Task<DungeonProgress> Create(DungeonProgress progress)
        {
            progress.CreatedAt = DateTime.UtcNow;
            await _context.DungeonProgresses.AddAsync(progress);  // Stage new entity for insertion in the next SaveChanges call
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return progress;
        }

        // Per-frame update loop for DungeonProgressRepository.
        // Handles real-time input polling, smooth interpolations, cooldown timers, and UI updates.
        public async Task<DungeonProgress> Update(DungeonProgress progress)
        {
            progress.UpdatedAt = DateTime.UtcNow;
            _context.DungeonProgresses.Update(progress);
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return progress;
        }
    }
}
