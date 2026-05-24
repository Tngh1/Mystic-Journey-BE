namespace DAL.Models
{
    public class GachaPullHistory
    {
        public int Id { get; set; }

        public int PlayerProfileId { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }

        public int GachaBannerId { get; set; }
        public GachaBanner? GachaBanner { get; set; }

        public int RewardItemId { get; set; }
        public Item? RewardItem { get; set; }

        public int PullCount { get; set; } = 1;
        public decimal CostSpent { get; set; } = 0;

        public DateTime PulledAt { get; set; } = DateTime.UtcNow;
    }
}