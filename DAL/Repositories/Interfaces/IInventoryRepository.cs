using DAL.Models;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IInventoryRepository
    {
        Task<InventoryItem?> GetById(int id);
        Task<InventoryItem?> GetByPlayerAndItem(int playerProfileId, int itemId);
        Task<InventoryItem> AddItem(InventoryItem item);
        Task<InventoryItem> UpdateItem(InventoryItem item);
        Task DeleteItem(int id);
    }
}
