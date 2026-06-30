using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
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
                .ToListAsync();
        }

        public async Task<Item?> GetQuestItemByNames(params string[] names)
        {
            return await _context.Items
                .Where(i => i.IsActive && i.Type == "QuestItem" && names.Contains(i.Name))
                .OrderBy(i => i.ItemId)
                .FirstOrDefaultAsync();
        }

        public async Task<Item?> GetItemByIdWithStats(int id)
        {
            return await _context.Items
                .Include(i => i.EquipmentStats)
                .FirstOrDefaultAsync(i => i.ItemId == id);
        }

        public async Task<Item> CreateItem(Item item)
        {
            await _context.Items.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<Item> UpdateItem(Item item)
        {
            _context.Items.Update(item);
            await _context.SaveChangesAsync();
            return item;
        }


        public async Task<(int TotalCount, List<Item> Items)> GetItemsPaged(int page, int pageSize, string? search, string? type, string? rarity, bool? isActive)
        {
            var query = _context.Items
                .Include(i => i.EquipmentStats)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Name.Contains(search));
            }
            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(x => x.Type == type);
            }
            if (!string.IsNullOrEmpty(rarity))
            {
                query = query.Where(x => x.Rarity == rarity);
            }
            if (isActive.HasValue)
            {
                query = query.Where(x => x.IsActive == isActive.Value);
            }

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (totalCount, items);
        }
    }
}
