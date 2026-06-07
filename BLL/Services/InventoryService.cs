using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IMapper _mapper;

        public InventoryService(IInventoryRepository inventoryRepository, IMapper mapper)
        {
            _inventoryRepository = inventoryRepository;
            _mapper = mapper;
        }

        public async Task<InventoryItemResponseDto> AddItemToInventory(int playerProfileId, int itemId, int quantity)
        {
            var existing = await _inventoryRepository.GetByPlayerAndItem(playerProfileId, itemId);

            if (existing != null)
            {
                existing.Quantity += quantity;
                var updated = await _inventoryRepository.UpdateItem(existing);
                return MapToResponseDto(updated);
            }

            var newItem = new InventoryItem
            {
                PlayerProfileId = playerProfileId,
                ItemId = itemId,
                Quantity = quantity,
                IsEquipped = false,
                IsSkin = false,
                EnhancementLevel = 0
            };

            var created = await _inventoryRepository.AddItem(newItem);
            return MapToResponseDto(created);
        }

        private InventoryItemResponseDto MapToResponseDto(InventoryItem item)
        {
            return new InventoryItemResponseDto
            {
                InventoryItemId = item.InventoryItemId,
                PlayerProfileId = item.PlayerProfileId,
                ItemId = item.ItemId,
                ItemName = item.Item?.Name ?? string.Empty,
                ItemDescription = item.Item?.Description,
                ItemType = item.Item?.Type ?? string.Empty,
                ItemRarity = item.Item?.Rarity ?? string.Empty,
                IconUrl = item.Item?.IconUrl,
                Quantity = item.Quantity,
                IsEquipped = item.IsEquipped,
                IsSkin = item.IsSkin,
                EquippedSlot = item.EquippedSlot,
                EnhancementLevel = item.EnhancementLevel,
                CreatedAt = item.CreatedAt,
                BaseHp = item.Item?.EquipmentStats?.BaseHp ?? 0,
                BaseAtk = item.Item?.EquipmentStats?.BaseAtk ?? 0,
                BaseDef = item.Item?.EquipmentStats?.BaseDef ?? 0,
                BonusHp = item.Item?.EquipmentStats?.BonusHp ?? 0,
                BonusAtk = item.Item?.EquipmentStats?.BonusAtk ?? 0,
                BonusDef = item.Item?.EquipmentStats?.BonusDef ?? 0,
                BonusCritRate = item.Item?.EquipmentStats?.BonusCritRate ?? 0,
                BonusCritDamage = item.Item?.EquipmentStats?.BonusCritDamage ?? 0
            };
        }
    }
}
