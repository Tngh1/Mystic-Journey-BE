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
        /// Sắp xếp theo EnterTime giảm dần: nếu dữ liệu cũ còn sót nhiều dòng Active,
        /// Resume phải lấy phiên MỚI NHẤT. Không có OrderBy thì Postgres trả về thứ tự
        /// bất kỳ, nên người chơi có thể bị đưa vào đúng phiên cũ đã chết.
        /// </summary>
        public async Task<DungeonSession?> GetActiveSession(int playerProfileId, int? dungeonConfigId = null)
        {
            var query = _context.DungeonSessions.Where(s => s.PlayerProfileId == playerProfileId && s.Status == "Active");

            if (dungeonConfigId.HasValue)
            {
                query = query.Where(s => s.DungeonConfigId == dungeonConfigId.Value);
            }

            return await query
                .Include(s => s.DungeonConfig)
                .Include(s => s.Progress)
                .OrderByDescending(s => s.EnterTime)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Đánh dấu Failed cho MỌI phiên đang Active của người chơi. Một người chơi chỉ có thể
        /// ở trong một dungeon tại một thời điểm, nên khi vào phiên mới thì mọi phiên cũ đều chết.
        /// Trước đây chỗ này chỉ huỷ phiên của CÙNG một dungeon, nên vào dungeon khác lúc đang
        /// có phiên treo sẽ để lại hai dòng Active và Resume không biết chọn dòng nào.
        /// Ghi trực tiếp một cột, không SELECT — dọn được cả nhiều dòng rác tồn từ trước.
        /// </summary>
        public Task<int> FailActiveSessions(int playerProfileId)
        {
            return _context.DungeonSessions
                .Where(s => s.PlayerProfileId == playerProfileId && s.Status == "Active")
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.Status, "Failed")
                    .SetProperty(x => x.UpdatedAt, DateTime.UtcNow));
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
