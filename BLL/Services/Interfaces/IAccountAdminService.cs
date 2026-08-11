using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Quản lý tài khoản người chơi.
    // Admin APIs: Xem, ban/unban tài khoản.
    //
    // Không có CreateAccount/UpdateAccount: hai hàm đó chỉ phục vụ màn quản lý Admin của
    // SuperAdmin, role đã bỏ. Nếu để lại thì đường tạo/nâng quyền Admin vẫn còn đó, chỉ
    // là chưa có controller gọi — dễ bị nối lại sau này mà không ai xét quyền.
    public interface IAccountAdminService
    {
        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy chi tiết account theo ID.
        Task<AccountAdminResponseDto?> GetAccountById(int id);

        // Lấy danh sách accounts có phân trang và lọc.
        Task<PagedResultDto<AccountAdminResponseDto>> GetAccountsPaged(int page, int pageSize, string? search, bool? isActive, string? roleName);

        // Ban tài khoản.
        Task<AccountAdminResponseDto> BanAccount(int accountId, string? banReason);

        // Unban tài khoản.
        Task<AccountAdminResponseDto> UnbanAccount(int accountId);
    }
}
