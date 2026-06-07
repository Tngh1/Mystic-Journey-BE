using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // ============ Guild ============
    public class GuildResponseDto
    {
        public int GuildId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? IconUrl { get; set; }
        public int LeaderId { get; set; }
        public string? LeaderName { get; set; }
        public int MaxMembers { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int MemberCount { get; set; }
    }

    public class GuildDetailResponseDto : GuildResponseDto
    {
        public List<GuildMemberResponseDto> Members { get; set; } = new();
    }

    public class CreateGuildRequestDto
    {
        [Required(ErrorMessage = "Guild name is required.")]
        [StringLength(100, ErrorMessage = "Guild name must not exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description must not exceed 500 characters.")]
        public string? Description { get; set; }

        public string? IconUrl { get; set; }
        public int MaxMembers { get; set; } = 50;
    }

    public class UpdateGuildRequestDto
    {
        [Required(ErrorMessage = "Guild name is required.")]
        [StringLength(100, ErrorMessage = "Guild name must not exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }
        public string? IconUrl { get; set; }
        public int MaxMembers { get; set; } = 50;
        public bool? IsActive { get; set; }
    }

    // ============ GuildMember ============
    public class GuildMemberResponseDto
    {
        public int GuildMemberId { get; set; }
        public int GuildId { get; set; }
        public string? GuildName { get; set; }
        public int PlayerProfileId { get; set; }
        public string? PlayerDisplayName { get; set; }
        public string? PlayerAvatarUrl { get; set; }
        public int PlayerLevel { get; set; }
        public string Role { get; set; } = "Member";
        public int Contribution { get; set; }
        public bool IsActive { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime? LeftAt { get; set; }
    }

    public class UpdateGuildMemberRequestDto
    {
        [Required]
        public string Role { get; set; } = "Member";
    }

    // ============ GuildInvitation ============
    public class GuildInvitationResponseDto
    {
        public int GuildInvitationId { get; set; }
        public int GuildId { get; set; }
        public string? GuildName { get; set; }
        public string? GuildIconUrl { get; set; }
        public int InviterId { get; set; }
        public string? InviterName { get; set; }
        public int InviteeId { get; set; }
        public string? InviteeName { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; }
        public DateTime? RespondedAt { get; set; }
    }

    public class CreateGuildInvitationRequestDto
    {
        [Required]
        public int GuildId { get; set; }

        [Required]
        public int InviteeId { get; set; }
    }

    public class RespondGuildInvitationRequestDto
    {
        [Required]
        public string Status { get; set; } = "Pending";
    }

    // ============ Guild - Player Views ============
    public class GuildListResponseDto
    {
        public int GuildId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? IconUrl { get; set; }
        public int Level { get; set; }
        public int MemberCount { get; set; }
        public int MaxMembers { get; set; }
        public string? LeaderName { get; set; }
        public bool IsActive { get; set; }
    }

    public class GuildApplicationRequestDto
    {
        [Required]
        public int GuildId { get; set; }

        public string? Message { get; set; }
    }
}
