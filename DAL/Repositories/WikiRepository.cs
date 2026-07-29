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
    // Tầng dữ liệu cho codex công khai. Xem IWikiRepository để biết vì sao
    // không có tham số isActive: điều kiện IsActive == true được ghim ngay ở đây.
    public class WikiRepository : IWikiRepository
    {
        private readonly MysticJourneyDbContext _context;

        public WikiRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        // ═══════════════════════════════════════════════════════════════════════
        // CLASSES
        // ═══════════════════════════════════════════════════════════════════════

        public async Task<List<ClassConfig>> GetClassConfigs()
        {
            return await _context.ClassConfigs
                .AsNoTracking()
                .OrderBy(c => c.ClassConfigId)
                .ToListAsync();
        }

        // ═══════════════════════════════════════════════════════════════════════
        // MONSTERS
        // ═══════════════════════════════════════════════════════════════════════

        public async Task<(int TotalCount, List<Monster> Items)> GetMonstersPaged(
            int page, int pageSize, string? search, string? type, string? sortBy, string? sortOrder)
        {
            // Đếm trên query chưa Include: COUNT không cần join sang MonsterDrops/Item.
            var filtered = _context.Monsters
                .AsNoTracking()
                .Where(x => x.IsActive);

            if (!string.IsNullOrEmpty(search))
                filtered = filtered.Where(x => x.Name.Contains(search) || x.Description.Contains(search));
            if (!string.IsNullOrEmpty(type))
                filtered = filtered.Where(x => x.Type == type);

            int totalCount = await filtered.CountAsync();

            var query = filtered
                .Include(m => m.MonsterDrops)
                    .ThenInclude(d => d.Item);

            bool desc = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);
            IQueryable<Monster> ordered = (sortBy?.ToLowerInvariant()) switch
            {
                "name" => desc ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),
                "type" => desc ? query.OrderByDescending(x => x.Type) : query.OrderBy(x => x.Type),
                "level" => desc ? query.OrderByDescending(x => x.Level) : query.OrderBy(x => x.Level),
                "maxhp" => desc ? query.OrderByDescending(x => x.MaxHp) : query.OrderBy(x => x.MaxHp),
                "attack" => desc ? query.OrderByDescending(x => x.Atk) : query.OrderBy(x => x.Atk),
                "defense" => desc ? query.OrderByDescending(x => x.Def) : query.OrderBy(x => x.Def),
                "goldreward" => desc ? query.OrderByDescending(x => x.GoldReward) : query.OrderBy(x => x.GoldReward),
                "expreward" => desc ? query.OrderByDescending(x => x.ExperienceReward) : query.OrderBy(x => x.ExperienceReward),
                _ => desc ? query.OrderByDescending(x => x.Level) : query.OrderBy(x => x.Level),
            };

            var items = await ordered.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (totalCount, items);
        }

        public async Task<Monster?> GetMonsterById(int id)
        {
            return await _context.Monsters
                .AsNoTracking()
                .Include(m => m.MonsterDrops)
                    .ThenInclude(d => d.Item)
                .FirstOrDefaultAsync(m => m.MonsterId == id && m.IsActive);
        }

        // ═══════════════════════════════════════════════════════════════════════
        // ITEMS
        // ═══════════════════════════════════════════════════════════════════════

        public async Task<(int TotalCount, List<Item> Items)> GetItemsPaged(
            int page, int pageSize, string? search, string? type, string? rarity, string? sortBy, string? sortOrder)
        {
            var filtered = _context.Items
                .AsNoTracking()
                .Where(x => x.IsActive);

            if (!string.IsNullOrEmpty(search))
                filtered = filtered.Where(x => x.Name.Contains(search) || (x.Description != null && x.Description.Contains(search)));
            if (!string.IsNullOrEmpty(type))
                filtered = filtered.Where(x => x.Type == type);
            if (!string.IsNullOrEmpty(rarity))
                filtered = filtered.Where(x => x.Rarity == rarity);

            int totalCount = await filtered.CountAsync();

            var query = filtered.Include(i => i.EquipmentStats);

            bool desc = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);
            IQueryable<Item> ordered = (sortBy?.ToLowerInvariant()) switch
            {
                "name" => desc ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),
                "type" => desc ? query.OrderByDescending(x => x.Type) : query.OrderBy(x => x.Type),
                // Rarity xếp theo THỨ BẬC, không theo bảng chữ cái: cột Rarity là
                // string nên OrderBy(x => x.Rarity) cho ra Common < Epic < Legendary,
                // sai với thang độ hiếm.
                //
                // Biểu thức tam phân được viết thẳng vào lambda, KHÔNG tách ra hàm
                // riêng: EF Core chỉ dịch được cây biểu thức nó nhìn thấy, gọi một
                // static method trong OrderBy sẽ ném lỗi "could not be translated"
                // lúc chạy. Viết inline thì provider sinh ra CASE WHEN và việc sắp
                // xếp diễn ra trên DB.
                "rarity" => desc
                    ? query.OrderByDescending(x =>
                        x.Rarity == "Common" ? 0
                        : x.Rarity == "Uncommon" ? 1
                        : x.Rarity == "Rare" ? 2
                        : x.Rarity == "Epic" ? 3
                        : x.Rarity == "Legendary" ? 4
                        : x.Rarity == "Mythic" ? 5
                        : 6)
                    : query.OrderBy(x =>
                        x.Rarity == "Common" ? 0
                        : x.Rarity == "Uncommon" ? 1
                        : x.Rarity == "Rare" ? 2
                        : x.Rarity == "Epic" ? 3
                        : x.Rarity == "Legendary" ? 4
                        : x.Rarity == "Mythic" ? 5
                        : 6),
                "basevalue" => desc ? query.OrderByDescending(x => x.BaseValue) : query.OrderBy(x => x.BaseValue),
                _ => desc ? query.OrderByDescending(x => x.ItemId) : query.OrderBy(x => x.ItemId),
            };

            var items = await ordered.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (totalCount, items);
        }

        public async Task<Item?> GetItemById(int id)
        {
            return await _context.Items
                .AsNoTracking()
                .Include(i => i.EquipmentStats)
                .FirstOrDefaultAsync(i => i.ItemId == id && i.IsActive);
        }

        // ═══════════════════════════════════════════════════════════════════════
        // SKILLS
        // ═══════════════════════════════════════════════════════════════════════

        public async Task<(int TotalCount, List<Skill> Items)> GetSkillsPaged(
            int page, int pageSize, string? search, string? type)
        {
            var query = _context.Skills
                .AsNoTracking()
                .Where(s => s.IsActive);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(s => s.Name.Contains(search) || (s.Description != null && s.Description.Contains(search)));
            if (!string.IsNullOrEmpty(type))
                query = query.Where(s => s.Type == type);

            int totalCount = await query.CountAsync();

            // Codex hiển thị kỹ năng theo lộ trình mở khoá, nên UnlockLevel là thứ
            // tự nhiên nhất. SkillRepository.GetSkillsPaged (dashboard) không có
            // ORDER BY nào cả — đó là lý do trang wiki cần truy vấn riêng.
            var items = await query
                .OrderBy(s => s.UnlockLevel)
                .ThenBy(s => s.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (totalCount, items);
        }

        public async Task<Skill?> GetSkillById(int id)
        {
            return await _context.Skills
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.SkillId == id && s.IsActive);
        }
    }
}
