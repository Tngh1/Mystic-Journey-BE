using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Quản lý túi đồ và skin người chơi.
    // Game APIs: Xem túi đồ, quản lý skin.
    // Admin APIs: Quản lý túi đồ người chơi.
    public interface IInventoryRepository
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy vật phẩm trong túi đồ theo mã.
        Task<InventoryItem?> GetById(int id);

        // Lấy vật phẩm trong túi đồ của người chơi theo mã vật phẩm.
        Task<InventoryItem?> GetByPlayerAndItem(int playerProfileId, int itemId);

        // Lấy toàn bộ túi đồ của người chơi.
        Task<List<InventoryItem>> GetByPlayerId(int playerProfileId);

        // Lấy skin của người chơi theo mã.
        Task<PlayerSkin?> GetPlayerSkinById(int id);

        // Lấy tất cả skin của một người chơi.
        Task<List<PlayerSkin>> GetPlayerSkinsByPlayerId(int playerProfileId);

        // Lấy tất cả skin đang hoạt động.
        Task<List<Skin>> GetAllActiveSkins();

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // Thêm vật phẩm vào túi đồ.
        Task<InventoryItem> AddItem(InventoryItem item);

        // Cập nhật vật phẩm trong túi đồ.
        Task<InventoryItem> UpdateItem(InventoryItem item);

        // Xóa vật phẩm khỏi túi đồ.
        Task DeleteItem(int id);

        // Cập nhật skin của người chơi.
        Task<PlayerSkin> UpdatePlayerSkin(PlayerSkin skin);
    }
}
