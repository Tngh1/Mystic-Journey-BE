using BLL.DTOs;
using System;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IMailService
    {
        Task<MailListResponseDto> GetMailsAsync(Guid accountId, int pageNumber = 1, int pageSize = 20);
        Task<MailListResponseDto> GetUnreadMailsAsync(Guid accountId);
        Task<MailApiResponseDto> GetMailByIdAsync(Guid accountId, Guid mailId);
        Task<MailApiResponseDto> MarkAsReadAsync(Guid accountId, Guid mailId);
        Task<MailApiResponseDto> ClaimMailAsync(Guid accountId, Guid mailId);
        Task<MailApiResponseDto> SendMailAsync(Guid accountId, SendMailRequestDto request);
        Task<MailApiResponseDto> DeleteMailAsync(Guid accountId, Guid mailId);
        Task<int> GetUnreadCountAsync(Guid accountId);
    }
}
