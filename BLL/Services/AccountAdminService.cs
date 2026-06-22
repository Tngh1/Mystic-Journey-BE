using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class AccountAdminService : IAccountAdminService
    {
        private readonly IAuthRepository _authRepository;

        public AccountAdminService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public async Task<AccountAdminResponseDto?> GetAccountById(int id)
        {
            var account = await _authRepository.GetAccountById(id);
            if (account == null)
                return null;

            return MapToResponseDto(account);
        }

        public async Task<AccountAdminResponseDto> CreateAccount(CreateAccountAdminRequestDto request)
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var normalizedUsername = request.UserName.Trim();

            if (await _authRepository.IsEmailExist(normalizedEmail))
                throw new InvalidOperationException("Email already exists.");

            if (await _authRepository.IsUsernameExist(normalizedUsername))
                throw new InvalidOperationException("Username already exists.");

            var account = new Account
            {
                UserName = normalizedUsername,
                Email = normalizedEmail,
                HashPassword = BCrypt.Net.BCrypt.HashPassword(request.Password),
                RoleId = request.RoleId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            if (request.RoleId == 1)
            {
                account.PlayerProfile = new PlayerProfile
                {
                    DisplayName = request.DisplayName ?? normalizedUsername,
                    Class = request.PlayerClass,
                    Level = 1,
                    ExperiencePoints = 0,
                    Gold = 0,
                    Gems = 0,
                    Energy = 100,
                    CreatedAt = DateTime.UtcNow
                };
            }

            await _authRepository.CreateAccount(account);

            var created = await _authRepository.GetAccountById(account.AccountId);
            return MapToResponseDto(created!);
        }

        public async Task<AccountAdminResponseDto> UpdateAccount(int id, UpdateAccountAdminRequestDto request)
        {
            var account = await _authRepository.GetAccountById(id)
                ?? throw new KeyNotFoundException($"Account with id {id} not found.");

            if (request.FullName != null)
            {
                account.UserName = request.FullName;
            }

            if (request.Email != null)
            {
                var normalizedEmail = request.Email.Trim().ToLowerInvariant();
                if (normalizedEmail != account.Email && await _authRepository.IsEmailExist(normalizedEmail))
                    throw new InvalidOperationException("Email already exists.");

                account.Email = normalizedEmail;
            }

            if (request.RoleId.HasValue)
            {
                account.RoleId = request.RoleId.Value;
            }

            if (request.IsActive.HasValue)
            {
                account.IsActive = request.IsActive.Value;
            }

            if (!string.IsNullOrEmpty(request.NewPassword))
            {
                account.HashPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            }

            account.UpdatedAt = DateTime.UtcNow;
            await _authRepository.UpdateAccount(account);

            return MapToResponseDto(account);
        }

        public async Task<PagedResultDto<AccountAdminResponseDto>> GetAccountsPaged(int page, int pageSize, string? search, bool? isActive, string? roleName)
        {
            var (totalCount, items) = await _authRepository.GetAccountsPaged(page, pageSize, search, isActive, roleName);

            var dtos = items.Select(MapToResponseDto).ToList();
            return new PagedResultDto<AccountAdminResponseDto>(totalCount, dtos);
        }

        public async Task<AccountAdminResponseDto> BanAccount(int accountId)
        {
            var account = await _authRepository.GetAccountById(accountId)
                ?? throw new KeyNotFoundException($"Account with id {accountId} not found.");
            account.IsActive = false;
            account.UpdatedAt = DateTime.UtcNow;
            await _authRepository.UpdateAccount(account);
            return MapToResponseDto(account);
        }

        public async Task<AccountAdminResponseDto> UnbanAccount(int accountId)
        {
            var account = await _authRepository.GetAccountById(accountId)
                ?? throw new KeyNotFoundException($"Account with id {accountId} not found.");
            account.IsActive = true;
            account.UpdatedAt = DateTime.UtcNow;
            await _authRepository.UpdateAccount(account);
            return MapToResponseDto(account);
        }

        private static AccountAdminResponseDto MapToResponseDto(Account account)
        {
            return new AccountAdminResponseDto
            {
                AccountId = account.AccountId,
                UserName = account.UserName,
                Email = account.Email,
                RoleName = account.Role?.Name ?? "Unknown",
                IsActive = account.IsActive,
                CreatedAt = account.CreatedAt,
                LastLogin = account.LastLogin,
                PlayerProfileId = account.PlayerProfile?.PlayerProfileId,
                PlayerDisplayName = account.PlayerProfile?.DisplayName
            };
        }
    }
}
