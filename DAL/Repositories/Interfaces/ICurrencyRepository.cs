using DAL.Models;
using DAL.Repositories.Results;

namespace DAL.Repositories.Interfaces
{
    // Initializes a new default instance of the ICurrencyRepository class.
    public interface ICurrencyRepository
    {
        Task<PlayerProfile?> GetPlayerProfile(int playerProfileId);

        Task<CurrencySpendResult> TrySpendCurrency(
            int playerProfileId,
            // Supported currencies: Gold or Gems; the selected currency determines which player balance is charged or credited.
            string currency,
            decimal amount,
            string reason,
            DateTime utcNow);
    }
}
