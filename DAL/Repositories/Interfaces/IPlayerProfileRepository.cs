using DAL.Models;
using System;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IPlayerProfileRepository
    {
        Task<PlayerProfile?> GetByIdAsync(int id);
        Task<PlayerProfile?> GetByAccountIdAsync(Guid accountId);
        Task AddAsync(PlayerProfile profile);
        Task UpdateAsync(PlayerProfile profile);
    }
}
