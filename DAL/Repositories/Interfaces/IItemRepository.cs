using DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IItemRepository
    {
        Task<Item?> GetItemById(int id);
        Task<Item?> GetItemByIdWithStats(int id);
        Task<List<Item>> GetAllItems();
        Task<List<Item>> GetActiveItems();
        Task<Item> CreateItem(Item item);
        Task<Item> UpdateItem(Item item);
        Task DeleteItem(int id);
        IQueryable<Item> GetItemsQueryable();
    }
}
