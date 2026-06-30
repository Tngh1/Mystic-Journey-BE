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

        /// <summary>Lấy snapshot chỉ số của người chơi.</summary>
        public async Task<PlayerStatsSnapshot?> GetSnapshotByPlayerProfileId(int playerProfileId)
        {
            return await _context.PlayerStatsSnapshots.FirstOrDefaultAsync(s => s.PlayerProfileId == playerProfileId);
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
            _context.PlayerProfiles.Update(profile);
            await _context.SaveChangesAsync();
            return profile;
        }

        // ── Tìm kiếm ──

        /// <summary>Tìm kiếm hồ sơ theo từ khóa (tên hiển thị hoặc username) và/hoặc lớp nhân vật.</summary>
        public async Task<List<PlayerProfile>> Search(string? keyword = null, string? playerClass = null)
        {
            var query = _context.PlayerProfiles.AsQueryable();

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

            return await query.ToListAsync();
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
