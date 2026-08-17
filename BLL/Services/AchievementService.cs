using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq;

namespace BLL.Services
{
    // Executes core business logic for i achievement service.
    public class AchievementService : IAchievementService
    {
        private readonly IAchievementRepository _repository;
        private readonly IMapper _mapper;
        private readonly IPlayerAchievementRepository _playerAchievementRepository;
        private readonly IPlayerProfileRepository _playerProfileRepository;
        private readonly IPlayerQuestRepository _playerQuestRepository;
        private readonly IRewardDeliveryService _rewardDeliveryService;
        private readonly ITransactionManager _transactionManager;

        // Initialize this instance from repository, mapper, player achievement repository, and player profile repository and store repository, mapper, player achievement repository, player profile repository, and player quest repository for later operations.
        public AchievementService(
            IAchievementRepository repository,
            IMapper mapper,
            IPlayerAchievementRepository playerAchievementRepository,
            IPlayerProfileRepository playerProfileRepository,
            IPlayerQuestRepository playerQuestRepository,
            IRewardDeliveryService rewardDeliveryService,
            ITransactionManager transactionManager)
        {
            _repository = repository;
            _mapper = mapper;
            _playerAchievementRepository = playerAchievementRepository;
            _playerProfileRepository = playerProfileRepository;
            _playerQuestRepository = playerQuestRepository;
            _rewardDeliveryService = rewardDeliveryService;
            _transactionManager = transactionManager;
        }

        // Executes core business logic for get achievement by id.
        // Logic details: delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models; throws KeyNotFoundException on invalid state or rule violations.
        // Returns the computed AchievementResponseDto? result asynchronously.
        public async Task<AchievementResponseDto?> GetAchievementById(int id)
        {
            var achievement = await _repository.GetAchievementByIdWithReward(id);
            if (achievement == null)  // Entity not found — short-circuit with appropriate error result
                return null;

            return _mapper.Map<AchievementResponseDto>(achievement);  // Transform domain entity into DTO for the API response layer
        }

        // Executes core business logic for update achievement.
        // Logic details: delegates data queries and updates to repository layer; throws KeyNotFoundException on invalid state or rule violations.
        // Returns the computed AchievementResponseDto result asynchronously.
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
            return _mapper.Map<AchievementResponseDto>(updated);  // Transform domain entity into DTO for the API response layer
        }

        // Executes core business logic for get achievements paged.
        // Logic details: delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models.
        // Returns the computed PagedResultDto<AchievementResponseDto result asynchronously.
        public async Task<PagedResultDto<AchievementResponseDto>> GetAchievementsPaged(int page, int pageSize, string? search, string? type, bool? isActive, string? sortBy = null, string? sortOrder = null)
        {
            var (totalCount, items) = await _repository.GetAchievementsPaged(page, pageSize, search, type, isActive, sortBy, sortOrder);

            var dtos = _mapper.Map<List<AchievementResponseDto>>(items);  // Transform domain entity into DTO for the API response layer
            return new PagedResultDto<AchievementResponseDto>(totalCount, dtos);
        }



        // Executes core business logic for get me achievements.
        // Logic details: delegates data queries and updates to repository layer.
        // Returns the computed PlayerMeAchievementsResponseDto result asynchronously.
        public async Task<PlayerMeAchievementsResponseDto> GetMeAchievements(int playerProfileId)
        {
            var allAchievements = await _repository.GetAllActiveAchievements();
            var existingPA = await _playerAchievementRepository.GetByPlayerProfileId(playerProfileId);
            var existingIds = existingPA.Select(pa => pa.AchievementId).ToHashSet();

            var newPAs = new List<PlayerAchievement>();
            foreach(var ach in allAchievements)
            {
                if (!existingIds.Contains(ach.AchievementId))
                {
                    var pa = new PlayerAchievement
                    {
                        PlayerProfileId = playerProfileId,
                        AchievementId = ach.AchievementId,
                        Progress = 0,
                        IsCompleted = false,
                        UnlockedAt = DateTime.UtcNow
                    };
                    newPAs.Add(pa);
                }
            }

            if (newPAs.Any())
            {
                await _playerAchievementRepository.AddRange(newPAs);

                existingPA = await _playerAchievementRepository.GetByPlayerProfileId(playerProfileId);
            }

            await RecalculateProgress(playerProfileId, existingPA);

            var dtos = _mapper.Map<List<PlayerAchievementResponseDto>>(existingPA);  // Transform domain entity into DTO for the API response layer

            return new PlayerMeAchievementsResponseDto
            {
                PlayerProfileId = playerProfileId,
                Achievements = dtos,
                TotalCount = dtos.Count,
                CompletedCount = dtos.Count(a => a.IsCompleted)
            };
        }

        // Executes core business logic for recalculate progress.
        // Logic details: delegates data queries and updates to repository layer.
        // Completes asynchronously upon successful execution.
        private async Task RecalculateProgress(int playerProfileId, List<PlayerAchievement> playerAchievements)
        {
            if (playerAchievements.Count == 0)
                return;

            var profile = await _playerProfileRepository.GetByIdFull(playerProfileId);
            if (profile == null)  // Entity not found — short-circuit with appropriate error result
                return;

            var stats = profile.PlayerStats;
            var quests = await _playerQuestRepository.GetByPlayerId(playerProfileId);
            var questsDone = quests.Count(q => q.Status == "Completed" || q.Status == "Claimed");

            var changed = new List<PlayerAchievement>();
            foreach (var pa in playerAchievements)
            {
                if (pa.IsCompleted)
                    continue;

                int? progress = pa.AchievementId switch
                {
                    1 => Math.Min(questsDone, 1),
                    2 => stats?.TotalKills,
                    3 => stats?.CritRate,
                    4 => (profile.Level >= 30 && (stats?.TotalDeaths ?? 0) < 10) ? 1 : 0,
                    7 => questsDone,
                    8 => profile.TotalDungeonClears,
                    _ => null
                };

                if (progress == null || progress.Value == pa.Progress)
                    continue;

                pa.Progress = progress.Value;
                changed.Add(pa);
            }

            if (changed.Count > 0)
                await _playerAchievementRepository.UpdateRange(changed);
        }

        // Executes core business logic for unlock achievement.
        // Logic details: delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models; throws InvalidOperationException, KeyNotFoundException, UnauthorizedAccessException on invalid state or rule violations.
        // Returns the computed PlayerAchievementResponseDto result asynchronously.
        public async Task<PlayerAchievementResponseDto> UnlockAchievement(int playerProfileId, int playerAchievementId)
        {
            return await _transactionManager.ExecuteInTransactionAsync(async () =>
            {
                var playerAchievement = await _playerAchievementRepository.GetByIdWithAchievement(playerAchievementId)
                    ?? throw new KeyNotFoundException($"Player achievement with id {playerAchievementId} not found.");

                if (playerAchievement.PlayerProfileId != playerProfileId)
                    throw new UnauthorizedAccessException("You cannot unlock another player's achievement.");  // Authentication token is invalid or expired

                var achievement = playerAchievement.Achievement
                    ?? throw new InvalidOperationException("Achievement data is missing.");  // Unexpected runtime state — propagate to global error handler

                if (playerAchievement.IsCompleted)
                    return _mapper.Map<PlayerAchievementResponseDto>(playerAchievement);  // Transform domain entity into DTO for the API response layer

                if (playerAchievement.Progress < achievement.RequiredValue)
                    throw new InvalidOperationException("Achievement progress is not high enough to unlock.");  // Unexpected runtime state — propagate to global error handler

                if (achievement.RewardGold > 0 || achievement.RewardGem > 0)
                {
                    var profile = await _playerProfileRepository.GetPlayerProfileById(playerProfileId)
                        ?? throw new KeyNotFoundException($"Player profile with id {playerProfileId} not found.");

                    profile.Gold += achievement.RewardGold;
                    profile.Gems += achievement.RewardGem;
                    await _playerProfileRepository.UpdatePlayerProfile(profile);
                }

                if (achievement.RewardItemId.HasValue && achievement.RewardQuantity > 0)
                {
                    await _rewardDeliveryService.DeliverItemAsync(
                        playerProfileId,
                        achievement.RewardItemId.Value,
                        achievement.RewardQuantity,
                        $"achievement '{achievement.Name}'");
                }

                playerAchievement.IsCompleted = true;
                playerAchievement.CompletedAt = DateTime.UtcNow;

                var updated = await _playerAchievementRepository.Update(playerAchievement);
                return _mapper.Map<PlayerAchievementResponseDto>(updated);  // Transform domain entity into DTO for the API response layer
            }, IsolationLevel.Serializable);
        }
    }
}
