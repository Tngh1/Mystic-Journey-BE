using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class PlayerProfileRepository : IPlayerProfileRepository
    {
        private readonly MysticJourneyDbContext _context;

        public PlayerProfileRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<PlayerProfile?> GetByIdAsync(Guid profileId)
        {
            return await _context.PlayerProfiles
                .FirstOrDefaultAsync(p => p.Id == profileId);
        }

        public async Task<PlayerProfile?> GetByAccountIdAsync(Guid accountId)
        {
            return await _context.PlayerProfiles
                .FirstOrDefaultAsync(p => p.AccountId == accountId);
        }

        public async Task<PlayerProfile?> GetByIdWithDetailsAsync(Guid profileId)
        {
            return await _context.PlayerProfiles
                .Include(p => p.PlayerStats)
                .FirstOrDefaultAsync(p => p.Id == profileId);
        }

        public async Task<PlayerProfile?> GetByAccountIdWithDetailsAsync(Guid accountId)
        {
            return await _context.PlayerProfiles
                .Include(p => p.PlayerStats)
                .FirstOrDefaultAsync(p => p.AccountId == accountId);
        }

        public async Task<PlayerProfile> CreateAsync(PlayerProfile profile)
        {
            await _context.PlayerProfiles.AddAsync(profile);
            await _context.SaveChangesAsync();
            return profile;
        }

        public async Task<PlayerProfile> UpdateAsync(PlayerProfile profile)
        {
            profile.UpdatedAt = DateTime.UtcNow;
            _context.PlayerProfiles.Update(profile);
            await _context.SaveChangesAsync();
            return profile;
        }

        public async Task<bool> ExistsAsync(Guid accountId)
        {
            return await _context.PlayerProfiles
                .AnyAsync(p => p.AccountId == accountId);
        }

        public async Task<PlayerStat?> GetStatsByProfileIdAsync(Guid profileId)
        {
            return await _context.PlayerStats
                .FirstOrDefaultAsync(s => s.PlayerProfileId == profileId);
        }

        public async Task<PlayerStat> UpdateStatsAsync(PlayerStat stats)
        {
            stats.UpdatedAt = DateTime.UtcNow;
            _context.PlayerStats.Update(stats);
            await _context.SaveChangesAsync();
            return stats;
        }

        public async Task UpdateCurrencyAsync(Guid profileId, decimal? gold = null, decimal? gems = null, int? energy = null)
        {
            var profile = await GetByIdAsync(profileId);
            if (profile == null) return;

            if (gold.HasValue) profile.Gold = gold.Value;
            if (gems.HasValue) profile.Gems = gems.Value;
            if (energy.HasValue) profile.Energy = energy.Value;

            profile.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
