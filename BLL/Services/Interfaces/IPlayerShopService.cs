using BLL.DTOs;

namespace BLL.Services.Interfaces
{
    public interface IPlayerShopService
    {
        Task<PagedResultDto<ShopItemPublicResponseDto>> GetShop(int playerProfileId, ViewShopQueryDto query);
        Task<PagedResultDto<ShopItemPublicResponseDto>> GetDailyDeals(int playerProfileId, ViewShopQueryDto query);
        Task<ShopRefreshStatusDto> GetRefreshStatus(int playerProfileId);
        Task<ShopRefreshResponseDto> RefreshShop(int playerProfileId, ViewShopQueryDto query);
        Task<ShopRefreshResponseDto> RefreshDailyDeals(int playerProfileId, ViewShopQueryDto query);
        Task<PurchaseShopItemResponseDto> PurchaseItem(int playerProfileId, PurchaseShopItemRequestDto request);
    }
}