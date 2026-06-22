using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class DungeonProgressRepository : IDungeonProgressRepository
    {
        private readonly MysticJourneyDbContext _context;

        public DungeonProgressRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<DungeonProgress?> GetBySessionId(int sessionId)
        {
            return await _context.DungeonProgresses
                .FirstOrDefaultAsync(p => p.DungeonSessionId == sessionId);
        }

        public async Task<DungeonProgress> Create(DungeonProgress progress)
        {
            progress.CreatedAt = DateTime.UtcNow;
            await _context.DungeonProgresses.AddAsync(progress);
            await _context.SaveChangesAsync();
            return progress;
        }

        public async Task<DungeonProgress> Update(DungeonProgress progress)
        {
            progress.UpdatedAt = DateTime.UtcNow;
            _context.DungeonProgresses.Update(progress);
            await _context.SaveChangesAsync();
            return progress;
        }
    }
}
