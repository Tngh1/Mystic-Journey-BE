using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    // Queries the database to retrieve i dungeon config repository records.
    public class DungeonConfigRepository : IDungeonConfigRepository
    {
        private readonly MysticJourneyDbContext _context;

        // Initializes a new instance of DungeonConfigRepository with dependencies: context.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public DungeonConfigRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }


        // Queries the database to retrieve get dungeon config by id records.
        // Query details: eagerly loads related entity navigation properties.
        // Returns the matching DungeonConfig? entity result or default if not found.
        public async Task<DungeonConfig?> GetDungeonConfigById(int id)
        {
            return await _context.DungeonConfigs
                .FirstOrDefaultAsync(d => d.DungeonConfigId == id);  // Fetch single matching record or null if not found
        }

        // Queries the database to retrieve get by id with chest records.
        // Query details: eagerly loads related entity navigation properties.
        // Returns the matching DungeonConfig? entity result or default if not found.
        public async Task<DungeonConfig?> GetByIdWithChest(int id)
        {
            return await _context.DungeonConfigs
                .Include(d => d.Chest)  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(c => c!.ChestItems)
                        .ThenInclude(ci => ci.Item)
                .FirstOrDefaultAsync(d => d.DungeonConfigId == id && d.IsActive);  // Fetch single matching record or null if not found
        }

        // Load all dungeon configs; it materializes the query results.
        public async Task<List<DungeonConfig>> GetAllDungeonConfigs()
        {
            return await _context.DungeonConfigs.ToListAsync();  // Materialize the query into a list from the database
        }

        // Load active dungeon configs; it filters the eligible records and materializes the query results.
        public async Task<List<DungeonConfig>> GetActiveDungeonConfigs()
        {
            return await _context.DungeonConfigs
                .Where(d => d.IsActive)  // Filter records matching the predicate
                .ToListAsync();  // Materialize the query into a list from the database
        }

        // Performs database query and transactional persistence workflow for dungeon exists.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns true if the operation succeeded or record exists; otherwise false.
        public async Task<bool> DungeonExists(int dungeonId)
        {
            return await _context.DungeonConfigs.AnyAsync(d => d.DungeonConfigId == dungeonId);  // Check existence without loading the full entity
        }


        // Performs database query and transactional persistence workflow for update dungeon config.
        // Query details: uses AsNoTracking() for read-only query optimization; eagerly loads related entity navigation properties; commits entity state changes via EF Core SaveChangesAsync; sorts records according to business ordering rules; applies pagination offset and limit parameters.
        // Returns the matching DungeonConfig entity result or default if not found.
        public async Task<DungeonConfig> UpdateDungeonConfig(DungeonConfig dungeon)
        {
            _context.DungeonConfigs.Update(dungeon);
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return dungeon;
        }


        // Queries the database to retrieve get dungeons paged records.
        // Query details: uses AsNoTracking() for read-only query optimization; eagerly loads related entity navigation properties; sorts records according to business ordering rules; applies pagination offset and limit parameters.
        public async Task<(int TotalCount, List<DungeonConfig> Items)> GetDungeonsPaged(int page, int pageSize, string? search, string? type, bool? isActive, string? sortBy = null, string? sortOrder = null)
        {
            // Execute this query without change tracking because the returned entities are read-only.
            var query = _context.DungeonConfigs.AsNoTracking()  // Disable EF Core change tracking for this read-only query
                .Include(d => d.Chest)  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(c => c!.ChestItems)
                        .ThenInclude(ci => ci.Item)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d => d.Name.Contains(search));  // Filter records matching the predicate
            }
            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(d => d.Type == type);  // Filter records matching the predicate
            }
            if (isActive.HasValue)
            {
                query = query.Where(d => d.IsActive == isActive.Value);  // Filter records matching the predicate
            }

            bool desc = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);
            query = (sortBy?.ToLowerInvariant()) switch
            {
                "name" => desc ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),  // Sort results newest/highest first
                "type" => desc ? query.OrderByDescending(x => x.Type) : query.OrderBy(x => x.Type),  // Sort results newest/highest first
                "levelrequirement" => desc ? query.OrderByDescending(x => x.LevelRequirement) : query.OrderBy(x => x.LevelRequirement),  // Sort results newest/highest first
                "isactive" => desc ? query.OrderByDescending(x => x.IsActive) : query.OrderBy(x => x.IsActive),  // Sort results newest/highest first
                _ => desc ? query.OrderByDescending(x => x.DungeonConfigId) : query.OrderBy(x => x.DungeonConfigId),  // Sort results newest/highest first
            };

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();  // Materialize the query into a list from the database

            return (totalCount, items);
        }
    }
}
