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
    public class AchievementService : IAchievementService
    {
        private readonly IAchievementRepository _repository;
        private readonly IMapper _mapper;
        private readonly IPlayerAchievementRepository _playerAchievementRepository;
        private readonly IPlayerProfileRepository _playerProfileRepository;
        private readonly IPlayerQuestRepository _playerQuestRepository;
        private readonly IRewardDeliveryService _rewardDeliveryService;
        private readonly ITransactionManager _transactionManager;

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

        public async Task<AchievementResponseDto?> GetAchievementById(int id)
        {
            var achievement = await _repository.GetAchievementByIdWithReward(id);
            if (achievement == null)
                return null;

            return _mapper.Map<AchievementResponseDto>(achievement);
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

        public async Task<PagedResultDto<AchievementResponseDto>> GetAchievementsPaged(int page, int pageSize, string? search, string? type, bool? isActive, string? sortBy = null, string? sortOrder = null)
        {
            var (totalCount, items) = await _repository.GetAchievementsPaged(page, pageSize, search, type, isActive, sortBy, sortOrder);

            var dtos = _mapper.Map<List<AchievementResponseDto>>(items);
            return new PagedResultDto<AchievementResponseDto>(totalCount, dtos);
        }



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

                // Re-fetch to include the Navigation properties like Achievement.IconUrl, etc.
                existingPA = await _playerAchievementRepository.GetByPlayerProfileId(playerProfileId);
            }

            await RecalculateProgress(playerProfileId, existingPA);

            var dtos = _mapper.Map<List<PlayerAchievementResponseDto>>(existingPA);

            return new PlayerMeAchievementsResponseDto
            {
                PlayerProfileId = playerProfileId,
                Achievements = dtos,
                TotalCount = dtos.Count,
                CompletedCount = dtos.Count(a => a.IsCompleted)
            };
        }

        // Progress trước đây luôn bằng 0 (chỉ được ghi lúc tạo dòng), nên điều kiện
        // "Progress >= RequiredValue" trong UnlockAchievement không bao giờ đạt được
        // => không thành tích nào có thể mở khoá. Ở đây tính lại Progress từ các bộ đếm
        // đã có sẵn (PlayerStat, PlayerProfile, PlayerQuest) mỗi lần người chơi mở bảng
        // thành tích. Không thêm cột/bảng mới.
        private async Task RecalculateProgress(int playerProfileId, List<PlayerAchievement> playerAchievements)
        {
            if (playerAchievements.Count == 0)
                return;

            var profile = await _playerProfileRepository.GetByIdFull(playerProfileId);
            if (profile == null)
                return;

            var stats = profile.PlayerStats;
            var quests = await _playerQuestRepository.GetByPlayerId(playerProfileId);
            // Claimed cũng là đã hoàn thành — chỉ khác ở chỗ đã nhận thưởng hay chưa.
            var questsDone = quests.Count(q => q.Status == "Completed" || q.Status == "Claimed");

            var changed = new List<PlayerAchievement>();
            foreach (var pa in playerAchievements)
            {
                // Đã hoàn thành thì chốt lại, không tính ngược khi bộ đếm thay đổi.
                if (pa.IsCompleted)
                    continue;

                int? progress = pa.AchievementId switch
                {
                    1 => Math.Min(questsDone, 1),                                             // Pioneer — xong chương đầu
                    2 => stats?.TotalKills,                                                   // Monster Hunter — 1.000 monster
                    3 => stats?.CritRate,                                                     // Deadeye — tổng Crit Rate
                    4 => (profile.Level >= 30 && (stats?.TotalDeaths ?? 0) < 10) ? 1 : 0,     // The Unyielding
                    7 => questsDone,                                                          // Adventurer — 100 quest
                    8 => profile.TotalDungeonClears,                                          // Faithful Companion — 100 co-op dungeon
                    // ponytail: 4 thành tích còn lại (5 Swift Wanderer, 6 Treasure Seeker,
                    // 9 Conqueror, 10 Legend of Elarion) giữ Progress = 0 vì BE chưa có bộ đếm:
                    // không có bảng vùng đã đi qua, IWorldRepository không có query đếm
                    // PlayerChest.IsOpened, không có log boss đã hạ, và không có hằng số max level.
                    // Muốn mở khoá thì phải thêm đúng bộ đếm còn thiếu rồi map thêm case ở đây.
                    _ => null
                };

                if (progress == null || progress.Value == pa.Progress)
                    continue;

                pa.Progress = progress.Value;
                changed.Add(pa);
            }

            // Update() gọi SaveChangesAsync mỗi lần -> dùng UpdateRange để chỉ ghi 1 lượt.
            if (changed.Count > 0)
                await _playerAchievementRepository.UpdateRange(changed);
        }

        public async Task<PlayerAchievementResponseDto> UnlockAchievement(int playerProfileId, int playerAchievementId)
        {
            return await _transactionManager.ExecuteInTransactionAsync(async () =>
            {
                var playerAchievement = await _playerAchievementRepository.GetByIdWithAchievement(playerAchievementId)
                    ?? throw new KeyNotFoundException($"Player achievement with id {playerAchievementId} not found.");

                if (playerAchievement.PlayerProfileId != playerProfileId)
                    throw new UnauthorizedAccessException("You cannot unlock another player's achievement.");

                var achievement = playerAchievement.Achievement
                    ?? throw new InvalidOperationException("Achievement data is missing.");

                // IsCompleted doubles as the idempotency marker: retries return the same result
                // without granting currency or items a second time.
                if (playerAchievement.IsCompleted)
                    return _mapper.Map<PlayerAchievementResponseDto>(playerAchievement);

                if (playerAchievement.Progress < achievement.RequiredValue)
                    throw new InvalidOperationException("Achievement progress is not high enough to unlock.");

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
                return _mapper.Map<PlayerAchievementResponseDto>(updated);
            }, IsolationLevel.Serializable);
        }
    }
}
