using DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account?> GetAccountById(int accountId);
        Task<Account?> GetAccountByIdWithRole(int id);
        Task<Account?> GetAccountByUsernameOrEmail(string emailOrUsername);
        Task<bool> IsEmailExist(string email);
        Task<bool> IsUsernameExist(string username);
        Task<Account> CreateAccount(Account account);
        Task<Account> UpdateAccount(Account account);
        Task<Account?> GetAccountByEmail(string email);
        Task<Account?> GetAccountByRefreshToken(string refreshToken);
        Task<List<Account>> GetAllAccounts();
        Task<List<Account>> GetAllAccountsWithRoles();
        Task<List<Account>> GetAdmins();
        Task<int> GetTotalAccountsCount();
        IQueryable<Account> GetAccountsQueryable();
    }
}
