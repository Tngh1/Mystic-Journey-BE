using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IAccountService
    {
        Task<AccountResponseDto> LoginAccount(LoginRequestDto request);
        Task<LoginGameResponseDto> LoginGame(LoginGameRequestDto request);
        Task<AccountResponseDto> RegisterAccount(RegisterRequestDto request);
        Task<AccountResponseDto> ChangePassword(int accountId, ChangePasswordRequestDto request);
        Task SendVerificationCode(string email);
        Task VerifyEmail(VerifyEmailRequestDto request);
        Task<AccountResponseDto> RefreshToken(string refreshToken);
        Task RevokeRefreshToken(int accountId);
        Task RevokeRefreshTokenByToken(string refreshToken);
        Task<MeResponseDto> GetMe(int accountId);
        Task ForgotPassword(string email);
        Task ResetPassword(string email, string verificationCode, string newPassword, string confirmPassword);
    }
}
