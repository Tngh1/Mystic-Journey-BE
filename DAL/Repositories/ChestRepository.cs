using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class ChestRepository : IChestRepository
    {
        private readonly MysticJourneyDbContext _context;

        public ChestRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<Chest> CreateChest(Chest chest)
        {
            _context.Chests.Add(chest);
            await _context.SaveChangesAsync();
            return chest;
        }

        public async Task<ChestItem?> GetChestItemById(int chestItemId)
        {
            return await _context.ChestItems
                .Include(ci => ci.Item)
                .FirstOrDefaultAsync(ci => ci.ChestItemId == chestItemId);
        }

        public async Task<ChestItem> AddChestItem(ChestItem chestItem)
        {
            _context.ChestItems.Add(chestItem);
            await _context.SaveChangesAsync();
            return chestItem;
        }

        public async Task<ChestItem> UpdateChestItem(ChestItem chestItem)
        {
            _context.ChestItems.Update(chestItem);
            await _context.SaveChangesAsync();
            return chestItem;
        }

        public async Task RemoveChestItem(int chestItemId)
        {
            var item = await _context.ChestItems.FindAsync(chestItemId);
            if (item != null)
            {
                _context.ChestItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}
