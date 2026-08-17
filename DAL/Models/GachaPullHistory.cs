namespace DAL.Models
{
    // Initializes a new default instance of the GachaPullHistory class.
    public class GachaPullHistory
    {
        // Executes gacha pull history id operation.
        public int GachaPullHistoryId { get; set; }

        // Executes player profile id operation.
        public int PlayerProfileId { get; set; }
        // Executes player profile operation.
        public PlayerProfile? PlayerProfile { get; set; }

        // Executes gacha banner id operation.
        public int GachaBannerId { get; set; }
        // Executes gacha banner operation.
        public GachaBanner? GachaBanner { get; set; }

        // Executes reward item id operation.
        public int RewardItemId { get; set; }
        // Executes reward item operation.
        public Item? RewardItem { get; set; }

        // Executes pull count operation.
        public int PullCount { get; set; } = 1;
        // Executes cost spent operation.
        public decimal CostSpent { get; set; } = 0;

        // Executes pulled at operation.
        public DateTime PulledAt { get; set; } = DateTime.UtcNow;
    }
}
