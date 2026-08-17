using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    // Queries the database to retrieve i item repository records.
    public class ItemRepository : IItemRepository
    {
        private readonly MysticJourneyDbContext _context;

        // Initializes a new instance of ItemRepository with dependencies: context.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public ItemRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        // Queries the database to retrieve get item by id records.
        // Returns the matching Item? entity result or default if not found.
        public async Task<Item?> GetItemById(int id)
        {
            return await _context.Items
                .FirstOrDefaultAsync(i => i.ItemId == id);  // Fetch single matching record or null if not found
        }

        // Queries the database to retrieve get total items count records.
        // Query details: uses AsNoTracking() for read-only query optimization; sorts records according to business ordering rules.
        // Returns the computed numeric count or database ID result.
        public async Task<int> GetTotalItemsCount()
        {
            return await _context.Items.CountAsync();
        }

        // Load quest items; it filters the eligible records, orders the resulting records, and materializes the query results.
        public async Task<List<Item>> GetQuestItems()
        {
            return await _context.Items
                .Where(i => i.IsActive && i.Type == "QuestItem")  // Filter records matching the predicate
                .OrderBy(i => i.ItemId)  // Sort results oldest/lowest first
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking()  // Disable EF Core change tracking for this read-only query
                .ToListAsync();  // Materialize the query into a list from the database
        }

        // Queries the database to retrieve get quest item by names records.
        // Query details: eagerly loads related entity navigation properties; sorts records according to business ordering rules.
        // Returns the matching Item? entity result or default if not found.
        public async Task<Item?> GetQuestItemByNames(params string[] names)
        {
            return await _context.Items
                .Where(i => i.IsActive && names.Contains(i.Name))  // Filter records matching the predicate
                .OrderBy(i => i.ItemId)  // Sort results oldest/lowest first
                .FirstOrDefaultAsync();  // Fetch single matching record or null if not found
        }

        // Performs database query and transactional persistence workflow for get item by id with stats.
        // Query details: eagerly loads related entity navigation properties; commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching Item? entity result or default if not found.
        public async Task<Item?> GetItemByIdWithStats(int id)
        {
            return await _context.Items
                .Include(i => i.EquipmentStats)  // Eagerly load related navigation entities to avoid N+1 queries
                .FirstOrDefaultAsync(i => i.ItemId == id);  // Fetch single matching record or null if not found
        }

        // Performs database query and transactional persistence workflow for update item.
        // Query details: uses AsNoTracking() for read-only query optimization; eagerly loads related entity navigation properties; commits entity state changes via EF Core SaveChangesAsync; sorts records according to business ordering rules; applies pagination offset and limit parameters.
        // Returns the matching Item entity result or default if not found.
        public async Task<Item> UpdateItem(Item item)
        {
            _context.Items.Update(item);
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return item;
        }


        // Queries the database to retrieve get items paged records.
        // Query details: uses AsNoTracking() for read-only query optimization; eagerly loads related entity navigation properties; sorts records according to business ordering rules; applies pagination offset and limit parameters.
        public async Task<(int TotalCount, List<Item> Items)> GetItemsPaged(int page, int pageSize, string? search, string? type, string? rarity, bool? isActive, string? sortBy = null, string? sortOrder = null)
        {
            // Execute this query without change tracking because the returned entities are read-only.
            var filtered = _context.Items.AsNoTracking();  // Disable EF Core change tracking for this read-only query

            if (!string.IsNullOrEmpty(search))
            {
                filtered = filtered.Where(x => x.Name.Contains(search));  // Filter records matching the predicate
            }
            if (!string.IsNullOrEmpty(type))
            {
                filtered = filtered.Where(x => x.Type == type);  // Filter records matching the predicate
            }
            if (!string.IsNullOrEmpty(rarity))
            {
                filtered = filtered.Where(x => x.Rarity == rarity);  // Filter records matching the predicate
            }
            if (isActive.HasValue)
            {
                filtered = filtered.Where(x => x.IsActive == isActive.Value);  // Filter records matching the predicate
            }

            int totalCount = await filtered.CountAsync();

            var query = filtered.Include(i => i.EquipmentStats);  // Eagerly load related navigation entities to avoid N+1 queries

            bool desc = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);
            IQueryable<Item> ordered = (sortBy?.ToLowerInvariant()) switch
            {
                "name" => desc ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),  // Sort results newest/highest first
                "type" => desc ? query.OrderByDescending(x => x.Type) : query.OrderBy(x => x.Type),  // Sort results newest/highest first
                "rarity" => desc ? query.OrderByDescending(x => x.Rarity) : query.OrderBy(x => x.Rarity),  // Sort results newest/highest first
                "basevalue" => desc ? query.OrderByDescending(x => x.BaseValue) : query.OrderBy(x => x.BaseValue),  // Sort results newest/highest first
                "isactive" => desc ? query.OrderByDescending(x => x.IsActive) : query.OrderBy(x => x.IsActive),  // Sort results newest/highest first
                _ => desc ? query.OrderByDescending(x => x.ItemId) : query.OrderBy(x => x.ItemId),  // Sort results newest/highest first
            };

            var items = await ordered.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();  // Materialize the query into a list from the database

            return (totalCount, items);
        }
    }
}
