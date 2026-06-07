using BLL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface ISaleService
    {
        Task<List<PurchaseHistoryResponseDto>> GetAllSales();
        Task<List<PurchaseHistoryResponseDto>> GetSalesByPlayerId(int playerProfileId);
    }
}
