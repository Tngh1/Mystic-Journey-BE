namespace DAL.Models
{
    // Initializes a new default instance of the PlayerShopRefreshState class.
    public class PlayerShopRefreshState
    {
        // Executes player shop refresh state id operation.
        public int PlayerShopRefreshStateId { get; set; }

        // Executes player profile id operation.
        public int PlayerProfileId { get; set; }
        // Executes player profile operation.
        public PlayerProfile? PlayerProfile { get; set; }

        // Executes shop date utc operation.
        public DateTime ShopDateUtc { get; set; }
        // Executes refresh count operation.
        public int RefreshCount { get; set; }
        // Executes created at operation.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // Executes last refresh at operation.
        public DateTime LastRefreshAt { get; set; } = DateTime.UtcNow;
    }
}
