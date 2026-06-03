namespace DAL.Models
{
    public class PlayerAnnouncement
    {
        public int PlayerAnnouncementId { get; set; }

        public int PlayerProfileId { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }

        public int GameAnnouncementId { get; set; }
        public GameAnnouncement? GameAnnouncement { get; set; }

        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }
    }
}
