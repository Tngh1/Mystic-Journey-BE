using DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IItemRepository
    {
        Task<Item?> GetByIdAsync(Guid itemId);
        Task<Item?> GetByIdWithStatsAsync(Guid itemId);
        Task<List<Item>> GetAllAsync(int pageNumber = 1, int pageSize = 20);
        Task<List<Item>> GetByTypeAsync(Item.ItemType type, int pageNumber = 1, int pageSize = 20);
        Task<List<Item>> GetByRarityAsync(Item.ItemRarity rarity, int pageNumber = 1, int pageSize = 20);
        Task<List<Item>> GetByTypeAndRarityAsync(Item.ItemType type, Item.ItemRarity rarity);
        Task<List<Item>> SearchByNameAsync(string name, int pageNumber = 1, int pageSize = 20);
        Task<Item> CreateAsync(Item item);
        Task<Item> UpdateAsync(Item item);
        Task<bool> ExistsAsync(Guid itemId);
        Task<int> GetTotalCountAsync();
    }
}
