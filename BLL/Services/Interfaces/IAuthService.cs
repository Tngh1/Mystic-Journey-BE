using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> Login(LoginRequestDto request);
        Task<AuthResponseDto> Register(RegisterRequestDto request);
        Task<AuthResponseDto> ChangePassword(int accountId, ChangePasswordRequestDto request);
        Task SendVerificationCode(string email);
        Task VerifyEmail(VerifyEmailRequestDto request);
        Task<AuthResponseDto> RefreshToken(string refreshToken);
        Task RevokeRefreshToken(int accountId);
        Task RevokeRefreshTokenByToken(string refreshToken);
        Task<MeResponseDto> GetMe(int accountId);
        Task ForgotPassword(string email);
        Task ResetPassword(string email, string verificationCode, string newPassword, string confirmPassword);
    }
}
