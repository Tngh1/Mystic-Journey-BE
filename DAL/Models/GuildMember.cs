namespace DAL.Models
{
    // Initializes a new default instance of the GuildMember class.
    public class GuildMember
    {
        // Executes guild member id operation.
        public int GuildMemberId { get; set; }

        // Executes guild id operation.
        public int GuildId { get; set; }
        // Executes guild operation.
        public Guild? Guild { get; set; }

        // Executes player profile id operation.
        public int PlayerProfileId { get; set; }
        // Executes player profile operation.
        public PlayerProfile? PlayerProfile { get; set; }

        // Supported guild roles: Member, Officer, or Leader; the role determines guild-management permissions.
        public GuildRole Role { get; set; } = GuildRole.Member;

        // Executes daily contribution operation.
        public int DailyContribution { get; set; } = 0;
        // Executes weekly contribution operation.
        public int WeeklyContribution { get; set; } = 0;
        // Executes total contribution operation.
        public int TotalContribution { get; set; } = 0;

        // Executes contribution operation.
        public int Contribution { get; set; } = 0;

        // Executes medals operation.
        public int Medals { get; set; } = 0;
        // Executes feats operation.
        public int Feats { get; set; } = 0;

        // Executes last donate at operation.
        public DateTime? LastDonateAt { get; set; }
        // Executes last chat at operation.
        public DateTime? LastChatAt { get; set; }
        // Executes joined at operation.
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        // Executes left at operation.
        public DateTime? LeftAt { get; set; }
    }
}
