using DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Initializes a new default instance of the IAuthRepository class.
    public interface IAuthRepository
    {

        Task<Account?> GetAccountById(int id);

        Task TouchLastSeen(int accountId, DateTime lastSeenUtc);

        Task ClearLastSeen(int accountId);

        Task<Account?> GetAccountByUsernameOrEmail(string emailOrUsername);

        Task<bool> IsEmailExist(string email);

        Task<bool> IsUsernameExist(string username);

        Task<Account?> GetAccountByEmail(string email);

        Task<Account?> GetAccountByRefreshToken(string refreshToken);

        Task RevokeRefreshToken(int accountId, string? clientType);


        Task<Account> CreateAccount(Account account);

        Task<Account> UpdateAccount(Account account);

        Task<int> GetTotalAccountsCount();

        Task<List<Account>> GetAllActiveAccountsAsync();

        Task<(int TotalCount, List<Account> Items)> GetAccountsPaged(int page, int pageSize, string? search, bool? isActive, string? roleName);
    }
}
