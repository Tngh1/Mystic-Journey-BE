using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Guild
    {
        public int GuildId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [MaxLength(200)]
        public string Notice { get; set; } = string.Empty;

        // Icon and Banner use preset IDs mapped to Sprites in Unity (0 = default)
        public int IconId { get; set; } = 0;
        public int BannerId { get; set; } = 0;

        public int LeaderId { get; set; }
        public PlayerProfile? Leader { get; set; }

        public int CreatedByProfileId { get; set; }
        public PlayerProfile? CreatedBy { get; set; }

        // MaxMembers is computed dynamically: 100 + (Level - 1) * 10
        public int RequiredLevel { get; set; } = 1;
        public int Level { get; set; } = 1;
        public int GuildExp { get; set; } = 0;
        public int TotalMedals { get; set; } = 0;

        public GuildJoinPolicy JoinPolicy { get; set; } = GuildJoinPolicy.Approval;
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Computed (not stored in DB)
        public int MaxMembers => 100 + (Level - 1) * 10;

        // EXP required to reach next level: index 0 = Lv1->2, index 1 = Lv2->3, etc.
        // After index 5 (Lv7+), uses 50000 * (level - 6) + 50000 to continue scaling
        public static readonly int[] ExpTable = { 2000, 5000, 10000, 18000, 30000, 50000 };
        public static readonly int[] MedalTable = { 500, 1000, 1500, 2500, 4000, 6000 };

        public int ExpToNextLevel => Level <= ExpTable.Length
            ? ExpTable[Level - 1]
            : ExpTable[^1] + (Level - ExpTable.Length) * 20000;

        public int MedalsToNextLevel => Level <= MedalTable.Length
            ? MedalTable[Level - 1]
            : MedalTable[^1] + (Level - MedalTable.Length) * 2000;

        public ICollection<GuildMember> Members { get; set; } = new List<GuildMember>();
        public ICollection<GuildInvitation> Invitations { get; set; } = new List<GuildInvitation>();
        public ICollection<GuildApplication> Applications { get; set; } = new List<GuildApplication>();
        public ICollection<GuildLog> Logs { get; set; } = new List<GuildLog>();
        public ICollection<GuildChatMessage> ChatMessages { get; set; } = new List<GuildChatMessage>();
    }
}
