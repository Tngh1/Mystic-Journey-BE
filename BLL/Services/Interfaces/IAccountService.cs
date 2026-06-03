using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IAccountService
    {
        Task<AccountResponseDto> LoginAsync(LoginRequestDto request);
        Task<AccountResponseDto> RegisterAsync(RegisterRequestDto request);
        Task<AccountResponseDto> ChangePasswordAsync(int accountId, ChangePasswordRequestDto request);
        Task SendVerificationCodeAsync(string email);
        Task VerifyEmailAsync(VerifyEmailRequestDto request);
        Task<AccountResponseDto> RefreshTokenAsync(string refreshToken);
        Task ForgotPasswordAsync(string email);
        Task ResetPasswordAsync(string email, string verificationCode, string newPassword, string confirmPassword);
    }
}
