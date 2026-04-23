using BLL.DTOs;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IQuestService
    {
        Task<QuestListResponseDto> GetAllQuestsAsync();
        Task<QuestListResponseDto> GetQuestsByTypeAsync(Quest.QuestType type);
        Task<QuestListResponseDto> GetAvailableQuestsAsync(Guid accountId);
        Task<QuestApiResponseDto> GetQuestByIdAsync(Guid questId);
        Task<QuestListResponseDto> GetPlayerQuestsAsync(Guid accountId);
        Task<QuestListResponseDto> GetActiveQuestsAsync(Guid accountId);
        Task<QuestListResponseDto> GetCompletedQuestsAsync(Guid accountId);
        Task<QuestApiResponseDto> AcceptQuestAsync(Guid accountId, AcceptQuestRequestDto request);
        Task<QuestApiResponseDto> UpdateProgressAsync(Guid accountId, UpdateQuestProgressRequestDto request);
        Task<QuestApiResponseDto> ClaimQuestRewardAsync(Guid accountId, ClaimQuestRequestDto request);
    }
}
