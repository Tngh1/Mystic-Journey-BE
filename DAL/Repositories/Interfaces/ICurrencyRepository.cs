using DAL.Models;
using DAL.Repositories.Results;

namespace DAL.Repositories.Interfaces
{
    public interface ICurrencyRepository
    {
        Task<PlayerProfile?> GetPlayerProfile(int playerProfileId);

        Task<CurrencySpendResult> TrySpendCurrency(
            int playerProfileId,
            string currency,
            decimal amount,
            string reason,
            DateTime utcNow);
    }
}
