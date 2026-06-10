using DAL.Models;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IInventoryRepository
    {
        Task<InventoryItem?> GetById(int id);
        Task<InventoryItem?> GetByPlayerAndItem(int playerProfileId, int itemId);
        Task<List<InventoryItem>> GetByPlayerId(int playerProfileId);
        Task<InventoryItem> AddItem(InventoryItem item);
        Task<InventoryItem> UpdateItem(InventoryItem item);
        Task DeleteItem(int id);
        // PlayerSkin helpers
        Task<PlayerSkin?> GetPlayerSkinById(int id);
        Task<List<PlayerSkin>> GetPlayerSkinsByPlayerId(int playerProfileId);
        Task<PlayerSkin> UpdatePlayerSkin(PlayerSkin skin);
    }
}
