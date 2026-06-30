using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    /// <summary>
    /// Triển khai các thao tác truy cập dữ liệu cho phiên chơi dungeon sử dụng Entity Framework.
    /// </summary>
    public class DungeonSessionRepository : IDungeonSessionRepository
    {
        private readonly MysticJourneyDbContext _context;

        public DungeonSessionRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        // ── Query ──

        /// <summary>
        /// Tìm phiên chơi dungeon theo mã, kèm cấu hình dungeon, rương phần thưởng và tiến độ chơi.
        /// </summary>
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

        /// <summary>Lấy tất cả phiên chơi của người chơi, sắp xếp theo thời gian vào giảm dần.</summary>
        public async Task<System.Collections.Generic.List<DungeonSession>> GetByPlayerProfileId(int playerProfileId)
        {
            return await _context.DungeonSessions
                .Include(s => s.DungeonConfig)
                .Include(s => s.Progress)
                .Where(s => s.PlayerProfileId == playerProfileId)
                .OrderByDescending(s => s.EnterTime)
                .ToListAsync();
        }

        /// <summary>
        /// Tìm phiên chơi đang hoạt động của người chơi trong dungeon cụ thể.
        /// Dùng để ngăn chặn chạy nhiều phiên cùng lúc.
        /// </summary>
        public async Task<DungeonSession?> GetActiveSession(int playerProfileId, int dungeonConfigId)
        {
            return await _context.DungeonSessions
                .FirstOrDefaultAsync(s =>
                    s.PlayerProfileId == playerProfileId &&
                    s.DungeonConfigId == dungeonConfigId &&
                    s.Status == "Active");
        }

        // ── CRUD ──

        /// <summary>Tạo phiên chơi dungeon mới (tự động ghi nhận thời gian tạo).</summary>
        public async Task<DungeonSession> Create(DungeonSession session)
        {
            session.CreatedAt = DateTime.UtcNow;
            await _context.DungeonSessions.AddAsync(session);
            await _context.SaveChangesAsync();
            return session;
        }

        /// <summary>Cập nhật phiên chơi dungeon (trạng thái, thời gian ra vào...).</summary>
        public async Task<DungeonSession> Update(DungeonSession session)
        {
            session.UpdatedAt = DateTime.UtcNow;
            _context.DungeonSessions.Update(session);
            await _context.SaveChangesAsync();
            return session;
        }
    }
}
