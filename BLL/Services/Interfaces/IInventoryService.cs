using BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<InventoryResponseDto> GetPlayerInventoryAsync(Guid accountId, int pageNumber = 1, int pageSize = 50);
        Task<InventoryResponseDto> GetEquippedItemsAsync(Guid accountId);
        Task<InventoryApiResponseDto> GetInventoryItemDetailAsync(Guid accountId, Guid inventoryItemId);
        Task<InventoryApiResponseDto> AddItemToInventoryAsync(Guid accountId, AddItemToInventoryRequestDto request);
        Task<InventoryApiResponseDto> RemoveItemFromInventoryAsync(Guid accountId, RemoveItemFromInventoryRequestDto request);
        Task<InventoryApiResponseDto> EquipItemAsync(Guid accountId, EquipItemRequestDto request);
        Task<InventoryApiResponseDto> UnequipItemAsync(Guid accountId, UnequipItemRequestDto request);
        Task<InventoryApiResponseDto> EnhanceItemAsync(Guid accountId, EnhanceItemRequestDto request);
    }
}
