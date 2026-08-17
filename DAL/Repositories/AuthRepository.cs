using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    // Queries the database to retrieve i auth repository records.
    public class AuthRepository : IAuthRepository
    {
        private readonly MysticJourneyDbContext _context;

        // Initializes a new instance of AuthRepository with dependencies: context.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public AuthRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        // Performs database query and transactional persistence workflow for get account by id.
        // Query details: eagerly loads related entity navigation properties.
        // Returns the matching Account? entity result or default if not found.
        public async Task<Account?> GetAccountById(int id)
        {
            return await _context.Accounts
                .Include(a => a.Role)  // Eagerly load related navigation entities to avoid N+1 queries
                .Include(a => a.PlayerProfile)  // Eagerly load related navigation entities to avoid N+1 queries
                .FirstOrDefaultAsync(a => a.AccountId == id);  // Fetch single matching record or null if not found
        }

        // Persists state modifications to the database for touch last seen.
        public Task TouchLastSeen(int accountId, DateTime lastSeenUtc)
        {
            return _context.PlayerProfiles
                .Where(p => p.AccountId == accountId)  // Filter records matching the predicate
                // Apply this bulk change directly in the database without loading every affected entity.
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.LastSeen, lastSeenUtc));
        }

        // Performs database query and transactional persistence workflow for clear last seen.
        // Query details: eagerly loads related entity navigation properties.
        public Task ClearLastSeen(int accountId)
        {
            return _context.PlayerProfiles
                .Where(p => p.AccountId == accountId)  // Filter records matching the predicate
                // Apply this bulk change directly in the database without loading every affected entity.
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.LastSeen, DateTime.UnixEpoch));
        }

        // Queries the database to retrieve get account by username or email records.
        // Query details: eagerly loads related entity navigation properties.
        // Returns the matching Account? entity result or default if not found.
        public async Task<Account?> GetAccountByUsernameOrEmail(string emailOrUsername)
        {
            return await _context.Accounts
                .Include(a => a.Role)  // Eagerly load related navigation entities to avoid N+1 queries
                .Include(a => a.PlayerProfile)  // Eagerly load related navigation entities to avoid N+1 queries
                .FirstOrDefaultAsync(a =>  // Fetch single matching record or null if not found
                    a.UserName.ToLower() == emailOrUsername.ToLower() || a.Email.ToLower() == emailOrUsername.ToLower());
        }

        // Queries the database to retrieve is email exist records.
        // Returns true if the operation succeeded or record exists; otherwise false.
        public async Task<bool> IsEmailExist(string email)
        {
            return await _context.Accounts
                .AnyAsync(a => a.Email.ToLower() == email.ToLower());  // Check existence without loading the full entity
        }

        // Performs database query and transactional persistence workflow for is username exist.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns true if the operation succeeded or record exists; otherwise false.
        public async Task<bool> IsUsernameExist(string username)
        {
            return await _context.Accounts
                .AnyAsync(a => a.UserName.ToLower() == username.ToLower());  // Check existence without loading the full entity
        }

        // Persists state modifications to the database for create account.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching Account entity result or default if not found.
        public async Task<Account> CreateAccount(Account account)
        {
            account.CreatedAt = DateTime.UtcNow;
            await _context.Accounts.AddAsync(account);  // Stage new entity for insertion in the next SaveChanges call
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return account;
        }

        // Performs database query and transactional persistence workflow for update account.
        // Query details: eagerly loads related entity navigation properties; commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching Account entity result or default if not found.
        public async Task<Account> UpdateAccount(Account account)
        {
            account.UpdatedAt = DateTime.UtcNow;
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return account;
        }

        // Queries the database to retrieve get account by email records.
        // Query details: eagerly loads related entity navigation properties.
        // Returns the matching Account? entity result or default if not found.
        public async Task<Account?> GetAccountByEmail(string email)
        {
            return await _context.Accounts
                .Include(a => a.Role)  // Eagerly load related navigation entities to avoid N+1 queries
                .FirstOrDefaultAsync(a => a.Email.ToLower() == email.ToLower() && a.IsActive);  // Fetch single matching record or null if not found
        }

        // Performs database query and transactional persistence workflow for get account by refresh token.
        // Query details: eagerly loads related entity navigation properties; commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching Account? entity result or default if not found.
        public async Task<Account?> GetAccountByRefreshToken(string refreshToken)
        {
            return await _context.Accounts
                .Include(a => a.Role)  // Eagerly load related navigation entities to avoid N+1 queries
                .Include(a => a.PlayerProfile)  // Eagerly load related navigation entities to avoid N+1 queries
                .FirstOrDefaultAsync(a =>  // Fetch single matching record or null if not found
                    (a.RefreshToken == refreshToken || a.GameRefreshToken == refreshToken) && a.IsActive);
        }

        // Queries the database to retrieve revoke refresh token records.
        public async Task RevokeRefreshToken(int accountId, string? clientType)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null)  // Entity not found — short-circuit with appropriate error result
                return;

            var isGame = string.Equals(clientType, "Game", StringComparison.OrdinalIgnoreCase);
            var isWeb = string.Equals(clientType, "Web", StringComparison.OrdinalIgnoreCase);

            if (clientType == null || !isWeb)
            {
                account.GameRefreshToken = null;
                account.GameRefreshTokenExpiresAt = null;
            }
            if (clientType == null || !isGame)
            {
                account.RefreshToken = null;
                account.RefreshTokenExpiresAt = null;
            }
            account.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
        }

        // Queries the database to retrieve get total accounts count records.
        // Query details: eagerly loads related entity navigation properties.
        // Returns the computed numeric count or database ID result.
        public async Task<int> GetTotalAccountsCount()
        {
            return await _context.Accounts
                .CountAsync(a => a.IsActive);
        }

        // Load all active accounts async; it filters the eligible records and materializes the query results.
        public async Task<List<Account>> GetAllActiveAccountsAsync()
        {
            return await _context.Accounts
                .Include(a => a.PlayerProfile)  // Eagerly load related navigation entities to avoid N+1 queries
                .Where(a => a.IsActive)  // Filter records matching the predicate
                .ToListAsync();  // Materialize the query into a list from the database
        }

        // Queries the database to retrieve get accounts paged records.
        // Query details: uses AsNoTracking() for read-only query optimization; eagerly loads related entity navigation properties.
        public async Task<(int TotalCount, List<Account> Items)> GetAccountsPaged(int page, int pageSize, string? search, bool? isActive, string? roleName)
        {
            var query = _context.Accounts
                .Include(a => a.Role)  // Eagerly load related navigation entities to avoid N+1 queries
                .Include(a => a.PlayerProfile)  // Eagerly load related navigation entities to avoid N+1 queries
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking();  // Disable EF Core change tracking for this read-only query

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(a => a.UserName.Contains(search) || a.Email.Contains(search));  // Filter records matching the predicate
            }
            if (isActive.HasValue)
            {
                query = query.Where(a => a.IsActive == isActive.Value);  // Filter records matching the predicate
            }
            if (!string.IsNullOrEmpty(roleName))
            {
                query = query.Where(a => a.Role != null && a.Role.Name == roleName);  // Filter records matching the predicate
            }

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();  // Materialize the query into a list from the database

            return (totalCount, items);
        }
    }
}
