using BLL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IGuildService
    {
        // View
        Task<GuildDetailResponseDto?> GetMyGuildAsync(int playerProfileId);
        Task<List<GuildResponseDto>> GetGuildListAsync(string searchTerm = "", int? joinPolicy = null, int? minLevel = null);
        Task<GuildDetailResponseDto?> GetGuildDetailAsync(int guildId);
        Task<List<GuildMemberResponseDto>> GetMembersAsync(int playerProfileId, int guildId);

        // Create / Dissolve
        Task<GuildResponseDto> CreateGuildAsync(int playerProfileId, CreateGuildRequestDto request);
        Task<bool> DissolveGuildAsync(int playerProfileId, int guildId);

        // Join / Leave (returns rich result with cooldown info)
        Task<GuildJoinResultDto> ApplyToGuildAsync(int playerProfileId, int guildId);
        Task<GuildJoinResultDto> LeaveGuildAsync(int playerProfileId, int guildId);

        // Applications
        Task<List<GuildApplicationDTO>> GetApplicationsAsync(int playerProfileId, int guildId);
        Task<bool> ApproveApplicationAsync(int playerProfileId, int guildId, int applicationId);
        Task<bool> RejectApplicationAsync(int playerProfileId, int guildId, int applicationId);

        // Member management
        Task<bool> KickMemberAsync(int playerProfileId, int guildId, int memberProfileId);
        Task<bool> PromoteMemberAsync(int playerProfileId, int guildId, int memberProfileId);
        Task<bool> DemoteMemberAsync(int playerProfileId, int guildId, int memberProfileId);
        Task<bool> TransferLeaderAsync(int playerProfileId, int guildId, int newLeaderProfileId);
        Task<bool> InviteMemberAsync(int playerProfileId, int guildId, int inviteeProfileId);

        // Settings
        Task<bool> UpdateNoticeAsync(int playerProfileId, int guildId, string notice);
        Task<bool> UpdateIconAsync(int playerProfileId, int guildId, int iconId, int? bannerId);

        // Donate (Guild EXP + Medal, level up requires both)
        Task<GuildDonateResultDto> DonateAsync(int playerProfileId, int guildId, int amount);
        Task<bool> LevelUpAsync(int playerProfileId, int guildId);

        // Logs
        Task<List<GuildLogDto>> GetLogsAsync(int playerProfileId, int guildId);

        // Chat (with spam control)
        Task<List<GuildMessageDTO>> GetGuildChatAsync(int playerProfileId, int guildId);
        Task<GuildMessageDTO> SendGuildMessageAsync(int playerProfileId, int guildId, string content);
    }
}
