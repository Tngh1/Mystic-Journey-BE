using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BLL.Services
{
    public class AchievementService : IAchievementService
    {
        private readonly IAchievementRepository _repository;
        private readonly IMapper _mapper;
        private readonly IPlayerProfileRepository _playerProfileRepository;
        private readonly IPlayerAchievementRepository _playerAchievementRepository;

        public AchievementService(
            IAchievementRepository repository,
            IMapper mapper,
            IPlayerProfileRepository playerProfileRepository,
            IPlayerAchievementRepository playerAchievementRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _playerProfileRepository = playerProfileRepository;
            _playerAchievementRepository = playerAchievementRepository;
        }

        public async Task<AchievementResponseDto?> GetAchievementById(int id)
        {
            var achievement = await _repository.GetAchievementByIdWithReward(id);
            if (achievement == null)
                return null;

            return _mapper.Map<AchievementResponseDto>(achievement);
        }

        public async Task<AchievementResponseDto> CreateAchievement(CreateAchievementRequestDto request)
        {
            var achievement = _mapper.Map<Achievement>(request);
            achievement.CreatedAt = DateTime.UtcNow;

            var created = await _repository.CreateAchievement(achievement);
            return await GetAchievementById(created.AchievementId)
                ?? _mapper.Map<AchievementResponseDto>(created);
        }

        public async Task<AchievementResponseDto> UpdateAchievement(int id, UpdateAchievementRequestDto request)
        {
            var achievement = await _repository.GetAchievementByIdWithReward(id)
                ?? throw new KeyNotFoundException($"Achievement with id {id} not found.");

            achievement.Name = request.Name;
            achievement.Description = request.Description;
            achievement.Type = request.Type;
            achievement.IconUrl = request.IconUrl;
            achievement.RequiredValue = request.RequiredValue;
            achievement.IsActive = request.IsActive;
            achievement.RewardItemId = request.RewardItemId;
            achievement.RewardQuantity = request.RewardQuantity;
            achievement.RewardGold = request.RewardGold;
            achievement.RewardGem = request.RewardGem;
            achievement.Point = request.Point;

            var updated = await _repository.UpdateAchievement(achievement);
            return _mapper.Map<AchievementResponseDto>(updated);
        }

        public async Task<PagedResultDto<AchievementResponseDto>> GetAchievementsPaged(int page, int pageSize, string? search, string? type, bool? isActive)
        {
            var (totalCount, items) = await _repository.GetAchievementsPaged(page, pageSize, search, type, isActive);

            var dtos = _mapper.Map<List<AchievementResponseDto>>(items);
            return new PagedResultDto<AchievementResponseDto>(totalCount, dtos);
        }



        public async Task<PlayerMeAchievementsResponseDto> GetMeAchievements(int playerProfileId)
        {
            var achievements = await _playerAchievementRepository.GetByPlayerProfileId(playerProfileId);

            var dtos = _mapper.Map<List<PlayerAchievementResponseDto>>(achievements);

            return new PlayerMeAchievementsResponseDto
            {
                PlayerProfileId = playerProfileId,
                Achievements = dtos,
                TotalCount = dtos.Count,
                CompletedCount = dtos.Count(a => a.IsCompleted)
            };
        }
    }
}
