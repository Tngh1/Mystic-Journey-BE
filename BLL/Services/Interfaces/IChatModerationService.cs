using BLL.DTOs;
using DAL.Models;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Initializes a new default instance of the IChatModerationService class.
    public interface IChatModerationService
    {
        Task EnsureCanSendChat(int playerProfileId);
        Task<ChatModerationResultDto> ReviewReportedWorldMessage(int reporterId, WorldChatMessage message, string? reason);
        Task<ChatModerationResultDto> ReviewReportedMessage(int reporterId, ChatMessage message, string? reason);
        Task<ChatModerationResultDto> ReviewReportedPartyMessage(int reporterId, int reportedPlayerId, string content, string? reason);
    }
}
