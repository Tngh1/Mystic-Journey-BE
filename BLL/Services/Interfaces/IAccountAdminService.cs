using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Quản lý tài khoản admin.
    // Admin APIs: Xem, tạo, cập nhật, ban/unban tài khoản.
    public interface IAccountAdminService
    {
        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy chi tiết account theo ID.
        Task<AccountAdminResponseDto?> GetAccountById(int id);

        // Lấy danh sách tất cả accounts có phân trang và lọc.
        Task<PagedResultDto<AccountAdminResponseDto>> GetAccountsPaged(int page, int pageSize, string? search, bool? isActive, string? roleName);

        // Tạo tài khoản admin mới.
        Task<AccountAdminResponseDto> CreateAccount(CreateAccountAdminRequestDto request);

        // Cập nhật tài khoản hiện có.
        Task<AccountAdminResponseDto> UpdateAccount(int id, UpdateAccountAdminRequestDto request);

        // Ban tài khoản.
        Task<AccountAdminResponseDto> BanAccount(int accountId);

        // Unban tài khoản.
        Task<AccountAdminResponseDto> UnbanAccount(int accountId);
    }
}
