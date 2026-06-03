using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
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

        public async Task<Account?> GetByIdAsync(Guid accountId)
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
                    (a.UserName.ToLower() == emailOrUsername.ToLower() || a.EmailAddress.ToLower() == emailOrUsername.ToLower())
                    && a.IsActive);
        }

        public async Task<bool> IsEmailExistAsync(string email)
        {
            return await _context.Accounts
                .AnyAsync(a => a.EmailAddress.ToLower() == email.ToLower());
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
                .FirstOrDefaultAsync(a => a.EmailAddress.ToLower() == email.ToLower() && a.IsActive);
        }

        public async Task<Account?> GetByEmailAndPasswordResetCodeAsync(string email, string code)
        {
            return await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a =>
                    a.EmailAddress.ToLower() == email.ToLower() &&
                    a.PasswordResetToken == code &&
                    a.IsActive);
        }

        public async Task<Account?> GetByPasswordResetTokenAsync(string token)
        {
            return await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.PasswordResetToken == token && a.IsActive);
        }
    }
}
