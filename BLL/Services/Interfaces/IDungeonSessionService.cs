using BLL.DTOs;
using System.Collections.Generic;

namespace BLL.Services.Interfaces
{
    // Initializes a new default instance of the IDungeonSessionService class.
    public interface IDungeonSessionService
    {

        Task<EnterDungeonResponseDto> EnterDungeon(int playerProfileId, int dungeonConfigId, List<string>? partyMembers = null);

        Task<DungeonProgressResponseDto> UpdateProgress(int sessionId, int playerProfileId, UpdateDungeonProgressRequestDto request);

        Task<CompleteDungeonResponseDto> CompleteSession(int sessionId, int playerProfileId);

        Task<ClaimDungeonRewardResponseDto> ClaimReward(int sessionId, int playerProfileId);

        Task<bool> AbandonSession(int sessionId, int playerProfileId);

        Task<EnterDungeonResponseDto?> GetActiveSession(int playerProfileId);

        Task<List<DungeonHistoryResponseDto>> GetHistory(int playerProfileId);
    }
}
