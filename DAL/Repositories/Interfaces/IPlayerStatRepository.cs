using DAL.Models;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Quản lý chỉ số người chơi và snapshot.
    // Game APIs: Xem và cập nhật chỉ số người chơi.
    public interface IPlayerStatRepository
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy chỉ số người chơi theo mã hồ sơ, trả về null nếu chưa có.
        Task<PlayerStat?> GetByPlayerProfileId(int playerProfileId);

        // Lấy snapshot chỉ số người chơi theo mã hồ sơ.
        Task<PlayerStatsSnapshot?> GetSnapshotByPlayerProfileId(int playerProfileId);

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // Tạo bản ghi chỉ số người chơi mới.
        Task<PlayerStat> Create(PlayerStat stat);

        // Cập nhật chỉ số người chơi hiện có.
        Task<PlayerStat> Update(PlayerStat stat);

        // Tạo snapshot chỉ số người chơi.
        Task<PlayerStatsSnapshot> CreateSnapshot(PlayerStatsSnapshot snapshot);

        // Cập nhật snapshot chỉ số người chơi.
        Task<PlayerStatsSnapshot> UpdateSnapshot(PlayerStatsSnapshot snapshot);
    }
}
