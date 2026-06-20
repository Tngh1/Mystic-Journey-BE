using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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

        public async Task<Account?> GetAccountById(int id)
        {
            return await _context.Accounts
                .Include(a => a.Role)
                .Include(a => a.PlayerProfile)
                .FirstOrDefaultAsync(a => a.AccountId == id);
        }

        public async Task<Account?> GetAccountByUsernameOrEmail(string emailOrUsername)
        {
            return await _context.Accounts
                .Include(a => a.Role)
                .Include(a => a.PlayerProfile)
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
                .Include(a => a.PlayerProfile)
                .FirstOrDefaultAsync(a => a.RefreshToken == refreshToken && a.IsActive);
        }

        public async Task RevokeRefreshToken(int accountId)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            if (account != null)
            {
                account.RefreshToken = null;
                account.RefreshTokenExpiresAt = null;
                account.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task RevokeRefreshTokenByToken(string refreshToken)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.RefreshToken == refreshToken);
            if (account != null)
            {
                account.RefreshToken = null;
                account.RefreshTokenExpiresAt = null;
                account.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetTotalAccountsCount()
        {
            return await _context.Accounts
                .CountAsync(a => a.IsActive);
        }

        public async Task<(int TotalCount, List<Account> Items)> GetAccountsPaged(int page, int pageSize, string? search, bool? isActive, string? roleName)
        {
            var query = _context.Accounts
                .Include(a => a.Role)
                .Include(a => a.PlayerProfile)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(a => a.UserName.Contains(search) || a.Email.Contains(search));
            }
            if (isActive.HasValue)
            {
                query = query.Where(a => a.IsActive == isActive.Value);
            }
            if (!string.IsNullOrEmpty(roleName))
            {
                query = query.Where(a => a.Role != null && a.Role.Name == roleName);
            }

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (totalCount, items);
        }
    }
}
