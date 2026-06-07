using BLL.DTOs;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IShopItemService
    {
        Task<ShopItemResponseDto?> GetShopItemById(int id);
        Task<ShopItemResponseDto> CreateShopItem(CreateShopItemRequestDto request);
        Task<ShopItemResponseDto> UpdateShopItem(int id, UpdateShopItemRequestDto request);
        Task<PagedResultDto<ShopItemResponseDto>> GetShopItemsPaged(int page, int pageSize, string? search, string? currency, bool? isActive);
    }
}
