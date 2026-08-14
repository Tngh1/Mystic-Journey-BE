using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Quản lý thế giới game (NPC, Rương, Đăng nhập hàng ngày).
    // Game APIs: Xem NPC, nhận rương, nhận phần thưởng đăng nhập.
    public interface IWorldRepository
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy danh sách NPC trên bản đồ, kèm hội thoại đang hoạt động.
        Task<List<NPC>> GetNpcsByMapName(string mapName, int take);

        // Lấy tất cả tên bản đồ có NPC đang hoạt động.
        Task<List<string>> GetAllNpcMapNames();

        // Lấy thông tin NPC theo mã định danh, kèm hội thoại và nhiệm vụ liên kết.
        Task<NPC?> GetNpcById(int npcId);

        // Kiểm tra nhiệm vụ có được gán cho NPC hay không.
        Task<bool> IsQuestLinkedToNpc(int npcId, int questId);

        // Lấy thông tin rương kho báu theo mã, kèm các vật phẩm bên trong.
        Task<Chest?> GetChestById(int chestId);

        // Lấy rương của người chơi kèm chi tiết rương và vật phẩm.
        Task<PlayerChest?> GetPlayerChest(int playerChestId, int playerProfileId);

        // Tạo rương mới cho người chơi.
        Task<PlayerChest> CreatePlayerChest(PlayerChest playerChest);

        // Cập nhật trạng thái rương của người chơi.
        Task<PlayerChest> UpdatePlayerChest(PlayerChest playerChest);

        // Lấy thông tin đăng nhập hàng ngày của người chơi.
        Task<PlayerDailyLogin?> GetPlayerDailyLogin(int playerProfileId);

        // Tạo bản ghi đăng nhập hàng ngày cho người chơi.
        Task<PlayerDailyLogin> CreatePlayerDailyLogin(PlayerDailyLogin login);

        // Cập nhật thông tin đăng nhập hàng ngày.
        Task<PlayerDailyLogin> UpdatePlayerDailyLogin(PlayerDailyLogin login);

        // Lấy phần thưởng đăng nhập theo số ngày.
        Task<DailyLoginReward?> GetDailyLoginReward(int dayNumber, int month, int year);
    }
}
