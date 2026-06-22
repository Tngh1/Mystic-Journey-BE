using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class DungeonSessionRepository : IDungeonSessionRepository
    {
        private readonly MysticJourneyDbContext _context;

        public DungeonSessionRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<DungeonSession?> GetById(int sessionId)
        {
            return await _context.DungeonSessions
                .Include(s => s.DungeonConfig)
                    .ThenInclude(d => d!.Chest)
                        .ThenInclude(c => c!.ChestItems)
                            .ThenInclude(ci => ci.Item)
                .Include(s => s.Progress)
                .FirstOrDefaultAsync(s => s.DungeonSessionId == sessionId);
        }

        public async Task<List<DungeonSession>> GetByPlayerProfileId(int playerProfileId)
        {
            return await _context.DungeonSessions
                .Include(s => s.DungeonConfig)
                .Include(s => s.Progress)
                .Where(s => s.PlayerProfileId == playerProfileId)
                .OrderByDescending(s => s.EnterTime)
                .ToListAsync();
        }

        public async Task<DungeonSession?> GetActiveSession(int playerProfileId, int dungeonConfigId)
        {
            return await _context.DungeonSessions
                .FirstOrDefaultAsync(s =>
                    s.PlayerProfileId == playerProfileId &&
                    s.DungeonConfigId == dungeonConfigId &&
                    s.Status == "Active");
        }

        public async Task<DungeonSession> Create(DungeonSession session)
        {
            session.CreatedAt = DateTime.UtcNow;
            await _context.DungeonSessions.AddAsync(session);
            await _context.SaveChangesAsync();
            return session;
        }

        public async Task<DungeonSession> Update(DungeonSession session)
        {
            session.UpdatedAt = DateTime.UtcNow;
            _context.DungeonSessions.Update(session);
            await _context.SaveChangesAsync();
            return session;
        }
    }
}
