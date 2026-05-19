namespace DAL.Models
{
    public class PlayerChest
    {
        public Guid Id { get; set; }

        public Guid PlayerProfileId { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }

        public Guid ChestId { get; set; }
        public Chest? Chest { get; set; }

        public bool IsOpened { get; set; } = false;
        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
        public DateTime? OpenedAt { get; set; }
    }
}
