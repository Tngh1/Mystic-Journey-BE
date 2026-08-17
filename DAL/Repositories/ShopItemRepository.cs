using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    // Queries the database to retrieve i shop item repository records.
    public class ShopItemRepository : IShopItemRepository
    {
        private readonly MysticJourneyDbContext _context;

        // Initializes a new instance of ShopItemRepository with dependencies: context.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public ShopItemRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        // Queries the database to retrieve get shop item by id records.
        // Query details: eagerly loads related entity navigation properties.
        // Returns the matching ShopItem? entity result or default if not found.
        public async Task<ShopItem?> GetShopItemById(int id)
        {
            return await _context.ShopItems
                .FirstOrDefaultAsync(s => s.ShopItemId == id);  // Fetch single matching record or null if not found
        }

        // Performs database query and transactional persistence workflow for get shop item by id with item.
        // Query details: eagerly loads related entity navigation properties; commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching ShopItem? entity result or default if not found.
        public async Task<ShopItem?> GetShopItemByIdWithItem(int id)
        {
            return await _context.ShopItems
                .Include(s => s.Item)  // Eagerly load related navigation entities to avoid N+1 queries
                .FirstOrDefaultAsync(s => s.ShopItemId == id);  // Fetch single matching record or null if not found
        }

        // Persists state modifications to the database for create shop item.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching ShopItem entity result or default if not found.
        public async Task<ShopItem> CreateShopItem(ShopItem shopItem)
        {
            await _context.ShopItems.AddAsync(shopItem);  // Stage new entity for insertion in the next SaveChanges call
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return shopItem;
        }

        // Performs database query and transactional persistence workflow for update shop item.
        // Query details: uses AsNoTracking() for read-only query optimization; eagerly loads related entity navigation properties; commits entity state changes via EF Core SaveChangesAsync; sorts records according to business ordering rules; applies pagination offset and limit parameters.
        // Returns the matching ShopItem entity result or default if not found.
        public async Task<ShopItem> UpdateShopItem(ShopItem shopItem)
        {
            _context.ShopItems.Update(shopItem);
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return shopItem;
        }

        // Process the supplied values: maps the input discriminator to the corresponding domain value and fallback.
        public async Task<(int TotalCount, List<ShopItem> Items)> GetShopItemsPaged(
            int page,
            int pageSize,
            string? search,
            // Supported currencies: Gold or Gems; the selected currency determines which player balance is charged or credited.
            string? currency,
            string? shopSection,
            bool? isActive,
            string? sortBy = null,
            string? sortOrder = null)
        {
            page = Math.Max(1, page);
            // Clamp the calculated value to the minimum and maximum accepted by this domain rule.
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _context.ShopItems
                .Include(s => s.Item)  // Eagerly load related navigation entities to avoid N+1 queries
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking();  // Disable EF Core change tracking for this read-only query

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Item != null && x.Item.Name.Contains(search));  // Filter records matching the predicate
            }

            if (!string.IsNullOrEmpty(currency))
            {
                query = query.Where(x => x.Currency == currency);  // Filter records matching the predicate
            }

            if (!string.IsNullOrEmpty(shopSection))
            {
                query = query.Where(x => x.ShopSection == shopSection);  // Filter records matching the predicate
            }

            if (isActive.HasValue)
            {
                query = query.Where(x => x.IsActive == isActive.Value);  // Filter records matching the predicate
            }

            bool desc = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);
            query = (sortBy?.ToLowerInvariant()) switch
            {
                "name" => desc ? query.OrderByDescending(x => x.Item!.Name) : query.OrderBy(x => x.Item!.Name),  // Sort results newest/highest first
                "shopsection" => desc ? query.OrderByDescending(x => x.ShopSection) : query.OrderBy(x => x.ShopSection),  // Sort results newest/highest first
                "currency" => desc ? query.OrderByDescending(x => x.Currency) : query.OrderBy(x => x.Currency),  // Sort results newest/highest first
                "price" => desc ? query.OrderByDescending(x => x.Price) : query.OrderBy(x => x.Price),  // Sort results newest/highest first
                "stock" => desc ? query.OrderByDescending(x => x.Stock) : query.OrderBy(x => x.Stock),  // Sort results newest/highest first
                "isactive" => desc ? query.OrderByDescending(x => x.IsActive) : query.OrderBy(x => x.IsActive),  // Sort results newest/highest first
                _ => desc ? query.OrderByDescending(x => x.ShopItemId) : query.OrderBy(x => x.ShopItemId),  // Sort results newest/highest first
            };

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();  // Materialize the query into a list from the database

            return (totalCount, items);
        }
    }
}
