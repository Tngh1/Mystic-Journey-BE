using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IInventoryItemService
    {
        Task<InventoryApiResponseDto> GetPlayerInventoryAsync(int playerProfileId);
        Task<InventoryApiResponseDto> AddItemToInventoryAsync(AddInventoryItemRequestDto request);
        Task<InventoryApiResponseDto> UpdateInventoryItemAsync(int id, UpdateInventoryItemRequestDto request);
        Task<InventoryApiResponseDto> RemoveItemFromInventoryAsync(int id);
    }
}
