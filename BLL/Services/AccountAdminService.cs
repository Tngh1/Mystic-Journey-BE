using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Repositories.Interfaces;

namespace BLL.Services
{
    public class AccountAdminService : IAccountAdminService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IMapper _mapper;

        public AccountAdminService(IAuthRepository authRepository, IMapper mapper)
        {
            _authRepository = authRepository;
            _mapper = mapper;
        }

        public async Task<AccountAdminResponseDto?> GetAccountById(int id)
        {
            var account = await _authRepository.GetAccountById(id);
            if (account == null)
                return null;

            return _mapper.Map<AccountAdminResponseDto>(account);
        }

        public async Task<PagedResultDto<AccountAdminResponseDto>> GetAccountsPaged(int page, int pageSize, string? search, bool? isActive, string? roleName)
        {
            var (totalCount, items) = await _authRepository.GetAccountsPaged(page, pageSize, search, isActive, roleName);

            var dtos = _mapper.Map<List<AccountAdminResponseDto>>(items);
            return new PagedResultDto<AccountAdminResponseDto>(totalCount, dtos);
        }

        public async Task<AccountAdminResponseDto> BanAccount(int accountId, string? banReason)
        {
            var account = await _authRepository.GetAccountById(accountId)
                ?? throw new KeyNotFoundException($"Account with id {accountId} not found.");
            account.IsActive = false;
            account.BanReason = string.IsNullOrWhiteSpace(banReason) ? null : banReason.Trim();
            account.UpdatedAt = DateTime.UtcNow;
            await _authRepository.UpdateAccount(account);
            return _mapper.Map<AccountAdminResponseDto>(account);
        }

        public async Task<AccountAdminResponseDto> UnbanAccount(int accountId)
        {
            var account = await _authRepository.GetAccountById(accountId)
                ?? throw new KeyNotFoundException($"Account with id {accountId} not found.");
            account.IsActive = true;
            account.BanReason = null;
            account.UpdatedAt = DateTime.UtcNow;
            await _authRepository.UpdateAccount(account);
            return _mapper.Map<AccountAdminResponseDto>(account);
        }


    }
}
