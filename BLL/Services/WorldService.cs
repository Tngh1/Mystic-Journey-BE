using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using System.Data;

namespace BLL.Services
{
    // Executes core business logic for i world service.
    public class WorldService : IWorldService
    {
        private const int MaxNpcsPerMap = 4;
        private const string TutorialMapName = "ElfForest";
        private const double TutorialSpawnX = 11.9;
        private const double TutorialSpawnY = 17.8;

        private readonly IPlayerProfileRepository _playerProfileRepository;
        private readonly IPlayerQuestService _playerQuestService;
        private readonly IPlayerProfileService _playerProfileService;
        private readonly IWorldRepository _worldRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly ITransactionManager _transactionManager;
        private readonly IPlayerQuestRepository _playerQuestRepository;
        private readonly IQuestRepository _questRepository;
        private readonly IMapper _mapper;
        private readonly IRewardDeliveryService _rewardDeliveryService;

        // Initialize this instance from player profile repository, player quest service, player profile service, and world repository and store player profile repository, player quest service, player profile service, world repository, and item repository for later operations.
        public WorldService(
            IPlayerProfileRepository playerProfileRepository,
            IPlayerQuestService playerQuestService,
            IPlayerProfileService playerProfileService,
            IWorldRepository worldRepository,
            IItemRepository itemRepository,
            IInventoryRepository inventoryRepository,
            ITransactionManager transactionManager,
            IPlayerQuestRepository playerQuestRepository,
            IQuestRepository questRepository,
            IMapper mapper,
            IRewardDeliveryService rewardDeliveryService)
        {
            _playerProfileRepository = playerProfileRepository;
            _playerQuestService = playerQuestService;
            _playerProfileService = playerProfileService;
            _worldRepository = worldRepository;
            _itemRepository = itemRepository;
            _inventoryRepository = inventoryRepository;
            _transactionManager = transactionManager;
            _playerQuestRepository = playerQuestRepository;
            _questRepository = questRepository;
            _mapper = mapper;
            _rewardDeliveryService = rewardDeliveryService;
        }

        // Executes core business logic for get world state.
        // Logic details: delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models.
        // Returns the computed WorldStateResponseDto result asynchronously.
        public async Task<WorldStateResponseDto> GetWorldState(int playerProfileId)
        {
            var profile = await GetProfile(playerProfileId);
            await EnsureTutorialSpawn(profile);
            var mapName = NormalizeMapName(profile.LastMapName);

            var npcs = await _worldRepository.GetNpcsByMapName(mapName, MaxNpcsPerMap);

            var quests = await _playerQuestService.GetMyQuests(playerProfileId);
            var dailyLogin = await GetDailyLogin(playerProfileId);

            return new WorldStateResponseDto
            {
                PlayerProfileId = playerProfileId,
                Position = new PlayerWorldPositionDto
                {
                    MapName = mapName,
                    PositionX = profile.PositionX,
                    PositionY = profile.PositionY
                },
                Maps = await BuildMapProgress(playerProfileId, mapName),
                Npcs = _mapper.Map<List<NPCResponseDto>>(npcs),  // Transform domain entity into DTO for the API response layer
                Quests = quests,
                ActiveQuest = quests.FirstOrDefault(q => q.Status == "InProgress")
                    ?? quests.FirstOrDefault(q => q.Status == "Completed")
                    ?? quests.FirstOrDefault(q => q.Status == "NotStarted"),
                DailyLogin = dailyLogin
            };
        }

        // Executes core business logic for get position.
        // Logic details: delegates data queries and updates to repository layer; throws KeyNotFoundException on invalid state or rule violations.
        // Returns the computed PlayerWorldPositionDto result asynchronously.
        public async Task<PlayerWorldPositionDto> GetPosition(int playerProfileId)
        {
            var profile = await _playerProfileRepository.GetPlayerProfileById(playerProfileId)
                ?? throw new KeyNotFoundException($"PlayerProfile {playerProfileId} not found.");
            await EnsureTutorialSpawn(profile);

            return new PlayerWorldPositionDto
            {
                MapName = NormalizeMapName(profile.LastMapName),
                PositionX = profile.PositionX,
                PositionY = profile.PositionY
            };
        }

        // Executes core business logic for update position.
        // Logic details: delegates data queries and updates to repository layer.
        // Returns the computed PlayerWorldPositionDto result asynchronously.
        public async Task<PlayerWorldPositionDto> UpdatePosition(int playerProfileId, UpdateWorldPositionRequestDto request)
        {
            var profile = await GetProfile(playerProfileId);
            profile.LastMapName = NormalizeMapName(request.MapName);
            profile.PositionX = request.PositionX;
            profile.PositionY = request.PositionY;
            await _playerProfileRepository.UpdatePlayerProfile(profile);

            return new PlayerWorldPositionDto
            {
                MapName = profile.LastMapName,
                PositionX = profile.PositionX,
                PositionY = profile.PositionY
            };
        }

        // Executes core business logic for talk to npc.
        // Logic details: delegates data queries and updates to repository layer; throws KeyNotFoundException on invalid state or rule violations.
        // Returns the computed TalkToNpcResponseDto result asynchronously.
        public async Task<TalkToNpcResponseDto> TalkToNpc(int playerProfileId, TalkToNpcRequestDto request)
        {
            var profile = await GetProfile(playerProfileId);
            var mapName = NormalizeMapName(profile.LastMapName);

            var npc = await _worldRepository.GetNpcById(request.NPCId)
                ?? throw new KeyNotFoundException($"NPC {request.NPCId} not found.");

            if (npc.MapName != mapName)
                throw new InvalidOperationException($"NPC {request.NPCId} is on map {npc.MapName}, but player is currently in {mapName}.");  // Unexpected runtime state — propagate to global error handler

            var linkedQuestIds = npc.Dialogues
                .Where(d => d.LinkedQuestId.HasValue)  // Filter records matching the predicate
                .Select(d => d.LinkedQuestId!.Value)
                .ToHashSet();
            var npcMapName = NormalizeMapName(npc.MapName);
            var playerQuests = await _playerQuestService.GetMyQuests(playerProfileId);
            var linkedQuests = playerQuests
                .Where(q => linkedQuestIds.Contains(q.QuestId)  // Filter records matching the predicate
                    || ((string.Equals(q.QuestGiverName, npc.Name, StringComparison.OrdinalIgnoreCase) || string.Equals(q.ObjectiveTarget, npc.Name, StringComparison.OrdinalIgnoreCase))
                        && string.Equals(NormalizeMapName(q.MapName), npcMapName, StringComparison.OrdinalIgnoreCase)))
                .GroupBy(q => q.QuestId)  // Aggregate records by grouping key
                .Select(g => g.First())
                .ToList();

            return new TalkToNpcResponseDto
            {
                Npc = _mapper.Map<NPCResponseDto>(npc),  // Transform domain entity into DTO for the API response layer
                LinkedQuests = linkedQuests
            };
        }

        // Executes core business logic for interact with object.
        // Logic details: delegates data queries and updates to repository layer; throws InvalidOperationException, KeyNotFoundException on invalid state or rule violations.
        // Returns the computed InteractObjectResponseDto result asynchronously.
        public Task<InteractObjectResponseDto> InteractWithObject(int playerProfileId, InteractObjectRequestDto request)
        {
            if (!request.QuestId.HasValue)
                return InteractWithObjectCore(playerProfileId, request);

            return _transactionManager.ExecuteInTransactionAsync(
                () => InteractWithObjectCore(playerProfileId, request),
                IsolationLevel.ReadCommitted);
        }

        // Executes core business logic for interact with object core.
        // Logic details: throws InvalidOperationException on invalid state or rule violations.
        // Returns the computed InteractObjectResponseDto result asynchronously.
        private async Task<InteractObjectResponseDto> InteractWithObjectCore(int playerProfileId, InteractObjectRequestDto request)
        {
            var profile = await GetProfile(playerProfileId);
            var mapName = NormalizeMapName(profile.LastMapName);
            var progressDelta = Math.Max(1, request.ProgressDelta);

            if (NormalizeMapName(request.MapName) != mapName)
            {
                throw new InvalidOperationException($"Player is currently in {mapName}, not {request.MapName}.");  // Unexpected runtime state — propagate to global error handler
            }

            if (!request.QuestId.HasValue)
            {
                var collectedItem = await TryCollectQuestItem(playerProfileId, request, null);
                return new InteractObjectResponseDto
                {
                    Success = true,
                    Message = collectedItem == null
                        ? $"Object {request.ObjectKey} interacted."
                        : $"Collected {DisplayItemName(collectedItem.Name)}.",
                    CollectedItemId = collectedItem?.ItemId,
                    CollectedItemName = collectedItem?.Name,
                    CollectedQuantity = collectedItem == null ? 0 : progressDelta
                };
            }

            var playerQuest = await _playerQuestRepository.GetByPlayerAndQuest(playerProfileId, request.QuestId.Value)
                ?? throw new KeyNotFoundException($"PlayerQuest not found for questId={request.QuestId.Value}.");

            if (playerQuest.Quest?.MapName != mapName)
                throw new InvalidOperationException($"Quest {request.QuestId.Value} does not belong to current map {mapName}.");  // Unexpected runtime state — propagate to global error handler

            if (playerQuest.Status != "InProgress")
            {
                return new InteractObjectResponseDto
                {
                    Success = true,
                    Message = $"Object {request.ObjectKey} interacted, but quest is {playerQuest.Status}."
                };
            }

            ValidateQuestInteraction(request, playerQuest.Quest);
            await ConsumeRequiredInteractionItems(playerProfileId, request.ObjectKey);

            var questItem = await TryCollectQuestItem(playerProfileId, request, playerQuest.Quest);
            var targetAmount = Math.Max(1, playerQuest.Quest?.TargetAmount ?? playerQuest.TargetValue);

            if (IsCollectQuest(playerQuest.Quest))
            {
                if (progressDelta != targetAmount)
                    throw new InvalidOperationException($"Collect quest {request.QuestId.Value} must be committed with exactly {targetAmount} items.");  // Unexpected runtime state — propagate to global error handler

                playerQuest.TargetValue = targetAmount;
                playerQuest.Progress = targetAmount;

                await _playerQuestRepository.Update(playerQuest);

                var collectQuest = await _playerQuestService.GetMyQuestDetail(playerProfileId, request.QuestId.Value);
                return new InteractObjectResponseDto
                {
                    Success = true,
                    Message = questItem == null
                        ? $"Object {request.ObjectKey} interacted."
                        : $"Collected {DisplayItemName(questItem.Name)}.",
                    Quest = collectQuest,
                    CollectedItemId = questItem?.ItemId,
                    CollectedItemName = questItem?.Name,
                    CollectedQuantity = questItem == null ? 0 : progressDelta
                };
            }

            var nextProgress = playerQuest.Progress + progressDelta;
            var updates = await _playerQuestService.BatchUpdateProgress(
                playerProfileId,
                new BatchProgressRequestDto
                {
                    Updates = new List<QuestProgressItemDto>
                    {
                        new() { QuestId = request.QuestId.Value, Progress = nextProgress }
                    }
                });

            return new InteractObjectResponseDto
            {
                Success = true,
                Message = $"Object {request.ObjectKey} interacted.",
                Quest = updates.FirstOrDefault()
            };
        }

        // Executes core business logic for turn in quest item.
        // Logic details: validates numeric boundary constraints; delegates data queries and updates to repository layer; throws InvalidOperationException, KeyNotFoundException on invalid state or rule violations.
        // Returns the computed TurnInQuestItemResponseDto result asynchronously.
        public Task<TurnInQuestItemResponseDto> TurnInQuestItem(int playerProfileId, TurnInQuestItemRequestDto request)
            => _transactionManager.ExecuteInTransactionAsync(
                () => TurnInQuestItemCore(playerProfileId, request),
                IsolationLevel.Serializable);

        // Executes core business logic for turn in quest item core.
        // Logic details: delegates data queries and updates to repository layer; throws KeyNotFoundException on invalid state or rule violations.
        // Returns the computed TurnInQuestItemResponseDto result asynchronously.
        private async Task<TurnInQuestItemResponseDto> TurnInQuestItemCore(int playerProfileId, TurnInQuestItemRequestDto request)
        {
            var profile = await GetProfile(playerProfileId);
            var mapName = NormalizeMapName(profile.LastMapName);

            var npc = await _worldRepository.GetNpcById(request.NPCId)
                ?? throw new KeyNotFoundException($"NPC {request.NPCId} not found.");

            if (npc.MapName != mapName)
                throw new InvalidOperationException($"NPC {request.NPCId} is on map {npc.MapName}, but player is currently in {mapName}.");  // Unexpected runtime state — propagate to global error handler

            var linkedToNpc = await _worldRepository.IsQuestLinkedToNpc(request.NPCId, request.QuestId);
            if (!linkedToNpc)
                throw new InvalidOperationException($"Quest {request.QuestId} is not linked to NPC {request.NPCId}.");  // Unexpected runtime state — propagate to global error handler

            var playerQuest = await _playerQuestRepository.GetByPlayerAndQuest(playerProfileId, request.QuestId)
                ?? throw new KeyNotFoundException($"PlayerQuest not found for questId={request.QuestId}.");

            if (playerQuest.Quest?.MapName != mapName)
                throw new InvalidOperationException($"Quest {request.QuestId} does not belong to current map {mapName}.");  // Unexpected runtime state — propagate to global error handler

            if (!IsCollectQuest(playerQuest.Quest))
                throw new InvalidOperationException($"Quest {request.QuestId} is not a QuestItem turn-in quest.");  // Unexpected runtime state — propagate to global error handler

            if (playerQuest.Status == "Claimed")
            {
                var claimedQuest = await _playerQuestService.GetMyQuestDetail(playerProfileId, request.QuestId);
                return new TurnInQuestItemResponseDto
                {
                    Success = true,
                    Message = "Reward already claimed.",
                    Quest = claimedQuest
                };
            }

            if (playerQuest.Status == "Completed")
            {
                var completedQuest = await _playerQuestService.GetMyQuestDetail(playerProfileId, request.QuestId);
                return new TurnInQuestItemResponseDto
                {
                    Success = true,
                    Message = "Quest item already handed over.",
                    Quest = completedQuest,
                    ConsumedQuantity = 0
                };
            }

            if (playerQuest.Status != "InProgress")
                throw new InvalidOperationException($"Quest {request.QuestId} is not in progress (status={playerQuest.Status}).");  // Unexpected runtime state — propagate to global error handler

            var targetAmount = Math.Max(1, playerQuest.Quest?.TargetAmount ?? playerQuest.TargetValue);
            if (playerQuest.Progress < targetAmount)
            {
                return new TurnInQuestItemResponseDto
                {
                    Success = false,
                    Message = $"Need {targetAmount - playerQuest.Progress} more quest items.",
                    Quest = await _playerQuestService.GetMyQuestDetail(playerProfileId, request.QuestId),
                    ConsumedQuantity = 0
                };
            }

            var item = await ResolveQuestItem(
                new InteractObjectRequestDto
                {
                    MapName = mapName,
                    ObjectKey = playerQuest.Quest?.ObjectiveTarget ?? string.Empty,
                    InteractionType = "Collect",
                    QuestId = request.QuestId,
                    ProgressDelta = targetAmount
                },
                playerQuest.Quest)
                ?? throw new KeyNotFoundException($"Quest item for questId={request.QuestId} was not found.");

            var inventoryItem = await _inventoryRepository.GetByPlayerAndItem(playerProfileId, item.ItemId);
            var available = inventoryItem?.Quantity ?? 0;
            if (available < targetAmount)
            {
                var missingQuest = await _playerQuestService.GetMyQuestDetail(playerProfileId, request.QuestId);
                return new TurnInQuestItemResponseDto
                {
                    Success = false,
                    Message = $"Need {targetAmount - available} more {DisplayItemName(item.Name)}.",
                    Quest = missingQuest,
                    ConsumedItemId = item.ItemId,
                    ConsumedItemName = item.Name,
                    ConsumedQuantity = 0
                };
            }

            if (inventoryItem != null)  // Entity exists — proceed with conditional branch
            {
                inventoryItem.Quantity -= targetAmount;
                if (inventoryItem.Quantity <= 0)
                    await _inventoryRepository.DeleteItem(inventoryItem.InventoryItemId);
                else
                    await _inventoryRepository.UpdateItem(inventoryItem);
            }

            playerQuest.TargetValue = targetAmount;
            playerQuest.Progress = targetAmount;
            playerQuest.Status = "Completed";
            playerQuest.CompletedAt ??= DateTime.UtcNow;
            await _playerQuestRepository.Update(playerQuest);

            var turnedInQuest = await _playerQuestService.GetMyQuestDetail(playerProfileId, request.QuestId);

            return new TurnInQuestItemResponseDto
            {
                Success = true,
                Message = $"Handed over {targetAmount} {DisplayItemName(item.Name)}.",
                Quest = turnedInQuest,
                ConsumedItemId = item.ItemId,
                ConsumedItemName = item.Name,
                ConsumedQuantity = targetAmount
            };
        }

        // Executes core business logic for open chest.
        // Logic details: delegates data queries and updates to repository layer; throws InvalidOperationException, KeyNotFoundException, ArgumentException on invalid state or rule violations.
        // Returns the computed OpenChestResponseDto result asynchronously.
        public async Task<OpenChestResponseDto> OpenChest(int playerProfileId, OpenWorldChestRequestDto request)
        {
            if (!request.ChestId.HasValue && !request.PlayerChestId.HasValue)
                throw new ArgumentException("ChestId or PlayerChestId is required.");

            return await _transactionManager.ExecuteInTransactionAsync(async () =>
            {
                PlayerChest? playerChest = null;
                Chest? chest = null;

                if (request.PlayerChestId.HasValue)
                {
                    playerChest = await _worldRepository.GetPlayerChest(request.PlayerChestId.Value, playerProfileId)
                        ?? throw new KeyNotFoundException($"PlayerChest {request.PlayerChestId.Value} not found.");

                    if (playerChest.IsOpened)
                        throw new InvalidOperationException("Chest has already been opened.");  // Unexpected runtime state — propagate to global error handler

                    chest = playerChest.Chest;
                }
                else
                {
                    chest = await _worldRepository.GetChestById(request.ChestId!.Value)
                        ?? throw new KeyNotFoundException($"Chest {request.ChestId!.Value} not found.");

                    playerChest = new PlayerChest
                    {
                        PlayerProfileId = playerProfileId,
                        ChestId = chest.ChestId,
                        IsOpened = false,
                        ReceivedAt = DateTime.UtcNow
                    };
                    await _worldRepository.CreatePlayerChest(playerChest);
                }

                if (chest == null)  // Entity not found — short-circuit with appropriate error result
                    throw new KeyNotFoundException("Chest definition not found.");

                var profile = await GetProfile(playerProfileId);
                var goldEarned = chest.GoldMaxReward > chest.GoldMinReward
                    ? Random.Shared.Next(chest.GoldMinReward, chest.GoldMaxReward + 1)
                    : chest.GoldMinReward;

                profile.Gold += goldEarned;
                profile.AddExperience(chest.ExperienceReward);

                var openedItems = new List<ChestOpenedItemDto>();
                foreach (var chestItem in chest.ChestItems)
                {
                    if (!chestItem.IsGuaranteed && Random.Shared.NextDouble() * 100 > (double)chestItem.DropRate)
                        continue;

                    var quantity = chestItem.QuantityMax > chestItem.QuantityMin
                        ? Random.Shared.Next(chestItem.QuantityMin, chestItem.QuantityMax + 1)
                        : chestItem.QuantityMin;

                    await AddItemToInventory(playerProfileId, chestItem.ItemId, quantity);
                    openedItems.Add(new ChestOpenedItemDto
                    {
                        ItemId = chestItem.ItemId,
                        ItemName = chestItem.Item?.Name ?? string.Empty,
                        ItemIconUrl = chestItem.Item?.IconUrl,
                        Rarity = chestItem.Item?.Rarity ?? string.Empty,
                        Quantity = quantity
                    });
                }

                playerChest.IsOpened = true;
                playerChest.OpenedAt = DateTime.UtcNow;
                await _worldRepository.UpdatePlayerChest(playerChest);
                await _playerProfileRepository.UpdatePlayerProfile(profile);

                return new OpenChestResponseDto
                {
                    Success = true,
                    GoldEarned = goldEarned,
                    ExperienceEarned = chest.ExperienceReward,
                    Items = openedItems
                };
            });
        }

        // Executes core business logic for get daily login status.
        // Logic details: delegates data queries and updates to repository layer.
        // Returns the computed PlayerDailyLoginResponseDto? result asynchronously.
        public async Task<PlayerDailyLoginResponseDto?> GetDailyLoginStatus(int playerProfileId)
        {
            await GetProfile(playerProfileId);
            return await GetDailyLogin(playerProfileId);
        }

        // Executes core business logic for claim daily login reward.
        // Logic details: delegates data queries and updates to repository layer.
        // Returns the computed ClaimDailyRewardResponseDto result asynchronously.
        public async Task<ClaimDailyRewardResponseDto> ClaimDailyLoginReward(int playerProfileId)
        {
            return await _transactionManager.ExecuteInTransactionAsync(async () =>
            {
                var profile = await GetProfile(playerProfileId);
                var today = DateTime.UtcNow;
                var dailyLogin = await _worldRepository.GetPlayerDailyLogin(playerProfileId);

                dailyLogin ??= new PlayerDailyLogin { PlayerProfileId = playerProfileId, CurrentYear = today.Year, CurrentMonth = today.Month };

                if (dailyLogin.CurrentYear != today.Year || dailyLogin.CurrentMonth != today.Month)
                {
                    dailyLogin.CurrentYear = today.Year;
                    dailyLogin.CurrentMonth = today.Month;
                    dailyLogin.ClaimedDaysStr = string.Empty;
                    dailyLogin.RetroClaimCount = 0;
                    dailyLogin.IsClaimedToday = false;
                }

                if (dailyLogin.ClaimedDays.Contains(today.Day))
                {
                    return new ClaimDailyRewardResponseDto
                    {
                        Success = false,
                        Message = "Daily login reward already claimed today.",
                        CurrentStreak = dailyLogin.CurrentStreak,
                        TotalDaysClaimed = dailyLogin.TotalDaysClaimed
                    };
                }

                var rewardDay = today.Day;
                var reward = await _worldRepository.GetDailyLoginReward(rewardDay, today.Month, today.Year);

                if (reward == null)  // Entity not found — short-circuit with appropriate error result
                {
                    return new ClaimDailyRewardResponseDto
                    {
                        Success = false,
                        Message = $"No daily login reward is configured for day {rewardDay}."
                    };
                }

                await ApplyDailyReward(profile, reward);

                var claimed = dailyLogin.ClaimedDays;
                claimed.Add(rewardDay);
                dailyLogin.ClaimedDays = claimed;

                dailyLogin.TotalDaysClaimed += 1;
                dailyLogin.LastClaimedAt = DateTime.UtcNow;
                dailyLogin.IsClaimedToday = true;

                if (dailyLogin.PlayerDailyLoginId == 0)
                    await _worldRepository.CreatePlayerDailyLogin(dailyLogin);
                else
                    await _worldRepository.UpdatePlayerDailyLogin(dailyLogin);

                await _playerProfileRepository.UpdatePlayerProfile(profile);

                return new ClaimDailyRewardResponseDto
                {
                    Success = true,
                    Message = "Daily login reward claimed.",
                    CurrentStreak = dailyLogin.CurrentStreak,
                    TotalDaysClaimed = dailyLogin.TotalDaysClaimed,
                    RewardType = reward.RewardType,
                    RewardValue = reward.RewardValue,
                    RewardItemId = reward.RewardItemId,
                    RewardItemName = reward.RewardItem?.Name,
                    RewardItemQuantity = reward.RewardItemQuantity
                };
            });
        }

        // Executes core business logic for retroactive claim daily login reward.
        // Logic details: delegates data queries and updates to repository layer.
        // Returns the computed ClaimDailyRewardResponseDto result asynchronously.
        public async Task<ClaimDailyRewardResponseDto> RetroactiveClaimDailyLoginReward(int playerProfileId, int dayToClaim)
        {
            return await _transactionManager.ExecuteInTransactionAsync(async () =>
            {
                var profile = await GetProfile(playerProfileId);
                var today = DateTime.UtcNow;

                if (dayToClaim >= today.Day)
                {
                    return new ClaimDailyRewardResponseDto
                    {
                        Success = false,
                        Message = "Cannot retro-claim current or future days."
                    };
                }

                var dailyLogin = await _worldRepository.GetPlayerDailyLogin(playerProfileId);

                dailyLogin ??= new PlayerDailyLogin { PlayerProfileId = playerProfileId, CurrentYear = today.Year, CurrentMonth = today.Month };

                if (dailyLogin.CurrentYear != today.Year || dailyLogin.CurrentMonth != today.Month)
                {
                    dailyLogin.CurrentYear = today.Year;
                    dailyLogin.CurrentMonth = today.Month;
                    dailyLogin.ClaimedDaysStr = string.Empty;
                    dailyLogin.RetroClaimCount = 0;
                    dailyLogin.IsClaimedToday = false;
                }

                if (dailyLogin.ClaimedDays.Contains(dayToClaim))
                {
                    return new ClaimDailyRewardResponseDto
                    {
                        Success = false,
                        Message = "This day is already claimed."
                    };
                }

                if (profile.Gems < 20)
                {
                    return new ClaimDailyRewardResponseDto
                    {
                        Success = false,
                        Message = "Not enough Gems to retro-claim."
                    };
                }

                if (dailyLogin.RetroClaimCount >= 5)
                {
                    return new ClaimDailyRewardResponseDto
                    {
                        Success = false,
                        Message = "You have reached the maximum of 5 retro-claims this month."
                    };
                }

                var claimedSet = dailyLogin.ClaimedDays.ToHashSet();
                int maxMissedDay = -1;
                for (int d = today.Day - 1; d >= 1; d--)
                {
                    if (!claimedSet.Contains(d))
                    {
                        maxMissedDay = d;
                        break;
                    }
                }

                if (dayToClaim != maxMissedDay)
                {
                    return new ClaimDailyRewardResponseDto
                    {
                        Success = false,
                        Message = "You must retro-claim the most recent missed day first."
                    };
                }

                var reward = await _worldRepository.GetDailyLoginReward(dayToClaim, today.Month, today.Year);
                if (reward == null)  // Entity not found — short-circuit with appropriate error result
                {
                    return new ClaimDailyRewardResponseDto
                    {
                        Success = false,
                        Message = $"No daily login reward is configured for day {dayToClaim}."
                    };
                }

                profile.Gems -= 20;
                dailyLogin.RetroClaimCount += 1;

                await ApplyDailyReward(profile, reward);

                var claimed = dailyLogin.ClaimedDays;
                claimed.Add(dayToClaim);
                dailyLogin.ClaimedDays = claimed;

                dailyLogin.TotalDaysClaimed += 1;

                if (dailyLogin.PlayerDailyLoginId == 0)
                {
                    await _worldRepository.CreatePlayerDailyLogin(dailyLogin);
                }
                else
                {
                    await _worldRepository.UpdatePlayerDailyLogin(dailyLogin);
                }

                await _playerProfileRepository.UpdatePlayerProfile(profile);

                return new ClaimDailyRewardResponseDto
                {
                    Success = true,
                    Message = "Daily login reward claimed.",
                    CurrentStreak = dailyLogin.CurrentStreak,
                    TotalDaysClaimed = dailyLogin.TotalDaysClaimed,
                    RewardType = reward.RewardType,
                    RewardValue = reward.RewardValue,
                    RewardItemId = reward.RewardItemId,
                    RewardItemName = reward.RewardItem?.Name,
                    RewardItemQuantity = reward.RewardItemQuantity
                };
            });
        }

        // Executes core business logic for apply daily reward.
        // Completes asynchronously upon successful execution.
        private async Task ApplyDailyReward(PlayerProfile profile, DailyLoginReward reward)
        {
            switch (reward.RewardType)
            {
                case "Gold":
                    profile.Gold += reward.RewardValue;
                    break;
                case "Gems":
                    profile.Gems += reward.RewardValue;
                    break;
                case "Energy":
                    _playerProfileService.RecalculateEnergy(profile);
                    profile.CurrentEnergy = Math.Min(profile.MaxEnergy, profile.CurrentEnergy + (int)reward.RewardValue);
                    if (profile.CurrentEnergy >= profile.MaxEnergy)
                    {
                        profile.LastEnergyUpdateTime = DateTime.UtcNow;
                    }
                    break;
                case "EXP":
                case "Experience":
                    profile.AddExperience((int)reward.RewardValue);
                    break;
                case "Item":
                    if (reward.RewardItemId.HasValue)
                        await AddItemToInventory(profile.PlayerProfileId, reward.RewardItemId.Value, Math.Max(1, reward.RewardItemQuantity));
                    break;
            }
        }

        // Executes core business logic for add item to inventory.
        // Logic details: validates required non-empty string arguments; throws InvalidOperationException on invalid state or rule violations.
        // Completes asynchronously upon successful execution.
        private async Task AddItemToInventory(int playerProfileId, int itemId, int quantity)
            => await _rewardDeliveryService.DeliverItemAsync(playerProfileId, itemId, quantity, "world reward");
        // Executes core business logic for validate quest interaction.
        // Logic details: throws InvalidOperationException on invalid state or rule violations.
        private static void ValidateQuestInteraction(InteractObjectRequestDto request, Quest? quest)
        {
            if (quest == null)  // Entity not found — short-circuit with appropriate error result
                throw new InvalidOperationException("Quest definition is unavailable.");  // Unexpected runtime state — propagate to global error handler

            // Supported quest objectives: Explore, Defeat, Collect, Talk, OpenChest, Interact, EquipSkill, or Kill; the value selects progress-tracking behavior.
            var objectiveType = quest.ObjectiveType?.Trim();
            if (!string.Equals(objectiveType, "Interact", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(objectiveType, "Collect", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Quest {quest.QuestId} cannot be progressed by a world interaction.");  // Unexpected runtime state — propagate to global error handler
            }

            var objectiveTarget = NormalizeToken(quest.ObjectiveTarget);
            var objectKey = NormalizeToken(request.ObjectKey);
            if (string.IsNullOrEmpty(objectiveTarget) || string.IsNullOrEmpty(objectKey) ||  // Mandatory string argument is null or empty — fail fast
                (!objectKey.Contains(objectiveTarget) && !objectiveTarget.Contains(objectKey)))
            {
                throw new InvalidOperationException($"Object {request.ObjectKey} is not an objective for quest {quest.QuestId}.");  // Unexpected runtime state — propagate to global error handler
            }
        }

        // Executes core business logic for consume required interaction items.
        // Completes asynchronously upon successful execution.
        private async Task ConsumeRequiredInteractionItems(int playerProfileId, string objectKey)
        {
            var normalizedKey = NormalizeToken(objectKey);
            var requirements = normalizedKey switch
            {
                var key when key.Contains("ivytree") => new[] { (itemId: 32, quantity: 1, name: "Spirit Skull") },
                var key when key.Contains("lockedbridgegate") => new[] { (itemId: 33, quantity: 1, name: "Mystic Key") },
                var key when key.Contains("origintree") => new[]
                {
                    (itemId: 29, quantity: 1, name: "Swamp Seal Book"),
                    (itemId: 26, quantity: 1, name: "Dragon Seal Book"),
                    (itemId: 27, quantity: 1, name: "Golem Seal Book"),
                    (itemId: 28, quantity: 1, name: "UnderKing Seal Book")
                },
                _ => Array.Empty<(int itemId, int quantity, string name)>()
            };

            if (requirements.Length == 0)
                return;

            var inventoryItems = new List<(InventoryItem item, int quantity, string name)>();
            foreach (var requirement in requirements)
            {
                var item = await _inventoryRepository.GetByPlayerAndItem(playerProfileId, requirement.itemId);
                if (item == null || item.Quantity < requirement.quantity)
                    throw new InvalidOperationException($"You need {requirement.quantity} {requirement.name}.");  // Unexpected runtime state — propagate to global error handler

                inventoryItems.Add((item, requirement.quantity, requirement.name));
            }

            foreach (var requirement in inventoryItems)
            {
                if (requirement.item.Quantity == requirement.quantity)
                    await _inventoryRepository.DeleteItem(requirement.item.InventoryItemId);
                else
                {
                    requirement.item.Quantity -= requirement.quantity;
                    await _inventoryRepository.UpdateItem(requirement.item);
                }
            }
        }


        // Executes core business logic for try collect quest item.
        // Logic details: delegates data queries and updates to repository layer.
        // Returns the computed Item? result asynchronously.
        private async Task<Item?> TryCollectQuestItem(int playerProfileId, InteractObjectRequestDto request, Quest? quest)
        {
            if (!IsQuestItemInteraction(request, quest))
                return null;

            if (quest != null)  // Entity exists — proceed with conditional branch
            {
                var item = await ResolveQuestItem(request, quest);
                if (item == null)  // Entity not found — short-circuit with appropriate error result
                    return null;

                var targetAmount = Math.Max(1, quest.TargetAmount);
                var inventoryItem = await _inventoryRepository.GetByPlayerAndItem(playerProfileId, item.ItemId);
                var available = inventoryItem?.Quantity ?? 0;
                if (available >= targetAmount)
                    return null;

                await AddItemToInventory(playerProfileId, item.ItemId, targetAmount - available);
                return item;
            }

            var unlinkedItem = await ResolveQuestItem(request, quest);
            if (unlinkedItem == null)  // Entity not found — short-circuit with appropriate error result
                return null;

            await AddItemToInventory(playerProfileId, unlinkedItem.ItemId, Math.Max(1, request.ProgressDelta));
            return unlinkedItem;
        }

        // Executes core business logic for resolve quest item.
        // Returns the computed Item? result asynchronously.
        private async Task<Item?> ResolveQuestItem(InteractObjectRequestDto request, Quest? quest)
        {
            var searchText = $"{request.ObjectKey} {request.InteractionType} {quest?.ObjectiveTarget} {quest?.ObjectiveLocation}";

            if (Contains(searchText, "White Flower") || Contains(searchText, "WhiteFlower") || Contains(searchText, "Flower"))
                return await FindQuestItemByNames("[ELF] White Flower", "White Flower");

            if (Contains(searchText, "Old Willow Branch") || Contains(searchText, "OldWillowBranch") || Contains(searchText, "Branch") || Contains(searchText, "Willow"))
                return await FindQuestItemByNames("[ELF] Old Willow Branch", "Old Willow Branch");

            if (Contains(searchText, "Pumpkin") || Contains(searchText, "Enchanted Pumpkin"))
                return await FindQuestItemByNames("Enchanted Pumpkin");

            if (Contains(searchText, "Flour") || Contains(searchText, "Magic Flour"))
                return await FindQuestItemByNames("Magic Flour");

            if (Contains(searchText, "Spirit Skull") || Contains(searchText, "Strange Object") || Contains(searchText, "cục kì lạ"))
                return await FindQuestItemByNames("Spirit Skull");

            if (Contains(searchText, "Leaves") || Contains(searchText, "Ancient Leaves"))
                return await FindQuestItemByNames("Ancient Leaves");

            var normalizedSearch = NormalizeToken(searchText);
            var questItems = await _itemRepository.GetQuestItems();

            return questItems.FirstOrDefault(i => normalizedSearch.Contains(NormalizeToken(i.Name)));
        }

        // Executes core business logic for find quest item by names.
        // Logic details: delegates data queries and updates to repository layer.
        // Returns the computed Item? result asynchronously.
        private async Task<Item?> FindQuestItemByNames(params string[] names)
        {
            return await _itemRepository.GetQuestItemByNames(names);
        }

        // Executes core business logic for is quest item interaction.
        // Returns a boolean indicating operation success.
        private static bool IsQuestItemInteraction(InteractObjectRequestDto request, Quest? quest)
        {
            return string.Equals(request.InteractionType, "Collect", StringComparison.OrdinalIgnoreCase) ||
                   IsCollectQuest(quest) ||
                   Contains(request.ObjectKey, "Flower") ||
                   Contains(request.ObjectKey, "Branch") ||
                   Contains(request.ObjectKey, "Willow") ||
                   Contains(request.ObjectKey, "Spirit Skull") ||
                   Contains(request.ObjectKey, "Strange Object") ||
                   Contains(request.ObjectKey, "cục kì lạ") ||
                   Contains(request.ObjectKey, "Pumpkin") ||
                   Contains(request.ObjectKey, "Flour") ||
                   Contains(request.ObjectKey, "Leaves");
        }

        // Executes core business logic for is collect quest.
        // Logic details: validates required non-empty string arguments.
        // Returns a boolean indicating operation success.
        private static bool IsCollectQuest(Quest? quest)
        {
            return string.Equals(quest?.ObjectiveType, "Collect", StringComparison.OrdinalIgnoreCase);
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
        // Executes core business logic for normalize token.
        // Logic details: validates required non-empty string arguments.
        private static string NormalizeToken(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))  // Mandatory string argument is blank — fail fast
                return string.Empty;

            var chars = value
                .Where(char.IsLetterOrDigit)  // Filter records matching the predicate
                .Select(char.ToLowerInvariant)
                .ToArray();
            return new string(chars);
        }

        // Executes core business logic for display item name.
        // Logic details: validates required non-empty string arguments; delegates data queries and updates to repository layer; throws KeyNotFoundException on invalid state or rule violations.
        private static string DisplayItemName(string? itemName)
        {
            if (string.IsNullOrWhiteSpace(itemName))  // Mandatory string argument is blank — fail fast
                return "quest item";

            var trimmed = itemName.Trim();
            var closingPrefix = trimmed.IndexOf(']');
            return closingPrefix >= 0 && closingPrefix + 1 < trimmed.Length
                ? trimmed[(closingPrefix + 1)..].Trim()
                : trimmed;
        }

        // Executes core business logic for get profile.
        // Logic details: delegates data queries and updates to repository layer; throws KeyNotFoundException on invalid state or rule violations.
        // Returns the computed PlayerProfile result asynchronously.
        private async Task<PlayerProfile> GetProfile(int playerProfileId)
        {
            var profile = await _playerProfileRepository.GetPlayerProfileById(playerProfileId)
                ?? throw new KeyNotFoundException($"PlayerProfile {playerProfileId} not found.");

            if (_playerProfileService.RecalculateEnergy(profile))
            {
                await _playerProfileRepository.UpdatePlayerProfile(profile);
            }

            return profile;
        }

        // Executes core business logic for ensure tutorial spawn.
        // Logic details: validates required non-empty string arguments; validates numeric boundary constraints; delegates data queries and updates to repository layer.
        // Completes asynchronously upon successful execution.
        private async Task EnsureTutorialSpawn(PlayerProfile profile)
        {
            var hasMap = !string.IsNullOrWhiteSpace(profile.LastMapName);
            var hasPosition = Math.Abs(profile.PositionX) > double.Epsilon ||
                              Math.Abs(profile.PositionY) > double.Epsilon;
            var isLegacyTutorialSpawn = string.Equals(NormalizeMapName(profile.LastMapName), TutorialMapName, StringComparison.OrdinalIgnoreCase) &&
                                        Math.Abs(profile.PositionX - 46.9) < 0.1 &&
                                        Math.Abs(profile.PositionY - 44.1) < 0.1;
            var shouldApplyTutorialSpawn = profile.Level <= 1 && (!hasPosition || isLegacyTutorialSpawn);

            if (hasMap && !shouldApplyTutorialSpawn)
                return;

            profile.LastMapName = TutorialMapName;
            profile.PositionX = TutorialSpawnX;
            profile.PositionY = TutorialSpawnY;
            await _playerProfileRepository.UpdatePlayerProfile(profile);
        }

        // Executes core business logic for get daily login.
        // Logic details: delegates data queries and updates to repository layer.
        // Returns the computed PlayerDailyLoginResponseDto? result asynchronously.
        private async Task<PlayerDailyLoginResponseDto?> GetDailyLogin(int playerProfileId)
        {
            var dailyLogin = await _worldRepository.GetPlayerDailyLogin(playerProfileId);

            if (dailyLogin == null)  // Entity not found — short-circuit with appropriate error result
                return null;

            var today = DateTime.UtcNow;
            if (dailyLogin.CurrentYear != today.Year || dailyLogin.CurrentMonth != today.Month)
            {
                dailyLogin.CurrentYear = today.Year;
                dailyLogin.CurrentMonth = today.Month;
                dailyLogin.ClaimedDaysStr = string.Empty;
                dailyLogin.RetroClaimCount = 0;
                dailyLogin.IsClaimedToday = false;
            }

            return new PlayerDailyLoginResponseDto
            {
                PlayerDailyLoginId = dailyLogin.PlayerDailyLoginId,
                PlayerProfileId = dailyLogin.PlayerProfileId,
                CurrentStreak = dailyLogin.CurrentStreak,
                TotalDaysClaimed = dailyLogin.TotalDaysClaimed,
                LastClaimedAt = dailyLogin.LastClaimedAt,
                IsClaimedToday = dailyLogin.ClaimedDays.Contains(today.Day),
                CurrentYear = dailyLogin.CurrentYear,
                CurrentMonth = dailyLogin.CurrentMonth,
                RetroClaimCount = dailyLogin.RetroClaimCount,
                ClaimedDays = dailyLogin.ClaimedDays
            };
        }

        // Executes core business logic for build map progress.
        // Logic details: validates required non-empty string arguments; delegates data queries and updates to repository layer.
        // Returns the computed List<WorldMapProgressDto result asynchronously.
        private async Task<List<WorldMapProgressDto>> BuildMapProgress(int playerProfileId, string currentMapName)
        {
            var activeQuests = await _questRepository.GetActiveQuests();

            var npcMapNames = await _worldRepository.GetAllNpcMapNames();

            var playerQuestStates = await _playerQuestRepository.GetByPlayerId(playerProfileId);

            var mapNames = activeQuests
                .Select(q => NormalizeMapName(q.MapName))
                .Concat(npcMapNames.Select(NormalizeMapName))
                .Append(NormalizeMapName(currentMapName))
                .Where(name => !string.IsNullOrWhiteSpace(name))  // Filter records matching the predicate
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(name => name)  // Sort results oldest/lowest first
                .ToList();

            return mapNames.Select(mapName =>
            {
                var questIds = activeQuests
                    .Where(q => string.Equals(NormalizeMapName(q.MapName), mapName, StringComparison.OrdinalIgnoreCase))  // Filter records matching the predicate
                    .Select(q => q.QuestId)
                    .ToHashSet();

                var completed = playerQuestStates.Count(pq =>
                    questIds.Contains(pq.QuestId) &&
                    (pq.Status == "Completed" || pq.Status == "Claimed"));

                var total = questIds.Count;

                var hasClaimedQuest = playerQuestStates.Any(pq =>
                    string.Equals(NormalizeMapName(pq.Quest?.MapName), mapName, StringComparison.OrdinalIgnoreCase)
                    && pq.Status == "Claimed");

                return new WorldMapProgressDto
                {
                    MapName = mapName,
                    DisplayName = ToDisplayMapName(mapName),
                    IsUnlocked = string.Equals(mapName, TutorialMapName, StringComparison.OrdinalIgnoreCase)
                        || string.Equals(mapName, currentMapName, StringComparison.OrdinalIgnoreCase)
                        || hasClaimedQuest,
                    ExplorationPercent = total == 0 ? 0 : (int)Math.Round(completed * 100.0 / total)
                };
            }).ToList();
        }



        // Normalizes world map names and maps aliases (such as ElfForest) to canonical map identifiers.
        private static string NormalizeMapName(string? mapName)
        {
            if (string.IsNullOrWhiteSpace(mapName))  // Mandatory string argument is blank — fail fast
                return TutorialMapName;

            var normalized = mapName.Trim();
            return IsTutorialMapAlias(normalized) ? TutorialMapName : normalized;
        }

        // Executes core business logic for is tutorial map alias.
        // Logic details: validates required non-empty string arguments.
        // Returns a boolean indicating operation success.
        private static bool IsTutorialMapAlias(string mapName)
        {
            return string.Equals(mapName, "ElfForest", StringComparison.OrdinalIgnoreCase)
                || string.Equals(mapName, "ElfLand", StringComparison.OrdinalIgnoreCase)
                || string.Equals(mapName, "Map1", StringComparison.OrdinalIgnoreCase)
                || string.Equals(mapName, "Chapter1", StringComparison.OrdinalIgnoreCase)
                || string.Equals(mapName, "Chapter 1", StringComparison.OrdinalIgnoreCase)
                || string.Equals(mapName, TutorialMapName, StringComparison.OrdinalIgnoreCase);
        }

        // Executes core business logic for to display map name.
        // Logic details: validates required non-empty string arguments.
        private static string ToDisplayMapName(string mapName)
        {
            if (string.IsNullOrWhiteSpace(mapName))  // Mandatory string argument is blank — fail fast
                return "Elf Forest";

            var chars = new List<char> { mapName[0] };
            for (var i = 1; i < mapName.Length; i++)
            {
                var current = mapName[i];
                var previous = mapName[i - 1];
                if (char.IsUpper(current) && !char.IsWhiteSpace(previous) && !char.IsUpper(previous))
                    chars.Add(' ');
                chars.Add(current == '_' ? ' ' : current);
            }
            return new string(chars.ToArray()).Trim();
        }

    }
}
