using DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Quản lý hồ sơ người chơi.
    // Game APIs: Xem và cập nhật hồ sơ người chơi.
    // Admin APIs: Quản lý hồ sơ người chơi.
    public interface IPlayerProfileRepository
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy hồ sơ người chơi theo mã định danh.
        Task<PlayerProfile?> GetPlayerProfileById(int id);

        // Lấy hồ sơ người chơi kèm chỉ số (stats).
        Task<PlayerProfile?> GetPlayerProfileByIdWithStats(int id);

        // Lấy hồ sơ đầy đủ kèm stats và tài khoản.
        Task<PlayerProfile?> GetByIdFull(int id);

        // Lấy hồ sơ người chơi theo mã tài khoản.
        Task<PlayerProfile?> GetByAccountId(int accountId);

        // Lấy snapshot chỉ số của người chơi.
        Task<PlayerStatsSnapshot?> GetSnapshotByPlayerProfileId(int playerProfileId);

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy tất cả hồ sơ người chơi.
        Task<List<PlayerProfile>> GetAllPlayerProfiles();

        // Tạo hồ sơ người chơi mới.
        Task<PlayerProfile> CreatePlayerProfile(PlayerProfile profile);

        // Cập nhật thông tin hồ sơ người chơi.
        Task<PlayerProfile> UpdatePlayerProfile(PlayerProfile profile);

        // Tìm kiếm hồ sơ theo từ khóa và/hoặc lớp nhân vật.
        Task<List<PlayerProfile>> Search(string? keyword = null, string? playerClass = null);

        // Đếm tổng số hồ sơ người chơi.
        Task<int> GetTotalPlayerProfilesCount();

        // Lấy danh sách hồ sơ có phân trang, lọc theo tìm kiếm và cấp độ.
        Task<(int TotalCount, List<PlayerProfile> Items)> GetProfilesPaged(int page, int pageSize, string? search, int? level);
    }
}
