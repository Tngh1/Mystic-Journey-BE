using BLL.DTOs;
using System.Collections.Generic;

namespace BLL.Services.Interfaces
{
    // Quản lý dungeon session (phiên chơi dungeon).
    // Game APIs: Vào dungeon, cập nhật tiến trình, hoàn thành, nhận thưởng.
    public interface IDungeonSessionService
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Vào dungeon. Kiểm tra player và dungeon, tạo phiên chơi mới.
        // Chưa trừ energy - sẽ trừ khi nhận thưởng.
        Task<EnterDungeonResponseDto> EnterDungeon(int playerProfileId, int dungeonConfigId, List<string>? partyMembers = null);

        // Cập nhật tiến trình chiến đấu (quái đã giết, boss, % hoàn thành).
        Task<DungeonProgressResponseDto> UpdateProgress(int sessionId, int playerProfileId, UpdateDungeonProgressRequestDto request);

        // Hoàn thành dungeon. Kiểm tra boss đã bị đánh bại, trả về preview rương.
        // Chưa cấp thưởng - phải gọi ClaimReward sau.
        Task<CompleteDungeonResponseDto> CompleteSession(int sessionId, int playerProfileId);

        // Nhận thưởng dungeon. Kiểm tra session đã hoàn thành và chưa nhận.
        // Trừ energy, tạo thưởng, lưu inventory (transactional - rollback nếu lỗi).
        Task<ClaimDungeonRewardResponseDto> ClaimReward(int sessionId, int playerProfileId);

        // Hủy bỏ dungeon session. Đóng session, không nhận thưởng, không trừ energy.
        Task<bool> AbandonSession(int sessionId, int playerProfileId);

        // Lấy session đang active của người chơi.
        Task<EnterDungeonResponseDto?> GetActiveSession(int playerProfileId);

        // Lấy lịch sử tham gia dungeon (đã hoàn thành hoặc nhận thưởng).
        Task<List<DungeonHistoryResponseDto>> GetHistory(int playerProfileId);
    }
}
