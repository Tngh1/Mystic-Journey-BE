using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // Initializes a new default instance of the PlayerDailyLoginResponseDto class.
    public class PlayerDailyLoginResponseDto
    {
        // Executes player daily login id operation.
        public int PlayerDailyLoginId { get; set; }
        // Executes player profile id operation.
        public int PlayerProfileId { get; set; }
        // Executes current streak operation.
        public int CurrentStreak { get; set; }
        // Executes total days claimed operation.
        public int TotalDaysClaimed { get; set; }
        // Executes last claimed at operation.
        public DateTime? LastClaimedAt { get; set; }
        // Executes is claimed today operation.
        public bool IsClaimedToday { get; set; }
        // Executes current year operation.
        public int CurrentYear { get; set; }
        // Executes current month operation.
        public int CurrentMonth { get; set; }
        // Executes retro claim count operation.
        public int RetroClaimCount { get; set; }
        // Executes claimed days operation.
        public List<int> ClaimedDays { get; set; } = new List<int>();
    }

    // Executes retro claim request dto operation.
    public class RetroClaimRequestDto
    {
        // Executes day number operation.
        [Range(1, 31, ErrorMessage = "DayNumber must be between 1 and 31.")]
        public int DayNumber { get; set; }
    }

    // Executes claim daily reward response dto operation.
    public class ClaimDailyRewardResponseDto
    {
        // Executes success operation.
        public bool Success { get; set; }
        // Executes message operation.
        public string Message { get; set; } = string.Empty;
        // Executes current streak operation.
        public int CurrentStreak { get; set; }
        // Executes total days claimed operation.
        public int TotalDaysClaimed { get; set; }
        // Supported reward types: Gold, Gems, EXP, Energy, or Item; Item rewards also require an item identifier and quantity.
        public string RewardType { get; set; } = string.Empty;
        // Executes reward value operation.
        public decimal RewardValue { get; set; }
        // Executes reward item id operation.
        public int? RewardItemId { get; set; }
        // Executes reward item name operation.
        public string? RewardItemName { get; set; }
        // Executes reward item quantity operation.
        public int RewardItemQuantity { get; set; }
    }
}
