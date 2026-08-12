using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using BLL.Exceptions;
using DAL.Models;
using DAL.Repositories.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Data;
using BLL.Utils;

namespace BLL.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IMapper _mapper;
        private readonly IPlayerStatRepository _statRepository;
        private readonly IPlayerProfileRepository _playerProfileRepository;
        private readonly ITransactionManager _transactionManager;
        private readonly ICharacterService _characterService;
        private const int BAG_CAPACITY = 100;
        private const int MAX_STACK_SIZE = 99;

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

        public InventoryService(
            IInventoryRepository inventoryRepository,
            IMapper mapper,
            IPlayerStatRepository statRepository,
            IPlayerProfileRepository playerProfileRepository,
            ITransactionManager transactionManager,
            ICharacterService characterService)
        {
            _inventoryRepository = inventoryRepository;
            _mapper = mapper;
            _statRepository = statRepository;
            _playerProfileRepository = playerProfileRepository;
            _transactionManager = transactionManager;
            _characterService = characterService;
        }

        public async Task<InventorySummaryDto> GetInventory(int playerProfileId)
        {
            var items = await _inventoryRepository.GetByPlayerId(playerProfileId);
            var dtos = _mapper.Map<List<InventoryItemResponseDto>>(items);

            var summary = new InventorySummaryDto
            {
                TotalItems = dtos.Sum(d => d.Quantity),
                BagItems = dtos.Where(d => !d.IsEquipped && !d.IsSkin).ToList(),
                EquippedItems = dtos.Where(d => d.IsEquipped).ToList(),
                BagCapacity = BAG_CAPACITY
            };

            var playerSkins = await _inventoryRepository.GetPlayerSkinsByPlayerId(playerProfileId);

            var profile = await _playerProfileRepository.GetPlayerProfileById(playerProfileId);
            string playerClass = profile?.Class ?? string.Empty;

            var allSkins = await _inventoryRepository.GetAllActiveSkins();
            var relevantSkins = allSkins.Where(skin => !IsSkinForAnotherClass(skin.Name, playerClass)).ToList();

            summary.TotalSkins = playerSkins.Count(ps => relevantSkins.Any(s => s.SkinId == ps.SkinId));
            
            summary.PlayerSkins = relevantSkins.Select(skin => {
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

        public async Task<InventoryActionResultDto> EquipItem(int actorPlayerProfileId, EquipItemRequestDto request)
        {
            var inv = await _inventoryRepository.GetById(request.InventoryItemId)
                ?? throw new KeyNotFoundException("Inventory item not found.");

            if (inv.PlayerProfileId != actorPlayerProfileId)
                throw new UnauthorizedAccessException("Item does not belong to player.");

            var slot = inv.Item?.Slot ?? inv.EquippedSlot;

            var (updatedInv, finalSnapshot) = await _transactionManager.ExecuteInTransactionAsync<(InventoryItem, PlayerStatsSnapshot?)>(async () =>
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

                int totalCritRate = equippedItems.Sum(i => i.Item?.EquipmentStats?.BonusCritRate ?? 0)
                    + equippedItems.Sum(i => i.EnhancementLevel * CRITRATE_ENHANCEMENT_PER_LEVEL_SCALED);
                int totalCritDamage = equippedItems.Sum(i => i.Item?.EquipmentStats?.BonusCritDamage ?? 0)
                    + equippedItems.Sum(i => i.EnhancementLevel * CRITDAMAGE_ENHANCEMENT_PER_LEVEL_SCALED);

                var snapshot = await _statRepository.GetSnapshotByPlayerProfileId(actorPlayerProfileId);
                bool isNewSnapshot = false;
                if (snapshot == null)
                {
                    snapshot = new PlayerStatsSnapshot { PlayerProfileId = actorPlayerProfileId };
                    isNewSnapshot = true;
                }

                snapshot.MaxHp = totalBaseHp + totalBonusHp + totalEnhHp;
                snapshot.Atk = totalBaseAtk + totalBonusAtk + totalEnhAtk;
                snapshot.Def = totalBaseDef + totalBonusDef + totalEnhDef;
                snapshot.CritRate = totalCritRate;
                snapshot.CritDamage = totalCritDamage;
                snapshot.MoveSpeed = equippedItems.Sum(i => i.Item?.EquipmentStats?.BonusMoveSpeed ?? 0)
                    + equippedItems.Sum(i => i.EnhancementLevel * MOVE_SPEED_ENH_PER_LEVEL_SCALED);
                snapshot.AttackSpeed = equippedItems.Sum(i => i.Item?.EquipmentStats?.BonusAttackSpeed ?? 0)
                    + equippedItems.Sum(i => i.EnhancementLevel * ATTACK_SPEED_ENH_PER_LEVEL_SCALED);
                snapshot.DamageBonus = equippedItems.Sum(i => i.Item?.EquipmentStats?.BonusDamageBonus ?? 0)
                    + equippedItems.Sum(i => i.EnhancementLevel * DAMAGEBONUS_ENH_PER_LEVEL_SCALED);
                snapshot.UpdatedAt = DateTime.UtcNow;

                if (isNewSnapshot)
                    await _statRepository.CreateSnapshot(snapshot);
                else
                    await _statRepository.UpdateSnapshot(snapshot);

                return (updated, snapshot);
            });

            var stats = finalSnapshot == null ? null : new PlayerStatsResponseDto
            {
                CurrentHp = finalSnapshot.MaxHp,
                MaxHp = finalSnapshot.MaxHp,
                Atk = finalSnapshot.Atk,
                Def = finalSnapshot.Def,
                MoveSpeed = (int)StatHelper.FromScaled(finalSnapshot.MoveSpeed, StatScale.MoveSpeed),
                AttackSpeed = (int)StatHelper.FromScaled(finalSnapshot.AttackSpeed, StatScale.AttackSpeed),
                CritRate = (int)StatHelper.FromScaled(finalSnapshot.CritRate, StatScale.CritRate),
                CritDamage = (int)StatHelper.FromScaled(finalSnapshot.CritDamage, StatScale.CritRate),
                DamageBonus = (int)StatHelper.FromScaled(finalSnapshot.DamageBonus, StatScale.DamageBonus),
                SkillPoints = 0,
                TotalWins = 0,
                TotalLosses = 0,
                TotalKills = 0,
                TotalDeaths = 0
            };

            return new InventoryActionResultDto
            {
                Item = _mapper.Map<InventoryItemResponseDto>(updatedInv),
                PlayerStats = stats
            };
        }

        public async Task<InventoryActionResultDto> UnequipItem(int actorPlayerProfileId, UnequipItemRequestDto request)
        {
            var inv = await _inventoryRepository.GetById(request.InventoryItemId)
                ?? throw new KeyNotFoundException("Inventory item not found.");

            if (inv.PlayerProfileId != actorPlayerProfileId)
                throw new UnauthorizedAccessException("Item does not belong to player.");

            var (updatedInv, finalSnapshot) = await _transactionManager.ExecuteInTransactionAsync<(InventoryItem, PlayerStatsSnapshot?)>(async () =>
            {
                inv.IsEquipped = false;
                inv.EquippedSlot = null;
                var updated = await _inventoryRepository.UpdateItem(inv);

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

                int totalCritRate = equippedItems.Sum(i => i.Item?.EquipmentStats?.BonusCritRate ?? 0)
                    + equippedItems.Sum(i => i.EnhancementLevel * CRITRATE_ENHANCEMENT_PER_LEVEL_SCALED);
                int totalCritDamage = equippedItems.Sum(i => i.Item?.EquipmentStats?.BonusCritDamage ?? 0)
                    + equippedItems.Sum(i => i.EnhancementLevel * CRITDAMAGE_ENHANCEMENT_PER_LEVEL_SCALED);

                var snapshot = await _statRepository.GetSnapshotByPlayerProfileId(actorPlayerProfileId);
                bool isNewSnapshot = false;
                if (snapshot == null)
                {
                    snapshot = new PlayerStatsSnapshot { PlayerProfileId = actorPlayerProfileId };
                    isNewSnapshot = true;
                }

                snapshot.MaxHp = totalBaseHp + totalBonusHp + totalEnhHp;
                snapshot.Atk = totalBaseAtk + totalBonusAtk + totalEnhAtk;
                snapshot.Def = totalBaseDef + totalBonusDef + totalEnhDef;
                snapshot.CritRate = totalCritRate;
                snapshot.CritDamage = totalCritDamage;
                snapshot.MoveSpeed = equippedItems.Sum(i => i.Item?.EquipmentStats?.BonusMoveSpeed ?? 0)
                    + equippedItems.Sum(i => i.EnhancementLevel * MOVE_SPEED_ENH_PER_LEVEL_SCALED);
                snapshot.AttackSpeed = equippedItems.Sum(i => i.Item?.EquipmentStats?.BonusAttackSpeed ?? 0)
                    + equippedItems.Sum(i => i.EnhancementLevel * ATTACK_SPEED_ENH_PER_LEVEL_SCALED);
                snapshot.DamageBonus = equippedItems.Sum(i => i.Item?.EquipmentStats?.BonusDamageBonus ?? 0)
                    + equippedItems.Sum(i => i.EnhancementLevel * DAMAGEBONUS_ENH_PER_LEVEL_SCALED);
                snapshot.UpdatedAt = DateTime.UtcNow;

                if (isNewSnapshot)
                    await _statRepository.CreateSnapshot(snapshot);
                else
                    await _statRepository.UpdateSnapshot(snapshot);

                return (updated, snapshot);
            });

            var stats = finalSnapshot == null ? null : new PlayerStatsResponseDto
            {
                CurrentHp = finalSnapshot.MaxHp,
                MaxHp = finalSnapshot.MaxHp,
                Atk = finalSnapshot.Atk,
                Def = finalSnapshot.Def,
                MoveSpeed = (int)StatHelper.FromScaled(finalSnapshot.MoveSpeed, StatScale.MoveSpeed),
                AttackSpeed = (int)StatHelper.FromScaled(finalSnapshot.AttackSpeed, StatScale.AttackSpeed),
                CritRate = (int)StatHelper.FromScaled(finalSnapshot.CritRate, StatScale.CritRate),
                CritDamage = (int)StatHelper.FromScaled(finalSnapshot.CritDamage, StatScale.CritRate),
                DamageBonus = (int)StatHelper.FromScaled(finalSnapshot.DamageBonus, StatScale.DamageBonus),
                SkillPoints = 0,
                TotalWins = 0,
                TotalLosses = 0,
                TotalKills = 0,
                TotalDeaths = 0
            };

            return new InventoryActionResultDto
            {
                Item = _mapper.Map<InventoryItemResponseDto>(updatedInv),
                PlayerStats = stats
            };
        }

        public async Task<ConsumeItemResultDto> ConsumeItem(int actorPlayerProfileId, ConsumeItemRequestDto request)
        {
            var inv = await _inventoryRepository.GetById(request.InventoryItemId)
                ?? throw new KeyNotFoundException("Inventory item not found.");

            if (inv.PlayerProfileId != actorPlayerProfileId)
                throw new UnauthorizedAccessException("Item does not belong to player.");

            bool isConsumable = inv.Item != null && inv.Item.Type == "Consumable";
            bool isQuestItem  = inv.Item != null && inv.Item.Type == "QuestItem";

            if (!isConsumable && !isQuestItem)
                throw new InvalidOperationException("Item is not consumable or a quest item.");

            if (request.Quantity <= 0)
                throw new ArgumentException("Quantity must be at least 1.");

            // QuestItem: chỉ xóa, không áp dụng hiệu ứng
            if (isQuestItem)
            {
                var questResult = new ConsumeItemResultDto
                {
                    ItemName          = inv.Item!.Name ?? string.Empty,
                    EffectType        = "None",
                    EffectValue       = 0,
                    RemainingQuantity = Math.Max(0, inv.Quantity - request.Quantity)
                };

                await _transactionManager.ExecuteInTransactionAsync(async () =>
                {
                    if (inv.Quantity < request.Quantity)
                        throw new InvalidOperationException("Not enough quantity to remove.");

                    inv.Quantity -= request.Quantity;
                    if (inv.Quantity <= 0)
                        await _inventoryRepository.DeleteItem(inv.InventoryItemId);
                    else
                        await _inventoryRepository.UpdateItem(inv);
                });

                return questResult;
            }

            var result = new ConsumeItemResultDto
            {
                ItemName          = inv.Item.Name ?? string.Empty,
                EffectType        = "None",
                EffectValue       = 0,
                RemainingQuantity = Math.Max(0, inv.Quantity - request.Quantity),
            };

            if (inv.Quantity < request.Quantity)
                throw new InvalidOperationException("Not enough quantity to consume.");

            // ── Resolve hiệu ứng TRƯỚC transaction ───────────────────────────────────
            // Luồng cũ trừ số lượng trước rồi mới apply hiệu ứng, nên uống bình máu lúc đã
            // đầy HP vẫn mất bình mà không hồi được điểm nào (các nhánh apply chỉ im lặng
            // no-op, không báo lỗi). Validate trước khi ghi để vật phẩm không bị tiêu vô ích
            // và client nhận được 400 kèm lý do để hiện popup.
            string itemName    = inv.Item?.Name ?? string.Empty;
            int healAmount     = ResolveHealPerUnit(itemName) * request.Quantity;
            int energyAmount   = itemName.Equals("Energy Elixir", StringComparison.OrdinalIgnoreCase)
                                    ? 60 * request.Quantity
                                    : 0;
            float corruptionPct = inv.Item?.CorruptionReduction ?? 0f;

            PlayerStat? stat    = null;
            int effectiveMaxHp  = 0;
            if (healAmount > 0)
            {
                stat = await _statRepository.GetByPlayerProfileId(actorPlayerProfileId);
                if (stat != null)
                {
                    // Trần HP thật = base + trang bị + buff achievement, KHÔNG phải stat.MaxHp.
                    effectiveMaxHp = await _characterService.GetEffectiveMaxHp(actorPlayerProfileId);
                    if (stat.CurrentHp >= effectiveMaxHp)
                        throw new InvalidOperationException("Your HP is already full.");
                }
            }

            PlayerProfile? profile = null;
            if (energyAmount > 0 || corruptionPct > 0)
            {
                profile = await _playerProfileRepository.GetPlayerProfileById(actorPlayerProfileId);
                if (profile != null)
                {
                    if (energyAmount > 0 && profile.CurrentEnergy >= profile.MaxEnergy)
                        throw new InvalidOperationException("Your energy is already full.");

                    if (corruptionPct > 0 && profile.CorruptionLevel <= 0)
                        throw new InvalidOperationException("Your corruption is already fully cleansed.");
                }
            }

            await _transactionManager.ExecuteInTransactionAsync(async () =>
            {
                inv.Quantity -= request.Quantity;
                if (inv.Quantity <= 0)
                    await _inventoryRepository.DeleteItem(inv.InventoryItemId);
                else
                    await _inventoryRepository.UpdateItem(inv);

                // ── Apply hiệu ứng đã resolve ở trên ─────────────────────────────────
                // Heal: clamp lên trần HP thật, nên 900/1000 uống bình 200 chỉ lên 1000.
                if (healAmount > 0 && stat != null)
                {
                    int before = stat.CurrentHp;
                    stat.CurrentHp = Math.Min(stat.CurrentHp + healAmount, effectiveMaxHp);
                    stat.UpdatedAt = DateTime.UtcNow;
                    await _statRepository.Update(stat);
                    result.EffectType  = "Heal";
                    // Trả về lượng hồi THỰC TẾ sau clamp, không phải mệnh giá bình, để client
                    // không hiện "+200 HP" khi thực chất chỉ hồi được 100.
                    result.EffectValue = stat.CurrentHp - before;
                    result.CurrentHp   = stat.CurrentHp;
                    result.MaxHp       = effectiveMaxHp;
                }

                if (energyAmount > 0 && profile != null)
                {
                    int before = profile.CurrentEnergy;
                    profile.CurrentEnergy = Math.Min(profile.CurrentEnergy + energyAmount, profile.MaxEnergy);
                    await _playerProfileRepository.UpdatePlayerProfile(profile);
                    result.EffectType    = "Energy";
                    result.EffectValue   = profile.CurrentEnergy - before;
                    result.CurrentEnergy = profile.CurrentEnergy;
                    result.MaxEnergy     = profile.MaxEnergy;
                }

                // CorruptionReduction là phần trăm 0..1 của mức corruption hiện tại.
                if (corruptionPct > 0 && profile != null)
                {
                    float reductionPct   = Math.Min(1f, corruptionPct); // clamp to 100%
                    float totalReduction = profile.CorruptionLevel * reductionPct * request.Quantity;
                    float before         = profile.CorruptionLevel;
                    profile.CorruptionLevel = Math.Max(0, profile.CorruptionLevel - totalReduction);
                    await _playerProfileRepository.UpdatePlayerProfile(profile);
                    result.EffectType      = "CorruptionReduction";
                    result.EffectValue     = (int)Math.Round(before - profile.CorruptionLevel);
                    result.CorruptionLevel = profile.CorruptionLevel;
                }
            });

            return result;
        }

        /// <summary>
        /// Lượng HP hồi trên mỗi đơn vị vật phẩm, tra theo tên. Trả 0 nếu không phải bình máu.
        /// Gộp 3 nhánh trùng nhau trước đây để guard "đã đầy máu" và phần apply dùng chung
        /// một nguồn số liệu, không lệch nhau khi thêm bình mới.
        /// </summary>
        private static int ResolveHealPerUnit(string itemName)
        {
            if (string.IsNullOrWhiteSpace(itemName)) return 0;

            if (itemName.Equals("Small Health Potion", StringComparison.OrdinalIgnoreCase)) return 80;
            if (itemName.Equals("Large Health Potion", StringComparison.OrdinalIgnoreCase)) return 200;

            // Fallback cho bình máu cũ/thêm sau, khớp hành vi trước đây.
            if (itemName.Contains("Health Potion", StringComparison.OrdinalIgnoreCase)) return 100;

            return 0;
        }


        public async Task<PlayerSkinResponseDto> EquipSkin(int actorPlayerProfileId, BLL.DTOs.EquipSkinRequestDto request)
        {
            var skin = await _inventoryRepository.GetPlayerSkinById(request.PlayerSkinId)
                ?? throw new KeyNotFoundException("PlayerSkin not found.");

            if (skin.PlayerProfileId != actorPlayerProfileId)
                throw new UnauthorizedAccessException("Skin does not belong to player.");

            return await _transactionManager.ExecuteInTransactionAsync(async () =>
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
                return _mapper.Map<PlayerSkinResponseDto>(updated);
            });
        }

        public async Task UnequipSkin(int actorPlayerProfileId, BLL.DTOs.UnequipSkinRequestDto request)
        {
            var playerSkins = await _inventoryRepository.GetPlayerSkinsByPlayerId(actorPlayerProfileId);
            var skin = playerSkins.FirstOrDefault(ps => ps.PlayerSkinId == request.PlayerSkinId)
                ?? throw new KeyNotFoundException("PlayerSkin not found.");

            if (skin.PlayerProfileId != actorPlayerProfileId)
                throw new UnauthorizedAccessException("Skin does not belong to player.");

            var profile = await _playerProfileRepository.GetPlayerProfileById(actorPlayerProfileId);
            string playerClass = profile?.Class ?? string.Empty;

            await _transactionManager.ExecuteInTransactionAsync(async () =>
            {
                skin.IsEquipped = false;
                await _inventoryRepository.UpdatePlayerSkin(skin);

                // Auto re-equip the Default skin for player's class if available
                var defaultSkin = playerSkins.FirstOrDefault(ps => ps.Skin != null && 
                    ps.Skin.Name.Contains("Default") && 
                    !IsSkinForAnotherClass(ps.Skin.Name, playerClass));

                if (defaultSkin != null && defaultSkin.PlayerSkinId != request.PlayerSkinId)
                {
                    defaultSkin.IsEquipped = true;
                    await _inventoryRepository.UpdatePlayerSkin(defaultSkin);
                }
            });
        }

        private static bool IsSkinForAnotherClass(string skinName, string playerClass)
        {
            if (string.IsNullOrWhiteSpace(skinName) || string.IsNullOrWhiteSpace(playerClass))
                return false;

            string cleanName = skinName.Trim();
            string cleanClass = playerClass.Trim();

            bool isDefault = cleanName.IndexOf("Default", StringComparison.OrdinalIgnoreCase) >= 0;
            if (isDefault)
            {
                if (cleanName.IndexOf("Knight", StringComparison.OrdinalIgnoreCase) >= 0 && !cleanClass.Equals("Knight", StringComparison.OrdinalIgnoreCase))
                    return true;
                if (cleanName.IndexOf("Archer", StringComparison.OrdinalIgnoreCase) >= 0 && !cleanClass.Equals("Archer", StringComparison.OrdinalIgnoreCase))
                    return true;
                if (cleanName.IndexOf("Mage", StringComparison.OrdinalIgnoreCase) >= 0 && !cleanClass.Equals("Mage", StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        public Task<InventoryItemResponseDto> AddItemToInventory(int playerProfileId, int itemId, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");

            return _transactionManager.ExecuteInTransactionAsync(async () =>
            {
                var inventory = await _inventoryRepository.GetByPlayerId(playerProfileId);
                var stackable = inventory
                    .Where(i => i.ItemId == itemId && !i.IsEquipped && !i.IsSkin && i.EnhancementLevel == 0)
                    .OrderBy(i => i.InventoryItemId)
                    .ToList();

                var freeInStacks = stackable.Sum(i => Math.Max(0, MAX_STACK_SIZE - i.Quantity));
                var freeSlots = Math.Max(0, BAG_CAPACITY - inventory.Count);
                var totalCapacity = freeInStacks + freeSlots * MAX_STACK_SIZE;
                if (totalCapacity < quantity)
                    throw new InventoryCapacityExceededException();

                var remaining = quantity;
                InventoryItem? lastChanged = null;
                foreach (var stack in stackable.Where(i => i.Quantity < MAX_STACK_SIZE))
                {
                    var added = Math.Min(MAX_STACK_SIZE - stack.Quantity, remaining);
                    stack.Quantity += added;
                    remaining -= added;
                    lastChanged = await _inventoryRepository.UpdateItem(stack);
                    if (remaining == 0)
                        break;
                }

                while (remaining > 0)
                {
                    var stackQuantity = Math.Min(MAX_STACK_SIZE, remaining);
                    var newItem = new InventoryItem
                    {
                        PlayerProfileId = playerProfileId,
                        ItemId = itemId,
                        Quantity = stackQuantity,
                        IsEquipped = false,
                        IsSkin = false,
                        EnhancementLevel = 0
                    };

                    lastChanged = await _inventoryRepository.AddItem(newItem);
                    remaining -= stackQuantity;
                }

                return _mapper.Map<InventoryItemResponseDto>(lastChanged!);
            }, IsolationLevel.Serializable);
        }



        public async Task<PlayerMeInventoryResponseDto> GetMeInventory(int playerProfileId)
        {
            var items = await _inventoryRepository.GetByPlayerId(playerProfileId);
            var dtos = _mapper.Map<List<InventoryItemResponseDto>>(items);
            return new PlayerMeInventoryResponseDto
            {
                PlayerProfileId = playerProfileId,
                Items = dtos,
                TotalCount = dtos.Count
            };
        }
    }
}
