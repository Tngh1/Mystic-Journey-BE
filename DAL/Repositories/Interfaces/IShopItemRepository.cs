using DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IShopItemRepository
    {
        Task<ShopItem?> GetShopItemById(int id);
        Task<ShopItem?> GetShopItemByIdWithItem(int id);
        Task<List<ShopItem>> GetAllShopItems();
        Task<List<ShopItem>> GetActiveShopItems();
        Task<ShopItem> CreateShopItem(ShopItem shopItem);
        Task<ShopItem> UpdateShopItem(ShopItem shopItem);
        Task DeleteShopItem(int id);
        IQueryable<ShopItem> GetShopItemsQueryable();
    }
}
