using DAL.Models;

namespace DAL.Repositories.Results
{
    // Executes currency spend status operation.
    public enum CurrencySpendStatus
    {
        Success,
        PlayerNotFound,
        InvalidAmount,
        UnsupportedCurrency,
        InsufficientCurrency
    }

    // Initializes a new default instance of the CurrencySpendResult class.
    public class CurrencySpendResult
    {
        // Executes status operation.
        public CurrencySpendStatus Status { get; set; }
        // Executes player profile operation.
        public PlayerProfile? PlayerProfile { get; set; }
        // Executes currency log operation.
        public PlayerCurrencyLog? CurrencyLog { get; set; }
        // Executes balance before operation.
        public decimal BalanceBefore { get; set; }
        // Executes balance after operation.
        public decimal BalanceAfter { get; set; }
    }
}
