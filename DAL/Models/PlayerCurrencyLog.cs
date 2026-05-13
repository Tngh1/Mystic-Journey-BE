namespace DAL.Models
{
    public class PlayerCurrencyLog
    {
        public Guid Id { get; set; }

        public Guid PlayerProfileId { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }

        public CurrencyType Currency { get; set; } = CurrencyType.Gold;
        public TransactionType Type { get; set; } = TransactionType.Earn;

        public decimal Amount { get; set; } = 0;
        public decimal BalanceAfter { get; set; } = 0;

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public enum CurrencyType
        {
            Gold = 0,
            Gems = 1
        }

        public enum TransactionType
        {
            Earn = 0,
            Spend = 1,
            Refund = 2,
            Reward = 3
        }
    }
}