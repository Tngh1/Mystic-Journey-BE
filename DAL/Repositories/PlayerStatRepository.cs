using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class PlayerStatRepository : IPlayerStatRepository
    {
        private readonly MysticJourneyDbContext _context;

        public PlayerStatRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<PlayerStat?> GetByPlayerProfileId(int playerProfileId)
        {
            return await _context.PlayerStats
                .FirstOrDefaultAsync(s => s.PlayerProfileId == playerProfileId);
        }

        public async Task<PlayerStat> Create(PlayerStat stat)
        {
            stat.CreatedAt = DateTime.UtcNow;
            await _context.PlayerStats.AddAsync(stat);
            await _context.SaveChangesAsync();
            return stat;
        }

        public async Task<PlayerStat> Update(PlayerStat stat)
        {
            stat.UpdatedAt = DateTime.UtcNow;
            _context.PlayerStats.Update(stat);
            await _context.SaveChangesAsync();
            return stat;
        }
    }
}
