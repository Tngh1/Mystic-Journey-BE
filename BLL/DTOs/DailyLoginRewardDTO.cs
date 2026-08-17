using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{

    // Initializes a new default instance of the DailyLoginRewardResponseDto class.
    public class DailyLoginRewardResponseDto
    {
        // Executes daily login reward id operation.
        public int DailyLoginRewardId { get; set; }
        // Executes day number operation.
        public int DayNumber { get; set; }

        // Executes month operation.
        public int? Month { get; set; }
        // Executes year operation.
        public int? Year { get; set; }

        // Executes is default operation.
        public bool IsDefault => Month == null && Year == null;

        // Supported reward types: Gold, Gems, EXP, Energy, or Item; Item rewards also require an item identifier and quantity.
        public string RewardType { get; set; } = "Gold";
        // Executes reward value operation.
        public decimal RewardValue { get; set; }
        // Executes reward item id operation.
        public int? RewardItemId { get; set; }
        // Executes reward item name operation.
        public string? RewardItemName { get; set; }
        // Executes reward item quantity operation.
        public int RewardItemQuantity { get; set; }
        // Executes is active operation.
        public bool IsActive { get; set; }
        // Executes created at operation.
        public DateTime CreatedAt { get; set; }
    }

    // Executes create daily login reward request dto operation.
    public class CreateDailyLoginRewardRequestDto
    {
        // Executes day number operation.
        [Range(1, 31)]
        public int DayNumber { get; set; }

        // Executes month operation.
        [Range(1, 12)]
        public int? Month { get; set; }

        // Executes year operation.
        [Range(2024, 2100)]
        public int? Year { get; set; }

        // Supported reward types: Gold, Gems, EXP, Energy, or Item; Item rewards also require an item identifier and quantity.
        public string RewardType { get; set; } = "Gold";
        // Executes reward value operation.
        public decimal RewardValue { get; set; }
        // Executes reward item id operation.
        public int? RewardItemId { get; set; }
        // Executes reward item quantity operation.
        public int RewardItemQuantity { get; set; }
        // Executes is active operation.
        public bool IsActive { get; set; } = true;
    }

    // Executes update daily login reward request dto operation.
    public class UpdateDailyLoginRewardRequestDto
    {
        // Supported reward types: Gold, Gems, EXP, Energy, or Item; Item rewards also require an item identifier and quantity.
        public string RewardType { get; set; } = "Gold";
        // Executes reward value operation.
        public decimal RewardValue { get; set; }
        // Executes reward item id operation.
        public int? RewardItemId { get; set; }
        // Executes reward item quantity operation.
        public int RewardItemQuantity { get; set; }
        // Executes is active operation.
        public bool IsActive { get; set; } = true;
    }
}
