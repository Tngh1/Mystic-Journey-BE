using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
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

        public async Task<Account?> GetByIdAsync(int accountId)
        {
            return await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.AccountId == accountId && a.IsActive);
        }

        public async Task<Account?> GetByUsernameOrEmailAsync(string emailOrUsername)
        {
            return await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a =>
                    (a.UserName.ToLower() == emailOrUsername.ToLower() || a.Email.ToLower() == emailOrUsername.ToLower())
                    && a.IsActive);
        }

        public async Task<bool> IsEmailExistAsync(string email)
        {
            return await _context.Accounts
                .AnyAsync(a => a.Email.ToLower() == email.ToLower());
        }

        public async Task<bool> IsUsernameExistAsync(string username)
        {
            return await _context.Accounts
                .AnyAsync(a => a.UserName.ToLower() == username.ToLower());
        }

        public async Task CreateAccountAsync(Account account)
        {
            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAccountAsync(Account account)
        {
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
        }

        public async Task<Account?> GetByEmailAsync(string email)
        {
            return await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.Email.ToLower() == email.ToLower() && a.IsActive);
        }

        public async Task<Account?> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.RefreshToken == refreshToken && a.IsActive);
        }
    }
}
