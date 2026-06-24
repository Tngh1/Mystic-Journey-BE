using BLL.DTOs;

namespace BLL.Services.Interfaces
{
    public interface IWorldService
    {
        Task<WorldStateResponseDto> GetWorldState(int playerProfileId);
        Task<PlayerWorldPositionDto> UpdatePosition(int playerProfileId, UpdateWorldPositionRequestDto request);
        Task<TalkToNpcResponseDto> TalkToNpc(int playerProfileId, TalkToNpcRequestDto request);
        Task<OpenChestResponseDto> OpenChest(int playerProfileId, OpenWorldChestRequestDto request);
        Task<InteractObjectResponseDto> InteractWithObject(int playerProfileId, InteractObjectRequestDto request);
        Task<TurnInQuestItemResponseDto> TurnInQuestItem(int playerProfileId, TurnInQuestItemRequestDto request);
        Task<PlayerDailyLoginResponseDto?> GetDailyLoginStatus(int playerProfileId);
        Task<ClaimDailyRewardResponseDto> ClaimDailyLoginReward(int playerProfileId);
        Task<ClaimDailyRewardResponseDto> RetroactiveClaimDailyLoginReward(int playerProfileId, int dayToClaim);
    }
}
