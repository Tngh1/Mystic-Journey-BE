using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IItemService
    {
        Task<ItemApiResponseDto> GetAllItemsAsync();
        Task<ItemApiResponseDto> GetItemByIdAsync(int id);
        Task<ItemApiResponseDto> CreateItemAsync(CreateItemRequestDto request);
        Task<ItemApiResponseDto> UpdateItemAsync(int id, UpdateItemRequestDto request);
        Task<ItemApiResponseDto> DeleteItemAsync(int id);
    }
}
