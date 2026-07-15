using System;

namespace BLL.DTOs
{
    public class GuildRankResponseDto
    {
        public int Rank { get; set; }
        public int GuildId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int IconId { get; set; }
        public int Level { get; set; }
        public int TotalMedals { get; set; }
        public int TotalFeats { get; set; }
        public int MemberCount { get; set; }
        public int MaxMembers { get; set; }
    }
}
