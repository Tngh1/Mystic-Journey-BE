using BLL.DTOs;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IAccountAdminService
    {
        Task<AccountAdminResponseDto?> GetAccountById(int id);
        Task<AccountAdminResponseDto> CreateAccount(CreateAccountAdminRequestDto request);
        Task<AccountAdminResponseDto> UpdateAccount(int id, UpdateAccountAdminRequestDto request);
        Task<PagedResultDto<AccountAdminResponseDto>> GetAccountsPaged(int page, int pageSize, string? search, bool? isActive, string? roleName);
        Task<AccountAdminResponseDto> BanAccount(int accountId);
        Task<AccountAdminResponseDto> UnbanAccount(int accountId);
    }
}
