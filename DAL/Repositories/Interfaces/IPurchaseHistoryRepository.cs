using DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IPurchaseHistoryRepository
    {
        Task<PurchaseHistory?> GetByIdAsync(Guid purchaseId);
        Task<List<PurchaseHistory>> GetByPlayerProfileIdAsync(Guid playerProfileId, int pageNumber = 1, int pageSize = 20);
        Task<int> GetDailyPurchaseCountAsync(Guid playerProfileId, Guid shopItemId);
        Task<PurchaseHistory> CreateAsync(PurchaseHistory purchase);
        Task<int> GetTotalCountAsync(Guid playerProfileId);
    }
}
