using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Initializes a new default instance of the IAuthService class.
    public interface IAuthService
    {

        Task<AuthResponseDto> Login(LoginRequestDto request);

        Task<AuthResponseDto> Register(RegisterRequestDto request);

        Task<MeResponseDto> GetMe(int accountId);

        Task<AuthResponseDto> ChangePassword(int accountId, ChangePasswordRequestDto request, string clientType);

        Task<AuthResponseDto> RefreshToken(string refreshToken);

        Task RevokeRefreshToken(int accountId, string? clientType);

        Task RevokeRefreshTokenByToken(string refreshToken);

        Task SendVerificationCode(string email);

        Task VerifyEmail(VerifyEmailRequestDto request);

        Task ForgetPassword(string email);

        Task ResetPassword(string email, string verificationCode, string newPassword, string confirmPassword);
    }
}
