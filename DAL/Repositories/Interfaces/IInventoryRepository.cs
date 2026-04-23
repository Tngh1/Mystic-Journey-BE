using DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IInventoryRepository
    {
        Task<InventoryItem?> GetByIdAsync(Guid inventoryItemId);
        Task<InventoryItem?> GetByIdWithDetailsAsync(Guid inventoryItemId);
        Task<List<InventoryItem>> GetByPlayerProfileIdAsync(Guid playerProfileId, int pageNumber = 1, int pageSize = 50);
        Task<List<InventoryItem>> GetEquippedItemsAsync(Guid playerProfileId);
        Task<List<InventoryItem>> GetByItemIdAsync(Guid playerProfileId, Guid itemId);
        Task<InventoryItem?> FindStackableItemAsync(Guid playerProfileId, Guid itemId);
        Task<InventoryItem> CreateAsync(InventoryItem item);
        Task<InventoryItem> UpdateAsync(InventoryItem item);
        Task DeleteAsync(InventoryItem item);
        Task<int> GetTotalCountAsync(Guid playerProfileId);
        Task<InventoryItem> UnequipAllBySlotAsync(Guid playerProfileId, Item.EquipmentSlot slot);
    }
}
