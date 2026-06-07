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

        public AchievementService(IAchievementRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<AchievementResponseDto?> GetAchievementById(int id)
        {
            var achievement = await _repository.GetAchievementByIdWithReward(id);
            if (achievement == null)
                return null;

            return MapToResponseDto(achievement);
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

            var updated = await _repository.UpdateAchievement(achievement);
            return MapToResponseDto(updated);
        }

        public async Task<PagedResultDto<AchievementResponseDto>> GetAchievementsPaged(int page, int pageSize, string? search, string? type, bool? isActive)
        {
            var (totalCount, items) = await _repository.GetAchievementsPaged(page, pageSize, search, type, isActive);

            var dtos = items.Select(MapToResponseDto).ToList();
            return new PagedResultDto<AchievementResponseDto>(totalCount, dtos);
        }

        private static AchievementResponseDto MapToResponseDto(Achievement achievement)
        {
            return new AchievementResponseDto
            {
                Id = achievement.AchievementId,
                Name = achievement.Name,
                Description = achievement.Description,
                Type = achievement.Type,
                IconUrl = achievement.IconUrl,
                RequiredValue = achievement.RequiredValue,
                IsActive = achievement.IsActive,
                CreatedAt = achievement.CreatedAt,
                RewardItemId = achievement.RewardItemId,
                RewardItemName = achievement.RewardItem?.Name,
                RewardQuantity = achievement.RewardQuantity,
                RewardGold = achievement.RewardGold,
                RewardGem = achievement.RewardGem
            };
        }
    }
}
