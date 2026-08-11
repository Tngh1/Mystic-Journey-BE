using DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Quản lý lịch sử giao dịch mua hàng.
    // Admin APIs: Xem lịch sử giao dịch, thống kê doanh thu.
    public interface IPurchaseHistoryRepository
    {
        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // Đếm tổng số giao dịch đã thực hiện.
        Task<int> GetTotalTransactionsCount();

        // Tính tổng doanh thu từ tất cả giao dịch.
        Task<decimal> GetTotalRevenue();

        // Lấy toàn bộ lịch sử giao dịch, sắp xếp theo thời gian giảm dần.
        Task<List<PurchaseHistory>> GetAllPurchaseHistories();

        // Lấy lịch sử giao dịch của một người chơi cụ thể.
        Task<List<PurchaseHistory>> GetPurchasesByPlayerId(int playerProfileId);

        // Lấy lịch sử giao dịch có phân trang và tìm kiếm theo từ khóa.
        Task<(int TotalCount, List<PurchaseHistory> Histories)> GetPurchaseHistoriesPaged(int page, int pageSize, string? search, string? sortBy = null, string? sortOrder = null);

        // Tạo bản ghi giao dịch mua hàng mới.
        Task<PurchaseHistory> CreatePurchaseHistory(PurchaseHistory history);
    }
}
