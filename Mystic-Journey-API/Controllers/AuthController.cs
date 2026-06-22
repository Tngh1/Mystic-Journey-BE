using BLL.DTOs;
using BLL.Services;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Security.Claims;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        private int GetCurrentAccountId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(claim, out var accountId))
                throw new UnauthorizedAccessException("Invalid authentication token.");
            return accountId;
        }

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

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var result = await _authService.Login(request);
            SetTokenCookies(result.AccessToken!, result.AccessTokenExpiresAt!.Value, result.RefreshToken!, result.RefreshTokenExpiresAt!.Value);
            return Ok(new ApiResponse<AuthResponseDto> { Success = true, Data = result });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var result = await _authService.Register(request);
            SetTokenCookies(result.AccessToken!, result.AccessTokenExpiresAt!.Value, result.RefreshToken!, result.RefreshTokenExpiresAt!.Value);
            return Ok(new ApiResponse<AuthResponseDto> { Success = true, Data = result });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var accountId = GetCurrentAccountId();
            var result = await _authService.GetMe(accountId);
            return Ok(new ApiResponse<MeResponseDto> { Success = true, Data = result });
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
        {
            var accountId = GetCurrentAccountId();
            await _authService.ChangePassword(accountId, request);
            return Ok(new ApiResponse<object> { Success = true, Message = "Password changed successfully." });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var accountId = GetCurrentAccountId();
            await _authService.RevokeRefreshToken(accountId);
            ClearTokenCookies();
            return Ok(new ApiResponse<object> { Success = true, Message = "Logged out successfully." });
        }

        [AllowAnonymous]
        [HttpPost("send-verification-code")]
        public async Task<IActionResult> SendVerificationCode([FromBody] SendVerificationCodeRequestDto request)
        {
            await _authService.SendVerificationCode(request.Email);
            return Ok(new ApiResponse<object> { Success = true, Message = $"Verification code sent to {request.Email}." });
        }

        [AllowAnonymous]
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequestDto request)
        {
            await _authService.VerifyEmail(request);
            return Ok(new ApiResponse<object> { Success = true, Message = "Email verified successfully." });
        }

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

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        {
            await _authService.ForgotPassword(request.Email);
            return Ok(new ApiResponse<object> { Success = true, Message = $"Reset code sent to {request.Email}." });
        }

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
    }
}
