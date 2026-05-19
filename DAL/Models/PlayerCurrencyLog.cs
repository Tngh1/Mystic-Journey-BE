namespace DAL.Models
{
    public class PlayerCurrencyLog
    {
        public Guid Id { get; set; }

        public Guid PlayerProfileId { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }

        // Currencies: Gold, Gems
        public string Currency { get; set; } = "Gold";
        // TransactionTypes: Earn, Spend, Refund, Reward
        public string Type { get; set; } = "Earn";

        public decimal Amount { get; set; } = 0;
        public decimal BalanceAfter { get; set; } = 0;

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}