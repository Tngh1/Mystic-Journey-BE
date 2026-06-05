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
        IQueryable<AccountAdminResponseDto> GetAccountsQueryable();
    }
}
