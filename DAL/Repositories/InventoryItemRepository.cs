using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class InventoryItemRepository : IInventoryItemRepository
    {
        private readonly MysticJourneyDbContext _context;

        public InventoryItemRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<InventoryItem?> GetByIdAsync(int id)
        {
            return await _context.InventoryItems
                .Include(i => i.Item)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<InventoryItem>> GetByPlayerProfileIdAsync(int playerProfileId)
        {
            return await _context.InventoryItems
                .Include(i => i.Item)
                .Where(i => i.PlayerProfileId == playerProfileId)
                .ToListAsync();
        }

        public async Task<InventoryItem?> GetByPlayerAndItemAsync(int playerProfileId, int itemId)
        {
            return await _context.InventoryItems
                .FirstOrDefaultAsync(i => i.PlayerProfileId == playerProfileId && i.ItemId == itemId);
        }

        public async Task AddAsync(InventoryItem item)
        {
            await _context.InventoryItems.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(InventoryItem item)
        {
            _context.InventoryItems.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(InventoryItem item)
        {
            _context.InventoryItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }
}
