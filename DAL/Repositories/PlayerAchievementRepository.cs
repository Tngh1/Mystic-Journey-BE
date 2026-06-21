using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class PlayerAchievementRepository : IPlayerAchievementRepository
    {
        private readonly MysticJourneyDbContext _context;

        public PlayerAchievementRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<List<PlayerAchievement>> GetByPlayerProfileId(int playerProfileId)
        {
            return await _context.PlayerAchievements
                .Include(pa => pa.Achievement)
                .Where(pa => pa.PlayerProfileId == playerProfileId)
                .ToListAsync();
        }
    }
}
