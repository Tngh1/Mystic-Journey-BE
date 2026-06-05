using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
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

        public async Task<Item?> GetItemByIdWithStats(int id)
        {
            return await _context.Items
                .Include(i => i.EquipmentStats)
                .FirstOrDefaultAsync(i => i.ItemId == id);
        }

        public async Task<List<Item>> GetAllItems()
        {
            return await _context.Items.ToListAsync();
        }

        public async Task<List<Item>> GetActiveItems()
        {
            return await _context.Items
                .Where(i => i.IsActive)
                .ToListAsync();
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

        public async Task DeleteItem(int id)
        {
            var item = await GetItemById(id);
            if (item != null)
            {
                _context.Items.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public IQueryable<Item> GetItemsQueryable()
        {
            return _context.Items
                .Include(i => i.EquipmentStats)
                .AsNoTracking();
        }
    }
}
