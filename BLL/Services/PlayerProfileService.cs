using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class PlayerProfileService : IPlayerProfileService
    {
        private readonly IPlayerProfileRepository _repository;
        private readonly IAccountRepository _accountRepository;

        public PlayerProfileService(IPlayerProfileRepository repository, IAccountRepository accountRepository)
        {
            _repository = repository;
            _accountRepository = accountRepository;
        }

        public async Task<PlayerProfileApiResponseDto> GetProfileByIdAsync(int id)
        {
            var profile = await _repository.GetByIdAsync(id);

            if (profile == null)
            {
                return new PlayerProfileApiResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            return new PlayerProfileApiResponseDto
            {
                Success = true,
                Message = "Profile retrieved successfully.",
                Data = MapToDto(profile)
            };
        }

        public async Task<PlayerProfileApiResponseDto> GetProfileByAccountIdAsync(Guid accountId)
        {
            var profile = await _repository.GetByAccountIdAsync(accountId);

            if (profile == null)
            {
                return new PlayerProfileApiResponseDto
                {
                    Success = false,
                    Message = "Player profile not found for this account."
                };
            }

            return new PlayerProfileApiResponseDto
            {
                Success = true,
                Message = "Profile retrieved successfully.",
                Data = MapToDto(profile)
            };
        }

        public async Task<PlayerProfileApiResponseDto> CreateProfileAsync(CreatePlayerProfileRequestDto request)
        {
            var account = await _accountRepository.GetByIdAsync(request.AccountId);
            if (account == null)
            {
                return new PlayerProfileApiResponseDto
                {
                    Success = false,
                    Message = "Account does not exist."
                };
            }

            var existingProfile = await _repository.GetByAccountIdAsync(request.AccountId);
            if (existingProfile != null)
            {
                return new PlayerProfileApiResponseDto
                {
                    Success = false,
                    Message = "A profile already exists for this account."
                };
            }

            var newProfile = new PlayerProfile
            {
                AccountId = request.AccountId,
                DisplayName = request.DisplayName,
                AvatarUrl = request.AvatarUrl,
                Class = request.Class,
                Level = 1,
                ExperiencePoints = 0,
                Gold = 0,
                Gems = 0,
                Energy = 100,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(newProfile);

            return new PlayerProfileApiResponseDto
            {
                Success = true,
                Message = "Player profile created successfully.",
                Data = MapToDto(newProfile)
            };
        }

        public async Task<PlayerProfileApiResponseDto> UpdateProfileAsync(int id, UpdatePlayerProfileRequestDto request)
        {
            var profile = await _repository.GetByIdAsync(id);

            if (profile == null)
            {
                return new PlayerProfileApiResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            if (!string.IsNullOrEmpty(request.AvatarUrl)) profile.AvatarUrl = request.AvatarUrl;
            if (!string.IsNullOrEmpty(request.Class)) profile.Class = request.Class;
            
            profile.Level = request.Level;
            profile.ExperiencePoints = request.ExperiencePoints;
            profile.Gold = request.Gold;
            profile.Gems = request.Gems;
            profile.Energy = request.Energy;
            profile.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(profile);

            return new PlayerProfileApiResponseDto
            {
                Success = true,
                Message = "Player profile updated successfully.",
                Data = MapToDto(profile)
            };
        }

        private static PlayerProfileResponseDto MapToDto(PlayerProfile profile)
        {
            return new PlayerProfileResponseDto
            {
                Id = profile.Id,
                AccountId = profile.AccountId,
                DisplayName = profile.DisplayName,
                AvatarUrl = profile.AvatarUrl,
                Class = profile.Class,
                Level = profile.Level,
                ExperiencePoints = profile.ExperiencePoints,
                Gold = profile.Gold,
                Gems = profile.Gems,
                Energy = profile.Energy,
                CreatedAt = profile.CreatedAt,
                UpdatedAt = profile.UpdatedAt
            };
        }
    }
}
