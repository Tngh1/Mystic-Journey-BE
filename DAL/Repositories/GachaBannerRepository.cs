using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class GachaBannerRepository : IGachaBannerRepository
    {
        private readonly MysticJourneyDbContext _context;

        public GachaBannerRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<GachaBanner?> GetGachaBannerById(int id)
        {
            return await _context.GachaBanners
                .FirstOrDefaultAsync(b => b.GachaBannerId == id);
        }

        public async Task<GachaBanner?> GetGachaBannerByIdWithItems(int id)
        {
            return await _context.GachaBanners
                .Include(b => b.BannerItems)
                    .ThenInclude(i => i.Item)
                .FirstOrDefaultAsync(b => b.GachaBannerId == id);
        }

        public async Task<GachaBanner> UpdateGachaBanner(GachaBanner banner)
        {
_context.GachaBanners.Update(banner);
            await _context.SaveChangesAsync();
            return banner;
        }


        public async Task<GachaBannerItem> CreateBannerItem(GachaBannerItem item)
        {
            await _context.GachaBannerItems.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<List<GachaBannerItem>> GetBannerItems(int bannerId)
        {
            return await _context.GachaBannerItems
                .Include(i => i.Item)
                .Where(i => i.GachaBannerId == bannerId)
                .ToListAsync();
        }


        public async Task<(int TotalCount, List<GachaBanner> Items)> GetBannersPaged(int page, int pageSize, string? search, string? type, bool? isActive, string? sortBy = null, string? sortOrder = null)
        {
            var query = _context.GachaBanners
                .Include(b => b.BannerItems)
                    .ThenInclude(bi => bi.Item)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Name.Contains(search));
            }
            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(x => x.Type == type);
            }
            if (isActive.HasValue)
            {
                query = query.Where(x => x.IsActive == isActive.Value);
            }

            bool desc = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);
            query = (sortBy?.ToLowerInvariant()) switch
            {
                "name" => desc ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),
                "type" => desc ? query.OrderByDescending(x => x.Type) : query.OrderBy(x => x.Type),
                "pullcost" => desc ? query.OrderByDescending(x => x.PullCost) : query.OrderBy(x => x.PullCost),
                "startdate" => desc ? query.OrderByDescending(x => x.StartAt) : query.OrderBy(x => x.StartAt),
                "enddate" => desc ? query.OrderByDescending(x => x.EndAt) : query.OrderBy(x => x.EndAt),
                "isactive" => desc ? query.OrderByDescending(x => x.IsActive) : query.OrderBy(x => x.IsActive),
                _ => desc ? query.OrderByDescending(x => x.GachaBannerId) : query.OrderBy(x => x.GachaBannerId),
            };

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (totalCount, items);
        }

        public async Task<(int TotalCount, List<GachaBannerItem> Items)> GetBannerItemsPaged(int page, int pageSize)
        {
            var query = _context.GachaBannerItems
                .Include(bi => bi.Item)
                .AsNoTracking();

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (totalCount, items);
        }

        public async Task<GachaPullHistory> AddGachaPullHistory(GachaPullHistory history)
        {
            await _context.GachaPullHistories.AddAsync(history);
            await _context.SaveChangesAsync();
            return history;
        }

        public async Task<List<GachaPullHistory>> GetPullHistoryByPlayerAndBanner(int playerProfileId, int bannerId)
        {
            return await _context.GachaPullHistories
                .Where(h => h.PlayerProfileId == playerProfileId && h.GachaBannerId == bannerId)
                .OrderByDescending(h => h.PulledAt)
                .ThenByDescending(h => h.GachaPullHistoryId)
                .ToListAsync();
        }

        public async Task<(int TotalCount, List<GachaPullHistory> Items)> GetGachaPullHistoryPaged(int playerProfileId, int page, int pageSize)
        {
            var query = _context.GachaPullHistories
                .Include(h => h.GachaBanner)
                .Include(h => h.RewardItem)
                .Where(h => h.PlayerProfileId == playerProfileId)
                .OrderByDescending(h => h.PulledAt)
                .AsNoTracking();

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (totalCount, items);
        }
    }
}
