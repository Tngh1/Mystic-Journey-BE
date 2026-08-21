using AutoMapper;
using AutoMapper.QueryableExtensions;
using BLL.DTOs;
using BLL.Utils;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using System.Linq;

namespace BLL.Services
{
    // Executes core business logic for i item service.
    public class ItemService : IItemService
    {
        private readonly IItemRepository _repository;
        private readonly IMapper _mapper;

        // Initializes a new instance of ItemService with dependencies: repository, mapper.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public ItemService(IItemRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // Executes core business logic for get item by id.
        // Logic details: delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models.
        // Returns the computed ItemResponseDto? result asynchronously.
        public async Task<ItemResponseDto?> GetItemById(int id)
        {
            var item = await _repository.GetItemByIdWithStats(id);
            if (item == null)  // Entity not found — short-circuit with appropriate error result
                return null;

            var dto = _mapper.Map<ItemResponseDto>(item);  // Transform domain entity into DTO for the API response layer

            if (item.EquipmentStats != null)
            {
                dto.BaseHp = item.EquipmentStats.BaseHp;
                dto.BaseAtk = item.EquipmentStats.BaseAtk;
                dto.BaseDef = item.EquipmentStats.BaseDef;
                dto.BonusHp = item.EquipmentStats.BonusHp;
                dto.BonusAtk = item.EquipmentStats.BonusAtk;
                dto.BonusDef = item.EquipmentStats.BonusDef;
                dto.BonusCritRate = item.EquipmentStats.BonusCritRate != 0 ? BLL.Utils.StatHelper.FromScaled(item.EquipmentStats.BonusCritRate, BLL.Utils.StatScale.CritRate) : 0f;
                dto.BonusCritDamage = item.EquipmentStats.BonusCritDamage != 0 ? BLL.Utils.StatHelper.FromScaled(item.EquipmentStats.BonusCritDamage, BLL.Utils.StatScale.CritRate) : 0f;
            }

            return dto;
        }

        // Executes core business logic for update item.
        // Logic details: delegates data queries and updates to repository layer; throws KeyNotFoundException on invalid state or rule violations.
        // Returns the computed ItemResponseDto result asynchronously.
        public async Task<ItemResponseDto> UpdateItem(int id, UpdateItemRequestDto request)
        {
            var item = await _repository.GetItemByIdWithStats(id)
                ?? throw new KeyNotFoundException($"Item with id {id} not found.");

            item.Name = request.Name;
            item.Description = request.Description;
            item.Type = request.Type;
            item.Rarity = request.Rarity;
            item.Slot = request.Slot;
            item.BaseValue = request.BaseValue;
            item.MaxStack = request.MaxStack;
            item.CorruptionReduction = request.CorruptionReduction;
            item.IsActive = request.IsActive;
            item.IconUrl = request.IconUrl;

            if (IsEquipmentType(request.Type, request.Slot))
            {
                if (item.EquipmentStats == null)
                {
                    item.EquipmentStats = new EquipmentStats
                    {
                        ItemId = item.ItemId
                    };
                }

                item.EquipmentStats.BaseHp = request.BaseHp ?? 0;
                item.EquipmentStats.BaseAtk = request.BaseAtk ?? 0;
                item.EquipmentStats.BaseDef = request.BaseDef ?? 0;
                item.EquipmentStats.BonusHp = request.BonusHp ?? 0;
                item.EquipmentStats.BonusAtk = request.BonusAtk ?? 0;
                item.EquipmentStats.BonusDef = request.BonusDef ?? 0;
                item.EquipmentStats.BonusCritRate = StatHelper.ToScaledFromFloat(request.BonusCritRate ?? 0f, StatScale.CritRate);
                item.EquipmentStats.BonusCritDamage = StatHelper.ToScaledFromFloat(request.BonusCritDamage ?? 0f, StatScale.CritRate);
            }

            var updated = await _repository.UpdateItem(item);
            return await GetItemById(updated.ItemId) ?? _mapper.Map<ItemResponseDto>(updated);  // Transform domain entity into DTO for the API response layer
        }

        // Executes core business logic for get items paged.
        // Logic details: delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models.
        // Returns the computed PagedResultDto<ItemResponseDto result asynchronously.
        public async Task<PagedResultDto<ItemResponseDto>> GetItemsPaged(int page, int pageSize, string? search, string? type, string? rarity, bool? isActive, string? sortBy = null, string? sortOrder = null)
        {
            var (totalCount, items) = await _repository.GetItemsPaged(page, pageSize, search, type, rarity, isActive, sortBy, sortOrder);

            var dtos = items.Select(item => {
                var dto = _mapper.Map<ItemResponseDto>(item);  // Transform domain entity into DTO for the API response layer
                if (item.EquipmentStats != null)
                {
                    dto.BaseHp = item.EquipmentStats.BaseHp;
                    dto.BaseAtk = item.EquipmentStats.BaseAtk;
                    dto.BaseDef = item.EquipmentStats.BaseDef;
                    dto.BonusHp = item.EquipmentStats.BonusHp;
                    dto.BonusAtk = item.EquipmentStats.BonusAtk;
                    dto.BonusDef = item.EquipmentStats.BonusDef;
                    dto.BonusCritRate = item.EquipmentStats.BonusCritRate != 0 ? BLL.Utils.StatHelper.FromScaled(item.EquipmentStats.BonusCritRate, BLL.Utils.StatScale.CritRate) : 0f;
                    dto.BonusCritDamage = item.EquipmentStats.BonusCritDamage != 0 ? BLL.Utils.StatHelper.FromScaled(item.EquipmentStats.BonusCritDamage, BLL.Utils.StatScale.CritRate) : 0f;
                }
                return dto;
            }).ToList();

            return new PagedResultDto<ItemResponseDto>(totalCount, dtos);
        }

        // Executes core business logic for is equipment type.
        // Returns a boolean indicating operation success.
        private static bool IsEquipmentType(string type, string? slot)
        {
            return type is "Weapon" or "Armor" or "Accessory" || (!string.IsNullOrEmpty(slot) && !string.Equals(slot, "None", StringComparison.OrdinalIgnoreCase));
        }
    }
}
