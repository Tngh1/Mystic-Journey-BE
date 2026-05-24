using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class GameSetting
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Value { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

    public class GameAnnouncement
    {
        public int Id { get; set; }

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
