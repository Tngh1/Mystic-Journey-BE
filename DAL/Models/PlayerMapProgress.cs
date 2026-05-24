namespace DAL.Models
{
    public class PlayerMapProgress
    {
        public int Id { get; set; }

        public int PlayerProfileId { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }

        public int GameMapId { get; set; }
        public GameMap? GameMap { get; set; }

        public int Visits { get; set; } = 0;
        public int MonstersDefeated { get; set; } = 0;
        public bool IsCompleted { get; set; } = false;

        public DateTime LastVisitedAt { get; set; } = DateTime.UtcNow;
    }
}
