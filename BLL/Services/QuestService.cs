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

            return MapToResponseDto(quest);
        }

        public async Task<QuestResponseDto> CreateQuest(CreateQuestRequestDto request)
        {
            var quest = _mapper.Map<Quest>(request);

            var created = await _repository.CreateQuest(quest);
            return await GetQuestById(created.QuestId)
                ?? new QuestResponseDto
                {
                    QuestId = created.QuestId,
                    Title = created.Title,
                    Description = created.Description,
                    Type = created.Type,
                    DefaultStatus = created.DefaultStatus,
                    MapName = created.MapName,
                    RegionName = created.RegionName,
                    ObjectiveType = created.ObjectiveType,
                    ObjectiveTarget = created.ObjectiveTarget,
                    ObjectiveLocation = created.ObjectiveLocation,
                    QuestGiverName = created.QuestGiverName,
                    RequiredLevel = created.RequiredLevel,
                    TargetAmount = created.TargetAmount,
                    RewardExperience = created.RewardExperience,
                    RewardGold = created.RewardGold,
                    RewardGems = created.RewardGems,
                    RewardItemId = created.RewardItemId,
                    RewardSkillId = created.RewardSkillId,
                    IsActive = created.IsActive
                };
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
            quest.RewardItemId = request.RewardItemId;
            quest.RewardSkillId = request.RewardSkillId;
            quest.IsActive = request.IsActive;

            var updated = await _repository.UpdateQuest(quest);
            return MapToResponseDto(updated);
        }

        public async Task<PagedResultDto<QuestResponseDto>> GetQuestsPaged(int page, int pageSize, string? search, string? type, bool? isActive, string? mapName)
        {
            var (totalCount, items) = await _repository.GetQuestsPaged(page, pageSize, search, type, isActive, mapName);
            var dtos = items.Select(MapToResponseDto).ToList();
            return new PagedResultDto<QuestResponseDto>(totalCount, dtos);
        }

        private static QuestResponseDto MapToResponseDto(Quest quest)
        {
            return new QuestResponseDto
            {
                QuestId = quest.QuestId,
                Title = quest.Title,
                Description = quest.Description,
                Type = quest.Type,
                DefaultStatus = quest.DefaultStatus,
                MapName = quest.MapName,
                RegionName = quest.RegionName,
                ObjectiveType = quest.ObjectiveType,
                ObjectiveTarget = quest.ObjectiveTarget,
                ObjectiveLocation = quest.ObjectiveLocation,
                QuestGiverName = quest.QuestGiverName,
                RequiredLevel = quest.RequiredLevel,
                TargetAmount = quest.TargetAmount,
                RewardExperience = quest.RewardExperience,
                RewardGold = quest.RewardGold,
                RewardGems = quest.RewardGems,
                RewardItemId = quest.RewardItemId,
                RewardItemName = quest.RewardItem?.Name,
                RewardSkillId = quest.RewardSkillId,
                RewardSkillName = quest.RewardSkill?.Name,
                IsActive = quest.IsActive
            };
        }
    }
}
