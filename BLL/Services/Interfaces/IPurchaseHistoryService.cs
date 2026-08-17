using BLL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Initializes a new default instance of the IPurchaseHistoryService class.
    public interface IPurchaseHistoryService
    {

        Task<List<PurchaseHistoryResponseDto>> GetPurchasesByPlayerId(int playerProfileId);


        Task<PagedResultDto<PurchaseHistoryResponseDto>> GetPurchaseHistoriesPaged(int page, int pageSize, string? search = null, string? sortBy = null, string? sortOrder = null);

        Task<List<PurchaseHistoryResponseDto>> GetAllPurchaseHistories();
    }
}
