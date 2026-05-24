namespace DAL.Models
{
    public class PartyMember
    {
        public int Id { get; set; }

        public int PartyId { get; set; }
        public Party? Party { get; set; }

        public int PlayerProfileId { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }

        public bool IsLeader { get; set; } = false;
        public bool IsReady { get; set; } = false;

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LeftAt { get; set; }
    }
}
