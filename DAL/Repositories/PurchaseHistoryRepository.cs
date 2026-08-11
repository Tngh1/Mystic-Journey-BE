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
    /// <summary>
    /// Triển khai các thao tác truy cập dữ liệu cho lịch sử giao dịch mua hàng sử dụng Entity Framework.
    /// </summary>
    public class PurchaseHistoryRepository : IPurchaseHistoryRepository
    {
        private readonly MysticJourneyDbContext _context;

        public PurchaseHistoryRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        // ── Thống kê ──

        /// <summary>Đếm tổng số giao dịch đã thực hiện trong hệ thống.</summary>
        public async Task<int> GetTotalTransactionsCount()
        {
            return await _context.PurchaseHistories.CountAsync();
        }

        /// <summary>Tính tổng doanh thu từ tất cả giao dịch.</summary>
        public async Task<decimal> GetTotalRevenue()
        {
            return await _context.PurchaseHistories.SumAsync(p => p.TotalPrice);
        }

        // ── Truy vấn ──

        /// <summary>Tạo bản ghi giao dịch mua hàng mới.</summary>
        public async Task<PurchaseHistory> CreatePurchaseHistory(PurchaseHistory history)
        {
            await _context.PurchaseHistories.AddAsync(history);
            await _context.SaveChangesAsync();
            return history;
        }

        /// <summary>Lấy toàn bộ lịch sử giao dịch, kèm thông tin người mua và sản phẩm, sắp xếp theo thời gian giảm dần.</summary>
        public async Task<List<PurchaseHistory>> GetAllPurchaseHistories()
        {
            return await _context.PurchaseHistories
                .Include(p => p.PlayerProfile)
                .Include(p => p.ShopItem)
                    .ThenInclude(s => s!.Item)
                .OrderByDescending(p => p.PurchasedAt)
                .ToListAsync();
        }

        /// <summary>Lấy lịch sử giao dịch của một người chơi cụ thể.</summary>
        public async Task<List<PurchaseHistory>> GetPurchasesByPlayerId(int playerProfileId)
        {
            return await _context.PurchaseHistories
                .Include(p => p.PlayerProfile)
                .Include(p => p.ShopItem)
                    .ThenInclude(s => s!.Item)
                .Where(p => p.PlayerProfileId == playerProfileId)
                .OrderByDescending(p => p.PurchasedAt)
                .ToListAsync();
        }

        /// <summary>Lấy lịch sử giao dịch có phân trang và tìm kiếm theo tên người chơi, tên sản phẩm hoặc loại tiền.</summary>
        public async Task<(int TotalCount, List<PurchaseHistory> Histories)> GetPurchaseHistoriesPaged(int page, int pageSize, string? search, string? sortBy = null, string? sortOrder = null)
        {
            var query = _context.PurchaseHistories
                .Include(p => p.PlayerProfile)
                .Include(p => p.ShopItem)
                    .ThenInclude(s => s!.Item)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p =>
                    (p.PlayerProfile != null && p.PlayerProfile.DisplayName.Contains(search)) ||
                    (p.ShopItem != null && p.ShopItem.Item != null && p.ShopItem.Item.Name.Contains(search)) ||
                    (p.ShopItem != null && p.ShopItem.Currency.Contains(search)));
            }

            bool desc = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);
            query = (sortBy?.ToLowerInvariant()) switch
            {
                "playername" => desc ? query.OrderByDescending(x => x.PlayerProfile!.DisplayName) : query.OrderBy(x => x.PlayerProfile!.DisplayName),
                "itemname" => desc ? query.OrderByDescending(x => x.ShopItem!.Item!.Name) : query.OrderBy(x => x.ShopItem!.Item!.Name),
                "currency" => desc ? query.OrderByDescending(x => x.ShopItem!.Currency) : query.OrderBy(x => x.ShopItem!.Currency),
                "pricepaid" => desc ? query.OrderByDescending(x => x.TotalPrice) : query.OrderBy(x => x.TotalPrice),
                "purchasedat" => desc ? query.OrderByDescending(x => x.PurchasedAt) : query.OrderBy(x => x.PurchasedAt),
                _ => desc ? query.OrderByDescending(x => x.PurchasedAt) : query.OrderBy(x => x.PurchasedAt),
            };

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (totalCount, items);
        }
    }
}
