using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Repositories.Interfaces;

namespace BLL.Services
{
    // Executes core business logic for i account admin service.
    public class AccountAdminService : IAccountAdminService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IMapper _mapper;

        // Initializes a new instance of AccountAdminService with dependencies: authRepository, mapper.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public AccountAdminService(IAuthRepository authRepository, IMapper mapper)
        {
            _authRepository = authRepository;
            _mapper = mapper;
        }

        // Executes core business logic for get account by id.
        // Logic details: delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models.
        // Returns the computed AccountAdminResponseDto? result asynchronously.
        public async Task<AccountAdminResponseDto?> GetAccountById(int id)
        {
            var account = await _authRepository.GetAccountById(id);
            if (account == null)  // Entity not found — short-circuit with appropriate error result
                return null;

            return _mapper.Map<AccountAdminResponseDto>(account);  // Transform domain entity into DTO for the API response layer
        }

        // Executes core business logic for get accounts paged.
        // Logic details: validates required non-empty string arguments; delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models; throws KeyNotFoundException on invalid state or rule violations.
        // Returns the computed PagedResultDto<AccountAdminResponseDto result asynchronously.
        public async Task<PagedResultDto<AccountAdminResponseDto>> GetAccountsPaged(int page, int pageSize, string? search, bool? isActive, string? roleName)
        {
            var (totalCount, items) = await _authRepository.GetAccountsPaged(page, pageSize, search, isActive, roleName);

            var dtos = _mapper.Map<List<AccountAdminResponseDto>>(items);  // Transform domain entity into DTO for the API response layer
            return new PagedResultDto<AccountAdminResponseDto>(totalCount, dtos);
        }

        // Executes core business logic for ban account.
        // Logic details: delegates data queries and updates to repository layer; throws KeyNotFoundException on invalid state or rule violations.
        // Returns the computed AccountAdminResponseDto result asynchronously.
        public async Task<AccountAdminResponseDto> BanAccount(int accountId, string? banReason)
        {
            var account = await _authRepository.GetAccountById(accountId)
                ?? throw new KeyNotFoundException($"Account with id {accountId} not found.");
            account.IsActive = false;
            account.BanReason = string.IsNullOrWhiteSpace(banReason) ? null : banReason.Trim();
            account.UpdatedAt = DateTime.UtcNow;
            await _authRepository.UpdateAccount(account);
            return _mapper.Map<AccountAdminResponseDto>(account);  // Transform domain entity into DTO for the API response layer
        }

        // Executes core business logic for unban account.
        // Logic details: delegates data queries and updates to repository layer; throws KeyNotFoundException on invalid state or rule violations.
        // Returns the computed AccountAdminResponseDto result asynchronously.
        public async Task<AccountAdminResponseDto> UnbanAccount(int accountId)
        {
            var account = await _authRepository.GetAccountById(accountId)
                ?? throw new KeyNotFoundException($"Account with id {accountId} not found.");
            account.IsActive = true;
            account.BanReason = null;
            account.UpdatedAt = DateTime.UtcNow;
            await _authRepository.UpdateAccount(account);
            return _mapper.Map<AccountAdminResponseDto>(account);  // Transform domain entity into DTO for the API response layer
        }


    }
}
