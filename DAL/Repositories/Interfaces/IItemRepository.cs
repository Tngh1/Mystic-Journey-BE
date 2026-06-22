using DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IItemRepository
    {
        Task<Item?> GetItemById(int id);
        Task<Item?> GetItemByIdWithStats(int id);
        Task<Item> CreateItem(Item item);
        Task<Item> UpdateItem(Item item);
        Task<(int TotalCount, List<Item> Items)> GetItemsPaged(int page, int pageSize, string? search, string? type, string? rarity, bool? isActive);
    }
}
