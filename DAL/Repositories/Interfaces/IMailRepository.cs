using DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IMailRepository
    {
        Task<Mail?> GetByIdAsync(Guid mailId);
        Task<Mail?> GetByIdWithDetailsAsync(Guid mailId);
        Task<List<Mail>> GetByPlayerProfileIdAsync(Guid playerProfileId, int pageNumber = 1, int pageSize = 20);
        Task<List<Mail>> GetUnreadMailsAsync(Guid playerProfileId);
        Task<int> GetUnreadCountAsync(Guid playerProfileId);
        Task<Mail> CreateAsync(Mail mail);
        Task<Mail> UpdateAsync(Mail mail);
        Task<int> GetTotalCountAsync(Guid playerProfileId);
    }
}
