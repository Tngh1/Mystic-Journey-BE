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
    public class ItemService : IItemService
    {
        private readonly IItemRepository _repository;
        private readonly IMapper _mapper;

        public ItemService(IItemRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ItemResponseDto?> GetItemById(int id)
        {
            var item = await _repository.GetItemByIdWithStats(id);
            if (item == null)
                return null;

            var dto = _mapper.Map<ItemResponseDto>(item);

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
            item.IsActive = request.IsActive;
            item.IconUrl = request.IconUrl;

            if (IsEquipmentType(request.Type))
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
            return await GetItemById(updated.ItemId) ?? _mapper.Map<ItemResponseDto>(updated);
        }

        public async Task<PagedResultDto<ItemResponseDto>> GetItemsPaged(int page, int pageSize, string? search, string? type, string? rarity, bool? isActive, string? sortBy = null, string? sortOrder = null)
        {
            var (totalCount, items) = await _repository.GetItemsPaged(page, pageSize, search, type, rarity, isActive, sortBy, sortOrder);

            var dtos = items.Select(item => {
                var dto = _mapper.Map<ItemResponseDto>(item);
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

        private static bool IsEquipmentType(string type)
        {
            return type is "Weapon" or "Armor" or "Accessory";
        }
    }
}
