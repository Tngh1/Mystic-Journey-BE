using DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Initializes a new default instance of the IShopItemRepository class.
    public interface IShopItemRepository
    {

        Task<ShopItem?> GetShopItemById(int id);

        Task<ShopItem?> GetShopItemByIdWithItem(int id);


        Task<ShopItem> CreateShopItem(ShopItem shopItem);

        Task<ShopItem> UpdateShopItem(ShopItem shopItem);

        Task<(int TotalCount, List<ShopItem> Items)> GetShopItemsPaged(int page, int pageSize, string? search, string? currency, string? shopSection, bool? isActive, string? sortBy = null, string? sortOrder = null);
    }
}
