using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // Initializes a new default instance of the GuildResponseDto class.
    public class GuildResponseDto
    {
        // Executes guild id operation.
        public int GuildId { get; set; }
        // Executes name operation.
        public string Name { get; set; } = string.Empty;
        // Executes description operation.
        public string? Description { get; set; }
        // Executes notice operation.
        public string Notice { get; set; } = string.Empty;
        // Executes icon id operation.
        public int IconId { get; set; }
        // Executes banner id operation.
        public int BannerId { get; set; }
        // Executes leader id operation.
        public int LeaderId { get; set; }
        // Executes leader name operation.
        public string? LeaderName { get; set; }
        // Executes leader avatar url operation.
        public string? LeaderAvatarUrl { get; set; }
        // Executes level operation.
        public int Level { get; set; }
        // Executes guild exp operation.
        public int GuildExp { get; set; }
        // Executes exp to next level operation.
        public int ExpToNextLevel { get; set; }
        // Executes medals to next level operation.
        public int MedalsToNextLevel { get; set; }
        // Executes member count operation.
        public int MemberCount { get; set; }
        // Executes max members operation.
        public int MaxMembers { get; set; }
        // Executes required level operation.
        public int RequiredLevel { get; set; }
        // Executes join policy operation.
        public int JoinPolicy { get; set; }
        // Executes total medals operation.
        public int TotalMedals { get; set; }
        // Executes is active operation.
        public bool IsActive { get; set; }
        // Executes is invited operation.
        public bool IsInvited { get; set; }
        // Executes created at operation.
        public DateTime CreatedAt { get; set; }
    }

    // Initializes a new default instance of the GuildResponseDto class.
    public class GuildDetailResponseDto : GuildResponseDto
    {
        // Executes members operation.
        public List<GuildMemberResponseDto> Members { get; set; } = new();
    }

    // Executes create guild request dto operation.
    public class CreateGuildRequestDto
    {
        // Executes name operation.
        [Required(ErrorMessage = "Guild name is required.")]
        [StringLength(15, ErrorMessage = "Guild name must be between 3 and 15 characters.", MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        // Executes notice operation.
        [StringLength(200)]
        public string Notice { get; set; } = string.Empty;

        // Executes required level operation.
        public int RequiredLevel { get; set; } = 1;

        // Executes join policy operation.
        public int? JoinPolicy { get; set; }

        // Executes icon id operation.
        public int IconId { get; set; } = 0;
        // Executes banner id operation.
        public int BannerId { get; set; } = 0;
    }

    // Executes update guild request dto operation.
    public class UpdateGuildRequestDto
    {
        // Executes name operation.
        [StringLength(100)]
        public string? Name { get; set; }

        // Executes notice operation.
        [StringLength(200)]
        public string? Notice { get; set; }

        // Executes required level operation.
        public int? RequiredLevel { get; set; }
        // Executes join policy operation.
        public int? JoinPolicy { get; set; }
    }

    // Executes change notice request operation.
    public class ChangeNoticeRequest
    {
        // Executes notice operation.
        [Required, StringLength(200)]
        public string Notice { get; set; } = string.Empty;
    }

    // Executes change icon request operation.
    public class ChangeIconRequest
    {
        // Executes icon id operation.
        public int IconId { get; set; } = 0;
        // Executes banner id operation.
        public int? BannerId { get; set; }
    }

    // Executes guild member response dto operation.
    public class GuildMemberResponseDto
    {
        // Executes guild member id operation.
        public int GuildMemberId { get; set; }
        // Executes guild id operation.
        public int GuildId { get; set; }
        // Executes guild name operation.
        public string? GuildName { get; set; }
        // Executes player profile id operation.
        public int PlayerProfileId { get; set; }
        // Executes player display name operation.
        public string PlayerDisplayName { get; set; } = string.Empty;
        // Executes player avatar url operation.
        public string? PlayerAvatarUrl { get; set; }
        // Executes player level operation.
        public int PlayerLevel { get; set; }
        // Supported guild roles: Member, Officer, or Leader; the role determines guild-management permissions.
        public string Role { get; set; } = "Member";
        // Executes medals operation.
        public int Medals { get; set; }
        // Executes feats operation.
        public int Feats { get; set; }
        // Executes daily contribution operation.
        public int DailyContribution { get; set; }
        // Executes weekly contribution operation.
        public int WeeklyContribution { get; set; }
        // Executes total contribution operation.
        public int TotalContribution { get; set; }
        // Executes is online operation.
        public bool IsOnline { get; set; }
        // Executes joined at operation.
        public DateTime JoinedAt { get; set; }
        // Executes left at operation.
        public DateTime? LeftAt { get; set; }
        // Executes last donate at operation.
        public DateTime? LastDonateAt { get; set; }
    }

    // Executes promote member request operation.
    public class PromoteMemberRequest
    {
        // Executes target player profile id operation.
        [Required]
        public int TargetPlayerProfileId { get; set; }
    }

    // Executes transfer leader request operation.
    public class TransferLeaderRequest
    {
        // Executes new leader profile id operation.
        [Required]
        public int NewLeaderProfileId { get; set; }
    }

    // Executes guild invitation response dto operation.
    public class GuildInvitationResponseDto
    {
        // Executes guild invitation id operation.
        public int GuildInvitationId { get; set; }
        // Executes guild id operation.
        public int GuildId { get; set; }
        // Executes guild name operation.
        public string? GuildName { get; set; }
        // Executes icon id operation.
        public int IconId { get; set; }
        // Executes inviter id operation.
        public int InviterId { get; set; }
        // Executes inviter name operation.
        public string? InviterName { get; set; }
        // Executes invitee id operation.
        public int InviteeId { get; set; }
        // Executes invitee name operation.
        public string? InviteeName { get; set; }
        // Supported guild request states: Pending, Accepted, Declined, or Expired; only Pending requests can transition to a final state.
        public string Status { get; set; } = "Pending";
        // Executes created at operation.
        public DateTime CreatedAt { get; set; }
        // Executes expires at operation.
        public DateTime ExpiresAt { get; set; }
    }

    // Executes invite player request operation.
    public class InvitePlayerRequest
    {
        // Executes invitee profile id operation.
        [Required]
        public int InviteeProfileId { get; set; }
    }

    // Executes guild join result dto operation.
    public class GuildJoinResultDto
    {
        // Executes success operation.
        public bool Success { get; set; }
        // Executes can join operation.
        public bool CanJoin { get; set; } = true;
        // Executes cooldown remaining seconds operation.
        public int CooldownRemainingSeconds { get; set; } = 0;
        // Executes message operation.
        public string Message { get; set; } = string.Empty;
    }

    // Executes guild application dto operation.
    public class GuildApplicationDTO
    {
        // Executes guild application id operation.
        public int GuildApplicationId { get; set; }
        // Executes player profile id operation.
        public int PlayerProfileId { get; set; }
        // Executes player name operation.
        public string PlayerName { get; set; } = string.Empty;
        // Executes player avatar url operation.
        public string? PlayerAvatarUrl { get; set; }
        // Executes player level operation.
        public int PlayerLevel { get; set; }
        // Executes medals operation.
        public int Medals { get; set; }
        // Executes feats operation.
        public int Feats { get; set; }
        // Supported guild request states: Pending, Accepted, Declined, or Expired; only Pending requests can transition to a final state.
        public string Status { get; set; } = string.Empty;
        // Executes created at operation.
        public DateTime CreatedAt { get; set; }
    }

    // Executes donate request operation.
    public class DonateRequest
    {
        [Required]
        // Supported currencies: Gold or Gems; the selected currency determines which player balance is charged or credited.
        public string CurrencyType { get; set; } = "Gold";

        // Executes amount operation.
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public int Amount { get; set; } = 1;
    }

    // Executes guild donate result dto operation.
    public class GuildDonateResultDto
    {
        // Executes gold spent operation.
        public int GoldSpent { get; set; }
        // Executes gem spent operation.
        public int GemSpent { get; set; }
        // Executes guild exp gained operation.
        public int GuildExpGained { get; set; }
        // Executes guild medals gained operation.
        public int GuildMedalsGained { get; set; }
        // Executes player medals gained operation.
        public int PlayerMedalsGained { get; set; }
        // Executes player feats gained operation.
        public int PlayerFeatsGained { get; set; }
        // Executes guild leveled up operation.
        public bool GuildLeveledUp { get; set; }
        // Executes new guild level operation.
        public int NewGuildLevel { get; set; }
        // Executes new guild exp operation.
        public int NewGuildExp { get; set; }
        // Executes exp to next level operation.
        public int ExpToNextLevel { get; set; }
        // Executes total medals operation.
        public int TotalMedals { get; set; }
        // Executes medals to next level operation.
        public int MedalsToNextLevel { get; set; }
    }

    // Executes guild message dto operation.
    public class GuildMessageDTO
    {
        // Executes message id operation.
        public int MessageId { get; set; }
        // Executes sender id operation.
        public int SenderId { get; set; }
        // Executes sender name operation.
        public string SenderName { get; set; } = string.Empty;
        // Executes content operation.
        public string Content { get; set; } = string.Empty;
        // Executes message type operation.
        public int MessageType { get; set; }
        // Executes sender role operation.
        public int SenderRole { get; set; }
        // Executes sent at operation.
        public DateTime SentAt { get; set; }
    }

    // Executes send guild message request operation.
    public class SendGuildMessageRequest
    {
        // Executes content operation.
        [Required]
        [StringLength(500)]
        public string Content { get; set; } = string.Empty;
    }

    // Executes guild log dto operation.
    public class GuildLogDto
    {
        // Executes guild log id operation.
        public int GuildLogId { get; set; }
        // Executes action operation.
        public string Action { get; set; } = string.Empty;
        // Executes actor name operation.
        public string? ActorName { get; set; }
        // Executes target name operation.
        public string? TargetName { get; set; }
        // Executes detail operation.
        public string? Detail { get; set; }
        // Executes created at operation.
        public DateTime CreatedAt { get; set; }
    }

}
