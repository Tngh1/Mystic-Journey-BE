using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using DAL.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using BLL.Utils;

namespace BLL.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IMapper _mapper;
        private readonly MysticJourneyDbContext _context;

        // enhancement scaling per level (unscaled integers for HP/Atk/Def)
        private const int HP_ENHANCEMENT_PER_LEVEL = 10;
        private const int ATK_ENHANCEMENT_PER_LEVEL = 2;
        private const int DEF_ENHANCEMENT_PER_LEVEL = 1;
        // scaled enhancement per level for scaled stats (stored in snapshot as scaled ints)
        private const int CRITRATE_ENHANCEMENT_PER_LEVEL_SCALED = 5;   // e.g. 0.5% -> stored as 5 when CritRate scale=10
        private const int CRITDAMAGE_ENHANCEMENT_PER_LEVEL_SCALED = 20; // e.g. 2.0% -> stored as 20 when CritRate scale=10
        private const int MOVE_SPEED_ENH_PER_LEVEL_SCALED = 0;
        private const int ATTACK_SPEED_ENH_PER_LEVEL_SCALED = 0;
        private const int DAMAGEBONUS_ENH_PER_LEVEL_SCALED = 0;

        public InventoryService(IInventoryRepository inventoryRepository, IMapper mapper, MysticJourneyDbContext context)
        {
            _inventoryRepository = inventoryRepository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<InventorySummaryDto> GetInventory(int playerProfileId)
        {
            var items = await _inventoryRepository.GetByPlayerId(playerProfileId);
            var dtos = items.Select(i => MapToResponseDto(i)).ToList();

            var summary = new InventorySummaryDto
            {
                TotalItems = dtos.Sum(d => d.Quantity),
                BagItems = dtos.Where(d => !d.IsEquipped && !d.IsSkin).ToList(),
                EquippedItems = dtos.Where(d => d.IsEquipped).ToList(),
                BagCapacity = 200
            };

            var playerSkins = await _inventoryRepository.GetPlayerSkinsByPlayerId(playerProfileId);
            summary.TotalSkins = playerSkins.Count;
            
            var allSkins = await _context.Skins.Where(s => s.IsActive).ToListAsync();
            summary.PlayerSkins = allSkins.Select(skin => {
                var ps = playerSkins.FirstOrDefault(x => x.SkinId == skin.SkinId);
                return new PlayerSkinResponseDto
                {
                    PlayerSkinId = ps?.PlayerSkinId ?? 0,
                    PlayerProfileId = playerProfileId,
                    SkinId = skin.SkinId,
                    SkinName = skin.Name,
                    SkinDescription = skin.Description,
                    SkinType = skin.Type,
                    SkinRarity = skin.Rarity,
                    IconUrl = skin.IconUrl,
                    PreviewUrl = skin.PreviewUrl,
                    IsEquipped = ps?.IsEquipped ?? false,
                    UnlockedAt = ps?.UnlockedAt ?? default
                };
            }).ToList();
            return summary;
        }

        public async Task<InventoryItemResponseDto> EquipItem(int actorPlayerProfileId, EquipItemRequestDto request)
        {
            var inv = await _inventoryRepository.GetById(request.InventoryItemId)
                ?? throw new KeyNotFoundException("Inventory item not found.");

            if (inv.PlayerProfileId != actorPlayerProfileId)
                throw new UnauthorizedAccessException("Item does not belong to player.");

            var slot = inv.Item?.Slot ?? inv.EquippedSlot;

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                if (!string.IsNullOrEmpty(slot))
                {
                    var playerItems = await _inventoryRepository.GetByPlayerId(actorPlayerProfileId);
                    var conflict = playerItems.FirstOrDefault(i => i.IsEquipped && i.EquippedSlot == slot);
                    if (conflict != null)
                    {
                        conflict.IsEquipped = false;
                        conflict.EquippedSlot = null;
                        await _inventoryRepository.UpdateItem(conflict);
                    }
                }

                inv.IsEquipped = true;
                inv.EquippedSlot = slot;
                var updated = await _inventoryRepository.UpdateItem(inv);

                // recompute and persist player stats snapshot
                // get equipped items after update
                var allPlayerItems = await _inventoryRepository.GetByPlayerId(actorPlayerProfileId);
                var equippedItems = allPlayerItems.Where(i => i.IsEquipped).ToList();

                int totalBaseHp = equippedItems.Sum(i => i.Item?.EquipmentStats?.BaseHp ?? 0);
                int totalBonusHp = equippedItems.Sum(i => i.Item?.EquipmentStats?.BonusHp ?? 0);
                int totalEnhHp = equippedItems.Sum(i => i.EnhancementLevel * HP_ENHANCEMENT_PER_LEVEL);

                int totalBaseAtk = equippedItems.Sum(i => i.Item?.EquipmentStats?.BaseAtk ?? 0);
                int totalBonusAtk = equippedItems.Sum(i => i.Item?.EquipmentStats?.BonusAtk ?? 0);
                int totalEnhAtk = equippedItems.Sum(i => i.EnhancementLevel * ATK_ENHANCEMENT_PER_LEVEL);

                int totalBaseDef = equippedItems.Sum(i => i.Item?.EquipmentStats?.BaseDef ?? 0);
                int totalBonusDef = equippedItems.Sum(i => i.Item?.EquipmentStats?.BonusDef ?? 0);
                int totalEnhDef = equippedItems.Sum(i => i.EnhancementLevel * DEF_ENHANCEMENT_PER_LEVEL);

                // EquipmentStats are stored as SCALED integers (see StatScale). Sum them directly.
                int totalCritRate = equippedItems.Sum(i => i.Item?.EquipmentStats?.BonusCritRate ?? 0) + equippedItems.Sum(i => i.EnhancementLevel * CRITRATE_ENHANCEMENT_PER_LEVEL_SCALED);
                int totalCritDamage = equippedItems.Sum(i => i.Item?.EquipmentStats?.BonusCritDamage ?? 0) + equippedItems.Sum(i => i.EnhancementLevel * CRITDAMAGE_ENHANCEMENT_PER_LEVEL_SCALED);

                // persist snapshot into PlayerStatsSnapshots table
                var snapshot = await _context.PlayerStatsSnapshots.FirstOrDefaultAsync(s => s.PlayerProfileId == actorPlayerProfileId);
                if (snapshot == null)
                {
                    snapshot = new PlayerStatsSnapshot { PlayerProfileId = actorPlayerProfileId };
                    await _context.PlayerStatsSnapshots.AddAsync(snapshot);
                }

                snapshot.MaxHp = totalBaseHp + totalBonusHp + totalEnhHp;
                snapshot.Atk = totalBaseAtk + totalBonusAtk + totalEnhAtk;
                snapshot.Def = totalBaseDef + totalBonusDef + totalEnhDef;
                // scaled integer assignments
                // EquipmentStats fields are already stored as scaled integers, and enhancement contributions above are scaled too.
                snapshot.CritRate = totalCritRate;
                snapshot.CritDamage = totalCritDamage;
                snapshot.MoveSpeed = equippedItems.Sum(i => i.Item?.EquipmentStats?.BonusMoveSpeed ?? 0) + equippedItems.Sum(i => i.EnhancementLevel * MOVE_SPEED_ENH_PER_LEVEL_SCALED);
                snapshot.AttackSpeed = equippedItems.Sum(i => i.Item?.EquipmentStats?.BonusAttackSpeed ?? 0) + equippedItems.Sum(i => i.EnhancementLevel * ATTACK_SPEED_ENH_PER_LEVEL_SCALED);
                snapshot.DamageBonus = equippedItems.Sum(i => i.Item?.EquipmentStats?.BonusDamageBonus ?? 0) + equippedItems.Sum(i => i.EnhancementLevel * DAMAGEBONUS_ENH_PER_LEVEL_SCALED);
                snapshot.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                await tx.CommitAsync();
                return MapToResponseDto(updated);
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<InventoryItemResponseDto> UnequipItem(int actorPlayerProfileId, UnequipItemRequestDto request)
        {
            var inv = await _inventoryRepository.GetById(request.InventoryItemId)
                ?? throw new KeyNotFoundException("Inventory item not found.");

            if (inv.PlayerProfileId != actorPlayerProfileId)
                throw new UnauthorizedAccessException("Item does not belong to player.");

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                inv.IsEquipped = false;
                inv.EquippedSlot = null;
                var updated = await _inventoryRepository.UpdateItem(inv);

                // recompute and persist player stats snapshot after unequip
                var allPlayerItems = await _inventoryRepository.GetByPlayerId(actorPlayerProfileId);
                var equippedItems = allPlayerItems.Where(i => i.IsEquipped).ToList();

                int totalBaseHp = equippedItems.Sum(i => i.Item?.EquipmentStats?.BaseHp ?? 0);
                int totalBonusHp = equippedItems.Sum(i => i.Item?.EquipmentStats?.BonusHp ?? 0);
                int totalEnhHp = equippedItems.Sum(i => i.EnhancementLevel * HP_ENHANCEMENT_PER_LEVEL);

                int totalBaseAtk = equippedItems.Sum(i => i.Item?.EquipmentStats?.BaseAtk ?? 0);
                int totalBonusAtk = equippedItems.Sum(i => i.Item?.EquipmentStats?.BonusAtk ?? 0);
                int totalEnhAtk = equippedItems.Sum(i => i.EnhancementLevel * ATK_ENHANCEMENT_PER_LEVEL);

                int totalBaseDef = equippedItems.Sum(i => i.Item?.EquipmentStats?.BaseDef ?? 0);
                int totalBonusDef = equippedItems.Sum(i => i.Item?.EquipmentStats?.BonusDef ?? 0);
                int totalEnhDef = equippedItems.Sum(i => i.EnhancementLevel * DEF_ENHANCEMENT_PER_LEVEL);

                int totalCritRate = equippedItems.Sum(i => i.Item?.EquipmentStats?.BonusCritRate ?? 0) + equippedItems.Sum(i => i.EnhancementLevel * CRITRATE_ENHANCEMENT_PER_LEVEL_SCALED);
                int totalCritDamage = equippedItems.Sum(i => i.Item?.EquipmentStats?.BonusCritDamage ?? 0) + equippedItems.Sum(i => i.EnhancementLevel * CRITDAMAGE_ENHANCEMENT_PER_LEVEL_SCALED);

                var snapshot = await _context.PlayerStatsSnapshots.FirstOrDefaultAsync(s => s.PlayerProfileId == actorPlayerProfileId);
                if (snapshot == null)
                {
                    snapshot = new PlayerStatsSnapshot { PlayerProfileId = actorPlayerProfileId };
                    await _context.PlayerStatsSnapshots.AddAsync(snapshot);
                }

                snapshot.MaxHp = totalBaseHp + totalBonusHp + totalEnhHp;
                snapshot.Atk = totalBaseAtk + totalBonusAtk + totalEnhAtk;
                snapshot.Def = totalBaseDef + totalBonusDef + totalEnhDef;
                snapshot.CritRate = totalCritRate;
                snapshot.CritDamage = totalCritDamage;
                snapshot.MoveSpeed = equippedItems.Sum(i => i.Item?.EquipmentStats?.BonusMoveSpeed ?? 0) + equippedItems.Sum(i => i.EnhancementLevel * MOVE_SPEED_ENH_PER_LEVEL_SCALED);
                snapshot.AttackSpeed = equippedItems.Sum(i => i.Item?.EquipmentStats?.BonusAttackSpeed ?? 0) + equippedItems.Sum(i => i.EnhancementLevel * ATTACK_SPEED_ENH_PER_LEVEL_SCALED);
                snapshot.DamageBonus = equippedItems.Sum(i => i.Item?.EquipmentStats?.BonusDamageBonus ?? 0) + equippedItems.Sum(i => i.EnhancementLevel * DAMAGEBONUS_ENH_PER_LEVEL_SCALED);
                snapshot.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                await tx.CommitAsync();
                return MapToResponseDto(updated);
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task ConsumeItem(int actorPlayerProfileId, ConsumeItemRequestDto request)
        {
            var inv = await _inventoryRepository.GetById(request.InventoryItemId)
                ?? throw new KeyNotFoundException("Inventory item not found.");

            if (inv.PlayerProfileId != actorPlayerProfileId)
                throw new UnauthorizedAccessException("Item does not belong to player.");

            if (inv.Item == null || inv.Item.Type != "Consumable")
                throw new InvalidOperationException("Item is not consumable.");

            if (request.Quantity <= 0)
                throw new ArgumentException("Quantity must be at least 1.");

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                if (inv.Quantity < request.Quantity)
                    throw new InvalidOperationException("Not enough quantity to consume.");

                inv.Quantity -= request.Quantity;
                if (inv.Quantity <= 0)
                {
                    await _inventoryRepository.DeleteItem(inv.InventoryItemId);
                }
                else
                {
                    await _inventoryRepository.UpdateItem(inv);
                }

                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<PlayerSkinResponseDto> EquipSkin(int actorPlayerProfileId, BLL.DTOs.EquipSkinRequestDto request)
        {
            var skin = await _inventoryRepository.GetPlayerSkinById(request.PlayerSkinId)
                ?? throw new KeyNotFoundException("PlayerSkin not found.");

            if (skin.PlayerProfileId != actorPlayerProfileId)
                throw new UnauthorizedAccessException("Skin does not belong to player.");

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var playerSkins = await _inventoryRepository.GetPlayerSkinsByPlayerId(actorPlayerProfileId);
                foreach (var ps in playerSkins)
                {
                    if (ps.PlayerSkinId != skin.PlayerSkinId && ps.IsEquipped)
                    {
                        ps.IsEquipped = false;
                        await _inventoryRepository.UpdatePlayerSkin(ps);
                    }
                }

                skin.IsEquipped = request.IsEquipped;
                var updated = await _inventoryRepository.UpdatePlayerSkin(skin);
                await tx.CommitAsync();
                return _mapper.Map<PlayerSkinResponseDto>(updated);
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task UnequipSkin(int actorPlayerProfileId, BLL.DTOs.UnequipSkinRequestDto request)
        {
            var playerSkins = await _inventoryRepository.GetPlayerSkinsByPlayerId(actorPlayerProfileId);
            var skin = playerSkins.FirstOrDefault(ps => ps.PlayerSkinId == request.PlayerSkinId)
                ?? throw new KeyNotFoundException("PlayerSkin not found.");

            if (skin.PlayerProfileId != actorPlayerProfileId)
                throw new UnauthorizedAccessException("Skin does not belong to player.");

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                skin.IsEquipped = false;
                await _inventoryRepository.UpdatePlayerSkin(skin);

                // Tự động mặc lại skin Default nếu có
                var defaultSkin = await _context.PlayerSkins
                    .Include(ps => ps.Skin)
                    .FirstOrDefaultAsync(ps => ps.PlayerProfileId == actorPlayerProfileId && ps.Skin != null && ps.Skin.Name.Contains("Default"));

                if (defaultSkin != null && defaultSkin.PlayerSkinId != request.PlayerSkinId)
                {
                    defaultSkin.IsEquipped = true;
                    await _inventoryRepository.UpdatePlayerSkin(defaultSkin);
                }

                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
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
                ItemSlot = item.Item?.Slot ?? "None",
                IconUrl = item.Item?.IconUrl,
                Quantity = item.Quantity,
                IsEquipped = item.IsEquipped,
                IsSkin = item.IsSkin,
                EquippedSlot = item.EquippedSlot,
                EnhancementLevel = item.EnhancementLevel,
                CreatedAt = item.CreatedAt
            };
        }
    }
}
