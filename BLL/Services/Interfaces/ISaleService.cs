using BLL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Initializes a new default instance of the ISaleService class.
    public interface ISaleService
    {

        Task<List<PurchaseHistoryResponseDto>> GetSalesByPlayerId(int playerProfileId);
    }
}
