using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly MysticJourneyDbContext _context;

        public AuthRepository(MysticJourneyDbContext context)
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

        /// <summary>
        /// UPDATE một cột duy nhất trên profile, không SELECT và không tracking.
        /// </summary>
        public Task TouchLastSeen(int accountId, DateTime lastSeenUtc)
        {
            return _context.PlayerProfiles
                .Where(p => p.AccountId == accountId)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.LastSeen, lastSeenUtc));
        }

        /// <summary>Xoá mốc presence của nhân vật khi game client đăng xuất.</summary>
        public Task ClearLastSeen(int accountId)
        {
            return _context.PlayerProfiles
                .Where(p => p.AccountId == accountId)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.LastSeen, DateTime.UnixEpoch));
        }

        public async Task<Account?> GetAccountByUsernameOrEmail(string emailOrUsername)
        {
            return await _context.Accounts
                .Include(a => a.Role)
                .Include(a => a.PlayerProfile)
                .FirstOrDefaultAsync(a =>
                    a.UserName.ToLower() == emailOrUsername.ToLower() || a.Email.ToLower() == emailOrUsername.ToLower());
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

        // Dò CẢ HAI slot: token game nằm ở cột riêng, chỉ so RefreshToken là client game
        // refresh mãi không ra tài khoản nào và bị đăng xuất oan.
        public async Task<Account?> GetAccountByRefreshToken(string refreshToken)
        {
            return await _context.Accounts
                .Include(a => a.Role)
                .Include(a => a.PlayerProfile)
                .FirstOrDefaultAsync(a =>
                    (a.RefreshToken == refreshToken || a.GameRefreshToken == refreshToken) && a.IsActive);
        }

        // clientType = null: xoá sạch cả hai slot (đổi/đặt lại mật khẩu, ban). Truyền Web/Game
        // để chỉ xoá một phía (logout) và giữ client kia đăng nhập.
        public async Task RevokeRefreshToken(int accountId, string? clientType)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null)
                return;

            // "Game"/"Web" viết thẳng vì DAL không được tham chiếu lên BLL (nơi giữ hằng
            // AuthService.ClientGame/ClientWeb). So không phân biệt hoa thường để lệch case
            // giữa hai tầng không âm thầm rơi vào nhánh Web.
            var isGame = string.Equals(clientType, "Game", StringComparison.OrdinalIgnoreCase);
            var isWeb = string.Equals(clientType, "Web", StringComparison.OrdinalIgnoreCase);

            // Điều kiện viết dạng phủ định slot kia để giá trị lạ (không phải Web/Game) xoá CẢ
            // HAI thay vì không xoá gì — thu hồi hụt trên đường bảo mật thì tệ hơn thu hồi thừa.
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
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetTotalAccountsCount()
        {
            return await _context.Accounts
                .CountAsync(a => a.IsActive);
        }

        public async Task<List<Account>> GetAllActiveAccountsAsync()
        {
            return await _context.Accounts
                .Include(a => a.PlayerProfile)
                .Where(a => a.IsActive)
                .ToListAsync();
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
