using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _repository;
        private readonly IMapper _mapper;

        public ItemService(IItemRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ItemListResponseDto> GetAllItemsAsync(int pageNumber = 1, int pageSize = 20)
        {
            var items = await _repository.GetAllAsync(pageNumber, pageSize);
            var totalCount = await _repository.GetTotalCountAsync();

            return new ItemListResponseDto
            {
                Success = true,
                Message = "Items retrieved successfully.",
                Items = _mapper.Map<List<ItemResponseDto>>(items),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<ItemListResponseDto> GetItemsByTypeAsync(Item.ItemType type, int pageNumber = 1, int pageSize = 20)
        {
            var items = await _repository.GetByTypeAsync(type, pageNumber, pageSize);
            var totalCount = await _repository.GetTotalCountAsync();

            return new ItemListResponseDto
            {
                Success = true,
                Message = $"Items of type {type} retrieved successfully.",
                Items = _mapper.Map<List<ItemResponseDto>>(items),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<ItemListResponseDto> GetItemsByRarityAsync(Item.ItemRarity rarity, int pageNumber = 1, int pageSize = 20)
        {
            var items = await _repository.GetByRarityAsync(rarity, pageNumber, pageSize);
            var totalCount = await _repository.GetTotalCountAsync();

            return new ItemListResponseDto
            {
                Success = true,
                Message = $"Items of rarity {rarity} retrieved successfully.",
                Items = _mapper.Map<List<ItemResponseDto>>(items),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<ItemListResponseDto> SearchItemsAsync(string name, int pageNumber = 1, int pageSize = 20)
        {
            var items = await _repository.SearchByNameAsync(name, pageNumber, pageSize);

            return new ItemListResponseDto
            {
                Success = true,
                Message = $"Search results for '{name}' retrieved successfully.",
                Items = _mapper.Map<List<ItemResponseDto>>(items),
                TotalCount = items.Count,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<ItemApiResponseDto> GetItemByIdAsync(Guid itemId)
        {
            var item = await _repository.GetByIdAsync(itemId);

            if (item == null)
            {
                return new ItemApiResponseDto
                {
                    Success = false,
                    Message = "Item not found."
                };
            }

            return new ItemApiResponseDto
            {
                Success = true,
                Message = "Item retrieved successfully.",
                Item = _mapper.Map<ItemResponseDto>(item)
            };
        }

        public async Task<ItemApiResponseDto> GetItemDetailAsync(Guid itemId)
        {
            var item = await _repository.GetByIdWithStatsAsync(itemId);

            if (item == null)
            {
                return new ItemApiResponseDto
                {
                    Success = false,
                    Message = "Item not found."
                };
            }

            var detail = _mapper.Map<ItemDetailResponseDto>(item);
            if (item.EquipmentStats != null)
            {
                detail.EquipmentStats = _mapper.Map<EquipmentStatsDto>(item.EquipmentStats);
            }

            return new ItemApiResponseDto
            {
                Success = true,
                Message = "Item details retrieved successfully.",
                Detail = detail
            };
        }

        public async Task<ItemApiResponseDto> CreateItemAsync(CreateItemRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return new ItemApiResponseDto
                {
                    Success = false,
                    Message = "Item name is required."
                };
            }

            if (!Enum.IsDefined(typeof(Item.ItemType), request.Type))
            {
                return new ItemApiResponseDto
                {
                    Success = false,
                    Message = "Invalid item type."
                };
            }

            if (!Enum.IsDefined(typeof(Item.ItemRarity), request.Rarity))
            {
                return new ItemApiResponseDto
                {
                    Success = false,
                    Message = "Invalid item rarity."
                };
            }

            var item = new Item
            {
                Id = Guid.NewGuid(),
                Name = request.Name.Trim(),
                Description = request.Description,
                Type = (Item.ItemType)request.Type,
                Rarity = (Item.ItemRarity)request.Rarity,
                Slot = Enum.IsDefined(typeof(Item.EquipmentSlot), request.Slot)
                    ? (Item.EquipmentSlot)request.Slot
                    : Item.EquipmentSlot.None,
                BaseValue = request.BaseValue,
                MaxStack = request.MaxStack,
                IsTradable = request.IsTradable,
                IsActive = true,
                IconUrl = request.IconUrl,
                CreatedAt = DateTime.UtcNow
            };

            if (request.Stats != null && (item.Type == Item.ItemType.Weapon || item.Type == Item.ItemType.Armor || item.Type == Item.ItemType.Accessory))
            {
                item.EquipmentStats = new EquipmentStats
                {
                    Id = Guid.NewGuid(),
                    ItemId = item.Id,
                    HealthBonus = request.Stats.HealthBonus,
                    ManaBonus = request.Stats.ManaBonus,
                    StrengthBonus = request.Stats.StrengthBonus,
                    DefenseBonus = request.Stats.DefenseBonus,
                    AgilityBonus = request.Stats.AgilityBonus,
                    IntelligenceBonus = request.Stats.IntelligenceBonus,
                    EnduranceBonus = request.Stats.EnduranceBonus,
                    LuckBonus = request.Stats.LuckBonus,
                    AttackBonus = request.Stats.AttackBonus,
                    CriticalRateBonus = request.Stats.CriticalRateBonus,
                    CriticalDamageBonus = request.Stats.CriticalDamageBonus,
                    ArmorPenetrationBonus = request.Stats.ArmorPenetrationBonus
                };
            }

            await _repository.CreateAsync(item);

            return new ItemApiResponseDto
            {
                Success = true,
                Message = "Item created successfully.",
                Item = _mapper.Map<ItemResponseDto>(item)
            };
        }

        public async Task<ItemApiResponseDto> UpdateItemAsync(Guid itemId, UpdateItemRequestDto request)
        {
            var item = await _repository.GetByIdAsync(itemId);

            if (item == null)
            {
                return new ItemApiResponseDto
                {
                    Success = false,
                    Message = "Item not found."
                };
            }

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                item.Name = request.Name.Trim();
            }

            if (request.Description != null)
            {
                item.Description = request.Description;
            }

            if (request.Type.HasValue && Enum.IsDefined(typeof(Item.ItemType), request.Type.Value))
            {
                item.Type = (Item.ItemType)request.Type.Value;
            }

            if (request.Rarity.HasValue && Enum.IsDefined(typeof(Item.ItemRarity), request.Rarity.Value))
            {
                item.Rarity = (Item.ItemRarity)request.Rarity.Value;
            }

            if (request.Slot.HasValue && Enum.IsDefined(typeof(Item.EquipmentSlot), request.Slot.Value))
            {
                item.Slot = (Item.EquipmentSlot)request.Slot.Value;
            }

            if (request.BaseValue.HasValue)
            {
                item.BaseValue = request.BaseValue.Value;
            }

            if (request.MaxStack.HasValue)
            {
                item.MaxStack = request.MaxStack.Value;
            }

            if (request.IsTradable.HasValue)
            {
                item.IsTradable = request.IsTradable.Value;
            }

            if (request.IsActive.HasValue)
            {
                item.IsActive = request.IsActive.Value;
            }

            if (request.IconUrl != null)
            {
                item.IconUrl = request.IconUrl;
            }

            await _repository.UpdateAsync(item);

            return new ItemApiResponseDto
            {
                Success = true,
                Message = "Item updated successfully.",
                Item = _mapper.Map<ItemResponseDto>(item)
            };
        }

        public async Task<ItemApiResponseDto> DeleteItemAsync(Guid itemId)
        {
            var item = await _repository.GetByIdAsync(itemId);

            if (item == null)
            {
                return new ItemApiResponseDto
                {
                    Success = false,
                    Message = "Item not found."
                };
            }

            item.IsActive = false;
            await _repository.UpdateAsync(item);

            return new ItemApiResponseDto
            {
                Success = true,
                Message = "Item deleted successfully."
            };
        }
    }
}
