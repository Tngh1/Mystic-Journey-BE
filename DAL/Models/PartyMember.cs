namespace DAL.Models
{
    public class PartyMember
    {
        public Guid Id { get; set; }

        public Guid PartyId { get; set; }
        public Party? Party { get; set; }

        public Guid PlayerProfileId { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }

        public bool IsLeader { get; set; } = false;
        public bool IsReady { get; set; } = false;

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LeftAt { get; set; }
    }
}
