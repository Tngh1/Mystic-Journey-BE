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
    /// <summary>
    /// Triển khai các thao tác truy cập dữ liệu cho hồ sơ người chơi sử dụng Entity Framework.
    /// </summary>
    public class PlayerProfileRepository : IPlayerProfileRepository
    {
        private readonly MysticJourneyDbContext _context;

        public PlayerProfileRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        // ── Query ──

        /// <summary>Tìm hồ sơ người chơi theo mã định danh.</summary>
        public async Task<PlayerProfile?> GetPlayerProfileById(int id)
        {
            return await _context.PlayerProfiles
                .FirstOrDefaultAsync(p => p.PlayerProfileId == id);
        }

        /// <summary>Tìm hồ sơ người chơi kèm chỉ số (stats).</summary>
        public async Task<PlayerProfile?> GetPlayerProfileByIdWithStats(int id)
        {
            return await _context.PlayerProfiles
                .Include(p => p.PlayerStats)
                .FirstOrDefaultAsync(p => p.PlayerProfileId == id);
        }

        /// <summary>Tìm hồ sơ đầy đủ kèm stats và tài khoản.</summary>
        public async Task<PlayerProfile?> GetByIdFull(int id)
        {
            return await _context.PlayerProfiles
                .Include(p => p.PlayerStats)
                .Include(p => p.Account)
                .FirstOrDefaultAsync(p => p.PlayerProfileId == id);
        }

        /// <summary>Tìm hồ sơ người chơi theo mã tài khoản.</summary>
        public async Task<PlayerProfile?> GetByAccountId(int accountId)
        {
            return await _context.PlayerProfiles
                .Include(p => p.PlayerStats)
                .Include(p => p.Account)
                .FirstOrDefaultAsync(p => p.AccountId == accountId);
        }

        public async Task<PlayerProfile?> GetPlayerProfileByName(string playerName)
        {
            return await _context.PlayerProfiles
                .FirstOrDefaultAsync(p => p.DisplayName == playerName);
        }

        /// <summary>Lấy tất cả hồ sơ người chơi trong hệ thống.</summary>
        public async Task<List<PlayerProfile>> GetAllPlayerProfiles()
        {
            return await _context.PlayerProfiles.ToListAsync();
        }

        // ── CRUD ──

        /// <summary>Tạo hồ sơ người chơi mới (tự động ghi nhận thời gian tạo).</summary>
        public async Task<PlayerProfile> CreatePlayerProfile(PlayerProfile profile)
        {
            profile.CreatedAt = DateTime.UtcNow;
            await _context.PlayerProfiles.AddAsync(profile);
            await _context.SaveChangesAsync();
            return profile;
        }

        /// <summary>Cập nhật thông tin hồ sơ người chơi (tự động ghi nhận thời gian cập nhật).</summary>
        public async Task<PlayerProfile> UpdatePlayerProfile(PlayerProfile profile)
        {
            profile.UpdatedAt = DateTime.UtcNow;
            // ponytail: profile luôn được EF track từ các hàm Get* ở trên (không AsNoTracking),
            // nên KHÔNG gọi .Update() ở đây — .Update() ép ghi lại toàn bộ cột theo snapshot
            // trong bộ nhớ của request này, đè mất các cột mà request khác vừa lưu song song
            // (VD: điểm cộng chỉ số khi lên level bị mất do trùng lúc đồng bộ vị trí/avatar).
            // SaveChangesAsync tự phát hiện đúng cột đã đổi vì entity đã tracked.
            await _context.SaveChangesAsync();
            return profile;
        }

        // ── Tìm kiếm ──

        /// <summary>Tìm kiếm hồ sơ theo từ khóa (tên hiển thị hoặc username) và/hoặc lớp nhân vật.</summary>
        public async Task<List<PlayerProfile>> Search(string? keyword = null, string? playerClass = null)
        {
            // Include Account: caller đọc Account.LastSeen để tính IsOnline. Thiếu Include
            // thì Account luôn null nên mọi kết quả tìm kiếm đều hiện Offline.
            var query = _context.PlayerProfiles
                .Include(p => p.Account)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var lowerKeyword = keyword.ToLower();
                query = query.Where(p =>
                    p.DisplayName.ToLower().Contains(lowerKeyword) ||
                    (p.Account != null && p.Account.UserName.ToLower().Contains(lowerKeyword)));
            }

            if (!string.IsNullOrWhiteSpace(playerClass))
            {
                query = query.Where(p => p.Class == playerClass);
            }

            if (string.IsNullOrWhiteSpace(keyword) && string.IsNullOrWhiteSpace(playerClass))
            {
                // ORDER BY random() phải sort toàn bảng mới lấy được 10 hàng. Thay bằng
                // một cửa sổ ngẫu nhiên trên index PK: chỉ seek, không sort.
                // ponytail: 10 người liền kề nhau theo id chứ không rải đều; nếu cần rải
                // thật thì đổi sang TABLESAMPLE hoặc bảng gợi ý dựng sẵn.
                var total = await _context.PlayerProfiles.CountAsync();
                var skip = total > 10 ? Random.Shared.Next(total - 9) : 0;
                return await query
                    .OrderBy(p => p.PlayerProfileId)
                    .Skip(skip)
                    .Take(10)
                    .ToListAsync();
            }

            return await query.Take(20).ToListAsync();
        }

        /// <summary>Đếm tổng số hồ sơ người chơi trong hệ thống.</summary>
        public async Task<int> GetTotalPlayerProfilesCount()
        {
            return await _context.PlayerProfiles.CountAsync();
        }

        // ── Phân trang ──

        /// <summary>Lấy danh sách hồ sơ có phân trang, lọc theo tìm kiếm (tên) và cấp độ.</summary>
        public async Task<(int TotalCount, List<PlayerProfile> Items)> GetProfilesPaged(int page, int pageSize, string? search, int? level)
        {
            var query = _context.PlayerProfiles
                .Include(p => p.Account)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.DisplayName.Contains(search));
            }
            if (level.HasValue)
            {
                query = query.Where(x => x.Level == level.Value);
            }

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (totalCount, items);
        }
    }
}
