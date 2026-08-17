using DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Initializes a new default instance of the IPurchaseHistoryRepository class.
    public interface IPurchaseHistoryRepository
    {

        Task<int> GetTotalTransactionsCount();

        Task<decimal> GetTotalRevenue();

        Task<List<PurchaseHistory>> GetAllPurchaseHistories();

        Task<List<PurchaseHistory>> GetPurchasesByPlayerId(int playerProfileId);

        Task<(int TotalCount, List<PurchaseHistory> Histories)> GetPurchaseHistoriesPaged(int page, int pageSize, string? search, string? sortBy = null, string? sortOrder = null);

        Task<PurchaseHistory> CreatePurchaseHistory(PurchaseHistory history);
    }
}
