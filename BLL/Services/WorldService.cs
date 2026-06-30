using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;

namespace BLL.Services
{
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
            IMapper mapper)
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
        }

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
                Npcs = _mapper.Map<List<NPCResponseDto>>(npcs),
                Quests = quests,
                ActiveQuest = quests.FirstOrDefault(q => q.Status == "InProgress")
                    ?? quests.FirstOrDefault(q => q.Status == "Completed")
                    ?? quests.FirstOrDefault(q => q.Status == "NotStarted"),
                DailyLogin = dailyLogin
            };
        }

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

        public async Task<TalkToNpcResponseDto> TalkToNpc(int playerProfileId, TalkToNpcRequestDto request)
        {
            var profile = await GetProfile(playerProfileId);
            var mapName = NormalizeMapName(profile.LastMapName);

            var npc = await _worldRepository.GetNpcById(request.NPCId)
                ?? throw new KeyNotFoundException($"NPC {request.NPCId} not found.");

            if (npc.MapName != mapName)
                throw new InvalidOperationException($"NPC {request.NPCId} is on map {npc.MapName}, but player is currently in {mapName}.");

            var linkedQuestIds = npc.Dialogues
                .Where(d => d.LinkedQuestId.HasValue)
                .Select(d => d.LinkedQuestId!.Value)
                .ToHashSet();
            var linkedQuests = (await _playerQuestService.GetMyQuests(playerProfileId))
                .Where(q => linkedQuestIds.Contains(q.QuestId))
                .ToList();

            return new TalkToNpcResponseDto
            {
                Npc = _mapper.Map<NPCResponseDto>(npc),
                LinkedQuests = linkedQuests
            };
        }

        public async Task<InteractObjectResponseDto> InteractWithObject(int playerProfileId, InteractObjectRequestDto request)
        {
            var profile = await GetProfile(playerProfileId);
            var mapName = NormalizeMapName(profile.LastMapName);
            var progressDelta = Math.Max(1, request.ProgressDelta);

            if (NormalizeMapName(request.MapName) != mapName)
            {
                throw new InvalidOperationException($"Player is currently in {mapName}, not {request.MapName}.");
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
                throw new InvalidOperationException($"Quest {request.QuestId.Value} does not belong to current map {mapName}.");

            if (playerQuest.Status != "InProgress")
            {
                return new InteractObjectResponseDto
                {
                    Success = true,
                    Message = $"Object {request.ObjectKey} interacted, but quest is {playerQuest.Status}."
                };
            }

            var questItem = await TryCollectQuestItem(playerProfileId, request, playerQuest.Quest);
            var targetAmount = Math.Max(1, playerQuest.Quest?.TargetAmount ?? playerQuest.TargetValue);

            if (IsCollectQuest(playerQuest.Quest))
            {
                playerQuest.TargetValue = targetAmount;
                playerQuest.Progress = Math.Min(targetAmount, playerQuest.Progress + progressDelta);
                
                if (playerQuest.Progress >= targetAmount)
                {
                    playerQuest.Status = "Completed";
                    playerQuest.CompletedAt ??= DateTime.UtcNow;
                }

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

        public async Task<TurnInQuestItemResponseDto> TurnInQuestItem(int playerProfileId, TurnInQuestItemRequestDto request)
        {
            var profile = await GetProfile(playerProfileId);
            var mapName = NormalizeMapName(profile.LastMapName);

            var npc = await _worldRepository.GetNpcById(request.NPCId)
                ?? throw new KeyNotFoundException($"NPC {request.NPCId} not found.");

            if (npc.MapName != mapName)
                throw new InvalidOperationException($"NPC {request.NPCId} is on map {npc.MapName}, but player is currently in {mapName}.");

            var linkedToNpc = await _worldRepository.IsQuestLinkedToNpc(request.NPCId, request.QuestId);
            if (!linkedToNpc)
                throw new InvalidOperationException($"Quest {request.QuestId} is not linked to NPC {request.NPCId}.");

            var playerQuest = await _playerQuestRepository.GetByPlayerAndQuest(playerProfileId, request.QuestId)
                ?? throw new KeyNotFoundException($"PlayerQuest not found for questId={request.QuestId}.");

            if (playerQuest.Quest?.MapName != mapName)
                throw new InvalidOperationException($"Quest {request.QuestId} does not belong to current map {mapName}.");

            if (!IsCollectQuest(playerQuest.Quest))
                throw new InvalidOperationException($"Quest {request.QuestId} is not a QuestItem turn-in quest.");

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
                throw new InvalidOperationException($"Quest {request.QuestId} is not in progress (status={playerQuest.Status}).");

            var targetAmount = Math.Max(1, playerQuest.Quest?.TargetAmount ?? playerQuest.TargetValue);
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

            return await _transactionManager.ExecuteInTransactionAsync(async () =>
            {
                if (inventoryItem != null)
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

                var completedQuest = await _playerQuestService.GetMyQuestDetail(playerProfileId, request.QuestId);

                return new TurnInQuestItemResponseDto
                {
                    Success = true,
                    Message = $"Handed over {targetAmount} {DisplayItemName(item.Name)}.",
                    Quest = completedQuest,
                    ConsumedItemId = item.ItemId,
                    ConsumedItemName = item.Name,
                    ConsumedQuantity = targetAmount
                };
            });
        }

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
                        throw new InvalidOperationException("Chest has already been opened.");

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

                if (chest == null)
                    throw new KeyNotFoundException("Chest definition not found.");

                var profile = await GetProfile(playerProfileId);
                var goldEarned = chest.GoldMaxReward > chest.GoldMinReward
                    ? Random.Shared.Next(chest.GoldMinReward, chest.GoldMaxReward + 1)
                    : chest.GoldMinReward;

                profile.Gold += goldEarned;
                profile.ExperiencePoints += chest.ExperienceReward;

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

        public async Task<PlayerDailyLoginResponseDto?> GetDailyLoginStatus(int playerProfileId)
        {
            await GetProfile(playerProfileId);
            return await GetDailyLogin(playerProfileId);
        }

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
                var reward = await _worldRepository.GetDailyLoginReward(rewardDay);

                if (reward != null)
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
                    RewardType = reward?.RewardType ?? string.Empty,
                    RewardValue = reward?.RewardValue ?? 0,
                    RewardItemId = reward?.RewardItemId,
                    RewardItemName = reward?.RewardItem?.Name,
                    RewardItemQuantity = reward?.RewardItemQuantity ?? 0
                };
            });
        }

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
                }

                if (dailyLogin.ClaimedDays.Contains(dayToClaim))
                {
                    return new ClaimDailyRewardResponseDto
                    {
                        Success = false,
                        Message = "This day is already claimed."
                    };
                }

                // Chi phí điểm danh bù là 20 Gems
                if (profile.Gems < 20)
                {
                    return new ClaimDailyRewardResponseDto
                    {
                        Success = false,
                        Message = "Not enough Gems to retro-claim."
                    };
                }
                
                // Giới hạn 5 lần/tháng
                if (dailyLogin.RetroClaimCount >= 5)
                {
                    return new ClaimDailyRewardResponseDto
                    {
                        Success = false,
                        Message = "You have reached the maximum of 5 retro-claims this month."
                    };
                }

                // Kiểm tra phải bù ngày gần nhất bị lỡ
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

                profile.Gems -= 20;
                dailyLogin.RetroClaimCount += 1;

                var reward = await _worldRepository.GetDailyLoginReward(dayToClaim);

                if (reward != null)
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
                    RewardType = reward?.RewardType ?? string.Empty,
                    RewardValue = reward?.RewardValue ?? 0,
                    RewardItemId = reward?.RewardItemId,
                    RewardItemName = reward?.RewardItem?.Name,
                    RewardItemQuantity = reward?.RewardItemQuantity ?? 0
                };
            });
        }

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
                case "Item":
                    if (reward.RewardItemId.HasValue)
                        await AddItemToInventory(profile.PlayerProfileId, reward.RewardItemId.Value, Math.Max(1, reward.RewardItemQuantity));
                    break;
            }
        }

        private async Task AddItemToInventory(int playerProfileId, int itemId, int quantity)
        {
            var existing = await _inventoryRepository.GetByPlayerAndItem(playerProfileId, itemId);

            if (existing != null)
            {
                existing.Quantity += quantity;
                await _inventoryRepository.UpdateItem(existing);
            }
            else
            {
                await _inventoryRepository.AddItem(new InventoryItem
                {
                    PlayerProfileId = playerProfileId,
                    ItemId = itemId,
                    Quantity = quantity,
                    IsEquipped = false,
                    IsSkin = false,
                    EnhancementLevel = 0
                });
            }
        }


        private async Task<Item?> TryCollectQuestItem(int playerProfileId, InteractObjectRequestDto request, Quest? quest)
        {
            if (!IsQuestItemInteraction(request, quest))
                return null;

            var item = await ResolveQuestItem(request, quest);
            if (item == null)
                return null;

            await AddItemToInventory(playerProfileId, item.ItemId, Math.Max(1, request.ProgressDelta));
            return item;
        }

        private async Task<Item?> ResolveQuestItem(InteractObjectRequestDto request, Quest? quest)
        {
            var searchText = $"{request.ObjectKey} {request.InteractionType} {quest?.ObjectiveTarget} {quest?.ObjectiveLocation}";

            if (Contains(searchText, "White Flower") || Contains(searchText, "WhiteFlower") || Contains(searchText, "Flower"))
                return await FindQuestItemByNames("[ELF] White Flower", "White Flower");

            if (Contains(searchText, "Old Willow Branch") || Contains(searchText, "OldWillowBranch") || Contains(searchText, "Branch") || Contains(searchText, "Willow"))
                return await FindQuestItemByNames("[ELF] Old Willow Branch", "Old Willow Branch");

            var normalizedSearch = NormalizeToken(searchText);
            var questItems = await _itemRepository.GetQuestItems();

            return questItems.FirstOrDefault(i => normalizedSearch.Contains(NormalizeToken(i.Name)));
        }

        private async Task<Item?> FindQuestItemByNames(params string[] names)
        {
            return await _itemRepository.GetQuestItemByNames(names);
        }

        private static bool IsQuestItemInteraction(InteractObjectRequestDto request, Quest? quest)
        {
            return string.Equals(request.InteractionType, "Collect", StringComparison.OrdinalIgnoreCase) ||
                   IsCollectQuest(quest) ||
                   Contains(request.ObjectKey, "Flower") ||
                   Contains(request.ObjectKey, "Branch") ||
                   Contains(request.ObjectKey, "Willow");
        }

        private static bool IsCollectQuest(Quest? quest)
        {
            return string.Equals(quest?.ObjectiveType, "Collect", StringComparison.OrdinalIgnoreCase);
        }


        private static bool Contains(string? source, string value)
        {
            return !string.IsNullOrWhiteSpace(source) &&
                   !string.IsNullOrWhiteSpace(value) &&
                   source.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
        }
        private static string NormalizeToken(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var chars = value
                .Where(char.IsLetterOrDigit)
                .Select(char.ToLowerInvariant)
                .ToArray();
            return new string(chars);
        }

        private static string DisplayItemName(string? itemName)
        {
            if (string.IsNullOrWhiteSpace(itemName))
                return "quest item";

            var trimmed = itemName.Trim();
            var closingPrefix = trimmed.IndexOf(']');
            return closingPrefix >= 0 && closingPrefix + 1 < trimmed.Length
                ? trimmed[(closingPrefix + 1)..].Trim()
                : trimmed;
        }

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

        private async Task<PlayerDailyLoginResponseDto?> GetDailyLogin(int playerProfileId)
        {
            var dailyLogin = await _worldRepository.GetPlayerDailyLogin(playerProfileId);

            if (dailyLogin == null)
                return null;

            // Reset month tracking if needed
            var today = DateTime.UtcNow;
            if (dailyLogin.CurrentYear != today.Year || dailyLogin.CurrentMonth != today.Month)
            {
                dailyLogin.CurrentYear = today.Year;
                dailyLogin.CurrentMonth = today.Month;
                dailyLogin.ClaimedDaysStr = string.Empty;
                // We just return the cleared state (will be saved when they claim something)
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

        private async Task<List<WorldMapProgressDto>> BuildMapProgress(int playerProfileId, string currentMapName)
        {
            var activeQuests = await _questRepository.GetActiveQuests();
            var activeQuestProjections = activeQuests.Select(q => new { q.QuestId, q.MapName }).ToList();

            var npcMapNames = await _worldRepository.GetAllNpcMapNames();

            var playerQuestStates = await _playerQuestRepository.GetByPlayerId(playerProfileId);
            var activePlayerQuestStates = playerQuestStates.Where(pq => pq.Quest != null && pq.Quest.IsActive).ToList();

            var mapNames = activeQuestProjections
                .Select(q => NormalizeMapName(q.MapName))
                .Concat(npcMapNames.Select(NormalizeMapName))
                .Append(NormalizeMapName(currentMapName))
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(name => name)
                .ToList();

            return mapNames.Select(mapName =>
            {
                var questIds = activeQuests
                    .Where(q => string.Equals(NormalizeMapName(q.MapName), mapName, StringComparison.OrdinalIgnoreCase))
                    .Select(q => q.QuestId)
                    .ToHashSet();

                var completed = playerQuestStates.Count(pq =>
                    questIds.Contains(pq.QuestId) &&
                    (pq.Status == "Completed" || pq.Status == "Claimed"));

                var total = questIds.Count;
                var hasAnyPlayerState = playerQuestStates.Any(pq =>
                    string.Equals(NormalizeMapName(pq.Quest?.MapName), mapName, StringComparison.OrdinalIgnoreCase));

                return new WorldMapProgressDto
                {
                    MapName = mapName,
                    DisplayName = ToDisplayMapName(mapName),
                    IsUnlocked = string.Equals(mapName, TutorialMapName, StringComparison.OrdinalIgnoreCase)
                        || string.Equals(mapName, currentMapName, StringComparison.OrdinalIgnoreCase)
                        || hasAnyPlayerState,
                    ExplorationPercent = total == 0 ? 0 : (int)Math.Round(completed * 100.0 / total)
                };
            }).ToList();
        }



        private static string NormalizeMapName(string? mapName)
        {
            if (string.IsNullOrWhiteSpace(mapName))
                return TutorialMapName;

            var normalized = mapName.Trim();
            return IsTutorialMapAlias(normalized) ? TutorialMapName : normalized;
        }

        private static bool IsTutorialMapAlias(string mapName)
        {
            return string.Equals(mapName, "ElfForest", StringComparison.OrdinalIgnoreCase)
                || string.Equals(mapName, "ElfLand", StringComparison.OrdinalIgnoreCase)
                || string.Equals(mapName, "Map1", StringComparison.OrdinalIgnoreCase)
                || string.Equals(mapName, "Chapter1", StringComparison.OrdinalIgnoreCase)
                || string.Equals(mapName, "Chapter 1", StringComparison.OrdinalIgnoreCase)
                || string.Equals(mapName, TutorialMapName, StringComparison.OrdinalIgnoreCase);
        }

        private static string ToDisplayMapName(string mapName)
        {
            if (string.IsNullOrWhiteSpace(mapName))
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
