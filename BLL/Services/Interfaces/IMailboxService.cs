using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Initializes a new default instance of the IMailboxService class.
    public interface IMailboxService
    {

        Task<MailboxListPagedDto> GetMyMailboxes(int playerProfileId, int page, int pageSize);

        Task<MailboxDetailDto?> GetMailboxById(int mailboxId);

        Task<MailboxDetailDto> MarkMailboxAsRead(int mailboxId);

        Task<MailboxDetailDto> ClaimMailboxReward(int mailboxId);

        Task DeleteMailbox(int mailboxId, int playerProfileId);


        Task<PagedResultDto<MailboxDetailDto>> GetMailboxesPaged(int page, int pageSize, string? search, bool? isRead, bool? isClaimed, string? sortBy = null, string? sortOrder = null);

        Task<List<MailboxDetailDto>> SendMailboxByListId(SendMailboxByListIdDto request);

        Task<List<MailboxDetailDto>> SendMailboxToAll(SendMailboxToAllDto request);
    }
}
