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
    // Executes controller base operation.
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        // Initializes a new instance of AuthController with dependencies: authService, configuration.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        // Compares the client semantic version against the minimum required server version.
        // Returns true if the client version is strictly lower and requires an update.
        private static bool IsVersionLower(string clientVer, string minVer)
        {
            if (Version.TryParse(clientVer, out var parsedClient) && Version.TryParse(minVer, out var parsedMin))
            {
                return parsedClient < parsedMin;
            }
            return string.Compare(clientVer, minVer, StringComparison.OrdinalIgnoreCase) < 0;
        }

        // Extracts and parses the integer account ID from the JWT NameIdentifier claim.
        // Throws UnauthorizedAccessException if the claim is missing, empty, or not a valid integer.
        private int GetCurrentAccountId()  // Extract authenticated caller's account ID from JWT
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);  // Read caller account ID from JWT NameIdentifier claim
            if (!int.TryParse(claim, out var accountId))  // Claim value missing or non-integer — reject as unauthorized
                throw new UnauthorizedAccessException("Invalid authentication token.");  // Authentication token is invalid or expired
            return accountId;
        }

        // Retrieves the client type (Web or Game) from the caller's JWT claims.
        // Defaults to 'Web' if no client type claim is present.
        private string GetCurrentClientType()  // Determine whether the caller is the Web or Game client
            => User.FindFirstValue(AuthService.ClientTypeClaim) ?? AuthService.ClientWeb;  // Read client type (Web / Game) from JWT claim

        // Constructs secure cross-site HTTP-only CookieOptions with the specified expiration date.
        // Enforces SameSite=None and Secure=true for cross-origin cookie sharing between Web and API.
        private CookieOptions BuildCookieOptions(DateTime expiry) => new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/",
            Expires = expiry
        };

        // Appends secure HTTP-only access_token and refresh_token cookies to the HTTP response.
        // Configures UTC expiration timestamps for both authentication cookies.
        private void SetTokenCookies(string accessToken, DateTime accessExpiry, string refreshToken, DateTime refreshExpiry)  // Write new access + refresh tokens into secure HTTP-only cookies
        {
            Response.Cookies.Append("access_token", accessToken, BuildCookieOptions(accessExpiry.ToUniversalTime()));
            Response.Cookies.Append("refresh_token", refreshToken, BuildCookieOptions(refreshExpiry.ToUniversalTime()));
        }

        // Removes access_token and refresh_token cookies from the client browser by setting expired cookies.
        private void ClearTokenCookies()  // Remove both JWT cookies to end the browser session
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


        // ─── Guest APIs ───────────────────────────────────────────────────────
        [AllowAnonymous]
        [HttpPost("login")]
        // Validate the login payload and client version, authenticate the account, issue access and refresh tokens, persist the correct session slot, and return the authenticated account data.
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)  // Reject request immediately if any DTO validation annotation fails
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });  // Return HTTP 400 with validation error details

            var clientVer = request.ClientVersion ?? Request.Headers["X-Client-Version"].FirstOrDefault();
            if (!string.IsNullOrEmpty(clientVer))
            {
                var minVer = _configuration["GameVersion:MinRequiredVersion"] ?? "1.0.0";
                if (IsVersionLower(clientVer, minVer))  // Client is below the minimum required version — block login
                {
                    var downloadUrl = _configuration["GameVersion:DownloadUrl"] ?? "";
                    return StatusCode(426, new ApiResponse<object>  // HTTP 426 Upgrade Required: force the client to update
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

            var result = await _authService.Login(request);  // Authenticate credentials and generate access/refresh token pair
            SetTokenCookies(result.AccessToken!, result.AccessTokenExpiresAt!.Value, result.RefreshToken!, result.RefreshTokenExpiresAt!.Value);  // Write new access + refresh tokens into secure HTTP-only cookies
            return Ok(new ApiResponse<AuthResponseDto> { Success = true, Data = result });  // Return HTTP 200 with standard ApiResponse envelope
        }

        [AllowAnonymous]
        [HttpPost("register")]
        // Validate the registration payload, create the account and initial profile, issue session tokens, and return the authenticated registration result.
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var result = await _authService.Register(request);  // Create account, initialize player profile, and issue tokens
            SetTokenCookies(result.AccessToken!, result.AccessTokenExpiresAt!.Value, result.RefreshToken!, result.RefreshTokenExpiresAt!.Value);  // Write new access + refresh tokens into secure HTTP-only cookies
            return Ok(new ApiResponse<AuthResponseDto> { Success = true, Data = result });  // Return HTTP 200 with standard ApiResponse envelope
        }

        // ─── Player APIs ───────────────────────────────────────────────────────
        [Authorize]
        [HttpGet("me")]
        // Retrieves account profile, selected character class, and spawn coordinates for the authenticated user.
        public async Task<IActionResult> Me()
        {
            var accountId = GetCurrentAccountId();  // Extract authenticated caller's account ID from JWT
            var result = await _authService.GetMe(accountId);  // Load complete account profile for the authenticated user
            return Ok(new ApiResponse<MeResponseDto> { Success = true, Data = result });  // Return HTTP 200 with standard ApiResponse envelope
        }

        [Authorize]
        [HttpPost("change-password")]
        // Validates current password and updates the account with the new hashed password.
        // Rotates session tokens, revokes old refresh tokens, and refreshes authentication cookies.
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
        {
            var accountId = GetCurrentAccountId();  // Extract authenticated caller's account ID from JWT
            var result = await _authService.ChangePassword(accountId, request, GetCurrentClientType());  // Determine whether the caller is the Web or Game client
            SetTokenCookies(result.AccessToken!, result.AccessTokenExpiresAt!.Value, result.RefreshToken!, result.RefreshTokenExpiresAt!.Value);  // Write new access + refresh tokens into secure HTTP-only cookies
            return Ok(new ApiResponse<object> { Success = true, Message = "Password changed successfully." });  // Return HTTP 200 with standard ApiResponse envelope
        }

        [Authorize]
        [HttpPost("logout")]
        // Revokes active refresh token for the calling client type and clears authentication session cookies.
        public async Task<IActionResult> Logout()
        {
            var accountId = GetCurrentAccountId();  // Extract authenticated caller's account ID from JWT
            await _authService.RevokeRefreshToken(accountId, GetCurrentClientType());  // Determine whether the caller is the Web or Game client
            ClearTokenCookies();  // Remove both JWT cookies to end the browser session
            return Ok(new ApiResponse<object> { Success = true, Message = "Logged out successfully." });  // Return HTTP 200 with standard ApiResponse envelope
        }

        // ─── Guest APIs ───────────────────────────────────────────────────────
        [AllowAnonymous]
        [HttpPost("refresh-token")]
        // Read and validate the refresh token, rotate both tokens on success, and revoke the stored token plus local cookies when rotation fails.
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refresh_token"];
            if (string.IsNullOrEmpty(refreshToken))  // Mandatory string argument is null or empty — fail fast
                return Unauthorized(new ApiResponse<object> { Success = false, Message = "No refresh token found.", ErrorCode = ErrorCodes.Unauthorized });

            try
            {
                var result = await _authService.RefreshToken(refreshToken);
                SetTokenCookies(result.AccessToken!, result.AccessTokenExpiresAt!.Value, result.RefreshToken!, result.RefreshTokenExpiresAt!.Value);  // Write new access + refresh tokens into secure HTTP-only cookies
                return Ok(new ApiResponse<AuthResponseDto> { Success = true, Data = result });  // Return HTTP 200 with standard ApiResponse envelope
            }
            catch (UnauthorizedAccessException ex)
            {
                await _authService.RevokeRefreshTokenByToken(refreshToken);
                ClearTokenCookies();  // Remove both JWT cookies to end the browser session
                return Unauthorized(new ApiResponse<object> { Success = false, Message = ex.Message, ErrorCode = ErrorCodes.Unauthorized });
            }
        }

        [AllowAnonymous]
        [HttpPost("forget-password")]
        // Generates a 6-digit OTP verification code, caches it in Redis with a TTL, and emails it to the user.
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequestDto request)
        {
            await _authService.ForgetPassword(request.Email);  // Generate OTP and dispatch password-reset email
            return Ok(new ApiResponse<object> { Success = true, Message = $"Reset code sent to {request.Email}." });  // Return HTTP 200 with standard ApiResponse envelope
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        // Verifies the submitted OTP code, updates the account password hash, and revokes all active sessions.
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
        {
            await _authService.ResetPassword(  // Verify OTP and persist the new hashed password
                request.Email,
                request.VerificationCode,
                request.NewPassword,
                request.ConfirmPassword);
            return Ok(new ApiResponse<object> { Success = true, Message = "Password reset successfully." });  // Return HTTP 200 with standard ApiResponse envelope
        }

        [AllowAnonymous]
        [HttpPost("send-verification-code")]
        // Sends a 6-digit email verification code for new user registration and stores it in Redis cache.
        public async Task<IActionResult> SendVerificationCode([FromBody] SendVerificationCodeRequestDto request)
        {
            await _authService.SendVerificationCode(request.Email);  // Generate and email a 6-digit registration verification code
            return Ok(new ApiResponse<object> { Success = true, Message = $"Verification code sent to {request.Email}." });  // Return HTTP 200 with standard ApiResponse envelope
        }

        [AllowAnonymous]
        [HttpPost("verify-email")]
        // Validates the registration OTP verification code against Redis cache and marks email as verified.
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequestDto request)
        {
            await _authService.VerifyEmail(request);  // Confirm OTP and mark email address as verified in cache
            return Ok(new ApiResponse<object> { Success = true, Message = "Email verified successfully." });  // Return HTTP 200 with standard ApiResponse envelope
        }
    }
}
