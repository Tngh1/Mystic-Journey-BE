using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
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
        private readonly IInventoryRepository _inventoryRepo;
        private readonly ISkillRepository _skillRepo;
        private readonly ITransactionManager _transactionManager;

        // Anti-cheat: max progress delta per batch call.
        private const int MaxProgressDeltaPerCall = 50;

        private readonly IMapper _mapper;

        public PlayerQuestService(
            IPlayerQuestRepository playerQuestRepo,
            IPlayerProfileRepository playerProfileRepo,
            IQuestRepository questRepo,
            IInventoryRepository inventoryRepo,
            ISkillRepository skillRepo,
            ITransactionManager transactionManager,
            IMapper mapper)
        {
            _playerQuestRepo = playerQuestRepo;
            _playerProfileRepo = playerProfileRepo;
            _questRepo = questRepo;
            _inventoryRepo = inventoryRepo;
            _skillRepo = skillRepo;
            _transactionManager = transactionManager;
            _mapper = mapper;
        }

        public async Task<List<PlayerQuestResponseDto>> GetMyQuests(int playerProfileId)
        {
            var profile = await _playerProfileRepo.GetPlayerProfileById(playerProfileId)
                ?? throw new KeyNotFoundException($"PlayerProfile {playerProfileId} not found.");

            var mapName = NormalizeMapName(profile.LastMapName);

            // 1. Fetch ALL player quest records across ALL maps to preserve active/in-progress quests
            var allPlayerQuests = await _playerQuestRepo.GetByPlayerId(playerProfileId);

            var allActiveQuests = await _questRepo.GetActiveQuests();

            var activeMapQuests = allActiveQuests
                .Where(q => string.Equals(NormalizeMapName(q.MapName), mapName, StringComparison.OrdinalIgnoreCase))
                .OrderBy(q => q.QuestId)
                .ToList();

            var existingMap = allPlayerQuests
                .GroupBy(pq => pq.QuestId)
                .ToDictionary(g => g.Key, g => g.First());

            var createdAny = false;

            foreach (var quest in activeMapQuests.Where(q => !IsMainQuest(q) && q.RequiredLevel <= profile.Level))
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
                    // Ignore duplicate key if created by a concurrent request
                }
            }

            // Main chain is GLOBAL (ordered by QuestId), not per-map. Chapters run 1-8 ElfForest,
            // 9-20 AutumnPumpkin, 21-25 FrozenMountain, 26-31 AbandonedCastle, then 32-34 back in
            // ElfForest. A per-map chain would unlock quest 32 ("Return with the Seals") the moment
            // quest 8 is claimed - the finale before the 4 Seal Books it turns in even exist.
            var mainChain = allActiveQuests
                .Where(IsMainQuest)
                .OrderBy(q => q.QuestId)
                .ToList();

            for (var i = 0; i < mainChain.Count; i++)
            {
                var quest = mainChain[i];
                // KHÔNG lọc theo map ở đây. Chuỗi main quest gối đầu qua các map: claim xong quest
                // 20 (AutumnPumpkin) thì quest kế là 21 (FrozenMountain). Nếu chỉ materialize quest
                // của map đang đứng thì sau khi claim 20 người chơi không còn quest nào chưa Claimed
                // → client PickPreferredQuest trả null → tracker "No quest available." và mũi tên bị
                // Clear(), nên không có gì chỉ đường ra Thuyền để sang map kế.
                // An toàn vì chain tự chặn: IsMainQuestUnlocked đòi quest trước phải "Claimed", nên
                // mỗi lần nhiều nhất chỉ thêm ĐÚNG 1 quest kế tiếp, và AcceptQuest vẫn chặn nhận
                // quest khi chưa đứng đúng map.
                // No level gate on the main chain: it is already paced by "previous quest claimed".
                // Gating it by level creates dead ends whenever the chapter's own exp rewards cannot
                // reach the next quest's RequiredLevel (e.g. quest 8 -> 9: lvl 2 with 85 exp vs lvl 3).
                // RequiredLevel stays a suggestion the client shows as "Suggested: Level X".
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
                    // Ignore duplicate key if created by a concurrent request
                }
            }

            if (createdAny)
                allPlayerQuests = await _playerQuestRepo.GetByPlayerId(playerProfileId);

            existingMap = allPlayerQuests
                .GroupBy(pq => pq.QuestId)
                .ToDictionary(g => g.Key, g => g.First());

            // 2. Visible list: include all active/in-progress/completed quests AND map-specific quests.
            // Main quest luôn hiện dù ở map nào: quest kế tiếp trong chuỗi có thể nằm ở map khác
            // (claim 20 ở AutumnPumpkin → 21 ở FrozenMountain). Lọc nó ra khiến client hết quest
            // "chưa Claimed" → tracker "No quest available." + mũi tên bị Clear() → không có gì chỉ
            // đường ra Thuyền. Side quest vẫn giữ nguyên: chỉ hiện khi đứng đúng map.
            var visible = allPlayerQuests
                .Where(pq => !IsStatus(pq, "NotStarted")
                             || IsMainQuest(pq.Quest)
                             || (pq.Quest != null && string.Equals(NormalizeMapName(pq.Quest.MapName), mapName, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            var sortedVisible = visible
                .GroupBy(pq => pq.QuestId)
                .Select(g => g.First())
                .OrderBy(pq => IsMainQuest(pq.Quest) ? 0 : 1)
                .ThenBy(pq => pq.QuestId)
                .ToList();

            return _mapper.Map<List<PlayerQuestResponseDto>>(sortedVisible);
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
            // Cả 2 phía đều normalize, nếu không quest có MapName="Chapter1" sẽ không bao giờ accept được
            // dù GetMyQuests (đã normalize) vẫn trả nó về cho client.
            if (!string.Equals(NormalizeMapName(quest.MapName), currentMap, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"Quest {request.QuestId} belongs to map {quest.MapName}, but player is currently in {currentMap}.");

            // Main quests are gated by chain order only (see GetMyQuests). Side quests keep the level gate.
            if (!IsMainQuest(quest) && profile.Level < quest.RequiredLevel)
                throw new InvalidOperationException($"Quest {request.QuestId} requires level {quest.RequiredLevel}.");

            if (IsMainQuest(quest) && !await IsMainQuestUnlocked(playerProfileId, quest))
                throw new InvalidOperationException($"Quest {request.QuestId} is locked until the previous main quest is claimed.");

            var existing = await _playerQuestRepo.GetByPlayerAndQuest(playerProfileId, request.QuestId);
            if (existing != null)
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

                return _mapper.Map<PlayerQuestResponseDto>(existing);
            }

            var playerQuest = CreateInitialQuestRecord(playerProfileId, quest);
            playerQuest.Status = "InProgress";
            playerQuest.Progress = 0;
            playerQuest.AcceptedAt = DateTime.UtcNow;

            var created = await _playerQuestRepo.Create(playerQuest);
            created.Quest = quest;
            return _mapper.Map<PlayerQuestResponseDto>(created);
        }

        public async Task<List<PlayerQuestResponseDto>> BatchUpdateProgress(
            int playerProfileId,
            BatchProgressRequestDto request)
        {
            if (request.Updates == null || request.Updates.Count == 0)
                return new List<PlayerQuestResponseDto>();

            var questIds = request.Updates.Select(u => u.QuestId).Distinct().ToList();
            var existingList = await _playerQuestRepo.GetByPlayerAndQuestIds(playerProfileId, questIds);
            var existingMap = existingList.GroupBy(pq => pq.QuestId).ToDictionary(g => g.Key, g => g.First());

            var toUpdate = new List<PlayerQuest>();
            var results = new List<PlayerQuestResponseDto>();

            foreach (var item in request.Updates)
            {
                if (!existingMap.TryGetValue(item.QuestId, out var pq))
                    continue;

                if (pq.Status != "InProgress")
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

                // Collect quest KHÔNG được tự chuyển Completed từ progress ngoài world: nó chỉ hoàn
                // thành khi người chơi nộp vật phẩm cho NPC (TurnInQuestItem — nơi trừ item trong
                // kho). Nếu flip ở đây, quest thành Completed lúc vừa hái đủ → client gặp NPC là
                // AutoClaimCompletedQuest bắn popup "Reward Claimed!" và trả thưởng khi chưa trả
                // nhiệm vụ, đồng thời vật phẩm không bao giờ bị trừ.
                if (pq.Progress >= targetAmount && !IsCollectQuest(pq.Quest))
                {
                    pq.Status = "Completed";
                    pq.CompletedAt ??= DateTime.UtcNow;
                }

                toUpdate.Add(pq);
                results.Add(_mapper.Map<PlayerQuestResponseDto>(pq));
            }

            if (toUpdate.Count > 0)
                await _playerQuestRepo.UpdateRange(toUpdate);

            return results;
        }

        // Atomic: status flips to "Claimed" first, then gold/exp/gems/items/skills are granted.
        // Without the transaction any failure after the status write burns the quest with no reward.
        public Task<PlayerQuestResponseDto> ClaimReward(int playerProfileId, ClaimQuestRequestDto request)
            => _transactionManager.ExecuteInTransactionAsync(() => ClaimRewardCore(playerProfileId, request));

        private async Task<PlayerQuestResponseDto> ClaimRewardCore(int playerProfileId, ClaimQuestRequestDto request)
        {
            var quest = await _questRepo.GetByIdWithReward(request.QuestId)
                ?? throw new ArgumentException($"Quest {request.QuestId} does not exist.");

            // Trust boundary: only a quest the player actually holds and finished can be claimed.
            // Fabricating a "Completed" record here let a client claim rewards for quests it never
            // accepted, skipping the whole chain (and its turn-in item costs).
            var pq = await _playerQuestRepo.GetByPlayerAndQuest(playerProfileId, request.QuestId)
                ?? throw new KeyNotFoundException($"PlayerQuest not found for questId={request.QuestId}.");

            if (pq.Status == "Claimed")
            {
                return _mapper.Map<PlayerQuestResponseDto>(pq);
            }

            if (pq.Status != "Completed")
                throw new InvalidOperationException($"Quest {request.QuestId} is not completed yet.");

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

            // Quest items are now consumed in CompleteQuest
            var rewardItems = quest.RewardItems
                .Where(item => item.ItemId > 0 && item.Quantity > 0)
                .GroupBy(item => item.ItemId)
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

            // If quest grants skills, add them to player's skills (if not already owned).
            var owned = await _skillRepo.GetPlayerSkillsByPlayerId(playerProfileId);
            var rewardSkillIds = quest.RewardSkills
                .Where(skill => skill.SkillId > 0)
                .Select(skill => skill.SkillId)
                .Distinct()
                .ToList();

            if (rewardSkillIds.Count == 0 && quest.RewardSkillId.HasValue)
            {
                // Dynamic class skill reward for Q3: Deliver White Flowers
                if (quest.Title != null && quest.Title.Contains("Deliver White Flowers"))
                {
                    string classSkillName = profile.Class switch
                    {
                        "Mage" => "AP_Skill",
                        "Archer" => "Skill_Ad",
                        "Knight" => "Skill_Knight Attack",
                        _ => "Dark Poison Zone"
                    };

                    var classSkill = await _skillRepo.GetSkillByName(classSkillName);
                    if (classSkill != null)
                        rewardSkillIds.Add(classSkill.SkillId);
                    else
                        rewardSkillIds.Add(quest.RewardSkillId.Value);
                }
                else
                {
                    rewardSkillIds.Add(quest.RewardSkillId.Value);
                }
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
// [HACK] Tutorial: Nhận đủ 3 skill cơ bản khi xong Quest Hái Hoa
            if (quest.Title != null && quest.Title.Contains("Gather White Flowers", StringComparison.OrdinalIgnoreCase))
            {
                var allSkills = await _skillRepo.GetAllSkillsAsync();
                if (allSkills != null)
                {
                    foreach (var skill in allSkills)
                    {
                        if (owned.Any(ps => ps.SkillId == skill.SkillId))
                            continue;

                        var createdSkill = await _skillRepo.CreatePlayerSkill(new PlayerSkill
                        {
                            PlayerProfileId = playerProfileId,
                            SkillId         = skill.SkillId,
                            Level           = 1,
                            Experience      = 0,
                            EquippedSlot    = null,
                            UnlockedAt      = DateTime.UtcNow
                        });
                        owned.Add(createdSkill);
                    }
                }
            }

            return _mapper.Map<PlayerQuestResponseDto>(pq);
        }

        public async Task<PlayerQuestResponseDto> CompleteQuest(int playerProfileId, CompleteQuestRequestDto request)
        {
            var pq = await _playerQuestRepo.GetByPlayerAndQuest(playerProfileId, request.QuestId)
                ?? throw new KeyNotFoundException($"PlayerQuest not found for questId={request.QuestId}.");

            if (pq.Status == "Claimed" || pq.Status == "Completed")
                return _mapper.Map<PlayerQuestResponseDto>(pq);

            if (pq.Status != "InProgress")
                throw new InvalidOperationException($"Quest {request.QuestId} is not in progress.");

            var targetAmount = Math.Max(1, pq.Quest?.TargetAmount ?? pq.TargetValue);
            pq.TargetValue = targetAmount;

            var canComplete = pq.Progress >= targetAmount ||
                IsTalkQuest(pq.Quest) ||
                (IsEquipSkillQuest(pq.Quest) && await HasEquippedSkill(playerProfileId));

            if (!canComplete)
                throw new InvalidOperationException("Quest objective is not complete yet.");

            await ConsumeQuestTurnInItemsIfNeeded(playerProfileId, pq.Quest);

            pq.Progress = targetAmount;
            pq.Status = "Completed";
            pq.CompletedAt ??= DateTime.UtcNow;

            // Quest turn-in items, if any, were consumed before marking the quest complete.
            var updated = await _playerQuestRepo.Update(pq);
            return _mapper.Map<PlayerQuestResponseDto>(updated);
        }

        private async Task ConsumeQuestTurnInItemsIfNeeded(int playerProfileId, Quest? quest)
        {
            var requirements = ResolveQuestTurnInRequirement(quest);
            if (requirements == null || requirements.Count == 0)
                return;

            var invItems = await _inventoryRepo.GetByPlayerId(playerProfileId);

            foreach (var req in requirements)
            {
                var targetItem = invItems.FirstOrDefault(i =>
                    i.Item != null &&
                    string.Equals(i.Item.Type, "QuestItem", StringComparison.OrdinalIgnoreCase) &&
                    Contains(i.Item.Name, req.itemName));

                var available = targetItem?.Quantity ?? 0;
                var consumedQuantity = Math.Min(available, req.quantity);
                if (targetItem == null || consumedQuantity <= 0)
                    continue;

                if (targetItem.Quantity <= consumedQuantity)
                    await _inventoryRepo.DeleteItem(targetItem.InventoryItemId);
                else
                {
                    targetItem.Quantity -= consumedQuantity;
                    await _inventoryRepo.UpdateItem(targetItem);
                }
            }
        }

        private static List<(string itemName, int quantity)> ResolveQuestTurnInRequirement(Quest? quest)
        {
            var reqs = new List<(string, int)>();
            if (quest == null) return reqs;

            var text = $"{quest.Title} {quest.Description} {quest.ObjectiveTarget} {quest.ObjectiveLocation}";
            
            // For Q25: use the 4 Seal Books
            if (Contains(text, "cleanse the tree") || Contains(text, "4 Seal Books"))
            {
                reqs.Add(("Swamp Seal Book", 1));
                reqs.Add(("Dragon Seal Book", 1));
                reqs.Add(("Golem Seal Book", 1));
                reqs.Add(("UnderKing Seal Book", 1));
            }

            // For Q22: Deserted Island (Consumes Mystic Key)
            if (Contains(text, "Deserted Island") || Contains(text, "Elf Guard"))
            {
                // Only deduct if this is the Deserted Island quest
                if (Contains(text, "collect 5 Ancient Leaves"))
                {
                    reqs.Add(("Mystic Key", 1));
                }
            }

            var isTurnInQuest = Contains(text, "Report") || Contains(text, "Return") || Contains(text, "Hand over") || Contains(text, "Handed over") || Contains(text, "Help") || Contains(text, "Deliver") || Contains(text, "Bury");
            
            if (!isTurnInQuest && reqs.Count == 0)
                return reqs;
            
            if (Contains(text, "White Flower") || Contains(text, "White Flowers"))
                reqs.Add(("White Flower", 3));

            if (Contains(text, "Old Willow Branch") || Contains(text, "Willow Branch"))
                reqs.Add(("Old Willow Branch", Math.Max(1, quest.TargetAmount)));

            // Deliver/Harvest quests in AutumnPumpkin require Enchanted Pumpkins
            // These quests have "Deliver" in title but may not have "Pumpkin" in description
            if (Contains(text, "Pumpkin") || Contains(text, "Enchanted Pumpkin") ||
                (Contains(text, "Deliver") && Contains(text, "Harvest")))
                reqs.Add(("Enchanted Pumpkin", Math.Max(1, quest.TargetAmount)));

            if (Contains(text, "Flour") || Contains(text, "Magic Flour"))
                reqs.Add(("Magic Flour", Math.Max(1, quest.TargetAmount)));

            if (Contains(text, "Skull") || Contains(text, "Spirit Skull") || Contains(text, "remains") || Contains(text, "Natalie"))
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

        private static bool Contains(string? source, string value)
        {
            return !string.IsNullOrWhiteSpace(source) &&
                   !string.IsNullOrWhiteSpace(value) &&
                   source.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private async Task AddItemToInventory(int playerProfileId, int itemId, int quantity)
        {
            var existing = await _inventoryRepo.GetByPlayerAndItem(playerProfileId, itemId);

            if (existing != null)
            {
                existing.Quantity += quantity;
                await _inventoryRepo.UpdateItem(existing);
            }
            else
            {
                await _inventoryRepo.AddItem(new InventoryItem
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

        private async Task<bool> IsMainQuestUnlocked(int playerProfileId, Quest quest)
        {
            // Global chain ordered by QuestId - must match GetMyQuests, otherwise a quest visible in
            // the list would be rejected on accept (or vice versa).
            var mainChain = (await _questRepo.GetActiveQuests())
                .Where(IsMainQuest)
                .OrderBy(q => q.QuestId)
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

        private static bool IsMainQuestUnlocked(List<Quest> mainChain, int index, IReadOnlyDictionary<int, PlayerQuest> existingMap)
        {
            if (index <= 0)
                return index == 0;

            var previousQuestId = mainChain[index - 1].QuestId;
            return existingMap.TryGetValue(previousQuestId, out var previousRecord) &&
                IsStatus(previousRecord, "Claimed");
        }

        private async Task<bool> HasEquippedSkill(int playerProfileId)
        {
            var skills = await _skillRepo.GetPlayerSkillsByPlayerId(playerProfileId);
            return skills.Any(ps => ps.EquippedSlot.HasValue);
        }

        private static bool IsMainQuest(Quest? quest)
            => string.Equals(quest?.Type, "Main", StringComparison.OrdinalIgnoreCase);

        private static bool IsTalkQuest(Quest? quest)
            => string.Equals(quest?.ObjectiveType, "Talk", StringComparison.OrdinalIgnoreCase);

        // Collect chỉ hoàn thành qua WorldService.TurnInQuestItem (nộp vật phẩm cho NPC).
        private static bool IsCollectQuest(Quest? quest)
            => string.Equals(quest?.ObjectiveType, "Collect", StringComparison.OrdinalIgnoreCase);


        private static bool IsEquipSkillQuest(Quest? quest)
            => string.Equals(quest?.ObjectiveType, "EquipSkill", StringComparison.OrdinalIgnoreCase);

        private static bool IsStatus(PlayerQuest? pq, string status)
            => string.Equals(pq?.Status, status, StringComparison.OrdinalIgnoreCase);

        private static string NormalizeMapName(string? mapName)
        {
            if (string.IsNullOrWhiteSpace(mapName))
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

        // Methods ApplyExperience and RequiredTotalExperienceForLevel have been moved to PlayerProfile model


    }
}