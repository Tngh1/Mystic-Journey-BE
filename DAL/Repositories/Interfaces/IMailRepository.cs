using DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IMailRepository
    {
        Task<Mail?> GetMailById(int id);
        Task<List<Mail>> GetMailsByPlayerId(int playerProfileId);
        Task<List<Mail>> GetUnreadMailsByPlayerId(int playerProfileId);
        Task<Mail> CreateMail(Mail mail);
        Task<List<Mail>> CreateBulkMails(List<Mail> mails);
        Task<Mail> UpdateMail(Mail mail);
        Task<(int TotalCount, List<Mail> Items)> GetMailsPaged(int page, int pageSize, string? search, bool? isRead, bool? isClaimed);
    }
}
