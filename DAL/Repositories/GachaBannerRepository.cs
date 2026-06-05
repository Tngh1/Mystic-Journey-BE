using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
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

        public async Task<List<GachaBanner>> GetAllGachaBanners()
        {
            return await _context.GachaBanners.ToListAsync();
        }

        public async Task<List<GachaBanner>> GetActiveGachaBanners()
        {
            return await _context.GachaBanners
                .Include(b => b.BannerItems)
                .Where(b => b.IsActive)
                .ToListAsync();
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

        public async Task DeleteGachaBanner(int id)
        {
            var banner = await GetGachaBannerById(id);
            if (banner != null)
            {
                _context.GachaBanners.Remove(banner);
                await _context.SaveChangesAsync();
            }
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

        public async Task DeleteBannerItems(int bannerId)
        {
            var items = await _context.GachaBannerItems
                .Where(i => i.GachaBannerId == bannerId)
                .ToListAsync();

            _context.GachaBannerItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }

        public IQueryable<GachaBanner> GetGachaBannersQueryable()
        {
            return _context.GachaBanners
                .Include(b => b.BannerItems)
                    .ThenInclude(bi => bi.Item)
                .AsNoTracking();
        }

        public IQueryable<GachaBannerItem> GetBannerItemsQueryable()
        {
            return _context.GachaBannerItems
                .Include(bi => bi.Item)
                .AsNoTracking();
        }
    }
}
