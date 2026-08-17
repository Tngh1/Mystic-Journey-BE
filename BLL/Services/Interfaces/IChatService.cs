using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Initializes a new default instance of the IChatService class.
    public interface IChatService
    {
        Task<PagedResultDto<WorldChatMessageResponseDto>> GetWorldMessages(
            int playerProfileId,
            WorldChatMessageListQueryDto query);

        Task<WorldChatMessageResponseDto> SendWorldMessage(int senderId, SendWorldChatMessageRequestDto request);
        Task<ReportWorldChatMessageResponseDto> ReportWorldMessage(int reporterId, ReportChatMessageRequestDto request);

        Task<PagedResultDto<ChatMessageResponseDto>> GetMessages(
            int playerProfileId,
            ChatMessageListQueryDto query);

        Task<ChatMessageResponseDto> SendMessage(int senderId, SendChatMessageRequestDto request);
        Task<ReportChatMessageResponseDto> ReportMessage(int reporterId, ReportChatMessageRequestDto request);
    }
}
