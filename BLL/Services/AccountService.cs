using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _repository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;

        private const string OTP_CACHE_PREFIX = "otp:";
        private const string VERIFIED_CACHE_PREFIX = "verified:";

        private int OtpExpiryMinutes => int.Parse(_configuration["TokenSettings:OtpExpiryMinutes"] ?? "5");
        private int VerifiedExpiryMinutes => int.Parse(_configuration["TokenSettings:VerifiedExpiryMinutes"] ?? "30");
        private int AccessTokenExpiryMinutes => int.Parse(_configuration["TokenSettings:AccessTokenExpiryMinutes"] ?? _configuration["Jwt:AccessTokenExpiryMinutes"] ?? "30");
        private int RefreshTokenExpiryDays => int.Parse(_configuration["TokenSettings:RefreshTokenExpiryDays"] ?? _configuration["Jwt:RefreshTokenExpireDays"] ?? "7");

        public AccountService(
            IAccountRepository repository,
            IMapper mapper,
            IConfiguration configuration,
            IMemoryCache cache)
        {
            _repository = repository;
            _mapper = mapper;
            _configuration = configuration;
            _cache = cache;
        }

        public async Task<AccountResponseDto> LoginAccount(LoginRequestDto request)
        {
            var account = await _repository.GetAccountByUsernameOrEmail(request.EmailOrUsername.Trim())
                ?? throw new UnauthorizedAccessException("Invalid email/username or password.");

            if (!account.IsActive)
                throw new UnauthorizedAccessException("Account has been deactivated.");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, account.HashPassword))
                throw new UnauthorizedAccessException("Invalid email/username or password.");

            var (accessToken, accessExpiry) = GenerateAccessToken(account);
            var (refreshToken, refreshExpiry) = GenerateRefreshToken();

            account.RefreshToken = refreshToken;
            account.RefreshTokenExpiresAt = refreshExpiry;
            account.LastLogin = DateTime.UtcNow;
            account.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAccount(account);

            var response = _mapper.Map<AccountResponseDto>(account);
            response.AccessToken = accessToken;
            response.AccessTokenExpiresAt = accessExpiry;
            response.RefreshToken = refreshToken;
            response.RefreshTokenExpiresAt = refreshExpiry;
            return response;
        }

        public async Task<LoginGameResponseDto> LoginGame(LoginGameRequestDto request)
        {
            var account = await _repository.GetAccountByUsernameOrEmail(request.EmailOrUsername.Trim())
                ?? throw new UnauthorizedAccessException("Invalid email/username or password.");

            if (!account.IsActive)
                throw new UnauthorizedAccessException("Account has been deactivated.");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, account.HashPassword))
                throw new UnauthorizedAccessException("Invalid email/username or password.");

            var (accessToken, accessExpiry) = GenerateAccessToken(account);
            var (refreshToken, refreshExpiry) = GenerateRefreshToken();

            account.RefreshToken = refreshToken;
            account.RefreshTokenExpiresAt = refreshExpiry;
            account.LastLogin = DateTime.UtcNow;
            account.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAccount(account);

            return new LoginGameResponseDto
            {
                AccountId = account.AccountId,
                UserName = account.UserName,
                EmailAddress = account.Email,
                RoleId = account.RoleId,
                PlayerProfileId = account.PlayerProfile?.PlayerProfileId,
                PlayerDisplayName = account.PlayerProfile?.DisplayName,
                AccessToken = accessToken,
                AccessTokenExpiresAt = accessExpiry,
                RefreshToken = refreshToken,
                RefreshTokenExpiresAt = refreshExpiry
            };
        }

        public async Task<AccountResponseDto> RegisterAccount(RegisterRequestDto request)
        {
            var normalizedEmail = request.EmailAddress.Trim().ToLowerInvariant();
            var normalizedUsername = request.UserName.Trim();

            var verifiedKey = $"{VERIFIED_CACHE_PREFIX}{normalizedEmail}";
            if (!_cache.TryGetValue(verifiedKey, out bool isVerified) || !isVerified)
                throw new BadRequestException("Email not verified. Please verify your email first.");

            if (await _repository.IsEmailExist(normalizedEmail))
                throw new BadRequestException("Email already registered.");

            if (await _repository.IsUsernameExist(normalizedUsername))
                throw new BadRequestException("Username already taken.");

            var account = _mapper.Map<Account>(request);
            account.UserName = normalizedUsername;
            account.Email = normalizedEmail;
            account.HashPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            account.RoleId = 1;
            account.CreatedAt = DateTime.UtcNow;
            account.UpdatedAt = DateTime.UtcNow;
            account.IsActive = true;
            account.PlayerProfile = new PlayerProfile
            {
                DisplayName = normalizedUsername,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.CreateAccount(account);
            _cache.Remove(verifiedKey);

            var (accessToken, accessExpiry) = GenerateAccessToken(account);
            var (refreshToken, refreshExpiry) = GenerateRefreshToken();

            account.RefreshToken = refreshToken;
            account.RefreshTokenExpiresAt = refreshExpiry;
            await _repository.UpdateAccount(account);

            var response = _mapper.Map<AccountResponseDto>(account);
            response.AccessToken = accessToken;
            response.AccessTokenExpiresAt = accessExpiry;
            response.RefreshToken = refreshToken;
            response.RefreshTokenExpiresAt = refreshExpiry;
            return response;
        }

        public async Task<AccountResponseDto> ChangePassword(int accountId, ChangePasswordRequestDto request)
        {
            var account = await _repository.GetAccountById(accountId)
                ?? throw new KeyNotFoundException("Account not found.");

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, account.HashPassword))
                throw new UnauthorizedAccessException("Current password is incorrect.");

            account.HashPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            account.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAccount(account);

            return _mapper.Map<AccountResponseDto>(account);
        }

        public async Task<AccountResponseDto> RefreshToken(string refreshToken)
        {
            var account = await _repository.GetAccountByRefreshToken(refreshToken)
                ?? throw new UnauthorizedAccessException("Invalid refresh token.");

            if (!account.IsActive)
                throw new UnauthorizedAccessException("Account has been deactivated.");

            if (account.RefreshTokenExpiresAt == null || account.RefreshTokenExpiresAt < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Refresh token expired. Please login again.");

            var (accessToken, accessExpiry) = GenerateAccessToken(account);
            var (newRefreshToken, newRefreshExpiry) = GenerateRefreshToken();

            account.RefreshToken = newRefreshToken;
            account.RefreshTokenExpiresAt = newRefreshExpiry;
            account.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAccount(account);

            var response = _mapper.Map<AccountResponseDto>(account);
            response.AccessToken = accessToken;
            response.AccessTokenExpiresAt = accessExpiry;
            response.RefreshToken = newRefreshToken;
            response.RefreshTokenExpiresAt = newRefreshExpiry;
            return response;
        }

        public async Task SendVerificationCode(string email)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();

            if (await _repository.IsEmailExist(normalizedEmail))
                throw new BadRequestException("Email already registered.");

            var otp = GenerateVerificationCode();
            var cacheKey = $"{OTP_CACHE_PREFIX}{normalizedEmail}";

            _cache.Set(cacheKey, otp, TimeSpan.FromMinutes(OtpExpiryMinutes));

            var sent = await SendEmailAsync(
                normalizedEmail,
                "Mystic Journey - Email Verification",
                $"Your verification code is: {otp}\n\nThis code will expire in {OtpExpiryMinutes} minutes.\n\nIf you did not request this code, please ignore this email.");

            if (!sent)
                throw new InvalidOperationException("Failed to send verification email.");
        }

        public async Task VerifyEmail(VerifyEmailRequestDto request)
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var cacheKey = $"{OTP_CACHE_PREFIX}{normalizedEmail}";

            if (!_cache.TryGetValue(cacheKey, out string? cachedOtp))
                throw new BadRequestException("No verification code found. Please request a new one.");

            if (cachedOtp != request.VerificationCode)
                throw new BadRequestException("Invalid verification code.");

            _cache.Remove(cacheKey);

            var verifiedKey = $"{VERIFIED_CACHE_PREFIX}{normalizedEmail}";
            _cache.Set(verifiedKey, true, TimeSpan.FromMinutes(VerifiedExpiryMinutes));
        }

        private static string GenerateVerificationCode()
            => RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");

        private static string GenerateRandomToken(int byteLength = 64)
        {
            var bytes = new byte[byteLength];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        private (string token, DateTime expires) GenerateAccessToken(Account account)
        {
            var jwt = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(AccessTokenExpiryMinutes);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, account.AccountId.ToString()),
                new Claim(ClaimTypes.Name, account.UserName),
                new Claim(ClaimTypes.Email, account.Email),
                new Claim(ClaimTypes.Role, account.Role?.Name ?? "Player")
            };

            var token = new JwtSecurityToken(
                jwt["Issuer"], jwt["Audience"], claims,
                expires: expires, signingCredentials: creds);

            return (new JwtSecurityTokenHandler().WriteToken(token), expires);
        }

        private (string token, DateTime expires) GenerateRefreshToken()
            => (GenerateRandomToken(64), DateTime.UtcNow.AddDays(RefreshTokenExpiryDays));

        private async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var smtp = _configuration.GetSection("Smtp");
                using var client = new SmtpClient(smtp["Host"], int.Parse(smtp["Port"]))
                {
                    Credentials = new NetworkCredential(smtp["Username"], smtp["Password"]),
                    EnableSsl = bool.Parse(smtp["UseSSL"])
                };
                var mail = new MailMessage(smtp["FromEmail"], to, subject, body);
                await client.SendMailAsync(mail);
                return true;
            }
            catch { return false; }
        }

        public async Task ForgotPassword(string email)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();

            if (!await _repository.IsEmailExist(normalizedEmail))
                throw new BadRequestException("Email not registered.");

            var otp = GenerateVerificationCode();
            var cacheKey = $"{OTP_CACHE_PREFIX}{normalizedEmail}";

            _cache.Set(cacheKey, otp, TimeSpan.FromMinutes(OtpExpiryMinutes));

            var sent = await SendEmailAsync(
                normalizedEmail,
                "Mystic Journey - Password Reset",
                $"Your password reset code is: {otp}\n\nThis code will expire in {OtpExpiryMinutes} minutes.\n\nIf you did not request this, please ignore this email.");

            if (!sent)
                throw new InvalidOperationException("Failed to send reset email.");
        }

        public async Task ResetPassword(string email, string verificationCode, string newPassword, string confirmPassword)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();

            if (!string.Equals(newPassword, confirmPassword, StringComparison.Ordinal))
                throw new BadRequestException("Passwords do not match.");

            var cacheKey = $"{OTP_CACHE_PREFIX}{normalizedEmail}";
            if (!_cache.TryGetValue(cacheKey, out string? cachedOtp) || cachedOtp != verificationCode)
                throw new BadRequestException("Invalid or expired verification code.");

            var account = await _repository.GetAccountByEmail(normalizedEmail)
                ?? throw new KeyNotFoundException("Account not found.");

            account.HashPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
            account.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAccount(account);

            _cache.Remove(cacheKey);
        }
    }

    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message) { }
    }
}
