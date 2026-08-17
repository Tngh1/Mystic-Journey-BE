using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    // Queries the database to retrieve i player profile repository records.
    public class PlayerProfileRepository : IPlayerProfileRepository
    {
        private readonly MysticJourneyDbContext _context;

        // Initializes a new instance of PlayerProfileRepository with dependencies: context.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public PlayerProfileRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }


        // Queries the database to retrieve get player profile by id records.
        // Query details: eagerly loads related entity navigation properties.
        // Returns the matching PlayerProfile? entity result or default if not found.
        public async Task<PlayerProfile?> GetPlayerProfileById(int id)
        {
            return await _context.PlayerProfiles
                .FirstOrDefaultAsync(p => p.PlayerProfileId == id);  // Fetch single matching record or null if not found
        }

        // Queries the database to retrieve get player profile by id with stats records.
        // Query details: eagerly loads related entity navigation properties.
        // Returns the matching PlayerProfile? entity result or default if not found.
        public async Task<PlayerProfile?> GetPlayerProfileByIdWithStats(int id)
        {
            return await _context.PlayerProfiles
                .Include(p => p.PlayerStats)  // Eagerly load related navigation entities to avoid N+1 queries
                .FirstOrDefaultAsync(p => p.PlayerProfileId == id);  // Fetch single matching record or null if not found
        }

        // Queries the database to retrieve get by id full records.
        // Query details: eagerly loads related entity navigation properties.
        // Returns the matching PlayerProfile? entity result or default if not found.
        public async Task<PlayerProfile?> GetByIdFull(int id)
        {
            return await _context.PlayerProfiles
                .Include(p => p.PlayerStats)  // Eagerly load related navigation entities to avoid N+1 queries
                .Include(p => p.Account)  // Eagerly load related navigation entities to avoid N+1 queries
                .FirstOrDefaultAsync(p => p.PlayerProfileId == id);  // Fetch single matching record or null if not found
        }

        // Queries the database to retrieve get by account id records.
        // Query details: eagerly loads related entity navigation properties.
        // Returns the matching PlayerProfile? entity result or default if not found.
        public async Task<PlayerProfile?> GetByAccountId(int accountId)
        {
            return await _context.PlayerProfiles
                .Include(p => p.PlayerStats)  // Eagerly load related navigation entities to avoid N+1 queries
                .Include(p => p.Account)  // Eagerly load related navigation entities to avoid N+1 queries
                .FirstOrDefaultAsync(p => p.AccountId == accountId);  // Fetch single matching record or null if not found
        }

        // Queries the database to retrieve get player profile by name records.
        // Returns the matching PlayerProfile? entity result or default if not found.
        public async Task<PlayerProfile?> GetPlayerProfileByName(string playerName)
        {
            return await _context.PlayerProfiles
                .FirstOrDefaultAsync(p => p.DisplayName == playerName);  // Fetch single matching record or null if not found
        }

        // Load all player profiles; it materializes the query results.
        public async Task<List<PlayerProfile>> GetAllPlayerProfiles()
        {
            return await _context.PlayerProfiles.ToListAsync();  // Materialize the query into a list from the database
        }


        // Persists state modifications to the database for create player profile.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching PlayerProfile entity result or default if not found.
        public async Task<PlayerProfile> CreatePlayerProfile(PlayerProfile profile)
        {
            profile.CreatedAt = DateTime.UtcNow;
            await _context.PlayerProfiles.AddAsync(profile);  // Stage new entity for insertion in the next SaveChanges call
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return profile;
        }

        // Performs database query and transactional persistence workflow for update player profile.
        // Query details: uses AsNoTracking() for read-only query optimization; commits entity state changes via EF Core SaveChangesAsync; sorts records according to business ordering rules; applies pagination offset and limit parameters.
        // Returns the matching PlayerProfile entity result or default if not found.
        public async Task<PlayerProfile> UpdatePlayerProfile(PlayerProfile profile)
        {
            profile.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return profile;
        }


        // Queries the database to retrieve search records.
        // Query details: uses AsNoTracking() for read-only query optimization; sorts records according to business ordering rules; applies pagination offset and limit parameters.
        // Returns the matching List<PlayerProfile entity result or default if not found.
        public async Task<List<PlayerProfile>> Search(string? keyword = null, string? playerClass = null)
        {
            var query = _context.PlayerProfiles
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking();  // Disable EF Core change tracking for this read-only query

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var lowerKeyword = keyword.ToLower();
                query = query.Where(p =>  // Filter records matching the predicate
                    p.DisplayName.ToLower().Contains(lowerKeyword) ||
                    (p.Account != null && p.Account.UserName.ToLower().Contains(lowerKeyword)));
            }

            if (!string.IsNullOrWhiteSpace(playerClass))
            {
                query = query.Where(p => p.Class == playerClass);  // Filter records matching the predicate
            }

            if (string.IsNullOrWhiteSpace(keyword) && string.IsNullOrWhiteSpace(playerClass))  // Mandatory string argument is blank — fail fast
            {
                var total = await _context.PlayerProfiles.CountAsync();
                var skip = total > 10 ? Random.Shared.Next(total - 9) : 0;
                return await query
                    .OrderBy(p => p.PlayerProfileId)  // Sort results oldest/lowest first
                    .Skip(skip)  // Apply pagination offset — skip already-seen records
                    .Take(10)  // Apply pagination limit — cap result set size
                    .ToListAsync();  // Materialize the query into a list from the database
            }

            return await query.Take(20).ToListAsync();  // Materialize the query into a list from the database
        }

        // Queries the database to retrieve get total player profiles count records.
        // Query details: uses AsNoTracking() for read-only query optimization; eagerly loads related entity navigation properties; applies pagination offset and limit parameters.
        // Returns the computed numeric count or database ID result.
        public async Task<int> GetTotalPlayerProfilesCount()
        {
            return await _context.PlayerProfiles.CountAsync();
        }


        // Queries the database to retrieve get profiles paged records.
        // Query details: uses AsNoTracking() for read-only query optimization; eagerly loads related entity navigation properties.
        public async Task<(int TotalCount, List<PlayerProfile> Items)> GetProfilesPaged(int page, int pageSize, string? search, int? level)
        {
            var query = _context.PlayerProfiles
                .Include(p => p.Account)  // Eagerly load related navigation entities to avoid N+1 queries
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking();  // Disable EF Core change tracking for this read-only query

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.DisplayName.Contains(search));  // Filter records matching the predicate
            }
            if (level.HasValue)
            {
                query = query.Where(x => x.Level == level.Value);  // Filter records matching the predicate
            }

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();  // Materialize the query into a list from the database

            return (totalCount, items);
        }
    }
}
