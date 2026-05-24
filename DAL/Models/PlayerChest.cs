namespace DAL.Models
{
    public class PlayerChest
    {
        public int Id { get; set; }

        public int PlayerProfileId { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }

        public int ChestId { get; set; }
        public Chest? Chest { get; set; }

        public bool IsOpened { get; set; } = false;
        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
        public DateTime? OpenedAt { get; set; }
    }
}
