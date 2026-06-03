using DAL.Models;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account?> GetByIdAsync(int accountId);
        Task<Account?> GetByUsernameOrEmailAsync(string emailOrUsername);
        Task<bool> IsEmailExistAsync(string email);
        Task<bool> IsUsernameExistAsync(string username);
        Task CreateAccountAsync(Account account);
        Task UpdateAccountAsync(Account account);
        Task<Account?> GetByEmailAsync(string email);
        Task<Account?> GetByRefreshTokenAsync(string refreshToken);
    }
}
