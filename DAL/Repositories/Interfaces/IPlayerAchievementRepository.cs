using DAL.Models;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Quản lý thành tích người chơi.
    // Game APIs: Xem danh sách thành tích đã đạt được.
    public interface IPlayerAchievementRepository
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy danh sách thành tích đã đạt được của người chơi.
        Task<List<PlayerAchievement>> GetByPlayerProfileId(int playerProfileId);
    }
}
