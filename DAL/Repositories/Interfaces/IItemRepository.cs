using DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Initializes a new default instance of the IItemRepository class.
    public interface IItemRepository
    {

        Task<List<Item>> GetQuestItems();

        Task<Item?> GetQuestItemByNames(params string[] names);


        Task<Item?> GetItemById(int id);

        Task<Item?> GetItemByIdWithStats(int id);

        Task<Item> UpdateItem(Item item);

        Task<(int TotalCount, List<Item> Items)> GetItemsPaged(int page, int pageSize, string? search, string? type, string? rarity, bool? isActive, string? sortBy = null, string? sortOrder = null);

        Task<int> GetTotalItemsCount();
    }
}
