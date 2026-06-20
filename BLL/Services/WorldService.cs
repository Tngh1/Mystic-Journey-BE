using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class WorldService : IWorldService
    {
        private const int MaxNpcsPerMap = 4;
        private const string TutorialMapName = "ElfForest";
        private const double TutorialSpawnX = 11.9;
        private const double TutorialSpawnY = 17.8;

        private readonly MysticJourneyDbContext _context;
        private readonly IPlayerProfileRepository _playerProfileRepository;
        private readonly IPlayerQuestService _playerQuestService;

        public WorldService(
            MysticJourneyDbContext context,
            IPlayerProfileRepository playerProfileRepository,
            IPlayerQuestService playerQuestService)
        {
            _context = context;
            _playerProfileRepository = playerProfileRepository;
            _playerQuestService = playerQuestService;
        }

        public async Task<WorldStateResponseDto> GetWorldState(int playerProfileId)
        {
            var profile = await GetProfile(playerProfileId);
            await EnsureTutorialSpawn(profile);
            var mapName = NormalizeMapName(profile.LastMapName);

            var npcs = await _context.NPCs
                .Include(n => n.Dialogues.Where(d => d.IsActive))
                    .ThenInclude(d => d.LinkedQuest)
                .Include(n => n.Dialogues.Where(d => d.IsActive))
                    .ThenInclude(d => d.LinkedShopItem)
                        .ThenInclude(si => si!.Item)
                .Where(n => n.IsActive && n.MapName == mapName)
                .OrderBy(n => n.NPCId)
                .Take(MaxNpcsPerMap)
                .ToListAsync();

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
                Npcs = npcs.Select(MapNpc).ToList(),
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

            var npc = await _context.NPCs
                .Include(n => n.Dialogues.Where(d => d.IsActive))
                    .ThenInclude(d => d.LinkedQuest)
                .Include(n => n.Dialogues.Where(d => d.IsActive))
                    .ThenInclude(d => d.LinkedShopItem)
                        .ThenInclude(si => si!.Item)
                .FirstOrDefaultAsync(n => n.NPCId == request.NPCId && n.IsActive)
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
                Npc = MapNpc(npc),
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

            var playerQuest = await _context.PlayerQuests
                .Include(pq => pq.Quest)
                .FirstOrDefaultAsync(pq => pq.PlayerProfileId == playerProfileId && pq.QuestId == request.QuestId.Value)
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
                _context.PlayerQuests.Update(playerQuest);
                await _context.SaveChangesAsync();

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

            var npc = await _context.NPCs
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.NPCId == request.NPCId && n.IsActive)
                ?? throw new KeyNotFoundException($"NPC {request.NPCId} not found.");

            if (npc.MapName != mapName)
                throw new InvalidOperationException($"NPC {request.NPCId} is on map {npc.MapName}, but player is currently in {mapName}.");

            var linkedToNpc = await _context.NPCDialogues
                .AnyAsync(d => d.NPCId == request.NPCId && d.LinkedQuestId == request.QuestId && d.IsActive);
            if (!linkedToNpc)
                throw new InvalidOperationException($"Quest {request.QuestId} is not linked to NPC {request.NPCId}.");

            var playerQuest = await _context.PlayerQuests
                .Include(pq => pq.Quest)
                .FirstOrDefaultAsync(pq => pq.PlayerProfileId == playerProfileId && pq.QuestId == request.QuestId)
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

            var inventoryItem = await _context.InventoryItems
                .FirstOrDefaultAsync(i => i.PlayerProfileId == playerProfileId && i.ItemId == item.ItemId);
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

            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                if (inventoryItem != null)
                {
                    inventoryItem.Quantity -= targetAmount;
                    if (inventoryItem.Quantity <= 0)
                        _context.InventoryItems.Remove(inventoryItem);
                    else
                        _context.InventoryItems.Update(inventoryItem);
                }

                await _context.SaveChangesAsync();

                PlayerQuestResponseDto? completedQuest;
                if (playerQuest.Status == "Completed")
                {
                    completedQuest = await _playerQuestService.GetMyQuestDetail(playerProfileId, request.QuestId);
                }
                else
                {
                    completedQuest = await _playerQuestService.CompleteQuest(
                        playerProfileId,
                        new CompleteQuestRequestDto { QuestId = request.QuestId });
                }

                await tx.CommitAsync();
                return new TurnInQuestItemResponseDto
                {
                    Success = true,
                    Message = $"Handed over {targetAmount} {DisplayItemName(item.Name)}.",
                    Quest = completedQuest,
                    ConsumedItemId = item.ItemId,
                    ConsumedItemName = item.Name,
                    ConsumedQuantity = targetAmount
                };
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<OpenChestResponseDto> OpenChest(int playerProfileId, OpenWorldChestRequestDto request)
        {
            if (!request.ChestId.HasValue && !request.PlayerChestId.HasValue)
                throw new ArgumentException("ChestId or PlayerChestId is required.");

            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                PlayerChest? playerChest = null;
                Chest? chest = null;

                if (request.PlayerChestId.HasValue)
                {
                    playerChest = await _context.PlayerChests
                        .Include(pc => pc.Chest)
                            .ThenInclude(c => c!.ChestItems)
                                .ThenInclude(ci => ci.Item)
                        .FirstOrDefaultAsync(pc =>
                            pc.PlayerChestId == request.PlayerChestId.Value &&
                            pc.PlayerProfileId == playerProfileId)
                        ?? throw new KeyNotFoundException($"PlayerChest {request.PlayerChestId.Value} not found.");

                    if (playerChest.IsOpened)
                        throw new InvalidOperationException("Chest has already been opened.");

                    chest = playerChest.Chest;
                }
                else
                {
                    chest = await _context.Chests
                        .Include(c => c.ChestItems)
                            .ThenInclude(ci => ci.Item)
                        .FirstOrDefaultAsync(c => c.ChestId == request.ChestId!.Value && c.IsActive)
                        ?? throw new KeyNotFoundException($"Chest {request.ChestId!.Value} not found.");

                    playerChest = new PlayerChest
                    {
                        PlayerProfileId = playerProfileId,
                        ChestId = chest.ChestId,
                        IsOpened = false,
                        ReceivedAt = DateTime.UtcNow
                    };
                    await _context.PlayerChests.AddAsync(playerChest);
                    await _context.SaveChangesAsync();
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
                _context.PlayerChests.Update(playerChest);
                _context.PlayerProfiles.Update(profile);
                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return new OpenChestResponseDto
                {
                    Success = true,
                    GoldEarned = goldEarned,
                    ExperienceEarned = chest.ExperienceReward,
                    Items = openedItems
                };
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<PlayerDailyLoginResponseDto?> GetDailyLoginStatus(int playerProfileId)
        {
            await GetProfile(playerProfileId);
            return await GetDailyLogin(playerProfileId);
        }

        public async Task<ClaimDailyRewardResponseDto> ClaimDailyLoginReward(int playerProfileId)
        {
            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var profile = await GetProfile(playerProfileId);
                var today = DateTime.UtcNow.Date;
                var dailyLogin = await _context.PlayerDailyLogins
                    .FirstOrDefaultAsync(x => x.PlayerProfileId == playerProfileId);

                if (dailyLogin != null && dailyLogin.LastClaimedAt?.Date == today)
                {
                    return new ClaimDailyRewardResponseDto
                    {
                        Success = false,
                        Message = "Daily login reward already claimed today.",
                        CurrentStreak = dailyLogin.CurrentStreak,
                        TotalDaysClaimed = dailyLogin.TotalDaysClaimed
                    };
                }

                dailyLogin ??= new PlayerDailyLogin { PlayerProfileId = playerProfileId };

                var yesterday = today.AddDays(-1);
                var nextStreak = dailyLogin.LastClaimedAt?.Date == yesterday
                    ? dailyLogin.CurrentStreak + 1
                    : 1;

                var rewards = await _context.DailyLoginRewards
                    .Include(r => r.RewardItem)
                    .Where(r => r.IsActive)
                    .OrderBy(r => r.DayNumber)
                    .ToListAsync();

                var maxDay = Math.Max(1, rewards.Count == 0 ? 1 : rewards.Max(r => r.DayNumber));
                var rewardDay = ((nextStreak - 1) % maxDay) + 1;
                var reward = rewards.FirstOrDefault(r => r.DayNumber == rewardDay)
                    ?? rewards.FirstOrDefault();

                if (reward != null)
                    await ApplyDailyReward(profile, reward);

                dailyLogin.CurrentStreak = nextStreak;
                dailyLogin.TotalDaysClaimed += 1;
                dailyLogin.LastClaimedAt = DateTime.UtcNow;
                dailyLogin.IsClaimedToday = true;

                if (dailyLogin.PlayerDailyLoginId == 0)
                    await _context.PlayerDailyLogins.AddAsync(dailyLogin);
                else
                    _context.PlayerDailyLogins.Update(dailyLogin);

                _context.PlayerProfiles.Update(profile);
                await _context.SaveChangesAsync();
                await tx.CommitAsync();

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
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
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
                    profile.Energy += (int)reward.RewardValue;
                    break;
                case "Item":
                    if (reward.RewardItemId.HasValue)
                        await AddItemToInventory(profile.PlayerProfileId, reward.RewardItemId.Value, Math.Max(1, reward.RewardItemQuantity));
                    break;
            }
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
            var questItems = await _context.Items
                .Where(i => i.IsActive && i.Type == "QuestItem")
                .OrderBy(i => i.ItemId)
                .ToListAsync();

            return questItems.FirstOrDefault(i => normalizedSearch.Contains(NormalizeToken(i.Name)));
        }

        private async Task<Item?> FindQuestItemByNames(params string[] names)
        {
            return await _context.Items
                .Where(i => i.IsActive && i.Type == "QuestItem" && names.Contains(i.Name))
                .OrderBy(i => i.ItemId)
                .FirstOrDefaultAsync();
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
            return await _playerProfileRepository.GetPlayerProfileById(playerProfileId)
                ?? throw new KeyNotFoundException($"PlayerProfile {playerProfileId} not found.");
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
            var dailyLogin = await _context.PlayerDailyLogins
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.PlayerProfileId == playerProfileId);

            if (dailyLogin == null)
                return null;

            var isClaimedToday = dailyLogin.LastClaimedAt?.Date == DateTime.UtcNow.Date;
            return new PlayerDailyLoginResponseDto
            {
                PlayerDailyLoginId = dailyLogin.PlayerDailyLoginId,
                PlayerProfileId = dailyLogin.PlayerProfileId,
                CurrentStreak = dailyLogin.CurrentStreak,
                TotalDaysClaimed = dailyLogin.TotalDaysClaimed,
                LastClaimedAt = dailyLogin.LastClaimedAt,
                IsClaimedToday = isClaimedToday
            };
        }

        private async Task<List<WorldMapProgressDto>> BuildMapProgress(int playerProfileId, string currentMapName)
        {
            var activeQuests = await _context.Quests
                .AsNoTracking()
                .Where(q => q.IsActive)
                .Select(q => new { q.QuestId, q.MapName })
                .ToListAsync();

            var npcMapNames = await _context.NPCs
                .AsNoTracking()
                .Where(n => n.IsActive)
                .Select(n => n.MapName)
                .ToListAsync();

            var playerQuestStates = await _context.PlayerQuests
                .AsNoTracking()
                .Include(pq => pq.Quest)
                .Where(pq => pq.PlayerProfileId == playerProfileId && pq.Quest != null && pq.Quest.IsActive)
                .ToListAsync();

            var mapNames = activeQuests
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
                    IsUnlocked = string.Equals(mapName, "ElfForest", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(mapName, currentMapName, StringComparison.OrdinalIgnoreCase)
                        || hasAnyPlayerState,
                    ExplorationPercent = total == 0 ? 0 : (int)Math.Round(completed * 100.0 / total)
                };
            }).ToList();
        }

        private static NPCResponseDto MapNpc(NPC npc)
        {
            return new NPCResponseDto
            {
                NPCId = npc.NPCId,
                Name = npc.Name,
                Description = npc.Description,
                Type = npc.Type,
                MapName = npc.MapName,
                PositionX = npc.PositionX,
                PositionY = npc.PositionY,
                InteractionRadius = npc.InteractionRadius,
                IconUrl = npc.IconUrl,
                IsActive = npc.IsActive,
                Dialogues = npc.Dialogues
                    .OrderBy(d => d.DisplayOrder)
                    .Select(d => new NPCDialogueResponseDto
                    {
                        NPCDialogueId = d.NPCDialogueId,
                        NPCId = d.NPCId,
                        NPCName = npc.Name,
                        Content = d.Content,
                        ResponseType = d.ResponseType,
                        LinkedQuestId = d.LinkedQuestId,
                        LinkedQuestTitle = d.LinkedQuest?.Title,
                        LinkedShopItemId = d.LinkedShopItemId,
                        LinkedShopItemName = d.LinkedShopItem?.Item?.Name,
                        DisplayOrder = d.DisplayOrder,
                        IsActive = d.IsActive
                    })
                    .ToList()
            };
        }

        private static string NormalizeMapName(string? mapName)
        {
            if (string.IsNullOrWhiteSpace(mapName))
                return TutorialMapName;

            var normalized = mapName.Trim();
            return string.Equals(normalized, "ElfLand", StringComparison.OrdinalIgnoreCase)
                ? TutorialMapName
                : normalized;
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
