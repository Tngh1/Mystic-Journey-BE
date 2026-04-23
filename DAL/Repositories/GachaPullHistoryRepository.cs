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
    public class GachaPullHistoryRepository : IGachaPullHistoryRepository
    {
        private readonly MysticJourneyDbContext _context;

        public GachaPullHistoryRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<GachaPullHistory?> GetByIdAsync(Guid pullId)
        {
            return await _context.GachaPullHistories
                .Include(ph => ph.GachaBanner)
                .Include(ph => ph.RewardItem)
                .FirstOrDefaultAsync(ph => ph.Id == pullId);
        }

        public async Task<List<GachaPullHistory>> GetByPlayerProfileIdAsync(Guid playerProfileId, int pageNumber = 1, int pageSize = 20)
        {
            return await _context.GachaPullHistories
                .Include(ph => ph.GachaBanner)
                .Include(ph => ph.RewardItem)
                .Where(ph => ph.PlayerProfileId == playerProfileId)
                .OrderByDescending(ph => ph.PulledAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<GachaPullHistory>> GetByBannerAsync(Guid playerProfileId, Guid bannerId)
        {
            return await _context.GachaPullHistories
                .Include(ph => ph.RewardItem)
                .Where(ph => ph.PlayerProfileId == playerProfileId && ph.GachaBannerId == bannerId)
                .OrderByDescending(ph => ph.PulledAt)
                .ToListAsync();
        }

        public async Task<int> GetPullCountSinceLastFeaturedAsync(Guid playerProfileId, Guid bannerId)
        {
            var lastFeaturedPull = await _context.GachaPullHistories
                .Include(ph => ph.RewardItem)
                .ThenInclude(ri => ri!.GachaBannerItems)
                .Where(ph => ph.PlayerProfileId == playerProfileId &&
                             ph.GachaBannerId == bannerId &&
                             ph.RewardItem!.GachaBannerItems.Any(bi => bi.IsFeatured))
                .OrderByDescending(ph => ph.PulledAt)
                .FirstOrDefaultAsync();

            if (lastFeaturedPull == null)
            {
                return await _context.GachaPullHistories
                    .CountAsync(ph => ph.PlayerProfileId == playerProfileId && ph.GachaBannerId == bannerId);
            }

            return await _context.GachaPullHistories
                .CountAsync(ph => ph.PlayerProfileId == playerProfileId &&
                                   ph.GachaBannerId == bannerId &&
                                   ph.PulledAt > lastFeaturedPull.PulledAt);
        }

        public async Task<GachaPullHistory> CreateAsync(GachaPullHistory pull)
        {
            await _context.GachaPullHistories.AddAsync(pull);
            await _context.SaveChangesAsync();
            return pull;
        }

        public async Task<int> GetTotalCountAsync(Guid playerProfileId)
        {
            return await _context.GachaPullHistories
                .Where(ph => ph.PlayerProfileId == playerProfileId)
                .CountAsync();
        }
    }
}
