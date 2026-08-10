using BLL.DTOs;
using BLL.Services;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Security.Claims;

namespace Mystic_Journey_API.Controllers
{
    // Quản lý xác thực (authentication) và quản lý tài khoản.
    // Cho phép đăng nhập, đăng ký, đổi mật khẩu, xác thực email.
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        private static bool IsVersionLower(string clientVer, string minVer)
        {
            if (Version.TryParse(clientVer, out var parsedClient) && Version.TryParse(minVer, out var parsedMin))
            {
                return parsedClient < parsedMin;
            }
            return string.Compare(clientVer, minVer, StringComparison.OrdinalIgnoreCase) < 0;
        }

        private int GetCurrentAccountId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(claim, out var accountId))
                throw new UnauthorizedAccessException("Invalid authentication token.");
            return accountId;
        }

        // Loại client đọc từ claim trong access token, KHÔNG từ body/header: client tự khai thì
        // web khai "Game" là thu hồi được phiên game của chính chủ. Token cũ (phát trước khi
        // tách slot) không có claim này nên mặc định Web — đúng, vì slot web là slot cũ.
        private string GetCurrentClientType()
            => User.FindFirstValue(AuthService.ClientTypeClaim) ?? AuthService.ClientWeb;

        private CookieOptions BuildCookieOptions(DateTime expiry) => new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/",
            Expires = expiry
        };

        private void SetTokenCookies(string accessToken, DateTime accessExpiry, string refreshToken, DateTime refreshExpiry)
        {
            Response.Cookies.Append("access_token", accessToken, BuildCookieOptions(accessExpiry.ToUniversalTime()));
            Response.Cookies.Append("refresh_token", refreshToken, BuildCookieOptions(refreshExpiry.ToUniversalTime()));
        }

        private void ClearTokenCookies()
        {
            var options = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/"
            };
            Response.Cookies.Delete("access_token", options);
            Response.Cookies.Delete("refresh_token", options);
        }

        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // ── POST /api/auth/login ─────────────────────────────────────
        // Đăng nhập bằng email và mật khẩu.
        // Trả về access token và refresh token.
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            // Version check for Client logins
            var clientVer = request.ClientVersion ?? Request.Headers["X-Client-Version"].FirstOrDefault();
            if (!string.IsNullOrEmpty(clientVer))
            {
                var minVer = _configuration["GameVersion:MinRequiredVersion"] ?? "1.0.0";
                if (IsVersionLower(clientVer, minVer))
                {
                    var downloadUrl = _configuration["GameVersion:DownloadUrl"] ?? "";
                    return StatusCode(426, new ApiResponse<object>
                    {
                        Success = false,
                        Message = $"Game version outdated (v{clientVer}). Minimum required version is v{minVer}.",
                        ErrorCode = "CLIENT_OUTDATED",
                        Data = new
                        {
                            MinRequiredVersion = minVer,
                            LatestVersion = _configuration["GameVersion:LatestVersion"] ?? minVer,
                            DownloadUrl = downloadUrl,
                            ForceUpdate = true
                        }
                    });
                }
            }

            var result = await _authService.Login(request);
            SetTokenCookies(result.AccessToken!, result.AccessTokenExpiresAt!.Value, result.RefreshToken!, result.RefreshTokenExpiresAt!.Value);
            return Ok(new ApiResponse<AuthResponseDto> { Success = true, Data = result });
        }

        // ── POST /api/auth/register ─────────────────────────────────
        // Đăng ký tài khoản mới.
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var result = await _authService.Register(request);
            SetTokenCookies(result.AccessToken!, result.AccessTokenExpiresAt!.Value, result.RefreshToken!, result.RefreshTokenExpiresAt!.Value);
            return Ok(new ApiResponse<AuthResponseDto> { Success = true, Data = result });
        }

        // ── GET /api/auth/me ────────────────────────────────────────
        // Lấy thông tin tài khoản hiện tại.
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var accountId = GetCurrentAccountId();
            var result = await _authService.GetMe(accountId);
            return Ok(new ApiResponse<MeResponseDto> { Success = true, Data = result });
        }

        // ── POST /api/auth/change-password ──────────────────────────
        // Đổi mật khẩu.
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
        {
            var accountId = GetCurrentAccountId();
            var result = await _authService.ChangePassword(accountId, request, GetCurrentClientType());
            // Đổi mật khẩu xoay session + refresh token để đá thiết bị cũ ra, nên cookie
            // hiện tại đã hết hiệu lực. Không set lại thì chính người vừa đổi cũng bị logout.
            SetTokenCookies(result.AccessToken!, result.AccessTokenExpiresAt!.Value, result.RefreshToken!, result.RefreshTokenExpiresAt!.Value);
            return Ok(new ApiResponse<object> { Success = true, Message = "Password changed successfully." });
        }

        // ── POST /api/auth/logout ───────────────────────────────────
        // Đăng xuất.
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var accountId = GetCurrentAccountId();
            // Chỉ thu hồi phía đang gọi: logout trên web không được đá phiên game đang chơi.
            await _authService.RevokeRefreshToken(accountId, GetCurrentClientType());
            ClearTokenCookies();
            return Ok(new ApiResponse<object> { Success = true, Message = "Logged out successfully." });
        }

        // ── POST /api/auth/refresh-token ────────────────────────────
        // Làm mới access token bằng refresh token.
        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refresh_token"];
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "No refresh token found.", ErrorCode = ErrorCodes.Unauthorized });

            try
            {
                var result = await _authService.RefreshToken(refreshToken);
                SetTokenCookies(result.AccessToken!, result.AccessTokenExpiresAt!.Value, result.RefreshToken!, result.RefreshTokenExpiresAt!.Value);
                return Ok(new ApiResponse<AuthResponseDto> { Success = true, Data = result });
            }
            catch (UnauthorizedAccessException ex)
            {
                await _authService.RevokeRefreshTokenByToken(refreshToken);
                ClearTokenCookies();
                return Unauthorized(new ApiResponse<object> { Success = false, Message = ex.Message, ErrorCode = ErrorCodes.Unauthorized });
            }
        }

        // ── POST /api/auth/forgot-password ──────────────────────────
        // Gửi mã đặt lại mật khẩu qua email.
        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        {
            await _authService.ForgotPassword(request.Email);
            return Ok(new ApiResponse<object> { Success = true, Message = $"Reset code sent to {request.Email}." });
        }

        // ── POST /api/auth/reset-password ────────────────────────────
        // Đặt lại mật khẩu bằng mã xác thực.
        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
        {
            await _authService.ResetPassword(
                request.Email,
                request.VerificationCode,
                request.NewPassword,
                request.ConfirmPassword);
            return Ok(new ApiResponse<object> { Success = true, Message = "Password reset successfully." });
        }

        // ── POST /api/auth/send-verification-code ───────────────────
        // Gửi mã xác thực email.
        [AllowAnonymous]
        [HttpPost("send-verification-code")]
        public async Task<IActionResult> SendVerificationCode([FromBody] SendVerificationCodeRequestDto request)
        {
            await _authService.SendVerificationCode(request.Email);
            return Ok(new ApiResponse<object> { Success = true, Message = $"Verification code sent to {request.Email}." });
        }

        // ── POST /api/auth/verify-email ─────────────────────────────
        // Xác thực email bằng mã.
        [AllowAnonymous]
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequestDto request)
        {
            await _authService.VerifyEmail(request);
            return Ok(new ApiResponse<object> { Success = true, Message = "Email verified successfully." });
        }
    }
}
