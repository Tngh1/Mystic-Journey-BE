using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class PlayerQuestService : IPlayerQuestService
    {
        private readonly IPlayerQuestRepository _playerQuestRepo;
        private readonly IPlayerProfileRepository _playerProfileRepo;
        private readonly IQuestRepository _questRepo;
        private readonly MysticJourneyDbContext _context;

        // Anti-cheat: max progress delta per batch call.
        private const int MaxProgressDeltaPerCall = 50;

        public PlayerQuestService(
            IPlayerQuestRepository playerQuestRepo,
            IPlayerProfileRepository playerProfileRepo,
            IQuestRepository questRepo,
            MysticJourneyDbContext context)
        {
            _playerQuestRepo = playerQuestRepo;
            _playerProfileRepo = playerProfileRepo;
            _questRepo = questRepo;
            _context = context;
        }

        public async Task<List<PlayerQuestResponseDto>> GetMyQuests(int playerProfileId)
        {
            var profile = await _playerProfileRepo.GetPlayerProfileById(playerProfileId)
                ?? throw new KeyNotFoundException($"PlayerProfile {playerProfileId} not found.");

            var mapName = NormalizeMapName(profile.LastMapName);
            var activeMapQuests = (await _questRepo.GetActiveQuests())
                .Where(q => q.MapName == mapName)
                .ToList();

            var records = await _playerQuestRepo.GetByPlayerIdAndMap(playerProfileId, mapName);
            var existingQuestIds = records.Select(pq => pq.QuestId).ToHashSet();

            foreach (var quest in activeMapQuests.Where(q => !existingQuestIds.Contains(q.QuestId)))
            {
                await _playerQuestRepo.Create(new PlayerQuest
                {
                    PlayerProfileId = playerProfileId,
                    QuestId = quest.QuestId,
                    Status = quest.DefaultStatus == "InProgress" ? "InProgress" : "NotStarted",
                    Progress = 0,
                    TargetValue = Math.Max(1, quest.TargetAmount),
                    AcceptedAt = DateTime.UtcNow
                });
            }

            records = await _playerQuestRepo.GetByPlayerIdAndMap(playerProfileId, mapName);
            return records.Select(MapToDto).ToList();
        }

        public async Task<PlayerQuestResponseDto?> GetMyQuestDetail(int playerProfileId, int questId)
        {
            var quests = await GetMyQuests(playerProfileId);
            return quests.FirstOrDefault(q => q.QuestId == questId);
        }

        public async Task<PlayerQuestResponseDto> AcceptQuest(int playerProfileId, AcceptQuestRequestDto request)
        {
            var quest = await _questRepo.GetByIdWithReward(request.QuestId)
                ?? throw new ArgumentException($"Quest {request.QuestId} does not exist.");

            if (!quest.IsActive)
                throw new InvalidOperationException($"Quest {request.QuestId} is inactive.");

            var profile = await _playerProfileRepo.GetPlayerProfileById(playerProfileId)
                ?? throw new KeyNotFoundException($"PlayerProfile {playerProfileId} not found.");

            var currentMap = NormalizeMapName(profile.LastMapName);
            if (quest.MapName != currentMap)
                throw new InvalidOperationException($"Quest {request.QuestId} belongs to map {quest.MapName}, but player is currently in {currentMap}.");

            if (profile.Level < quest.RequiredLevel)
                throw new InvalidOperationException($"Quest {request.QuestId} requires level {quest.RequiredLevel}.");

            var existing = await _playerQuestRepo.GetByPlayerAndQuest(playerProfileId, request.QuestId);
            if (existing != null)
            {
                if (existing.Status == "NotStarted" || existing.Status == "Failed")
                {
                    existing.Status = "InProgress";
                    existing.Progress = 0;
                    existing.TargetValue = Math.Max(1, quest.TargetAmount);
                    existing.AcceptedAt = DateTime.UtcNow;
                    existing.CompletedAt = null;
                    existing.ClaimedAt = null;
                    existing = await _playerQuestRepo.Update(existing);
                }
                return MapToDto(existing);
            }

            var targetAmount = Math.Max(1, quest.TargetAmount);
            var playerQuest = new PlayerQuest
            {
                PlayerProfileId = playerProfileId,
                QuestId = request.QuestId,
                Status = "InProgress",
                Progress = 0,
                TargetValue = targetAmount,
                AcceptedAt = DateTime.UtcNow,
            };

            var created = await _playerQuestRepo.Create(playerQuest);
            return MapToDto(created);
        }

        public async Task<List<PlayerQuestResponseDto>> BatchUpdateProgress(
            int playerProfileId,
            BatchProgressRequestDto request)
        {
            if (request.Updates == null || request.Updates.Count == 0)
                return new List<PlayerQuestResponseDto>();

            var questIds = request.Updates.Select(u => u.QuestId).Distinct().ToList();
            var existingList = await _playerQuestRepo.GetByPlayerAndQuestIds(playerProfileId, questIds);
            var existingMap = existingList.ToDictionary(pq => pq.QuestId);

            var toUpdate = new List<PlayerQuest>();
            var results = new List<PlayerQuestResponseDto>();

            foreach (var item in request.Updates)
            {
                if (!existingMap.TryGetValue(item.QuestId, out var pq))
                    continue;

                if (pq.Status != "InProgress")
                    continue;

                var targetAmount = Math.Max(1, pq.TargetValue);
                if (pq.TargetValue != targetAmount)
                    pq.TargetValue = targetAmount;

                var delta = item.Progress - pq.Progress;
                if (delta < 0)
                    continue;

                var nextProgress = item.Progress;
                if (delta > MaxProgressDeltaPerCall)
                    nextProgress = pq.Progress + MaxProgressDeltaPerCall;

                pq.Progress = Math.Min(nextProgress, targetAmount);

                if (pq.Progress >= targetAmount)
                {
                    pq.Status = "Completed";
                    pq.CompletedAt = DateTime.UtcNow;
                }

                toUpdate.Add(pq);
                results.Add(MapToDto(pq));
            }

            if (toUpdate.Count > 0)
                await _playerQuestRepo.UpdateRange(toUpdate);

            return results;
        }

        public async Task<PlayerQuestResponseDto> ClaimReward(int playerProfileId, ClaimQuestRequestDto request)
        {
            var pq = await _playerQuestRepo.GetByPlayerAndQuest(playerProfileId, request.QuestId)
                ?? throw new KeyNotFoundException($"PlayerQuest not found for questId={request.QuestId}.");

            if (pq.Status != "Completed")
                throw new InvalidOperationException($"Quest {request.QuestId} is not Completed (status={pq.Status}).");

            var quest = await _questRepo.GetByIdWithReward(request.QuestId)
                ?? throw new ArgumentException($"Quest {request.QuestId} does not exist.");

            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                pq.Status = "Claimed";
                pq.ClaimedAt = DateTime.UtcNow;
                await _playerQuestRepo.Update(pq);

                var profile = await _playerProfileRepo.GetPlayerProfileById(playerProfileId)
                    ?? throw new KeyNotFoundException($"PlayerProfile {playerProfileId} not found.");

                profile.Gold += quest.RewardGold;
                profile.ExperiencePoints += quest.RewardExperience;
                if (quest.RewardGems > 0)
                    profile.Gems += quest.RewardGems;

                await _playerProfileRepo.UpdatePlayerProfile(profile);

                if (quest.RewardItemId.HasValue)
                    await AddItemToInventory(playerProfileId, quest.RewardItemId.Value, 1);

                await tx.CommitAsync();
                return MapToDto(pq);
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<PlayerQuestResponseDto> CompleteQuest(int playerProfileId, CompleteQuestRequestDto request)
        {
            var pq = await _playerQuestRepo.GetByPlayerAndQuest(playerProfileId, request.QuestId)
                ?? throw new KeyNotFoundException($"PlayerQuest not found for questId={request.QuestId}.");

            if (pq.Status == "Claimed")
                return MapToDto(pq);

            if (pq.Status != "InProgress" && pq.Status != "Completed")
                throw new InvalidOperationException($"Quest {request.QuestId} is not in progress.");

            var targetAmount = Math.Max(1, pq.Quest?.TargetAmount ?? pq.TargetValue);
            pq.TargetValue = targetAmount;
            pq.Progress = targetAmount;
            pq.Status = "Completed";
            pq.CompletedAt ??= DateTime.UtcNow;

            var updated = await _playerQuestRepo.Update(pq);
            return MapToDto(updated);
        }

        private async Task AddItemToInventory(int playerProfileId, int itemId, int quantity)
        {
            var existing = await _context.InventoryItems
                .FirstOrDefaultAsync(i => i.PlayerProfileId == playerProfileId && i.ItemId == itemId);

            if (existing != null)
            {
                existing.Quantity += quantity;
                _context.InventoryItems.Update(existing);
            }
            else
            {
                await _context.InventoryItems.AddAsync(new InventoryItem
                {
                    PlayerProfileId = playerProfileId,
                    ItemId = itemId,
                    Quantity = quantity,
                    IsEquipped = false,
                    IsSkin = false,
                    EnhancementLevel = 0
                });
            }

            await _context.SaveChangesAsync();
        }

        private static string NormalizeMapName(string? mapName)
            => string.IsNullOrWhiteSpace(mapName) ? "ElfForest" : mapName.Trim();

        private static PlayerQuestResponseDto MapToDto(PlayerQuest pq) => new()
        {
            PlayerQuestId = pq.PlayerQuestId,
            QuestId = pq.QuestId,
            QuestTitle = pq.Quest?.Title ?? string.Empty,
            QuestDescription = pq.Quest?.Description,
            QuestType = pq.Quest?.Type ?? "Main",
            MapName = pq.Quest?.MapName ?? "ElfForest",
            RegionName = pq.Quest?.RegionName,
            ObjectiveType = pq.Quest?.ObjectiveType ?? "Explore",
            ObjectiveTarget = pq.Quest?.ObjectiveTarget,
            ObjectiveLocation = pq.Quest?.ObjectiveLocation,
            QuestGiverName = pq.Quest?.QuestGiverName,
            Status = pq.Status,
            Progress = pq.Progress,
            TargetAmount = Math.Max(1, pq.Quest?.TargetAmount ?? pq.TargetValue),
            RequiredLevel = pq.Quest?.RequiredLevel ?? 1,
            RewardExperience = pq.Quest?.RewardExperience ?? 0,
            RewardGold = pq.Quest?.RewardGold ?? 0,
            RewardGems = pq.Quest?.RewardGems ?? 0,
            RewardItemId = pq.Quest?.RewardItemId,
            RewardItemName = pq.Quest?.RewardItem?.Name,
            AcceptedAt = pq.AcceptedAt,
            CompletedAt = pq.CompletedAt,
            ClaimedAt = pq.ClaimedAt,
        };
    }
}
