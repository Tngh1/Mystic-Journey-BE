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

        // IMemoryCache lưu được bool; Redis chỉ có string nên cờ "email đã xác thực" phải mã
        // hoá tường minh. So sánh bằng hằng này thay vì bool.Parse: key trống trả null và null
        // != "1" là chưa xác thực — không cần try/catch, và giá trị lạ cũng rơi về chưa xác thực.
        private const string VerifiedFlag = "1";
        private const string DefaultMapName = "ElfForest";
        private const double DefaultSpawnX = 11.9;
        private const double DefaultSpawnY = 17.8;

        private int OtpExpiryMinutes => int.Parse(_configuration["TokenSettings:OtpExpiryMinutes"] ?? "5");
        private int VerifiedExpiryMinutes => int.Parse(_configuration["TokenSettings:VerifiedExpiryMinutes"] ?? "30");
        private int AccessTokenExpiryMinutes => int.Parse(_configuration["TokenSettings:AccessTokenExpiryMinutes"] ?? _configuration["Jwt:AccessTokenExpiryMinutes"] ?? "30");
        private int RefreshTokenExpiryDays => int.Parse(_configuration["TokenSettings:RefreshTokenExpiryDays"] ?? _configuration["Jwt:RefreshTokenExpireDays"] ?? "7");

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

        private static string HashRefreshToken(string token)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(bytes);
        }

        // Web (portal/swagger) và game là HAI phiên độc lập. "Một phiên duy nhất" vẫn giữ
        // nguyên nhưng chỉ trong cùng một loại client: game login mới đá game login cũ,
        // không đá web. Muốn vậy phải tách CẢ HAI cơ chế single-slot — cột refresh token
        // trong DB và khoá active_session — vì chỉ tách một cái là bên kia vẫn chết.
        public const string ClientWeb = "Web";
        public const string ClientGame = "Game";

        // Claim mang loại client trong access token. OnTokenValidated cần nó để biết so sid
        // với khoá session nào; thiếu claim này thì không phân biệt được web/game.
        public const string ClientTypeClaim = "ctyp";

        private static string NormalizeClient(string? clientType)
            => IsGameClient(clientType) ? ClientGame : ClientWeb;

        // Public để Program.cs dùng đúng format khoá thay vì tự nối string ở hai nơi.
        public static string ActiveSessionKey(int accountId, string? clientType)
            => $"active_session:{accountId}:{NormalizeClient(clientType)}";

        // IDistributedCache chỉ nhận string/byte[], không nhận TimeSpan trực tiếp như
        // IMemoryCache. Gom vào một chỗ để 5 call site không ai viết lẫn sang
        // AbsoluteExpiration (mốc tuyệt đối) khi ý là "kể từ bây giờ".
        private static DistributedCacheEntryOptions Ttl(TimeSpan ttl)
            => new() { AbsoluteExpirationRelativeToNow = ttl };

        // MỌI access token phải mang một sid đang là session hiện hành, vì OnTokenValidated
        // chỉ kick được thiết bị khi so sánh được sid. Token không sid = không bao giờ bị kick.
        // Session nằm ở Redis (IDistributedCache) nên mọi instance API đọc chung một nguồn sự
        // thật và phiên sống sót qua restart. Với IMemoryCache mỗi instance giữ cache riêng:
        // login đè phiên ở instance A không kick nổi thiết bị đang gọi vào instance B.
        private async Task<string> StartNewSession(int accountId, string clientType)
        {
            var key = ActiveSessionKey(accountId, clientType);

            // Đọc phiên cũ TRƯỚC khi ghi đè: đây là thông tin duy nhất cho biết có ai vừa bị
            // đá hay không. Ghi trước rồi mới đọc là mất nó.
            var previousSessionId = await _cache.GetStringAsync(key);

            var sessionId = Guid.NewGuid().ToString();
            await _cache.SetStringAsync(
                key,
                sessionId,
                Ttl(TimeSpan.FromDays(RefreshTokenExpiryDays)));

            // Chỉ báo khi thật sự có phiên cũ và nó khác phiên mới. Login đầu tiên (key trống)
            // không đá ai nên gửi thông báo là làm client hiểu sai. Đặt notify ở đây vì đây là
            // chỗ DUY NHẤT một phiên bị đè — Login và ChangePassword đi qua cùng đường này, nên
            // không có call site nào lỡ quên.
            if (!string.IsNullOrEmpty(previousSessionId) && previousSessionId != sessionId)
            {
                await _sessionNotifier.SessionOverridden(accountId, clientType, sessionId);
            }

            return sessionId;
        }

        // Refresh không phải đăng nhập mới: giữ nguyên session đang hiện hành để không tự
        // đá chính mình. Chỉ mint mới khi key trống (hết TTL, hoặc Redis bị flush).
        private async Task<string> ContinueSession(int accountId, string clientType)
        {
            var key = ActiveSessionKey(accountId, clientType);
            var sessionId = await _cache.GetStringAsync(key);
            if (!string.IsNullOrEmpty(sessionId))
            {
                // Gia hạn TTL mỗi lần refresh để phiên đang hoạt động không tự hết hạn giữa
                // lúc chơi; TTL bằng tuổi refresh token nên hai thứ chết cùng nhau.
                await _cache.SetStringAsync(key, sessionId, Ttl(TimeSpan.FromDays(RefreshTokenExpiryDays)));
                return sessionId;
            }
            return await StartNewSession(accountId, clientType);
        }

        // Bốn cột refresh token (2 slot × token+expiry) nên gom vào một chỗ; rải if/else
        // ra 5 call site là kiểu bug ghi sai slot mà compiler không bắt được.
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

        private static DateTime? RefreshSlotExpiry(Account account, string clientType)
            => NormalizeClient(clientType) == ClientGame
                ? account.GameRefreshTokenExpiresAt
                : account.RefreshTokenExpiresAt;

        // Token do client trình lên chỉ là chuỗi, không nói nó thuộc slot nào — phải dò để
        // xoay đúng slot. Xoay sai slot là đá client kia ra, đúng cái bug đang sửa.
        private static string? MatchRefreshSlot(Account account, string hashedToken)
        {
            if (!string.IsNullOrEmpty(account.GameRefreshToken) && account.GameRefreshToken == hashedToken)
                return ClientGame;
            if (!string.IsNullOrEmpty(account.RefreshToken) && account.RefreshToken == hashedToken)
                return ClientWeb;
            return null;
        }

        public async Task<AuthResponseDto> Login(LoginRequestDto request)
        {
            var account = await _repository.GetAccountByUsernameOrEmail(request.EmailOrUsername.Trim());

            if (account == null)
                throw new AccountNotFoundException("Account is not registered. Please register before logging in.");

            if (!account.IsActive)
                throw new UnauthorizedAccessException(
                    string.IsNullOrWhiteSpace(account.BanReason)
                        ? "Your account has been banned."
                        : $"Your account has been banned. Reason: {account.BanReason}");

            if (!await VerifyPasswordWithFallback(account, request.Password))
                throw new UnauthorizedAccessException("Invalid email/username or password.");

            // Gán SessionId mới cho MỌI đăng nhập để phiên cũ CÙNG LOẠI CLIENT bị đè và đăng
            // xuất ngay (Single Active Session). Web và game tách khoá nên không đá lẫn nhau.
            var clientType = NormalizeClient(request.ClientType);
            var sessionId = await StartNewSession(account.AccountId, clientType);

            var (accessToken, accessExpiry) = GenerateAccessToken(account, sessionId, clientType);
            var (refreshToken, refreshExpiry) = GenerateRefreshToken();

            SetRefreshSlot(account, clientType, HashRefreshToken(refreshToken), refreshExpiry);
            account.LastLogin = DateTime.UtcNow;
            if (clientType == ClientGame)
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
            if (await _cache.GetStringAsync(verifiedKey) != VerifiedFlag)
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
            await _cache.RemoveAsync(verifiedKey);

            // Register chỉ có trên web (client game không có luồng đăng ký), nên cấp slot Web.
            var (accessToken, accessExpiry) = GenerateAccessToken(
                account, await StartNewSession(account.AccountId, ClientWeb), ClientWeb);
            var (refreshToken, refreshExpiry) = GenerateRefreshToken();

            SetRefreshSlot(account, ClientWeb, HashRefreshToken(refreshToken), refreshExpiry);
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

        public async Task<AuthResponseDto> ChangePassword(int accountId, ChangePasswordRequestDto request, string clientType)
        {
            var account = await _repository.GetAccountById(accountId)
                ?? throw new KeyNotFoundException("Account not found.");

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, account.HashPassword))
                throw new UnauthorizedAccessException("Current password is incorrect.");

            account.HashPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            account.UpdatedAt = DateTime.UtcNow;

            // Đổi mật khẩu phải xoay phiên, không chỉ đổi hash. Đây là chỗ DUY NHẤT cố tình
            // đụng vào cả hai slot: nếu chỉ xoay slot của client đang gọi, kẻ chiếm tài khoản
            // đang ngồi ở phía kia vẫn giữ phiên — đúng thứ trang Security hứa là sẽ cắt.
            var caller = NormalizeClient(clientType);
            var other = caller == ClientGame ? ClientWeb : ClientGame;

            SetRefreshSlot(account, other, null, null);
            await _cache.RemoveAsync(ActiveSessionKey(accountId, other));

            // Phiên bên kia bị cắt mà KHÔNG có phiên kế nhiệm, nên newSessionId rỗng: không có
            // sid nào để client đối chiếu, cứ nhận là bị đá. StartNewSession phía dưới chỉ phủ
            // client đang gọi, nên chỗ này phải báo riêng.
            await _sessionNotifier.SessionOverridden(accountId, other, string.Empty);

            // Client đang gọi thì được cấp bộ token mới nên đăng nhập liên tục, không bị tự đá.
            var sessionId = await StartNewSession(accountId, caller);
            var (accessToken, accessExpiry) = GenerateAccessToken(account, sessionId, caller);
            var (refreshToken, refreshExpiry) = GenerateRefreshToken();
            SetRefreshSlot(account, caller, HashRefreshToken(refreshToken), refreshExpiry);

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

        public async Task<AuthResponseDto> RefreshToken(string refreshToken)
        {
            var hashed = HashRefreshToken(refreshToken);
            var account = await _repository.GetAccountByRefreshToken(hashed)
                ?? throw new UnauthorizedAccessException("Invalid refresh token.");

            if (!account.IsActive)
                throw new UnauthorizedAccessException(
                    string.IsNullOrWhiteSpace(account.BanReason)
                        ? "Your account has been banned."
                        : $"Your account has been banned. Reason: {account.BanReason}");

            // Slot suy ra từ chính token, không từ tham số: client không nói nó là web hay game
            // khi refresh, và đoán sai là xoay token của client kia → 30 phút sau client đó
            // refresh thất bại và tự logout, đúng cái bug đang sửa.
            var clientType = MatchRefreshSlot(account, hashed)
                ?? throw new UnauthorizedAccessException("Invalid refresh token.");

            var slotExpiry = RefreshSlotExpiry(account, clientType);
            if (slotExpiry == null || slotExpiry < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Refresh token expired. Please login again.");

            var (accessToken, accessExpiry) = GenerateAccessToken(
                account, await ContinueSession(account.AccountId, clientType), clientType);
            var (newRefreshToken, newRefreshExpiry) = GenerateRefreshToken();

            SetRefreshSlot(account, clientType, HashRefreshToken(newRefreshToken), newRefreshExpiry);
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

        public async Task RevokeRefreshToken(int accountId, string? clientType)
        {
            await _repository.RevokeRefreshToken(accountId, clientType);

            // LastSeen là mốc presence của RIÊNG client game (web login không ghi cột này —
            // xem OnlineTimeout.cs). Logout web mà xoá nó là báo người đang chơi thành offline,
            // nên chỉ xoá khi thu hồi phía game hoặc thu hồi tất cả.
            if (clientType == null || NormalizeClient(clientType) == ClientGame)
            {
                await _repository.ClearLastSeen(accountId);
            }

            if (clientType == null)
            {
                await _cache.RemoveAsync(ActiveSessionKey(accountId, ClientWeb));
                await _cache.RemoveAsync(ActiveSessionKey(accountId, ClientGame));
            }
            else
            {
                await _cache.RemoveAsync(ActiveSessionKey(accountId, clientType));
            }
        }

        public async Task RevokeRefreshTokenByToken(string refreshToken)
        {
            var hashed = HashRefreshToken(refreshToken);
            var account = await _repository.GetAccountByRefreshToken(hashed);
            if (account == null)
                return;

            // Chỉ thu hồi slot khớp token. MatchRefreshSlot không thể null ở đây (repo lọc theo
            // đúng hai cột đó), nhưng null xuống dưới nghĩa là "xoá MỌI slot" nên không truyền
            // thẳng: logout một phía tuyệt đối không được đá client kia ra.
            var clientType = MatchRefreshSlot(account, hashed);
            if (clientType == null)
                return;

            await RevokeRefreshToken(account.AccountId, clientType);
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

            await _cache.SetStringAsync(cacheKey, otp, Ttl(TimeSpan.FromMinutes(OtpExpiryMinutes)));

            var htmlBody = BuildHtmlEmailTemplate(
                "Xác thực Tài khoản Người chơi",
                "Mã xác thực đăng ký tài khoản <strong>Mystic Journey</strong> của bạn là:",
                otp,
                OtpExpiryMinutes);

            var sent = await SendEmailAsync(
                normalizedEmail,
                "Mystic Journey - Email Verification (OTP)",
                htmlBody);

            if (!sent)
                throw new InvalidOperationException("Failed to send verification email.");
        }

        public async Task VerifyEmail(VerifyEmailRequestDto request)
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var cacheKey = $"{OTP_CACHE_PREFIX}{normalizedEmail}";

            var cachedOtp = await _cache.GetStringAsync(cacheKey);
            if (string.IsNullOrEmpty(cachedOtp))
                throw new BadRequestException("No verification code found. Please request a new one.");

            if (cachedOtp != request.VerificationCode)
                throw new BadRequestException("Invalid verification code.");

            await _cache.RemoveAsync(cacheKey);

            var verifiedKey = $"{VERIFIED_CACHE_PREFIX}{normalizedEmail}";
            await _cache.SetStringAsync(verifiedKey, VerifiedFlag, Ttl(TimeSpan.FromMinutes(VerifiedExpiryMinutes)));
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

        // sessionId là tham số bắt buộc, không có default: token thiếu sid thì OnTokenValidated
        // không so được và thiết bị đó miễn kick vĩnh viễn. Bỏ default để compiler chặn
        // call site nào quên truyền, thay vì lỗi im lặng.
        //
        // clientType cũng bắt buộc và cùng lý do: khoá session giờ có dạng
        // active_session:{id}:{client}, nên thiếu client là OnTokenValidated dò sai slot.
        private (string token, DateTime expires) GenerateAccessToken(Account account, string sessionId, string clientType)
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
                new Claim("playerProfileId", account.PlayerProfile?.PlayerProfileId.ToString() ?? "0"),
                new Claim(JwtRegisteredClaimNames.Sid, sessionId),
                // Client nằm trong token vì các endpoint dùng access token (logout,
                // change-password) không có cách nào khác để biết mình đang là web hay game,
                // và đoán sai ở đó là thu hồi token của client kia.
                new Claim(ClientTypeClaim, NormalizeClient(clientType))
            };

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
                var mail = new MailMessage(smtp["FromEmail"], to, subject, body)
                {
                    IsBodyHtml = true
                };
                await client.SendMailAsync(mail);
                return true;
            }
            catch { return false; }
        }

        private static string BuildHtmlEmailTemplate(string subtitle, string messageText, string otpCode, int expiryMinutes)
        {
            return $"""
            <!DOCTYPE html>
            <html lang="vi">
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
                                                ⏳ Mã xác thực này sẽ hết hạn trong <strong>{expiryMinutes} phút</strong>.
                                            </p>
                                        </div>

                                        <p style="margin: 0; font-size: 13px; color: #64748b; line-height: 1.5;">
                                            Nếu bạn không thực hiện yêu cầu này, vui lòng an tâm bỏ qua email này.
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

        public async Task ForgotPassword(string email)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();

            if (!await _repository.IsEmailExist(normalizedEmail))
                throw new BadRequestException("Email not registered.");

            var otp = GenerateVerificationCode();
            var cacheKey = $"{OTP_CACHE_PREFIX}{normalizedEmail}";

            await _cache.SetStringAsync(cacheKey, otp, Ttl(TimeSpan.FromMinutes(OtpExpiryMinutes)));

            var htmlBody = BuildHtmlEmailTemplate(
                "Đặt lại Mật khẩu Người chơi",
                "Mã xác thực yêu cầu đặt lại mật khẩu <strong>Mystic Journey</strong> của bạn là:",
                otp,
                OtpExpiryMinutes);

            var sent = await SendEmailAsync(
                normalizedEmail,
                "Mystic Journey - Password Reset (OTP)",
                htmlBody);

            if (!sent)
                throw new InvalidOperationException("Failed to send reset email.");
        }

        public async Task ResetPassword(string email, string verificationCode, string newPassword, string confirmPassword)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();

            if (!string.Equals(newPassword, confirmPassword, StringComparison.Ordinal))
                throw new BadRequestException("Passwords do not match.");

            var cacheKey = $"{OTP_CACHE_PREFIX}{normalizedEmail}";
            var cachedOtp = await _cache.GetStringAsync(cacheKey);
            if (string.IsNullOrEmpty(cachedOtp) || cachedOtp != verificationCode)
                throw new BadRequestException("Invalid or expired verification code.");

            var account = await _repository.GetAccountByEmail(normalizedEmail)
                ?? throw new KeyNotFoundException("Account not found.");

            account.HashPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
            account.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAccount(account);

            // Đổi mật khẩu phải đá mọi phiên cũ ra: nếu không, kẻ đã chiếm được tài khoản
            // vẫn giữ refresh token hợp lệ tới 7 ngày và việc đặt lại mật khẩu vô nghĩa.
            // null = cả web lẫn game — luồng này không có client nào cần giữ đăng nhập
            // (người dùng đang ở trang quên mật khẩu, chưa đăng nhập ở đâu cả).
            await RevokeRefreshToken(account.AccountId, null);

            await _cache.RemoveAsync(cacheKey);
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
}
