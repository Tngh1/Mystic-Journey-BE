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
    // Queries the database to retrieve i purchase history repository records.
    public class PurchaseHistoryRepository : IPurchaseHistoryRepository
    {
        private readonly MysticJourneyDbContext _context;

        // Initializes a new instance of PurchaseHistoryRepository with dependencies: context.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public PurchaseHistoryRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }


        // Queries the database to retrieve get total transactions count records.
        // Returns the computed numeric count or database ID result.
        public async Task<int> GetTotalTransactionsCount()
        {
            return await _context.PurchaseHistories.CountAsync();
        }

        // Persists state modifications to the database for get total revenue.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching decimal entity result or default if not found.
        public async Task<decimal> GetTotalRevenue()
        {
            return await _context.PurchaseHistories.SumAsync(p => p.TotalPrice);
        }


        // Performs database query and transactional persistence workflow for create purchase history.
        // Query details: eagerly loads related entity navigation properties; commits entity state changes via EF Core SaveChangesAsync; sorts records according to business ordering rules.
        // Returns the matching PurchaseHistory entity result or default if not found.
        public async Task<PurchaseHistory> CreatePurchaseHistory(PurchaseHistory history)
        {
            await _context.PurchaseHistories.AddAsync(history);  // Stage new entity for insertion in the next SaveChanges call
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return history;
        }

        // Load all purchase histories; it orders the resulting records and materializes the query results.
        public async Task<List<PurchaseHistory>> GetAllPurchaseHistories()
        {
            return await _context.PurchaseHistories
                .Include(p => p.PlayerProfile)  // Eagerly load related navigation entities to avoid N+1 queries
                .Include(p => p.ShopItem)  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(s => s!.Item)
                .OrderByDescending(p => p.PurchasedAt)  // Sort results newest/highest first
                .ToListAsync();  // Materialize the query into a list from the database
        }

        // Queries the database to retrieve get purchases by player id records.
        // Query details: uses AsNoTracking() for read-only query optimization; eagerly loads related entity navigation properties; sorts records according to business ordering rules; applies pagination offset and limit parameters.
        // Returns the matching List<PurchaseHistory entity result or default if not found.
        public async Task<List<PurchaseHistory>> GetPurchasesByPlayerId(int playerProfileId)
        {
            return await _context.PurchaseHistories
                .Include(p => p.PlayerProfile)  // Eagerly load related navigation entities to avoid N+1 queries
                .Include(p => p.ShopItem)  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(s => s!.Item)
                .Where(p => p.PlayerProfileId == playerProfileId)  // Filter records matching the predicate
                .OrderByDescending(p => p.PurchasedAt)  // Sort results newest/highest first
                .ToListAsync();  // Materialize the query into a list from the database
        }

        // Queries the database to retrieve get purchase histories paged records.
        // Query details: uses AsNoTracking() for read-only query optimization; eagerly loads related entity navigation properties; sorts records according to business ordering rules; applies pagination offset and limit parameters.
        public async Task<(int TotalCount, List<PurchaseHistory> Histories)> GetPurchaseHistoriesPaged(int page, int pageSize, string? search, string? sortBy = null, string? sortOrder = null)
        {
            var query = _context.PurchaseHistories
                .Include(p => p.PlayerProfile)  // Eagerly load related navigation entities to avoid N+1 queries
                .Include(p => p.ShopItem)  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(s => s!.Item)
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking();  // Disable EF Core change tracking for this read-only query

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p =>  // Filter records matching the predicate
                    (p.PlayerProfile != null && p.PlayerProfile.DisplayName.Contains(search)) ||
                    (p.ShopItem != null && p.ShopItem.Item != null && p.ShopItem.Item.Name.Contains(search)) ||
                    (p.ShopItem != null && p.ShopItem.Currency.Contains(search)));
            }

            bool desc = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);
            query = (sortBy?.ToLowerInvariant()) switch
            {
                "playername" => desc ? query.OrderByDescending(x => x.PlayerProfile!.DisplayName) : query.OrderBy(x => x.PlayerProfile!.DisplayName),  // Sort results newest/highest first
                "itemname" => desc ? query.OrderByDescending(x => x.ShopItem!.Item!.Name) : query.OrderBy(x => x.ShopItem!.Item!.Name),  // Sort results newest/highest first
                "currency" => desc ? query.OrderByDescending(x => x.ShopItem!.Currency) : query.OrderBy(x => x.ShopItem!.Currency),  // Sort results newest/highest first
                "pricepaid" => desc ? query.OrderByDescending(x => x.TotalPrice) : query.OrderBy(x => x.TotalPrice),  // Sort results newest/highest first
                "purchasedat" => desc ? query.OrderByDescending(x => x.PurchasedAt) : query.OrderBy(x => x.PurchasedAt),  // Sort results newest/highest first
                _ => desc ? query.OrderByDescending(x => x.PurchasedAt) : query.OrderBy(x => x.PurchasedAt),  // Sort results newest/highest first
            };

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)  // Apply pagination offset — skip already-seen records
                .Take(pageSize)  // Apply pagination limit — cap result set size
                .ToListAsync();  // Materialize the query into a list from the database

            return (totalCount, items);
        }
    }
}
