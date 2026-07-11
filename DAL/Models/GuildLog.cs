using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class GuildLog
    {
        public int GuildLogId { get; set; }

        public int GuildId { get; set; }
        public Guild? Guild { get; set; }

        // FK references (may be null for system events)
        public int? ActorProfileId { get; set; }
        public PlayerProfile? Actor { get; set; }

        public int? TargetProfileId { get; set; }
        public PlayerProfile? Target { get; set; }

        // Snapshot: store display names at time of action so renames don't corrupt history
        [MaxLength(100)]
        public string? ActorName { get; set; }

        [MaxLength(100)]
        public string? TargetName { get; set; }

        public GuildLogAction Action { get; set; }

        [MaxLength(300)]
        public string? Detail { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
