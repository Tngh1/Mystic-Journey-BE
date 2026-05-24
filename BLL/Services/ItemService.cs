using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _repository;

        public ItemService(IItemRepository repository)
        {
            _repository = repository;
        }

        public async Task<ItemApiResponseDto> GetAllItemsAsync()
        {
            var items = await _repository.GetAllAsync();
            var dtoList = items.Select(i => new ItemResponseDto
            {
                Id = i.Id,
                Name = i.Name,
                Description = i.Description,
                Type = i.Type,
                Rarity = i.Rarity,
                Slot = i.Slot,
                BaseValue = i.BaseValue,
                MaxStack = i.MaxStack,
                IsTradable = i.IsTradable,
                IsActive = i.IsActive,
                IconUrl = i.IconUrl,
                CreatedAt = i.CreatedAt
            }).ToList();

            return new ItemApiResponseDto
            {
                Success = true,
                Message = "Items retrieved successfully.",
                Data = dtoList
            };
        }

        public async Task<ItemApiResponseDto> GetItemByIdAsync(int id)
        {
            var item = await _repository.GetByIdAsync(id);

            if (item == null)
            {
                return new ItemApiResponseDto
                {
                    Success = false,
                    Message = "Item not found."
                };
            }

            var dto = new ItemResponseDto
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Type = item.Type,
                Rarity = item.Rarity,
                Slot = item.Slot,
                BaseValue = item.BaseValue,
                MaxStack = item.MaxStack,
                IsTradable = item.IsTradable,
                IsActive = item.IsActive,
                IconUrl = item.IconUrl,
                CreatedAt = item.CreatedAt
            };

            return new ItemApiResponseDto
            {
                Success = true,
                Message = "Item retrieved successfully.",
                Data = dto
            };
        }

        public async Task<ItemApiResponseDto> CreateItemAsync(CreateItemRequestDto request)
        {
            var newItem = new Item
            {
                Name = request.Name,
                Description = request.Description,
                Type = request.Type,
                Rarity = request.Rarity,
                Slot = request.Slot,
                BaseValue = request.BaseValue,
                MaxStack = request.MaxStack,
                IsTradable = request.IsTradable,
                IsActive = request.IsActive,
                IconUrl = request.IconUrl,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(newItem);

            return new ItemApiResponseDto
            {
                Success = true,
                Message = "Item created successfully."
            };
        }

        public async Task<ItemApiResponseDto> UpdateItemAsync(int id, UpdateItemRequestDto request)
        {
            var item = await _repository.GetByIdAsync(id);

            if (item == null)
            {
                return new ItemApiResponseDto
                {
                    Success = false,
                    Message = "Item not found."
                };
            }

            item.Name = request.Name;
            item.Description = request.Description;
            item.Type = request.Type;
            item.Rarity = request.Rarity;
            item.Slot = request.Slot;
            item.BaseValue = request.BaseValue;
            item.MaxStack = request.MaxStack;
            item.IsTradable = request.IsTradable;
            item.IsActive = request.IsActive;
            item.IconUrl = request.IconUrl;

            await _repository.UpdateAsync(item);

            return new ItemApiResponseDto
            {
                Success = true,
                Message = "Item updated successfully."
            };
        }

        public async Task<ItemApiResponseDto> DeleteItemAsync(int id)
        {
            var item = await _repository.GetByIdAsync(id);

            if (item == null)
            {
                return new ItemApiResponseDto
                {
                    Success = false,
                    Message = "Item not found."
                };
            }

            await _repository.DeleteAsync(item);

            return new ItemApiResponseDto
            {
                Success = true,
                Message = "Item deleted successfully."
            };
        }
    }
}
