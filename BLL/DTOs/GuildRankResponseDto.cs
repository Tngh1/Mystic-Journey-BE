using System;

namespace BLL.DTOs
{
    // Initializes a new default instance of the GuildRankResponseDto class.
    public class GuildRankResponseDto
    {
        // Executes rank operation.
        public int Rank { get; set; }
        // Executes guild id operation.
        public int GuildId { get; set; }
        // Executes name operation.
        public string Name { get; set; } = string.Empty;
        // Executes icon id operation.
        public int IconId { get; set; }
        // Executes level operation.
        public int Level { get; set; }
        // Executes total medals operation.
        public int TotalMedals { get; set; }
        // Executes total feats operation.
        public int TotalFeats { get; set; }
        // Executes member count operation.
        public int MemberCount { get; set; }
        // Executes max members operation.
        public int MaxMembers { get; set; }
    }
}
