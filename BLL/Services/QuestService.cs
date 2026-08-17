using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using System.Linq;

namespace BLL.Services
{
    // Executes core business logic for i quest service.
    public class QuestService : IQuestService
    {
        private readonly IQuestRepository _repository;
        private readonly IMapper _mapper;

        // Initializes a new instance of QuestService with dependencies: repository, mapper.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public QuestService(IQuestRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // Executes core business logic for get quest by id.
        // Logic details: validates required non-empty string arguments; delegates data queries and updates to repository layer; throws InvalidOperationException on invalid state or rule violations.
        // Returns the computed QuestResponseDto? result asynchronously.
        public async Task<QuestResponseDto?> GetQuestById(int id)
        {
            var quest = await _repository.GetByIdWithReward(id);
            if (quest == null)  // Entity not found — short-circuit with appropriate error result
                return null;

            return await MapQuestResponse(quest);
        }

        // Executes core business logic for create quest.
        // Returns the computed QuestResponseDto result asynchronously.
        public async Task<QuestResponseDto> CreateQuest(UpdateQuestRequestDto request)
        {
            var quest = new Quest
            {
                Title = request.Title,
                Description = request.Description,
                Type = request.Type,
                DefaultStatus = request.DefaultStatus,
                MapName = request.MapName,
                RegionName = request.RegionName,
                ObjectiveType = request.ObjectiveType,
                ObjectiveTarget = request.ObjectiveTarget,
                ObjectiveLocation = request.ObjectiveLocation,
                QuestGiverName = request.QuestGiverName,
                RequiredLevel = request.RequiredLevel,
                TargetAmount = request.TargetAmount,
                RewardExperience = request.RewardExperience,
                RewardGold = request.RewardGold,
                RewardGems = request.RewardGems,
                IsActive = request.IsActive
            };

            var rewardItems = NormalizeRewardItems(request);
            quest.RewardItemId = rewardItems.FirstOrDefault()?.ItemId;
            SyncRewardItems(quest, rewardItems);

            var rewardSkills = NormalizeRewardSkills(request);
            quest.RewardSkillId = rewardSkills.FirstOrDefault()?.SkillId;
            SyncRewardSkills(quest, rewardSkills);

            if (request.SyncDialogue && !string.IsNullOrWhiteSpace(request.DialogueContent))
            {
                var npc = await _repository.GetNpcByNameAndMap(quest.QuestGiverName, quest.MapName);
                if (npc == null)  // Entity not found — short-circuit with appropriate error result
                    throw new InvalidOperationException("Cannot create quest dialogue because Quest Giver / NPC does not match an existing NPC.");  // Unexpected runtime state — propagate to global error handler
            }

            var created = await _repository.AddQuest(quest);

            if (request.SyncDialogue)
            {
                await SyncQuestDialogue(created, request);
                await _repository.UpdateQuest(created);
            }

            var persisted = await _repository.GetByIdWithReward(created.QuestId) ?? created;
            return await MapQuestResponse(persisted);
        }
        // Executes core business logic for update quest.
        // Logic details: delegates data queries and updates to repository layer; throws KeyNotFoundException on invalid state or rule violations.
        // Returns the computed QuestResponseDto result asynchronously.
        public async Task<QuestResponseDto> UpdateQuest(int id, UpdateQuestRequestDto request)
        {
            var quest = await _repository.GetByIdWithReward(id)
                ?? throw new KeyNotFoundException($"Quest with id {id} not found.");

            quest.Title = request.Title;
            quest.Description = request.Description;
            quest.Type = request.Type;
            quest.DefaultStatus = request.DefaultStatus;
            quest.MapName = request.MapName;
            quest.RegionName = request.RegionName;
            quest.ObjectiveType = request.ObjectiveType;
            quest.ObjectiveTarget = request.ObjectiveTarget;
            quest.ObjectiveLocation = request.ObjectiveLocation;
            quest.QuestGiverName = request.QuestGiverName;
            quest.RequiredLevel = request.RequiredLevel;
            quest.TargetAmount = request.TargetAmount;
            quest.RewardExperience = request.RewardExperience;
            quest.RewardGold = request.RewardGold;
            quest.RewardGems = request.RewardGems;
            quest.IsActive = request.IsActive;

            var rewardItems = NormalizeRewardItems(request);
            quest.RewardItemId = rewardItems.FirstOrDefault()?.ItemId;
            SyncRewardItems(quest, rewardItems);

            var rewardSkills = NormalizeRewardSkills(request);
            quest.RewardSkillId = rewardSkills.FirstOrDefault()?.SkillId;
            SyncRewardSkills(quest, rewardSkills);

            if (request.SyncDialogue)
                await SyncQuestDialogue(quest, request);

            var updated = await _repository.UpdateQuest(quest);
            return await MapQuestResponse(updated);
        }

        // Executes core business logic for get quests paged.
        // Logic details: delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models.
        // Returns the computed PagedResultDto<QuestResponseDto result asynchronously.
        public async Task<PagedResultDto<QuestResponseDto>> GetQuestsPaged(int page, int pageSize, string? search, string? type, bool? isActive, string? mapName, string? sortBy = null, string? sortOrder = null)
        {
            var (totalCount, items) = await _repository.GetQuestsPaged(page, pageSize, search, type, isActive, mapName, sortBy, sortOrder);
            var dtos = _mapper.Map<List<QuestResponseDto>>(items);  // Transform domain entity into DTO for the API response layer
            return new PagedResultDto<QuestResponseDto>(totalCount, dtos);
        }

        // Executes core business logic for get quest npc options.
        // Logic details: delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models.
        // Returns the computed List<NPCResponseDto result asynchronously.
        public async Task<List<NPCResponseDto>> GetQuestNpcOptions(string? mapName)
        {
            var npcs = await _repository.GetQuestNpcOptions(mapName);
            return _mapper.Map<List<NPCResponseDto>>(npcs);  // Transform domain entity into DTO for the API response layer
        }
        // Executes core business logic for map quest response.
        // Logic details: validates required non-empty string arguments; delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models; throws InvalidOperationException on invalid state or rule violations.
        // Returns the computed QuestResponseDto result asynchronously.
        private async Task<QuestResponseDto> MapQuestResponse(Quest quest)
        {
            var dto = _mapper.Map<QuestResponseDto>(quest);  // Transform domain entity into DTO for the API response layer
            var dialogue = await _repository.GetQuestDialogueByQuestId(quest.QuestId);
            if (dialogue == null)  // Entity not found — short-circuit with appropriate error result
                return dto;

            dto.DialogueId = dialogue.NPCDialogueId;
            dto.DialogueNpcId = dialogue.NPCId;
            dto.DialogueNpcName = dialogue.NPC?.Name;
            dto.DialogueContent = dialogue.Content;
            dto.DialogueDisplayOrder = dialogue.DisplayOrder;
            dto.DialogueIsActive = dialogue.IsActive;
            return dto;
        }

        // Executes core business logic for sync quest dialogue.
        // Logic details: validates required non-empty string arguments; delegates data queries and updates to repository layer.
        // Completes asynchronously upon successful execution.
        private async Task SyncQuestDialogue(Quest quest, UpdateQuestRequestDto request)
        {
            var existing = await _repository.GetQuestDialogueByQuestId(quest.QuestId);
            var content = request.DialogueContent?.Trim();

            if (string.IsNullOrWhiteSpace(content))  // Mandatory string argument is blank — fail fast
            {
                if (existing != null)  // Entity exists — proceed with conditional branch
                {
                    existing.Content = string.Empty;
                    existing.IsActive = false;
                    existing.ResponseType = "Quest";
                    existing.LinkedQuestId = quest.QuestId;
                    existing.LinkedShopItemId = null;
                }

                return;
            }

            var npc = await _repository.GetNpcByNameAndMap(quest.QuestGiverName, quest.MapName)
                ?? throw new InvalidOperationException("Cannot create quest dialogue because Quest Giver / NPC does not match an existing NPC.");  // Unexpected runtime state — propagate to global error handler

            if (existing == null)  // Entity not found — short-circuit with appropriate error result
            {
                _repository.AddQuestDialogue(new NPCDialogue
                {
                    NPCId = npc.NPCId,
                    Content = content,
                    ResponseType = "Quest",
                    LinkedQuestId = quest.QuestId,
                    LinkedShopItemId = null,
                    DisplayOrder = Math.Max(0, request.DialogueDisplayOrder ?? 0),
                    IsActive = request.DialogueIsActive ?? true
                });
                return;
            }

            existing.NPCId = npc.NPCId;
            existing.Content = content;
            existing.ResponseType = "Quest";
            existing.LinkedQuestId = quest.QuestId;
            existing.LinkedShopItemId = null;
            existing.DisplayOrder = Math.Max(0, request.DialogueDisplayOrder ?? existing.DisplayOrder);
            existing.IsActive = request.DialogueIsActive ?? true;
        }

        // Executes core business logic for normalize reward items.
        private static List<RewardItemValue> NormalizeRewardItems(UpdateQuestRequestDto request)
        {
            var normalized = (request.RewardItems ?? new List<UpdateQuestRewardItemDto>())
                .Where(item => item.ItemId > 0)  // Filter records matching the predicate
                .GroupBy(item => item.ItemId)  // Aggregate records by grouping key
                .Select(group => new RewardItemValue
                {
                    ItemId = group.Key,
                    // Clamp the calculated value to the minimum and maximum accepted by this domain rule.
                    Quantity = Math.Min(10000, group.Sum(item => Math.Max(1, item.Quantity)))
                })
                .ToList();

            if (normalized.Count == 0 && request.RewardItemId.HasValue && request.RewardItemId.Value > 0)
            {
                normalized.Add(new RewardItemValue
                {
                    ItemId = request.RewardItemId.Value,
                    Quantity = 1
                });
            }

            return normalized;
        }

        // Executes core business logic for sync reward items.
        private static void SyncRewardItems(Quest quest, List<RewardItemValue> rewardItems)
        {
            var requestedItemIds = rewardItems.Select(item => item.ItemId).ToHashSet();
            var removedItems = quest.RewardItems
                .Where(item => !requestedItemIds.Contains(item.ItemId))  // Filter records matching the predicate
                .ToList();

            foreach (var removedItem in removedItems)
            {
                quest.RewardItems.Remove(removedItem);  // Mark entity for deletion in the next SaveChanges call
            }

            foreach (var rewardItem in rewardItems)
            {
                var existing = quest.RewardItems.FirstOrDefault(item => item.ItemId == rewardItem.ItemId);
                if (existing == null)  // Entity not found — short-circuit with appropriate error result
                {
                    quest.RewardItems.Add(new QuestRewardItem
                    {
                        ItemId = rewardItem.ItemId,
                        Quantity = rewardItem.Quantity
                    });
                    continue;
                }

                existing.Quantity = rewardItem.Quantity;
            }
        }

        // Executes core business logic for normalize reward skills.
        private static List<RewardSkillValue> NormalizeRewardSkills(UpdateQuestRequestDto request)
        {
            var normalized = (request.RewardSkills ?? new List<UpdateQuestRewardSkillDto>())
                .Where(skill => skill.SkillId > 0)  // Filter records matching the predicate
                .GroupBy(skill => skill.SkillId)  // Aggregate records by grouping key
                .Select(group => new RewardSkillValue { SkillId = group.Key })
                .ToList();

            if (normalized.Count == 0 && request.RewardSkillId.HasValue && request.RewardSkillId.Value > 0)
            {
                normalized.Add(new RewardSkillValue { SkillId = request.RewardSkillId.Value });
            }

            return normalized;
        }

        // Executes core business logic for sync reward skills.
        private static void SyncRewardSkills(Quest quest, List<RewardSkillValue> rewardSkills)
        {
            var requestedSkillIds = rewardSkills.Select(skill => skill.SkillId).ToHashSet();
            var removedSkills = quest.RewardSkills
                .Where(skill => !requestedSkillIds.Contains(skill.SkillId))  // Filter records matching the predicate
                .ToList();

            foreach (var removedSkill in removedSkills)
            {
                quest.RewardSkills.Remove(removedSkill);  // Mark entity for deletion in the next SaveChanges call
            }

            foreach (var rewardSkill in rewardSkills)
            {
                if (quest.RewardSkills.Any(skill => skill.SkillId == rewardSkill.SkillId))
                    continue;

                quest.RewardSkills.Add(new QuestRewardSkill
                {
                    SkillId = rewardSkill.SkillId
                });
            }
        }
        // Executes core business logic for reward item value.
        private sealed class RewardItemValue
        {
            // Executes core business logic for item id.
            public int ItemId { get; init; }
            // Executes core business logic for quantity.
            public int Quantity { get; init; }
        }

        // Executes core business logic for reward skill value.
        private sealed class RewardSkillValue
        {
            // Executes core business logic for skill id.
            public int SkillId { get; init; }
        }
    }
}
