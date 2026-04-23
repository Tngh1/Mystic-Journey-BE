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
    public class ShopItemRepository : IShopItemRepository
    {
        private readonly MysticJourneyDbContext _context;

        public ShopItemRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<ShopItem?> GetByIdAsync(Guid shopItemId)
        {
            return await _context.ShopItems
                .Include(si => si.Item)
                .FirstOrDefaultAsync(si => si.Id == shopItemId && si.IsActive);
        }

        public async Task<ShopItem?> GetByIdWithItemAsync(Guid shopItemId)
        {
            return await _context.ShopItems
                .Include(si => si.Item)
                    .ThenInclude(i => i!.EquipmentStats)
                .FirstOrDefaultAsync(si => si.Id == shopItemId && si.IsActive);
        }

        public async Task<List<ShopItem>> GetAllActiveAsync()
        {
            return await _context.ShopItems
                .Include(si => si.Item)
                .Where(si => si.IsActive)
                .OrderBy(si => si.Item!.Name)
                .ToListAsync();
        }

        public async Task<List<ShopItem>> GetAvailableNowAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.ShopItems
                .Include(si => si.Item)
                .Where(si => si.IsActive &&
                            (!si.AvailableFrom.HasValue || si.AvailableFrom <= now) &&
                            (!si.AvailableTo.HasValue || si.AvailableTo >= now))
                .OrderBy(si => si.Item!.Name)
                .ToListAsync();
        }

        public async Task<ShopItem> CreateAsync(ShopItem shopItem)
        {
            await _context.ShopItems.AddAsync(shopItem);
            await _context.SaveChangesAsync();
            return shopItem;
        }

        public async Task<ShopItem> UpdateAsync(ShopItem shopItem)
        {
            _context.ShopItems.Update(shopItem);
            await _context.SaveChangesAsync();
            return shopItem;
        }
    }
}
