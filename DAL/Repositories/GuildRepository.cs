using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

// Queries the database to retrieve i guild repository records.
// Query details: eagerly loads related entity navigation properties.
public class GuildRepository : IGuildRepository
{
    private readonly MysticJourneyDbContext _ctx;

    // Initializes a new instance of GuildRepository with dependencies: ctx.
    // Assigns injected service and configuration instances to readonly fields for runtime operations.
    public GuildRepository(MysticJourneyDbContext ctx) => _ctx = ctx;


    // Load guild by id async using guild id, include members, include leader, and include member profiles; it filters the eligible records and selects the matching record and guards invalid or unavailable states.
    public async Task<Guild?> GetGuildByIdAsync(int guildId,
        bool includeMembers = false,
        bool includeLeader = false,
        bool includeMemberProfiles = false)
    {
        var query = _ctx.Guilds.AsQueryable();

        if (includeLeader)
            query = query.Include(g => g.Leader);  // Eagerly load related navigation entities to avoid N+1 queries

        if (includeMembers || includeMemberProfiles)
        {
            query = query.Include(g => g.Members.Where(m => m.LeftAt == null));  // Eagerly load related navigation entities to avoid N+1 queries

            if (includeMemberProfiles)
                query = query
                    .Include(g => g.Members.Where(m => m.LeftAt == null))  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(m => m.PlayerProfile);
        }

        return await query.FirstOrDefaultAsync(g => g.GuildId == guildId && g.IsActive);  // Fetch single matching record or null if not found
    }

    // Load active guild by leader async; it selects the matching record.
    public async Task<Guild?> GetActiveGuildByLeaderAsync(int leaderProfileId)
        => await _ctx.Guilds.FirstOrDefaultAsync(g => g.LeaderId == leaderProfileId && g.IsActive);  // Fetch single matching record or null if not found

    // Queries the database to retrieve get guilds by ids async records.
    // Query details: eagerly loads related entity navigation properties.
    // Returns the matching List<Guild entity result or default if not found.
    public async Task<List<Guild>> GetGuildsByIdsAsync(IEnumerable<int> guildIds)
    {
        var ids = guildIds.ToList();
        return ids.Count == 0
            ? []
            : await _ctx.Guilds
                .Include(g => g.Members)  // Eagerly load related navigation entities to avoid N+1 queries
                .Where(g => g.IsActive && ids.Contains(g.GuildId))  // Filter records matching the predicate
                .ToListAsync();  // Materialize the query into a list from the database
    }

    // Process the supplied values: normalizes or validates the text before returning the derived result.
    public async Task<List<Guild>> GetActiveGuildsAsync(
        IEnumerable<int> excludeIds,
        string? searchTerm,
        int? joinPolicy,
        int? minLevel,
        int take)
    {
        var exclude = excludeIds.ToList();
        var query = _ctx.Guilds
            .Where(g => g.IsActive && !exclude.Contains(g.GuildId))  // Filter records matching the predicate
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(g => g.Name.Contains(searchTerm));  // Filter records matching the predicate
        if (minLevel.HasValue)
            query = query.Where(g => g.RequiredLevel <= minLevel.Value);  // Filter records matching the predicate
        if (joinPolicy.HasValue)
            query = query.Where(g => (int)g.JoinPolicy == joinPolicy.Value);  // Filter records matching the predicate

        return await query.Include(g => g.Members).Take(take).ToListAsync();  // Eagerly load related navigation entities to avoid N+1 queries
    }

    // Load top guilds async; it filters the eligible records, orders the resulting records, materializes the query results, creates guild async, and creates add.
    public async Task<List<Guild>> GetTopGuildsAsync(int top)
        => await _ctx.Guilds
            .Where(g => g.IsActive)  // Filter records matching the predicate
            .Include(g => g.Members)  // Eagerly load related navigation entities to avoid N+1 queries
            .OrderByDescending(g => g.TotalFeats)  // Sort results newest/highest first
            .ThenByDescending(g => g.GuildExp)
            .Take(top)  // Apply pagination limit — cap result set size
            .ToListAsync();  // Materialize the query into a list from the database

    // Persists state modifications to the database for add guild async.
    // Query details: commits entity state changes via EF Core SaveChangesAsync.
    public async Task AddGuildAsync(Guild guild)
    {
        _ctx.Guilds.Add(guild);
        await _ctx.SaveChangesAsync();
    }

    // Performs database query and transactional persistence workflow for update guild async.
    // Query details: eagerly loads related entity navigation properties; commits entity state changes via EF Core SaveChangesAsync.
    public async Task UpdateGuildAsync(Guild guild)
    {
        _ctx.Guilds.Update(guild);
        await _ctx.SaveChangesAsync();
    }


    // Queries the database to retrieve get member async records.
    // Query details: eagerly loads related entity navigation properties.
    // Returns the matching GuildMember? entity result or default if not found.
    public async Task<GuildMember?> GetMemberAsync(int guildId, int playerProfileId, bool includeProfile = false)
    {
        var query = _ctx.GuildMembers.AsQueryable();
        if (includeProfile)
            query = query.Include(m => m.PlayerProfile);  // Eagerly load related navigation entities to avoid N+1 queries
        return await query.FirstOrDefaultAsync(m => m.GuildId == guildId && m.PlayerProfileId == playerProfileId && m.LeftAt == null);  // Fetch single matching record or null if not found
    }

    // Load active members async using guild id and include profile; it filters the eligible records and materializes the query results and guards invalid or unavailable states.
    public async Task<List<GuildMember>> GetActiveMembersAsync(int guildId,
        bool includeProfile = false)
    {
        var query = _ctx.GuildMembers
            .Where(m => m.GuildId == guildId && m.LeftAt == null)  // Filter records matching the predicate
            .AsQueryable();
        if (includeProfile)
            query = query.Include(m => m.PlayerProfile);  // Eagerly load related navigation entities to avoid N+1 queries
        return await query.ToListAsync();  // Materialize the query into a list from the database
    }

    // Performs database query and transactional persistence workflow for is guild member async.
    // Query details: commits entity state changes via EF Core SaveChangesAsync.
    // Returns true if the operation succeeded or record exists; otherwise false.
    public async Task<bool> IsGuildMemberAsync(int guildId, int playerProfileId)
        => await _ctx.GuildMembers
            .AnyAsync(m => m.GuildId == guildId && m.PlayerProfileId == playerProfileId && m.LeftAt == null);  // Check existence without loading the full entity

    // Persists state modifications to the database for add member async.
    // Query details: commits entity state changes via EF Core SaveChangesAsync.
    public async Task AddMemberAsync(GuildMember member)
    {
        _ctx.GuildMembers.Add(member);
        await _ctx.SaveChangesAsync();
    }

    // Persists state modifications to the database for add members async.
    // Query details: commits entity state changes via EF Core SaveChangesAsync.
    public async Task AddMembersAsync(IEnumerable<GuildMember> members)
    {
        _ctx.GuildMembers.AddRange(members);
        await _ctx.SaveChangesAsync();
    }

    // Persists state modifications to the database for remove member async.
    // Query details: commits entity state changes via EF Core SaveChangesAsync.
    public async Task RemoveMemberAsync(GuildMember member)
    {
        _ctx.GuildMembers.Remove(member);  // Mark entity for deletion in the next SaveChanges call
        await _ctx.SaveChangesAsync();
    }

    // Persists state modifications to the database for remove members async.
    // Query details: commits entity state changes via EF Core SaveChangesAsync.
    public async Task RemoveMembersAsync(IEnumerable<GuildMember> members)
    {
        _ctx.GuildMembers.RemoveRange(members);
        await _ctx.SaveChangesAsync();
    }

    // Performs database query and transactional persistence workflow for update member async.
    // Query details: eagerly loads related entity navigation properties; commits entity state changes via EF Core SaveChangesAsync.
    public async Task UpdateMemberAsync(GuildMember member)
    {
        _ctx.GuildMembers.Update(member);
        await _ctx.SaveChangesAsync();
    }


    // Load player profile async using player profile id, include guild member, and include account; it selects the matching record and guards invalid or unavailable states.
    public async Task<PlayerProfile?> GetPlayerProfileAsync(int playerProfileId,
        bool includeGuildMember = false,
        bool includeAccount = false)
    {
        var query = _ctx.PlayerProfiles.AsQueryable();
        if (includeGuildMember)
            query = query.Include(p => p.GuildMember);  // Eagerly load related navigation entities to avoid N+1 queries
        if (includeAccount)
            query = query.Include(p => p.Account);  // Eagerly load related navigation entities to avoid N+1 queries
        return await query.FirstOrDefaultAsync(p => p.PlayerProfileId == playerProfileId);  // Fetch single matching record or null if not found
    }

    // Performs database query and transactional persistence workflow for get player profiles by ids async.
    // Query details: commits entity state changes via EF Core SaveChangesAsync.
    // Returns the matching List<PlayerProfile entity result or default if not found.
    public async Task<List<PlayerProfile>> GetPlayerProfilesByIdsAsync(IEnumerable<int> profileIds)
    {
        var ids = profileIds.ToList();
        return ids.Count == 0
            ? []
            : await _ctx.PlayerProfiles
                .Where(p => ids.Contains(p.PlayerProfileId))  // Filter records matching the predicate
                .ToListAsync();  // Materialize the query into a list from the database
    }

    // Performs database query and transactional persistence workflow for update player profile async.
    // Query details: eagerly loads related entity navigation properties; commits entity state changes via EF Core SaveChangesAsync.
    public async Task UpdatePlayerProfileAsync(PlayerProfile profile)
    {
        _ctx.PlayerProfiles.Update(profile);
        await _ctx.SaveChangesAsync();
    }


    // Performs database query and transactional persistence workflow for get application async.
    // Query details: eagerly loads related entity navigation properties; commits entity state changes via EF Core SaveChangesAsync.
    // Returns the matching GuildApplication? entity result or default if not found.
    public async Task<GuildApplication?> GetApplicationAsync(int applicationId, int guildId)
        => await _ctx.GuildApplications
            .Include(a => a.PlayerProfile)  // Eagerly load related navigation entities to avoid N+1 queries
            .FirstOrDefaultAsync(a => a.GuildApplicationId == applicationId && a.GuildId == guildId && a.Status == "Pending");  // Fetch single matching record or null if not found

    // Load player pending application async; it selects the matching record.
    public async Task<GuildApplication?> GetPlayerPendingApplicationAsync(int playerProfileId)
        => await _ctx.GuildApplications
            .FirstOrDefaultAsync(a => a.PlayerProfileId == playerProfileId && a.Status == "Pending");  // Fetch single matching record or null if not found

    // Load pending applications async; it filters the eligible records, materializes the query results, creates application async, creates add, and updates changes async.
    public async Task<List<GuildApplication>> GetPendingApplicationsAsync(int guildId)
        => await _ctx.GuildApplications
            .Include(a => a.PlayerProfile)  // Eagerly load related navigation entities to avoid N+1 queries
            .Where(a => a.GuildId == guildId && a.Status == "Pending")  // Filter records matching the predicate
            .ToListAsync();  // Materialize the query into a list from the database

    // Persists state modifications to the database for add application async.
    // Query details: commits entity state changes via EF Core SaveChangesAsync.
    public async Task AddApplicationAsync(GuildApplication application)
    {
        _ctx.GuildApplications.Add(application);
        await _ctx.SaveChangesAsync();
    }

    // Persists state modifications to the database for remove applications async.
    // Query details: commits entity state changes via EF Core SaveChangesAsync.
    public async Task RemoveApplicationsAsync(IEnumerable<GuildApplication> applications)
    {
        _ctx.GuildApplications.RemoveRange(applications);
        await _ctx.SaveChangesAsync();
    }

    // Performs database query and transactional persistence workflow for update application async.
    // Query details: commits entity state changes via EF Core SaveChangesAsync.
    public async Task UpdateApplicationAsync(GuildApplication application)
    {
        _ctx.GuildApplications.Update(application);
        await _ctx.SaveChangesAsync();
    }


    // Performs database query and transactional persistence workflow for get active invitation async.
    // Query details: commits entity state changes via EF Core SaveChangesAsync.
    // Returns the matching GuildInvitation? entity result or default if not found.
    public async Task<GuildInvitation?> GetActiveInvitationAsync(int guildId, int inviteeId, DateTime now)
        => await _ctx.GuildInvitations
            .FirstOrDefaultAsync(i =>  // Fetch single matching record or null if not found
                i.GuildId == guildId &&
                i.InviteeId == inviteeId &&
                i.Status == "Pending" &&
                i.ExpiresAt > now);

    // Performs database query and transactional persistence workflow for get active invitations for player async.
    // Query details: commits entity state changes via EF Core SaveChangesAsync.
    // Returns the matching List<GuildInvitation entity result or default if not found.
    public async Task<List<GuildInvitation>> GetActiveInvitationsForPlayerAsync(int inviteeId, DateTime now)
        => await _ctx.GuildInvitations
            .Where(i => i.InviteeId == inviteeId && i.Status == "Pending" && i.ExpiresAt > now)  // Filter records matching the predicate
            .ToListAsync();  // Materialize the query into a list from the database

    // Performs database query and transactional persistence workflow for get expired invitations async.
    // Query details: commits entity state changes via EF Core SaveChangesAsync.
    // Returns the matching List<GuildInvitation entity result or default if not found.
    public async Task<List<GuildInvitation>> GetExpiredInvitationsAsync(int inviteeId, DateTime now)
        => await _ctx.GuildInvitations
            .Where(i => i.InviteeId == inviteeId && i.Status == "Pending" && i.ExpiresAt < now)  // Filter records matching the predicate
            .ToListAsync();  // Materialize the query into a list from the database

    // Performs database query and transactional persistence workflow for get other pending invitations async.
    // Query details: commits entity state changes via EF Core SaveChangesAsync.
    // Returns the matching List<GuildInvitation entity result or default if not found.
    public async Task<List<GuildInvitation>> GetOtherPendingInvitationsAsync(int inviteeId, int excludeInvitationId)
        => await _ctx.GuildInvitations
            .Where(i => i.InviteeId == inviteeId && i.Status == "Pending" && i.GuildInvitationId != excludeInvitationId)  // Filter records matching the predicate
            .ToListAsync();  // Materialize the query into a list from the database

    // Persists state modifications to the database for add invitation async.
    // Query details: commits entity state changes via EF Core SaveChangesAsync.
    public async Task AddInvitationAsync(GuildInvitation invitation)
    {
        _ctx.GuildInvitations.Add(invitation);
        await _ctx.SaveChangesAsync();
    }

    // Persists state modifications to the database for update invitation async.
    // Query details: commits entity state changes via EF Core SaveChangesAsync.
    public async Task UpdateInvitationAsync(GuildInvitation invitation)
    {
        _ctx.GuildInvitations.Update(invitation);
        await _ctx.SaveChangesAsync();
    }


    // Performs database query and transactional persistence workflow for add log async.
    // Query details: commits entity state changes via EF Core SaveChangesAsync; sorts records according to business ordering rules.
    public async Task AddLogAsync(GuildLog log)
    {
        _ctx.GuildLogs.Add(log);
        await _ctx.SaveChangesAsync();
    }

    // Performs database query and transactional persistence workflow for get recent logs async.
    // Query details: commits entity state changes via EF Core SaveChangesAsync; sorts records according to business ordering rules.
    // Returns the matching List<GuildLog entity result or default if not found.
    public async Task<List<GuildLog>> GetRecentLogsAsync(int guildId, int take)
        => await _ctx.GuildLogs
            .Where(l => l.GuildId == guildId)  // Filter records matching the predicate
            .OrderByDescending(l => l.CreatedAt)  // Sort results newest/highest first
            .Take(take)  // Apply pagination limit — cap result set size
            .ToListAsync();  // Materialize the query into a list from the database


    // Queries the database to retrieve add chat message async records.
    public async Task AddChatMessageAsync(GuildChatMessage message)
    {
        _ctx.GuildChatMessages.Add(message);
        await _ctx.SaveChangesAsync();
    }

    // Queries the database to retrieve get recent chat async records.
    // Query details: eagerly loads related entity navigation properties; sorts records according to business ordering rules.
    // Returns the matching List<GuildChatMessage entity result or default if not found.
    public async Task<List<GuildChatMessage>> GetRecentChatAsync(int guildId, int take)
        => await _ctx.GuildChatMessages
            .Include(m => m.Sender)  // Eagerly load related navigation entities to avoid N+1 queries
            .Where(m => m.GuildId == guildId)  // Filter records matching the predicate
            .OrderByDescending(m => m.SentAt)  // Sort results newest/highest first
            .Take(take)  // Apply pagination limit — cap result set size
            .ToListAsync();  // Materialize the query into a list from the database


    // Persists state modifications to the database for save changes async.
    // Query details: commits entity state changes via EF Core SaveChangesAsync.
    public async Task SaveChangesAsync()
        => await _ctx.SaveChangesAsync();
}
