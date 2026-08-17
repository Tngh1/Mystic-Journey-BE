using System;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    // Initializes a new default instance of the GuildApplication class.
    public class GuildApplication
    {
        // Executes guild application id operation.
        public int GuildApplicationId { get; set; }

        // Executes guild id operation.
        public int GuildId { get; set; }
        // Executes guild operation.
        public Guild? Guild { get; set; }

        // Executes player profile id operation.
        public int PlayerProfileId { get; set; }
        // Executes player profile operation.
        public PlayerProfile? PlayerProfile { get; set; }

        // Supported guild request states: Pending, Accepted, Declined, or Expired; only Pending requests can transition to a final state.
        public string Status { get; set; } = "Pending";

        // Executes created at operation.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
