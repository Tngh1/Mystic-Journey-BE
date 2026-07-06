using BLL.DTOs;
using DAL.Models;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IChatModerationService
    {
        Task EnsureCanSendChat(int playerProfileId);
        Task<ChatModerationResultDto> ReviewReportedWorldMessage(int reporterId, WorldChatMessage message, string? reason);
        Task<ChatModerationResultDto> ReviewReportedMessage(int reporterId, ChatMessage message, string? reason);
    }
}