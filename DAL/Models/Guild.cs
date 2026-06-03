using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Guild
    {
        public int GuildId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
        public string? IconUrl { get; set; }

        public int LeaderId { get; set; }
        public PlayerProfile? Leader { get; set; }

        public int MaxMembers { get; set; } = 50;
        public int Level { get; set; } = 1;
        public int Experience { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<GuildMember> Members { get; set; } = new List<GuildMember>();
        public ICollection<GuildInvitation> Invitations { get; set; } = new List<GuildInvitation>();
    }
}
