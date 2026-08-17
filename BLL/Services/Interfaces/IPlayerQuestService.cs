using BLL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Initializes a new default instance of the IPlayerQuestService class.
    public interface IPlayerQuestService
    {

        Task<List<PlayerQuestResponseDto>> GetMyQuests(int playerProfileId);

        Task<PlayerQuestResponseDto?> GetMyQuestDetail(int playerProfileId, int questId);

        Task<PlayerQuestResponseDto> AcceptQuest(int playerProfileId, AcceptQuestRequestDto request);

        Task<List<PlayerQuestResponseDto>> BatchUpdateProgress(int playerProfileId, BatchProgressRequestDto request);

        Task<PlayerQuestResponseDto> CompleteQuest(int playerProfileId, CompleteQuestRequestDto request);

        Task<PlayerQuestResponseDto> ClaimReward(int playerProfileId, ClaimQuestRequestDto request);
    }
}
