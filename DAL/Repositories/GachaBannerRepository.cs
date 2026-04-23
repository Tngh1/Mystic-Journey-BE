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
    public class GachaBannerRepository : IGachaBannerRepository
    {
        private readonly MysticJourneyDbContext _context;

        public GachaBannerRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<GachaBanner?> GetByIdAsync(Guid bannerId)
        {
            return await _context.GachaBanners
                .FirstOrDefaultAsync(b => b.Id == bannerId && b.IsActive);
        }

        public async Task<GachaBanner?> GetByIdWithItemsAsync(Guid bannerId)
        {
            return await _context.GachaBanners
                .Include(b => b.BannerItems)
                    .ThenInclude(bi => bi.Item)
                .FirstOrDefaultAsync(b => b.Id == bannerId && b.IsActive);
        }

        public async Task<List<GachaBanner>> GetAllActiveAsync()
        {
            return await _context.GachaBanners
                .Where(b => b.IsActive)
                .OrderBy(b => b.Name)
                .ToListAsync();
        }

        public async Task<List<GachaBanner>> GetAvailableNowAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.GachaBanners
                .Where(b => b.IsActive && b.StartAt <= now && b.EndAt >= now)
                .OrderBy(b => b.Name)
                .ToListAsync();
        }

        public async Task<GachaBanner> CreateAsync(GachaBanner banner)
        {
            await _context.GachaBanners.AddAsync(banner);
            await _context.SaveChangesAsync();
            return banner;
        }

        public async Task<GachaBanner> UpdateAsync(GachaBanner banner)
        {
            _context.GachaBanners.Update(banner);
            await _context.SaveChangesAsync();
            return banner;
        }
    }
}
