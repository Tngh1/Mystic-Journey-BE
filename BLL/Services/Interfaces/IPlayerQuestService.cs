using BLL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Quản lý quests của người chơi (nhiệm vụ đang thực hiện).
    // Cho phép xem, nhận, cập nhật tiến độ, hoàn thành và nhận thưởng quest.
    public interface IPlayerQuestService
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy danh sách quests của player đang đăng nhập.
        Task<List<PlayerQuestResponseDto>> GetMyQuests(int playerProfileId);

        // Lấy chi tiết quest cụ thể của player.
        Task<PlayerQuestResponseDto?> GetMyQuestDetail(int playerProfileId, int questId);

        // Nhận quest mới. Tạo PlayerQuest với status=InProgress.
        Task<PlayerQuestResponseDto> AcceptQuest(int playerProfileId, AcceptQuestRequestDto request);

        // Cập nhật tiến độ nhiều quests cùng lúc.
        // Nếu progress >= TargetAmount thì tự động set status=Completed.
        Task<List<PlayerQuestResponseDto>> BatchUpdateProgress(int playerProfileId, BatchProgressRequestDto request);

        // Hoàn thành quest (sau khi đã đạt đủ điều kiện).
        Task<PlayerQuestResponseDto> CompleteQuest(int playerProfileId, CompleteQuestRequestDto request);

        // Nhận thưởng quest đã Completed. Cộng gold/exp/gems vào PlayerProfile.
        Task<PlayerQuestResponseDto> ClaimReward(int playerProfileId, ClaimQuestRequestDto request);
    }
}
