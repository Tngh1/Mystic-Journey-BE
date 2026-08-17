using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Initializes a new default instance of the IItemService class.
    public interface IItemService
    {

        Task<ItemResponseDto?> GetItemById(int id);

        Task<PagedResultDto<ItemResponseDto>> GetItemsPaged(int page, int pageSize, string? search, string? type, string? rarity, bool? isActive, string? sortBy = null, string? sortOrder = null);


        Task<ItemResponseDto> UpdateItem(int id, UpdateItemRequestDto request);
    }
}
