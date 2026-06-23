using BLL.DTOs;
using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mystic_Journey_API.Extensions;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Mystic_Journey_API.Controllers
{
    // =============================================================================
    // SEED: Data Seeder Controller
    // Dùng để tạo dữ liệu mẫu cho development/testing
    // Không phải code của Player hay Manager Dashboard
    // =============================================================================
    // POST /api/seed/inventory  → Chèn toàn bộ dữ liệu mẫu
    // DELETE /api/seed/inventory → Xoá toàn bộ dữ liệu mẫu (để reset)
    //
    // Dữ liệu tạo ra:
    //   Items (4 loại):
    //     [1] Skin item  (IsSkin=true, tạo bằng InventoryItem với IsSkin=true)
    //     [2] Potion / Health Potion (Consumable, MaxStack=99)
    //     [3] Sword of Dawn (Weapon, +stat)
    //     [4] Iron Helm (Helmet/Armor, +stat)
    //
    //   Skins (3 loại):
    //     [1] Knight Default Skin  (class default)
    //     [2] Archer Default Skin  (class default)
    //     [3] Mage Default Skin    (class default)
    //     [4] Dragon Knight Skin   (premium skin)
    //
    //   Account test:
    //     testplayer / testplayer@mystic.test / Abc@12345
    //     Level 1, Class=Knight, mặc định skin Knight
    //     Inventory: 1 mũ (equipped), 2 bình máu, 1 skin khác (Dragon Knight) trong túi
    //
    //   Quests (3 quest cho 3 map):
    //     Quest 1 – Map 1: The Forest Awakening (Main)
    //     Quest 2 – Map 2: Dark Caverns (Main)
    //     Quest 3 – Map 3: Dragon Lair (Main)
    // =============================================================================
    [Route("api/[controller]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly MysticJourneyDbContext _ctx;

        public SeedController(MysticJourneyDbContext ctx)
        {
            _ctx = ctx;
        }

        // ─────────────────────────────────────────────────────────────────────────
        // POST /api/seed/inventory  → Seed toàn bộ dữ liệu mẫu UC 20
        // ─────────────────────────────────────────────────────────────────────────
        [HttpPost("inventory")]
        public async Task<IActionResult> SeedInventory()
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                // ── 1. Items (game items) ──────────────────────────────────────
                var existingItems = await _ctx.Items
                    .Where(i => EF.Functions.Like(i.Name, "[SEED]%"))
                    .ToListAsync();
                _ctx.Items.RemoveRange(existingItems);
                await _ctx.SaveChangesAsync();

                // 1a. Health Potion – Consumable
                var potion = new Item
                {
                    Name        = "[SEED] Health Potion",
                    Description = "Hồi phục 200 HP tức thì.",
                    Type        = "Consumable",
                    Rarity      = "Common",
                    Slot        = "None",
                    BaseValue   = 50,
                    MaxStack    = 99,
                    IsActive    = true,
                };
                _ctx.Items.Add(potion);

                // 1b. Sword of Dawn – Weapon (+ATK)
                var sword = new Item
                {
                    Name        = "[SEED] Sword of Dawn",
                    Description = "Kiếm bình minh, ánh sáng phá tan bóng tối.",
                    Type        = "Weapon",
                    Rarity      = "Rare",
                    Slot        = "Weapon",
                    BaseValue   = 500,
                    MaxStack    = 1,
                    IsActive    = true,
                };
                _ctx.Items.Add(sword);

                // 1c. Iron Helm – Helmet (+DEF, chỉ tăng chỉ số, không thay đổi ngoại hình)
                var helm = new Item
                {
                    Name        = "[SEED] Iron Helm",
                    Description = "Mũ sắt kiên cố, tăng phòng thủ.",
                    Type        = "Armor",
                    Rarity      = "Uncommon",
                    Slot        = "Helmet",
                    BaseValue   = 300,
                    MaxStack    = 1,
                    IsActive    = true,
                };
                _ctx.Items.Add(helm);

                await _ctx.SaveChangesAsync();

                // Gắn EquipmentStats cho Sword
                _ctx.EquipmentStats.Add(new EquipmentStats
                {
                    ItemId   = sword.ItemId,
                    BaseHp   = 0,
                    BaseAtk  = 45,
                    BaseDef  = 0,
                    BonusHp  = 0,
                    BonusAtk = 10,
                    BonusDef = 0,
                    BonusCritRate   = 50,   // 5.0% (scale=10)
                    BonusCritDamage = 100,  // 10.0%
                });

                // Gắn EquipmentStats cho Helm
                _ctx.EquipmentStats.Add(new EquipmentStats
                {
                    ItemId   = helm.ItemId,
                    BaseHp   = 100,
                    BaseAtk  = 0,
                    BaseDef  = 30,
                    BonusHp  = 20,
                    BonusAtk = 0,
                    BonusDef = 8,
                });

                await _ctx.SaveChangesAsync();

                // ── 2. Skins ─────────────────────────────────────────────────
                var existingSkins = await _ctx.Skins
                    .Where(s => EF.Functions.Like(s.Name, "[SEED]%"))
                    .ToListAsync();
                _ctx.Skins.RemoveRange(existingSkins);
                await _ctx.SaveChangesAsync();

                var skinKnight = new Skin
                {
                    Name        = "[SEED] Knight Default",
                    Description = "Trang phục mặc định của hiệp sĩ.",
                    Type        = "FullSet",
                    Rarity      = "Common",
                    IsForSale   = false,
                    IsActive    = true,
                };
                var skinArcher = new Skin
                {
                    Name        = "[SEED] Archer Default",
                    Description = "Trang phục mặc định của xạ thủ.",
                    Type        = "FullSet",
                    Rarity      = "Common",
                    IsForSale   = false,
                    IsActive    = true,
                };
                var skinMage = new Skin
                {
                    Name        = "[SEED] Mage Default",
                    Description = "Trang phục mặc định của pháp sư.",
                    Type        = "FullSet",
                    Rarity      = "Common",
                    IsForSale   = false,
                    IsActive    = true,
                };
                var skinDragon = new Skin
                {
                    Name        = "[SEED] Dragon Knight",
                    Description = "Giáp rồng huyền thoại. Thay đổi ngoại hình.",
                    Type        = "FullSet",
                    Rarity      = "Epic",
                    IsForSale   = true,
                    Price       = 500,
                    Currency    = "Gems",
                    IsActive    = true,
                };

                _ctx.Skins.AddRange(skinKnight, skinArcher, skinMage, skinDragon);
                await _ctx.SaveChangesAsync();

                // ── 3. Quests (3 quest – 3 map) ──────────────────────────────
                var existingQuests = await _ctx.Quests
                    .Where(q => EF.Functions.Like(q.Title, "[SEED]%"))
                    .ToListAsync();
                _ctx.Quests.RemoveRange(existingQuests);
                await _ctx.SaveChangesAsync();

                // If a seed skill exists for Knight Slash, set it as reward for Map 1.
                var knightSlash = await _ctx.Skills.FirstOrDefaultAsync(s => s.Name == "[SEED] Knight Slash");

                _ctx.Quests.AddRange(
                    new Quest
                    {
                        Title             = "[SEED] Map 1 – The Forest Awakening",
                        Description       = "Khám phá khu rừng cổ đại và tiêu diệt quái vật đầu tiên. (Map 1)",
                        Type              = "Main",
                        DefaultStatus     = "NotStarted",
                        RequiredLevel     = 1,
                        TargetAmount      = 5,
                        RewardExperience  = 200,
                        RewardGold        = 500,
                        RewardSkillId     = knightSlash != null ? (int?)knightSlash.SkillId : null,
                        IsActive          = true,
                    },
                    new Quest
                    {
                        Title             = "[SEED] Map 2 – Dark Caverns",
                        Description       = "Thâm nhập hang động tối tăm đầy bẫy và quái vật. (Map 2)",
                        Type              = "Main",
                        DefaultStatus     = "NotStarted",
                        RequiredLevel     = 2,
                        TargetAmount      = 3,
                        RewardExperience  = 150,
                        RewardGold        = 300,
                        IsActive          = true,
                    },
                    new Quest
                    {
                        Title             = "[SEED] Map 3 – Dragon Lair",
                        Description       = "Đối mặt với Rồng Thủ Lĩnh trong hang ổ của nó. (Map 3)",
                        Type              = "Main",
                        DefaultStatus     = "NotStarted",
                        RequiredLevel     = 3,
                        TargetAmount      = 10,
                        RewardExperience  = 400,
                        RewardGold        = 800,
                        RewardGems        = 5,
                        RewardItemId      = potion.ItemId,
                        IsActive          = true,
                    }
                );
                await _ctx.SaveChangesAsync();

                // ── 4. Account + PlayerProfile test ──────────────────────────
                const string TEST_EMAIL    = "testplayer@mystic.test";
                const string TEST_USERNAME = "testplayer";

                // Xoá nếu đã tồn tại
                var existingAcc = await _ctx.Accounts
                    .Include(a => a.PlayerProfile)
                    .FirstOrDefaultAsync(a => a.Email == TEST_EMAIL);

                if (existingAcc != null)
                {
                    var pp = existingAcc.PlayerProfile;
                    if (pp != null)
                    {
                        // Xoá inventory, PlayerSkins, PlayerQuests, PlayerStats
                        _ctx.InventoryItems.RemoveRange(
                            _ctx.InventoryItems.Where(x => x.PlayerProfileId == pp.PlayerProfileId));
                        _ctx.PlayerSkins.RemoveRange(
                            _ctx.PlayerSkins.Where(x => x.PlayerProfileId == pp.PlayerProfileId));
                        _ctx.PlayerQuests.RemoveRange(
                            _ctx.PlayerQuests.Where(x => x.PlayerProfileId == pp.PlayerProfileId));
                        _ctx.PlayerStats.RemoveRange(
                            _ctx.PlayerStats.Where(x => x.PlayerProfileId == pp.PlayerProfileId));
                        _ctx.PlayerStatsSnapshots.RemoveRange(
                            _ctx.PlayerStatsSnapshots.Where(x => x.PlayerProfileId == pp.PlayerProfileId));
                        await _ctx.SaveChangesAsync();
                        _ctx.PlayerProfiles.Remove(pp);
                    }
                    _ctx.Accounts.Remove(existingAcc);
                    await _ctx.SaveChangesAsync();
                }

                // Tạo account
                var account = new Account
                {
                    UserName       = TEST_USERNAME,
                    Email          = TEST_EMAIL,
                    HashPassword   = HashPassword("Abc@12345"),
                    RoleId         = 1, // Player
                    IsActive       = true,
                    CreatedAt      = DateTime.UtcNow,
                };
                _ctx.Accounts.Add(account);
                await _ctx.SaveChangesAsync();

                // Tạo PlayerProfile
                var profile = new PlayerProfile
                {
                    AccountId    = account.AccountId,
                    DisplayName  = "Test Player",
                    Class        = "Knight",
                    Level        = 1,
                    ExperiencePoints = 0,
                    Gold         = 1500,
                    Gems         = 200,
                    Energy       = 100,
                    LastMapName  = "ElfForest",
                    PositionX    = 11.9,
                    PositionY    = 17.8,
                    AvatarUrl    = "",
                };
                _ctx.PlayerProfiles.Add(profile);
                await _ctx.SaveChangesAsync();

                int pid = profile.PlayerProfileId;

                // Tạo PlayerStats (base stats lv 3)
                _ctx.PlayerStats.Add(new PlayerStat
                {
                    PlayerProfileId = pid,
                    CurrentHp = 350, MaxHp = 350,
                    Atk = 45, Def = 20,
                    MoveSpeed = 50, AttackSpeed = 10,
                    CritRate = 50, CritDamage = 150,
                    DamageBonus = 0,
                    SkillPoints = 3,
                    TotalWins = 5, TotalLosses = 2, TotalKills = 23, TotalDeaths = 4
                });
                await _ctx.SaveChangesAsync();

                // ── 5. Inventory của player test ─────────────────────────────
                // 5a. Iron Helm – EQUIPPED (Helmet)
                _ctx.InventoryItems.Add(new InventoryItem
                {
                    PlayerProfileId  = pid,
                    ItemId           = helm.ItemId,
                    Quantity         = 1,
                    IsEquipped       = true,
                    IsSkin           = false,
                    EquippedSlot     = "Helmet",
                    EnhancementLevel = 0,
                });

                // 5b. Health Potion – 2 bình (trong túi)
                _ctx.InventoryItems.Add(new InventoryItem
                {
                    PlayerProfileId  = pid,
                    ItemId           = potion.ItemId,
                    Quantity         = 2,
                    IsEquipped       = false,
                    IsSkin           = false,
                    EnhancementLevel = 0,
                });

                await _ctx.SaveChangesAsync();

                // ── 6. PlayerSkins ────────────────────────────────────────────
                // Knight Default – EQUIPPED (mặc định theo class)
                _ctx.PlayerSkins.Add(new PlayerSkin
                {
                    PlayerProfileId = pid,
                    SkinId          = skinKnight.SkinId,
                    IsEquipped      = true,
                    UnlockedAt      = DateTime.UtcNow,
                });

                // Dragon Knight – CÓ nhưng không mặc (skin khác)
                _ctx.PlayerSkins.Add(new PlayerSkin
                {
                    PlayerProfileId = pid,
                    SkinId          = skinDragon.SkinId,
                    IsEquipped      = false,
                    UnlockedAt      = DateTime.UtcNow,
                });

                await _ctx.SaveChangesAsync();

                // ── 7. PlayerQuests ──────────────────────────────────────────
                var quests = await _ctx.Quests
                    .Where(q => EF.Functions.Like(q.Title, "[SEED]%"))
                    .ToListAsync();

                foreach (var q in quests)
                {
                    _ctx.PlayerQuests.Add(new PlayerQuest
                    {
                        PlayerProfileId = pid,
                        QuestId         = q.QuestId,
                        Status          = "NotStarted",
                        TargetValue     = q.TargetAmount,
                        AcceptedAt      = DateTime.UtcNow,
                    });
                }
                await _ctx.SaveChangesAsync();

                // ── 8. PlayerStatsSnapshot (equip snapshot) ──────────────────
                _ctx.PlayerStatsSnapshots.Add(new PlayerStatsSnapshot
                {
                    PlayerProfileId = pid,
                    MaxHp        = 350 + 100 + 20, // base + BaseHp(helm) + BonusHp(helm)
                    Atk          = 45,
                    Def          = 20 + 30 + 8,    // base + BaseDef + BonusDef
                    CritRate     = 50,
                    CritDamage   = 150,
                    MoveSpeed    = 0,
                    AttackSpeed  = 0,
                    DamageBonus  = 0,
                    CreatedAt    = DateTime.UtcNow,
                });
                await _ctx.SaveChangesAsync();

                await tx.CommitAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Seed thành công!",
                    Data = new
                    {
                        accountEmail    = TEST_EMAIL,
                        password        = "Abc@12345",
                        playerProfileId = pid,
                        level           = 3,
                        playerClass     = "Knight",
                        items = new[]
                        {
                            new { name = "[SEED] Iron Helm",      type = "Armor (Helmet)",   status = "EQUIPPED"  },
                            new { name = "[SEED] Health Potion",  type = "Consumable",        status = "BAG x2"    },
                        },
                        skins = new[]
                        {
                            new { name = "[SEED] Knight Default", status = "EQUIPPED (default)" },
                            new { name = "[SEED] Dragon Knight",  status = "BAG (unlocked)"     },
                        },
                        quests = new[]
                        {
                            new { title = "[SEED] Map 1 – The Forest Awakening", requiredLevel = 1 },
                            new { title = "[SEED] Map 2 – Dark Caverns",         requiredLevel = 2 },
                            new { title = "[SEED] Map 3 – Dragon Lair",          requiredLevel = 3 },
                        },
                        seedItems_available_in_admin = new[]
                        {
                            new { name = "[SEED] Health Potion", itemId = potion.ItemId },
                            new { name = "[SEED] Sword of Dawn", itemId = sword.ItemId  },
                            new { name = "[SEED] Iron Helm",     itemId = helm.ItemId   },
                        },
                    }
                });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return StatusCode(500, new ApiResponse<object> { Success = false, Message = ex.Message, ErrorCode = ErrorCodes.InternalError });
            }
        }

        // ─────────────────────────────────────────────────────────────────────────
        // POST /api/seed/skills  → Upsert 3 hệ thống Skill mẫu
        // Dùng để nhanh chóng chèn 3 skill cơ bản cho toàn bộ hệ thống (không phải player)
        // ─────────────────────────────────────────────────────────────────────────
        [HttpPost("skills")]
        public async Task<IActionResult> SeedSkills()
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                // Ensure five skill records exist. Use explicit SkillId values so external
                // client assets that expect stable ids (1..5) can reference them.
                var skillList = new List<Skill>
                {
                    new Skill
                    {
                        SkillId = 1,
                        Name = "AP_Skill",
                        Description = "AP system skill (area/magic example).",
                        Type = "Active",
                        DamageType = "Magical",
                        TargetType = "Area",
                        ClassRequirement = "Mage",
                        CooldownSeconds = 8,
                        BaseDamage = 120.0,
                        DamagePerLevel = 15.0,
                        DamageGrowthPercent = 5.0,
                        UnlockLevel = 1,
                        IsActive = true
                    },
                    new Skill
                    {
                        SkillId = 2,
                        Name = "Adrenaline",
                        Description = "Temporary buff that increases damage output.",
                        Type = "Buff",
                        DamageType = "TrueDamage",
                        TargetType = "Self",
                        ClassRequirement = "All",
                        CooldownSeconds = 30,
                        BaseDamage = 0.0,
                        DamagePerLevel = 0.0,
                        DamageGrowthPercent = 0.0,
                        UnlockLevel = 1,
                        IsActive = true
                    },
                    new Skill
                    {
                        SkillId = 3,
                        Name = "Knight_Slash",
                        Description = "Basic knight melee slash (single target physical).",
                        Type = "Active",
                        DamageType = "Physical",
                        TargetType = "SingleTarget",
                        ClassRequirement = "Knight",
                        CooldownSeconds = 5,
                        BaseDamage = 90.0,
                        DamagePerLevel = 12.0,
                        DamageGrowthPercent = 3.0,
                        UnlockLevel = 1,
                        IsActive = true
                    },
                    new Skill
                    {
                        SkillId = 4,
                        Name = "Multi_Arrow",
                        Description = "Archer multi-arrow attack hitting several targets.",
                        Type = "Active",
                        DamageType = "Physical",
                        TargetType = "MultiTarget",
                        ClassRequirement = "Archer",
                        CooldownSeconds = 7,
                        BaseDamage = 75.0,
                        DamagePerLevel = 10.0,
                        DamageGrowthPercent = 2.5,
                        UnlockLevel = 1,
                        IsActive = true
                    },
                    new Skill
                    {
                        SkillId = 5,
                        Name = "Light_Explosion",
                        Description = "Mage light explosion skill (aoe).",
                        Type = "Active",
                        DamageType = "Magical",
                        TargetType = "Area",
                        ClassRequirement = "Mage",
                        CooldownSeconds = 10,
                        BaseDamage = 140.0,
                        DamagePerLevel = 18.0,
                        DamageGrowthPercent = 6.0,
                        UnlockLevel = 1,
                        IsActive = true
                    }
                };

                foreach (var s in skillList)
                {
                    var existing = await _ctx.Skills.FirstOrDefaultAsync(x => x.SkillId == s.SkillId || x.Name == s.Name);
                    if (existing == null)
                    {
                        // Insert with explicit SkillId (identity column allows explicit values)
                        _ctx.Skills.Add(s);
                    }
                    else
                    {
                        existing.Name = s.Name;
                        existing.Description = s.Description;
                        existing.Type = s.Type;
                        existing.DamageType = s.DamageType;
                        existing.TargetType = s.TargetType;
                        existing.ClassRequirement = s.ClassRequirement;
                        existing.CooldownSeconds = s.CooldownSeconds;
                        existing.BaseDamage = s.BaseDamage;
                        existing.DamagePerLevel = s.DamagePerLevel;
                        existing.DamageGrowthPercent = s.DamageGrowthPercent;
                        existing.UnlockLevel = s.UnlockLevel;
                        existing.IsActive = s.IsActive;
                    }
                }

                await _ctx.SaveChangesAsync();
                await tx.CommitAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Seed 5 skills thành công",
                    Data = new { seededSkillIds = skillList.Select(x => x.SkillId).ToArray() }
                });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return StatusCode(500, new ApiResponse<object> { Success = false, Message = ex.Message, ErrorCode = ErrorCodes.InternalError });
            }
        }

        // ─────────────────────────────────────────────────────────────────────────
        // POST /api/seed/elfforest -> Seed tutorial world on map ElfForest
        // ─────────────────────────────────────────────────────────────────────────
        [HttpPost("elfforest")]
        public async Task<IActionResult> SeedElfForest()
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                await EnsureElfForestSchema();

                // 1. Upsert ElfForest items. Keep IDs stable so existing inventory/rewards do not break.
                var itemNames = new[]
                {
                    "[ELF] Health Potion",
                    "[ELF] Short Sword",
                    "[ELF] Leather Armor",
                    "[ELF] White Flower",
                    "[ELF] Old Willow Branch"
                };
                var existingItems = await _ctx.Items
                    .Where(i => itemNames.Contains(i.Name))
                    .ToListAsync();

                Item UpsertItem(string name, string description, string type, string rarity, string slot, decimal baseValue, int maxStack)
                {
                    var item = existingItems.FirstOrDefault(i => i.Name == name);
                    if (item == null)
                    {
                        item = new Item { Name = name };
                        _ctx.Items.Add(item);
                    }

                    item.Description = description;
                    item.Type = type;
                    item.Rarity = rarity;
                    item.Slot = slot;
                    item.BaseValue = baseValue;
                    item.MaxStack = maxStack;
                    item.IsActive = true;
                    return item;
                }

                var potion = UpsertItem("[ELF] Health Potion", "Restores 150 HP.", "Consumable", "Common", "None", 30, 99);
                var sword = UpsertItem("[ELF] Short Sword", "A short sword used by forest scouts.", "Weapon", "Uncommon", "Weapon", 200, 1);
                var armor = UpsertItem("[ELF] Leather Armor", "Light leather armor that grants a small defense bonus.", "Armor", "Common", "Armor", 180, 1);
                UpsertItem("[ELF] White Flower", "A white flower used for the ElfForest tutorial quest.", "QuestItem", "Common", "None", 0, 99);
                UpsertItem("[ELF] Old Willow Branch", "A branch from the old willow, used for ElfForest quest objectives.", "QuestItem", "Common", "None", 0, 99);
                await _ctx.SaveChangesAsync();

                var existingEquipmentStats = await _ctx.EquipmentStats
                    .Where(s => s.ItemId == sword.ItemId || s.ItemId == armor.ItemId)
                    .ToListAsync();

                void UpsertEquipmentStats(Item item, int baseHp, int baseAtk, int baseDef, int bonusHp, int bonusAtk, int bonusDef)
                {
                    var stats = existingEquipmentStats.FirstOrDefault(s => s.ItemId == item.ItemId);
                    if (stats == null)
                    {
                        stats = new EquipmentStats { ItemId = item.ItemId };
                        _ctx.EquipmentStats.Add(stats);
                    }

                    stats.BaseHp = baseHp;
                    stats.BaseAtk = baseAtk;
                    stats.BaseDef = baseDef;
                    stats.BonusHp = bonusHp;
                    stats.BonusAtk = bonusAtk;
                    stats.BonusDef = bonusDef;
                    stats.BonusMoveSpeed = 0;
                    stats.BonusAttackSpeed = 0;
                    stats.BonusCritRate = 0;
                    stats.BonusCritDamage = 0;
                    stats.BonusDamageBonus = 0;
                }

                UpsertEquipmentStats(sword, 0, 30, 0, 0, 5, 0);
                UpsertEquipmentStats(armor, 50, 0, 12, 10, 0, 2);
                await _ctx.SaveChangesAsync();

                // 2. Upsert skins, same reason as items: player skin ownership can reference them.
                var skinNames = new[] { "[ELF] ElfForest Default", "[ELF] Ranger Cloak" };
                var existingSkins = await _ctx.Skins
                    .Where(s => skinNames.Contains(s.Name))
                    .ToListAsync();

                Skin UpsertSkin(string name, string description, string type, string rarity)
                {
                    var skin = existingSkins.FirstOrDefault(s => s.Name == name);
                    if (skin == null)
                    {
                        skin = new Skin { Name = name };
                        _ctx.Skins.Add(skin);
                    }

                    skin.Description = description;
                    skin.Type = type;
                    skin.Rarity = rarity;
                    skin.IsForSale = false;
                    skin.IsActive = true;
                    return skin;
                }

                var skinDefault = UpsertSkin("[ELF] ElfForest Default", "Default outfit for the ElfForest tutorial area.", "FullSet", "Common");
                var skinAlt = UpsertSkin("[ELF] Ranger Cloak", "A cloak worn by forest rangers.", "Cloak", "Rare");
                await _ctx.SaveChangesAsync();

                // 3. Reset ElfForest quests and dependent records in FK-safe order.
                var existingQuests = await _ctx.Quests.Where(q => EF.Functions.Like(q.Title, "[ELFFOREST]%")).ToListAsync();
                if (existingQuests.Count > 0)
                {
                    var existingQuestIds = existingQuests.Select(q => q.QuestId).ToList();
                    _ctx.NPCDialogues.RemoveRange(_ctx.NPCDialogues.Where(d => d.LinkedQuestId.HasValue && existingQuestIds.Contains(d.LinkedQuestId.Value)));
                    _ctx.PlayerQuests.RemoveRange(_ctx.PlayerQuests.Where(pq => existingQuestIds.Contains(pq.QuestId)));
                    await _ctx.SaveChangesAsync();

                    _ctx.Quests.RemoveRange(existingQuests);
                    await _ctx.SaveChangesAsync();
                }

                // Upsert a tutorial skill that will be rewarded by the Gather White Flowers quest
                var tutorialSkill = await _ctx.Skills.FirstOrDefaultAsync(s => s.Name == "[ELF] First Strike");
                if (tutorialSkill == null)
                {
                    tutorialSkill = new Skill
                    {
                        Name = "[ELF] First Strike",
                        Description = "A simple tutorial melee strike taught by Elder Rowan.",
                        Type = "Active",
                        DamageType = "Physical",
                        TargetType = "SingleTarget",
                        ClassRequirement = "All",
                        CooldownSeconds = 6,
                        BaseDamage = 40.0,
                        DamagePerLevel = 6.0,
                        DamageGrowthPercent = 2.0,
                        UnlockLevel = 1,
                        IsActive = true
                    };
                    _ctx.Skills.Add(tutorialSkill);
                    await _ctx.SaveChangesAsync();
                }

                var quests = new List<Quest>
                {
                    new Quest
                    {
                        Title = "[ELFFOREST] Speak With Elder Rowan",
                        Description = "Speak with Elder Rowan and answer his first questions to begin the tutorial.",
                        Type = "Main",
                        DefaultStatus = "NotStarted",
                        MapName = "ElfForest",
                        RegionName = "ElfLand",
                        ObjectiveType = "Talk",
                        ObjectiveTarget = "Elder Rowan",
                        ObjectiveLocation = "Elder Rowan's Camp",
                        QuestGiverName = "Elder Rowan",
                        RequiredLevel = 1,
                        TargetAmount = 1,
                        RewardExperience = 10,
                        RewardGold = 10,
                        RewardGems = 0,
                        RewardItemId = null,
                        RewardSkillId = null,
                        IsActive = true,
                    },
                    new Quest
                    {
                        Title = "[ELFFOREST] Gather White Flowers",
                        Description = "Elder Rowan asks the player to collect White Flowers around the old willow clearing.",
                        Type = "Main",
                        DefaultStatus = "NotStarted",
                        MapName = "ElfForest",
                        RegionName = "ElfLand",
                        ObjectiveType = "Collect",
                        ObjectiveTarget = "White Flower",
                        ObjectiveLocation = "Old Willow Clearing",
                        QuestGiverName = "Elder Rowan",
                        RequiredLevel = 1,
                        TargetAmount = 3,
                        RewardExperience = 120,
                        RewardGold = 80,
                        RewardGems = 0,
                        RewardItemId = potion.ItemId,
                        RewardSkillId = tutorialSkill.SkillId,
                        IsActive = true,
                    },
                    new Quest
                    {
                        Title = "[ELFFOREST] Report To Elder Rowan",
                        Description = "Return to Elder Rowan and report that the first White Flowers have been gathered.",
                        Type = "Main",
                        DefaultStatus = "NotStarted",
                        MapName = "ElfForest",
                        RegionName = "ElfLand",
                        ObjectiveType = "Talk",
                        ObjectiveTarget = "Elder Rowan",
                        ObjectiveLocation = "Elder Rowan's Camp",
                        QuestGiverName = "Elder Rowan",
                        RequiredLevel = 2,
                        TargetAmount = 1,
                        RewardExperience = 90,
                        RewardGold = 120,
                        RewardGems = 0,
                        RewardItemId = null,
                        RewardSkillId = null,
                        IsActive = true,
                    },
                    new Quest
                    {
                        Title = "[ELFFOREST] Equip & Use Your First Skill",
                        Description = "Equip the skill you just learned and use it to defeat five Shadow Sprouts near the forest edge.",
                        Type = "Main",
                        DefaultStatus = "NotStarted",
                        MapName = "ElfForest",
                        RegionName = "ElfLand",
                        ObjectiveType = "EquipSkillAndDefeat",
                        ObjectiveTarget = tutorialSkill.Name,
                        ObjectiveLocation = "Forest Edge",
                        QuestGiverName = "Elder Rowan",
                        RequiredLevel = 3,
                        TargetAmount = 5,
                        RewardExperience = 220,
                        RewardGold = 260,
                        RewardGems = 4,
                        RewardItemId = null,
                        RewardSkillId = null,
                        IsActive = true,
                    }
                };
                _ctx.Quests.AddRange(quests);
                await _ctx.SaveChangesAsync();

                var existingNpcs = await _ctx.NPCs
                    .Where(n => n.MapName == "ElfForest")
                    .ToListAsync();
                if (existingNpcs.Count > 0)
                {
                    var existingNpcIds = existingNpcs.Select(n => n.NPCId).ToList();
                    _ctx.NPCDialogues.RemoveRange(_ctx.NPCDialogues.Where(d => existingNpcIds.Contains(d.NPCId)));
                    await _ctx.SaveChangesAsync();

                    _ctx.NPCs.RemoveRange(existingNpcs);
                    await _ctx.SaveChangesAsync();
                }

                var elderRowan = new NPC
                {
                    Name = "Elder Rowan",
                    Description = "Tutorial elder and main quest giver.",
                    Type = "QuestGiver",
                    MapName = "ElfForest",
                    PositionX = 12.4932,
                    PositionY = 18.61223,
                    InteractionRadius = 2.25f,
                    IsActive = true,
                };

                _ctx.NPCs.Add(elderRowan);
                await _ctx.SaveChangesAsync();

                _ctx.NPCDialogues.AddRange(
                    new NPCDialogue
                    {
                        NPCId = elderRowan.NPCId,
                        Content = "Welcome to ElfLand. I am Elder Rowan, keeper of this clearing. Speak with me whenever you need guidance.",
                        ResponseType = "Dialogue",
                        LinkedQuestId = null,
                        DisplayOrder = 0,
                        IsActive = true,
                    },
                    new NPCDialogue
                    {
                        NPCId = elderRowan.NPCId,
                        Content = "First, let us make sure you can hear the forest. Talk with me and accept your first task.",
                        ResponseType = "Quest",
                        LinkedQuestId = quests[0].QuestId,
                        DisplayOrder = 1,
                        IsActive = true,
                    },
                    new NPCDialogue
                    {
                        NPCId = elderRowan.NPCId,
                        Content = "White Flowers grow near the old willow. Gather three of them, then return here.",
                        ResponseType = "Quest",
                        LinkedQuestId = quests[1].QuestId,
                        DisplayOrder = 2,
                        IsActive = true,
                    },
                    new NPCDialogue
                    {
                        NPCId = elderRowan.NPCId,
                        Content = "Good work. Report back to me so I can record your first lesson as complete.",
                        ResponseType = "Quest",
                        LinkedQuestId = quests[2].QuestId,
                        DisplayOrder = 3,
                        IsActive = true,
                    },
                    new NPCDialogue
                    {
                        NPCId = elderRowan.NPCId,
                        Content = "Before you fight, equip your first skill. A prepared hand survives longer than a brave one.",
                        ResponseType = "Quest",
                        LinkedQuestId = quests[3].QuestId,
                        DisplayOrder = 4,
                        IsActive = true,
                    },
                    new NPCDialogue
                    {
                        NPCId = elderRowan.NPCId,
                        Content = "Now equip that skill and use it against the Shadow Sprouts near the forest edge. Defeat five of them and come back stronger.",
                        ResponseType = "Quest",
                        LinkedQuestId = quests[3].QuestId,
                        DisplayOrder = 5,
                        IsActive = true,
                    }
                );
                await _ctx.SaveChangesAsync();
                if (!await _ctx.DailyLoginRewards.AnyAsync())
                {
                    _ctx.DailyLoginRewards.AddRange(
                        new DailyLoginReward { DayNumber = 1, RewardType = "Gold", RewardValue = 300, IsActive = true },
                        new DailyLoginReward { DayNumber = 2, RewardType = "Energy", RewardValue = 20, IsActive = true },
                        new DailyLoginReward { DayNumber = 3, RewardType = "Item", RewardItemId = potion.ItemId, RewardItemQuantity = 2, IsActive = true },
                        new DailyLoginReward { DayNumber = 4, RewardType = "Gold", RewardValue = 500, IsActive = true },
                        new DailyLoginReward { DayNumber = 5, RewardType = "Gems", RewardValue = 5, IsActive = true },
                        new DailyLoginReward { DayNumber = 6, RewardType = "Energy", RewardValue = 30, IsActive = true },
                        new DailyLoginReward { DayNumber = 7, RewardType = "Gems", RewardValue = 10, IsActive = true }
                    );
                    await _ctx.SaveChangesAsync();
                }

                // 4. Upsert 2 tutorial accounts with inventory.
                async Task<int> CreatePlayer(string username, string email, string displayName, string cls)
                {
                    var account = await _ctx.Accounts
                        .Include(a => a.PlayerProfile)
                        .FirstOrDefaultAsync(a => a.Email == email || a.UserName == username);

                    if (account == null)
                    {
                        account = new Account
                        {
                            CreatedAt = DateTime.UtcNow,
                        };
                        _ctx.Accounts.Add(account);
                    }

                    account.UserName = username;
                    account.Email = email;
                    account.HashPassword = HashPassword("Abc@12345");
                    account.RoleId = 1;
                    account.IsActive = true;
                    account.RefreshToken = null;
                    account.RefreshTokenExpiresAt = null;
                    account.UpdatedAt = DateTime.UtcNow;
                    await _ctx.SaveChangesAsync();

                    var profile = account.PlayerProfile;
                    if (profile == null)
                    {
                        profile = new PlayerProfile
                        {
                            AccountId = account.AccountId,
                            CreatedAt = DateTime.UtcNow,
                        };
                        _ctx.PlayerProfiles.Add(profile);
                        await _ctx.SaveChangesAsync();
                    }

                    int pid = profile.PlayerProfileId;

                    _ctx.InventoryItems.RemoveRange(_ctx.InventoryItems.Where(x => x.PlayerProfileId == pid));
                    _ctx.PlayerSkins.RemoveRange(_ctx.PlayerSkins.Where(x => x.PlayerProfileId == pid));
                    _ctx.PlayerQuests.RemoveRange(_ctx.PlayerQuests.Where(x => x.PlayerProfileId == pid));
                    _ctx.PlayerChests.RemoveRange(_ctx.PlayerChests.Where(x => x.PlayerProfileId == pid));
                    _ctx.PlayerDailyLogins.RemoveRange(_ctx.PlayerDailyLogins.Where(x => x.PlayerProfileId == pid));
                    _ctx.PlayerStats.RemoveRange(_ctx.PlayerStats.Where(x => x.PlayerProfileId == pid));
                    _ctx.PlayerStatsSnapshots.RemoveRange(_ctx.PlayerStatsSnapshots.Where(x => x.PlayerProfileId == pid));
                    _ctx.PlayerSkills.RemoveRange(_ctx.PlayerSkills.Where(x => x.PlayerProfileId == pid));
                    _ctx.PlayerAchievements.RemoveRange(_ctx.PlayerAchievements.Where(x => x.PlayerProfileId == pid));
                    _ctx.PlayerAnnouncements.RemoveRange(_ctx.PlayerAnnouncements.Where(x => x.PlayerProfileId == pid));
                    _ctx.PlayerCurrencyLogs.RemoveRange(_ctx.PlayerCurrencyLogs.Where(x => x.PlayerProfileId == pid));
                    _ctx.PurchaseHistories.RemoveRange(_ctx.PurchaseHistories.Where(x => x.PlayerProfileId == pid));
                    _ctx.GachaPullHistories.RemoveRange(_ctx.GachaPullHistories.Where(x => x.PlayerProfileId == pid));
                    _ctx.Mails.RemoveRange(_ctx.Mails.Where(x => x.PlayerProfileId == pid));
                    _ctx.GuildMembers.RemoveRange(_ctx.GuildMembers.Where(x => x.PlayerProfileId == pid));
                    await _ctx.SaveChangesAsync();

                    profile.DisplayName = displayName;
                    profile.Class = cls;
                    profile.Level = 1;
                    profile.ExperiencePoints = 0;
                    profile.Gold = 100;
                    profile.Gems = 10;
                    profile.Energy = 100;
                    profile.LastMapName = "ElfForest";
                    profile.PositionX = 11.9;
                    profile.PositionY = 17.8;
                    profile.AvatarUrl = string.Empty;
                    profile.UpdatedAt = DateTime.UtcNow;
                    await _ctx.SaveChangesAsync();

                    // player stats
                    _ctx.PlayerStats.Add(new PlayerStat
                    {
                        PlayerProfileId = pid,
                        CurrentHp = 200,
                        MaxHp = 200,
                        Atk = 25,
                        Def = 10,
                        MoveSpeed = 50,
                        AttackSpeed = 10,
                        CritRate = 50,
                        CritDamage = 150,
                    });
                    await _ctx.SaveChangesAsync();

                    // inventory: sword (equipped), armor (equipped or in bag), potion x3, optional extra potion
                    _ctx.InventoryItems.Add(new InventoryItem
                    {
                        PlayerProfileId = pid,
                        ItemId = sword.ItemId,
                        Quantity = 1,
                        IsEquipped = true,
                        IsSkin = false,
                        EquippedSlot = "Weapon",
                    });
                    _ctx.InventoryItems.Add(new InventoryItem
                    {
                        PlayerProfileId = pid,
                        ItemId = armor.ItemId,
                        Quantity = 1,
                        IsEquipped = true,
                        IsSkin = false,
                        EquippedSlot = "Armor",
                    });
                    _ctx.InventoryItems.Add(new InventoryItem
                    {
                        PlayerProfileId = pid,
                        ItemId = potion.ItemId,
                        Quantity = 3,
                        IsEquipped = false,
                        IsSkin = false,
                    });

                    // player skin: default equipped + one extra skin in bag
                    _ctx.PlayerSkins.Add(new PlayerSkin
                    {
                        PlayerProfileId = pid,
                        SkinId = skinDefault.SkinId,
                        IsEquipped = true,
                        UnlockedAt = DateTime.UtcNow,
                    });
                    _ctx.PlayerSkins.Add(new PlayerSkin
                    {
                        PlayerProfileId = pid,
                        SkinId = skinAlt.SkinId,
                        IsEquipped = false,
                        UnlockedAt = DateTime.UtcNow,
                    });

                    await _ctx.SaveChangesAsync();

                    // assign quests
                    var elfQuests = await _ctx.Quests
                        .Where(q => EF.Functions.Like(q.Title, "[ELFFOREST]%") && q.RequiredLevel <= profile.Level)
                        .ToListAsync();
                    foreach (var q in elfQuests)
                    {
                        _ctx.PlayerQuests.Add(new PlayerQuest
                        {
                            PlayerProfileId = pid,
                            QuestId = q.QuestId,
                            Status = "NotStarted",
                            TargetValue = q.TargetAmount,
                            AcceptedAt = DateTime.UtcNow,
                        });
                    }
                    await _ctx.SaveChangesAsync();

                    return pid;
                }

                var p1 = await CreatePlayer("elf_user1", "elf1@mystic.test", "Tutorial Knight 1", "Knight");
                var p2 = await CreatePlayer("elf_user2", "elf2@mystic.test", "Tutorial Knight 2", "Knight");

                await tx.CommitAsync();

                return Ok(new ApiResponse<object> { Success = true, Message = "Seed ElfForest completed", Data = new { players = new[] { p1, p2 } } });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return StatusCode(500, new ApiResponse<object> { Success = false, Message = ex.Message, ErrorCode = ErrorCodes.InternalError });
            }
        }

        // ─────────────────────────────────────────────────────────────────────────
        // DELETE /api/seed/inventory → Xoá toàn bộ seed data
        // ─────────────────────────────────────────────────────────────────────────
        [HttpDelete("inventory")]
        public async Task<IActionResult> DeleteSeedData()
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                const string TEST_EMAIL = "testplayer@mystic.test";

                var acc = await _ctx.Accounts
                    .Include(a => a.PlayerProfile)
                    .FirstOrDefaultAsync(a => a.Email == TEST_EMAIL);

                if (acc?.PlayerProfile != null)
                {
                    int pid = acc.PlayerProfile.PlayerProfileId;
                    _ctx.InventoryItems.RemoveRange(      _ctx.InventoryItems.Where(x => x.PlayerProfileId == pid));
                    _ctx.PlayerSkins.RemoveRange(         _ctx.PlayerSkins.Where(x => x.PlayerProfileId == pid));
                    _ctx.PlayerQuests.RemoveRange(        _ctx.PlayerQuests.Where(x => x.PlayerProfileId == pid));
                    _ctx.PlayerStats.RemoveRange(         _ctx.PlayerStats.Where(x => x.PlayerProfileId == pid));
                    _ctx.PlayerStatsSnapshots.RemoveRange(_ctx.PlayerStatsSnapshots.Where(x => x.PlayerProfileId == pid));
                    await _ctx.SaveChangesAsync();
                    _ctx.PlayerProfiles.Remove(acc.PlayerProfile);
                }
                if (acc != null) _ctx.Accounts.Remove(acc);

                var seedItems = await _ctx.Items.Where(i => EF.Functions.Like(i.Name, "[SEED]%")).ToListAsync();
                _ctx.Items.RemoveRange(seedItems);

                var seedSkins = await _ctx.Skins.Where(s => EF.Functions.Like(s.Name, "[SEED]%")).ToListAsync();
                _ctx.Skins.RemoveRange(seedSkins);

                var seedQuests = await _ctx.Quests.Where(q => EF.Functions.Like(q.Title, "[SEED]%")).ToListAsync();
                _ctx.Quests.RemoveRange(seedQuests);

                await _ctx.SaveChangesAsync();
                await tx.CommitAsync();

                return Ok(new ApiResponse<object> { Success = true, Message = "Xoá seed data thành công." });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return StatusCode(500, new ApiResponse<object> { Success = false, Message = ex.Message, ErrorCode = ErrorCodes.InternalError });
            }
        }

        private async Task EnsureElfForestSchema()
        {
            await _ctx.Database.ExecuteSqlRawAsync(@"
ALTER TABLE ""Quests"" ADD COLUMN IF NOT EXISTS ""TargetAmount"" integer NOT NULL DEFAULT 1;
ALTER TABLE ""Quests"" ADD COLUMN IF NOT EXISTS ""MapName"" character varying(100) NOT NULL DEFAULT 'ElfForest';
ALTER TABLE ""Quests"" ADD COLUMN IF NOT EXISTS ""RegionName"" character varying(100) NULL;
ALTER TABLE ""Quests"" ADD COLUMN IF NOT EXISTS ""ObjectiveType"" text NOT NULL DEFAULT 'Explore';
ALTER TABLE ""Quests"" ADD COLUMN IF NOT EXISTS ""ObjectiveTarget"" text NULL;
ALTER TABLE ""Quests"" ADD COLUMN IF NOT EXISTS ""ObjectiveLocation"" text NULL;
ALTER TABLE ""Quests"" ADD COLUMN IF NOT EXISTS ""QuestGiverName"" text NULL;
ALTER TABLE ""Quests"" ADD COLUMN IF NOT EXISTS ""RewardSkillId"" integer NULL;

CREATE TABLE IF NOT EXISTS ""NPCs"" (
    ""NPCId"" integer GENERATED BY DEFAULT AS IDENTITY,
    ""Name"" character varying(150) NOT NULL,
    ""Description"" text NULL,
    ""Type"" text NOT NULL,
    ""MapName"" character varying(100) NOT NULL,
    ""PositionX"" double precision NOT NULL,
    ""PositionY"" double precision NOT NULL,
    ""InteractionRadius"" real NOT NULL,
    ""IconUrl"" text NULL,
    ""IsActive"" boolean NOT NULL,
    CONSTRAINT ""PK_NPCs"" PRIMARY KEY (""NPCId"")
);

CREATE TABLE IF NOT EXISTS ""NPCDialogues"" (
    ""NPCDialogueId"" integer GENERATED BY DEFAULT AS IDENTITY,
    ""NPCId"" integer NOT NULL,
    ""Content"" text NOT NULL,
    ""ResponseType"" text NOT NULL,
    ""LinkedQuestId"" integer NULL,
    ""LinkedShopItemId"" integer NULL,
    ""DisplayOrder"" integer NOT NULL,
    ""IsActive"" boolean NOT NULL,
    CONSTRAINT ""PK_NPCDialogues"" PRIMARY KEY (""NPCDialogueId"")
);

DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'FK_NPCDialogues_NPCs_NPCId') THEN
        ALTER TABLE ""NPCDialogues"" ADD CONSTRAINT ""FK_NPCDialogues_NPCs_NPCId""
            FOREIGN KEY (""NPCId"") REFERENCES ""NPCs"" (""NPCId"") ON DELETE CASCADE;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'FK_NPCDialogues_Quests_LinkedQuestId') THEN
        ALTER TABLE ""NPCDialogues"" ADD CONSTRAINT ""FK_NPCDialogues_Quests_LinkedQuestId""
            FOREIGN KEY (""LinkedQuestId"") REFERENCES ""Quests"" (""QuestId"") ON DELETE SET NULL;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'FK_NPCDialogues_ShopItems_LinkedShopItemId') THEN
        ALTER TABLE ""NPCDialogues"" ADD CONSTRAINT ""FK_NPCDialogues_ShopItems_LinkedShopItemId""
            FOREIGN KEY (""LinkedShopItemId"") REFERENCES ""ShopItems"" (""ShopItemId"") ON DELETE SET NULL;
    END IF;
END $$;

CREATE INDEX IF NOT EXISTS ""IX_Quests_MapName"" ON ""Quests"" (""MapName"");
CREATE INDEX IF NOT EXISTS ""IX_NPCs_MapName"" ON ""NPCs"" (""MapName"");
CREATE INDEX IF NOT EXISTS ""IX_NPCDialogues_NPCId"" ON ""NPCDialogues"" (""NPCId"");
CREATE INDEX IF NOT EXISTS ""IX_NPCDialogues_LinkedQuestId"" ON ""NPCDialogues"" (""LinkedQuestId"");
CREATE INDEX IF NOT EXISTS ""IX_NPCDialogues_LinkedShopItemId"" ON ""NPCDialogues"" (""LinkedShopItemId"");
");
        }
        // ─────────────────────────────────────────────────────────────────────────
        private static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}

