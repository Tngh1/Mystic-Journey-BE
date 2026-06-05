using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class ShopItemRepository : IShopItemRepository
    {
        private readonly MysticJourneyDbContext _context;

        public ShopItemRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<ShopItem?> GetShopItemById(int id)
        {
            return await _context.ShopItems
                .FirstOrDefaultAsync(s => s.ShopItemId == id);
        }

        public async Task<ShopItem?> GetShopItemByIdWithItem(int id)
        {
            return await _context.ShopItems
                .Include(s => s.Item)
                .FirstOrDefaultAsync(s => s.ShopItemId == id);
        }

        public async Task<List<ShopItem>> GetAllShopItems()
        {
            return await _context.ShopItems.ToListAsync();
        }

        public async Task<List<ShopItem>> GetActiveShopItems()
        {
            return await _context.ShopItems
                .Include(s => s.Item)
                .Where(s => s.IsActive)
                .ToListAsync();
        }

        public async Task<ShopItem> CreateShopItem(ShopItem shopItem)
        {
            await _context.ShopItems.AddAsync(shopItem);
            await _context.SaveChangesAsync();
            return shopItem;
        }

        public async Task<ShopItem> UpdateShopItem(ShopItem shopItem)
        {
_context.ShopItems.Update(shopItem);
            await _context.SaveChangesAsync();
            return shopItem;
        }

        public async Task DeleteShopItem(int id)
        {
            var shopItem = await GetShopItemById(id);
            if (shopItem != null)
            {
                _context.ShopItems.Remove(shopItem);
                await _context.SaveChangesAsync();
            }
        }

        public IQueryable<ShopItem> GetShopItemsQueryable()
        {
            return _context.ShopItems
                .Include(s => s.Item)
                .AsNoTracking();
        }
    }
}
