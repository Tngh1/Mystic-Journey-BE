using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
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

        public AccountService(IAccountRepository repository, IMapper mapper, IConfiguration configuration)
        {
            _repository = repository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.EmailOrUsername) || string.IsNullOrWhiteSpace(request.Password))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Email/username and password must not be empty."
                };
            }

            var account = await _repository.GetByUsernameOrEmailAsync(request.EmailOrUsername.Trim());
            if (account == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Account does not exist or has been deactivated."
                };
            }

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, account.HashPassword);
            if (!isPasswordValid)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Incorrect password."
                };
            }

            var (accessToken, accessTokenExpiry) = GenerateAccessToken(account);
            var loginRefreshToken = GenerateToken();
            var loginRefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            account.RefreshToken = loginRefreshToken;
            account.RefreshTokenExpiryTime = loginRefreshTokenExpiry;
            account.LastLogin = DateTime.UtcNow;
            account.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAccountAsync(account);

            var loginResponse = _mapper.Map<AuthResponseDto>(account);
            loginResponse.Success = true;
            loginResponse.Message = "Login successful.";
            loginResponse.AccessToken = accessToken;
            loginResponse.AccessTokenExpiresAt = accessTokenExpiry;
            loginResponse.RefreshToken = loginRefreshToken;
            loginResponse.RefreshTokenExpiresAt = loginRefreshTokenExpiry;

            return loginResponse;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            if (request == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid registration data."
                };
            }

            if (string.IsNullOrWhiteSpace(request.UserName) ||
                string.IsNullOrWhiteSpace(request.FullName) ||
                string.IsNullOrWhiteSpace(request.EmailAddress) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Registration information must not be empty."
                };
            }

            if (!string.Equals(request.Password, request.ConfirmPassword, StringComparison.Ordinal))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Password confirmation does not match."
                };
            }

            var normalizedEmail = request.EmailAddress.Trim().ToLowerInvariant();
            var normalizedUsername = request.UserName.Trim();

            if (await _repository.IsEmailExistAsync(normalizedEmail))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Email already exists."
                };
            }

            if (await _repository.IsUsernameExistAsync(normalizedUsername))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Username already exists."
                };
            }

            var registerRefreshToken = GenerateToken();
            var registerRefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            var account = _mapper.Map<Account>(request);
            account.Id = Guid.NewGuid();
            account.UserName = normalizedUsername;
            account.EmailAddress = normalizedEmail;
            account.HashPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            account.Role = Account.AccountRole.Player;
            account.CreatedAt = DateTime.UtcNow;
            account.UpdatedAt = DateTime.UtcNow;
            account.IsActive = true;
            account.EmailConfirmed = false;
            account.RefreshToken = registerRefreshToken;
            account.RefreshTokenExpiryTime = registerRefreshTokenExpiry;
            account.EmailVerificationToken = GenerateToken();
            account.EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(24);

            await _repository.CreateAccountAsync(account);

            var (accessToken, accessTokenExpiry) = GenerateAccessToken(account);

            var registerResponse = _mapper.Map<AuthResponseDto>(account);
            registerResponse.Success = true;
            registerResponse.Message = "Registration successful.";
            registerResponse.AccessToken = accessToken;
            registerResponse.AccessTokenExpiresAt = accessTokenExpiry;
            registerResponse.RefreshToken = registerRefreshToken;
            registerResponse.RefreshTokenExpiresAt = registerRefreshTokenExpiry;

            return registerResponse;
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequestDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email))
            {
                return false;
            }

            var account = await _repository.GetByEmailAsync(request.Email.Trim());
            if (account == null)
            {
                return false;
            }

            account.PasswordResetToken = GenerateToken();
            account.PasswordResetTokenExpiry = DateTime.UtcNow.AddMinutes(30);
            account.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAccountAsync(account);
            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.PasswordResetToken) ||
                string.IsNullOrWhiteSpace(request.NewPassword) ||
                string.IsNullOrWhiteSpace(request.ConfirmPassword))
            {
                return false;
            }

            if (!string.Equals(request.NewPassword, request.ConfirmPassword, StringComparison.Ordinal))
            {
                return false;
            }

            var account = await _repository.GetByPasswordResetTokenAsync(request.PasswordResetToken.Trim());
            if (account == null)
            {
                return false;
            }

            if (!account.PasswordResetTokenExpiry.HasValue || account.PasswordResetTokenExpiry.Value < DateTime.UtcNow)
            {
                return false;
            }

            account.HashPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            account.PasswordResetToken = null;
            account.PasswordResetTokenExpiry = null;
            account.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAccountAsync(account);
            return true;
        }

        private static string GenerateToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        }

        private (string Token, DateTime ExpiresAt) GenerateAccessToken(Account account)
        {
            var jwtSection = _configuration.GetSection("Jwt");
            var key = jwtSection["Key"] ?? throw new InvalidOperationException("Jwt:Key is missing.");
            var issuer = jwtSection["Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer is missing.");
            var audience = jwtSection["Audience"] ?? throw new InvalidOperationException("Jwt:Audience is missing.");

            var expireMinutesText = jwtSection["ExpireMinutes"];
            var expireMinutes = int.TryParse(expireMinutesText, out var parsedExpireMinutes) ? parsedExpireMinutes : 60;

            var expiresAt = DateTime.UtcNow.AddMinutes(expireMinutes);
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, account.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, account.UserName),
                new Claim(ClaimTypes.Name, string.IsNullOrWhiteSpace(account.FullName) ? account.UserName : account.FullName),
                new Claim(JwtRegisteredClaimNames.Email, account.EmailAddress),
                new Claim(ClaimTypes.Email, account.EmailAddress),
                new Claim(ClaimTypes.Role, account.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokenDescriptor = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: signingCredentials);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
            return (accessToken, expiresAt);
        }
    }
}
