using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    /// <summary>
    /// Triển khai các thao tác truy cập dữ liệu cho tiến độ chơi dungeon sử dụng Entity Framework.
    /// </summary>
    public class DungeonProgressRepository : IDungeonProgressRepository
    {
        private readonly MysticJourneyDbContext _context;

        public DungeonProgressRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        /// <summary>Tìm tiến độ chơi dungeon theo mã phiên, trả về null nếu chưa được tạo.</summary>
        public async Task<DungeonProgress?> GetBySessionId(int sessionId)
        {
            return await _context.DungeonProgresses
                .FirstOrDefaultAsync(p => p.DungeonSessionId == sessionId);
        }

        /// <summary>Tạo bản ghi tiến độ dungeon mới (tự động ghi nhận thời gian tạo).</summary>
        public async Task<DungeonProgress> Create(DungeonProgress progress)
        {
            progress.CreatedAt = DateTime.UtcNow;
            await _context.DungeonProgresses.AddAsync(progress);
            await _context.SaveChangesAsync();
            return progress;
        }

        /// <summary>Cập nhật tiến độ dungeon (tự động ghi nhận thời gian cập nhật).</summary>
        public async Task<DungeonProgress> Update(DungeonProgress progress)
        {
            progress.UpdatedAt = DateTime.UtcNow;
            _context.DungeonProgresses.Update(progress);
            await _context.SaveChangesAsync();
            return progress;
        }
    }
}
