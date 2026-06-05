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
                    Id = created.QuestId,
                    Title = created.Title,
                    Description = created.Description,
                    Type = created.Type,
                    DefaultStatus = created.DefaultStatus,
                    RequiredLevel = created.RequiredLevel,
                    RewardExperience = created.RewardExperience,
                    RewardGold = created.RewardGold,
                    RewardGems = created.RewardGems,
                    RewardItemId = created.RewardItemId,
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
            quest.RequiredLevel = request.RequiredLevel;
            quest.RewardExperience = request.RewardExperience;
            quest.RewardGold = request.RewardGold;
            quest.RewardGems = request.RewardGems;
            quest.RewardItemId = request.RewardItemId;
            quest.IsActive = request.IsActive;

            var updated = await _repository.UpdateQuest(quest);
            return MapToResponseDto(updated);
        }

        public IQueryable<QuestResponseDto> GetQuestsQueryable()
        {
            return _repository.GetQuestsQueryable()
                .Select(MapToResponseDto)
                .AsQueryable();
        }

        private static QuestResponseDto MapToResponseDto(Quest quest)
        {
            return new QuestResponseDto
            {
                Id = quest.QuestId,
                Title = quest.Title,
                Description = quest.Description,
                Type = quest.Type,
                DefaultStatus = quest.DefaultStatus,
                RequiredLevel = quest.RequiredLevel,
                RewardExperience = quest.RewardExperience,
                RewardGold = quest.RewardGold,
                RewardGems = quest.RewardGems,
                RewardItemId = quest.RewardItemId,
                RewardItemName = quest.RewardItem?.Name,
                IsActive = quest.IsActive
            };
        }
    }
}
