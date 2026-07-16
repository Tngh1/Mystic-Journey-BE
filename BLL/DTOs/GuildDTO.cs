using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // ============ Guild ============
    public class GuildResponseDto
    {
        public int GuildId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Notice { get; set; } = string.Empty;
        public int IconId { get; set; }
        public int BannerId { get; set; }
        public int LeaderId { get; set; }
        public string? LeaderName { get; set; }
        public int Level { get; set; }
        public int GuildExp { get; set; }
        public int ExpToNextLevel { get; set; }
        public int MedalsToNextLevel { get; set; }
        public int MemberCount { get; set; }
        public int MaxMembers { get; set; }
        public int RequiredLevel { get; set; }
        /// <summary>0=Open, 1=Approval, 2=InviteOnly</summary>
        public int JoinPolicy { get; set; }
        public int TotalMedals { get; set; }
        public bool IsActive { get; set; }
        public bool IsInvited { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class GuildDetailResponseDto : GuildResponseDto
    {
        public List<GuildMemberResponseDto> Members { get; set; } = new();
    }

    public class CreateGuildRequestDto
    {
        [Required(ErrorMessage = "Guild name is required.")]
        [StringLength(15, ErrorMessage = "Guild name must be between 3 and 15 characters.", MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        public string Notice { get; set; } = string.Empty;

        public int RequiredLevel { get; set; } = 1;

        /// <summary>0=Open, 1=Approval, 2=InviteOnly</summary>
        public int? JoinPolicy { get; set; }

        public int IconId { get; set; } = 0;
        public int BannerId { get; set; } = 0;
    }

    public class UpdateGuildRequestDto
    {
        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(200)]
        public string? Notice { get; set; }

        public int? RequiredLevel { get; set; }
        public int? JoinPolicy { get; set; }
    }

    public class ChangeNoticeRequest
    {
        [Required, StringLength(200)]
        public string Notice { get; set; } = string.Empty;
    }

    public class ChangeIconRequest
    {
        public int IconId { get; set; } = 0;
        public int? BannerId { get; set; }
    }

    // ============ GuildMember ============
    public class GuildMemberResponseDto
    {
        public int GuildMemberId { get; set; }
        public int GuildId { get; set; }
        public string? GuildName { get; set; }
        public int PlayerProfileId { get; set; }
        public string PlayerDisplayName { get; set; } = string.Empty;
        public string? PlayerAvatarUrl { get; set; }
        public int PlayerLevel { get; set; }
        public string Role { get; set; } = "Member";
        public int Medals { get; set; }
        public int Feats { get; set; }
        public int DailyContribution { get; set; }
        public int WeeklyContribution { get; set; }
        public int TotalContribution { get; set; }
        public bool IsOnline { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime? LeftAt { get; set; }
    }

    public class PromoteMemberRequest
    {
        [Required]
        public int TargetPlayerProfileId { get; set; }
    }

    public class TransferLeaderRequest
    {
        [Required]
        public int NewLeaderProfileId { get; set; }
    }

    // ============ GuildInvitation ============
    public class GuildInvitationResponseDto
    {
        public int GuildInvitationId { get; set; }
        public int GuildId { get; set; }
        public string? GuildName { get; set; }
        public int IconId { get; set; }
        public int InviterId { get; set; }
        public string? InviterName { get; set; }
        public int InviteeId { get; set; }
        public string? InviteeName { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    public class InvitePlayerRequest
    {
        [Required]
        public int InviteeProfileId { get; set; }
    }

    // ============ Guild Join / Leave ============
    public class GuildJoinResultDto
    {
        public bool Success { get; set; }
        public bool CanJoin { get; set; } = true;
        /// <summary>Seconds remaining before player can join a new guild (leave cooldown)</summary>
        public int CooldownRemainingSeconds { get; set; } = 0;
        public string Message { get; set; } = string.Empty;
    }

    // ============ GuildApplication ============
    public class GuildApplicationDTO
    {
        public int GuildApplicationId { get; set; }
        public int PlayerProfileId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public string? PlayerAvatarUrl { get; set; }
        public int PlayerLevel { get; set; }
        public int Medals { get; set; }
        public int Feats { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class GuildApplicationRequestDto
    {
        public int GuildId { get; set; }
        public string? Message { get; set; }
    }

    // ============ Donate ============
    public class DonateRequest
    {
        [Required]
        [Range(1, 100, ErrorMessage = "Amount must be between 1 and 100")]
        public int Amount { get; set; } = 1;
    }

    public class GuildDonateResultDto
    {
        public int GoldSpent { get; set; }
        public int GuildExpGained { get; set; }
        public int GuildMedalsGained { get; set; }
        public int PlayerMedalsGained { get; set; }
        public int PlayerFeatsGained { get; set; }
        public bool GuildLeveledUp { get; set; }
        public int NewGuildLevel { get; set; }
        public int NewGuildExp { get; set; }
        public int ExpToNextLevel { get; set; }
        public int TotalMedals { get; set; }
        public int MedalsToNextLevel { get; set; }
    }

    // ============ Guild Chat ============
    public class GuildMessageDTO
    {
        public int MessageId { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        /// <summary>0=Text, 1=System, 2=Join, 3=Leave, 4=Promotion</summary>
        public int MessageType { get; set; }
        /// <summary>0=Member, 1=Officer, 2=Leader</summary>
        public int SenderRole { get; set; }
        public DateTime SentAt { get; set; }
    }

    public class SendGuildMessageRequest
    {
        [Required]
        [StringLength(500)]
        public string Content { get; set; } = string.Empty;
    }

    // ============ Guild Log ============
    public class GuildLogDto
    {
        public int GuildLogId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? ActorName { get; set; }
        public string? TargetName { get; set; }
        public string? Detail { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // ============ Legacy / List view ============
    public class GuildListResponseDto
    {
        public int GuildId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int IconId { get; set; }
        public int Level { get; set; }
        public int MemberCount { get; set; }
        public int MaxMembers { get; set; }
        public string? LeaderName { get; set; }
        public bool IsActive { get; set; }
        public int RequiredLevel { get; set; }
        public int JoinPolicy { get; set; }
    }
}
