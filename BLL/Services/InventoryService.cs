using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IPlayerProfileRepository _profileRepository;
        private readonly IMapper _mapper;

        public InventoryService(
            IInventoryRepository inventoryRepository,
            IItemRepository itemRepository,
            IPlayerProfileRepository profileRepository,
            IMapper mapper)
        {
            _inventoryRepository = inventoryRepository;
            _itemRepository = itemRepository;
            _profileRepository = profileRepository;
            _mapper = mapper;
        }

        public async Task<InventoryResponseDto> GetPlayerInventoryAsync(Guid accountId, int pageNumber = 1, int pageSize = 50)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(accountId);
            if (profile == null)
            {
                return new InventoryResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var items = await _inventoryRepository.GetByPlayerProfileIdAsync(profile.Id, pageNumber, pageSize);
            var totalCount = await _inventoryRepository.GetTotalCountAsync(profile.Id);

            var itemDtos = items.Select(i => new InventoryItemResponseDto
            {
                InventoryItemId = i.Id,
                PlayerProfileId = i.PlayerProfileId,
                ItemId = i.ItemId,
                ItemName = i.Item?.Name ?? string.Empty,
                ItemType = i.Item?.Type.ToString() ?? string.Empty,
                ItemRarity = i.Item?.Rarity.ToString() ?? string.Empty,
                IconUrl = i.Item?.IconUrl,
                Quantity = i.Quantity,
                IsEquipped = i.IsEquipped,
                EnhancementLevel = i.EnhancementLevel,
                CreatedAt = i.CreatedAt
            }).ToList();

            return new InventoryResponseDto
            {
                Success = true,
                Message = "Inventory retrieved successfully.",
                Items = itemDtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<InventoryResponseDto> GetEquippedItemsAsync(Guid accountId)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(accountId);
            if (profile == null)
            {
                return new InventoryResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var items = await _inventoryRepository.GetEquippedItemsAsync(profile.Id);

            var itemDtos = items.Select(i => new InventoryItemResponseDto
            {
                InventoryItemId = i.Id,
                PlayerProfileId = i.PlayerProfileId,
                ItemId = i.ItemId,
                ItemName = i.Item?.Name ?? string.Empty,
                ItemType = i.Item?.Type.ToString() ?? string.Empty,
                ItemRarity = i.Item?.Rarity.ToString() ?? string.Empty,
                IconUrl = i.Item?.IconUrl,
                Quantity = i.Quantity,
                IsEquipped = i.IsEquipped,
                EnhancementLevel = i.EnhancementLevel,
                CreatedAt = i.CreatedAt
            }).ToList();

            return new InventoryResponseDto
            {
                Success = true,
                Message = "Equipped items retrieved successfully.",
                Items = itemDtos,
                TotalCount = itemDtos.Count
            };
        }

        public async Task<InventoryApiResponseDto> GetInventoryItemDetailAsync(Guid accountId, Guid inventoryItemId)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(accountId);
            if (profile == null)
            {
                return new InventoryApiResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var item = await _inventoryRepository.GetByIdWithDetailsAsync(inventoryItemId);
            if (item == null || item.PlayerProfileId != profile.Id)
            {
                return new InventoryApiResponseDto
                {
                    Success = false,
                    Message = "Inventory item not found."
                };
            }

            var detail = new InventoryItemDetailResponseDto
            {
                InventoryItemId = item.Id,
                PlayerProfileId = item.PlayerProfileId,
                ItemId = item.ItemId,
                ItemName = item.Item?.Name ?? string.Empty,
                ItemType = item.Item?.Type.ToString() ?? string.Empty,
                ItemRarity = item.Item?.Rarity.ToString() ?? string.Empty,
                IconUrl = item.Item?.IconUrl,
                Quantity = item.Quantity,
                IsEquipped = item.IsEquipped,
                EnhancementLevel = item.EnhancementLevel,
                CreatedAt = item.CreatedAt,
                Description = item.Item?.Description,
                BaseValue = item.Item?.BaseValue ?? 0,
                MaxStack = item.Item?.MaxStack ?? 1,
                IsTradable = item.Item?.IsTradable ?? true,
                EquipmentStats = item.Item?.EquipmentStats != null ? _mapper.Map<EquipmentStatsDto>(item.Item.EquipmentStats) : null
            };

            return new InventoryApiResponseDto
            {
                Success = true,
                Message = "Item details retrieved successfully.",
                Detail = detail
            };
        }

        public async Task<InventoryApiResponseDto> AddItemToInventoryAsync(Guid accountId, AddItemToInventoryRequestDto request)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(accountId);
            if (profile == null)
            {
                return new InventoryApiResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var item = await _itemRepository.GetByIdAsync(request.ItemId);
            if (item == null)
            {
                return new InventoryApiResponseDto
                {
                    Success = false,
                    Message = "Item not found."
                };
            }

            var quantity = Math.Max(1, request.Quantity);
            var existingStack = await _inventoryRepository.FindStackableItemAsync(profile.Id, request.ItemId);

            if (existingStack != null && existingStack.Quantity < existingStack.Item!.MaxStack)
            {
                var canAdd = Math.Min(quantity, existingStack.Item.MaxStack - existingStack.Quantity);
                existingStack.Quantity += canAdd;
                quantity -= canAdd;
                await _inventoryRepository.UpdateAsync(existingStack);
            }

            while (quantity > 0)
            {
                var stackSize = Math.Min(quantity, item.MaxStack);
                var newInventoryItem = new InventoryItem
                {
                    Id = Guid.NewGuid(),
                    PlayerProfileId = profile.Id,
                    ItemId = item.Id,
                    Quantity = stackSize,
                    IsEquipped = false,
                    EnhancementLevel = 0,
                    CreatedAt = DateTime.UtcNow
                };
                await _inventoryRepository.CreateAsync(newInventoryItem);
                quantity -= stackSize;
            }

            return new InventoryApiResponseDto
            {
                Success = true,
                Message = "Item added to inventory successfully."
            };
        }

        public async Task<InventoryApiResponseDto> RemoveItemFromInventoryAsync(Guid accountId, RemoveItemFromInventoryRequestDto request)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(accountId);
            if (profile == null)
            {
                return new InventoryApiResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var inventoryItem = await _inventoryRepository.GetByIdAsync(request.InventoryItemId);
            if (inventoryItem == null || inventoryItem.PlayerProfileId != profile.Id)
            {
                return new InventoryApiResponseDto
                {
                    Success = false,
                    Message = "Inventory item not found."
                };
            }

            if (inventoryItem.IsEquipped)
            {
                return new InventoryApiResponseDto
                {
                    Success = false,
                    Message = "Cannot remove equipped item. Please unequip first."
                };
            }

            var quantity = Math.Min(request.Quantity, inventoryItem.Quantity);
            inventoryItem.Quantity -= quantity;

            if (inventoryItem.Quantity <= 0)
            {
                await _inventoryRepository.DeleteAsync(inventoryItem);
            }
            else
            {
                await _inventoryRepository.UpdateAsync(inventoryItem);
            }

            return new InventoryApiResponseDto
            {
                Success = true,
                Message = $"Removed {quantity} item(s) from inventory."
            };
        }

        public async Task<InventoryApiResponseDto> EquipItemAsync(Guid accountId, EquipItemRequestDto request)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(accountId);
            if (profile == null)
            {
                return new InventoryApiResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var inventoryItem = await _inventoryRepository.GetByIdWithDetailsAsync(request.InventoryItemId);
            if (inventoryItem == null || inventoryItem.PlayerProfileId != profile.Id)
            {
                return new InventoryApiResponseDto
                {
                    Success = false,
                    Message = "Inventory item not found."
                };
            }

            if (inventoryItem.IsEquipped)
            {
                return new InventoryApiResponseDto
                {
                    Success = false,
                    Message = "Item is already equipped."
                };
            }

            var itemType = inventoryItem.Item?.Type;
            if (itemType != Item.ItemType.Weapon && itemType != Item.ItemType.Armor && itemType != Item.ItemType.Accessory)
            {
                return new InventoryApiResponseDto
                {
                    Success = false,
                    Message = "This item cannot be equipped."
                };
            }

            if (inventoryItem.Item?.Slot != Item.EquipmentSlot.None)
            {
                await _inventoryRepository.UnequipAllBySlotAsync(profile.Id, inventoryItem.Item.Slot);
            }

            inventoryItem.IsEquipped = true;
            await _inventoryRepository.UpdateAsync(inventoryItem);

            return new InventoryApiResponseDto
            {
                Success = true,
                Message = $"Equipped {inventoryItem.Item?.Name} successfully.",
                Item = _mapper.Map<InventoryItemResponseDto>(inventoryItem)
            };
        }

        public async Task<InventoryApiResponseDto> UnequipItemAsync(Guid accountId, UnequipItemRequestDto request)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(accountId);
            if (profile == null)
            {
                return new InventoryApiResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var inventoryItem = await _inventoryRepository.GetByIdAsync(request.InventoryItemId);
            if (inventoryItem == null || inventoryItem.PlayerProfileId != profile.Id)
            {
                return new InventoryApiResponseDto
                {
                    Success = false,
                    Message = "Inventory item not found."
                };
            }

            if (!inventoryItem.IsEquipped)
            {
                return new InventoryApiResponseDto
                {
                    Success = false,
                    Message = "Item is not equipped."
                };
            }

            inventoryItem.IsEquipped = false;
            await _inventoryRepository.UpdateAsync(inventoryItem);

            return new InventoryApiResponseDto
            {
                Success = true,
                Message = "Item unequipped successfully.",
                Item = _mapper.Map<InventoryItemResponseDto>(inventoryItem)
            };
        }

        public async Task<InventoryApiResponseDto> EnhanceItemAsync(Guid accountId, EnhanceItemRequestDto request)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(accountId);
            if (profile == null)
            {
                return new InventoryApiResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var inventoryItem = await _inventoryRepository.GetByIdWithDetailsAsync(request.InventoryItemId);
            if (inventoryItem == null || inventoryItem.PlayerProfileId != profile.Id)
            {
                return new InventoryApiResponseDto
                {
                    Success = false,
                    Message = "Inventory item not found."
                };
            }

            if (inventoryItem.EnhancementLevel >= 15)
            {
                return new InventoryApiResponseDto
                {
                    Success = false,
                    Message = "Item has reached maximum enhancement level."
                };
            }

            var enhancementCost = (inventoryItem.EnhancementLevel + 1) * 100;
            if (profile.Gold < enhancementCost)
            {
                return new InventoryApiResponseDto
                {
                    Success = false,
                    Message = $"Not enough gold. Required: {enhancementCost}"
                };
            }

            var successRate = 100 - (inventoryItem.EnhancementLevel * 5);
            var random = new Random();
            var roll = random.Next(100);

            profile.Gold -= enhancementCost;
            await _profileRepository.UpdateCurrencyAsync(profile.Id, profile.Gold, null, null);

            if (roll < successRate)
            {
                inventoryItem.EnhancementLevel++;
                await _inventoryRepository.UpdateAsync(inventoryItem);

                return new InventoryApiResponseDto
                {
                    Success = true,
                    Message = $"Item enhanced to level {inventoryItem.EnhancementLevel}!",
                    Item = _mapper.Map<InventoryItemResponseDto>(inventoryItem)
                };
            }

            return new InventoryApiResponseDto
            {
                Success = true,
                Message = "Enhancement failed. Gold has been spent but item was not enhanced."
            };
        }
    }
}
