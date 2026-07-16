using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using System.Linq;

namespace BLL.Services
{
    public class QuestService : IQuestService
    {
        private readonly IQuestRepository _repository;
        private readonly IMapper _mapper;

        public QuestService(IQuestRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<QuestResponseDto?> GetQuestById(int id)
        {
            var quest = await _repository.GetByIdWithReward(id);
            if (quest == null)
                return null;

            return await MapQuestResponse(quest);
        }

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
                if (npc == null)
                    throw new InvalidOperationException("Cannot create quest dialogue because Quest Giver / NPC does not match an existing NPC.");
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

        public async Task<PagedResultDto<QuestResponseDto>> GetQuestsPaged(int page, int pageSize, string? search, string? type, bool? isActive, string? mapName, string? sortBy = null, string? sortOrder = null)
        {
            var (totalCount, items) = await _repository.GetQuestsPaged(page, pageSize, search, type, isActive, mapName, sortBy, sortOrder);
            var dtos = _mapper.Map<List<QuestResponseDto>>(items);
            return new PagedResultDto<QuestResponseDto>(totalCount, dtos);
        }

        public async Task<List<NPCResponseDto>> GetQuestNpcOptions(string? mapName)
        {
            var npcs = await _repository.GetQuestNpcOptions(mapName);
            return _mapper.Map<List<NPCResponseDto>>(npcs);
        }
        private async Task<QuestResponseDto> MapQuestResponse(Quest quest)
        {
            var dto = _mapper.Map<QuestResponseDto>(quest);
            var dialogue = await _repository.GetQuestDialogueByQuestId(quest.QuestId);
            if (dialogue == null)
                return dto;

            dto.DialogueId = dialogue.NPCDialogueId;
            dto.DialogueNpcId = dialogue.NPCId;
            dto.DialogueNpcName = dialogue.NPC?.Name;
            dto.DialogueContent = dialogue.Content;
            dto.DialogueDisplayOrder = dialogue.DisplayOrder;
            dto.DialogueIsActive = dialogue.IsActive;
            return dto;
        }

        private async Task SyncQuestDialogue(Quest quest, UpdateQuestRequestDto request)
        {
            var existing = await _repository.GetQuestDialogueByQuestId(quest.QuestId);
            var content = request.DialogueContent?.Trim();

            if (string.IsNullOrWhiteSpace(content))
            {
                if (existing != null)
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
                ?? throw new InvalidOperationException("Cannot create quest dialogue because Quest Giver / NPC does not match an existing NPC.");

            if (existing == null)
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

        private static List<RewardItemValue> NormalizeRewardItems(UpdateQuestRequestDto request)
        {
            var normalized = (request.RewardItems ?? new List<UpdateQuestRewardItemDto>())
                .Where(item => item.ItemId > 0)
                .GroupBy(item => item.ItemId)
                .Select(group => new RewardItemValue
                {
                    ItemId = group.Key,
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

        private static void SyncRewardItems(Quest quest, List<RewardItemValue> rewardItems)
        {
            var requestedItemIds = rewardItems.Select(item => item.ItemId).ToHashSet();
            var removedItems = quest.RewardItems
                .Where(item => !requestedItemIds.Contains(item.ItemId))
                .ToList();

            foreach (var removedItem in removedItems)
            {
                quest.RewardItems.Remove(removedItem);
            }

            foreach (var rewardItem in rewardItems)
            {
                var existing = quest.RewardItems.FirstOrDefault(item => item.ItemId == rewardItem.ItemId);
                if (existing == null)
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

        private static List<RewardSkillValue> NormalizeRewardSkills(UpdateQuestRequestDto request)
        {
            var normalized = (request.RewardSkills ?? new List<UpdateQuestRewardSkillDto>())
                .Where(skill => skill.SkillId > 0)
                .GroupBy(skill => skill.SkillId)
                .Select(group => new RewardSkillValue { SkillId = group.Key })
                .ToList();

            if (normalized.Count == 0 && request.RewardSkillId.HasValue && request.RewardSkillId.Value > 0)
            {
                normalized.Add(new RewardSkillValue { SkillId = request.RewardSkillId.Value });
            }

            return normalized;
        }

        private static void SyncRewardSkills(Quest quest, List<RewardSkillValue> rewardSkills)
        {
            var requestedSkillIds = rewardSkills.Select(skill => skill.SkillId).ToHashSet();
            var removedSkills = quest.RewardSkills
                .Where(skill => !requestedSkillIds.Contains(skill.SkillId))
                .ToList();

            foreach (var removedSkill in removedSkills)
            {
                quest.RewardSkills.Remove(removedSkill);
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
        private sealed class RewardItemValue
        {
            public int ItemId { get; init; }
            public int Quantity { get; init; }
        }

        private sealed class RewardSkillValue
        {
            public int SkillId { get; init; }
        }
    }
}