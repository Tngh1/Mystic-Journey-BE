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
    public class ItemRepository : IItemRepository
    {
        private readonly MysticJourneyDbContext _context;

        public ItemRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<Item?> GetByIdAsync(Guid itemId)
        {
            return await _context.Items
                .FirstOrDefaultAsync(i => i.Id == itemId && i.IsActive);
        }

        public async Task<Item?> GetByIdWithStatsAsync(Guid itemId)
        {
            return await _context.Items
                .Include(i => i.EquipmentStats)
                .FirstOrDefaultAsync(i => i.Id == itemId && i.IsActive);
        }

        public async Task<List<Item>> GetAllAsync(int pageNumber = 1, int pageSize = 20)
        {
            return await _context.Items
                .Where(i => i.IsActive)
                .OrderBy(i => i.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<Item>> GetByTypeAsync(Item.ItemType type, int pageNumber = 1, int pageSize = 20)
        {
            return await _context.Items
                .Where(i => i.Type == type && i.IsActive)
                .OrderBy(i => i.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<Item>> GetByRarityAsync(Item.ItemRarity rarity, int pageNumber = 1, int pageSize = 20)
        {
            return await _context.Items
                .Where(i => i.Rarity == rarity && i.IsActive)
                .OrderBy(i => i.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<Item>> GetByTypeAndRarityAsync(Item.ItemType type, Item.ItemRarity rarity)
        {
            return await _context.Items
                .Include(i => i.EquipmentStats)
                .Where(i => i.Type == type && i.Rarity == rarity && i.IsActive)
                .OrderBy(i => i.Name)
                .ToListAsync();
        }

        public async Task<List<Item>> SearchByNameAsync(string name, int pageNumber = 1, int pageSize = 20)
        {
            return await _context.Items
                .Where(i => i.Name.ToLower().Contains(name.ToLower()) && i.IsActive)
                .OrderBy(i => i.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Item> CreateAsync(Item item)
        {
            await _context.Items.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<Item> UpdateAsync(Item item)
        {
            _context.Items.Update(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<bool> ExistsAsync(Guid itemId)
        {
            return await _context.Items.AnyAsync(i => i.Id == itemId && i.IsActive);
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Items.Where(i => i.IsActive).CountAsync();
        }
    }
}
