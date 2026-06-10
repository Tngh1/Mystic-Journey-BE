using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
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

        public async Task<InventoryItem?> GetById(int id)
        {
            return await _context.InventoryItems
                .Include(i => i.Item)
                .Include(i => i.PlayerProfile)
                .FirstOrDefaultAsync(i => i.InventoryItemId == id);
        }

        public async Task<InventoryItem?> GetByPlayerAndItem(int playerProfileId, int itemId)
        {
            return await _context.InventoryItems
                .Include(i => i.Item)
                .FirstOrDefaultAsync(i => i.PlayerProfileId == playerProfileId && i.ItemId == itemId);
        }

        public async Task<List<InventoryItem>> GetByPlayerId(int playerProfileId)
        {
            return await _context.InventoryItems
                .Include(i => i.Item)
                .Where(i => i.PlayerProfileId == playerProfileId)
                .ToListAsync();
        }

        public async Task<InventoryItem> AddItem(InventoryItem item)
        {
            item.CreatedAt = DateTime.UtcNow;
            await _context.InventoryItems.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<InventoryItem> UpdateItem(InventoryItem item)
        {
            _context.InventoryItems.Update(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task DeleteItem(int id)
        {
            var item = await _context.InventoryItems.FindAsync(id);
            if (item != null)
            {
                _context.InventoryItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<PlayerSkin?> GetPlayerSkinById(int id)
        {
            return await _context.PlayerSkins
                .Include(ps => ps.Skin)
                .FirstOrDefaultAsync(ps => ps.PlayerSkinId == id);
        }

        public async Task<List<PlayerSkin>> GetPlayerSkinsByPlayerId(int playerProfileId)
        {
            return await _context.PlayerSkins
                .Include(ps => ps.Skin)
                .Where(ps => ps.PlayerProfileId == playerProfileId)
                .ToListAsync();
        }

        public async Task<PlayerSkin> UpdatePlayerSkin(PlayerSkin skin)
        {
            _context.PlayerSkins.Update(skin);
            await _context.SaveChangesAsync();
            return skin;
        }
    }
}
