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
    public class QuestService : IQuestService
    {
        private readonly IQuestRepository _questRepository;
        private readonly IPlayerQuestRepository _playerQuestRepository;
        private readonly IPlayerProfileRepository _profileRepository;
        private readonly IPlayerProfileService _playerProfileService;

        public QuestService(
            IQuestRepository questRepository,
            IPlayerQuestRepository playerQuestRepository,
            IPlayerProfileRepository profileRepository,
            IPlayerProfileService playerProfileService)
        {
            _questRepository = questRepository;
            _playerQuestRepository = playerQuestRepository;
            _profileRepository = profileRepository;
            _playerProfileService = playerProfileService;
        }

        public async Task<QuestListResponseDto> GetAllQuestsAsync()
        {
            var quests = await _questRepository.GetAllActiveAsync();

            var dtos = quests.Select(q => new QuestResponseDto
            {
                QuestId = q.Id,
                Title = q.Title,
                Description = q.Description,
                Type = q.Type.ToString(),
                RequiredLevel = q.RequiredLevel,
                RewardExperience = q.RewardExperience,
                RewardGold = q.RewardGold,
                RewardGems = q.RewardGems,
                RewardItemId = q.RewardItemId,
                RewardItemName = q.RewardItem?.Name,
                IsActive = q.IsActive
            }).ToList();

            return new QuestListResponseDto
            {
                Success = true,
                Message = "Quests retrieved successfully.",
                Quests = dtos,
                TotalCount = dtos.Count
            };
        }

        public async Task<QuestListResponseDto> GetQuestsByTypeAsync(Quest.QuestType type)
        {
            var quests = await _questRepository.GetByTypeAsync(type);

            var dtos = quests.Select(q => new QuestResponseDto
            {
                QuestId = q.Id,
                Title = q.Title,
                Description = q.Description,
                Type = q.Type.ToString(),
                RequiredLevel = q.RequiredLevel,
                RewardExperience = q.RewardExperience,
                RewardGold = q.RewardGold,
                RewardGems = q.RewardGems,
                RewardItemId = q.RewardItemId,
                RewardItemName = q.RewardItem?.Name,
                IsActive = q.IsActive
            }).ToList();

            return new QuestListResponseDto
            {
                Success = true,
                Message = $"Quests of type {type} retrieved successfully.",
                Quests = dtos,
                TotalCount = dtos.Count
            };
        }

        public async Task<QuestListResponseDto> GetAvailableQuestsAsync(Guid accountId)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(accountId);
            if (profile == null)
            {
                return new QuestListResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var quests = await _questRepository.GetAvailableForLevelAsync(profile.Level);

            var dtos = quests.Select(q => new QuestResponseDto
            {
                QuestId = q.Id,
                Title = q.Title,
                Description = q.Description,
                Type = q.Type.ToString(),
                RequiredLevel = q.RequiredLevel,
                RewardExperience = q.RewardExperience,
                RewardGold = q.RewardGold,
                RewardGems = q.RewardGems,
                RewardItemId = q.RewardItemId,
                RewardItemName = q.RewardItem?.Name,
                IsActive = q.IsActive
            }).ToList();

            return new QuestListResponseDto
            {
                Success = true,
                Message = "Available quests retrieved successfully.",
                Quests = dtos,
                TotalCount = dtos.Count
            };
        }

        public async Task<QuestApiResponseDto> GetQuestByIdAsync(Guid questId)
        {
            var quest = await _questRepository.GetByIdWithRewardAsync(questId);
            if (quest == null)
            {
                return new QuestApiResponseDto
                {
                    Success = false,
                    Message = "Quest not found."
                };
            }

            var dto = new QuestResponseDto
            {
                QuestId = quest.Id,
                Title = quest.Title,
                Description = quest.Description,
                Type = quest.Type.ToString(),
                RequiredLevel = quest.RequiredLevel,
                RewardExperience = quest.RewardExperience,
                RewardGold = quest.RewardGold,
                RewardGems = quest.RewardGems,
                RewardItemId = quest.RewardItemId,
                RewardItemName = quest.RewardItem?.Name,
                IsActive = quest.IsActive
            };

            return new QuestApiResponseDto
            {
                Success = true,
                Message = "Quest retrieved successfully.",
                Quest = dto
            };
        }

        public async Task<QuestListResponseDto> GetPlayerQuestsAsync(Guid accountId)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(accountId);
            if (profile == null)
            {
                return new QuestListResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var playerQuests = await _playerQuestRepository.GetByPlayerProfileIdAsync(profile.Id);

            var dtos = playerQuests.Select(pq => new PlayerQuestResponseDto
            {
                PlayerQuestId = pq.Id,
                PlayerProfileId = pq.PlayerProfileId,
                QuestId = pq.QuestId,
                QuestTitle = pq.Quest?.Title ?? string.Empty,
                QuestDescription = pq.Quest?.Description,
                Type = pq.Quest?.Type.ToString() ?? string.Empty,
                Status = pq.Status.ToString(),
                Progress = pq.Progress,
                TargetValue = pq.TargetValue,
                RewardExperience = pq.Quest?.RewardExperience ?? 0,
                RewardGold = pq.Quest?.RewardGold ?? 0,
                RewardGems = pq.Quest?.RewardGems ?? 0,
                AcceptedAt = pq.AcceptedAt,
                CompletedAt = pq.CompletedAt,
                ClaimedAt = pq.ClaimedAt
            }).ToList();

            return new QuestListResponseDto
            {
                Success = true,
                Message = "Player quests retrieved successfully.",
                PlayerQuests = dtos,
                TotalCount = dtos.Count
            };
        }

        public async Task<QuestListResponseDto> GetActiveQuestsAsync(Guid accountId)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(accountId);
            if (profile == null)
            {
                return new QuestListResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var playerQuests = await _playerQuestRepository.GetActiveQuestsAsync(profile.Id);

            var dtos = playerQuests.Select(pq => new PlayerQuestResponseDto
            {
                PlayerQuestId = pq.Id,
                PlayerProfileId = pq.PlayerProfileId,
                QuestId = pq.QuestId,
                QuestTitle = pq.Quest?.Title ?? string.Empty,
                QuestDescription = pq.Quest?.Description,
                Type = pq.Quest?.Type.ToString() ?? string.Empty,
                Status = pq.Status.ToString(),
                Progress = pq.Progress,
                TargetValue = pq.TargetValue,
                RewardExperience = pq.Quest?.RewardExperience ?? 0,
                RewardGold = pq.Quest?.RewardGold ?? 0,
                RewardGems = pq.Quest?.RewardGems ?? 0,
                AcceptedAt = pq.AcceptedAt,
                CompletedAt = pq.CompletedAt,
                ClaimedAt = pq.ClaimedAt
            }).ToList();

            return new QuestListResponseDto
            {
                Success = true,
                Message = "Active quests retrieved successfully.",
                PlayerQuests = dtos,
                TotalCount = dtos.Count
            };
        }

        public async Task<QuestListResponseDto> GetCompletedQuestsAsync(Guid accountId)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(accountId);
            if (profile == null)
            {
                return new QuestListResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var playerQuests = await _playerQuestRepository.GetCompletedQuestsAsync(profile.Id);

            var dtos = playerQuests.Select(pq => new PlayerQuestResponseDto
            {
                PlayerQuestId = pq.Id,
                PlayerProfileId = pq.PlayerProfileId,
                QuestId = pq.QuestId,
                QuestTitle = pq.Quest?.Title ?? string.Empty,
                QuestDescription = pq.Quest?.Description,
                Type = pq.Quest?.Type.ToString() ?? string.Empty,
                Status = pq.Status.ToString(),
                Progress = pq.Progress,
                TargetValue = pq.TargetValue,
                RewardExperience = pq.Quest?.RewardExperience ?? 0,
                RewardGold = pq.Quest?.RewardGold ?? 0,
                RewardGems = pq.Quest?.RewardGems ?? 0,
                AcceptedAt = pq.AcceptedAt,
                CompletedAt = pq.CompletedAt,
                ClaimedAt = pq.ClaimedAt
            }).ToList();

            return new QuestListResponseDto
            {
                Success = true,
                Message = "Completed quests retrieved successfully.",
                PlayerQuests = dtos,
                TotalCount = dtos.Count
            };
        }

        public async Task<QuestApiResponseDto> AcceptQuestAsync(Guid accountId, AcceptQuestRequestDto request)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(accountId);
            if (profile == null)
            {
                return new QuestApiResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var quest = await _questRepository.GetByIdWithRewardAsync(request.QuestId);
            if (quest == null)
            {
                return new QuestApiResponseDto
                {
                    Success = false,
                    Message = "Quest not found."
                };
            }

            if (await _playerQuestRepository.HasQuestAsync(profile.Id, request.QuestId))
            {
                return new QuestApiResponseDto
                {
                    Success = false,
                    Message = "You have already accepted this quest."
                };
            }

            if (profile.Level < quest.RequiredLevel)
            {
                return new QuestApiResponseDto
                {
                    Success = false,
                    Message = $"You need to be level {quest.RequiredLevel} to accept this quest."
                };
            }

            var playerQuest = new PlayerQuest
            {
                Id = Guid.NewGuid(),
                PlayerProfileId = profile.Id,
                QuestId = quest.Id,
                Status = Quest.QuestStatus.InProgress,
                Progress = 0,
                TargetValue = 1,
                AcceptedAt = DateTime.UtcNow
            };

            await _playerQuestRepository.CreateAsync(playerQuest);

            return new QuestApiResponseDto
            {
                Success = true,
                Message = $"Quest '{quest.Title}' accepted successfully!"
            };
        }

        public async Task<QuestApiResponseDto> UpdateProgressAsync(Guid accountId, UpdateQuestProgressRequestDto request)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(accountId);
            if (profile == null)
            {
                return new QuestApiResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var playerQuest = await _playerQuestRepository.GetByIdWithDetailsAsync(request.PlayerQuestId);
            if (playerQuest == null || playerQuest.PlayerProfileId != profile.Id)
            {
                return new QuestApiResponseDto
                {
                    Success = false,
                    Message = "Player quest not found."
                };
            }

            if (playerQuest.Status == Quest.QuestStatus.Completed)
            {
                return new QuestApiResponseDto
                {
                    Success = false,
                    Message = "Quest is already completed. Please claim your reward."
                };
            }

            if (playerQuest.Status == Quest.QuestStatus.Claimed)
            {
                return new QuestApiResponseDto
                {
                    Success = false,
                    Message = "Quest reward has already been claimed."
                };
            }

            if (playerQuest.Status == Quest.QuestStatus.NotStarted)
            {
                playerQuest.Status = Quest.QuestStatus.InProgress;
            }

            playerQuest.Progress += request.ProgressAmount;

            if (playerQuest.Progress >= playerQuest.TargetValue)
            {
                playerQuest.Progress = playerQuest.TargetValue;
                playerQuest.Status = Quest.QuestStatus.Completed;
                playerQuest.CompletedAt = DateTime.UtcNow;
            }

            await _playerQuestRepository.UpdateAsync(playerQuest);

            var message = playerQuest.Status == Quest.QuestStatus.Completed
                ? $"Quest '{playerQuest.Quest?.Title}' completed! Claim your reward now."
                : $"Progress updated: {playerQuest.Progress}/{playerQuest.TargetValue}";

            return new QuestApiResponseDto
            {
                Success = true,
                Message = message
            };
        }

        public async Task<QuestApiResponseDto> ClaimQuestRewardAsync(Guid accountId, ClaimQuestRequestDto request)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(accountId);
            if (profile == null)
            {
                return new QuestApiResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var playerQuest = await _playerQuestRepository.GetByIdWithDetailsAsync(request.PlayerQuestId);
            if (playerQuest == null || playerQuest.PlayerProfileId != profile.Id)
            {
                return new QuestApiResponseDto
                {
                    Success = false,
                    Message = "Player quest not found."
                };
            }

            if (playerQuest.Status != Quest.QuestStatus.Completed)
            {
                return new QuestApiResponseDto
                {
                    Success = false,
                    Message = "Quest is not completed yet."
                };
            }

            if (playerQuest.Status == Quest.QuestStatus.Claimed)
            {
                return new QuestApiResponseDto
                {
                    Success = false,
                    Message = "Quest reward has already been claimed."
                };
            }

            profile.Gold += playerQuest.Quest?.RewardGold ?? 0;
            profile.Gems += playerQuest.Quest?.RewardGems ?? 0;
            await _profileRepository.UpdateAsync(profile);

            if (playerQuest.Quest?.RewardExperience > 0)
            {
                await _playerProfileService.AddExperienceAsync(accountId, playerQuest.Quest.RewardExperience);
            }

            playerQuest.Status = Quest.QuestStatus.Claimed;
            playerQuest.ClaimedAt = DateTime.UtcNow;
            await _playerQuestRepository.UpdateAsync(playerQuest);

            return new QuestApiResponseDto
            {
                Success = true,
                Message = $"Quest reward claimed! +{playerQuest.Quest?.RewardGold} Gold, +{playerQuest.Quest?.RewardGems} Gems, +{playerQuest.Quest?.RewardExperience} EXP"
            };
        }
    }
}
