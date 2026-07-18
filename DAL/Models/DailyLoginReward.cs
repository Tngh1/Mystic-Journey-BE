namespace DAL.Models
{
    public class DailyLoginReward
    {
        public int DailyLoginRewardId { get; set; }

        public int DayNumber { get; set; } = 1;

        // NULL = default (áp dụng mọi tháng nếu tháng đó chưa có override)
        // 1–12 = override riêng cho tháng này
        public int? Month { get; set; }

        // NULL = default, cụ thể = override cho năm này
        public int? Year { get; set; }

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
