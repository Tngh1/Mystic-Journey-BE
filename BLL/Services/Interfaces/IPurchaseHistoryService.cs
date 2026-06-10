using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IPurchaseHistoryService
    {
        Task<List<PurchaseHistoryResponseDto>> GetAllPurchaseHistories();
        Task<List<PurchaseHistoryResponseDto>> GetPurchasesByPlayerId(int playerProfileId);
        Task<PagedResultDto<PurchaseHistoryResponseDto>> GetPurchaseHistoriesPaged(int page, int pageSize, string? search = null);
    }
}
