using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    // Queries the database to retrieve i chest repository records.
    public class ChestRepository : IChestRepository
    {
        private readonly MysticJourneyDbContext _context;

        // Initializes a new instance of ChestRepository with dependencies: context.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public ChestRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        // Performs database query and transactional persistence workflow for create chest.
        // Query details: eagerly loads related entity navigation properties; commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching Chest entity result or default if not found.
        public async Task<Chest> CreateChest(Chest chest)
        {
            _context.Chests.Add(chest);
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return chest;
        }

        // Performs database query and transactional persistence workflow for get chest item by id.
        // Query details: eagerly loads related entity navigation properties; commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching ChestItem? entity result or default if not found.
        public async Task<ChestItem?> GetChestItemById(int chestItemId)
        {
            return await _context.ChestItems
                .Include(ci => ci.Item)  // Eagerly load related navigation entities to avoid N+1 queries
                .FirstOrDefaultAsync(ci => ci.ChestItemId == chestItemId);  // Fetch single matching record or null if not found
        }

        // Persists state modifications to the database for add chest item.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching ChestItem entity result or default if not found.
        public async Task<ChestItem> AddChestItem(ChestItem chestItem)
        {
            _context.ChestItems.Add(chestItem);
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return chestItem;
        }

        // Persists state modifications to the database for update chest item.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching ChestItem entity result or default if not found.
        public async Task<ChestItem> UpdateChestItem(ChestItem chestItem)
        {
            _context.ChestItems.Update(chestItem);
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return chestItem;
        }

        // Persists state modifications to the database for remove chest item.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        public async Task RemoveChestItem(int chestItemId)
        {
            var item = await _context.ChestItems.FindAsync(chestItemId);
            if (item != null)  // Entity exists — proceed with conditional branch
            {
                _context.ChestItems.Remove(item);  // Mark entity for deletion in the next SaveChanges call
                await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            }
        }
    }
}
