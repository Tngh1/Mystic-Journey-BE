namespace DAL.Models
{
    // Tracks which players have discovered which monsters. Enables the UI
    // to show '?' for undiscovered monsters per player.
    public class PlayerMonsterDiscovery
    {
        public int PlayerMonsterDiscoveryId { get; set; }

        public int PlayerProfileId { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }

        public int MonsterId { get; set; }
        public Monster? Monster { get; set; }

        public bool IsDiscovered { get; set; } = false;

        public DateTime? DiscoveredAt { get; set; }

        public int TimesDefeated { get; set; } = 0;
    }
}
