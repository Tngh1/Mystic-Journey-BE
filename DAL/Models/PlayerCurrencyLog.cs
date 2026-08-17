namespace DAL.Models
{
    // Initializes a new default instance of the PlayerCurrencyLog class.
    public class PlayerCurrencyLog
    {
        // Executes player currency log id operation.
        public int PlayerCurrencyLogId { get; set; }

        // Executes player profile id operation.
        public int PlayerProfileId { get; set; }
        // Executes player profile operation.
        public PlayerProfile? PlayerProfile { get; set; }

        // Supported currencies: Gold or Gems; the selected currency determines which player balance is charged or credited.
        public string Currency { get; set; } = "Gold";
        // Executes type operation.
        public string Type { get; set; } = "Earn";

        // Executes amount operation.
        public decimal Amount { get; set; } = 0;
        // Executes balance after operation.
        public decimal BalanceAfter { get; set; } = 0;

        // Executes note operation.
        public string? Note { get; set; }

        // Executes created at operation.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
