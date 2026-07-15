using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Data;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class GuildService : IGuildService
    {
        private readonly MysticJourneyDbContext _context;
        private const int LeaveCooldownHours = 24;
        private const int DonateGoldCostPerUnit = 100;  // 100 gold per donate unit
        private const int DonateExpGainPerUnit = 50;    // 50 Guild EXP per donate
        private const int DonateMedalsGainPerUnit = 10; // 10 Guild Medals per donate
        private const int DonatePlayerMedalsPerUnit = 10; // 10 personal medals
        private const int DonatePlayerFeatsPerUnit = 5;
        private const int ChatSpamCooldownMs = 1000; // 1 message per second

        public GuildService(MysticJourneyDbContext context) => _context = context;

        // ─── Permission Helper ────────────────────────────────────────────

        private static bool IsLeaderOrOfficer(GuildMember m) =>
            m.Role == GuildRole.Leader || m.Role == GuildRole.Officer;

        private static bool IsLeader(GuildMember m) => m.Role == GuildRole.Leader;

        // ─── Log Helper ───────────────────────────────────────────────────

        private void AddLog(int guildId, GuildLogAction action,
            int? actorId, string? actorName,
            int? targetId = null, string? targetName = null,
            string? detail = null)
        {
            _context.GuildLogs.Add(new GuildLog
            {
                GuildId = guildId,
                Action = action,
                ActorProfileId = actorId,
                ActorName = actorName,
                TargetProfileId = targetId,
                TargetName = targetName,
                Detail = detail,
                CreatedAt = DateTime.UtcNow
            });
        }

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
                // Reuse shared LastSeen from Account — single source of truth
                IsOnline = m.PlayerProfile != null && m.PlayerProfile.Account != null && m.PlayerProfile.Account.LastSeen.HasValue &&
                           (DateTime.UtcNow - m.PlayerProfile.Account.LastSeen.Value).TotalMinutes < 5
            };
        }

        // ─── View ─────────────────────────────────────────────────────────

        public async Task<GuildDetailResponseDto?> GetMyGuildAsync(int playerProfileId)
        {
            var member = await _context.GuildMembers.FirstOrDefaultAsync(m => m.PlayerProfileId == playerProfileId && m.LeftAt == null);
            if (member != null)
                return await GetGuildDetailAsync(member.GuildId);

            // Fallback: check if this player is a leader of an active guild
            // This handles the case where GuildMember row was missing (e.g. creation race condition)
            var leadedGuild = await _context.Guilds
                .FirstOrDefaultAsync(g => g.LeaderId == playerProfileId && g.IsActive);

            if (leadedGuild != null)
            {
                // Auto-recover: insert the missing GuildMember row for the leader
                var recoveredMember = new GuildMember
                {
                    GuildId = leadedGuild.GuildId,
                    PlayerProfileId = playerProfileId,
                    Role = GuildRole.Leader,
                    JoinedAt = leadedGuild.CreatedAt
                };
                _context.GuildMembers.Add(recoveredMember);
                await _context.SaveChangesAsync();
                return await GetGuildDetailAsync(leadedGuild.GuildId);
            }

            return null;
        }

        public async Task<List<GuildResponseDto>> GetGuildListAsync(
            string searchTerm = "", int? joinPolicy = null, int? minLevel = null)
        {
            var query = _context.Guilds.Where(g => g.IsActive).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(g => g.Name.Contains(searchTerm));
            if (minLevel.HasValue)
                query = query.Where(g => g.RequiredLevel <= minLevel.Value);
            if (joinPolicy.HasValue)
                query = query.Where(g => (int)g.JoinPolicy == joinPolicy.Value);

            var guilds = await query.Include(g => g.Members).Take(20).ToListAsync();

            return guilds.Select(g => MapGuildDto(g, g.Members.Count)).ToList();
        }

        public async Task<List<GuildRankResponseDto>> GetGuildRankingsAsync(int top = 100)
        {
            var query = _context.Guilds
                .Where(g => g.IsActive)
                .Include(g => g.Members)
                .OrderByDescending(g => g.TotalFeats)
                .ThenByDescending(g => g.GuildExp);

            var guilds = await query.Take(top).ToListAsync();

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
            var guild = await _context.Guilds
                .Include(g => g.Leader)
                .Include(g => g.Members.Where(m => m.LeftAt == null)).ThenInclude(m => m.PlayerProfile).ThenInclude(p => p.Account)
                .FirstOrDefaultAsync(g => g.GuildId == guildId && g.IsActive);

            if (guild == null) return null;

            var dto = new GuildDetailResponseDto();
            var baseDto = MapGuildDto(guild, guild.Members.Count);
            // copy base properties
            dto.GuildId = baseDto.GuildId; dto.Name = baseDto.Name; dto.Notice = baseDto.Notice;
            dto.IconId = baseDto.IconId; dto.BannerId = baseDto.BannerId;
            dto.LeaderId = baseDto.LeaderId; dto.Level = baseDto.Level;
            dto.GuildExp = baseDto.GuildExp; dto.ExpToNextLevel = baseDto.ExpToNextLevel;
            dto.MedalsToNextLevel = baseDto.MedalsToNextLevel;
            dto.MemberCount = baseDto.MemberCount; dto.MaxMembers = baseDto.MaxMembers;
            dto.RequiredLevel = baseDto.RequiredLevel; dto.JoinPolicy = baseDto.JoinPolicy;
            dto.TotalMedals = baseDto.TotalMedals; dto.IsActive = baseDto.IsActive;
            dto.CreatedAt = baseDto.CreatedAt; dto.Description = baseDto.Description;
            dto.LeaderName = guild.Leader?.DisplayName ?? "Unknown";
            dto.Members = guild.Members.Select(MapMemberDto).ToList();
            return dto;
        }

        public async Task<List<GuildMemberResponseDto>> GetMembersAsync(int playerProfileId, int guildId)
        {
            // Any active member can view the member list
            var isMember = await _context.GuildMembers
                .AnyAsync(m => m.GuildId == guildId && m.PlayerProfileId == playerProfileId && m.LeftAt == null);
            if (!isMember) throw new UnauthorizedAccessException("Not a member");

            var members = await _context.GuildMembers
                .Include(m => m.PlayerProfile)
                .Where(m => m.GuildId == guildId && m.LeftAt == null)
                .ToListAsync();

            return members.Select(MapMemberDto).ToList();
        }

        // ─── Create / Dissolve ────────────────────────────────────────────

        public async Task<GuildResponseDto> CreateGuildAsync(int playerProfileId, CreateGuildRequestDto request)
        {
            var player = await _context.PlayerProfiles
                .Include(p => p.GuildMember)
                .FirstOrDefaultAsync(p => p.PlayerProfileId == playerProfileId);

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

            _context.Guilds.Add(guild);
            await _context.SaveChangesAsync();

            AddLog(guild.GuildId, GuildLogAction.Join, playerProfileId, player.DisplayName,
                playerProfileId, player.DisplayName, "Guild founded");
            await _context.SaveChangesAsync();

            return MapGuildDto(guild, 1);
        }

        public async Task<bool> DissolveGuildAsync(int playerProfileId, int guildId)
        {
            var guild = await _context.Guilds
                .Include(g => g.Members)
                .FirstOrDefaultAsync(g => g.GuildId == guildId && g.IsActive);
            if (guild == null || guild.LeaderId != playerProfileId) return false;

            var player = await _context.PlayerProfiles.FindAsync(playerProfileId);

            // Soft-delete guild
            guild.IsActive = false;

            // Set LastLeaveAt for all members and remove GuildMember records
            var memberIds = guild.Members.Select(m => m.PlayerProfileId).ToList();
            var profiles = await _context.PlayerProfiles
                .Where(p => memberIds.Contains(p.PlayerProfileId))
                .ToListAsync();
            foreach (var profile in profiles)
                profile.LastLeaveAt = DateTime.UtcNow;

            _context.GuildMembers.RemoveRange(guild.Members);

            // Clean up pending applications
            var pendingApps = await _context.GuildApplications
                .Where(a => a.GuildId == guildId && a.Status == "Pending")
                .ToListAsync();
            _context.GuildApplications.RemoveRange(pendingApps);

            AddLog(guildId, GuildLogAction.GuildDissolved, playerProfileId, player?.DisplayName);
            await _context.SaveChangesAsync();
            return true;
        }

        // ─── Join / Leave / Apply ─────────────────────────────────────────

        public async Task<GuildJoinResultDto> ApplyToGuildAsync(int playerProfileId, int guildId)
        {
            var player = await _context.PlayerProfiles
                .Include(p => p.GuildMember)
                .FirstOrDefaultAsync(p => p.PlayerProfileId == playerProfileId);

            if (player == null) return new GuildJoinResultDto { Success = false, Message = "Player not found" };
            if (player.GuildMember != null)
                return new GuildJoinResultDto { Success = false, Message = "Already in a guild" };

            // Check leave cooldown
            if (player.LastLeaveAt.HasValue)
            {
                var remaining = (player.LastLeaveAt.Value.AddHours(LeaveCooldownHours) - DateTime.UtcNow);
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

            var guild = await _context.Guilds.Include(g => g.Members)
                .FirstOrDefaultAsync(g => g.GuildId == guildId && g.IsActive);
            if (guild == null) return new GuildJoinResultDto { Success = false, Message = "Guild not found" };
            if (guild.Members.Count >= guild.MaxMembers)
                return new GuildJoinResultDto { Success = false, Message = "Guild is full" };
            if (player.Level < guild.RequiredLevel)
                return new GuildJoinResultDto { Success = false, Message = $"Required level {guild.RequiredLevel}" };
            if (guild.JoinPolicy == GuildJoinPolicy.InviteOnly)
                return new GuildJoinResultDto { Success = false, Message = "This guild is invite only" };

            // Open policy: join directly
            if (guild.JoinPolicy == GuildJoinPolicy.Open)
            {
                _context.GuildMembers.Add(new GuildMember
                {
                    GuildId = guildId,
                    PlayerProfileId = playerProfileId,
                    Role = GuildRole.Member,
                    JoinedAt = DateTime.UtcNow
                });
                AddLog(guildId, GuildLogAction.Join, playerProfileId, player.DisplayName);
                await _context.SaveChangesAsync();
                return new GuildJoinResultDto { Success = true, Message = "Joined guild" };
            }

            // Approval: check for any existing pending application globally (1 per player)
            var existingApp = await _context.GuildApplications
                .FirstOrDefaultAsync(a => a.PlayerProfileId == playerProfileId && a.Status == "Pending");
            if (existingApp != null)
                return new GuildJoinResultDto { Success = false, Message = "You already have a pending application to another guild" };

            _context.GuildApplications.Add(new GuildApplication
            {
                GuildId = guildId,
                PlayerProfileId = playerProfileId,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
            return new GuildJoinResultDto { Success = true, Message = "Application submitted" };
        }

        public async Task<GuildJoinResultDto> LeaveGuildAsync(int playerProfileId, int guildId)
        {
            var member = await _context.GuildMembers
                .FirstOrDefaultAsync(m => m.GuildId == guildId && m.PlayerProfileId == playerProfileId);
            if (member == null) return new GuildJoinResultDto { Success = false, Message = "Not a member" };

            if (member.Role == GuildRole.Leader)
                return new GuildJoinResultDto
                {
                    Success = false,
                    Message = "Leader must transfer leadership or dissolve the guild before leaving"
                };

            var player = await _context.PlayerProfiles.FindAsync(playerProfileId);
            if (player != null) player.LastLeaveAt = DateTime.UtcNow;

            _context.GuildMembers.Remove(member);
            AddLog(guildId, GuildLogAction.Leave, playerProfileId, player?.DisplayName);
            await _context.SaveChangesAsync();

            return new GuildJoinResultDto { Success = true, Message = "Left guild" };
        }

        // ─── Applications ─────────────────────────────────────────────────

        public async Task<List<GuildApplicationDTO>> GetApplicationsAsync(int playerProfileId, int guildId)
        {
            var member = await _context.GuildMembers
                .FirstOrDefaultAsync(m => m.GuildId == guildId && m.PlayerProfileId == playerProfileId);
            if (member == null || !IsLeaderOrOfficer(member))
                throw new UnauthorizedAccessException("Not authorized");

            return await _context.GuildApplications
                .Include(a => a.PlayerProfile)
                .Where(a => a.GuildId == guildId && a.Status == "Pending")
                .Select(a => new GuildApplicationDTO
                {
                    GuildApplicationId = a.GuildApplicationId,
                    PlayerProfileId = a.PlayerProfileId,
                    PlayerName = a.PlayerProfile!.DisplayName,
                    PlayerAvatarUrl = a.PlayerProfile.AvatarUrl,
                    PlayerLevel = a.PlayerProfile.Level,
                    Medals = a.PlayerProfile.Medals,
                    Feats = a.PlayerProfile.Feats,
                    Status = a.Status,
                    CreatedAt = a.CreatedAt
                }).ToListAsync();
        }

        public async Task<bool> ApproveApplicationAsync(int playerProfileId, int guildId, int applicationId)
        {
            var executor = await _context.GuildMembers.Include(m => m.PlayerProfile)
                .FirstOrDefaultAsync(m => m.GuildId == guildId && m.PlayerProfileId == playerProfileId);
            if (executor == null || !IsLeaderOrOfficer(executor)) return false;

            var application = await _context.GuildApplications.Include(a => a.PlayerProfile)
                .FirstOrDefaultAsync(a => a.GuildApplicationId == applicationId
                    && a.GuildId == guildId && a.Status == "Pending");
            if (application == null) return false;

            var guild = await _context.Guilds.Include(g => g.Members)
                .FirstOrDefaultAsync(g => g.GuildId == guildId && g.IsActive);
            if (guild == null || guild.Members.Count >= guild.MaxMembers) return false;

            application.Status = "Approved";
            _context.GuildMembers.Add(new GuildMember
            {
                GuildId = guildId,
                PlayerProfileId = application.PlayerProfileId,
                Role = GuildRole.Member,
                JoinedAt = DateTime.UtcNow
            });

            AddLog(guildId, GuildLogAction.ApplicationApproved,
                playerProfileId, executor.PlayerProfile?.DisplayName,
                application.PlayerProfileId, application.PlayerProfile?.DisplayName);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectApplicationAsync(int playerProfileId, int guildId, int applicationId)
        {
            var executor = await _context.GuildMembers.Include(m => m.PlayerProfile)
                .FirstOrDefaultAsync(m => m.GuildId == guildId && m.PlayerProfileId == playerProfileId);
            if (executor == null || !IsLeaderOrOfficer(executor)) return false;

            var application = await _context.GuildApplications.Include(a => a.PlayerProfile)
                .FirstOrDefaultAsync(a => a.GuildApplicationId == applicationId
                    && a.GuildId == guildId && a.Status == "Pending");
            if (application == null) return false;

            application.Status = "Rejected";
            AddLog(guildId, GuildLogAction.ApplicationRejected,
                playerProfileId, executor.PlayerProfile?.DisplayName,
                application.PlayerProfileId, application.PlayerProfile?.DisplayName);
            await _context.SaveChangesAsync();
            return true;
        }

        // ─── Member Management ────────────────────────────────────────────

        public async Task<bool> KickMemberAsync(int playerProfileId, int guildId, int memberProfileId)
        {
            if (playerProfileId == memberProfileId) return false;

            var executor = await _context.GuildMembers.Include(m => m.PlayerProfile)
                .FirstOrDefaultAsync(m => m.GuildId == guildId && m.PlayerProfileId == playerProfileId);
            if (executor == null || !IsLeaderOrOfficer(executor)) return false;

            var target = await _context.GuildMembers.Include(m => m.PlayerProfile)
                .FirstOrDefaultAsync(m => m.GuildId == guildId && m.PlayerProfileId == memberProfileId);
            if (target == null || target.Role == GuildRole.Leader) return false;
            // Officers cannot kick other Officers
            if (executor.Role == GuildRole.Officer && target.Role == GuildRole.Officer) return false;

            _context.GuildMembers.Remove(target);

            var targetPlayer = await _context.PlayerProfiles.FindAsync(memberProfileId);
            if (targetPlayer != null) targetPlayer.LastLeaveAt = DateTime.UtcNow;

            AddLog(guildId, GuildLogAction.Kick,
                playerProfileId, executor.PlayerProfile?.DisplayName,
                memberProfileId, target.PlayerProfile?.DisplayName);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> PromoteMemberAsync(int playerProfileId, int guildId, int memberProfileId)
        {
            var executor = await _context.GuildMembers.Include(m => m.PlayerProfile)
                .FirstOrDefaultAsync(m => m.GuildId == guildId && m.PlayerProfileId == playerProfileId);
            if (executor == null || !IsLeader(executor)) return false;

            var target = await _context.GuildMembers.Include(m => m.PlayerProfile)
                .FirstOrDefaultAsync(m => m.GuildId == guildId && m.PlayerProfileId == memberProfileId);
            if (target == null || target.Role != GuildRole.Member) return false;

            target.Role = GuildRole.Officer;
            AddLog(guildId, GuildLogAction.Promote,
                playerProfileId, executor.PlayerProfile?.DisplayName,
                memberProfileId, target.PlayerProfile?.DisplayName, "Member → Officer");
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DemoteMemberAsync(int playerProfileId, int guildId, int memberProfileId)
        {
            var executor = await _context.GuildMembers.Include(m => m.PlayerProfile)
                .FirstOrDefaultAsync(m => m.GuildId == guildId && m.PlayerProfileId == playerProfileId);
            if (executor == null || !IsLeader(executor)) return false;

            var target = await _context.GuildMembers.Include(m => m.PlayerProfile)
                .FirstOrDefaultAsync(m => m.GuildId == guildId && m.PlayerProfileId == memberProfileId);
            if (target == null || target.Role != GuildRole.Officer) return false;

            target.Role = GuildRole.Member;
            AddLog(guildId, GuildLogAction.Demote,
                playerProfileId, executor.PlayerProfile?.DisplayName,
                memberProfileId, target.PlayerProfile?.DisplayName, "Officer → Member");
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> TransferLeaderAsync(int playerProfileId, int guildId, int newLeaderProfileId)
        {
            var guild = await _context.Guilds.Include(g => g.Members)
                .ThenInclude(m => m.PlayerProfile)
                .FirstOrDefaultAsync(g => g.GuildId == guildId && g.IsActive);
            if (guild == null || guild.LeaderId != playerProfileId) return false;

            var currentLeader = guild.Members.FirstOrDefault(m => m.PlayerProfileId == playerProfileId);
            var newLeader = guild.Members.FirstOrDefault(m => m.PlayerProfileId == newLeaderProfileId);
            if (currentLeader == null || newLeader == null) return false;

            currentLeader.Role = GuildRole.Officer;
            newLeader.Role = GuildRole.Leader;
            guild.LeaderId = newLeaderProfileId;

            AddLog(guildId, GuildLogAction.TransferLeader,
                playerProfileId, currentLeader.PlayerProfile?.DisplayName,
                newLeaderProfileId, newLeader.PlayerProfile?.DisplayName);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> InviteMemberAsync(int playerProfileId, int guildId, int inviteeProfileId)
        {
            var executor = await _context.GuildMembers.Include(m => m.PlayerProfile)
                .FirstOrDefaultAsync(m => m.GuildId == guildId && m.PlayerProfileId == playerProfileId);
            if (executor == null || !IsLeaderOrOfficer(executor)) return false;

            var guild = await _context.Guilds.Include(g => g.Members)
                .FirstOrDefaultAsync(g => g.GuildId == guildId && g.IsActive);
            if (guild == null || guild.Members.Count >= guild.MaxMembers) return false;

            var invitee = await _context.PlayerProfiles.Include(p => p.GuildMember)
                .FirstOrDefaultAsync(p => p.PlayerProfileId == inviteeProfileId);
            if (invitee == null || invitee.GuildMember != null) return false;

            // Expire old invitations first
            var now = DateTime.UtcNow;
            var expiredInvites = await _context.GuildInvitations
                .Where(i => i.InviteeId == inviteeProfileId && i.Status == "Pending" && i.ExpiresAt < now)
                .ToListAsync();
            foreach (var inv in expiredInvites) inv.Status = "Expired";

            // Check no active pending invitation
            var exists = await _context.GuildInvitations.AnyAsync(i =>
                i.GuildId == guildId && i.InviteeId == inviteeProfileId
                && i.Status == "Pending" && i.ExpiresAt >= now);
            if (exists) return false;

            _context.GuildInvitations.Add(new GuildInvitation
            {
                GuildId = guildId,
                InviterId = playerProfileId,
                InviteeId = inviteeProfileId,
                Status = "Pending",
                CreatedAt = now,
                ExpiresAt = now.AddMinutes(5)
            });

            AddLog(guildId, GuildLogAction.Invite,
                playerProfileId, executor.PlayerProfile?.DisplayName,
                inviteeProfileId, invitee.DisplayName);
            await _context.SaveChangesAsync();
            return true;
        }

        // ─── Settings ────────────────────────────────────────────────────

        public async Task<bool> UpdateSettingsAsync(int playerProfileId, int guildId, UpdateGuildRequestDto request)
        {
            var executor = await _context.GuildMembers.Include(m => m.PlayerProfile)
                .FirstOrDefaultAsync(m => m.GuildId == guildId && m.PlayerProfileId == playerProfileId);

            if (executor == null || (executor.Role != GuildRole.Leader && executor.Role != GuildRole.Officer))
                throw new UnauthorizedAccessException("Must be Leader or Officer to update settings");

            var guild = await _context.Guilds.FindAsync(guildId);
            if (guild == null) return false;

            if (request.RequiredLevel.HasValue) guild.RequiredLevel = request.RequiredLevel.Value;
            if (request.JoinPolicy.HasValue) guild.JoinPolicy = (GuildJoinPolicy)request.JoinPolicy.Value;
            if (!string.IsNullOrEmpty(request.Name)) guild.Name = request.Name;
            if (!string.IsNullOrEmpty(request.Notice)) guild.Notice = request.Notice;

            AddLog(guildId, GuildLogAction.NoticeUpdated, playerProfileId, executor.PlayerProfile?.DisplayName);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateNoticeAsync(int playerProfileId, int guildId, string notice)
        {
            var executor = await _context.GuildMembers.Include(m => m.PlayerProfile)
                .FirstOrDefaultAsync(m => m.GuildId == guildId && m.PlayerProfileId == playerProfileId);
            if (executor == null || !IsLeaderOrOfficer(executor)) return false;

            var guild = await _context.Guilds.FindAsync(guildId);
            if (guild == null) return false;

            guild.Notice = notice.Length > 200 ? notice[..200] : notice;
            AddLog(guildId, GuildLogAction.NoticeUpdated, playerProfileId, executor.PlayerProfile?.DisplayName);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateIconAsync(int playerProfileId, int guildId, int iconId, int? bannerId)
        {
            var executor = await _context.GuildMembers.Include(m => m.PlayerProfile)
                .FirstOrDefaultAsync(m => m.GuildId == guildId && m.PlayerProfileId == playerProfileId);
            if (executor == null || !IsLeader(executor)) return false;

            var guild = await _context.Guilds.FindAsync(guildId);
            if (guild == null) return false;

            guild.IconId = iconId;
            if (bannerId.HasValue) guild.BannerId = bannerId.Value;
            AddLog(guildId, GuildLogAction.IconUpdated, playerProfileId, executor.PlayerProfile?.DisplayName);
            await _context.SaveChangesAsync();
            return true;
        }

        // ─── Donate ───────────────────────────────────────────────────────

        public async Task<GuildDonateResultDto> DonateAsync(int playerProfileId, int guildId, int amount)
        {
            var guild = await _context.Guilds.FirstOrDefaultAsync(g => g.GuildId == guildId && g.IsActive);
            if (guild == null) throw new Exception("Guild not found");

            var member = await _context.GuildMembers.Include(m => m.PlayerProfile)
                .FirstOrDefaultAsync(m => m.GuildId == guildId && m.PlayerProfileId == playerProfileId);
            if (member == null) throw new Exception("Not a member");

            var player = await _context.PlayerProfiles.FindAsync(playerProfileId);
            if (player == null) throw new Exception("Player not found");

            int totalCost = amount * DonateGoldCostPerUnit;
            if (player.Gold < totalCost) throw new Exception($"Not enough gold. Need {totalCost}");

            player.Gold -= totalCost;

            int expGained = amount * DonateExpGainPerUnit;
            int medalsGained = amount * DonateMedalsGainPerUnit;
            int playerFeats = amount * DonatePlayerFeatsPerUnit;

            guild.GuildExp += expGained;
            guild.TotalMedals += medalsGained;
            guild.TotalFeats += playerFeats;

            int playerMedals = amount * DonatePlayerMedalsPerUnit;
            member.Medals += playerMedals;
            member.Feats += playerFeats;
            member.DailyContribution += amount;
            member.WeeklyContribution += amount;
            member.TotalContribution += amount;
            member.Contribution += amount;
            member.LastDonateAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new GuildDonateResultDto
            {
                GoldSpent = totalCost,
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
            var executor = await _context.GuildMembers.Include(m => m.PlayerProfile)
                .FirstOrDefaultAsync(m => m.GuildId == guildId && m.PlayerProfileId == playerProfileId);
            if (executor == null || !IsLeader(executor)) return false;

            var guild = await _context.Guilds.FirstOrDefaultAsync(g => g.GuildId == guildId && g.IsActive);
            if (guild == null) return false;

            if (guild.GuildExp < guild.ExpToNextLevel || guild.TotalMedals < guild.MedalsToNextLevel)
            {
                throw new Exception("Not enough Guild EXP or Guild Medals to level up.");
            }

            int medalCost = guild.MedalsToNextLevel;
            guild.TotalMedals -= medalCost;
            guild.GuildExp -= guild.ExpToNextLevel;
            guild.Level++;

            AddLog(guildId, GuildLogAction.LevelUp, playerProfileId, executor.PlayerProfile?.DisplayName, null, null,
                $"Guild leveled up to {guild.Level} (cost {medalCost} medals)");
            
            await _context.SaveChangesAsync();
            return true;
        }

        // ─── Logs ─────────────────────────────────────────────────────────

        public async Task<List<GuildLogDto>> GetLogsAsync(int playerProfileId, int guildId)
        {
            var isMember = await _context.GuildMembers
                .AnyAsync(m => m.GuildId == guildId && m.PlayerProfileId == playerProfileId);
            if (!isMember) throw new UnauthorizedAccessException("Not a member");

            return await _context.GuildLogs
                .Where(l => l.GuildId == guildId)
                .OrderByDescending(l => l.CreatedAt)
                .Take(50)
                .Select(l => new GuildLogDto
                {
                    GuildLogId = l.GuildLogId,
                    Action = l.Action.ToString(),
                    ActorName = l.ActorName ?? "System",
                    TargetName = l.TargetName,
                    Detail = l.Detail,
                    CreatedAt = l.CreatedAt
                })
                .ToListAsync();
        }

        // ─── Chat ─────────────────────────────────────────────────────────

        public async Task<List<GuildMessageDTO>> GetGuildChatAsync(int playerProfileId, int guildId)
        {
            var isMember = await _context.GuildMembers
                .AnyAsync(m => m.GuildId == guildId && m.PlayerProfileId == playerProfileId);
            if (!isMember) throw new UnauthorizedAccessException("Not a member");

            return await _context.GuildChatMessages
                .Where(m => m.GuildId == guildId)
                .OrderByDescending(m => m.SentAt)
                .Take(50)
                .Select(m => new GuildMessageDTO
                {
                    MessageId = m.GuildChatMessageId,
                    SenderId = m.SenderId,
                    SenderName = m.Sender != null ? m.Sender.DisplayName : "Unknown",
                    Content = m.Content,
                    MessageType = (int)m.MessageType,
                    SenderRole = (int)m.SenderRole,
                    SentAt = m.SentAt
                })
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<GuildMessageDTO> SendGuildMessageAsync(int playerProfileId, int guildId, string content)
        {
            var member = await _context.GuildMembers
                .Include(m => m.PlayerProfile)
                .FirstOrDefaultAsync(m => m.GuildId == guildId && m.PlayerProfileId == playerProfileId);
            if (member == null) throw new UnauthorizedAccessException("Not a member");

            // Anti-spam: 1 message per second
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
            _context.GuildChatMessages.Add(message);
            await _context.SaveChangesAsync();

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
}
