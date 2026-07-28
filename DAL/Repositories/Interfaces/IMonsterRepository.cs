using DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Quản lý quái vật và vật phẩm rơi.
    // Game APIs: Xem quái vật, khám phá quái vật.
    // Admin APIs: Tạo, cập nhật quái vật và vật phẩm rơi.
    public interface IMonsterRepository
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy thông tin quái vật theo mã định danh.
        Task<Monster?> GetMonsterById(int id);

        // Lấy quái vật kèm danh sách vật phẩm rơi.
        Task<Monster?> GetMonsterByIdWithDrops(int id);

        // Lấy thông tin khám phá quái vật của người chơi.
        Task<PlayerMonsterDiscovery?> GetPlayerDiscovery(int playerProfileId, int monsterId);

        // Lấy dictionary khám phá quái vật của người chơi (theo monsterId).
        Task<Dictionary<int, PlayerMonsterDiscovery>> GetPlayerDiscoveriesDict(int playerProfileId);

        // Lấy danh sách mã quái vật boss thuộc nhiệm vụ đã hoàn thành.
        Task<HashSet<int>> GetCompletedQuestBossMonsterIds(int playerProfileId);

        // Lấy danh sách mã quái vật đã được khám phá.
        Task<HashSet<int>> GetDiscoveredMonsterIds(int playerProfileId);

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // Cập nhật thông tin quái vật.
        Task<Monster> UpdateMonster(Monster monster);

        // Thêm vật phẩm rơi cho quái vật.
        Task<MonsterDrop> CreateDrop(MonsterDrop drop);

        // Tạo hoặc cập nhật trạng thái khám phá quái vật của người chơi.
        Task<PlayerMonsterDiscovery> CreateOrUpdatePlayerDiscovery(PlayerMonsterDiscovery discovery);

        // Lấy các điểm spawn của quái vật theo mã.
        Task<List<MonsterSpawn>> GetSpawnsByMonsterId(int monsterId);

        // Lấy các điểm spawn đang hoạt động trên bản đồ hoặc trong dungeon.
        Task<List<MonsterSpawn>> GetActiveSpawns(string mapName, string? regionName, int? dungeonId);

        // Tạo điểm spawn mới cho quái vật.
        Task<MonsterSpawn> CreateSpawn(MonsterSpawn spawn);

        // Lấy tất cả vật phẩm rơi của quái vật (kể cả không hoạt động).
        Task<List<MonsterDrop>> GetDropsByMonsterId(int monsterId);

        // Lấy danh sách quái vật có phân trang, lọc theo tìm kiếm, loại và trạng thái.
        Task<(int TotalCount, List<Monster> Items)> GetMonstersPaged(int page, int pageSize, string? search, string? type, bool? isActive, string? sortBy = null, string? sortOrder = null);

        // Lấy danh sách vật phẩm rơi có phân trang.
        Task<(int TotalCount, List<MonsterDrop> Items)> GetMonsterDropsPaged(int page, int pageSize);

        // Đếm tổng số quái vật trong hệ thống.
        Task<int> GetTotalMonstersCount();
    }
}
