using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Initializes a new default instance of the IShopItemService class.
    public interface IShopItemService
    {

        Task<ShopItemResponseDto?> GetShopItemById(int id);

        Task<PagedResultDto<ShopItemResponseDto>> GetShopItemsPaged(int page, int pageSize, string? search, string? currency, string? shopSection, bool? isActive, string? sortBy = null, string? sortOrder = null);


        Task<ShopItemResponseDto> CreateShopItem(CreateShopItemRequestDto request);

        Task<ShopItemResponseDto> UpdateShopItem(int id, UpdateShopItemRequestDto request);
    }
}
