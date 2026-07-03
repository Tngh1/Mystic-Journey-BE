using BLL.DTOs;

namespace BLL.Services.Interfaces
{
    public interface IPlayerShopService
    {
        Task<PagedResultDto<ShopItemPublicResponseDto>> GetShop(int playerProfileId, ViewShopQueryDto query);
        Task<PurchaseShopItemResponseDto> PurchaseItem(int playerProfileId, PurchaseShopItemRequestDto request);
    }
}
