namespace DAL.Models
{
    public class PlayerAnnouncement
    {
        public Guid Id { get; set; }

        public Guid PlayerProfileId { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }

        public Guid AnnouncementId { get; set; }
        public GameAnnouncement? Announcement { get; set; }

        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }
    }
}
