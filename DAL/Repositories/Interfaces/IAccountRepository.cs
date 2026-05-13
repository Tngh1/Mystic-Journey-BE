using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account?> GetByIdAsync(Guid accountId);
        Task<Account?> GetByUsernameOrEmailAsync(string emailOrUsername);
        Task<bool> IsEmailExistAsync(string email);
        Task<bool> IsUsernameExistAsync(string username);
        Task CreateAccountAsync(Account account);
        Task UpdateAccountAsync(Account account);
        Task<Account?> GetByEmailAsync(string email);
        Task<Account?> GetByEmailAndVerificationCodeAsync(string email, string code);
        Task<Account?> GetByEmailAndPasswordResetCodeAsync(string email, string code);
        Task<Account?> GetByPasswordResetTokenAsync(string token);

    }
}
