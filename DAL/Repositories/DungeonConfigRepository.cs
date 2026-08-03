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
    /// Triển khai các thao tác truy cập dữ liệu cho cấu hình dungeon sử dụng Entity Framework.
    /// </summary>
    public class DungeonConfigRepository : IDungeonConfigRepository
    {
        private readonly MysticJourneyDbContext _context;

        public DungeonConfigRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        // ── Query ──

        /// <summary>Tìm cấu hình dungeon theo mã định danh.</summary>
        public async Task<DungeonConfig?> GetDungeonConfigById(int id)
        {
            return await _context.DungeonConfigs
                .FirstOrDefaultAsync(d => d.DungeonConfigId == id);
        }

        /// <summary>
        /// Lấy cấu hình dungeon kèm rương và vật phẩm trong rương.
        /// Dùng để kiểm tra năng lượng và xem trước phần thưởng.
        /// </summary>
        public async Task<DungeonConfig?> GetByIdWithChest(int id)
        {
            return await _context.DungeonConfigs
                .Include(d => d.Chest)
                    .ThenInclude(c => c!.ChestItems)
                        .ThenInclude(ci => ci.Item)
                .FirstOrDefaultAsync(d => d.DungeonConfigId == id && d.IsActive);
        }

        /// <summary>Lấy tất cả cấu hình dungeon.</summary>
        public async Task<List<DungeonConfig>> GetAllDungeonConfigs()
        {
            return await _context.DungeonConfigs.ToListAsync();
        }

        /// <summary>Lấy các dungeon đang hoạt động.</summary>
        public async Task<List<DungeonConfig>> GetActiveDungeonConfigs()
        {
            return await _context.DungeonConfigs
                .Where(d => d.IsActive)
                .ToListAsync();
        }

        /// <summary>Kiểm tra dungeon có tồn tại hay không theo mã.</summary>
        public async Task<bool> DungeonExists(int dungeonId)
        {
            return await _context.DungeonConfigs.AnyAsync(d => d.DungeonConfigId == dungeonId);
        }

        // ── CRUD ──

        /// <summary>Cập nhật cấu hình dungeon.</summary>
        public async Task<DungeonConfig> UpdateDungeonConfig(DungeonConfig dungeon)
        {
            _context.DungeonConfigs.Update(dungeon);
            await _context.SaveChangesAsync();
            return dungeon;
        }

        // ── Phân trang ──

        /// <summary>Lấy danh sách dungeon có phân trang, lọc theo tìm kiếm (tên), loại và trạng thái hoạt động.</summary>
        public async Task<(int TotalCount, List<DungeonConfig> Items)> GetDungeonsPaged(int page, int pageSize, string? search, string? type, bool? isActive, string? sortBy = null, string? sortOrder = null)
        {
            var query = _context.DungeonConfigs.AsNoTracking()
                .Include(d => d.Chest)
                    .ThenInclude(c => c!.ChestItems)
                        .ThenInclude(ci => ci.Item)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d => d.Name.Contains(search));
            }
            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(d => d.Type == type);
            }
            if (isActive.HasValue)
            {
                query = query.Where(d => d.IsActive == isActive.Value);
            }

            bool desc = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);
            query = (sortBy?.ToLowerInvariant()) switch
            {
                "name" => desc ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),
                "type" => desc ? query.OrderByDescending(x => x.Type) : query.OrderBy(x => x.Type),
                "levelrequirement" => desc ? query.OrderByDescending(x => x.LevelRequirement) : query.OrderBy(x => x.LevelRequirement),
                "isactive" => desc ? query.OrderByDescending(x => x.IsActive) : query.OrderBy(x => x.IsActive),
                _ => desc ? query.OrderByDescending(x => x.DungeonConfigId) : query.OrderBy(x => x.DungeonConfigId),
            };

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (totalCount, items);
        }
    }
}
