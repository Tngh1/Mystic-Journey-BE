using DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Initializes a new default instance of the IMailboxRepository class.
    public interface IMailboxRepository
    {

        Task<Mailbox?> GetMailboxById(int id);

        Task<List<Mailbox>> GetMailboxesByPlayerId(int playerProfileId);

        Task<List<Mailbox>> GetUnreadMailboxesByPlayerId(int playerProfileId);

        Task<(int TotalCount, List<Mailbox> Items)> GetMailboxesByPlayerIdPaged(int playerProfileId, int page, int pageSize);


        Task<Mailbox> CreateMailbox(Mailbox mailbox);

        Task<List<Mailbox>> CreateBulkMailboxes(List<Mailbox> mailboxes);

        Task<Mailbox> UpdateMailbox(Mailbox mailbox);

        Task<Mailbox> SoftDeleteMailbox(int mailboxId);

        Task<(int TotalCount, List<Mailbox> Items)> GetMailboxesPaged(int page, int pageSize, string? search, bool? isRead, bool? isClaimed, string? sortBy = null, string? sortOrder = null);
    }
}
