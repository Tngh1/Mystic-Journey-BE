using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class GameAnnouncement
    {
        public int GameAnnouncementId { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        // Types: Info, Event, Maintenance, Update, Alert
        public string Type { get; set; } = "Info";

        public bool IsActive { get; set; } = true;

        public DateTime StartsAt { get; set; }
        public DateTime? EndsAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<PlayerAnnouncement> PlayerAnnouncements { get; set; } = new List<PlayerAnnouncement>();
    }
}
