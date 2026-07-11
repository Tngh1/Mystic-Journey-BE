using System;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class GuildApplication
    {
        public int GuildApplicationId { get; set; }

        public int GuildId { get; set; }
        public Guild? Guild { get; set; }

        public int PlayerProfileId { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }

        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
