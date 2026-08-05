using BLL.DTOs;

namespace BLL.Services.Interfaces
{
    // Quản lý world (thế giới game) của người chơi.
    // Cho phép xem trạng thái world, tương tác với NPC, rương, quest, và nhận thưởng đăng nhập hàng ngày.
    public interface IWorldService
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy trạng thái world của player (vị trí, quest đang thực hiện, NPCs...).
        Task<WorldStateResponseDto> GetWorldState(int playerProfileId);

        // Cập nhật vị trí của player trong world (map, tọa độ).
        Task<PlayerWorldPositionDto> UpdatePosition(int playerProfileId, UpdateWorldPositionRequestDto request);

        // Nói chuyện với NPC, nhận dialogue và quest.
        Task<TalkToNpcResponseDto> TalkToNpc(int playerProfileId, TalkToNpcRequestDto request);

        // Nộp item quest cho NPC.
        Task<TurnInQuestItemResponseDto> TurnInQuestItem(int playerProfileId, TurnInQuestItemRequestDto request);

        // Mở rương trong world.
        Task<OpenChestResponseDto> OpenChest(int playerProfileId, OpenWorldChestRequestDto request);

        // Tương tác với object trong world (lever, button, v.v.).
        Task<InteractObjectResponseDto> InteractWithObject(int playerProfileId, InteractObjectRequestDto request);

        // Lấy trạng thái đăng nhập hàng ngày của player.
        Task<PlayerDailyLoginResponseDto?> GetDailyLoginStatus(int playerProfileId);

        // Nhận thưởng đăng nhập hàng ngày.
        Task<ClaimDailyRewardResponseDto> ClaimDailyLoginReward(int playerProfileId);

        // Nhận thưởng bù ngày trước (retroactive claim).
        Task<ClaimDailyRewardResponseDto> RetroactiveClaimDailyLoginReward(int playerProfileId, int dayToClaim);

        // Nhặt vật phẩm rơi ra map thế giới (World Drop Pickup)
        Task<ClaimDropResponseDto> ClaimDrop(int playerProfileId, ClaimDropRequestDto request);
    }
}
