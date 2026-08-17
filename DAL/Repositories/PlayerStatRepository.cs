using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    // Queries the database to retrieve i player stat repository records.
    public class PlayerStatRepository : IPlayerStatRepository
    {
        private readonly MysticJourneyDbContext _context;

        // Initializes a new instance of PlayerStatRepository with dependencies: context.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public PlayerStatRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }


        // Performs database query and transactional persistence workflow for get by player profile id.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching PlayerStat? entity result or default if not found.
        public async Task<PlayerStat?> GetByPlayerProfileId(int playerProfileId)
        {
            return await _context.PlayerStats
                .FirstOrDefaultAsync(s => s.PlayerProfileId == playerProfileId);  // Fetch single matching record or null if not found
        }

        // Persists state modifications to the database for create.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching PlayerStat entity result or default if not found.
        public async Task<PlayerStat> Create(PlayerStat stat)
        {
            stat.CreatedAt = DateTime.UtcNow;
            await _context.PlayerStats.AddAsync(stat);  // Stage new entity for insertion in the next SaveChanges call
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return stat;
        }

        // Per-frame update loop for PlayerStatRepository.
        // Handles real-time input polling, smooth interpolations, cooldown timers, and UI updates.
        public async Task<PlayerStat> Update(PlayerStat stat)
        {
            stat.UpdatedAt = DateTime.UtcNow;
            _context.PlayerStats.Update(stat);
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return stat;
        }


        // Performs database query and transactional persistence workflow for get snapshot by player profile id.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching PlayerStatsSnapshot? entity result or default if not found.
        public async Task<PlayerStatsSnapshot?> GetSnapshotByPlayerProfileId(int playerProfileId)
        {
            return await _context.PlayerStatsSnapshots
                .FirstOrDefaultAsync(s => s.PlayerProfileId == playerProfileId);  // Fetch single matching record or null if not found
        }

        // Persists state modifications to the database for create snapshot.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching PlayerStatsSnapshot entity result or default if not found.
        public async Task<PlayerStatsSnapshot> CreateSnapshot(PlayerStatsSnapshot snapshot)
        {
            await _context.PlayerStatsSnapshots.AddAsync(snapshot);  // Stage new entity for insertion in the next SaveChanges call
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return snapshot;
        }

        // Persists state modifications to the database for update snapshot.
        // Returns the matching PlayerStatsSnapshot entity result or default if not found.
        public async Task<PlayerStatsSnapshot> UpdateSnapshot(PlayerStatsSnapshot snapshot)
        {
            _context.PlayerStatsSnapshots.Update(snapshot);
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return snapshot;
        }
    }
}
