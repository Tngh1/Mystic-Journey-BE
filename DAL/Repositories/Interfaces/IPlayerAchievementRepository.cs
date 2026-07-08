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

        // Lấy một thành tích người chơi theo ID kèm dữ liệu achievement.
        Task<PlayerAchievement?> GetByIdWithAchievement(int playerAchievementId);

        // Cập nhật một thành tích người chơi.
        Task<PlayerAchievement> Update(PlayerAchievement playerAchievement);
    }
}
