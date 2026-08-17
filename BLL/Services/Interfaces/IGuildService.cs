using BLL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Initializes a new default instance of the IGuildService class.
    public interface IGuildService
    {
        Task<GuildDetailResponseDto?> GetMyGuildAsync(int playerProfileId);
        Task<List<GuildResponseDto>> GetGuildListAsync(int playerProfileId, string searchTerm = "", int? joinPolicy = null, int? minLevel = null);
        Task<List<GuildRankResponseDto>> GetGuildRankingsAsync(int top = 100);
        Task<GuildDetailResponseDto?> GetGuildDetailAsync(int guildId);
        Task<List<GuildMemberResponseDto>> GetMembersAsync(int playerProfileId, int guildId);

        Task<GuildResponseDto> CreateGuildAsync(int playerProfileId, CreateGuildRequestDto request);
        Task<bool> DissolveGuildAsync(int playerProfileId, int guildId);

        Task<GuildJoinResultDto> ApplyToGuildAsync(int playerProfileId, int guildId);
        Task<GuildJoinResultDto> LeaveGuildAsync(int playerProfileId, int guildId);

        Task<List<GuildApplicationDTO>> GetApplicationsAsync(int playerProfileId, int guildId);
        Task<bool> ApproveApplicationAsync(int playerProfileId, int guildId, int applicationId);
        Task<bool> RejectApplicationAsync(int playerProfileId, int guildId, int applicationId);

        Task<bool> KickMemberAsync(int playerProfileId, int guildId, int memberProfileId);
        Task<bool> PromoteMemberAsync(int playerProfileId, int guildId, int memberProfileId);
        Task<bool> DemoteMemberAsync(int playerProfileId, int guildId, int memberProfileId);
        Task<bool> TransferLeaderAsync(int playerProfileId, int guildId, int newLeaderProfileId);
        Task<bool> InviteMemberAsync(int playerProfileId, int guildId, int inviteeProfileId);

        Task<bool> UpdateSettingsAsync(int playerProfileId, int guildId, UpdateGuildRequestDto request);
        Task<bool> UpdateNoticeAsync(int playerProfileId, int guildId, string notice);
        Task<bool> UpdateIconAsync(int playerProfileId, int guildId, int iconId, int? bannerId);

        Task<GuildDonateResultDto> DonateAsync(int playerProfileId, int guildId, string currencyType, int amount);
        Task<bool> LevelUpAsync(int playerProfileId, int guildId);

        Task<List<GuildLogDto>> GetLogsAsync(int playerProfileId, int guildId);

        Task<List<GuildMessageDTO>> GetGuildChatAsync(int playerProfileId, int guildId);
        Task<GuildMessageDTO> SendGuildMessageAsync(int playerProfileId, int guildId, string content);
    }
}
