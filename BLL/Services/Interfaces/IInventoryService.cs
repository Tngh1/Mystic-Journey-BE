using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<InventoryItemResponseDto> AddItemToInventory(int playerProfileId, int itemId, int quantity);
    }
}
