using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    // Initializes a new default instance of the GuildLog class.
    public class GuildLog
    {
        // Executes guild log id operation.
        public int GuildLogId { get; set; }

        // Executes guild id operation.
        public int GuildId { get; set; }
        // Executes guild operation.
        public Guild? Guild { get; set; }

        // Executes actor profile id operation.
        public int? ActorProfileId { get; set; }
        // Executes actor operation.
        public PlayerProfile? Actor { get; set; }

        // Executes target profile id operation.
        public int? TargetProfileId { get; set; }
        // Executes target operation.
        public PlayerProfile? Target { get; set; }

        // Executes actor name operation.
        [MaxLength(100)]
        public string? ActorName { get; set; }

        // Executes target name operation.
        [MaxLength(100)]
        public string? TargetName { get; set; }

        // Executes action operation.
        public GuildLogAction Action { get; set; }

        // Executes detail operation.
        [MaxLength(300)]
        public string? Detail { get; set; }

        // Executes created at operation.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
