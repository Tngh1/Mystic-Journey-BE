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

        public async Task<PlayerProfile?> GetByIdAsync(int id)
        {
            return await _context.PlayerProfiles
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PlayerProfile?> GetByAccountIdAsync(Guid accountId)
        {
            return await _context.PlayerProfiles
                .FirstOrDefaultAsync(p => p.AccountId == accountId);
        }

        public async Task AddAsync(PlayerProfile profile)
        {
            await _context.PlayerProfiles.AddAsync(profile);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PlayerProfile profile)
        {
            _context.PlayerProfiles.Update(profile);
            await _context.SaveChangesAsync();
        }
    }
}
