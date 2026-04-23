using BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IShopService
    {
        Task<ShopListResponseDto> GetAllShopItemsAsync();
        Task<ShopListResponseDto> GetAvailableItemsAsync();
        Task<ShopApiResponseDto> GetShopItemByIdAsync(Guid shopItemId);
        Task<PurchaseApiResponseDto> PurchaseItemAsync(Guid accountId, PurchaseRequestDto request);
        Task<PurchaseHistoryListResponseDto> GetPurchaseHistoryAsync(Guid accountId, int pageNumber = 1, int pageSize = 20);
    }
}
