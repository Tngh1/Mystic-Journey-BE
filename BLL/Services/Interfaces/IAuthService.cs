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

        // Đổi mật khẩu. clientType là loại client đang gọi (Web/Game) — đổi mật khẩu thu hồi
        // CẢ HAI slot, nên phải biết client nào gọi để cấp lại token cho đúng nó.
        Task<AuthResponseDto> ChangePassword(int accountId, ChangePasswordRequestDto request, string clientType);

        // Làm mới access token bằng refresh token. Slot (Web/Game) được suy ra từ chính token.
        Task<AuthResponseDto> RefreshToken(string refreshToken);

        // Thu hồi refresh token. clientType = null nghĩa là thu hồi MỌI slot (đổi/đặt lại mật
        // khẩu, ban); truyền Web/Game để chỉ thu hồi một phía (logout). Không có default:
        // chọn sai ở đây là đá oan client kia hoặc để hở phiên của kẻ chiếm tài khoản.
        Task RevokeRefreshToken(int accountId, string? clientType);

        // Thu hồi refresh token bằng token cụ thể.
        Task RevokeRefreshTokenByToken(string refreshToken);

        // Gửi mã xác thực email.
        Task SendVerificationCode(string email);

        // Xác thực email bằng mã.
        Task VerifyEmail(VerifyEmailRequestDto request);

        // Gửi mã đặt lại mật khẩu qua email.
        Task ForgetPassword(string email);

        // Đặt lại mật khẩu bằng mã xác thực.
        Task ResetPassword(string email, string verificationCode, string newPassword, string confirmPassword);
    }
}
