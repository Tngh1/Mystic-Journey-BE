using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // ============ PlayerDailyLogin ============
    public class PlayerDailyLoginResponseDto
    {
        public int PlayerDailyLoginId { get; set; }
        public int PlayerProfileId { get; set; }
        public int CurrentStreak { get; set; }
        public int TotalDaysClaimed { get; set; }
        public DateTime? LastClaimedAt { get; set; }
        public bool IsClaimedToday { get; set; }
        public int CurrentYear { get; set; }
        public int CurrentMonth { get; set; }
        public int RetroClaimCount { get; set; }
        public List<int> ClaimedDays { get; set; } = new List<int>();
    }

    public class RetroClaimRequestDto
    {
        // Chan DayNumber am: khi da claim het cac ngay truoc, maxMissedDay = -1 nen
        // request DayNumber = -1 lot qua moi guard trong RetroactiveClaimDailyLoginReward.
        [Range(1, 31, ErrorMessage = "DayNumber must be between 1 and 31.")]
        public int DayNumber { get; set; }
    }

    public class ClaimDailyRewardResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int CurrentStreak { get; set; }
        public int TotalDaysClaimed { get; set; }
        public string RewardType { get; set; } = string.Empty;
        public decimal RewardValue { get; set; }
        public int? RewardItemId { get; set; }
        public string? RewardItemName { get; set; }
        public int RewardItemQuantity { get; set; }
    }
}
