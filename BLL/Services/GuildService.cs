using BLL.DTOs;
using BLL.Services.Interfaces;
using BLL.Utils;
using DAL.Models;
using DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services;

public class GuildService : IGuildService
{
    private readonly IGuildRepository _guildRepo;
    private const int LeaveCooldownHours = 24;
    private const int DonateGoldCostPerUnit = 100;
    private const int DonateExpGainPerUnit = 50;
    private const int DonateMedalsGainPerUnit = 10;
    private const int DonatePlayerMedalsPerUnit = 10;
    private const int DonatePlayerFeatsPerUnit = 5;
    private const int ChatSpamCooldownMs = 1000;

    public GuildService(IGuildRepository guildRepo) => _guildRepo = guildRepo;

    // ─── Permission Helper ────────────────────────────────────────────

    private static bool IsLeaderOrOfficer(GuildMember m)
        => m.Role == GuildRole.Leader || m.Role == GuildRole.Officer;

    private static bool IsLeader(GuildMember m) => m.Role == GuildRole.Leader;

    // ─── Map Helpers ──────────────────────────────────────────────────

    private static GuildResponseDto MapGuildDto(Guild g, int memberCount)
    {
        return new GuildResponseDto
        {
            GuildId = g.GuildId,
            Name = g.Name,
            Description = g.Description,
            Notice = g.Notice,
            IconId = g.IconId,
            BannerId = g.BannerId,
            LeaderId = g.LeaderId,
            LeaderName = g.Leader?.DisplayName ?? "Unknown",
            LeaderAvatarUrl = g.Leader?.AvatarUrl ?? "",
            Level = g.Level,
            GuildExp = g.GuildExp,
            ExpToNextLevel = g.ExpToNextLevel,
            MedalsToNextLevel = g.MedalsToNextLevel,
            MemberCount = memberCount,
            MaxMembers = g.MaxMembers,
            RequiredLevel = g.RequiredLevel,
            JoinPolicy = (int)g.JoinPolicy,
            TotalMedals = g.TotalMedals,
            IsActive = g.IsActive,
            CreatedAt = g.CreatedAt
        };
    }

    private static GuildMemberResponseDto MapMemberDto(GuildMember m)
    {
        return new GuildMemberResponseDto
        {
            GuildMemberId = m.GuildMemberId,
            GuildId = m.GuildId,
            PlayerProfileId = m.PlayerProfileId,
            PlayerDisplayName = m.PlayerProfile?.DisplayName ?? "Unknown",
            PlayerAvatarUrl = m.PlayerProfile?.AvatarUrl ?? "",
            PlayerLevel = m.PlayerProfile?.Level ?? 1,
            Role = m.Role.ToString(),
            Medals = m.Medals,
            Feats = m.Feats,
            DailyContribution = m.DailyContribution,
            WeeklyContribution = m.WeeklyContribution,
            TotalContribution = m.TotalContribution,
            JoinedAt = m.JoinedAt,
            LastDonateAt = m.LastDonateAt,
            IsOnline = OnlineTimeout.IsWithin(m.PlayerProfile?.LastSeen, OnlineTimeout.Presence)
        };
    }

    // ─── View ─────────────────────────────────────────────────────────

    public async Task<GuildDetailResponseDto?> GetMyGuildAsync(int playerProfileId)
    {
        var player = await _guildRepo.GetPlayerProfileAsync(playerProfileId, includeGuildMember: true);

        if (player?.GuildMember != null && player.GuildMember.LeftAt == null)
            return await GetGuildDetailAsync(player.GuildMember.GuildId);

        // Fallback: check if this player is a leader of an active guild
        var leadedGuild = await _guildRepo.GetActiveGuildByLeaderAsync(playerProfileId);

        if (leadedGuild != null)
        {
            var recoveredMember = new GuildMember
            {
                GuildId = leadedGuild.GuildId,
                PlayerProfileId = playerProfileId,
                Role = GuildRole.Leader,
                JoinedAt = leadedGuild.CreatedAt
            };
            await _guildRepo.AddMemberAsync(recoveredMember);
            return await GetGuildDetailAsync(leadedGuild.GuildId);
        }

        return null;
    }

    public async Task<List<GuildResponseDto>> GetGuildListAsync(
        int playerProfileId, string searchTerm = "", int? joinPolicy = null, int? minLevel = null)
    {
        var now = DateTime.UtcNow;

        var invitedGuildIds = (await _guildRepo.GetActiveInvitationsForPlayerAsync(playerProfileId, now))
            .Select(i => i.GuildId)
            .ToList();

        var invitedGuilds = await _guildRepo.GetGuildsByIdsAsync(invitedGuildIds);

        var resultDtos = new List<GuildResponseDto>();
        foreach (var ig in invitedGuilds)
        {
            var dto = MapGuildDto(ig, ig.Members.Count);
            dto.IsInvited = true;
            resultDtos.Add(dto);
        }

        var normalGuilds = await _guildRepo.GetActiveGuildsAsync(
            excludeIds: invitedGuildIds,
            searchTerm: searchTerm,
            joinPolicy: joinPolicy,
            minLevel: minLevel,
            take: 20);

        foreach (var ng in normalGuilds)
        {
            var dto = MapGuildDto(ng, ng.Members.Count);
            dto.IsInvited = false;
            resultDtos.Add(dto);
        }

        return resultDtos;
    }

    public async Task<List<GuildRankResponseDto>> GetGuildRankingsAsync(int top = 100)
    {
        var guilds = await _guildRepo.GetTopGuildsAsync(top);

        var result = new List<GuildRankResponseDto>();
        for (int i = 0; i < guilds.Count; i++)
        {
            var g = guilds[i];
            result.Add(new GuildRankResponseDto
            {
                Rank = i + 1,
                GuildId = g.GuildId,
                Name = g.Name,
                IconId = g.IconId,
                Level = g.Level,
                TotalMedals = g.TotalMedals,
                TotalFeats = g.TotalFeats,
                MemberCount = g.Members.Count,
                MaxMembers = g.MaxMembers
            });
        }
        return result;
    }

    public async Task<GuildDetailResponseDto?> GetGuildDetailAsync(int guildId)
    {
        var guild = await _guildRepo.GetGuildByIdAsync(guildId,
            includeMembers: true,
            includeLeader: true,
            includeMemberProfiles: true);

        if (guild == null) return null;

        var dto = new GuildDetailResponseDto();
        var baseDto = MapGuildDto(guild, guild.Members.Count);
        dto.GuildId = baseDto.GuildId;
        dto.Name = baseDto.Name;
        dto.Notice = baseDto.Notice;
        dto.IconId = baseDto.IconId;
        dto.BannerId = baseDto.BannerId;
        dto.LeaderId = baseDto.LeaderId;
        dto.Level = baseDto.Level;
        dto.GuildExp = baseDto.GuildExp;
        dto.ExpToNextLevel = baseDto.ExpToNextLevel;
        dto.MedalsToNextLevel = baseDto.MedalsToNextLevel;
        dto.MemberCount = baseDto.MemberCount;
        dto.MaxMembers = baseDto.MaxMembers;
        dto.RequiredLevel = baseDto.RequiredLevel;
        dto.JoinPolicy = baseDto.JoinPolicy;
        dto.TotalMedals = baseDto.TotalMedals;
        dto.IsActive = baseDto.IsActive;
        dto.CreatedAt = baseDto.CreatedAt;
        dto.Description = baseDto.Description;
        dto.LeaderName = baseDto.LeaderName;
        dto.LeaderAvatarUrl = baseDto.LeaderAvatarUrl;
        dto.Members = guild.Members.Select(MapMemberDto).ToList();
        return dto;
    }

    public async Task<List<GuildMemberResponseDto>> GetMembersAsync(int playerProfileId, int guildId)
    {
        if (!await _guildRepo.IsGuildMemberAsync(guildId, playerProfileId))
            throw new UnauthorizedAccessException("Not a member");

        var members = await _guildRepo.GetActiveMembersAsync(guildId, includeProfile: true);
        return members.Select(MapMemberDto).ToList();
    }

    // ─── Create / Dissolve ────────────────────────────────────────────

    public async Task<GuildResponseDto> CreateGuildAsync(int playerProfileId, CreateGuildRequestDto request)
    {
        var player = await _guildRepo.GetPlayerProfileAsync(playerProfileId, includeGuildMember: true);

        if (player == null) throw new Exception("Player not found");
        if (player.GuildMember != null) throw new Exception("Player is already in a guild");

        var guild = new Guild
        {
            Name = request.Name,
            Notice = request.Notice ?? "",
            RequiredLevel = request.RequiredLevel,
            JoinPolicy = (GuildJoinPolicy)(request.JoinPolicy ?? (int)GuildJoinPolicy.Open),
            IconId = request.IconId,
            BannerId = request.BannerId,
            LeaderId = playerProfileId,
            CreatedByProfileId = playerProfileId,
            CreatedAt = DateTime.UtcNow,
            Members = new List<GuildMember>
            {
                new GuildMember
                {
                    PlayerProfileId = playerProfileId,
                    Role = GuildRole.Leader,
                    JoinedAt = DateTime.UtcNow
                }
            }
        };

        await _guildRepo.AddGuildAsync(guild);

        await _guildRepo.AddLogAsync(new GuildLog
        {
            GuildId = guild.GuildId,
            Action = GuildLogAction.Join,
            ActorProfileId = playerProfileId,
            ActorName = player.DisplayName,
            TargetProfileId = playerProfileId,
            TargetName = player.DisplayName,
            Detail = "Guild founded",
            CreatedAt = DateTime.UtcNow
        });

        return MapGuildDto(guild, 1);
    }

    public async Task<bool> DissolveGuildAsync(int playerProfileId, int guildId)
    {
        var guild = await _guildRepo.GetGuildByIdAsync(guildId, includeMembers: true);

        if (guild == null || guild.LeaderId != playerProfileId) return false;

        var player = await _guildRepo.GetPlayerProfileAsync(playerProfileId);

        guild.IsActive = false;

        var memberIds = guild.Members.Select(m => m.PlayerProfileId).ToList();
        var profiles = await _guildRepo.GetPlayerProfilesByIdsAsync(memberIds);
        foreach (var profile in profiles)
            profile.LastLeaveGuildAt = DateTime.UtcNow;

        await _guildRepo.RemoveMembersAsync(guild.Members);

        var pendingApps = (await _guildRepo.GetPendingApplicationsAsync(guildId))
            .Where(a => a.Status == "Pending")
            .ToList();
        await _guildRepo.RemoveApplicationsAsync(pendingApps);

        await _guildRepo.AddLogAsync(new GuildLog
        {
            GuildId = guildId,
            Action = GuildLogAction.GuildDissolved,
            ActorProfileId = playerProfileId,
            ActorName = player?.DisplayName,
            CreatedAt = DateTime.UtcNow
        });

        await _guildRepo.SaveChangesAsync();
        return true;
    }

    // ─── Join / Leave / Apply ─────────────────────────────────────────

    public async Task<GuildJoinResultDto> ApplyToGuildAsync(int playerProfileId, int guildId)
    {
        var player = await _guildRepo.GetPlayerProfileAsync(playerProfileId, includeGuildMember: true);

        if (player == null) return new GuildJoinResultDto { Success = false, Message = "Player not found" };
        if (player.GuildMember != null)
            return new GuildJoinResultDto { Success = false, Message = "Already in a guild" };

        var now = DateTime.UtcNow;
        var activeInvitation = await _guildRepo.GetActiveInvitationAsync(guildId, playerProfileId, now);

        if (player.LastLeaveGuildAt.HasValue)
        {
            var remaining = player.LastLeaveGuildAt.Value.AddHours(LeaveCooldownHours) - now;
            if (remaining.TotalSeconds > 0)
            {
                return new GuildJoinResultDto
                {
                    Success = false,
                    CanJoin = false,
                    CooldownRemainingSeconds = (int)remaining.TotalSeconds,
                    Message = $"You can join another guild after: {(int)remaining.TotalHours}h {remaining.Minutes}m"
                };
            }
        }

        var guild = await _guildRepo.GetGuildByIdAsync(guildId, includeMembers: true);

        if (guild == null) return new GuildJoinResultDto { Success = false, Message = "Guild not found" };
        if (guild.Members.Count >= guild.MaxMembers)
            return new GuildJoinResultDto { Success = false, Message = "Guild is full" };

        bool bypassPolicy = (activeInvitation != null);

        if (!bypassPolicy)
        {
            if (player.Level < guild.RequiredLevel)
                return new GuildJoinResultDto { Success = false, Message = $"Required level {guild.RequiredLevel}" };
            if (guild.JoinPolicy == GuildJoinPolicy.InviteOnly)
                return new GuildJoinResultDto { Success = false, Message = "This guild is invite only" };
        }

        if (bypassPolicy || guild.JoinPolicy == GuildJoinPolicy.Open)
        {
            await _guildRepo.AddMemberAsync(new GuildMember
            {
                GuildId = guildId,
                PlayerProfileId = playerProfileId,
                Role = GuildRole.Member,
                JoinedAt = DateTime.UtcNow
            });

            await _guildRepo.AddLogAsync(new GuildLog
            {
                GuildId = guildId,
                Action = GuildLogAction.Join,
                ActorProfileId = playerProfileId,
                ActorName = player.DisplayName,
                CreatedAt = DateTime.UtcNow
            });

            if (activeInvitation != null)
            {
                activeInvitation.Status = "Accepted";
                activeInvitation.RespondedAt = DateTime.UtcNow;
                await _guildRepo.UpdateInvitationAsync(activeInvitation);

                var otherInvitations = await _guildRepo.GetOtherPendingInvitationsAsync(
                    playerProfileId, activeInvitation.GuildInvitationId);
                foreach (var otherInv in otherInvitations)
                {
                    otherInv.Status = "Declined";
                    otherInv.RespondedAt = DateTime.UtcNow;
                    await _guildRepo.UpdateInvitationAsync(otherInv);
                }
            }

            await _guildRepo.SaveChangesAsync();
            return new GuildJoinResultDto { Success = true, Message = "Joined guild" };
        }

        var existingApp = await _guildRepo.GetPlayerPendingApplicationAsync(playerProfileId);
        if (existingApp != null)
            return new GuildJoinResultDto { Success = false, Message = "You already have a pending application to another guild" };

        await _guildRepo.AddApplicationAsync(new GuildApplication
        {
            GuildId = guildId,
            PlayerProfileId = playerProfileId,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        });

        await _guildRepo.SaveChangesAsync();
        return new GuildJoinResultDto { Success = true, Message = "Application submitted" };
    }

    public async Task<GuildJoinResultDto> LeaveGuildAsync(int playerProfileId, int guildId)
    {
        var member = await _guildRepo.GetMemberAsync(guildId, playerProfileId);

        if (member == null) return new GuildJoinResultDto { Success = false, Message = "Not a member" };

        if (member.Role == GuildRole.Leader)
            return new GuildJoinResultDto
            {
                Success = false,
                Message = "Leader must transfer leadership or dissolve the guild before leaving"
            };

        var player = await _guildRepo.GetPlayerProfileAsync(playerProfileId);
        if (player != null) player.LastLeaveGuildAt = DateTime.UtcNow;

        await _guildRepo.RemoveMemberAsync(member);

        await _guildRepo.AddLogAsync(new GuildLog
        {
            GuildId = guildId,
            Action = GuildLogAction.Leave,
            ActorProfileId = playerProfileId,
            ActorName = player?.DisplayName,
            CreatedAt = DateTime.UtcNow
        });

        await _guildRepo.SaveChangesAsync();
        return new GuildJoinResultDto { Success = true, Message = "Left guild" };
    }

    // ─── Applications ─────────────────────────────────────────────────

    public async Task<List<GuildApplicationDTO>> GetApplicationsAsync(int playerProfileId, int guildId)
    {
        var member = await _guildRepo.GetMemberAsync(guildId, playerProfileId, includeProfile: true);
        if (member == null || !IsLeaderOrOfficer(member))
            throw new UnauthorizedAccessException("Not authorized");

        var apps = await _guildRepo.GetPendingApplicationsAsync(guildId);
        return apps.Select(a => new GuildApplicationDTO
        {
            GuildApplicationId = a.GuildApplicationId,
            PlayerProfileId = a.PlayerProfileId,
            PlayerName = a.PlayerProfile?.DisplayName ?? "Unknown",
            PlayerAvatarUrl = a.PlayerProfile?.AvatarUrl ?? "",
            PlayerLevel = a.PlayerProfile?.Level ?? 1,
            Medals = a.PlayerProfile?.Medals ?? 0,
            Feats = a.PlayerProfile?.Feats ?? 0,
            Status = a.Status,
            CreatedAt = a.CreatedAt
        }).ToList();
    }

    public async Task<bool> ApproveApplicationAsync(int playerProfileId, int guildId, int applicationId)
    {
        var executor = await _guildRepo.GetMemberAsync(guildId, playerProfileId, includeProfile: true);
        if (executor == null || !IsLeaderOrOfficer(executor)) return false;

        var application = await _guildRepo.GetApplicationAsync(applicationId, guildId);
        if (application == null) return false;

        var guild = await _guildRepo.GetGuildByIdAsync(guildId, includeMembers: true);
        if (guild == null || guild.Members.Count >= guild.MaxMembers) return false;

        application.Status = "Approved";
        await _guildRepo.UpdateApplicationAsync(application);

        await _guildRepo.AddMemberAsync(new GuildMember
        {
            GuildId = guildId,
            PlayerProfileId = application.PlayerProfileId,
            Role = GuildRole.Member,
            JoinedAt = DateTime.UtcNow
        });

        await _guildRepo.AddLogAsync(new GuildLog
        {
            GuildId = guildId,
            Action = GuildLogAction.ApplicationApproved,
            ActorProfileId = playerProfileId,
            ActorName = executor.PlayerProfile?.DisplayName,
            TargetProfileId = application.PlayerProfileId,
            TargetName = application.PlayerProfile?.DisplayName,
            CreatedAt = DateTime.UtcNow
        });

        await _guildRepo.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RejectApplicationAsync(int playerProfileId, int guildId, int applicationId)
    {
        var executor = await _guildRepo.GetMemberAsync(guildId, playerProfileId, includeProfile: true);
        if (executor == null || !IsLeaderOrOfficer(executor)) return false;

        var application = await _guildRepo.GetApplicationAsync(applicationId, guildId);
        if (application == null) return false;

        application.Status = "Rejected";
        await _guildRepo.UpdateApplicationAsync(application);

        await _guildRepo.AddLogAsync(new GuildLog
        {
            GuildId = guildId,
            Action = GuildLogAction.ApplicationRejected,
            ActorProfileId = playerProfileId,
            ActorName = executor.PlayerProfile?.DisplayName,
            TargetProfileId = application.PlayerProfileId,
            TargetName = application.PlayerProfile?.DisplayName,
            CreatedAt = DateTime.UtcNow
        });

        await _guildRepo.SaveChangesAsync();
        return true;
    }

    // ─── Member Management ────────────────────────────────────────────

    public async Task<bool> KickMemberAsync(int playerProfileId, int guildId, int memberProfileId)
    {
        if (playerProfileId == memberProfileId) return false;

        var executor = await _guildRepo.GetMemberAsync(guildId, playerProfileId, includeProfile: true);
        if (executor == null || !IsLeaderOrOfficer(executor)) return false;

        var target = await _guildRepo.GetMemberAsync(guildId, memberProfileId, includeProfile: true);
        if (target == null || target.Role == GuildRole.Leader) return false;
        if (executor.Role == GuildRole.Officer && target.Role == GuildRole.Officer) return false;

        await _guildRepo.RemoveMemberAsync(target);

        var targetPlayer = await _guildRepo.GetPlayerProfileAsync(memberProfileId);
        if (targetPlayer != null) targetPlayer.LastLeaveGuildAt = DateTime.UtcNow;

        await _guildRepo.AddLogAsync(new GuildLog
        {
            GuildId = guildId,
            Action = GuildLogAction.Kick,
            ActorProfileId = playerProfileId,
            ActorName = executor.PlayerProfile?.DisplayName,
            TargetProfileId = memberProfileId,
            TargetName = target.PlayerProfile?.DisplayName,
            CreatedAt = DateTime.UtcNow
        });

        await _guildRepo.SaveChangesAsync();
        return true;
    }

    public async Task<bool> PromoteMemberAsync(int playerProfileId, int guildId, int memberProfileId)
    {
        var executor = await _guildRepo.GetMemberAsync(guildId, playerProfileId, includeProfile: true);
        if (executor == null || !IsLeader(executor)) return false;

        var target = await _guildRepo.GetMemberAsync(guildId, memberProfileId, includeProfile: true);
        if (target == null || target.Role != GuildRole.Member) return false;

        target.Role = GuildRole.Officer;
        await _guildRepo.UpdateMemberAsync(target);

        await _guildRepo.AddLogAsync(new GuildLog
        {
            GuildId = guildId,
            Action = GuildLogAction.Promote,
            ActorProfileId = playerProfileId,
            ActorName = executor.PlayerProfile?.DisplayName,
            TargetProfileId = memberProfileId,
            TargetName = target.PlayerProfile?.DisplayName,
            Detail = "Member → Officer",
            CreatedAt = DateTime.UtcNow
        });

        await _guildRepo.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DemoteMemberAsync(int playerProfileId, int guildId, int memberProfileId)
    {
        var executor = await _guildRepo.GetMemberAsync(guildId, playerProfileId, includeProfile: true);
        if (executor == null || !IsLeader(executor)) return false;

        var target = await _guildRepo.GetMemberAsync(guildId, memberProfileId, includeProfile: true);
        if (target == null || target.Role != GuildRole.Officer) return false;

        target.Role = GuildRole.Member;
        await _guildRepo.UpdateMemberAsync(target);

        await _guildRepo.AddLogAsync(new GuildLog
        {
            GuildId = guildId,
            Action = GuildLogAction.Demote,
            ActorProfileId = playerProfileId,
            ActorName = executor.PlayerProfile?.DisplayName,
            TargetProfileId = memberProfileId,
            TargetName = target.PlayerProfile?.DisplayName,
            Detail = "Officer → Member",
            CreatedAt = DateTime.UtcNow
        });

        await _guildRepo.SaveChangesAsync();
        return true;
    }

    public async Task<bool> TransferLeaderAsync(int playerProfileId, int guildId, int newLeaderProfileId)
    {
        var guild = await _guildRepo.GetGuildByIdAsync(guildId, includeMembers: true, includeMemberProfiles: true);
        if (guild == null || guild.LeaderId != playerProfileId) return false;

        var currentLeader = guild.Members.FirstOrDefault(m => m.PlayerProfileId == playerProfileId);
        var newLeader = guild.Members.FirstOrDefault(m => m.PlayerProfileId == newLeaderProfileId);
        if (currentLeader == null || newLeader == null) return false;

        currentLeader.Role = GuildRole.Officer;
        newLeader.Role = GuildRole.Leader;
        guild.LeaderId = newLeaderProfileId;

        await _guildRepo.UpdateMemberAsync(currentLeader);
        await _guildRepo.UpdateMemberAsync(newLeader);
        await _guildRepo.UpdateGuildAsync(guild);

        await _guildRepo.AddLogAsync(new GuildLog
        {
            GuildId = guildId,
            Action = GuildLogAction.TransferLeader,
            ActorProfileId = playerProfileId,
            ActorName = currentLeader.PlayerProfile?.DisplayName,
            TargetProfileId = newLeaderProfileId,
            TargetName = newLeader.PlayerProfile?.DisplayName,
            CreatedAt = DateTime.UtcNow
        });

        await _guildRepo.SaveChangesAsync();
        return true;
    }

    public async Task<bool> InviteMemberAsync(int playerProfileId, int guildId, int inviteeProfileId)
    {
        var executor = await _guildRepo.GetMemberAsync(guildId, playerProfileId, includeProfile: true);
        if (executor == null || !IsLeaderOrOfficer(executor))
            throw new Exception("You don't have permission to invite.");

        var guild = await _guildRepo.GetGuildByIdAsync(guildId, includeMembers: true);
        if (guild == null) throw new Exception("Guild not found.");
        if (guild.Members.Count >= guild.MaxMembers) throw new Exception("Guild is full.");

        var invitee = await _guildRepo.GetPlayerProfileAsync(inviteeProfileId, includeGuildMember: true);
        if (invitee == null) throw new Exception("Player not found.");
        if (invitee.GuildMember != null) throw new Exception("Player is already in a guild.");

        var now = DateTime.UtcNow;

        var expiredInvites = await _guildRepo.GetExpiredInvitationsAsync(inviteeProfileId, now);
        foreach (var inv in expiredInvites)
        {
            inv.Status = "Expired";
            await _guildRepo.UpdateInvitationAsync(inv);
        }

        var exists = await _guildRepo.GetActiveInvitationAsync(guildId, inviteeProfileId, now);
        if (exists != null) throw new Exception("Player already has a pending invitation from this guild.");

        await _guildRepo.AddInvitationAsync(new GuildInvitation
        {
            GuildId = guildId,
            InviterId = playerProfileId,
            InviteeId = inviteeProfileId,
            Status = "Pending",
            CreatedAt = now,
            ExpiresAt = now.AddMinutes(5)
        });

        await _guildRepo.AddLogAsync(new GuildLog
        {
            GuildId = guildId,
            Action = GuildLogAction.Invite,
            ActorProfileId = playerProfileId,
            ActorName = executor.PlayerProfile?.DisplayName,
            TargetProfileId = inviteeProfileId,
            TargetName = invitee.DisplayName,
            CreatedAt = DateTime.UtcNow
        });

        await _guildRepo.SaveChangesAsync();
        return true;
    }

    // ─── Settings ────────────────────────────────────────────────────

    public async Task<bool> UpdateSettingsAsync(int playerProfileId, int guildId, UpdateGuildRequestDto request)
    {
        var executor = await _guildRepo.GetMemberAsync(guildId, playerProfileId, includeProfile: true);
        if (executor == null || (executor.Role != GuildRole.Leader && executor.Role != GuildRole.Officer))
            throw new UnauthorizedAccessException("Must be Leader or Officer to update settings");

        var guild = await _guildRepo.GetGuildByIdAsync(guildId);
        if (guild == null) return false;

        if (request.RequiredLevel.HasValue) guild.RequiredLevel = request.RequiredLevel.Value;
        if (request.JoinPolicy.HasValue) guild.JoinPolicy = (GuildJoinPolicy)request.JoinPolicy.Value;
        if (!string.IsNullOrEmpty(request.Name)) guild.Name = request.Name;
        if (!string.IsNullOrEmpty(request.Notice)) guild.Notice = request.Notice;

        await _guildRepo.UpdateGuildAsync(guild);

        await _guildRepo.AddLogAsync(new GuildLog
        {
            GuildId = guildId,
            Action = GuildLogAction.NoticeUpdated,
            ActorProfileId = playerProfileId,
            ActorName = executor.PlayerProfile?.DisplayName,
            CreatedAt = DateTime.UtcNow
        });

        await _guildRepo.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateNoticeAsync(int playerProfileId, int guildId, string notice)
    {
        var executor = await _guildRepo.GetMemberAsync(guildId, playerProfileId, includeProfile: true);
        if (executor == null || !IsLeaderOrOfficer(executor)) return false;

        var guild = await _guildRepo.GetGuildByIdAsync(guildId);
        if (guild == null) return false;

        guild.Notice = notice.Length > 200 ? notice[..200] : notice;
        await _guildRepo.UpdateGuildAsync(guild);

        await _guildRepo.AddLogAsync(new GuildLog
        {
            GuildId = guildId,
            Action = GuildLogAction.NoticeUpdated,
            ActorProfileId = playerProfileId,
            ActorName = executor.PlayerProfile?.DisplayName,
            CreatedAt = DateTime.UtcNow
        });

        await _guildRepo.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateIconAsync(int playerProfileId, int guildId, int iconId, int? bannerId)
    {
        var executor = await _guildRepo.GetMemberAsync(guildId, playerProfileId, includeProfile: true);
        if (executor == null || !IsLeader(executor)) return false;

        var guild = await _guildRepo.GetGuildByIdAsync(guildId);
        if (guild == null) return false;

        guild.IconId = iconId;
        if (bannerId.HasValue) guild.BannerId = bannerId.Value;
        await _guildRepo.UpdateGuildAsync(guild);

        await _guildRepo.AddLogAsync(new GuildLog
        {
            GuildId = guildId,
            Action = GuildLogAction.IconUpdated,
            ActorProfileId = playerProfileId,
            ActorName = executor.PlayerProfile?.DisplayName,
            CreatedAt = DateTime.UtcNow
        });

        await _guildRepo.SaveChangesAsync();
        return true;
    }

    // ─── Donate ───────────────────────────────────────────────────────

    public async Task<GuildDonateResultDto> DonateAsync(int playerProfileId, int guildId, string currencyType, int amount)
    {
        var guild = await _guildRepo.GetGuildByIdAsync(guildId);
        if (guild == null) throw new Exception("Guild not found");

        var member = await _guildRepo.GetMemberAsync(guildId, playerProfileId, includeProfile: true);
        if (member == null) throw new Exception("Not a member");

        if (member.LastDonateAt.HasValue && member.LastDonateAt.Value.Date == DateTime.UtcNow.Date)
        {
            throw new Exception("You have already donated today. Please try again tomorrow.");
        }

        var player = await _guildRepo.GetPlayerProfileAsync(playerProfileId);
        if (player == null) throw new Exception("Player not found");

        int expGained = 0;
        int medalsGained = 0;
        int playerFeats = 0;
        int goldSpent = 0;
        int gemSpent = 0;

        if (currencyType.Equals("Gold", StringComparison.OrdinalIgnoreCase))
        {
            if (player.Gold < amount) throw new Exception($"Not enough gold. Need {amount}");
            player.Gold -= amount;
            goldSpent = amount;

            // 10,000 Gold = 100 Feats/Exp (Amount / 100)
            expGained = amount / 100;
            medalsGained = amount / 100;
            playerFeats = amount / 100;
        }
        else if (currencyType.Equals("Gem", StringComparison.OrdinalIgnoreCase))
        {
            if (player.Gems < amount) throw new Exception($"Not enough gems. Need {amount}");
            player.Gems -= amount;
            gemSpent = amount;

            // 50 Gem = 500 Feats/Exp (Amount * 10)
            expGained = amount * 10;
            medalsGained = amount * 10;
            playerFeats = amount * 10;
        }
        else
        {
            throw new Exception("Invalid currency type for donation.");
        }

        await _guildRepo.UpdatePlayerProfileAsync(player);

        guild.GuildExp += expGained;
        guild.TotalMedals += medalsGained;
        guild.TotalFeats += playerFeats;

        int playerMedals = medalsGained; // Same as medalsGained for simplicity
        member.Medals += playerMedals;
        member.Feats += playerFeats;
        member.DailyContribution += playerFeats; // Track contribution by feats instead of raw amount
        member.WeeklyContribution += playerFeats;
        member.TotalContribution += playerFeats;
        member.Contribution += playerFeats;
        member.LastDonateAt = DateTime.UtcNow;

        await _guildRepo.UpdateGuildAsync(guild);
        await _guildRepo.UpdateMemberAsync(member);

        await _guildRepo.SaveChangesAsync();

        return new GuildDonateResultDto
        {
            GoldSpent = goldSpent,
            GemSpent = gemSpent,
            GuildExpGained = expGained,
            GuildMedalsGained = medalsGained,
            PlayerMedalsGained = playerMedals,
            PlayerFeatsGained = playerFeats,
            GuildLeveledUp = false,
            NewGuildLevel = guild.Level,
            NewGuildExp = guild.GuildExp,
            ExpToNextLevel = guild.ExpToNextLevel,
            TotalMedals = guild.TotalMedals,
            MedalsToNextLevel = guild.MedalsToNextLevel
        };
    }

    public async Task<bool> LevelUpAsync(int playerProfileId, int guildId)
    {
        var executor = await _guildRepo.GetMemberAsync(guildId, playerProfileId, includeProfile: true);
        if (executor == null || !IsLeader(executor)) return false;

        var guild = await _guildRepo.GetGuildByIdAsync(guildId);
        if (guild == null) return false;

        if (guild.GuildExp < guild.ExpToNextLevel || guild.TotalMedals < guild.MedalsToNextLevel)
        {
            throw new Exception("Not enough Guild EXP or Guild Medals to level up.");
        }

        int medalCost = guild.MedalsToNextLevel;
        guild.TotalMedals -= medalCost;
        guild.GuildExp -= guild.ExpToNextLevel;
        guild.Level++;

        await _guildRepo.UpdateGuildAsync(guild);

        await _guildRepo.AddLogAsync(new GuildLog
        {
            GuildId = guildId,
            Action = GuildLogAction.LevelUp,
            ActorProfileId = playerProfileId,
            ActorName = executor.PlayerProfile?.DisplayName,
            Detail = $"Guild leveled up to {guild.Level} (cost {medalCost} medals)",
            CreatedAt = DateTime.UtcNow
        });

        await _guildRepo.SaveChangesAsync();
        return true;
    }

    // ─── Logs ─────────────────────────────────────────────────────────

    public async Task<List<GuildLogDto>> GetLogsAsync(int playerProfileId, int guildId)
    {
        if (!await _guildRepo.IsGuildMemberAsync(guildId, playerProfileId))
            throw new UnauthorizedAccessException("Not a member");

        var logs = await _guildRepo.GetRecentLogsAsync(guildId, 50);
        return logs.Select(l => new GuildLogDto
        {
            GuildLogId = l.GuildLogId,
            Action = l.Action.ToString(),
            ActorName = l.ActorName ?? "System",
            TargetName = l.TargetName,
            Detail = l.Detail,
            CreatedAt = l.CreatedAt
        }).ToList();
    }

    // ─── Chat ─────────────────────────────────────────────────────────

    public async Task<List<GuildMessageDTO>> GetGuildChatAsync(int playerProfileId, int guildId)
    {
        if (!await _guildRepo.IsGuildMemberAsync(guildId, playerProfileId))
            throw new UnauthorizedAccessException("Not a member");

        var messages = await _guildRepo.GetRecentChatAsync(guildId, 50);
        return messages
            .OrderBy(m => m.SentAt)
            .Select(m => new GuildMessageDTO
            {
                MessageId = m.GuildChatMessageId,
                SenderId = m.SenderId,
                SenderName = m.Sender != null ? m.Sender.DisplayName : "Unknown",
                Content = m.Content,
                MessageType = (int)m.MessageType,
                SenderRole = (int)m.SenderRole,
                SentAt = m.SentAt
            }).ToList();
    }

    public async Task<GuildMessageDTO> SendGuildMessageAsync(int playerProfileId, int guildId, string content)
    {
        var member = await _guildRepo.GetMemberAsync(guildId, playerProfileId, includeProfile: true);
        if (member == null) throw new UnauthorizedAccessException("Not a member");

        if (member.LastChatAt.HasValue
            && (DateTime.UtcNow - member.LastChatAt.Value).TotalMilliseconds < ChatSpamCooldownMs)
        {
            throw new InvalidOperationException("Sending messages too fast. Please wait a moment.");
        }

        var message = new GuildChatMessage
        {
            GuildId = guildId,
            SenderId = playerProfileId,
            Content = content,
            MessageType = GuildMessageType.Text,
            SenderRole = member.Role,
            SentAt = DateTime.UtcNow
        };

        member.LastChatAt = message.SentAt;
        await _guildRepo.UpdateMemberAsync(member);
        await _guildRepo.AddChatMessageAsync(message);
        await _guildRepo.SaveChangesAsync();

        return new GuildMessageDTO
        {
            MessageId = message.GuildChatMessageId,
            SenderId = message.SenderId,
            SenderName = member.PlayerProfile!.DisplayName,
            Content = message.Content,
            MessageType = (int)message.MessageType,
            SenderRole = (int)message.SenderRole,
            SentAt = message.SentAt
        };
    }
}
