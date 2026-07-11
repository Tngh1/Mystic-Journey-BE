namespace DAL.Models
{
    public class GuildMember
    {
        public int GuildMemberId { get; set; }

        public int GuildId { get; set; }
        public Guild? Guild { get; set; }

        public int PlayerProfileId { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }

        // Stored as int to match GuildRole enum
        public GuildRole Role { get; set; } = GuildRole.Member;

        // Contribution tracking
        public int DailyContribution { get; set; } = 0;
        public int WeeklyContribution { get; set; } = 0;
        public int TotalContribution { get; set; } = 0;

        // Kept for backward compat, acts as lifetime contribution alias
        public int Contribution { get; set; } = 0;

        // Guild currency earned through contribution
        public int Medals { get; set; } = 0;
        public int Feats { get; set; } = 0;

        public DateTime? LastDonateAt { get; set; }
        public DateTime? LastChatAt { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LeftAt { get; set; }
    }
}
