using BLL.DTOs;

namespace BLL.Services.Interfaces
{
    public interface ICurrencyService
    {
        Task<CurrencyBalanceResponseDto> GetBalance(int playerProfileId);
        Task<CurrencySpendResponseDto> SpendCurrency(int playerProfileId, SpendCurrencyRequestDto request);
    }
}
