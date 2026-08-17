namespace DAL.Models
{
    // Initializes a new default instance of the DailyLoginReward class.
    public class DailyLoginReward
    {
        // Executes daily login reward id operation.
        public int DailyLoginRewardId { get; set; }

        // Executes day number operation.
        public int DayNumber { get; set; } = 1;

        // Executes month operation.
        public int? Month { get; set; }

        // Executes year operation.
        public int? Year { get; set; }

        // Supported reward types: Gold, Gems, EXP, Energy, or Item; Item rewards also require an item identifier and quantity.
        public string RewardType { get; set; } = "Gold";
        // Executes reward value operation.
        public decimal RewardValue { get; set; } = 0;

        // Executes reward item id operation.
        public int? RewardItemId { get; set; }
        // Executes reward item operation.
        public Item? RewardItem { get; set; }
        // Executes reward item quantity operation.
        public int RewardItemQuantity { get; set; } = 0;

        // Executes is active operation.
        public bool IsActive { get; set; } = true;

        // Executes created at operation.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
