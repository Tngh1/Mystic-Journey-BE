using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class InventoryItemService : IInventoryItemService
    {
        private readonly IInventoryItemRepository _repository;

        public InventoryItemService(IInventoryItemRepository repository)
        {
            _repository = repository;
        }

        public async Task<InventoryApiResponseDto> GetPlayerInventoryAsync(int playerProfileId)
        {
            var items = await _repository.GetByPlayerProfileIdAsync(playerProfileId);
            
            var dtoList = items.Select(i => new InventoryItemResponseDto
            {
                Id = i.Id,
                PlayerProfileId = i.PlayerProfileId,
                ItemId = i.ItemId,
                ItemName = i.Item?.Name ?? string.Empty,
                Quantity = i.Quantity,
                IsEquipped = i.IsEquipped,
                IsSkin = i.IsSkin,
                EquippedSlot = i.EquippedSlot,
                EnhancementLevel = i.EnhancementLevel,
                CreatedAt = i.CreatedAt
            }).ToList();

            return new InventoryApiResponseDto
            {
                Success = true,
                Message = "Inventory retrieved successfully.",
                Data = dtoList
            };
        }

        public async Task<InventoryApiResponseDto> AddItemToInventoryAsync(AddInventoryItemRequestDto request)
        {
            if (request == null || request.Quantity <= 0)
            {
                return new InventoryApiResponseDto
                {
                    Success = false,
                    Message = "Invalid request data."
                };
            }

            var existingItem = await _repository.GetByPlayerAndItemAsync(request.PlayerProfileId, request.ItemId);

            if (existingItem != null && !request.IsSkin)
            {
                existingItem.Quantity += request.Quantity;
                await _repository.UpdateAsync(existingItem);
            }
            else
            {
                var newItem = new InventoryItem
                {
                    PlayerProfileId = request.PlayerProfileId,
                    ItemId = request.ItemId,
                    Quantity = request.Quantity,
                    IsSkin = request.IsSkin,
                    IsEquipped = false,
                    EnhancementLevel = 0,
                    CreatedAt = DateTime.UtcNow
                };
                
                await _repository.AddAsync(newItem);
            }

            return new InventoryApiResponseDto
            {
                Success = true,
                Message = "Item added to inventory successfully."
            };
        }

        public async Task<InventoryApiResponseDto> UpdateInventoryItemAsync(int id, UpdateInventoryItemRequestDto request)
        {
            var item = await _repository.GetByIdAsync(id);

            if (item == null)
            {
                return new InventoryApiResponseDto
                {
                    Success = false,
                    Message = "Inventory item not found."
                };
            }

            item.Quantity = request.Quantity;
            item.IsEquipped = request.IsEquipped;
            item.EquippedSlot = request.EquippedSlot;
            item.EnhancementLevel = request.EnhancementLevel;

            await _repository.UpdateAsync(item);

            return new InventoryApiResponseDto
            {
                Success = true,
                Message = "Inventory item updated successfully."
            };
        }

        public async Task<InventoryApiResponseDto> RemoveItemFromInventoryAsync(int id)
        {
            var item = await _repository.GetByIdAsync(id);

            if (item == null)
            {
                return new InventoryApiResponseDto
                {
                    Success = false,
                    Message = "Inventory item not found."
                };
            }

            await _repository.DeleteAsync(item);

            return new InventoryApiResponseDto
            {
                Success = true,
                Message = "Inventory item removed successfully."
            };
        }
    }
}
