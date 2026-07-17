using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    /// <summary>
    /// Triển khai các thao tác truy cập dữ liệu cho thành tích trong game sử dụng Entity Framework.
    /// </summary>
    public class AchievementRepository : IAchievementRepository
    {
        private readonly MysticJourneyDbContext _context;

        public AchievementRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        // ── Query ──

        /// <summary>Tìm thành tích theo mã định danh.</summary>
        public async Task<Achievement?> GetAchievementById(int id)
        {
            return await _context.Achievements
                .FirstOrDefaultAsync(a => a.AchievementId == id);
        }

        /// <summary>Tìm thành tích kèm vật phẩm thưởng.</summary>
        public async Task<Achievement?> GetAchievementByIdWithReward(int id)
        {
            return await _context.Achievements
                .Include(a => a.RewardItem)
                .FirstOrDefaultAsync(a => a.AchievementId == id);
        }

        /// <summary>Lấy toàn bộ thành tích đang hoạt động.</summary>
        public async Task<List<Achievement>> GetAllActiveAchievements()
        {
            return await _context.Achievements
                .Where(a => a.IsActive)
                .ToListAsync();
        }

        // ── CRUD ──

        /// <summary>Cập nhật thông tin thành tích.</summary>
        public async Task<Achievement> UpdateAchievement(Achievement achievement)
        {
            _context.Achievements.Update(achievement);
            await _context.SaveChangesAsync();
            return achievement;
        }

        // ── Phân trang ──

        /// <summary>Lấy danh sách thành tích có phân trang, lọc theo tìm kiếm (tên), loại và trạng thái hoạt động.</summary>
        public async Task<(int TotalCount, List<Achievement> Items)> GetAchievementsPaged(int page, int pageSize, string? search, string? type, bool? isActive, string? sortBy = null, string? sortOrder = null)
        {
            var query = _context.Achievements
                .Include(a => a.RewardItem)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(a => a.Name.Contains(search));
            }
            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(a => a.Type == type);
            }
            if (isActive.HasValue)
            {
                query = query.Where(a => a.IsActive == isActive.Value);
            }

            bool desc = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);
            query = (sortBy?.ToLowerInvariant()) switch
            {
                "name" => desc ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),
                "type" => desc ? query.OrderByDescending(x => x.Type) : query.OrderBy(x => x.Type),
                "requiredvalue" => desc ? query.OrderByDescending(x => x.RequiredValue) : query.OrderBy(x => x.RequiredValue),
                "rewardgold" => desc ? query.OrderByDescending(x => x.RewardGold) : query.OrderBy(x => x.RewardGold),
                "rewardgems" => desc ? query.OrderByDescending(x => x.RewardGem) : query.OrderBy(x => x.RewardGem),
                "isactive" => desc ? query.OrderByDescending(x => x.IsActive) : query.OrderBy(x => x.IsActive),
                _ => desc ? query.OrderByDescending(x => x.AchievementId) : query.OrderBy(x => x.AchievementId),
            };

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (totalCount, items);
        }
    }
}
