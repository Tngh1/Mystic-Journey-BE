using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
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

        public AccountService(
            IAccountRepository repository,
            IMapper mapper,
            IConfiguration configuration)
        {
            _repository = repository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<ApiResponseDto> LoginAsync(LoginRequestDto request)
        {
            if (request == null)
            {
                return new ApiResponseDto
                {
                    Success = false,
                    Message = "Something went wrong. Please try logging in again."
                };
            }

            var emailOrUsername = request.EmailOrUsername?.Trim();
            if (string.IsNullOrWhiteSpace(emailOrUsername) || string.IsNullOrWhiteSpace(request.Password))
            {
                return new ApiResponseDto
                {
                    Success = false,
                    Message = "Please enter both your email/username and password."
                };
            }

            var account = await _repository.GetByUsernameOrEmailAsync(emailOrUsername);
            if (account == null || !account.IsActive)
            {
                return new ApiResponseDto
                {
                    Success = false,
                    Message = "We couldn't find your account or it has been deactivated."
                };
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, account.HashPassword))
            {
                return new ApiResponseDto
                {
                    Success = false,
                    Message = "The password you entered is incorrect."
                };
            }

            var (accessToken, accessTokenExpiry) = GenerateAccessToken(account);
            account.LastLogin = DateTime.UtcNow;
            account.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAccountAsync(account);

            var response = _mapper.Map<ApiResponseDto>(account);
            response.Success = true;
            response.Message = "Login successful! Welcome back.";
            response.Account ??= _mapper.Map<AccountResponseDto>(account);
            response.Account.AccessToken = accessToken;
            response.Account.AccessTokenExpiresAt = accessTokenExpiry;

            return response;
        }

        public async Task<ApiResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            if (request == null)
            {
                return new ApiResponseDto
                {
                    Success = false,
                    Message = "Something went wrong. Please check your information and try again."
                };
            }

            if (string.IsNullOrWhiteSpace(request.UserName) ||
                string.IsNullOrWhiteSpace(request.FullName) ||
                string.IsNullOrWhiteSpace(request.EmailAddress) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return new ApiResponseDto
                {
                    Success = false,
                    Message = "Please fill in all required information."
                };
            }

            if (!string.Equals(request.Password, request.ConfirmPassword, StringComparison.Ordinal))
            {
                return new ApiResponseDto
                {
                    Success = false,
                    Message = "Passwords do not match. Please check again."
                };
            }

            var normalizedEmail = request.EmailAddress.Trim().ToLowerInvariant();
            var normalizedUsername = request.UserName.Trim();

            if (await _repository.IsEmailExistAsync(normalizedEmail))
            {
                return new ApiResponseDto
                {
                    Success = false,
                    Message = "This email is already registered. Please use another email or log in."
                };
            }

            if (await _repository.IsUsernameExistAsync(normalizedUsername))
            {
                return new ApiResponseDto
                {
                    Success = false,
                    Message = "This username is already taken. Please choose another one."
                };
            }

            var account = _mapper.Map<Account>(request);
            account.AccountId = Guid.NewGuid();
            account.UserName = normalizedUsername;
            account.EmailAddress = normalizedEmail;
            account.HashPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            account.RoleId = 1;
            account.CreatedAt = DateTime.UtcNow;
            account.UpdatedAt = DateTime.UtcNow;
            account.IsActive = true;

            await _repository.CreateAccountAsync(account);

            var (accessToken, accessTokenExpiry) = GenerateAccessToken(account);

            var response = _mapper.Map<ApiResponseDto>(account);
            response.Success = true;
            response.Message = "Your account has been created successfully.";
            response.Account ??= _mapper.Map<AccountResponseDto>(account);
            response.Account.AccessToken = accessToken;
            response.Account.AccessTokenExpiresAt = accessTokenExpiry;

            return response;
        }

        public async Task<ApiResponseDto> ForgotPasswordAsync(ForgotPasswordRequestDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email))
            {
                return new ApiResponseDto
                {
                    Success = false,
                    Message = "Please enter your email address."
                };
            }

            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var account = await _repository.GetByEmailAsync(normalizedEmail);

            if (account == null || !account.IsActive)
            {
                return new ApiResponseDto
                {
                    Success = false,
                    Message = "We couldn't find an account with this email."
                };
            }

            var code = GenerateVerificationCode();

            account.PasswordResetToken = code;
            account.PasswordResetTokenExpiry = DateTime.UtcNow.AddMinutes(15);
            account.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAccountAsync(account);

            var emailSent = await SendEmailAsync(
                normalizedEmail,
                "Reset Password Code",
                $"Your verification code is: {code}. This code will expire in 15 minutes.");

            if (!emailSent)
            {
                return new ApiResponseDto
                {
                    Success = false,
                    Message = "We couldn't send the verification email. Please try again later."
                };
            }

            return new ApiResponseDto
            {
                Success = true,
                Message = "A verification code has been sent to your email."
            };
        }

        public async Task<ApiResponseDto> ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            if (request == null)
            {
                return new ApiResponseDto
                {
                    Success = false,
                    Message = "Something went wrong. Please try again."
                };
            }

            if (!string.Equals(request.NewPassword, request.ConfirmPassword, StringComparison.Ordinal))
            {
                return new ApiResponseDto
                {
                    Success = false,
                    Message = "Passwords do not match. Please re-enter them."
                };
            }

            var account = await _repository.GetByEmailAndPasswordResetCodeAsync(
                request.Email.Trim().ToLower(),
                request.VerificationCode.Trim());

            if (account == null)
            {
                return new ApiResponseDto
                {
                    Success = false,
                    Message = "The verification code is invalid."
                };
            }

            if (account.PasswordResetTokenExpiry < DateTime.UtcNow)
            {
                return new ApiResponseDto
                {
                    Success = false,
                    Message = "This verification code has expired. Please request a new one."
                };
            }

            account.HashPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            account.PasswordResetToken = null;
            account.PasswordResetTokenExpiry = null;

            await _repository.UpdateAccountAsync(account);

            return new ApiResponseDto
            {
                Success = true,
                Message = "Your password has been reset successfully. You can now log in."
            };
        }

        public async Task<ApiResponseDto> ChangePasswordAsync(Guid accountId, ChangePasswordRequestDto request)
        {
            var account = await _repository.GetByIdAsync(accountId);

            if (account == null)
            {
                return new ApiResponseDto
                {
                    Success = false,
                    Message = "We couldn't find your account."
                };
            }

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, account.HashPassword))
            {
                return new ApiResponseDto
                {
                    Success = false,
                    Message = "The current password you entered is incorrect."
                };
            }

            account.HashPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            await _repository.UpdateAccountAsync(account);

            return new ApiResponseDto
            {
                Success = true,
                Message = "Your password has been changed successfully."
            };
        }

        public async Task<ApiResponseDto> UpdateProfileAsync(Guid accountId, UpdateProfileRequestDto request)
        {
            var account = await _repository.GetByIdAsync(accountId);

            if (account == null || !account.IsActive)
            {
                return new ApiResponseDto
                {
                    Success = false,
                    Message = "We couldn't find your account."
                };
            }

            account.FullName = request.FullName;
            account.Gender = request.Gender;
            account.PhoneNumber = request.PhoneNumber;
            account.Birthday = request.Birthday;
            account.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAccountAsync(account);

            var response = new ApiResponseDto
            {
                Success = true,
                Message = "Your profile has been updated successfully.",
                Account = _mapper.Map<AccountResponseDto>(account)
            };

            return response;
        }

        private static string GenerateVerificationCode()
        {
            return RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
        }

        private (string, DateTime) GenerateAccessToken(Account account)
        {
            var jwt = _configuration.GetSection("Jwt");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddDays(int.Parse(jwt["ExpireDays"]));

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, account.AccountId.ToString()),
                new Claim(ClaimTypes.Name, account.UserName),
                new Claim(ClaimTypes.Email, account.EmailAddress),
                new Claim(ClaimTypes.Role, account.Role != null ? account.Role.Name : "Player")
            };

            var token = new JwtSecurityToken(
                jwt["Issuer"],
                jwt["Audience"],
                claims,
                expires: expires,
                signingCredentials: creds);

            return (new JwtSecurityTokenHandler().WriteToken(token), expires);
        }

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
            catch
            {
                return false;
            }
        }
    }
}
