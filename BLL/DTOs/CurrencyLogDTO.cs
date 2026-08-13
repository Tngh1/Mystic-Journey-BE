namespace BLL.DTOs
{
    // ============ PlayerCurrencyLog ============
    public class PlayerCurrencyLogResponseDto
    {
        public int PlayerCurrencyLogId { get; set; }
        public int PlayerProfileId { get; set; }
        public string Currency { get; set; } = "Gold";
        public string Type { get; set; } = "Earn";
        public decimal Amount { get; set; }
        public decimal BalanceAfter { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
