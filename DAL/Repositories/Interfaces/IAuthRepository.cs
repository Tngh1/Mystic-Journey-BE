using DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Quản lý xác thực (authentication) và quản lý tài khoản.
    // Cho phép đăng nhập, đăng ký, đổi mật khẩu, xác thực email.
    public interface IAuthRepository
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy tài khoản theo mã định danh.
        Task<Account?> GetAccountById(int id);

        // Lấy tài khoản theo username hoặc email.
        Task<Account?> GetAccountByUsernameOrEmail(string emailOrUsername);

        // Kiểm tra email đã được sử dụng chưa.
        Task<bool> IsEmailExist(string email);

        // Kiểm tra username đã được sử dụng chưa.
        Task<bool> IsUsernameExist(string username);

        // Lấy tài khoản theo email.
        Task<Account?> GetAccountByEmail(string email);

        // Lấy tài khoản theo refresh token.
        Task<Account?> GetAccountByRefreshToken(string refreshToken);

        // Thu hồi refresh token của tài khoản.
        Task RevokeRefreshToken(int accountId);

        // Thu hồi refresh token bằng giá trị token.
        Task RevokeRefreshTokenByToken(string refreshToken);

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // Tạo tài khoản mới.
        Task<Account> CreateAccount(Account account);

        // Cập nhật thông tin tài khoản.
        Task<Account> UpdateAccount(Account account);

        // Đếm tổng số tài khoản đang hoạt động.
        Task<int> GetTotalAccountsCount();

        // Lấy danh sách tài khoản có phân trang, lọc theo tìm kiếm, trạng thái và vai trò.
        Task<(int TotalCount, List<Account> Items)> GetAccountsPaged(int page, int pageSize, string? search, bool? isActive, string? roleName);
    }
}
