using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // ============ DailyLoginReward ============
    public class DailyLoginRewardResponseDto
    {
        public int DailyLoginRewardId { get; set; }
        public int DayNumber { get; set; }
        public string RewardType { get; set; } = "Gold";
        public decimal RewardValue { get; set; }
        public int? RewardItemId { get; set; }
        public string? RewardItemName { get; set; }
        public int RewardItemQuantity { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateDailyLoginRewardRequestDto
    {
        [Range(1, 365)]
        public int DayNumber { get; set; }

        public string RewardType { get; set; } = "Gold";
        public decimal RewardValue { get; set; }
        public int? RewardItemId { get; set; }
        public int RewardItemQuantity { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
