using BLL.DTOs;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IMailService
    {
        Task<MailResponseDto?> GetMailById(int id);
        Task<List<MailResponseDto>> GetMailsByPlayerId(int playerProfileId);
        Task<MailResponseDto> SendMail(SendMailRequestDto request);
        Task SendBulkMail(BulkSendMailRequestDto request);
        Task<MailResponseDto> MarkMailAsRead(int mailId);
        Task<MailResponseDto> ClaimMailReward(int mailId);
        Task<PagedResultDto<MailResponseDto>> GetMailsPaged(int page, int pageSize, string? search, bool? isRead, bool? isClaimed);
    }
}
