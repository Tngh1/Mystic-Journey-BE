using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Quản lý mối quan hệ kết bạn.
    // Game APIs: Xem danh sách bạn bè.
    public interface IFriendRepository
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy danh sách bạn bè của người chơi (đã chấp nhận lời mời).
        Task<List<PlayerProfile>> GetFriends(int playerProfileId);
    }
}
