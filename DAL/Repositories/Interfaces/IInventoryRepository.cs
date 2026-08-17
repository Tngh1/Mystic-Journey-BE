using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Initializes a new default instance of the IInventoryRepository class.
    public interface IInventoryRepository
    {

        Task<InventoryItem?> GetById(int id);

        Task<InventoryItem?> GetByPlayerAndItem(int playerProfileId, int itemId);

        Task<List<InventoryItem>> GetByPlayerId(int playerProfileId);

        Task<PlayerSkin?> GetPlayerSkinById(int id);

        Task<List<PlayerSkin>> GetPlayerSkinsByPlayerId(int playerProfileId);

        Task<List<Skin>> GetAllActiveSkins();


        Task<InventoryItem> AddItem(InventoryItem item);

        Task<InventoryItem> UpdateItem(InventoryItem item);

        Task DeleteItem(int id);

        Task<PlayerSkin> UpdatePlayerSkin(PlayerSkin skin);
    }
}
