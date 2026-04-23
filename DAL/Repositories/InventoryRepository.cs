using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly MysticJourneyDbContext _context;

        public InventoryRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<InventoryItem?> GetByIdAsync(Guid inventoryItemId)
        {
            return await _context.InventoryItems
                .FirstOrDefaultAsync(i => i.Id == inventoryItemId);
        }

        public async Task<InventoryItem?> GetByIdWithDetailsAsync(Guid inventoryItemId)
        {
            return await _context.InventoryItems
                .Include(i => i.Item)
                    .ThenInclude(item => item!.EquipmentStats)
                .Include(i => i.PlayerProfile)
                .FirstOrDefaultAsync(i => i.Id == inventoryItemId);
        }

        public async Task<List<InventoryItem>> GetByPlayerProfileIdAsync(Guid playerProfileId, int pageNumber = 1, int pageSize = 50)
        {
            return await _context.InventoryItems
                .Include(i => i.Item)
                .Where(i => i.PlayerProfileId == playerProfileId && i.Quantity > 0)
                .OrderByDescending(i => i.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<InventoryItem>> GetEquippedItemsAsync(Guid playerProfileId)
        {
            return await _context.InventoryItems
                .Include(i => i.Item)
                    .ThenInclude(item => item!.EquipmentStats)
                .Where(i => i.PlayerProfileId == playerProfileId && i.IsEquipped && i.Quantity > 0)
                .ToListAsync();
        }

        public async Task<List<InventoryItem>> GetByItemIdAsync(Guid playerProfileId, Guid itemId)
        {
            return await _context.InventoryItems
                .Include(i => i.Item)
                .Where(i => i.PlayerProfileId == playerProfileId && i.ItemId == itemId && i.Quantity > 0)
                .ToListAsync();
        }

        public async Task<InventoryItem?> FindStackableItemAsync(Guid playerProfileId, Guid itemId)
        {
            return await _context.InventoryItems
                .FirstOrDefaultAsync(i =>
                    i.PlayerProfileId == playerProfileId &&
                    i.ItemId == itemId &&
                    !i.IsEquipped &&
                    i.Quantity < i.Item!.MaxStack);
        }

        public async Task<InventoryItem> CreateAsync(InventoryItem item)
        {
            await _context.InventoryItems.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<InventoryItem> UpdateAsync(InventoryItem item)
        {
            _context.InventoryItems.Update(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task DeleteAsync(InventoryItem item)
        {
            _context.InventoryItems.Remove(item);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetTotalCountAsync(Guid playerProfileId)
        {
            return await _context.InventoryItems
                .Where(i => i.PlayerProfileId == playerProfileId && i.Quantity > 0)
                .CountAsync();
        }

        public async Task<InventoryItem> UnequipAllBySlotAsync(Guid playerProfileId, Item.EquipmentSlot slot)
        {
            var equippedItems = await _context.InventoryItems
                .Include(i => i.Item)
                .Where(i => i.PlayerProfileId == playerProfileId && i.IsEquipped && i.Item!.Slot == slot)
                .ToListAsync();

            foreach (var item in equippedItems)
            {
                item.IsEquipped = false;
            }

            await _context.SaveChangesAsync();
            return equippedItems.FirstOrDefault()!;
        }
    }
}
