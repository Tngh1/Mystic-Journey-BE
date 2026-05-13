namespace DAL.Models
{
    public class GachaPullHistory
    {
        public Guid Id { get; set; }

        public Guid PlayerProfileId { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }

        public Guid GachaBannerId { get; set; }
        public GachaBanner? GachaBanner { get; set; }

        public Guid RewardItemId { get; set; }
        public Item? RewardItem { get; set; }

        public int PullCount { get; set; } = 1;
        public decimal CostSpent { get; set; } = 0;

        public DateTime PulledAt { get; set; } = DateTime.UtcNow;
    }
}