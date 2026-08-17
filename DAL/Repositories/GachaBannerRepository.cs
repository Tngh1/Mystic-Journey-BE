using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    // Queries the database to retrieve i gacha banner repository records.
    public class GachaBannerRepository : IGachaBannerRepository
    {
        private readonly MysticJourneyDbContext _context;

        // Initializes a new instance of GachaBannerRepository with dependencies: context.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public GachaBannerRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        // Queries the database to retrieve get gacha banner by id records.
        // Query details: eagerly loads related entity navigation properties.
        // Returns the matching GachaBanner? entity result or default if not found.
        public async Task<GachaBanner?> GetGachaBannerById(int id)
        {
            return await _context.GachaBanners
                .FirstOrDefaultAsync(b => b.GachaBannerId == id);  // Fetch single matching record or null if not found
        }

        // Performs database query and transactional persistence workflow for get gacha banner by id with items.
        // Query details: eagerly loads related entity navigation properties; commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching GachaBanner? entity result or default if not found.
        public async Task<GachaBanner?> GetGachaBannerByIdWithItems(int id)
        {
            return await _context.GachaBanners
                .Include(b => b.BannerItems)  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(i => i.Item)
                .FirstOrDefaultAsync(b => b.GachaBannerId == id);  // Fetch single matching record or null if not found
        }

        // Persists state modifications to the database for create gacha banner.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching GachaBanner entity result or default if not found.
        public async Task<GachaBanner> CreateGachaBanner(GachaBanner banner)
        {
            await _context.GachaBanners.AddAsync(banner);  // Stage new entity for insertion in the next SaveChanges call
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return banner;
        }

        // Persists state modifications to the database for update gacha banner.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching GachaBanner entity result or default if not found.
        public async Task<GachaBanner> UpdateGachaBanner(GachaBanner banner)
        {
_context.GachaBanners.Update(banner);
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return banner;
        }


        // Performs database query and transactional persistence workflow for create banner item.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching GachaBannerItem entity result or default if not found.
        public async Task<GachaBannerItem> CreateBannerItem(GachaBannerItem item)
        {
            await _context.GachaBannerItems.AddAsync(item);  // Stage new entity for insertion in the next SaveChanges call
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            await _context.Entry(item).Reference(i => i.Item).LoadAsync();
            return item;
        }

        // Performs database query and transactional persistence workflow for remove banner item.
        // Query details: eagerly loads related entity navigation properties; commits entity state changes via EF Core SaveChangesAsync.
        // Returns true if the operation succeeded or record exists; otherwise false.
        public async Task<bool> RemoveBannerItem(int bannerId, int bannerItemId)
        {
            var item = await _context.GachaBannerItems
                .FirstOrDefaultAsync(i => i.GachaBannerItemId == bannerItemId && i.GachaBannerId == bannerId);  // Fetch single matching record or null if not found
            if (item == null) return false;  // Entity not found — short-circuit with appropriate error result
            _context.GachaBannerItems.Remove(item);  // Mark entity for deletion in the next SaveChanges call
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return true;
        }

        // Queries the database to retrieve get banner items records.
        // Query details: uses AsNoTracking() for read-only query optimization; eagerly loads related entity navigation properties; sorts records according to business ordering rules; applies pagination offset and limit parameters.
        // Returns the matching List<GachaBannerItem entity result or default if not found.
        public async Task<List<GachaBannerItem>> GetBannerItems(int bannerId)
        {
            return await _context.GachaBannerItems
                .Include(i => i.Item)  // Eagerly load related navigation entities to avoid N+1 queries
                .Where(i => i.GachaBannerId == bannerId)  // Filter records matching the predicate
                .ToListAsync();  // Materialize the query into a list from the database
        }


        // Queries the database to retrieve get banners paged records.
        // Query details: uses AsNoTracking() for read-only query optimization; eagerly loads related entity navigation properties; sorts records according to business ordering rules; applies pagination offset and limit parameters.
        public async Task<(int TotalCount, List<GachaBanner> Items)> GetBannersPaged(int page, int pageSize, string? search, string? type, bool? isActive, string? sortBy = null, string? sortOrder = null)
        {
            var query = _context.GachaBanners
                .Include(b => b.BannerItems)  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(bi => bi.Item)
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking();  // Disable EF Core change tracking for this read-only query

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Name.Contains(search));  // Filter records matching the predicate
            }
            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(x => x.Type == type);  // Filter records matching the predicate
            }
            if (isActive.HasValue)
            {
                query = query.Where(x => x.IsActive == isActive.Value);  // Filter records matching the predicate
            }

            bool desc = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);
            query = (sortBy?.ToLowerInvariant()) switch
            {
                "name" => desc ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),  // Sort results newest/highest first
                "type" => desc ? query.OrderByDescending(x => x.Type) : query.OrderBy(x => x.Type),  // Sort results newest/highest first
                "pullcost" => desc ? query.OrderByDescending(x => x.PullCost) : query.OrderBy(x => x.PullCost),  // Sort results newest/highest first
                "startdate" => desc ? query.OrderByDescending(x => x.StartAt) : query.OrderBy(x => x.StartAt),  // Sort results newest/highest first
                "enddate" => desc ? query.OrderByDescending(x => x.EndAt) : query.OrderBy(x => x.EndAt),  // Sort results newest/highest first
                "isactive" => desc ? query.OrderByDescending(x => x.IsActive) : query.OrderBy(x => x.IsActive),  // Sort results newest/highest first
                _ => desc ? query.OrderByDescending(x => x.GachaBannerId) : query.OrderBy(x => x.GachaBannerId),  // Sort results newest/highest first
            };

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();  // Materialize the query into a list from the database

            return (totalCount, items);
        }

        // Performs database query and transactional persistence workflow for get banner items paged.
        // Query details: uses AsNoTracking() for read-only query optimization; eagerly loads related entity navigation properties; commits entity state changes via EF Core SaveChangesAsync; applies pagination offset and limit parameters.
        public async Task<(int TotalCount, List<GachaBannerItem> Items)> GetBannerItemsPaged(int page, int pageSize)
        {
            var query = _context.GachaBannerItems
                .Include(bi => bi.Item)  // Eagerly load related navigation entities to avoid N+1 queries
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking();  // Disable EF Core change tracking for this read-only query

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();  // Materialize the query into a list from the database

            return (totalCount, items);
        }

        // Performs database query and transactional persistence workflow for add gacha pull history.
        // Query details: commits entity state changes via EF Core SaveChangesAsync; sorts records according to business ordering rules.
        // Returns the matching GachaPullHistory entity result or default if not found.
        public async Task<GachaPullHistory> AddGachaPullHistory(GachaPullHistory history)
        {
            await _context.GachaPullHistories.AddAsync(history);  // Stage new entity for insertion in the next SaveChanges call
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return history;
        }

        // Queries the database to retrieve get pull history by player and banner records.
        // Query details: uses AsNoTracking() for read-only query optimization; eagerly loads related entity navigation properties; sorts records according to business ordering rules; applies pagination offset and limit parameters.
        // Returns the matching List<GachaPullHistory entity result or default if not found.
        public async Task<List<GachaPullHistory>> GetPullHistoryByPlayerAndBanner(int playerProfileId, int bannerId)
        {
            return await _context.GachaPullHistories
                .Where(h => h.PlayerProfileId == playerProfileId && h.GachaBannerId == bannerId)  // Filter records matching the predicate
                .OrderByDescending(h => h.PulledAt)  // Sort results newest/highest first
                .ThenByDescending(h => h.GachaPullHistoryId)
                .ToListAsync();  // Materialize the query into a list from the database
        }

        // Queries the database to retrieve get gacha pull history paged records.
        // Query details: uses AsNoTracking() for read-only query optimization; eagerly loads related entity navigation properties; sorts records according to business ordering rules; applies pagination offset and limit parameters.
        public async Task<(int TotalCount, List<GachaPullHistory> Items)> GetGachaPullHistoryPaged(int playerProfileId, int page, int pageSize)
        {
            var query = _context.GachaPullHistories
                .Include(h => h.GachaBanner)  // Eagerly load related navigation entities to avoid N+1 queries
                .Include(h => h.RewardItem)  // Eagerly load related navigation entities to avoid N+1 queries
                .Where(h => h.PlayerProfileId == playerProfileId)  // Filter records matching the predicate
                .OrderByDescending(h => h.PulledAt)  // Sort results newest/highest first
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking();  // Disable EF Core change tracking for this read-only query

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();  // Materialize the query into a list from the database

            return (totalCount, items);
        }

        // Queries the database to retrieve get all gacha pull history paged records.
        // Query details: uses AsNoTracking() for read-only query optimization; eagerly loads related entity navigation properties; sorts records according to business ordering rules; applies pagination offset and limit parameters.
        public async Task<(int TotalCount, List<GachaPullHistory> Items)> GetAllGachaPullHistoryPaged(int page, int pageSize, int? bannerId, string? rarity)
        {
            var query = _context.GachaPullHistories
                .Include(h => h.GachaBanner)  // Eagerly load related navigation entities to avoid N+1 queries
                .Include(h => h.RewardItem)  // Eagerly load related navigation entities to avoid N+1 queries
                .Include(h => h.PlayerProfile)  // Eagerly load related navigation entities to avoid N+1 queries
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking();  // Disable EF Core change tracking for this read-only query

            if (bannerId.HasValue)
                query = query.Where(h => h.GachaBannerId == bannerId.Value);  // Filter records matching the predicate

            if (!string.IsNullOrEmpty(rarity))
                query = query.Where(h => h.RewardItem != null && h.RewardItem.Rarity == rarity);  // Filter records matching the predicate

            query = query.OrderByDescending(h => h.PulledAt);  // Sort results newest/highest first

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();  // Materialize the query into a list from the database
            return (totalCount, items);
        }

        // Queries the database to retrieve get player gacha stats async records.
        public async Task<(int TotalPulls, decimal TotalCost, int LegendaryPulls, string PlayerName, int AccountId)?> GetPlayerGachaStatsAsync(int playerProfileId)
        {
            var profile = await _context.PlayerProfiles
                .Where(p => p.PlayerProfileId == playerProfileId)  // Filter records matching the predicate
                .Select(p => new { p.DisplayName, p.AccountId })
                .FirstOrDefaultAsync();  // Fetch single matching record or null if not found

            if (profile == null) return null;  // Entity not found — short-circuit with appropriate error result

            var histories = await _context.GachaPullHistories
                .Include(h => h.RewardItem)  // Eagerly load related navigation entities to avoid N+1 queries
                .Where(h => h.PlayerProfileId == playerProfileId)  // Filter records matching the predicate
                .ToListAsync();  // Materialize the query into a list from the database

            int totalPulls = histories.Sum(h => h.PullCount);
            decimal totalCost = histories.Sum(h => h.CostSpent);
            int legendaryPulls = histories.Count(h => h.RewardItem != null && h.RewardItem.Rarity == "Legendary");
            decimal actualRate = totalPulls > 0 ? ((decimal)legendaryPulls / totalPulls) * 100 : 0;

            return (totalPulls, totalCost, legendaryPulls, profile.DisplayName, profile.AccountId);
        }
    }
}
