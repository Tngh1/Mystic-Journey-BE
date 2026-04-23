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
    public class PurchaseHistoryRepository : IPurchaseHistoryRepository
    {
        private readonly MysticJourneyDbContext _context;

        public PurchaseHistoryRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<PurchaseHistory?> GetByIdAsync(Guid purchaseId)
        {
            return await _context.PurchaseHistories
                .Include(ph => ph.ShopItem)
                    .ThenInclude(si => si!.Item)
                .FirstOrDefaultAsync(ph => ph.Id == purchaseId);
        }

        public async Task<List<PurchaseHistory>> GetByPlayerProfileIdAsync(Guid playerProfileId, int pageNumber = 1, int pageSize = 20)
        {
            return await _context.PurchaseHistories
                .Include(ph => ph.ShopItem)
                    .ThenInclude(si => si!.Item)
                .Where(ph => ph.PlayerProfileId == playerProfileId)
                .OrderByDescending(ph => ph.PurchasedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetDailyPurchaseCountAsync(Guid playerProfileId, Guid shopItemId)
        {
            var startOfDay = DateTime.UtcNow.Date;
            return await _context.PurchaseHistories
                .Where(ph => ph.PlayerProfileId == playerProfileId &&
                             ph.ShopItemId == shopItemId &&
                             ph.PurchasedAt >= startOfDay)
                .SumAsync(ph => ph.Quantity);
        }

        public async Task<PurchaseHistory> CreateAsync(PurchaseHistory purchase)
        {
            await _context.PurchaseHistories.AddAsync(purchase);
            await _context.SaveChangesAsync();
            return purchase;
        }

        public async Task<int> GetTotalCountAsync(Guid playerProfileId)
        {
            return await _context.PurchaseHistories
                .Where(ph => ph.PlayerProfileId == playerProfileId)
                .CountAsync();
        }
    }
}
