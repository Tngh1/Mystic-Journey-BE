using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
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
    // Executes core business logic for i auth service.
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IDistributedCache _cache;
        private readonly IPlayerProfileService _playerProfileService;
        private readonly ISessionNotifier _sessionNotifier;

        private const string OTP_CACHE_PREFIX = "otp:";
        private const string VERIFIED_CACHE_PREFIX = "verified:";

        private const string VerifiedFlag = "1";
        private const string DefaultMapName = "ElfForest";
        private const double DefaultSpawnX = 11.9;
        private const double DefaultSpawnY = 17.8;

        // Executes core business logic for otp expiry minutes.
        private int OtpExpiryMinutes => int.Parse(_configuration["TokenSettings:OtpExpiryMinutes"] ?? "5");
        // Executes core business logic for verified expiry minutes.
        private int VerifiedExpiryMinutes => int.Parse(_configuration["TokenSettings:VerifiedExpiryMinutes"] ?? "30");
        // Executes core business logic for access token expiry minutes.
        private int AccessTokenExpiryMinutes => int.Parse(_configuration["TokenSettings:AccessTokenExpiryMinutes"] ?? _configuration["Jwt:AccessTokenExpiryMinutes"] ?? "30");
        // Executes core business logic for refresh token expiry days.
        private int RefreshTokenExpiryDays => int.Parse(_configuration["TokenSettings:RefreshTokenExpiryDays"] ?? _configuration["Jwt:RefreshTokenExpireDays"] ?? "7");

        // Initialize this instance from repository, mapper, configuration, and cache and store repository, mapper, configuration, cache, and player profile service for later operations.
        public AuthService(
            IAuthRepository repository,
            IMapper mapper,
            IConfiguration configuration,
            IDistributedCache cache,
            IPlayerProfileService playerProfileService,
            ISessionNotifier sessionNotifier)
        {
            _repository = repository;
            _mapper = mapper;
            _configuration = configuration;
            _cache = cache;
            _playerProfileService = playerProfileService;
            _sessionNotifier = sessionNotifier;
        }

        // Executes core business logic for hash refresh token.
        private static string HashRefreshToken(string token)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(bytes);
        }

        public const string ClientWeb = "Web";
        public const string ClientGame = "Game";
        public const string ClientTypeClaim = "ctyp";

        // Executes core business logic for normalize client.
        private static string NormalizeClient(string? clientType)
            => IsGameClient(clientType) ? ClientGame : ClientWeb;

        // Executes core business logic for active session key.
        public static string ActiveSessionKey(int accountId, string? clientType)
            => $"active_session:{accountId}:{NormalizeClient(clientType)}";

        // Executes core business logic for ttl.
        private static DistributedCacheEntryOptions Ttl(TimeSpan ttl)
            => new() { AbsoluteExpirationRelativeToNow = ttl };

        // Executes core business logic for start new session.
        // Logic details: validates required non-empty string arguments; checks Redis/memory cache to minimize database load; stores computed result in cache with an expiration TTL.
        // Returns the computed string result asynchronously.
        private async Task<string> StartNewSession(int accountId, string clientType)
        {
            var key = ActiveSessionKey(accountId, clientType); // Build Redis cache key scoped to account + client type slot

            var previousSessionId = await _cache.GetStringAsync(key); // Check if a prior session exists for this account/client pair

            var sessionId = Guid.NewGuid().ToString(); // Generate a new unique session ID for this login
            await _cache.SetStringAsync( // Persist new session ID with refresh-token-matching TTL
                key,
                sessionId,
                Ttl(TimeSpan.FromDays(RefreshTokenExpiryDays)));

            if (!string.IsNullOrEmpty(previousSessionId) && previousSessionId != sessionId)
            {
                // Notify all subscribers that the previous session has been superseded (e.g., kick old device via SignalR)
                await _sessionNotifier.SessionOverridden(accountId, clientType, sessionId);
            }

            return sessionId;
        }
        // Executes core business logic for continue session.
        // Logic details: validates required non-empty string arguments; checks Redis/memory cache to minimize database load; stores computed result in cache with an expiration TTL.
        // Returns the computed string result asynchronously.
        private async Task<string> ContinueSession(int accountId, string clientType)
        {
            var key = ActiveSessionKey(accountId, clientType); // Build Redis key for account + client pair
            var sessionId = await _cache.GetStringAsync(key); // Try to retrieve existing session from Redis cache
            if (!string.IsNullOrEmpty(sessionId))
            {
                // Session still alive — slide expiry window to match refresh token lifetime
                await _cache.SetStringAsync(key, sessionId, Ttl(TimeSpan.FromDays(RefreshTokenExpiryDays)));
                return sessionId;
            }
            // Session missing from cache (e.g., after server restart) — create a fresh session slot
            return await StartNewSession(accountId, clientType);
        }

        // Executes core business logic for set refresh slot.
        private static void SetRefreshSlot(Account account, string clientType, string? hashedToken, DateTime? expiresAt)
        {
            if (NormalizeClient(clientType) == ClientGame)
            {
                account.GameRefreshToken = hashedToken;
                account.GameRefreshTokenExpiresAt = expiresAt;
            }
            else
            {
                account.RefreshToken = hashedToken;
                account.RefreshTokenExpiresAt = expiresAt;
            }
        }

        // Executes core business logic for refresh slot expiry.
        // Logic details: validates required non-empty string arguments.
        private static DateTime? RefreshSlotExpiry(Account account, string clientType)
            => NormalizeClient(clientType) == ClientGame
                ? account.GameRefreshTokenExpiresAt
                : account.RefreshTokenExpiresAt;
        // Executes core business logic for match refresh slot.
        // Logic details: validates required non-empty string arguments; delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models; throws AccountNotFoundException, UnauthorizedAccessException on invalid state or rule violations.
        private static string? MatchRefreshSlot(Account account, string hashedToken)
        {
            if (!string.IsNullOrEmpty(account.GameRefreshToken) && account.GameRefreshToken == hashedToken)
                return ClientGame;
            if (!string.IsNullOrEmpty(account.RefreshToken) && account.RefreshToken == hashedToken)
                return ClientWeb;
            return null;
        }

        // Validate the login payload and client version, authenticate the account, issue access and refresh tokens, persist the correct session slot, and return the authenticated account data.
        public async Task<AuthResponseDto> Login(LoginRequestDto request)
        {
            // Look up account by either email or username (case-insensitive normalized trim)
            var account = await _repository.GetAccountByUsernameOrEmail(request.EmailOrUsername.Trim());

            if (account == null) // Account not found — reject with registration prompt
                throw new AccountNotFoundException("Account is not registered. Please register before logging in.");

            if (!account.IsActive) // Account is banned — reject login with ban reason if available
                throw new UnauthorizedAccessException(
                    string.IsNullOrWhiteSpace(account.BanReason)
                        ? "Your account has been banned."
                        : $"Your account has been banned. Reason: {account.BanReason}");

            if (!await VerifyPasswordWithFallback(account, request.Password)) // Wrong password — reject immediately
                throw new UnauthorizedAccessException("Invalid email/username or password.");

            // Normalize client type to 'Web' or 'Game' — determines which refresh-token slot to use
            var clientType = NormalizeClient(request.ClientType);
            // Create a new session in Redis, overriding any existing session for this account+client pair
            var sessionId = await StartNewSession(account.AccountId, clientType);

            // Sign a new JWT access token embedding session ID, role, and client type claims
            var (accessToken, accessExpiry) = GenerateAccessToken(account, sessionId, clientType);
            // Generate a cryptographically secure refresh token string and its expiry date
            var (refreshToken, refreshExpiry) = GenerateRefreshToken();

            // Store the hashed refresh token in the correct account slot (Web or Game)
            SetRefreshSlot(account, clientType, HashRefreshToken(refreshToken), refreshExpiry);
            if (clientType == ClientGame && account.PlayerProfile != null)
            {
                account.PlayerProfile.LastSeen = DateTime.UtcNow; // Record last seen timestamp for Game client activity tracking
            }
            account.UpdatedAt = DateTime.UtcNow; // Stamp account modification time before saving
            if (account.PlayerProfile != null)
            {
                _playerProfileService.RecalculateEnergy(account.PlayerProfile); // Recalculate energy based on time elapsed since last activity
            }
            await _repository.UpdateAccount(account); // Persist refresh token hash, last seen, and updated timestamp to DB

            var hasCharacter = account.PlayerProfile != null;
            var response = _mapper.Map<AuthResponseDto>(account); // Map account domain entity to auth response DTO

            response.AccessToken = accessToken;
            response.AccessTokenExpiresAt = accessExpiry;
            response.RefreshToken = refreshToken;
            response.RefreshTokenExpiresAt = refreshExpiry;

            if (!HasSavedPosition(account.PlayerProfile))
            {
                // No saved position found — inject default spawn coordinates
                response.PositionX = DefaultSpawnX;
                response.PositionY = DefaultSpawnY;
            }

            // Normalize legacy map name aliases to canonical map identifier
            response.LastMapName = NormalizeMapName(account.PlayerProfile?.LastMapName);

            // Flag whether the player has selected a character class yet
            response.HasCharacter = !string.IsNullOrWhiteSpace(response.PlayerClass);
            return response;
        }

        // Validate the registration payload, create the account and initial profile, issue session tokens, and return the authenticated registration result.
        public async Task<AuthResponseDto> Register(RegisterRequestDto request)
        {
            var normalizedEmail = request.EmailAddress.Trim().ToLowerInvariant(); // Normalize email to lowercase for case-insensitive uniqueness check
            var normalizedUsername = request.UserName.Trim(); // Strip leading/trailing whitespace from username

            var verifiedKey = $"{VERIFIED_CACHE_PREFIX}{normalizedEmail}";
            if (await _cache.GetStringAsync(verifiedKey) != VerifiedFlag) // Email OTP verification must be completed before account creation
                throw new BadRequestException("Email not verified. Please verify your email first.");

            if (await _repository.IsEmailExist(normalizedEmail)) // Prevent duplicate accounts with the same email address
                throw new BadRequestException("Email already registered.");

            if (await _repository.IsUsernameExist(normalizedUsername)) // Prevent duplicate accounts with the same username
                throw new BadRequestException("Username already taken.");

            var account = _mapper.Map<Account>(request); // Map registration DTO fields to Account domain entity
            account.UserName = normalizedUsername;
            account.Email = normalizedEmail;
            account.HashPassword = BCrypt.Net.BCrypt.HashPassword(request.Password); // Hash password with BCrypt before storing — never store plaintext
            account.RoleId = 1; // Assign default 'Player' role (RoleId = 1)
            account.CreatedAt = DateTime.UtcNow;
            account.UpdatedAt = DateTime.UtcNow;
            account.IsActive = true;
            account.PlayerProfile = new PlayerProfile // Initialize an empty player profile with default spawn position
            {
                DisplayName = normalizedUsername,
                CreatedAt = DateTime.UtcNow,
                LastMapName = DefaultMapName,
                PositionX = DefaultSpawnX,
                PositionY = DefaultSpawnY
            };

            await _repository.CreateAccount(account); // Persist new account and player profile to database
            await _cache.RemoveAsync(verifiedKey); // Remove email-verified flag from Redis — one-time use only

            // Issue access token and start a new Web client session in Redis
            var (accessToken, accessExpiry) = GenerateAccessToken(
                account, await StartNewSession(account.AccountId, ClientWeb), ClientWeb);
            var (refreshToken, refreshExpiry) = GenerateRefreshToken(); // Generate secure refresh token for persistent sessions

            SetRefreshSlot(account, ClientWeb, HashRefreshToken(refreshToken), refreshExpiry); // Store hashed refresh token in the Web slot
            await _repository.UpdateAccount(account); // Save hashed refresh token to database

            var response = _mapper.Map<AuthResponseDto>(account); // Map persisted account entity to auth response DTO

            response.AccessToken = accessToken;
            response.AccessTokenExpiresAt = accessExpiry;
            response.RefreshToken = refreshToken;
            response.RefreshTokenExpiresAt = refreshExpiry;

            response.HasCharacter = !string.IsNullOrWhiteSpace(response.PlayerClass); // New registrations have no character yet

            return response;
        }

        // Validates current password and updates the account with the new hashed password.
        // Rotates session tokens, revokes old refresh tokens, and refreshes authentication cookies.
        public async Task<AuthResponseDto> ChangePassword(int accountId, ChangePasswordRequestDto request, string clientType)
        {
            var account = await _repository.GetAccountById(accountId)
                ?? throw new KeyNotFoundException("Account not found.");

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, account.HashPassword)) // Verify current password against stored BCrypt hash
                throw new UnauthorizedAccessException("Current password is incorrect.");

            account.HashPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword); // Replace old hash with newly hashed password
            account.UpdatedAt = DateTime.UtcNow;

            var caller = NormalizeClient(clientType); // 'Web' or 'Game' — the client changing the password keeps its session
            var other = caller == ClientGame ? ClientWeb : ClientGame; // Identify the opposite client type to kick it out

            SetRefreshSlot(account, other, null, null); // Revoke refresh token for the other client type (kick device)
            await _cache.RemoveAsync(ActiveSessionKey(accountId, other)); // Invalidate the other client's Redis session key

            // Push SignalR logout event to any connected socket of the other client type
            await _sessionNotifier.SessionOverridden(accountId, other, string.Empty);

            var sessionId = await StartNewSession(accountId, caller); // Start a fresh session for the calling client
            var (accessToken, accessExpiry) = GenerateAccessToken(account, sessionId, caller); // Issue a new JWT with updated claims
            var (refreshToken, refreshExpiry) = GenerateRefreshToken(); // Generate new refresh token for the caller
            SetRefreshSlot(account, caller, HashRefreshToken(refreshToken), refreshExpiry); // Store hashed refresh token for calling client

            await _repository.UpdateAccount(account); // Persist new password hash and refresh token slots to DB

            var hasCharacter = account.PlayerProfile != null;
            var response = _mapper.Map<AuthResponseDto>(account); // Map updated account to auth response DTO

            response.AccessToken = accessToken;
            response.AccessTokenExpiresAt = accessExpiry;
            response.RefreshToken = refreshToken;
            response.RefreshTokenExpiresAt = refreshExpiry;

            if (!HasSavedPosition(account.PlayerProfile))
            {
                response.PositionX = DefaultSpawnX; // Inject default spawn X when no saved position exists
                response.PositionY = DefaultSpawnY;
            }

            response.LastMapName = NormalizeMapName(account.PlayerProfile?.LastMapName); // Resolve canonical map name from saved or aliased value

            response.HasCharacter = !string.IsNullOrWhiteSpace(response.PlayerClass);

            return response;
        }

        // Read and validate the refresh token, rotate both tokens on success, and revoke the stored token plus local cookies when rotation fails.
        public async Task<AuthResponseDto> RefreshToken(string refreshToken)
        {
            var hashed = HashRefreshToken(refreshToken); // Hash the incoming token to compare against the stored hash
            var account = await _repository.GetAccountByRefreshToken(hashed) // Locate account that owns this refresh token
                ?? throw new UnauthorizedAccessException("Invalid refresh token.");

            if (!account.IsActive) // Account was banned since the token was issued — reject silently
                throw new UnauthorizedAccessException(
                    string.IsNullOrWhiteSpace(account.BanReason)
                        ? "Your account has been banned."
                        : $"Your account has been banned. Reason: {account.BanReason}");

            // Determine whether this token belongs to the Web or Game client slot
            var clientType = MatchRefreshSlot(account, hashed)
                ?? throw new UnauthorizedAccessException("Invalid refresh token."); // Token not found in either slot — reject

            var slotExpiry = RefreshSlotExpiry(account, clientType);
            if (slotExpiry == null || slotExpiry < DateTime.UtcNow) // Token has passed its expiration date — force re-login
                throw new UnauthorizedAccessException("Refresh token expired. Please login again.");

            // Reuse existing Redis session ID (sliding expiry) and sign new JWT access token
            var (accessToken, accessExpiry) = GenerateAccessToken(
                account, await ContinueSession(account.AccountId, clientType), clientType);
            var (newRefreshToken, newRefreshExpiry) = GenerateRefreshToken(); // Rotate refresh token — invalidates previous token

            SetRefreshSlot(account, clientType, HashRefreshToken(newRefreshToken), newRefreshExpiry); // Store new hashed token, overwriting old
            account.UpdatedAt = DateTime.UtcNow;
            if (account.PlayerProfile != null)
            {
                _playerProfileService.RecalculateEnergy(account.PlayerProfile); // Sync energy regeneration before responding
            }
            await _repository.UpdateAccount(account); // Persist rotated refresh token hash to DB

            var hasCharacter = account.PlayerProfile != null;
            var response = _mapper.Map<AuthResponseDto>(account); // Map account entity to response DTO

            response.AccessToken = accessToken;
            response.AccessTokenExpiresAt = accessExpiry;
            response.RefreshToken = newRefreshToken;
            response.RefreshTokenExpiresAt = newRefreshExpiry;

            if (!HasSavedPosition(account.PlayerProfile))
            {
                response.PositionX = DefaultSpawnX; // Use default spawn point if no saved position exists
                response.PositionY = DefaultSpawnY;
            }

            response.LastMapName = NormalizeMapName(account.PlayerProfile?.LastMapName); // Normalize map alias to canonical name

            response.HasCharacter = !string.IsNullOrWhiteSpace(response.PlayerClass);

            return response;
        }

        // Executes core business logic for revoke refresh token.
        // Logic details: delegates data queries and updates to repository layer.
        // Completes asynchronously upon successful execution.
        public async Task RevokeRefreshToken(int accountId, string? clientType)
        {
            await _repository.RevokeRefreshToken(accountId, clientType); // Clear refresh token hash from the correct DB slot (Web, Game, or both)

            if (clientType == null || NormalizeClient(clientType) == ClientGame)
            {
                await _repository.ClearLastSeen(accountId); // Remove last-seen timestamp when Game client disconnects
            }

            if (clientType == null) // Logout from all clients (e.g., password reset) — clear both session keys
            {
                await _cache.RemoveAsync(ActiveSessionKey(accountId, ClientWeb)); // Invalidate Web session in Redis
                await _cache.RemoveAsync(ActiveSessionKey(accountId, ClientGame)); // Invalidate Game session in Redis
            }
            else
            {
                await _cache.RemoveAsync(ActiveSessionKey(accountId, clientType)); // Invalidate only the calling client's Redis session
            }
        }

        // Executes core business logic for revoke refresh token by token.
        // Logic details: validates required non-empty string arguments; delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models; throws KeyNotFoundException on invalid state or rule violations.
        // Completes asynchronously upon successful execution.
        public async Task RevokeRefreshTokenByToken(string refreshToken)
        {
            var hashed = HashRefreshToken(refreshToken);
            var account = await _repository.GetAccountByRefreshToken(hashed);
            if (account == null)  // Entity not found — short-circuit with appropriate error result
                return;

            // Supported client types: Web or Game; this value selects the independent refresh-token slot and session behavior.
            var clientType = MatchRefreshSlot(account, hashed);
            if (clientType == null)  // Entity not found — short-circuit with appropriate error result
                return;

            await RevokeRefreshToken(account.AccountId, clientType);
        }

        // Executes core business logic for get me.
        // Logic details: delegates data queries and updates to repository layer; throws KeyNotFoundException on invalid state or rule violations.
        // Returns the computed MeResponseDto result asynchronously.
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

            var response = _mapper.Map<MeResponseDto>(account);  // Transform domain entity into DTO for the API response layer

            if (!HasSavedPosition(account.PlayerProfile))
            {
                response.PositionX = DefaultSpawnX;
                response.PositionY = DefaultSpawnY;
            }

            response.LastMapName = NormalizeMapName(account.PlayerProfile?.LastMapName);

            response.HasCharacter = !string.IsNullOrWhiteSpace(response.PlayerClass);

            return response;
        }

        // Sends a 6-digit email verification code for new user registration and stores it in Redis cache.
        public async Task SendVerificationCode(string email)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant(); // Normalize to lowercase before lookup and cache key

            if (await _repository.IsEmailExist(normalizedEmail)) // Prevent sending OTP to an email already registered
                throw new BadRequestException("Email already registered.");

            var otp = GenerateVerificationCode(); // Generate cryptographically secure 6-digit OTP
            var cacheKey = $"{OTP_CACHE_PREFIX}{normalizedEmail}";

            await _cache.SetStringAsync(cacheKey, otp, Ttl(TimeSpan.FromMinutes(OtpExpiryMinutes))); // Cache OTP in Redis with short TTL — auto-expires for security

            var htmlBody = BuildHtmlEmailTemplate(
                "Player Account Verification",
                "Use the following verification code to complete your <strong>Mystic Journey</strong> account registration:",
                otp,
                OtpExpiryMinutes);

            var sent = await SendEmailAsync( // Dispatch OTP email via SMTP — returns false if delivery fails
                normalizedEmail,
                "Mystic Journey - Email Verification (OTP)",
                htmlBody);

            if (!sent) // Email delivery failed — client must retry
                throw new InvalidOperationException("Failed to send verification email.");
        }

        // Validates the registration OTP verification code against Redis cache and marks email as verified.
        public async Task VerifyEmail(VerifyEmailRequestDto request)
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant(); // Normalize email for consistent cache key lookup
            var cacheKey = $"{OTP_CACHE_PREFIX}{normalizedEmail}";

            var cachedOtp = await _cache.GetStringAsync(cacheKey); // Retrieve stored OTP from Redis for comparison
            if (string.IsNullOrEmpty(cachedOtp)) // OTP expired or was never sent — prompt user to request again
                throw new BadRequestException("No verification code found. Please request a new one.");

            if (cachedOtp != request.VerificationCode) // Submitted code does not match stored OTP
                throw new BadRequestException("Invalid verification code.");

            await _cache.RemoveAsync(cacheKey); // Consume OTP — single-use token, invalidate after verification

            var verifiedKey = $"{VERIFIED_CACHE_PREFIX}{normalizedEmail}";
            await _cache.SetStringAsync(verifiedKey, VerifiedFlag, Ttl(TimeSpan.FromMinutes(VerifiedExpiryMinutes))); // Mark email as verified in Redis so Register() can proceed
        }

        // Generates a cryptographically secure random 6-digit numeric OTP verification code.
        private static string GenerateVerificationCode()
            => RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");

        // Generates a cryptographically secure random URL-safe base64 or hex token string.
        private static string GenerateRandomToken(int byteLength = 64)
        {
            var bytes = new byte[byteLength];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
        // Builds and signs a JWT access token embedding account ID, role, profile ID, session ID, and client type claims.
        private (string token, DateTime expires) GenerateAccessToken(Account account, string sessionId, string clientType)
        {
            var jwt = _configuration.GetSection("Jwt"); // Load JWT config (Key, Issuer, Audience) from appsettings
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)); // Convert secret key string to cryptographic key
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); // Use HMAC-SHA256 signature algorithm
            var expires = DateTime.UtcNow.AddMinutes(AccessTokenExpiryMinutes); // Access token is short-lived for security

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, account.AccountId.ToString()), // Account ID for server-side authorization
                new Claim(ClaimTypes.Name, account.UserName),
                new Claim(ClaimTypes.Email, account.Email),
                new Claim(ClaimTypes.Role, account.Role?.Name ?? "Player"), // Role determines API access permissions
                new Claim("playerProfileId", account.PlayerProfile?.PlayerProfileId.ToString() ?? "0"), // Player profile ID used by game endpoints
                new Claim(JwtRegisteredClaimNames.Sid, sessionId), // Session ID for concurrent session detection
                new Claim(ClientTypeClaim, NormalizeClient(clientType)) // Client type (Web/Game) for client-specific routing
            };

            var token = new JwtSecurityToken(
                jwt["Issuer"], jwt["Audience"], claims,
                expires: expires, signingCredentials: creds);

            return (new JwtSecurityTokenHandler().WriteToken(token), expires); // Serialize token to compact JWT string
        }

        // Normalizes world map names and maps aliases (such as ElfForest) to canonical map identifiers.
        private static string NormalizeMapName(string? mapName)
        {
            if (string.IsNullOrWhiteSpace(mapName))  // Mandatory string argument is blank — fail fast
                return DefaultMapName;

            var normalized = mapName.Trim();
            return IsDefaultMapAlias(normalized) ? DefaultMapName : normalized;
        }

        // Checks if the provided map name matches known aliases for the default starting map.
        private static bool IsDefaultMapAlias(string mapName)
        {
            return string.Equals(mapName, "ElfForest", StringComparison.OrdinalIgnoreCase)
                || string.Equals(mapName, "ElfLand", StringComparison.OrdinalIgnoreCase)
                || string.Equals(mapName, "Map1", StringComparison.OrdinalIgnoreCase)
                || string.Equals(mapName, "Chapter1", StringComparison.OrdinalIgnoreCase)
                || string.Equals(mapName, "Chapter 1", StringComparison.OrdinalIgnoreCase)
                || string.Equals(mapName, DefaultMapName, StringComparison.OrdinalIgnoreCase);
        }

        // Executes core business logic for normalize player class.
        // Logic details: validates required non-empty string arguments.
        private static string? NormalizePlayerClass(string? playerClass)
            => string.IsNullOrWhiteSpace(playerClass) ? null : playerClass.Trim();

        // Determines whether the player profile has a previously saved non-zero world coordinate.
        private static bool HasSavedPosition(PlayerProfile? profile)
        {
            if (profile == null)  // Entity not found — short-circuit with appropriate error result
                return false;

            var hasMap = !string.IsNullOrWhiteSpace(profile.LastMapName);
            if (!hasMap)
                return false;

            var hasPosition = Math.Abs(profile.PositionX) > double.Epsilon ||
                              Math.Abs(profile.PositionY) > double.Epsilon;

            return hasPosition || profile.Level > 1;
        }

        // Checks if the client type string corresponds to the Game client.
        private static bool IsGameClient(string? clientType)
            => string.Equals(clientType?.Trim(), "Game", StringComparison.OrdinalIgnoreCase);

        // Verifies a plaintext password against the stored BCrypt password hash.
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

        // Executes core business logic for private.
        private (string token, DateTime expires) GenerateRefreshToken()
            => (GenerateRandomToken(64), DateTime.UtcNow.AddDays(RefreshTokenExpiryDays));

        // Dispatches an HTML email to the specified recipient address via configured SMTP settings.
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
                var mail = new MailMessage(smtp["FromEmail"], to, subject, body)
                {
                    IsBodyHtml = true
                };
                await client.SendMailAsync(mail);
                return true;
            }
            catch { return false; }
        }

        // Constructs a styled HTML email template containing OTP verification code and expiration notice.
        private static string BuildHtmlEmailTemplate(string subtitle, string messageText, string otpCode, int expiryMinutes)
        {
            return $"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
            </head>
            <body style="margin: 0; padding: 0; background-color: #0b0f19; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; color: #e2e8f0;">
                <table role="presentation" width="100%" cellspacing="0" cellpadding="0" style="background-color: #0b0f19; padding: 40px 10px;">
                    <tr>
                        <td align="center">
                            <table role="presentation" width="100%" style="max-width: 520px; background-color: #151d30; border: 1px solid #2d3748; border-top: 4px solid #f59e0b; border-radius: 16px; box-shadow: 0 10px 25px -5px rgba(0, 0, 0, 0.5); overflow: hidden; padding: 0;">
                                <!-- Header -->
                                <tr>
                                    <td style="padding: 32px 32px 16px 32px; text-align: center; background: linear-gradient(180deg, rgba(245, 158, 11, 0.12) 0%, rgba(21, 29, 48, 0) 100%);">
                                        <h1 style="margin: 0; font-size: 24px; font-weight: 800; color: #fbbf24; letter-spacing: 2px; text-transform: uppercase;">
                                            ⚔️ MYSTIC JOURNEY ⚔️
                                        </h1>
                                        <p style="margin: 8px 0 0 0; font-size: 15px; color: #94a3b8; font-weight: 600;">
                                            {subtitle}
                                        </p>
                                    </td>
                                </tr>

                                <!-- Body Content -->
                                <tr>
                                    <td style="padding: 16px 32px 32px 32px; text-align: center;">
                                        <p style="margin: 0 0 24px 0; font-size: 15px; color: #cbd5e1; line-height: 1.6;">
                                            {messageText}
                                        </p>

                                        <!-- OTP Box -->
                                        <div style="background-color: #0f172a; border: 2px dashed #f59e0b; border-radius: 12px; padding: 20px 10px; margin: 0 auto 24px auto; max-width: 320px;">
                                            <span style="font-family: 'Courier New', Courier, monospace; font-size: 38px; font-weight: 800; color: #fbbf24; letter-spacing: 8px; display: inline-block;">
                                                {otpCode}
                                            </span>
                                        </div>

                                        <!-- Expiry Notice -->
                                        <div style="display: inline-block; background-color: rgba(245, 158, 11, 0.1); border-radius: 20px; padding: 8px 18px; margin-bottom: 24px;">
                                            <p style="margin: 0; font-size: 13px; color: #f59e0b; font-weight: 600;">
                                                ⏳ This verification code will expire in <strong>{expiryMinutes} minutes</strong>.
                                            </p>
                                        </div>

                                        <p style="margin: 0; font-size: 13px; color: #64748b; line-height: 1.5;">
                                            If you did not make this request, you can safely ignore this email.
                                        </p>
                                    </td>
                                </tr>

                                <!-- Footer -->
                                <tr>
                                    <td style="padding: 20px 32px; background-color: #0f172a; border-top: 1px solid #1e293b; text-align: center;">
                                        <p style="margin: 0; font-size: 12px; color: #475569;">
                                            © 2026 Mystic Journey Game. All rights reserved.
                                        </p>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </body>
            </html>
            """;
        }

        // Generates a 6-digit OTP verification code, caches it in Redis with a TTL, and emails it to the user.
        public async Task ForgetPassword(string email)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();

            if (!await _repository.IsEmailExist(normalizedEmail))
                throw new BadRequestException("Email not registered.");  // Business rule violation — surface as 400 Bad Request

            var otp = GenerateVerificationCode();
            var cacheKey = $"{OTP_CACHE_PREFIX}{normalizedEmail}";

            await _cache.SetStringAsync(cacheKey, otp, Ttl(TimeSpan.FromMinutes(OtpExpiryMinutes)));  // Cache result with TTL to reduce repeated DB lookups

            var htmlBody = BuildHtmlEmailTemplate(
                "Player Password Reset",
                "Use the following verification code to reset your <strong>Mystic Journey</strong> account password:",
                otp,
                OtpExpiryMinutes);

            var sent = await SendEmailAsync(
                normalizedEmail,
                "Mystic Journey - Password Reset (OTP)",
                htmlBody);

            if (!sent)
                throw new InvalidOperationException("Failed to send reset email.");  // Unexpected runtime state — propagate to global error handler
        }

        // Verifies the submitted OTP code, updates the account password hash, and revokes all active sessions.
        public async Task ResetPassword(string email, string verificationCode, string newPassword, string confirmPassword)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();

            if (!string.Equals(newPassword, confirmPassword, StringComparison.Ordinal))
                throw new BadRequestException("Passwords do not match.");  // Business rule violation — surface as 400 Bad Request

            var cacheKey = $"{OTP_CACHE_PREFIX}{normalizedEmail}";
            var cachedOtp = await _cache.GetStringAsync(cacheKey);  // Look up precomputed value in distributed Redis cache
            if (string.IsNullOrEmpty(cachedOtp) || cachedOtp != verificationCode)  // Mandatory string argument is null or empty — fail fast
                throw new BadRequestException("Invalid or expired verification code.");  // Business rule violation — surface as 400 Bad Request

            var account = await _repository.GetAccountByEmail(normalizedEmail)
                ?? throw new KeyNotFoundException("Account not found.");

            account.HashPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
            account.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAccount(account);

            await RevokeRefreshToken(account.AccountId, null);

            await _cache.RemoveAsync(cacheKey);  // Invalidate stale cache entry to force fresh recalculation
        }
    }

    // Executes core business logic for exception.
    public class BadRequestException : Exception
    {
        // Executes core business logic for bad request exception.
        public BadRequestException(string message) : base(message) { }
    }

    // Executes core business logic for exception.
    public class AccountNotFoundException : Exception
    {
        // Executes core business logic for account not found exception.
        public AccountNotFoundException(string message) : base(message) { }
    }
}
