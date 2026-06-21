using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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


        public async Task<(int TotalCount, List<ShopItem> Items)> GetShopItemsPaged(int page, int pageSize, string? search, string? currency, bool? isActive)
        {
            var query = _context.ShopItems
                .Include(s => s.Item)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Item != null && x.Item.Name.Contains(search));
            }
            if (!string.IsNullOrEmpty(currency))
            {
                query = query.Where(x => x.Currency == currency);
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
