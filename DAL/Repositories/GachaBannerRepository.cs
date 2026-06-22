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

        public async Task<GachaBanner> CreateGachaBanner(GachaBanner banner)
        {
            await _context.GachaBanners.AddAsync(banner);
            await _context.SaveChangesAsync();
            return banner;
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


        public async Task<(int TotalCount, List<GachaBanner> Items)> GetBannersPaged(int page, int pageSize, string? search, string? type, bool? isActive)
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
    }
}
