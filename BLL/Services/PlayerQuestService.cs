using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    // Executes core business logic for i player quest service.
    public class PlayerQuestService : IPlayerQuestService
    {
        private readonly IPlayerQuestRepository _playerQuestRepo;
        private readonly IPlayerProfileRepository _playerProfileRepo;
        private readonly IQuestRepository _questRepo;
        private readonly IInventoryRepository _inventoryRepo;
        private readonly ISkillRepository _skillRepo;
        private readonly ITransactionManager _transactionManager;

        private readonly IRewardDeliveryService _rewardDeliveryService;
        private const int MaxProgressDeltaPerCall = 50;

        private readonly IMapper _mapper;

        // Initialize this instance from player quest repo, player profile repo, quest repo, and inventory repo and store player quest repo, player profile repo, quest repo, inventory repo, and skill repo for later operations.
        public PlayerQuestService(
            IPlayerQuestRepository playerQuestRepo,
            IPlayerProfileRepository playerProfileRepo,
            IQuestRepository questRepo,
            IInventoryRepository inventoryRepo,
            ISkillRepository skillRepo,
            ITransactionManager transactionManager,
            IMapper mapper,
            IRewardDeliveryService rewardDeliveryService)
        {
            _playerQuestRepo = playerQuestRepo;
            _playerProfileRepo = playerProfileRepo;
            _questRepo = questRepo;
            _inventoryRepo = inventoryRepo;
            _skillRepo = skillRepo;
            _transactionManager = transactionManager;
            _mapper = mapper;
            _rewardDeliveryService = rewardDeliveryService;
        }

        // Executes core business logic for get my quests.
        // Logic details: throws KeyNotFoundException on invalid state or rule violations.
        // Returns the computed List<PlayerQuestResponseDto result asynchronously.
        public async Task<List<PlayerQuestResponseDto>> GetMyQuests(int playerProfileId)
        {
            var profile = await _playerProfileRepo.GetPlayerProfileById(playerProfileId)
                ?? throw new KeyNotFoundException($"PlayerProfile {playerProfileId} not found.");

            var mapName = NormalizeMapName(profile.LastMapName);

            var allPlayerQuests = await _playerQuestRepo.GetByPlayerId(playerProfileId);

            var allActiveQuests = await _questRepo.GetActiveQuests();

            var activeMapQuests = allActiveQuests
                .Where(q => string.Equals(NormalizeMapName(q.MapName), mapName, StringComparison.OrdinalIgnoreCase))  // Filter records matching the predicate
                .OrderBy(q => q.QuestId)  // Sort results oldest/lowest first
                .ToList();

            var existingMap = allPlayerQuests
                .GroupBy(pq => pq.QuestId)  // Aggregate records by grouping key
                .ToDictionary(g => g.Key, g => g.First());

            var createdAny = false;

            foreach (var quest in activeMapQuests.Where(q => !IsMainQuest(q) && q.RequiredLevel <= profile.Level))  // Filter records matching the predicate
            {
                if (existingMap.ContainsKey(quest.QuestId))
                    continue;

                try
                {
                    var created = await _playerQuestRepo.Create(CreateInitialQuestRecord(playerProfileId, quest));
                    existingMap[quest.QuestId] = created;
                    createdAny = true;
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException)
                {
                }
            }

            var mainChain = allActiveQuests
                .Where(IsMainQuest)  // Filter records matching the predicate
                .OrderBy(q => q.QuestId)  // Sort results oldest/lowest first
                .ToList();

            for (var i = 0; i < mainChain.Count; i++)
            {
                var quest = mainChain[i];
                if (!IsMainQuestUnlocked(mainChain, i, existingMap))
                    continue;
                if (existingMap.ContainsKey(quest.QuestId))
                    continue;

                try
                {
                    var created = await _playerQuestRepo.Create(CreateInitialQuestRecord(playerProfileId, quest));
                    existingMap[quest.QuestId] = created;
                    createdAny = true;
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException)
                {
                }
            }

            if (createdAny)
                allPlayerQuests = await _playerQuestRepo.GetByPlayerId(playerProfileId);

            existingMap = allPlayerQuests
                .GroupBy(pq => pq.QuestId)  // Aggregate records by grouping key
                .ToDictionary(g => g.Key, g => g.First());

            var visible = allPlayerQuests
                .Where(pq => !IsStatus(pq, "NotStarted")  // Filter records matching the predicate
                             || IsMainQuest(pq.Quest)
                             || (pq.Quest != null && string.Equals(NormalizeMapName(pq.Quest.MapName), mapName, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            var sortedVisible = visible
                .GroupBy(pq => pq.QuestId)  // Aggregate records by grouping key
                .Select(g => g.First())
                .OrderBy(pq => IsMainQuest(pq.Quest) ? 0 : 1)  // Sort results oldest/lowest first
                .ThenBy(pq => pq.QuestId)
                .ToList();

            return _mapper.Map<List<PlayerQuestResponseDto>>(sortedVisible);  // Transform domain entity into DTO for the API response layer
        }

        // Executes core business logic for get my quest detail.
        // Logic details: transforms domain entities into DTO transfer models; throws InvalidOperationException, KeyNotFoundException, ArgumentException on invalid state or rule violations.
        // Returns the computed PlayerQuestResponseDto? result asynchronously.
        public async Task<PlayerQuestResponseDto?> GetMyQuestDetail(int playerProfileId, int questId)
        {
            var quests = await GetMyQuests(playerProfileId);
            return quests.FirstOrDefault(q => q.QuestId == questId);
        }

        // Executes core business logic for accept quest.
        // Logic details: throws ArgumentException on invalid state or rule violations.
        // Returns the computed PlayerQuestResponseDto result asynchronously.
        public async Task<PlayerQuestResponseDto> AcceptQuest(int playerProfileId, AcceptQuestRequestDto request)
        {
            var quest = await _questRepo.GetByIdWithReward(request.QuestId)
                ?? throw new ArgumentException($"Quest {request.QuestId} does not exist.");

            if (!quest.IsActive)
                throw new InvalidOperationException($"Quest {request.QuestId} is inactive.");  // Unexpected runtime state — propagate to global error handler

            var profile = await _playerProfileRepo.GetPlayerProfileById(playerProfileId)
                ?? throw new KeyNotFoundException($"PlayerProfile {playerProfileId} not found.");

            var currentMap = NormalizeMapName(profile.LastMapName);
            if (!string.Equals(NormalizeMapName(quest.MapName), currentMap, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"Quest {request.QuestId} belongs to map {quest.MapName}, but player is currently in {currentMap}.");  // Unexpected runtime state — propagate to global error handler

            if (!IsMainQuest(quest) && profile.Level < quest.RequiredLevel)
                throw new InvalidOperationException($"Quest {request.QuestId} requires level {quest.RequiredLevel}.");  // Unexpected runtime state — propagate to global error handler

            if (IsMainQuest(quest) && !await IsMainQuestUnlocked(playerProfileId, quest))
                throw new InvalidOperationException($"Quest {request.QuestId} is locked until the previous main quest is claimed.");  // Unexpected runtime state — propagate to global error handler

            var existing = await _playerQuestRepo.GetByPlayerAndQuest(playerProfileId, request.QuestId);
            if (existing != null)  // Entity exists — proceed with conditional branch
            {
                if (existing.Status == "NotStarted" || existing.Status == "Failed" || existing.Status == "InProgress")
                {
                    if (existing.Status != "InProgress")
                    {
                        existing.Status = "InProgress";
                        existing.Progress = 0;
                        existing.AcceptedAt = DateTime.UtcNow;
                    }
                    existing.TargetValue = Math.Max(1, quest.TargetAmount);
                    existing = await _playerQuestRepo.Update(existing);
                }

                return _mapper.Map<PlayerQuestResponseDto>(existing);  // Transform domain entity into DTO for the API response layer
            }

            var playerQuest = CreateInitialQuestRecord(playerProfileId, quest);
            playerQuest.Status = "InProgress";
            playerQuest.Progress = 0;
            playerQuest.AcceptedAt = DateTime.UtcNow;

            var created = await _playerQuestRepo.Create(playerQuest);
            created.Quest = quest;
            return _mapper.Map<PlayerQuestResponseDto>(created);  // Transform domain entity into DTO for the API response layer
        }

        // Process the supplied values: maps the input discriminator to the corresponding domain value and fallback.
        public async Task<List<PlayerQuestResponseDto>> BatchUpdateProgress(
            int playerProfileId,
            BatchProgressRequestDto request)
        {
            if (request.Updates == null || request.Updates.Count == 0)
                return new List<PlayerQuestResponseDto>();

            var questIds = request.Updates.Select(u => u.QuestId).Distinct().ToList();
            var existingList = await _playerQuestRepo.GetByPlayerAndQuestIds(playerProfileId, questIds);
            var existingMap = existingList.GroupBy(pq => pq.QuestId).ToDictionary(g => g.Key, g => g.First());  // Aggregate records by grouping key

            var toUpdate = new List<PlayerQuest>();
            var results = new List<PlayerQuestResponseDto>();

            foreach (var item in request.Updates)
            {
                if (!existingMap.TryGetValue(item.QuestId, out var pq))
                    continue;

                if (pq.Status != "InProgress" || IsCollectQuest(pq.Quest))
                    continue;

                var targetAmount = Math.Max(1, pq.Quest?.TargetAmount ?? pq.TargetValue);
                if (pq.TargetValue != targetAmount)
                    pq.TargetValue = targetAmount;

                var delta = item.Progress - pq.Progress;
                if (delta < 0)
                    continue;

                var nextProgress = item.Progress;
                if (delta > MaxProgressDeltaPerCall)
                    nextProgress = pq.Progress + MaxProgressDeltaPerCall;

                pq.Progress = Math.Min(nextProgress, targetAmount);

                if (pq.Progress >= targetAmount && !IsCollectQuest(pq.Quest))
                {
                    pq.Status = "Completed";
                    pq.CompletedAt ??= DateTime.UtcNow;
                }

                toUpdate.Add(pq);
                results.Add(_mapper.Map<PlayerQuestResponseDto>(pq));  // Transform domain entity into DTO for the API response layer
            }

            if (toUpdate.Count > 0)
                await _playerQuestRepo.UpdateRange(toUpdate);

            return results;
        }

        // Executes core business logic for claim reward.
        // Logic details: transforms domain entities into DTO transfer models; throws InvalidOperationException, KeyNotFoundException, ArgumentException on invalid state or rule violations.
        // Returns the computed PlayerQuestResponseDto result asynchronously.
        public Task<PlayerQuestResponseDto> ClaimReward(int playerProfileId, ClaimQuestRequestDto request)
            => _transactionManager.ExecuteInTransactionAsync(() => ClaimRewardCore(playerProfileId, request));

        // Executes core business logic for claim reward core.
        // Logic details: throws ArgumentException on invalid state or rule violations.
        // Returns the computed PlayerQuestResponseDto result asynchronously.
        private async Task<PlayerQuestResponseDto> ClaimRewardCore(int playerProfileId, ClaimQuestRequestDto request)
        {
            var quest = await _questRepo.GetByIdWithReward(request.QuestId)
                ?? throw new ArgumentException($"Quest {request.QuestId} does not exist.");

            var pq = await _playerQuestRepo.GetByPlayerAndQuest(playerProfileId, request.QuestId)
                ?? throw new KeyNotFoundException($"PlayerQuest not found for questId={request.QuestId}.");

            if (pq.Status == "Claimed")
            {
                return _mapper.Map<PlayerQuestResponseDto>(pq);  // Transform domain entity into DTO for the API response layer
            }

            if (pq.Status != "Completed")
                throw new InvalidOperationException($"Quest {request.QuestId} is not completed yet.");  // Unexpected runtime state — propagate to global error handler

            pq.Status = "Claimed";
            pq.ClaimedAt = DateTime.UtcNow;
            await _playerQuestRepo.Update(pq);

            var profile = await _playerProfileRepo.GetPlayerProfileById(playerProfileId)
                ?? throw new KeyNotFoundException($"PlayerProfile {playerProfileId} not found.");

            profile.Gold += quest.RewardGold;
            profile.AddExperience(quest.RewardExperience);
            if (quest.RewardGems > 0)
                profile.Gems += quest.RewardGems;

            await _playerProfileRepo.UpdatePlayerProfile(profile);

            var rewardItems = quest.RewardItems
                .Where(item => item.ItemId > 0 && item.Quantity > 0)  // Filter records matching the predicate
                .GroupBy(item => item.ItemId)  // Aggregate records by grouping key
                .Select(group => new
                {
                    ItemId = group.Key,
                    Quantity = group.Sum(item => Math.Max(1, item.Quantity))
                })
                .ToList();

            if (rewardItems.Count > 0)
            {
                foreach (var rewardItem in rewardItems)
                    await AddItemToInventory(playerProfileId, rewardItem.ItemId, rewardItem.Quantity);
            }
            else if (quest.RewardItemId.HasValue)
            {
                await AddItemToInventory(playerProfileId, quest.RewardItemId.Value, 1);
            }

            var owned = await _skillRepo.GetPlayerSkillsByPlayerId(playerProfileId);
            var rewardSkillIds = quest.RewardSkills
                .Where(reward => reward.SkillId > 0 &&  // Filter records matching the predicate
                    reward.Skill != null &&
                    (reward.Skill.ClassRequirement == profile.Class || reward.Skill.ClassRequirement == "All"))
                .Select(skill => skill.SkillId)
                .Distinct()
                .ToList();

            if (rewardSkillIds.Count == 0 &&
                quest.RewardSkillId.HasValue &&
                quest.RewardSkill != null &&
                (quest.RewardSkill.ClassRequirement == profile.Class || quest.RewardSkill.ClassRequirement == "All"))
            {
                rewardSkillIds.Add(quest.RewardSkillId.Value);
            }

            foreach (var rewardSkillId in rewardSkillIds)
            {
                if (owned.Any(ps => ps.SkillId == rewardSkillId))
                    continue;

                var newPlayerSkill = await _skillRepo.CreatePlayerSkill(new PlayerSkill
                {
                    PlayerProfileId = playerProfileId,
                    SkillId = rewardSkillId,
                    Level = 1,
                    Experience = 0,
                    EquippedSlot = null,
                    UnlockedAt = DateTime.UtcNow
                });

                owned.Add(newPlayerSkill);
            }

            return _mapper.Map<PlayerQuestResponseDto>(pq);  // Transform domain entity into DTO for the API response layer
        }

        // Executes core business logic for complete quest.
        // Logic details: transforms domain entities into DTO transfer models; throws InvalidOperationException, KeyNotFoundException on invalid state or rule violations.
        // Returns the computed PlayerQuestResponseDto result asynchronously.
        public Task<PlayerQuestResponseDto> CompleteQuest(int playerProfileId, CompleteQuestRequestDto request)
            => _transactionManager.ExecuteInTransactionAsync(
                () => CompleteQuestCore(playerProfileId, request),
                IsolationLevel.Serializable);

        // Executes core business logic for complete quest core.
        // Logic details: throws KeyNotFoundException on invalid state or rule violations.
        // Returns the computed PlayerQuestResponseDto result asynchronously.
        private async Task<PlayerQuestResponseDto> CompleteQuestCore(int playerProfileId, CompleteQuestRequestDto request)
        {
            var pq = await _playerQuestRepo.GetByPlayerAndQuest(playerProfileId, request.QuestId)
                ?? throw new KeyNotFoundException($"PlayerQuest not found for questId={request.QuestId}.");

            if (pq.Status == "Claimed" || pq.Status == "Completed")
                return _mapper.Map<PlayerQuestResponseDto>(pq);  // Transform domain entity into DTO for the API response layer

            if (pq.Status != "InProgress")
                throw new InvalidOperationException($"Quest {request.QuestId} is not in progress.");  // Unexpected runtime state — propagate to global error handler

            if (IsCollectQuest(pq.Quest))
                throw new InvalidOperationException("Collect quests must be handed in to their NPC.");  // Unexpected runtime state — propagate to global error handler

            var targetAmount = Math.Max(1, pq.Quest?.TargetAmount ?? pq.TargetValue);
            pq.TargetValue = targetAmount;

            var canComplete = pq.Progress >= targetAmount ||
                IsTalkQuest(pq.Quest) ||
                (IsEquipSkillQuest(pq.Quest) && await HasEquippedSkill(playerProfileId));

            if (!canComplete)
                throw new InvalidOperationException("Quest objective is not complete yet.");  // Unexpected runtime state — propagate to global error handler

            await ConsumeQuestTurnInItemsIfNeeded(playerProfileId, pq.Quest);

            pq.Progress = targetAmount;
            pq.Status = "Completed";
            pq.CompletedAt ??= DateTime.UtcNow;

            var updated = await _playerQuestRepo.Update(pq);
            return _mapper.Map<PlayerQuestResponseDto>(updated);  // Transform domain entity into DTO for the API response layer
        }

        // Executes core business logic for consume quest turn in items if needed.
        // Logic details: throws InvalidOperationException on invalid state or rule violations.
        // Completes asynchronously upon successful execution.
        private async Task ConsumeQuestTurnInItemsIfNeeded(int playerProfileId, Quest? quest)
        {
            var requirements = ResolveQuestTurnInRequirement(quest);
            if (requirements == null || requirements.Count == 0)
                return;

            var invItems = await _inventoryRepo.GetByPlayerId(playerProfileId);

            foreach (var req in requirements)
            {
                var available = invItems
                    .Where(i => i.Item != null && Contains(i.Item.Name, req.itemName))  // Filter records matching the predicate
                    .Sum(i => i.Quantity);
                if (available < req.quantity)
                    throw new InvalidOperationException($"Need {req.quantity - available} more {req.itemName}.");  // Unexpected runtime state — propagate to global error handler
            }

            foreach (var req in requirements)
            {
                var remaining = req.quantity;
                foreach (var targetItem in invItems.Where(i =>  // Filter records matching the predicate
                             i.Item != null && Contains(i.Item.Name, req.itemName)))
                {
                    if (remaining <= 0)
                        break;

                    var consumedQuantity = Math.Min(targetItem.Quantity, remaining);
                    if (targetItem.Quantity <= consumedQuantity)
                        await _inventoryRepo.DeleteItem(targetItem.InventoryItemId);
                    else
                    {
                        targetItem.Quantity -= consumedQuantity;
                        await _inventoryRepo.UpdateItem(targetItem);
                    }

                    remaining -= consumedQuantity;
                }
            }
        }

        // Executes core business logic for resolve quest turn in requirement.
        private static List<(string itemName, int quantity)> ResolveQuestTurnInRequirement(Quest? quest)
        {
            var reqs = new List<(string, int)>();
            if (quest == null) return reqs;  // Entity not found — short-circuit with appropriate error result

            var text = $"{quest.Title} {quest.Description} {quest.ObjectiveTarget} {quest.ObjectiveLocation}";

            var isTurnInQuest = Contains(text, "Report") || Contains(text, "Return") || Contains(text, "Hand over") || Contains(text, "Handed over") || Contains(text, "Help") || Contains(text, "Deliver") || Contains(text, "Bury");

            if (!isTurnInQuest && reqs.Count == 0)
                return reqs;

            if ((Contains(text, "White Flower") || Contains(text, "White Flowers")) && quest.QuestId != 3)
                reqs.Add(("White Flower", 3));

            if (Contains(text, "Old Willow Branch") || Contains(text, "Willow Branch"))
                reqs.Add(("Old Willow Branch", Math.Max(1, quest.TargetAmount)));

            if ((Contains(text, "Pumpkin") || Contains(text, "Enchanted Pumpkin") ||
                (Contains(text, "Deliver") && Contains(text, "Harvest"))) && quest.QuestId != 11)
                reqs.Add(("Enchanted Pumpkin", Math.Max(1, quest.TargetAmount)));

            if (Contains(text, "Flour") || Contains(text, "Magic Flour"))
                reqs.Add(("Magic Flour", Math.Max(1, quest.TargetAmount)));

            if (Contains(text, "Bury Natalie") || Contains(text, "Lay Natalie to Rest"))
                reqs.Add(("Spirit Skull", 1));

            if (Contains(text, "Leaves") || Contains(text, "Ancient Leaves"))
            {
                if (isTurnInQuest || Contains(text, "collect 5 Ancient Leaves"))
                {
                    reqs.Add(("Ancient Leaves", Math.Max(1, quest.TargetAmount)));
                }
            }

            return reqs;
        }

        // Executes core business logic for contains.
        // Logic details: validates required non-empty string arguments.
        // Returns a boolean indicating operation success.
        private static bool Contains(string? source, string value)
        {
            return !string.IsNullOrWhiteSpace(source) &&
                   !string.IsNullOrWhiteSpace(value) &&
                   source.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        // Executes core business logic for add item to inventory.
        // Completes asynchronously upon successful execution.
        private async Task AddItemToInventory(int playerProfileId, int itemId, int quantity)
            => await _rewardDeliveryService.DeliverItemAsync(playerProfileId, itemId, quantity, "quest reward");

        // Executes core business logic for create initial quest record.
        private static PlayerQuest CreateInitialQuestRecord(int playerProfileId, Quest quest)
        {
            var targetAmount = Math.Max(1, quest.TargetAmount);
            return new PlayerQuest
            {
                PlayerProfileId = playerProfileId,
                QuestId = quest.QuestId,
                Status = string.Equals(quest.DefaultStatus, "InProgress", StringComparison.OrdinalIgnoreCase)
                    ? "InProgress"
                    : "NotStarted",
                Progress = 0,
                TargetValue = targetAmount,
                AcceptedAt = DateTime.UtcNow
            };
        }

        // Executes core business logic for is main quest unlocked.
        // Logic details: validates numeric boundary constraints.
        // Returns the computed bool result asynchronously.
        private async Task<bool> IsMainQuestUnlocked(int playerProfileId, Quest quest)
        {
            var mainChain = (await _questRepo.GetActiveQuests())
                .Where(IsMainQuest)  // Filter records matching the predicate
                .OrderBy(q => q.QuestId)  // Sort results oldest/lowest first
                .ToList();

            var index = mainChain.FindIndex(q => q.QuestId == quest.QuestId);
            if (index < 0)
                return false;
            if (index == 0)
                return true;

            var previousQuestId = mainChain[index - 1].QuestId;
            var previousRecord = await _playerQuestRepo.GetByPlayerAndQuest(playerProfileId, previousQuestId);
            return IsStatus(previousRecord, "Claimed");
        }

        // Executes core business logic for is main quest unlocked.
        // Logic details: validates numeric boundary constraints.
        // Returns a boolean indicating operation success.
        private static bool IsMainQuestUnlocked(List<Quest> mainChain, int index, IReadOnlyDictionary<int, PlayerQuest> existingMap)
        {
            if (index <= 0)
                return index == 0;

            var previousQuestId = mainChain[index - 1].QuestId;
            return existingMap.TryGetValue(previousQuestId, out var previousRecord) &&
                IsStatus(previousRecord, "Claimed");
        }

        // Executes core business logic for has equipped skill.
        // Logic details: validates required non-empty string arguments.
        // Returns the computed bool result asynchronously.
        private async Task<bool> HasEquippedSkill(int playerProfileId)
        {
            var skills = await _skillRepo.GetPlayerSkillsByPlayerId(playerProfileId);
            return skills.Any(ps => ps.EquippedSlot.HasValue);
        }

        // Executes core business logic for is main quest.
        // Logic details: validates required non-empty string arguments.
        // Returns a boolean indicating operation success.
        private static bool IsMainQuest(Quest? quest)
            => string.Equals(quest?.Type, "Main", StringComparison.OrdinalIgnoreCase);

        // Executes core business logic for is talk quest.
        // Logic details: validates required non-empty string arguments.
        // Returns a boolean indicating operation success.
        private static bool IsTalkQuest(Quest? quest)
            => string.Equals(quest?.ObjectiveType, "Talk", StringComparison.OrdinalIgnoreCase);

        // Executes core business logic for is collect quest.
        // Logic details: validates required non-empty string arguments.
        // Returns a boolean indicating operation success.
        private static bool IsCollectQuest(Quest? quest)
            => string.Equals(quest?.ObjectiveType, "Collect", StringComparison.OrdinalIgnoreCase);


        // Executes core business logic for is equip skill quest.
        // Logic details: validates required non-empty string arguments.
        // Returns a boolean indicating operation success.
        private static bool IsEquipSkillQuest(Quest? quest)
            => string.Equals(quest?.ObjectiveType, "EquipSkill", StringComparison.OrdinalIgnoreCase);

        // Executes core business logic for is status.
        // Logic details: validates required non-empty string arguments.
        // Returns a boolean indicating operation success.
        private static bool IsStatus(PlayerQuest? pq, string status)
            => string.Equals(pq?.Status, status, StringComparison.OrdinalIgnoreCase);

        // Normalizes world map names and maps aliases (such as ElfForest) to canonical map identifiers.
        private static string NormalizeMapName(string? mapName)
        {
            if (string.IsNullOrWhiteSpace(mapName))  // Mandatory string argument is blank — fail fast
                return "ElfForest";

            var normalized = mapName.Trim();
            return string.Equals(normalized, "ElfForest", StringComparison.OrdinalIgnoreCase)
                || string.Equals(normalized, "ElfLand", StringComparison.OrdinalIgnoreCase)
                || string.Equals(normalized, "Map1", StringComparison.OrdinalIgnoreCase)
                || string.Equals(normalized, "Chapter1", StringComparison.OrdinalIgnoreCase)
                || string.Equals(normalized, "Chapter 1", StringComparison.OrdinalIgnoreCase)
                    ? "ElfForest"
                    : normalized;
        }



    }
}
