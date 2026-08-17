namespace DAL.Models
{
    // Initializes a new default instance of the PlayerChest class.
    public class PlayerChest
    {
        // Executes player chest id operation.
        public int PlayerChestId { get; set; }

        // Executes player profile id operation.
        public int PlayerProfileId { get; set; }
        // Executes player profile operation.
        public PlayerProfile? PlayerProfile { get; set; }

        // Executes chest id operation.
        public int ChestId { get; set; }
        // Executes chest operation.
        public Chest? Chest { get; set; }

        // Executes is opened operation.
        public bool IsOpened { get; set; } = false;
        // Executes received at operation.
        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
        // Executes opened at operation.
        public DateTime? OpenedAt { get; set; }
    }
}
