using DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<Account?> GetAccountById(int id);
        Task<Account?> GetAccountByUsernameOrEmail(string emailOrUsername);
        Task<bool> IsEmailExist(string email);
        Task<bool> IsUsernameExist(string username);
        Task<Account> CreateAccount(Account account);
        Task<Account> UpdateAccount(Account account);
        Task<Account?> GetAccountByEmail(string email);
        Task<Account?> GetAccountByRefreshToken(string refreshToken);
        Task RevokeRefreshToken(int accountId);
        Task RevokeRefreshTokenByToken(string refreshToken);
        Task<int> GetTotalAccountsCount();
        Task<(int TotalCount, List<Account> Items)> GetAccountsPaged(int page, int pageSize, string? search, bool? isActive, string? roleName);
    }
}
