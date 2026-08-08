using DAL.Models;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IChestRepository
    {
        Task<Chest> CreateChest(Chest chest);
        Task<ChestItem?> GetChestItemById(int chestItemId);
        Task<ChestItem> AddChestItem(ChestItem chestItem);
        Task<ChestItem> UpdateChestItem(ChestItem chestItem);
        Task RemoveChestItem(int chestItemId);
    }
}
