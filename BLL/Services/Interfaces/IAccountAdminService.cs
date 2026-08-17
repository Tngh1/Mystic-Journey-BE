using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Initializes a new default instance of the IAccountAdminService class.
    public interface IAccountAdminService
    {

        Task<AccountAdminResponseDto?> GetAccountById(int id);

        Task<PagedResultDto<AccountAdminResponseDto>> GetAccountsPaged(int page, int pageSize, string? search, bool? isActive, string? roleName);

        Task<AccountAdminResponseDto> BanAccount(int accountId, string? banReason);

        Task<AccountAdminResponseDto> UnbanAccount(int accountId);
    }
}
