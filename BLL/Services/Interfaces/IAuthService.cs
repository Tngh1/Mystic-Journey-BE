using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Quản lý xác thực (authentication) và quản lý tài khoản.
    // Cho phép đăng nhập, đăng ký, đổi mật khẩu, xác thực email.
    public interface IAuthService
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Đăng nhập bằng email và mật khẩu. Trả về access token và refresh token.
        Task<AuthResponseDto> Login(LoginRequestDto request);

        // Đăng ký tài khoản mới.
        Task<AuthResponseDto> Register(RegisterRequestDto request);

        // Lấy thông tin tài khoản hiện tại.
        Task<MeResponseDto> GetMe(int accountId);

        // Đổi mật khẩu.
        Task<AuthResponseDto> ChangePassword(int accountId, ChangePasswordRequestDto request);

        // Làm mới access token bằng refresh token.
        Task<AuthResponseDto> RefreshToken(string refreshToken);

        // Thu hồi refresh token.
        Task RevokeRefreshToken(int accountId);

        // Thu hồi refresh token bằng token cụ thể.
        Task RevokeRefreshTokenByToken(string refreshToken);

        // Gửi mã xác thực email.
        Task SendVerificationCode(string email);

        // Xác thực email bằng mã.
        Task VerifyEmail(VerifyEmailRequestDto request);

        // Gửi mã đặt lại mật khẩu qua email.
        Task ForgotPassword(string email);

        // Đặt lại mật khẩu bằng mã xác thực.
        Task ResetPassword(string email, string verificationCode, string newPassword, string confirmPassword);
    }
}
