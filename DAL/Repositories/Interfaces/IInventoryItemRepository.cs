using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IInventoryItemRepository
    {
        Task<InventoryItem?> GetByIdAsync(int id);
        Task<IEnumerable<InventoryItem>> GetByPlayerProfileIdAsync(int playerProfileId);
        Task<InventoryItem?> GetByPlayerAndItemAsync(int playerProfileId, int itemId);
        Task AddAsync(InventoryItem item);
        Task UpdateAsync(InventoryItem item);
        Task DeleteAsync(InventoryItem item);
    }
}
