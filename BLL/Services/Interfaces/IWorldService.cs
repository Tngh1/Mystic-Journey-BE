using BLL.DTOs;

namespace BLL.Services.Interfaces
{
    // Initializes a new default instance of the IWorldService class.
    public interface IWorldService
    {

        Task<WorldStateResponseDto> GetWorldState(int playerProfileId);

        Task<PlayerWorldPositionDto> GetPosition(int playerProfileId);

        Task<PlayerWorldPositionDto> UpdatePosition(int playerProfileId, UpdateWorldPositionRequestDto request);

        Task<TalkToNpcResponseDto> TalkToNpc(int playerProfileId, TalkToNpcRequestDto request);

        Task<TurnInQuestItemResponseDto> TurnInQuestItem(int playerProfileId, TurnInQuestItemRequestDto request);

        Task<OpenChestResponseDto> OpenChest(int playerProfileId, OpenWorldChestRequestDto request);

        Task<InteractObjectResponseDto> InteractWithObject(int playerProfileId, InteractObjectRequestDto request);

        Task<PlayerDailyLoginResponseDto?> GetDailyLoginStatus(int playerProfileId);

        Task<ClaimDailyRewardResponseDto> ClaimDailyLoginReward(int playerProfileId);

        Task<ClaimDailyRewardResponseDto> RetroactiveClaimDailyLoginReward(int playerProfileId, int dayToClaim);
    }
}
