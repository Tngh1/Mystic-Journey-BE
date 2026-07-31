using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly MysticJourneyDbContext _context;

        public ItemRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<Item?> GetItemById(int id)
        {
            return await _context.Items
                .FirstOrDefaultAsync(i => i.ItemId == id);
        }

        public async Task<int> GetTotalItemsCount()
        {
            return await _context.Items.CountAsync();
        }

        public async Task<List<Item>> GetQuestItems()
        {
            return await _context.Items
                .Where(i => i.IsActive && i.Type == "QuestItem")
                .OrderBy(i => i.ItemId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Item?> GetQuestItemByNames(params string[] names)
        {
            // Search by name only (not by Type) so renamed types (e.g. Magic Flour changed from
            // QuestItem to Consumable) are still resolved correctly during quest item collection.
            return await _context.Items
                .Where(i => i.IsActive && names.Contains(i.Name))
                .OrderBy(i => i.ItemId)
                .FirstOrDefaultAsync();
        }

        public async Task<Item?> GetItemByIdWithStats(int id)
        {
            return await _context.Items
                .Include(i => i.EquipmentStats)
                .FirstOrDefaultAsync(i => i.ItemId == id);
        }

        public async Task<Item> UpdateItem(Item item)
        {
            _context.Items.Update(item);
            await _context.SaveChangesAsync();
            return item;
        }


        public async Task<(int TotalCount, List<Item> Items)> GetItemsPaged(int page, int pageSize, string? search, string? type, string? rarity, bool? isActive, string? sortBy = null, string? sortOrder = null)
        {
            // Đếm trên query chưa Include: COUNT không cần join sang EquipmentStats.
            var filtered = _context.Items.AsNoTracking();

            if (!string.IsNullOrEmpty(search))
            {
                filtered = filtered.Where(x => x.Name.Contains(search));
            }
            if (!string.IsNullOrEmpty(type))
            {
                filtered = filtered.Where(x => x.Type == type);
            }
            if (!string.IsNullOrEmpty(rarity))
            {
                filtered = filtered.Where(x => x.Rarity == rarity);
            }
            if (isActive.HasValue)
            {
                filtered = filtered.Where(x => x.IsActive == isActive.Value);
            }

            int totalCount = await filtered.CountAsync();

            var query = filtered.Include(i => i.EquipmentStats);

            bool desc = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);
            IQueryable<Item> ordered = (sortBy?.ToLowerInvariant()) switch
            {
                "name" => desc ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),
                "type" => desc ? query.OrderByDescending(x => x.Type) : query.OrderBy(x => x.Type),
                "rarity" => desc ? query.OrderByDescending(x => x.Rarity) : query.OrderBy(x => x.Rarity),
                "basevalue" => desc ? query.OrderByDescending(x => x.BaseValue) : query.OrderBy(x => x.BaseValue),
                "isactive" => desc ? query.OrderByDescending(x => x.IsActive) : query.OrderBy(x => x.IsActive),
                _ => desc ? query.OrderByDescending(x => x.ItemId) : query.OrderBy(x => x.ItemId),
            };

            var items = await ordered.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (totalCount, items);
        }
    }
}
