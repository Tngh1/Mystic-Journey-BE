using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Quản lý nhiệm vụ của người chơi.
    // Game APIs: Xem, tiếp nhận, cập nhật tiến độ nhiệm vụ.
    public interface IPlayerQuestRepository
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy tất cả nhiệm vụ của một người chơi.
        Task<List<PlayerQuest>> GetByPlayerId(int playerProfileId);

        // Lấy nhiệm vụ của người chơi theo bản đồ.
        Task<List<PlayerQuest>> GetByPlayerIdAndMap(int playerProfileId, string mapName);

        // Lấy một nhiệm vụ cụ thể của người chơi.
        Task<PlayerQuest?> GetByPlayerAndQuest(int playerProfileId, int questId);

        // Lấy nhiều nhiệm vụ theo danh sách questId (dùng cho batch-progress).
        Task<List<PlayerQuest>> GetByPlayerAndQuestIds(int playerProfileId, List<int> questIds);

        // Tiếp nhận nhiệm vụ mới.
        Task<PlayerQuest> Create(PlayerQuest entity);

        // Cập nhật tiến độ và trạng thái nhiệm vụ.
        Task<PlayerQuest> Update(PlayerQuest entity);

        // Cập nhật nhiều nhiệm vụ cùng lúc (batch save).
        Task UpdateRange(List<PlayerQuest> entities);
    }
}
