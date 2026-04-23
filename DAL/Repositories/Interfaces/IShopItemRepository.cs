using DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IShopItemRepository
    {
        Task<ShopItem?> GetByIdAsync(Guid shopItemId);
        Task<ShopItem?> GetByIdWithItemAsync(Guid shopItemId);
        Task<List<ShopItem>> GetAllActiveAsync();
        Task<List<ShopItem>> GetAvailableNowAsync();
        Task<ShopItem> CreateAsync(ShopItem shopItem);
        Task<ShopItem> UpdateAsync(ShopItem shopItem);
    }
}
