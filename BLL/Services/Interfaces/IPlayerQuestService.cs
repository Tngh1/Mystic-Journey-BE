using BLL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IPlayerQuestService
    {
        /// <summary>UC 25.1 – Lấy toàn bộ PlayerQuest của player đang đăng nhập.</summary>
        Task<List<PlayerQuestResponseDto>> GetMyQuests(int playerProfileId);

        Task<PlayerQuestResponseDto?> GetMyQuestDetail(int playerProfileId, int questId);

        /// <summary>UC 25.3 – Accept quest mới. Tạo PlayerQuest(status=InProgress).</summary>
        Task<PlayerQuestResponseDto> AcceptQuest(int playerProfileId, AcceptQuestRequestDto request);

        /// <summary>
        /// UC 25.4 – Batch cập nhật progress nhiều quest cùng lúc.
        /// Nếu progress >= TargetAmount → tự set status=Completed.
        /// </summary>
        Task<List<PlayerQuestResponseDto>> BatchUpdateProgress(int playerProfileId, BatchProgressRequestDto request);

        Task<PlayerQuestResponseDto> CompleteQuest(int playerProfileId, CompleteQuestRequestDto request);

        /// <summary>UC 25.5 – Nhận thưởng quest đã Completed. Cộng gold/exp/gems vào PlayerProfile.</summary>
        Task<PlayerQuestResponseDto> ClaimReward(int playerProfileId, ClaimQuestRequestDto request);
    }
}
