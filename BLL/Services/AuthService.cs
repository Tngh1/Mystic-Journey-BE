using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
        private readonly IPlayerProfileService _playerProfileService;

        private const string OTP_CACHE_PREFIX = "otp:";
        private const string VERIFIED_CACHE_PREFIX = "verified:";
        private const string DefaultMapName = "ElfForest";
        private const double DefaultSpawnX = 11.9;
        private const double DefaultSpawnY = 17.8;

        private int OtpExpiryMinutes => int.Parse(_configuration["TokenSettings:OtpExpiryMinutes"] ?? "5");
        private int VerifiedExpiryMinutes => int.Parse(_configuration["TokenSettings:VerifiedExpiryMinutes"] ?? "30");
        private int AccessTokenExpiryMinutes => int.Parse(_configuration["TokenSettings:AccessTokenExpiryMinutes"] ?? _configuration["Jwt:AccessTokenExpiryMinutes"] ?? "30");
        private int RefreshTokenExpiryDays => int.Parse(_configuration["TokenSettings:RefreshTokenExpiryDays"] ?? _configuration["Jwt:RefreshTokenExpireDays"] ?? "7");

        // Client game heartbeat mỗi 30 giây => 90 giây là 3 nhịp bị lỡ. Đủ rộng để không đá
        // nhầm người đang chơi khi mạng chập chờn, đủ hẹp để client crash không khoá tài khoản lâu.
        private int GameSessionTimeoutSeconds => int.Parse(_configuration["TokenSettings:GameSessionTimeoutSeconds"] ?? "90");

        public AuthService(
            IAuthRepository repository,
            IMapper mapper,
            IConfiguration configuration,
            IMemoryCache cache,
            IPlayerProfileService playerProfileService)
        {
            _repository = repository;
            _mapper = mapper;
            _configuration = configuration;
            _cache = cache;
            _playerProfileService = playerProfileService;
        }

        private static string HashRefreshToken(string token)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(bytes);
        }

        public async Task<AuthResponseDto> Login(LoginRequestDto request)
        {
            var account = await _repository.GetAccountByUsernameOrEmail(request.EmailOrUsername.Trim());

            if (account == null)
                throw new AccountNotFoundException("Account is not registered. Please register before logging in.");

            if (!account.IsActive)
                throw new UnauthorizedAccessException("Account has been deactivated.");

            if (!await VerifyPasswordWithFallback(account, request.Password))
                throw new UnauthorizedAccessException("Invalid email/username or password.");

            // Gán SessionId mới cho MỌI đăng nhập để đảm bảo phiên đăng nhập cũ trên thiết bị trước
            // sẽ bị đè và đăng xuất ngay lập tức (Single Active Session).
            var sessionId = Guid.NewGuid().ToString();
            _cache.Set($"active_session:{account.AccountId}", sessionId, TimeSpan.FromDays(7));

            var (accessToken, accessExpiry) = GenerateAccessToken(account, sessionId);
            var (refreshToken, refreshExpiry) = GenerateRefreshToken();

            account.RefreshToken = HashRefreshToken(refreshToken);
            account.RefreshTokenExpiresAt = refreshExpiry;
            account.LastLogin = DateTime.UtcNow;
            if (IsGameClient(request.ClientType))
            {
                account.LastSeen = DateTime.UtcNow;
            }
            account.UpdatedAt = DateTime.UtcNow;
            if (account.PlayerProfile != null)
            {
                _playerProfileService.RecalculateEnergy(account.PlayerProfile);
            }
            await _repository.UpdateAccount(account);

            var hasCharacter = account.PlayerProfile != null;
            return new AuthResponseDto
            {
                AccountId = account.AccountId,
                UserName = account.UserName,
                EmailAddress = account.Email,
                RoleId = account.RoleId,
                Role = account.Role?.Name ?? "Player",
                HasCharacter = hasCharacter,
                PlayerProfileId = account.PlayerProfile?.PlayerProfileId,
                PlayerDisplayName = account.PlayerProfile?.DisplayName,
                PlayerClass = NormalizePlayerClass(account.PlayerProfile?.Class),
                Level = account.PlayerProfile?.Level ?? 1,
                LastMapName = NormalizeMapName(account.PlayerProfile?.LastMapName),
                PositionX = HasSavedPosition(account.PlayerProfile) ? account.PlayerProfile!.PositionX : DefaultSpawnX,
                PositionY = HasSavedPosition(account.PlayerProfile) ? account.PlayerProfile!.PositionY : DefaultSpawnY,
                AccessToken = accessToken,
                AccessTokenExpiresAt = accessExpiry,
                RefreshToken = refreshToken,
                RefreshTokenExpiresAt = refreshExpiry
            };
        }

        public async Task<AuthResponseDto> Register(RegisterRequestDto request)
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
                CreatedAt = DateTime.UtcNow,
                LastMapName = DefaultMapName,
                PositionX = DefaultSpawnX,
                PositionY = DefaultSpawnY
            };

            await _repository.CreateAccount(account);
            _cache.Remove(verifiedKey);

            var (accessToken, accessExpiry) = GenerateAccessToken(account);
            var (refreshToken, refreshExpiry) = GenerateRefreshToken();

            account.RefreshToken = HashRefreshToken(refreshToken);
            account.RefreshTokenExpiresAt = refreshExpiry;
            await _repository.UpdateAccount(account);

            var response = new AuthResponseDto
            {
                AccountId = account.AccountId,
                UserName = account.UserName,
                EmailAddress = account.Email,
                RoleId = account.RoleId,
                Role = account.Role?.Name ?? "Player",
                HasCharacter = true,
                PlayerProfileId = account.PlayerProfile.PlayerProfileId,
                PlayerDisplayName = account.PlayerProfile.DisplayName,
                PlayerClass = NormalizePlayerClass(account.PlayerProfile.Class),
                Level = account.PlayerProfile.Level,
                LastMapName = NormalizeMapName(account.PlayerProfile.LastMapName),
                PositionX = account.PlayerProfile.PositionX,
                PositionY = account.PlayerProfile.PositionY,
                AccessToken = accessToken,
                AccessTokenExpiresAt = accessExpiry,
                RefreshToken = refreshToken,
                RefreshTokenExpiresAt = refreshExpiry
            };
            return response;
        }

        public async Task<AuthResponseDto> ChangePassword(int accountId, ChangePasswordRequestDto request)
        {
            var account = await _repository.GetAccountById(accountId)
                ?? throw new KeyNotFoundException("Account not found.");

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, account.HashPassword))
                throw new UnauthorizedAccessException("Current password is incorrect.");

            account.HashPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            account.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAccount(account);

            var hasCharacter = account.PlayerProfile != null;
            return new AuthResponseDto
            {
                AccountId = account.AccountId,
                UserName = account.UserName,
                EmailAddress = account.Email,
                RoleId = account.RoleId,
                Role = account.Role?.Name ?? "Player",
                HasCharacter = hasCharacter,
                PlayerProfileId = account.PlayerProfile?.PlayerProfileId,
                PlayerDisplayName = account.PlayerProfile?.DisplayName,
                PlayerClass = NormalizePlayerClass(account.PlayerProfile?.Class),
                Level = account.PlayerProfile?.Level ?? 1,
                LastMapName = NormalizeMapName(account.PlayerProfile?.LastMapName),
                PositionX = HasSavedPosition(account.PlayerProfile) ? account.PlayerProfile!.PositionX : DefaultSpawnX,
                PositionY = HasSavedPosition(account.PlayerProfile) ? account.PlayerProfile!.PositionY : DefaultSpawnY
            };
        }

        public async Task<AuthResponseDto> RefreshToken(string refreshToken)
        {
            var hashed = HashRefreshToken(refreshToken);
            var account = await _repository.GetAccountByRefreshToken(hashed)
                ?? throw new UnauthorizedAccessException("Invalid refresh token.");

            if (!account.IsActive)
                throw new UnauthorizedAccessException("Account has been deactivated.");

            if (account.RefreshTokenExpiresAt == null || account.RefreshTokenExpiresAt < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Refresh token expired. Please login again.");

            var (accessToken, accessExpiry) = GenerateAccessToken(account);
            var (newRefreshToken, newRefreshExpiry) = GenerateRefreshToken();

            account.RefreshToken = HashRefreshToken(newRefreshToken);
            account.RefreshTokenExpiresAt = newRefreshExpiry;
            account.UpdatedAt = DateTime.UtcNow;
            if (account.PlayerProfile != null)
            {
                _playerProfileService.RecalculateEnergy(account.PlayerProfile);
            }
            await _repository.UpdateAccount(account);

            var hasCharacter = account.PlayerProfile != null;
            return new AuthResponseDto
            {
                AccountId = account.AccountId,
                UserName = account.UserName,
                EmailAddress = account.Email,
                RoleId = account.RoleId,
                Role = account.Role?.Name ?? "Player",
                HasCharacter = hasCharacter,
                PlayerProfileId = account.PlayerProfile?.PlayerProfileId,
                PlayerDisplayName = account.PlayerProfile?.DisplayName,
                PlayerClass = NormalizePlayerClass(account.PlayerProfile?.Class),
                Level = account.PlayerProfile?.Level ?? 1,
                LastMapName = NormalizeMapName(account.PlayerProfile?.LastMapName),
                PositionX = HasSavedPosition(account.PlayerProfile) ? account.PlayerProfile!.PositionX : DefaultSpawnX,
                PositionY = HasSavedPosition(account.PlayerProfile) ? account.PlayerProfile!.PositionY : DefaultSpawnY,
                AccessToken = accessToken,
                AccessTokenExpiresAt = accessExpiry,
                RefreshToken = newRefreshToken,
                RefreshTokenExpiresAt = newRefreshExpiry
            };
        }

        public async Task RevokeRefreshToken(int accountId)
        {
            await _repository.RevokeRefreshToken(accountId);
            await _repository.ClearLastSeen(accountId);
            _cache.Remove($"active_session:{accountId}");
        }

        public async Task RevokeRefreshTokenByToken(string refreshToken)
        {
            var hashed = HashRefreshToken(refreshToken);
            await _repository.RevokeRefreshTokenByToken(hashed);
        }

        public async Task<MeResponseDto> GetMe(int accountId)
        {
            var account = await _repository.GetAccountById(accountId)
                ?? throw new KeyNotFoundException("Account not found.");

            if (account.PlayerProfile != null)
            {
                if (_playerProfileService.RecalculateEnergy(account.PlayerProfile))
                {
                    await _repository.UpdateAccount(account);
                }
            }

            return new MeResponseDto
            {
                AccountId = account.AccountId,
                UserName = account.UserName,
                Email = account.Email,
                Role = account.Role?.Name ?? "Player",
                PlayerProfileId = account.PlayerProfile?.PlayerProfileId,
                PlayerClass = NormalizePlayerClass(account.PlayerProfile?.Class),
                Level = account.PlayerProfile?.Level ?? 1,
                LastMapName = NormalizeMapName(account.PlayerProfile?.LastMapName),
                PositionX = HasSavedPosition(account.PlayerProfile) ? account.PlayerProfile!.PositionX : DefaultSpawnX,
                PositionY = HasSavedPosition(account.PlayerProfile) ? account.PlayerProfile!.PositionY : DefaultSpawnY
            };
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

        private (string token, DateTime expires) GenerateAccessToken(Account account, string? sessionId = null)
        {
            var jwt = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(AccessTokenExpiryMinutes);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, account.AccountId.ToString()),
                new Claim(ClaimTypes.Name, account.UserName),
                new Claim(ClaimTypes.Email, account.Email),
                new Claim(ClaimTypes.Role, account.Role?.Name ?? "Player"),
                new Claim("playerProfileId", account.PlayerProfile?.PlayerProfileId.ToString() ?? "0")
            };

            if (!string.IsNullOrEmpty(sessionId))
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Sid, sessionId));
            }

            var token = new JwtSecurityToken(
                jwt["Issuer"], jwt["Audience"], claims,
                expires: expires, signingCredentials: creds);

            return (new JwtSecurityTokenHandler().WriteToken(token), expires);
        }

        private static string NormalizeMapName(string? mapName)
        {
            if (string.IsNullOrWhiteSpace(mapName))
                return DefaultMapName;

            var normalized = mapName.Trim();
            return IsDefaultMapAlias(normalized) ? DefaultMapName : normalized;
        }

        private static bool IsDefaultMapAlias(string mapName)
        {
            return string.Equals(mapName, "ElfForest", StringComparison.OrdinalIgnoreCase)
                || string.Equals(mapName, "ElfLand", StringComparison.OrdinalIgnoreCase)
                || string.Equals(mapName, "Map1", StringComparison.OrdinalIgnoreCase)
                || string.Equals(mapName, "Chapter1", StringComparison.OrdinalIgnoreCase)
                || string.Equals(mapName, "Chapter 1", StringComparison.OrdinalIgnoreCase)
                || string.Equals(mapName, DefaultMapName, StringComparison.OrdinalIgnoreCase);
        }

        private static string? NormalizePlayerClass(string? playerClass)
            => string.IsNullOrWhiteSpace(playerClass) ? null : playerClass.Trim();

        private static bool HasSavedPosition(PlayerProfile? profile)
        {
            if (profile == null)
                return false;

            var hasMap = !string.IsNullOrWhiteSpace(profile.LastMapName);
            if (!hasMap)
                return false;

            var hasPosition = Math.Abs(profile.PositionX) > double.Epsilon ||
                              Math.Abs(profile.PositionY) > double.Epsilon;

            return hasPosition || profile.Level > 1;
        }

        private static bool IsGameClient(string? clientType)
            => string.Equals(clientType?.Trim(), "Game", StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// LastSeen chỉ được cập nhật bởi heartbeat của client game (và lúc login game), nên
        /// LastSeen còn trong ngưỡng timeout nghĩa là đang có một phiên game sống.
        /// </summary>
        private bool IsGameSessionActive(DateTime? lastSeen)
        {
            if (!lastSeen.HasValue)
                return false;

            return lastSeen.Value >= DateTime.UtcNow.AddSeconds(-GameSessionTimeoutSeconds);
        }

        private async Task<bool> VerifyPasswordWithFallback(Account account, string password)
        {
            try
            {
                if (BCrypt.Net.BCrypt.Verify(password, account.HashPassword))
                    return true;
            }
            catch (BCrypt.Net.SaltParseException)
            {
            }

            try
            {
                var sha = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
                if (string.Equals(sha, account.HashPassword, StringComparison.Ordinal))
                {
                    account.HashPassword = BCrypt.Net.BCrypt.HashPassword(password);
                    account.UpdatedAt = DateTime.UtcNow;
                    await _repository.UpdateAccount(account);
                    return true;
                }
            }
            catch { }

            return false;
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

    public class AccountNotFoundException : Exception
    {
        public AccountNotFoundException(string message) : base(message) { }
    }

    /// <summary>
    /// Tài khoản đang được dùng để chơi ở một client game khác. Ánh xạ sang 409 Conflict để
    /// client phân biệt được với 401 (sai mật khẩu) và hiển thị đúng thông báo.
    /// </summary>
    public class AccountInUseException : Exception
    {
        public AccountInUseException(string message) : base(message) { }
    }
}
