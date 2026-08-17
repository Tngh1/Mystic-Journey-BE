using DAL.Models;

namespace DAL.Repositories.Interfaces;

// Initializes a new default instance of the IGuildRepository class.
public interface IGuildRepository
{

    Task<Guild?> GetGuildByIdAsync(int guildId,
        bool includeMembers = false,
        bool includeLeader = false,
        bool includeMemberProfiles = false);

    Task<Guild?> GetActiveGuildByLeaderAsync(int leaderProfileId);

    Task<List<Guild>> GetGuildsByIdsAsync(IEnumerable<int> guildIds);

    Task<List<Guild>> GetActiveGuildsAsync(
        IEnumerable<int> excludeIds,
        string? searchTerm,
        int? joinPolicy,
        int? minLevel,
        int take);

    Task<List<Guild>> GetTopGuildsAsync(int top);

    Task AddGuildAsync(Guild guild);

    Task UpdateGuildAsync(Guild guild);


    Task<GuildMember?> GetMemberAsync(int guildId, int playerProfileId, bool includeProfile = false);

    Task<List<GuildMember>> GetActiveMembersAsync(int guildId,
        bool includeProfile = false);

    Task<bool> IsGuildMemberAsync(int guildId, int playerProfileId);

    Task AddMemberAsync(GuildMember member);

    Task AddMembersAsync(IEnumerable<GuildMember> members);

    Task RemoveMemberAsync(GuildMember member);

    Task RemoveMembersAsync(IEnumerable<GuildMember> members);

    Task UpdateMemberAsync(GuildMember member);


    Task<PlayerProfile?> GetPlayerProfileAsync(int playerProfileId,
        bool includeGuildMember = false,
        bool includeAccount = false);

    Task<List<PlayerProfile>> GetPlayerProfilesByIdsAsync(IEnumerable<int> profileIds);

    Task UpdatePlayerProfileAsync(PlayerProfile profile);


    Task<GuildApplication?> GetApplicationAsync(int applicationId, int guildId);

    Task<GuildApplication?> GetPlayerPendingApplicationAsync(int playerProfileId);

    Task<List<GuildApplication>> GetPendingApplicationsAsync(int guildId);

    Task AddApplicationAsync(GuildApplication application);

    Task UpdateApplicationAsync(GuildApplication application);

    Task RemoveApplicationsAsync(IEnumerable<GuildApplication> applications);


    Task<GuildInvitation?> GetActiveInvitationAsync(int guildId, int inviteeId, DateTime now);

    Task<List<GuildInvitation>> GetActiveInvitationsForPlayerAsync(int inviteeId, DateTime now);

    Task<List<GuildInvitation>> GetExpiredInvitationsAsync(int inviteeId, DateTime now);

    Task<List<GuildInvitation>> GetOtherPendingInvitationsAsync(int inviteeId, int excludeInvitationId);

    Task AddInvitationAsync(GuildInvitation invitation);

    Task UpdateInvitationAsync(GuildInvitation invitation);


    Task AddLogAsync(GuildLog log);

    Task<List<GuildLog>> GetRecentLogsAsync(int guildId, int take);


    Task AddChatMessageAsync(GuildChatMessage message);

    Task<List<GuildChatMessage>> GetRecentChatAsync(int guildId, int take);


    Task SaveChangesAsync();
}
