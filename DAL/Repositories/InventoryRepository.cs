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
    // Queries the database to retrieve i inventory repository records.
    public class InventoryRepository : IInventoryRepository
    {
        private readonly MysticJourneyDbContext _context;

        // Initializes a new instance of InventoryRepository with dependencies: context.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public InventoryRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }


        // Queries the database to retrieve get by id records.
        // Query details: eagerly loads related entity navigation properties.
        // Returns the matching InventoryItem? entity result or default if not found.
        public async Task<InventoryItem?> GetById(int id)
        {
            return await _context.InventoryItems
                .Include(i => i.Item)  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(it => it!.EquipmentStats)
                .Include(i => i.PlayerProfile)  // Eagerly load related navigation entities to avoid N+1 queries
                .FirstOrDefaultAsync(i => i.InventoryItemId == id);  // Fetch single matching record or null if not found
        }

        // Queries the database to retrieve get by player and item records.
        // Query details: eagerly loads related entity navigation properties.
        // Returns the matching InventoryItem? entity result or default if not found.
        public async Task<InventoryItem?> GetByPlayerAndItem(int playerProfileId, int itemId)
        {
            return await _context.InventoryItems
                .Include(i => i.Item)  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(it => it!.EquipmentStats)
                .FirstOrDefaultAsync(i => i.PlayerProfileId == playerProfileId && i.ItemId == itemId);  // Fetch single matching record or null if not found
        }

        // Performs database query and transactional persistence workflow for get by player id.
        // Query details: eagerly loads related entity navigation properties; commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching List<InventoryItem entity result or default if not found.
        public async Task<List<InventoryItem>> GetByPlayerId(int playerProfileId)
        {
            return await _context.InventoryItems
                .Include(i => i.Item)  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(it => it!.EquipmentStats)
                .Where(i => i.PlayerProfileId == playerProfileId)  // Filter records matching the predicate
                .ToListAsync();  // Materialize the query into a list from the database
        }

        // Persists state modifications to the database for add item.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching InventoryItem entity result or default if not found.
        public async Task<InventoryItem> AddItem(InventoryItem item)
        {
            item.CreatedAt = DateTime.UtcNow;
            await _context.InventoryItems.AddAsync(item);  // Stage new entity for insertion in the next SaveChanges call
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return item;
        }

        // Persists state modifications to the database for update item.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching InventoryItem entity result or default if not found.
        public async Task<InventoryItem> UpdateItem(InventoryItem item)
        {
            _context.InventoryItems.Update(item);
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return item;
        }

        // Persists state modifications to the database for delete item.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        public async Task DeleteItem(int id)
        {
            var item = await _context.InventoryItems.FindAsync(id);
            if (item != null)  // Entity exists — proceed with conditional branch
            {
                _context.InventoryItems.Remove(item);  // Mark entity for deletion in the next SaveChanges call
                await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            }
        }


        // Queries the database to retrieve get player skin by id records.
        // Query details: eagerly loads related entity navigation properties.
        // Returns the matching PlayerSkin? entity result or default if not found.
        public async Task<PlayerSkin?> GetPlayerSkinById(int id)
        {
            return await _context.PlayerSkins
                .Include(ps => ps.Skin)  // Eagerly load related navigation entities to avoid N+1 queries
                .FirstOrDefaultAsync(ps => ps.PlayerSkinId == id);  // Fetch single matching record or null if not found
        }

        // Performs database query and transactional persistence workflow for get player skins by player id.
        // Query details: eagerly loads related entity navigation properties; commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching List<PlayerSkin entity result or default if not found.
        public async Task<List<PlayerSkin>> GetPlayerSkinsByPlayerId(int playerProfileId)
        {
            return await _context.PlayerSkins
                .Include(ps => ps.Skin)  // Eagerly load related navigation entities to avoid N+1 queries
                .Where(ps => ps.PlayerProfileId == playerProfileId)  // Filter records matching the predicate
                .ToListAsync();  // Materialize the query into a list from the database
        }

        // Performs database query and transactional persistence workflow for update player skin.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching PlayerSkin entity result or default if not found.
        public async Task<PlayerSkin> UpdatePlayerSkin(PlayerSkin skin)
        {
            _context.PlayerSkins.Update(skin);
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return skin;
        }

        // Load all active skins; it filters the eligible records and materializes the query results.
        public async Task<List<Skin>> GetAllActiveSkins()
        {
            return await _context.Skins.Where(s => s.IsActive).ToListAsync();  // Materialize the query into a list from the database
        }
    }
}
