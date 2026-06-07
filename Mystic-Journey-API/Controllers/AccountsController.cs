using BLL.DTOs;
using BLL.Services;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
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
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Strict,
            Path = "/",
            Expires = expiry
        };

        private void SetTokenCookies(string accessToken, DateTime accessExpiry, string refreshToken, DateTime refreshExpiry)
        {
            Response.Cookies.Append("access_token", accessToken, BuildCookieOptions(accessExpiry));
            Response.Cookies.Append("refresh_token", refreshToken, BuildCookieOptions(refreshExpiry));
        }

        private void ClearTokenCookies()
        {
            Response.Cookies.Delete("access_token", new CookieOptions { Path = "/" });
            Response.Cookies.Delete("refresh_token", new CookieOptions { Path = "/" });
        }

        private MeResponseDto MeResponse(AccountResponseDto dto) => new MeResponseDto
        {
            AccountId = dto.AccountId,
            UserName = dto.UserName,
            Email = dto.EmailAddress,
            Role = dto.Role
        };

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var result = await _accountService.LoginAccount(request);
                SetTokenCookies(result.AccessToken!, result.AccessTokenExpiresAt!.Value.ToUniversalTime(), result.RefreshToken!, result.RefreshTokenExpiresAt!.Value.ToUniversalTime());
                return Ok(MeResponse(result));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("login-game")]
        public async Task<IActionResult> LoginGame([FromBody] LoginGameRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _accountService.LoginGame(request);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            try
            {
                var result = await _accountService.RegisterAccount(request);
                SetTokenCookies(result.AccessToken!, result.AccessTokenExpiresAt!.Value.ToUniversalTime(), result.RefreshToken!, result.RefreshTokenExpiresAt!.Value.ToUniversalTime());
                return Ok(MeResponse(result));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var accountId = GetCurrentAccountId();
            var result = await _accountService.GetMe(accountId);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
        {
            try
            {
                var accountId = GetCurrentAccountId();
                await _accountService.ChangePassword(accountId, request);
                return Ok(new { message = "Password changed successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var accountId = GetCurrentAccountId();
            await _accountService.RevokeRefreshToken(accountId);
            ClearTokenCookies();
            return Ok(new { message = "Logged out successfully." });
        }

        [AllowAnonymous]
        [HttpPost("send-verification-code")]
        public async Task<IActionResult> SendVerificationCode([FromBody] SendVerificationCodeRequestDto request)
        {
            try
            {
                await _accountService.SendVerificationCode(request.Email);
                return Ok(new { message = $"Verification code sent to {request.Email}." });
            }
            catch (System.ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequestDto request)
        {
            try
            {
                await _accountService.VerifyEmail(request);
                return Ok(new { message = "Email verified successfully." });
            }
            catch (System.ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refresh_token"];
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized(new { message = "No refresh token found." });

            try
            {
                var result = await _accountService.RefreshToken(refreshToken);
                SetTokenCookies(result.AccessToken!, result.AccessTokenExpiresAt!.Value.ToUniversalTime(), result.RefreshToken!, result.RefreshTokenExpiresAt!.Value.ToUniversalTime());
                return Ok(MeResponse(result));
            }
            catch (UnauthorizedAccessException ex)
            {
                await _accountService.RevokeRefreshTokenByToken(refreshToken);
                ClearTokenCookies();
                return Unauthorized(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        {
            try
            {
                await _accountService.ForgotPassword(request.Email);
                return Ok(new { message = $"Reset code sent to {request.Email}." });
            }
            catch (System.ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
        {
            try
            {
                await _accountService.ResetPassword(
                    request.Email,
                    request.VerificationCode,
                    request.NewPassword,
                    request.ConfirmPassword);
                return Ok(new { message = "Password reset successfully." });
            }
            catch (System.ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
