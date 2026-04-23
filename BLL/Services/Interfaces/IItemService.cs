using BLL.DTOs;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IItemService
    {
        Task<ItemListResponseDto> GetAllItemsAsync(int pageNumber = 1, int pageSize = 20);
        Task<ItemListResponseDto> GetItemsByTypeAsync(Item.ItemType type, int pageNumber = 1, int pageSize = 20);
        Task<ItemListResponseDto> GetItemsByRarityAsync(Item.ItemRarity rarity, int pageNumber = 1, int pageSize = 20);
        Task<ItemListResponseDto> SearchItemsAsync(string name, int pageNumber = 1, int pageSize = 20);
        Task<ItemApiResponseDto> GetItemByIdAsync(Guid itemId);
        Task<ItemApiResponseDto> GetItemDetailAsync(Guid itemId);
        Task<ItemApiResponseDto> CreateItemAsync(CreateItemRequestDto request);
        Task<ItemApiResponseDto> UpdateItemAsync(Guid itemId, UpdateItemRequestDto request);
        Task<ItemApiResponseDto> DeleteItemAsync(Guid itemId);
    }
}
