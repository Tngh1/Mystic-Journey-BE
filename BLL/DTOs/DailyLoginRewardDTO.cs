using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // ============ DailyLoginReward ============

    public class DailyLoginRewardResponseDto
    {
        public int DailyLoginRewardId { get; set; }
        public int DayNumber { get; set; }

        // NULL = default record; có giá trị = override tháng/năm cụ thể
        public int? Month { get; set; }
        public int? Year { get; set; }

        // true nếu đây là record default (Month=null)
        public bool IsDefault => Month == null && Year == null;

        public string RewardType { get; set; } = "Gold";
        public decimal RewardValue { get; set; }
        public int? RewardItemId { get; set; }
        public string? RewardItemName { get; set; }
        public int RewardItemQuantity { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateDailyLoginRewardRequestDto
    {
        [Range(1, 31)]
        public int DayNumber { get; set; }

        // NULL = default; có giá trị = override tháng/năm cụ thể
        [Range(1, 12)]
        public int? Month { get; set; }

        [Range(2024, 2100)]
        public int? Year { get; set; }

        // RewardTypes: Gold, Gems, Item, Energy
        public string RewardType { get; set; } = "Gold";
        public decimal RewardValue { get; set; }
        public int? RewardItemId { get; set; }
        public int RewardItemQuantity { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateDailyLoginRewardRequestDto
    {
        // Không cho đổi Month/Year khi update (phải delete + create mới)
        public string RewardType { get; set; } = "Gold";
        public decimal RewardValue { get; set; }
        public int? RewardItemId { get; set; }
        public int RewardItemQuantity { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
