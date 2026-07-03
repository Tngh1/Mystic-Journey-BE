using DAL.Models;

namespace DAL.Repositories.Results
{
    public enum CurrencySpendStatus
    {
        Success,
        PlayerNotFound,
        InvalidAmount,
        UnsupportedCurrency,
        InsufficientCurrency
    }

    public class CurrencySpendResult
    {
        public CurrencySpendStatus Status { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }
        public PlayerCurrencyLog? CurrencyLog { get; set; }
        public decimal BalanceBefore { get; set; }
        public decimal BalanceAfter { get; set; }
    }
}
