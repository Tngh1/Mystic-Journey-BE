using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IMailService
    {
        Task<MailResponseDto?> GetMailById(int id);
        Task<List<MailResponseDto>> GetMailsByPlayerId(int playerProfileId);
        Task<PagedResultDto<MailResponseDto>> GetMailsByPlayerIdPaged(int playerProfileId, int page, int pageSize);
        Task SendMailByListId(SendMailByListIdDto request);
        Task SendMailToAll(SendMailToAllDto request);
        Task<MailResponseDto> MarkMailAsRead(int mailId);
        Task<MailResponseDto> ClaimMailReward(int mailId);
        Task<MailResponseDto> DeleteMail(int mailId, int playerProfileId);
        Task<PagedResultDto<MailResponseDto>> GetMailsPaged(int page, int pageSize, string? search, bool? isRead, bool? isClaimed);
    }
}
