using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    // Initializes a new default instance of the Guild class.
    public class Guild
    {
        // Executes guild id operation.
        public int GuildId { get; set; }

        // Executes name operation.
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        // Executes description operation.
        public string? Description { get; set; }

        // Executes notice operation.
        [MaxLength(200)]
        public string Notice { get; set; } = string.Empty;

        // Executes icon id operation.
        public int IconId { get; set; } = 0;
        // Executes banner id operation.
        public int BannerId { get; set; } = 0;

        // Executes leader id operation.
        public int LeaderId { get; set; }
        // Executes leader operation.
        public PlayerProfile? Leader { get; set; }

        // Executes created by profile id operation.
        public int CreatedByProfileId { get; set; }
        // Executes created by operation.
        public PlayerProfile? CreatedBy { get; set; }

        // Executes required level operation.
        public int RequiredLevel { get; set; } = 1;
        // Executes level operation.
        public int Level { get; set; } = 1;
        // Executes guild exp operation.
        public int GuildExp { get; set; } = 0;
        // Executes total medals operation.
        public int TotalMedals { get; set; } = 0;
        // Executes total feats operation.
        public int TotalFeats { get; set; } = 0;

        // Executes join policy operation.
        public GuildJoinPolicy JoinPolicy { get; set; } = GuildJoinPolicy.Approval;
        // Executes is active operation.
        public bool IsActive { get; set; } = true;

        // Executes created at operation.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Executes max members operation.
        public int MaxMembers => 100 + (Level - 1) * 10;

        public static readonly int[] ExpTable = { 2000, 5000, 10000, 18000, 30000, 50000 };
        public static readonly int[] MedalTable = { 500, 1000, 1500, 2500, 4000, 6000 };

        // Executes exp to next level operation.
        public int ExpToNextLevel => Level <= ExpTable.Length
            ? ExpTable[Level - 1]
            : ExpTable[^1] + (Level - ExpTable.Length) * 20000;

        // Executes medals to next level operation.
        public int MedalsToNextLevel => Level <= MedalTable.Length
            ? MedalTable[Level - 1]
            : MedalTable[^1] + (Level - MedalTable.Length) * 2000;

        // Executes members operation.
        public ICollection<GuildMember> Members { get; set; } = new List<GuildMember>();
        // Executes invitations operation.
        public ICollection<GuildInvitation> Invitations { get; set; } = new List<GuildInvitation>();
        // Executes applications operation.
        public ICollection<GuildApplication> Applications { get; set; } = new List<GuildApplication>();
        // Executes logs operation.
        public ICollection<GuildLog> Logs { get; set; } = new List<GuildLog>();
        // Executes chat messages operation.
        public ICollection<GuildChatMessage> ChatMessages { get; set; } = new List<GuildChatMessage>();
    }
}
