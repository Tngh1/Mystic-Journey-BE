namespace DAL.Models
{
    // Initializes a new default instance of the PlayerMonsterDiscovery class.
    public class PlayerMonsterDiscovery
    {
        // Executes player monster discovery id operation.
        public int PlayerMonsterDiscoveryId { get; set; }

        // Executes player profile id operation.
        public int PlayerProfileId { get; set; }
        // Executes player profile operation.
        public PlayerProfile? PlayerProfile { get; set; }

        // Executes monster id operation.
        public int MonsterId { get; set; }
        // Executes monster operation.
        public Monster? Monster { get; set; }

        // Executes is discovered operation.
        public bool IsDiscovered { get; set; } = false;

        // Executes discovered at operation.
        public DateTime? DiscoveredAt { get; set; }

        // Executes times defeated operation.
        public int TimesDefeated { get; set; } = 0;
    }
}
