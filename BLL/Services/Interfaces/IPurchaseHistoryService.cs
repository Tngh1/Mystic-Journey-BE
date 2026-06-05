using BLL.DTOs;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IPurchaseHistoryService
    {
        Task<List<PurchaseHistoryResponseDto>> GetAllPurchaseHistories();
        Task<List<PurchaseHistoryResponseDto>> GetPurchasesByPlayerId(int playerProfileId);
        IQueryable<PurchaseHistoryResponseDto> GetPurchaseHistoriesQueryable();
    }
}
