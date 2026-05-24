namespace DAL.Models
{
    public class DailyLoginReward
    {
        public int Id { get; set; }

        public int DayNumber { get; set; } = 1;

        // RewardTypes: Gold, Gems, Item, Energy
        public string RewardType { get; set; } = "Gold";
        public decimal RewardValue { get; set; } = 0;

        public int? RewardItemId { get; set; }
        public Item? RewardItem { get; set; }
        public int RewardItemQuantity { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
