using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL.Services
{
    public class PlayerProfileService : IPlayerProfileService
    {
        private readonly IPlayerProfileRepository _repository;
        private readonly IMapper _mapper;
        private readonly IFriendRepository _friendRepository;

        public PlayerProfileService(
            IPlayerProfileRepository repository,
            IMapper mapper,
            IFriendRepository friendRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _friendRepository = friendRepository;
        }

        public bool RecalculateEnergy(PlayerProfile profile)
        {
            if (profile.CurrentEnergy >= profile.MaxEnergy)
            {
                profile.LastEnergyUpdateTime = DateTime.UtcNow;
                return false;
            }

            var now = DateTime.UtcNow;
            var timeElapsed = now - profile.LastEnergyUpdateTime;
            if (timeElapsed.TotalMinutes >= 6)
            {
                int energyToRegen = (int)(timeElapsed.TotalMinutes / 6);
                if (energyToRegen > 0)
                {
                    int newEnergy = profile.CurrentEnergy + energyToRegen;
                    if (newEnergy >= profile.MaxEnergy)
                    {
                        profile.CurrentEnergy = profile.MaxEnergy;
                        profile.LastEnergyUpdateTime = now;
                    }
                    else
                    {
                        profile.CurrentEnergy = newEnergy;
                        profile.LastEnergyUpdateTime = profile.LastEnergyUpdateTime.AddMinutes(energyToRegen * 6);
                    }
                    return true;
                }
            }
            return false;
        }

        public async Task<PlayerProfileDetailResponseDto?> GetProfileById(int id)
        {
            var profile = await _repository.GetPlayerProfileByIdWithStats(id);
            if (profile == null)
                return null;

            if (RecalculateEnergy(profile))
            {
                await _repository.UpdatePlayerProfile(profile);
            }

            return _mapper.Map<PlayerProfileDetailResponseDto>(profile);
        }

        public async Task<PlayerProfileResponseDto?> GetByAccountIdAsync(int accountId)
        {
            var profile = await _repository.GetByAccountId(accountId);
            if (profile == null)
                return null;

            RecalculateEnergy(profile);
            return _mapper.Map<PlayerProfileResponseDto>(profile);
        }

        public async Task<PlayerProfileResponseDto> UpdateProfile(int id, UpdatePlayerProfileRequestDto request)
        {
            var profile = await _repository.GetPlayerProfileById(id)
                ?? throw new KeyNotFoundException($"Player profile with id {id} not found.");

            if (request.DisplayName != null)
                profile.DisplayName = request.DisplayName;

            if (request.AvatarUrl != null)
                profile.AvatarUrl = request.AvatarUrl;

            if (request.PlayerClass != null)
                profile.Class = request.PlayerClass;

            if (request.Level > 0)
                profile.Level = request.Level;

            if (request.ExperiencePoints >= 0)
                profile.ExperiencePoints = request.ExperiencePoints;

            if (request.Gold >= 0)
                profile.Gold = request.Gold;

            if (request.Gems >= 0)
                profile.Gems = request.Gems;

            // Recalculate energy first before updating
            RecalculateEnergy(profile);

            if (request.Energy >= 0)
                profile.CurrentEnergy = request.Energy;

            if (request.MaxEnergy > 0)
                profile.MaxEnergy = request.MaxEnergy;

            if (request.CorruptionLevel.HasValue)
                profile.CorruptionLevel = request.CorruptionLevel.Value;

            profile.UpdatedAt = DateTime.UtcNow;

            var updated = await _repository.UpdatePlayerProfile(profile);
            return _mapper.Map<PlayerProfileResponseDto>(updated);
        }

        public async Task<PagedResultDto<PlayerProfileResponseDto>> GetProfilesPaged(int page, int pageSize, string? search, int? level)
        {
            var (totalCount, items) = await _repository.GetProfilesPaged(page, pageSize, search, level);
            var dtos = _mapper.Map<List<PlayerProfileResponseDto>>(items);
            return new PagedResultDto<PlayerProfileResponseDto>(totalCount, dtos);
        }



        public async Task<List<PlayerProfileResponseDto>> GetFriends(int playerProfileId)
        {
            var friends = await _friendRepository.GetFriends(playerProfileId);
            return _mapper.Map<List<PlayerProfileResponseDto>>(friends);
        }
    }
}
