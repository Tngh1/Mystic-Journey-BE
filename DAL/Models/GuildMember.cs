namespace DAL.Models
{
    public class GuildMember
    {
        public Guid Id { get; set; }

        public Guid GuildId { get; set; }
        public Guild? Guild { get; set; }

        public Guid PlayerProfileId { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }

        // Roles: Leader, Officer, Member
        public string Role { get; set; } = "Member";

        public int Contribution { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LeftAt { get; set; }
    }
}
