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
        Task<PagedResultDto<ItemResponseDto>> GetItemsPaged(int page, int pageSize, string? search, string? type, string? rarity, bool? isActive);
    }
}
