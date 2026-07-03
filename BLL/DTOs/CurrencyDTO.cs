using System;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    public class CurrencyBalanceResponseDto
    {
        public int PlayerProfileId { get; set; }
        public decimal Gold { get; set; }
        public decimal Gems { get; set; }
        public DateTime ServerTimeUtc { get; set; }
    }

    public class SpendCurrencyRequestDto
    {
        [Required(ErrorMessage = "Currency is required.")]
        [RegularExpression("^(Gold|Gems)$", ErrorMessage = "Currency must be Gold or Gems.")]
        public string Currency { get; set; } = "Gold";

        [Range(typeof(decimal), "0.01", "79228162514264337593543950335", ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Reason is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Reason must be between 2 and 100 characters.")]
        public string Reason { get; set; } = "Spend";
    }

    public class CurrencySpendResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public decimal AmountSpent { get; set; }
        public decimal BalanceBefore { get; set; }
        public decimal BalanceAfter { get; set; }
        public CurrencyBalanceResponseDto Balance { get; set; } = new();
        public PlayerCurrencyLogResponseDto? Transaction { get; set; }
    }
}
