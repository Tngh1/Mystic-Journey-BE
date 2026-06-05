using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using System.Linq;

namespace BLL.Services
{
    public class PlayerProfileService : IPlayerProfileService
    {
        private readonly IPlayerProfileRepository _repository;
        private readonly IMapper _mapper;

        public PlayerProfileService(IPlayerProfileRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<PlayerProfileResponseDto>> GetAllProfiles()
        {
            var profiles = await _repository.GetAllPlayerProfilesWithAccounts();
            return profiles.Select(MapToResponseDto).ToList();
        }

        public async Task<PlayerProfileDetailResponseDto?> GetProfileById(int id)
        {
            var profile = await _repository.GetPlayerProfileByIdWithStats(id);
            if (profile == null)
                return null;

            return MapToDetailResponseDto(profile);
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

            if (request.Energy >= 0)
                profile.Energy = request.Energy;

            if (request.IsBanned.HasValue)
            {
                profile.PlayerStats ??= new PlayerStat
                {
                    PlayerProfileId = profile.PlayerProfileId,
                    CurrentHp = 100,
                    MaxHp = 100,
                    Atk = 10,
                    Def = 5,
                    MoveSpeed = 100,
                    AttackSpeed = 100,
                    CritRate = 5,
                    CritDamage = 150,
                    DamageBonus = 0,
                    CreatedAt = DateTime.UtcNow
                };
            }

            profile.UpdatedAt = DateTime.UtcNow;

            var updated = await _repository.UpdatePlayerProfile(profile);
            return MapToResponseDto(updated);
        }

        public IQueryable<PlayerProfileResponseDto> GetProfilesQueryable()
        {
            return _repository.GetPlayerProfilesQueryable()
                .Select(MapToResponseDto)
                .AsQueryable();
        }

        private static PlayerProfileResponseDto MapToResponseDto(PlayerProfile profile)
        {
            return new PlayerProfileResponseDto
            {
                Id = profile.PlayerProfileId,
                AccountId = profile.AccountId,
                AccountEmail = profile.Account?.Email,
                DisplayName = profile.DisplayName,
                AvatarUrl = string.IsNullOrEmpty(profile.AvatarUrl) ? null : profile.AvatarUrl,
                PlayerClass = profile.Class,
                Level = profile.Level,
                ExperiencePoints = profile.ExperiencePoints,
                Gold = profile.Gold,
                Gems = profile.Gems,
                Energy = profile.Energy,
                CreatedAt = profile.CreatedAt,
                UpdatedAt = profile.UpdatedAt,
                IsBanned = profile.Account != null && !profile.Account.IsActive
            };
        }

        private static PlayerProfileDetailResponseDto MapToDetailResponseDto(PlayerProfile profile)
        {
            return new PlayerProfileDetailResponseDto
            {
                Id = profile.PlayerProfileId,
                AccountId = profile.AccountId,
                AccountEmail = profile.Account?.Email,
                DisplayName = profile.DisplayName,
                AvatarUrl = string.IsNullOrEmpty(profile.AvatarUrl) ? null : profile.AvatarUrl,
                PlayerClass = profile.Class,
                Level = profile.Level,
                ExperiencePoints = profile.ExperiencePoints,
                Gold = profile.Gold,
                Gems = profile.Gems,
                Energy = profile.Energy,
                CreatedAt = profile.CreatedAt,
                UpdatedAt = profile.UpdatedAt,
                IsBanned = profile.Account != null && !profile.Account.IsActive,
                Stats = profile.PlayerStats != null ? new PlayerStatsResponseDto
                {
                    CurrentHp = profile.PlayerStats.CurrentHp,
                    MaxHp = profile.PlayerStats.MaxHp,
                    Atk = profile.PlayerStats.Atk,
                    Def = profile.PlayerStats.Def,
                    MoveSpeed = profile.PlayerStats.MoveSpeed,
                    AttackSpeed = profile.PlayerStats.AttackSpeed,
                    CritRate = profile.PlayerStats.CritRate,
                    CritDamage = profile.PlayerStats.CritDamage,
                    DamageBonus = profile.PlayerStats.DamageBonus,
                    SkillPoints = profile.PlayerStats.SkillPoints,
                    TotalWins = profile.PlayerStats.TotalWins,
                    TotalLosses = profile.PlayerStats.TotalLosses,
                    TotalKills = profile.PlayerStats.TotalKills,
                    TotalDeaths = profile.PlayerStats.TotalDeaths
                } : null
            };
        }
    }
}
