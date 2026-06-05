using BLL.DTOs;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IItemService
    {
        Task<ItemResponseDto?> GetItemById(int id);
        Task<ItemResponseDto> CreateItem(CreateItemRequestDto request);
        Task<ItemResponseDto> UpdateItem(int id, UpdateItemRequestDto request);
        IQueryable<ItemResponseDto> GetItemsQueryable();
    }
}
