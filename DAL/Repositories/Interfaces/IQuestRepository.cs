using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Quản lý nhiệm vụ (quest).
    // Game APIs: Xem nhiệm vụ, tiến độ nhiệm vụ của người chơi.
    // Admin APIs: Tạo, cập nhật nhiệm vụ.
    public interface IQuestRepository
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy nhiệm vụ theo mã định danh.
        Task<Quest?> GetQuestById(int id);

        // Lấy nhiệm vụ kèm phần thưởng (item và skill).
        Task<Quest?> GetByIdWithReward(int id);

        // Lấy tất cả nhiệm vụ đang hoạt động.
        Task<List<Quest>> GetActiveQuests();

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // Cập nhật thông tin nhiệm vụ.
        Task<Quest> AddQuest(Quest quest);

        Task<Quest> UpdateQuest(Quest quest);

        Task<NPCDialogue?> GetQuestDialogueByQuestId(int questId);

        Task<NPC?> GetNpcByNameAndMap(string? npcName, string mapName);

        Task<List<NPC>> GetQuestNpcOptions(string? mapName);

        void AddQuestDialogue(NPCDialogue dialogue);

        // Lấy danh sách nhiệm vụ có phân trang, lọc theo tìm kiếm, loại, trạng thái và bản đồ.
        Task<(int TotalCount, List<Quest> Items)> GetQuestsPaged(int page, int pageSize, string? search, string? type, bool? isActive, string? mapName, string? sortBy = null, string? sortOrder = null);
    }
}
