using BLL.DTOs;

namespace BLL.Services.Interfaces
{
    // Initializes a new default instance of the ICurrencyService class.
    public interface ICurrencyService
    {
        Task<CurrencyBalanceResponseDto> GetBalance(int playerProfileId);
        Task<CurrencySpendResponseDto> SpendCurrency(int playerProfileId, SpendCurrencyRequestDto request);
    }
}
