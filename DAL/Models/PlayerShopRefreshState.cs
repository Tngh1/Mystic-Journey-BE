namespace DAL.Models
{
    public class PlayerShopRefreshState
    {
        public int PlayerShopRefreshStateId { get; set; }

        public int PlayerProfileId { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }

        public DateTime ShopDateUtc { get; set; }
        public int RefreshCount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastRefreshAt { get; set; } = DateTime.UtcNow;
    }
}
