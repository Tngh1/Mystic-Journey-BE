using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class GuildRepository : IGuildRepository
{
    private readonly MysticJourneyDbContext _ctx;

    public GuildRepository(MysticJourneyDbContext ctx) => _ctx = ctx;

    // ─── Guild ─────────────────────────────────────────────────────────────────

    public async Task<Guild?> GetGuildByIdAsync(int guildId,
        bool includeMembers = false,
        bool includeLeader = false,
        bool includeMemberProfiles = false)
    {
        var query = _ctx.Guilds.AsQueryable();

        if (includeLeader)
            query = query.Include(g => g.Leader);

        if (includeMembers || includeMemberProfiles)
        {
            query = query.Include(g => g.Members.Where(m => m.LeftAt == null));

            if (includeMemberProfiles)
                query = query
                    .Include(g => g.Members.Where(m => m.LeftAt == null))
                    .ThenInclude(m => m.PlayerProfile);
        }

        return await query.FirstOrDefaultAsync(g => g.GuildId == guildId && g.IsActive);
    }

    public async Task<Guild?> GetActiveGuildByLeaderAsync(int leaderProfileId)
        => await _ctx.Guilds.FirstOrDefaultAsync(g => g.LeaderId == leaderProfileId && g.IsActive);

    public async Task<List<Guild>> GetGuildsByIdsAsync(IEnumerable<int> guildIds)
    {
        var ids = guildIds.ToList();
        return ids.Count == 0
            ? []
            : await _ctx.Guilds
                .Include(g => g.Members)
                .Where(g => g.IsActive && ids.Contains(g.GuildId))
                .ToListAsync();
    }

    public async Task<List<Guild>> GetActiveGuildsAsync(
        IEnumerable<int> excludeIds,
        string? searchTerm,
        int? joinPolicy,
        int? minLevel,
        int take)
    {
        var exclude = excludeIds.ToList();
        var query = _ctx.Guilds
            .Where(g => g.IsActive && !exclude.Contains(g.GuildId))
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(g => g.Name.Contains(searchTerm));
        if (minLevel.HasValue)
            query = query.Where(g => g.RequiredLevel <= minLevel.Value);
        if (joinPolicy.HasValue)
            query = query.Where(g => (int)g.JoinPolicy == joinPolicy.Value);

        return await query.Include(g => g.Members).Take(take).ToListAsync();
    }

    public async Task<List<Guild>> GetTopGuildsAsync(int top)
        => await _ctx.Guilds
            .Where(g => g.IsActive)
            .Include(g => g.Members)
            .OrderByDescending(g => g.TotalFeats)
            .ThenByDescending(g => g.GuildExp)
            .Take(top)
            .ToListAsync();

    public async Task AddGuildAsync(Guild guild)
    {
        _ctx.Guilds.Add(guild);
        await _ctx.SaveChangesAsync();
    }

    public async Task UpdateGuildAsync(Guild guild)
    {
        _ctx.Guilds.Update(guild);
        await _ctx.SaveChangesAsync();
    }

    // ─── Guild Member ──────────────────────────────────────────────────────────

    public async Task<GuildMember?> GetMemberAsync(int guildId, int playerProfileId, bool includeProfile = false)
    {
        var query = _ctx.GuildMembers.AsQueryable();
        if (includeProfile)
            query = query.Include(m => m.PlayerProfile);
        return await query.FirstOrDefaultAsync(m => m.GuildId == guildId && m.PlayerProfileId == playerProfileId && m.LeftAt == null);
    }

    public async Task<List<GuildMember>> GetActiveMembersAsync(int guildId,
        bool includeProfile = false)
    {
        var query = _ctx.GuildMembers
            .Where(m => m.GuildId == guildId && m.LeftAt == null)
            .AsQueryable();
        if (includeProfile)
            query = query.Include(m => m.PlayerProfile);
        return await query.ToListAsync();
    }

    public async Task<bool> IsGuildMemberAsync(int guildId, int playerProfileId)
        => await _ctx.GuildMembers
            .AnyAsync(m => m.GuildId == guildId && m.PlayerProfileId == playerProfileId && m.LeftAt == null);

    public async Task AddMemberAsync(GuildMember member)
    {
        _ctx.GuildMembers.Add(member);
        await _ctx.SaveChangesAsync();
    }

    public async Task AddMembersAsync(IEnumerable<GuildMember> members)
    {
        _ctx.GuildMembers.AddRange(members);
        await _ctx.SaveChangesAsync();
    }

    public async Task RemoveMemberAsync(GuildMember member)
    {
        _ctx.GuildMembers.Remove(member);
        await _ctx.SaveChangesAsync();
    }

    public async Task RemoveMembersAsync(IEnumerable<GuildMember> members)
    {
        _ctx.GuildMembers.RemoveRange(members);
        await _ctx.SaveChangesAsync();
    }

    public async Task UpdateMemberAsync(GuildMember member)
    {
        _ctx.GuildMembers.Update(member);
        await _ctx.SaveChangesAsync();
    }

    // ─── Player Profile ───────────────────────────────────────────────────────

    public async Task<PlayerProfile?> GetPlayerProfileAsync(int playerProfileId,
        bool includeGuildMember = false,
        bool includeAccount = false)
    {
        var query = _ctx.PlayerProfiles.AsQueryable();
        if (includeGuildMember)
            query = query.Include(p => p.GuildMember);
        if (includeAccount)
            query = query.Include(p => p.Account);
        return await query.FirstOrDefaultAsync(p => p.PlayerProfileId == playerProfileId);
    }

    public async Task<List<PlayerProfile>> GetPlayerProfilesByIdsAsync(IEnumerable<int> profileIds)
    {
        var ids = profileIds.ToList();
        return ids.Count == 0
            ? []
            : await _ctx.PlayerProfiles
                .Where(p => ids.Contains(p.PlayerProfileId))
                .ToListAsync();
    }

    public async Task UpdatePlayerProfileAsync(PlayerProfile profile)
    {
        _ctx.PlayerProfiles.Update(profile);
        await _ctx.SaveChangesAsync();
    }

    // ─── Guild Application ────────────────────────────────────────────────────

    public async Task<GuildApplication?> GetApplicationAsync(int applicationId, int guildId)
        => await _ctx.GuildApplications
            .Include(a => a.PlayerProfile)
            .FirstOrDefaultAsync(a => a.GuildApplicationId == applicationId && a.GuildId == guildId && a.Status == "Pending");

    public async Task<GuildApplication?> GetPlayerPendingApplicationAsync(int playerProfileId)
        => await _ctx.GuildApplications
            .FirstOrDefaultAsync(a => a.PlayerProfileId == playerProfileId && a.Status == "Pending");

    public async Task<List<GuildApplication>> GetPendingApplicationsAsync(int guildId)
        => await _ctx.GuildApplications
            .Include(a => a.PlayerProfile)
            .Where(a => a.GuildId == guildId && a.Status == "Pending")
            .ToListAsync();

    public async Task AddApplicationAsync(GuildApplication application)
    {
        _ctx.GuildApplications.Add(application);
        await _ctx.SaveChangesAsync();
    }

    public async Task RemoveApplicationsAsync(IEnumerable<GuildApplication> applications)
    {
        _ctx.GuildApplications.RemoveRange(applications);
        await _ctx.SaveChangesAsync();
    }

    public async Task UpdateApplicationAsync(GuildApplication application)
    {
        _ctx.GuildApplications.Update(application);
        await _ctx.SaveChangesAsync();
    }

    // ─── Guild Invitation ────────────────────────────────────────────────────

    public async Task<GuildInvitation?> GetActiveInvitationAsync(int guildId, int inviteeId, DateTime now)
        => await _ctx.GuildInvitations
            .FirstOrDefaultAsync(i =>
                i.GuildId == guildId &&
                i.InviteeId == inviteeId &&
                i.Status == "Pending" &&
                i.ExpiresAt > now);

    public async Task<List<GuildInvitation>> GetActiveInvitationsForPlayerAsync(int inviteeId, DateTime now)
        => await _ctx.GuildInvitations
            .Where(i => i.InviteeId == inviteeId && i.Status == "Pending" && i.ExpiresAt > now)
            .ToListAsync();

    public async Task<List<GuildInvitation>> GetExpiredInvitationsAsync(int inviteeId, DateTime now)
        => await _ctx.GuildInvitations
            .Where(i => i.InviteeId == inviteeId && i.Status == "Pending" && i.ExpiresAt < now)
            .ToListAsync();

    public async Task<List<GuildInvitation>> GetOtherPendingInvitationsAsync(int inviteeId, int excludeInvitationId)
        => await _ctx.GuildInvitations
            .Where(i => i.InviteeId == inviteeId && i.Status == "Pending" && i.GuildInvitationId != excludeInvitationId)
            .ToListAsync();

    public async Task AddInvitationAsync(GuildInvitation invitation)
    {
        _ctx.GuildInvitations.Add(invitation);
        await _ctx.SaveChangesAsync();
    }

    public async Task UpdateInvitationAsync(GuildInvitation invitation)
    {
        _ctx.GuildInvitations.Update(invitation);
        await _ctx.SaveChangesAsync();
    }

    // ─── Guild Log ───────────────────────────────────────────────────────────

    public async Task AddLogAsync(GuildLog log)
    {
        _ctx.GuildLogs.Add(log);
        await _ctx.SaveChangesAsync();
    }

    public async Task<List<GuildLog>> GetRecentLogsAsync(int guildId, int take)
        => await _ctx.GuildLogs
            .Where(l => l.GuildId == guildId)
            .OrderByDescending(l => l.CreatedAt)
            .Take(take)
            .ToListAsync();

    // ─── Guild Chat ───────────────────────────────────────────────────────────

    public async Task AddChatMessageAsync(GuildChatMessage message)
    {
        _ctx.GuildChatMessages.Add(message);
        await _ctx.SaveChangesAsync();
    }

    public async Task<List<GuildChatMessage>> GetRecentChatAsync(int guildId, int take)
        => await _ctx.GuildChatMessages
            .Include(m => m.Sender)
            .Where(m => m.GuildId == guildId)
            .OrderByDescending(m => m.SentAt)
            .Take(take)
            .ToListAsync();

    // ─── Persistence ──────────────────────────────────────────────────────────

    public async Task SaveChangesAsync()
        => await _ctx.SaveChangesAsync();
}
