using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly MysticJourneyDbContext _context;

        public AccountRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<Account?> GetAccountById(int accountId)
        {
            return await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.AccountId == accountId && a.IsActive);
        }

        public async Task<Account?> GetAccountByIdWithRole(int id)
        {
            return await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.AccountId == id && a.IsActive);
        }

        public async Task<Account?> GetAccountByUsernameOrEmail(string emailOrUsername)
        {
            return await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a =>
                    (a.UserName.ToLower() == emailOrUsername.ToLower() || a.Email.ToLower() == emailOrUsername.ToLower())
                    && a.IsActive);
        }

        public async Task<bool> IsEmailExist(string email)
        {
            return await _context.Accounts
                .AnyAsync(a => a.Email.ToLower() == email.ToLower());
        }

        public async Task<bool> IsUsernameExist(string username)
        {
            return await _context.Accounts
                .AnyAsync(a => a.UserName.ToLower() == username.ToLower());
        }

        public async Task<Account> CreateAccount(Account account)
        {
            account.CreatedAt = DateTime.UtcNow;
            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();
            return account;
        }

        public async Task<Account> UpdateAccount(Account account)
        {
            account.UpdatedAt = DateTime.UtcNow;
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
            return account;
        }

        public async Task<Account?> GetAccountByEmail(string email)
        {
            return await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.Email.ToLower() == email.ToLower() && a.IsActive);
        }

        public async Task<Account?> GetAccountByRefreshToken(string refreshToken)
        {
            return await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.RefreshToken == refreshToken && a.IsActive);
        }

        public async Task<List<Account>> GetAllAccounts()
        {
            return await _context.Accounts
                .Where(a => a.IsActive)
                .ToListAsync();
        }

        public async Task<List<Account>> GetAllAccountsWithRoles()
        {
            return await _context.Accounts
                .Include(a => a.Role)
                .Where(a => a.IsActive)
                .ToListAsync();
        }

        public async Task<List<Account>> GetAdmins()
        {
            return await _context.Accounts
                .Include(a => a.Role)
                .Where(a => a.IsActive && a.RoleId != 1)
                .ToListAsync();
        }

        public async Task<int> GetTotalAccountsCount()
        {
            return await _context.Accounts
                .CountAsync(a => a.IsActive);
        }

        public IQueryable<Account> GetAccountsQueryable()
        {
            return _context.Accounts
                .Include(a => a.Role)
                .Include(a => a.PlayerProfile)
                .Where(a => a.IsActive)
                .AsNoTracking();
        }
    }
}
