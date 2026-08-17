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
    // Queries the database to retrieve i wiki repository records.
    public class WikiRepository : IWikiRepository
    {
        private readonly MysticJourneyDbContext _context;

        // Initializes a new instance of WikiRepository with dependencies: context.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public WikiRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }


        // Load class configs; it orders the resulting records and materializes the query results.
        public async Task<List<ClassConfig>> GetClassConfigs()
        {
            return await _context.ClassConfigs
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking()  // Disable EF Core change tracking for this read-only query
                .OrderBy(c => c.ClassConfigId)  // Sort results oldest/lowest first
                .ToListAsync();  // Materialize the query into a list from the database
        }


        // Process the supplied values: maps the input discriminator to the corresponding domain value and fallback.
        public async Task<(int TotalCount, List<Monster> Items)> GetMonstersPaged(
            int page, int pageSize, string? search, string? type, string? sortBy, string? sortOrder)
        {
            var filtered = _context.Monsters
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking()  // Disable EF Core change tracking for this read-only query
                .Where(x => x.IsActive);  // Filter records matching the predicate

            if (!string.IsNullOrEmpty(search))
                filtered = filtered.Where(x => x.Name.Contains(search) || x.Description.Contains(search));  // Filter records matching the predicate
            if (!string.IsNullOrEmpty(type))
                filtered = filtered.Where(x => x.Type == type);  // Filter records matching the predicate

            int totalCount = await filtered.CountAsync();

            var query = filtered
                .Include(m => m.MonsterDrops)  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(d => d.Item);

            bool desc = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);
            IQueryable<Monster> ordered = (sortBy?.ToLowerInvariant()) switch
            {
                "name" => desc ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),  // Sort results newest/highest first
                "type" => desc ? query.OrderByDescending(x => x.Type) : query.OrderBy(x => x.Type),  // Sort results newest/highest first
                "level" => desc ? query.OrderByDescending(x => x.Level) : query.OrderBy(x => x.Level),  // Sort results newest/highest first
                "maxhp" => desc ? query.OrderByDescending(x => x.MaxHp) : query.OrderBy(x => x.MaxHp),  // Sort results newest/highest first
                "attack" => desc ? query.OrderByDescending(x => x.Atk) : query.OrderBy(x => x.Atk),  // Sort results newest/highest first
                "defense" => desc ? query.OrderByDescending(x => x.Def) : query.OrderBy(x => x.Def),  // Sort results newest/highest first
                "goldreward" => desc ? query.OrderByDescending(x => x.GoldReward) : query.OrderBy(x => x.GoldReward),  // Sort results newest/highest first
                "expreward" => desc ? query.OrderByDescending(x => x.ExperienceReward) : query.OrderBy(x => x.ExperienceReward),  // Sort results newest/highest first
                _ => desc ? query.OrderByDescending(x => x.Level) : query.OrderBy(x => x.Level),  // Sort results newest/highest first
            };

            var items = await ordered.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();  // Materialize the query into a list from the database

            return (totalCount, items);
        }

        // Queries the database to retrieve get monster by id records.
        // Query details: uses AsNoTracking() for read-only query optimization; eagerly loads related entity navigation properties; sorts records according to business ordering rules; applies pagination offset and limit parameters.
        // Returns the matching Monster? entity result or default if not found.
        public async Task<Monster?> GetMonsterById(int id)
        {
            return await _context.Monsters
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking()  // Disable EF Core change tracking for this read-only query
                .Include(m => m.MonsterDrops)  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(d => d.Item)
                .FirstOrDefaultAsync(m => m.MonsterId == id && m.IsActive);  // Fetch single matching record or null if not found
        }


        // Process the supplied values: maps the input discriminator to the corresponding domain value and fallback.
        public async Task<(int TotalCount, List<Item> Items)> GetItemsPaged(
            int page, int pageSize, string? search, string? type, string? rarity, string? sortBy, string? sortOrder)
        {
            var filtered = _context.Items
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking()  // Disable EF Core change tracking for this read-only query
                .Where(x => x.IsActive);  // Filter records matching the predicate

            if (!string.IsNullOrEmpty(search))
                filtered = filtered.Where(x => x.Name.Contains(search) || (x.Description != null && x.Description.Contains(search)));  // Filter records matching the predicate
            if (!string.IsNullOrEmpty(type))
                filtered = filtered.Where(x => x.Type == type);  // Filter records matching the predicate
            if (!string.IsNullOrEmpty(rarity))
                filtered = filtered.Where(x => x.Rarity == rarity);  // Filter records matching the predicate

            int totalCount = await filtered.CountAsync();

            var query = filtered.Include(i => i.EquipmentStats);  // Eagerly load related navigation entities to avoid N+1 queries

            bool desc = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);
            IQueryable<Item> ordered = (sortBy?.ToLowerInvariant()) switch
            {
                "name" => desc ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),  // Sort results newest/highest first
                "type" => desc ? query.OrderByDescending(x => x.Type) : query.OrderBy(x => x.Type),  // Sort results newest/highest first
                "rarity" => desc
                    ? query.OrderByDescending(x =>  // Sort results newest/highest first
                        x.Rarity == "Common" ? 0
                        : x.Rarity == "Uncommon" ? 1
                        : x.Rarity == "Rare" ? 2
                        : x.Rarity == "Epic" ? 3
                        : x.Rarity == "Legendary" ? 4
                        : x.Rarity == "Mythic" ? 5
                        : 6)
                    : query.OrderBy(x =>  // Sort results oldest/lowest first
                        x.Rarity == "Common" ? 0
                        : x.Rarity == "Uncommon" ? 1
                        : x.Rarity == "Rare" ? 2
                        : x.Rarity == "Epic" ? 3
                        : x.Rarity == "Legendary" ? 4
                        : x.Rarity == "Mythic" ? 5
                        : 6),
                "basevalue" => desc ? query.OrderByDescending(x => x.BaseValue) : query.OrderBy(x => x.BaseValue),  // Sort results newest/highest first
                _ => desc ? query.OrderByDescending(x => x.ItemId) : query.OrderBy(x => x.ItemId),  // Sort results newest/highest first
            };

            var items = await ordered.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();  // Materialize the query into a list from the database

            return (totalCount, items);
        }

        // Queries the database to retrieve get item by id records.
        // Query details: uses AsNoTracking() for read-only query optimization; eagerly loads related entity navigation properties; sorts records according to business ordering rules; applies pagination offset and limit parameters.
        // Returns the matching Item? entity result or default if not found.
        public async Task<Item?> GetItemById(int id)
        {
            return await _context.Items
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking()  // Disable EF Core change tracking for this read-only query
                .Include(i => i.EquipmentStats)  // Eagerly load related navigation entities to avoid N+1 queries
                .FirstOrDefaultAsync(i => i.ItemId == id && i.IsActive);  // Fetch single matching record or null if not found
        }


        // Load skills paged using total count, page, page size, and search; it filters the eligible records, orders the resulting records, and materializes the query results and guards invalid or unavailable states.
        public async Task<(int TotalCount, List<Skill> Items)> GetSkillsPaged(
            int page, int pageSize, string? search, string? type)
        {
            var query = _context.Skills
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking()  // Disable EF Core change tracking for this read-only query
                .Where(s => s.IsActive);  // Filter records matching the predicate

            if (!string.IsNullOrEmpty(search))
                query = query.Where(s => s.Name.Contains(search) || (s.Description != null && s.Description.Contains(search)));  // Filter records matching the predicate
            if (!string.IsNullOrEmpty(type))
                query = query.Where(s => s.Type == type);  // Filter records matching the predicate

            int totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(s => s.UnlockLevel)  // Sort results oldest/lowest first
                .ThenBy(s => s.Name)
                .Skip((page - 1) * pageSize)  // Apply pagination offset — skip already-seen records
                .Take(pageSize)  // Apply pagination limit — cap result set size
                .ToListAsync();  // Materialize the query into a list from the database

            return (totalCount, items);
        }

        // Queries the database to retrieve get skill by id records.
        // Query details: uses AsNoTracking() for read-only query optimization.
        // Returns the matching Skill? entity result or default if not found.
        public async Task<Skill?> GetSkillById(int id)
        {
            return await _context.Skills
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking()  // Disable EF Core change tracking for this read-only query
                .FirstOrDefaultAsync(s => s.SkillId == id && s.IsActive);  // Fetch single matching record or null if not found
        }
    }
}
