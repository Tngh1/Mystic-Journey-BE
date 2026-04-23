using AutoMapper;
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
        private readonly IMapper _mapper;

        public PlayerProfileService(
            IPlayerProfileRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PlayerProfileApiResponseDto> CreateProfileAsync(Guid accountId, CreatePlayerProfileRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.DisplayName))
            {
                return new PlayerProfileApiResponseDto
                {
                    Success = false,
                    Message = "Display name is required."
                };
            }

            if (await _repository.ExistsAsync(accountId))
            {
                return new PlayerProfileApiResponseDto
                {
                    Success = false,
                    Message = "Player profile already exists for this account."
                };
            }

            var characterClass = Enum.IsDefined(typeof(PlayerProfile.CharacterClass), request.Class)
                ? (PlayerProfile.CharacterClass)request.Class
                : PlayerProfile.CharacterClass.Knight;

            var profile = new PlayerProfile
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                DisplayName = request.DisplayName.Trim(),
                AvatarUrl = request.AvatarUrl ?? string.Empty,
                Class = characterClass,
                Level = 1,
                ExperiencePoints = 0,
                Gold = 100,
                Gems = 10,
                Energy = 100,
                CreatedAt = DateTime.UtcNow
            };

            var stats = new PlayerStat
            {
                Id = Guid.NewGuid(),
                PlayerProfileId = profile.Id,
                Health = 100,
                Mana = 50,
                Strength = 10,
                Defense = 10,
                Agility = 10,
                Intelligence = 10,
                Endurance = 10,
                Luck = 0,
                SkillPoints = 5
            };

            profile.PlayerStats = stats;

            await _repository.CreateAsync(profile);

            return new PlayerProfileApiResponseDto
            {
                Success = true,
                Message = "Player profile created successfully.",
                Data = _mapper.Map<PlayerProfileResponseDto>(profile)
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
                    Message = "Player profile not found."
                };
            }

            return new PlayerProfileApiResponseDto
            {
                Success = true,
                Message = "Profile retrieved successfully.",
                Data = _mapper.Map<PlayerProfileResponseDto>(profile)
            };
        }

        public async Task<PlayerProfileApiResponseDto> GetProfileDetailByAccountIdAsync(Guid accountId)
        {
            var profile = await _repository.GetByAccountIdWithDetailsAsync(accountId);

            if (profile == null)
            {
                return new PlayerProfileApiResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var response = new PlayerProfileDetailResponseDto
            {
                ProfileId = profile.Id,
                AccountId = profile.AccountId,
                DisplayName = profile.DisplayName,
                AvatarUrl = profile.AvatarUrl,
                Class = profile.Class.ToString(),
                Level = profile.Level,
                ExperiencePoints = profile.ExperiencePoints,
                ExperienceToNextLevel = CalculateExperienceForLevel(profile.Level + 1),
                Gold = profile.Gold,
                Gems = profile.Gems,
                Energy = profile.Energy,
                CreatedAt = profile.CreatedAt,
                UpdatedAt = profile.UpdatedAt,
                Stats = profile.PlayerStats != null ? _mapper.Map<PlayerStatsResponseDto>(profile.PlayerStats) : null
            };

            return new PlayerProfileApiResponseDto
            {
                Success = true,
                Message = "Profile details retrieved successfully.",
                Detail = response
            };
        }

        public async Task<PlayerProfileApiResponseDto> UpdateProfileAsync(Guid accountId, UpdatePlayerProfileRequestDto request)
        {
            var profile = await _repository.GetByAccountIdAsync(accountId);

            if (profile == null)
            {
                return new PlayerProfileApiResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            if (!string.IsNullOrWhiteSpace(request.DisplayName))
            {
                profile.DisplayName = request.DisplayName.Trim();
            }

            if (request.AvatarUrl != null)
            {
                profile.AvatarUrl = request.AvatarUrl;
            }

            if (request.Class.HasValue && Enum.IsDefined(typeof(PlayerProfile.CharacterClass), request.Class.Value))
            {
                profile.Class = (PlayerProfile.CharacterClass)request.Class.Value;
            }

            await _repository.UpdateAsync(profile);

            return new PlayerProfileApiResponseDto
            {
                Success = true,
                Message = "Profile updated successfully.",
                Data = _mapper.Map<PlayerProfileResponseDto>(profile)
            };
        }

        public async Task<PlayerStatsResponseDto?> GetPlayerStatsAsync(Guid accountId)
        {
            var profile = await _repository.GetByAccountIdAsync(accountId);
            if (profile == null) return null;

            var stats = await _repository.GetStatsByProfileIdAsync(profile.Id);
            return stats != null ? _mapper.Map<PlayerStatsResponseDto>(stats) : null;
        }

        public async Task<PlayerCurrencyApiResponseDto> GetCurrencyAsync(Guid accountId)
        {
            var profile = await _repository.GetByAccountIdAsync(accountId);

            if (profile == null)
            {
                return new PlayerCurrencyApiResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            return new PlayerCurrencyApiResponseDto
            {
                Success = true,
                Message = "Currency retrieved successfully.",
                Data = new PlayerCurrencyResponseDto
                {
                    Gold = profile.Gold,
                    Gems = profile.Gems,
                    Energy = profile.Energy,
                    MaxEnergy = 100
                }
            };
        }

        public async Task<PlayerCurrencyApiResponseDto> AddCurrencyAsync(Guid accountId, CurrencyUpdateDto request)
        {
            var profile = await _repository.GetByAccountIdAsync(accountId);

            if (profile == null)
            {
                return new PlayerCurrencyApiResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            if (request.Amount <= 0)
            {
                return new PlayerCurrencyApiResponseDto
                {
                    Success = false,
                    Message = "Amount must be greater than zero."
                };
            }

            if (request.CurrencyType == (int)PlayerCurrencyLog.CurrencyType.Gold)
            {
                profile.Gold += request.Amount;
            }
            else if (request.CurrencyType == (int)PlayerCurrencyLog.CurrencyType.Gems)
            {
                profile.Gems += request.Amount;
            }

            await _repository.UpdateCurrencyAsync(profile.Id, profile.Gold, profile.Gems, null);

            return new PlayerCurrencyApiResponseDto
            {
                Success = true,
                Message = "Currency added successfully.",
                Data = new PlayerCurrencyResponseDto
                {
                    Gold = profile.Gold,
                    Gems = profile.Gems,
                    Energy = profile.Energy,
                    MaxEnergy = 100
                }
            };
        }

        public async Task<PlayerCurrencyApiResponseDto> SpendCurrencyAsync(Guid accountId, CurrencyUpdateDto request)
        {
            var profile = await _repository.GetByAccountIdAsync(accountId);

            if (profile == null)
            {
                return new PlayerCurrencyApiResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            if (request.Amount <= 0)
            {
                return new PlayerCurrencyApiResponseDto
                {
                    Success = false,
                    Message = "Amount must be greater than zero."
                };
            }

            if (request.CurrencyType == (int)PlayerCurrencyLog.CurrencyType.Gold)
            {
                if (profile.Gold < request.Amount)
                {
                    return new PlayerCurrencyApiResponseDto
                    {
                        Success = false,
                        Message = "Insufficient gold."
                    };
                }
                profile.Gold -= request.Amount;
            }
            else if (request.CurrencyType == (int)PlayerCurrencyLog.CurrencyType.Gems)
            {
                if (profile.Gems < request.Amount)
                {
                    return new PlayerCurrencyApiResponseDto
                    {
                        Success = false,
                        Message = "Insufficient gems."
                    };
                }
                profile.Gems -= request.Amount;
            }

            await _repository.UpdateCurrencyAsync(profile.Id, profile.Gold, profile.Gems, null);

            return new PlayerCurrencyApiResponseDto
            {
                Success = true,
                Message = "Currency spent successfully.",
                Data = new PlayerCurrencyResponseDto
                {
                    Gold = profile.Gold,
                    Gems = profile.Gems,
                    Energy = profile.Energy,
                    MaxEnergy = 100
                }
            };
        }

        public async Task<PlayerCurrencyApiResponseDto> UpdateEnergyAsync(Guid accountId, int energyChange)
        {
            var profile = await _repository.GetByAccountIdAsync(accountId);

            if (profile == null)
            {
                return new PlayerCurrencyApiResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var newEnergy = profile.Energy + energyChange;
            newEnergy = Math.Max(0, Math.Min(100, newEnergy));

            await _repository.UpdateCurrencyAsync(profile.Id, null, null, newEnergy);

            return new PlayerCurrencyApiResponseDto
            {
                Success = true,
                Message = "Energy updated successfully.",
                Data = new PlayerCurrencyResponseDto
                {
                    Gold = profile.Gold,
                    Gems = profile.Gems,
                    Energy = newEnergy,
                    MaxEnergy = 100
                }
            };
        }

        public async Task<bool> HasProfileAsync(Guid accountId)
        {
            return await _repository.ExistsAsync(accountId);
        }

        public async Task<PlayerProfileApiResponseDto> AddExperienceAsync(Guid accountId, int amount)
        {
            var profile = await _repository.GetByAccountIdWithDetailsAsync(accountId);

            if (profile == null)
            {
                return new PlayerProfileApiResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            profile.ExperiencePoints += amount;
            int levelsGained = 0;

            while (profile.ExperiencePoints >= CalculateExperienceForLevel(profile.Level + 1))
            {
                profile.ExperiencePoints -= CalculateExperienceForLevel(profile.Level + 1);
                profile.Level++;
                levelsGained++;

                if (profile.PlayerStats != null)
                {
                    profile.PlayerStats.SkillPoints += 1;
                    profile.PlayerStats.Health += 10;
                    profile.PlayerStats.Mana += 5;
                    profile.PlayerStats.Strength += 2;
                    profile.PlayerStats.Defense += 2;
                    profile.PlayerStats.Agility += 2;
                    await _repository.UpdateStatsAsync(profile.PlayerStats);
                }
            }

            await _repository.UpdateAsync(profile);

            var response = new PlayerProfileDetailResponseDto
            {
                ProfileId = profile.Id,
                AccountId = profile.AccountId,
                DisplayName = profile.DisplayName,
                AvatarUrl = profile.AvatarUrl,
                Class = profile.Class.ToString(),
                Level = profile.Level,
                ExperiencePoints = profile.ExperiencePoints,
                ExperienceToNextLevel = CalculateExperienceForLevel(profile.Level + 1),
                Gold = profile.Gold,
                Gems = profile.Gems,
                Energy = profile.Energy,
                Stats = profile.PlayerStats != null ? _mapper.Map<PlayerStatsResponseDto>(profile.PlayerStats) : null
            };

            return new PlayerProfileApiResponseDto
            {
                Success = true,
                Message = levelsGained > 0
                    ? $"Experience added. You leveled up {levelsGained} time(s)!"
                    : "Experience added successfully.",
                Detail = response
            };
        }

        private static int CalculateExperienceForLevel(int level)
        {
            return 100 * level * level;
        }
    }
}
