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


        public async Task<(int TotalCount, List<ShopItem> Items)> GetShopItemsPaged(int page, int pageSize, string? search, string? currency, bool? isActive, string? sortBy = null, string? sortOrder = null)
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

            bool desc = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);
            query = (sortBy?.ToLowerInvariant()) switch
            {
                "name" => desc ? query.OrderByDescending(x => x.Item!.Name) : query.OrderBy(x => x.Item!.Name),
                "currency" => desc ? query.OrderByDescending(x => x.Currency) : query.OrderBy(x => x.Currency),
                "price" => desc ? query.OrderByDescending(x => x.Price) : query.OrderBy(x => x.Price),
                "stock" => desc ? query.OrderByDescending(x => x.Stock) : query.OrderBy(x => x.Stock),
                "isactive" => desc ? query.OrderByDescending(x => x.IsActive) : query.OrderBy(x => x.IsActive),
                _ => desc ? query.OrderByDescending(x => x.ShopItemId) : query.OrderBy(x => x.ShopItemId),
            };

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (totalCount, items);
        }
    }
}
