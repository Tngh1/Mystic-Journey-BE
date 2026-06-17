using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Mystic_Journey_API.Controllers
{
    // =============================================================================
    // SeedController – Seeder dữ liệu mẫu cho UC 20 (Inventory)
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
    //     Level 3, Class=Knight, mặc định skin Knight
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
                    Level        = 3,
                    ExperiencePoints = 280,
                    Gold         = 1500,
                    Gems         = 200,
                    Energy       = 100,
                    LastMapName  = "ElfForest",
                    PositionX    = 0,
                    PositionY    = 0,
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

                return Ok(new
                {
                    success = true,
                    message = "Seed thành công!",
                    data = new
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
                return StatusCode(500, new { error = ex.Message, detail = ex.InnerException?.Message });
            }
        }

        // ─────────────────────────────────────────────────────────────────────────
        // POST /api/seed/elfforest → Seed 2 users + 15 quests on map ElfForest
        // ─────────────────────────────────────────────────────────────────────────
        [HttpPost("elfforest")]
        public async Task<IActionResult> SeedElfForest()
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                // 1. Create 4 items (weapon, armor, potion, skin item)
                var existing = await _ctx.Items.Where(i => EF.Functions.Like(i.Name, "[ELF]%")).ToListAsync();
                _ctx.Items.RemoveRange(existing);
                await _ctx.SaveChangesAsync();

                var potion = new Item
                {
                    Name = "[ELF] Health Potion",
                    Description = "Hồi phục 150 HP.",
                    Type = "Consumable",
                    Rarity = "Common",
                    Slot = "None",
                    BaseValue = 30,
                    MaxStack = 99,
                    IsActive = true,
                };
                var sword = new Item
                {
                    Name = "[ELF] Short Sword",
                    Description = "Kiếm ngắn dùng cho du kích rừng.",
                    Type = "Weapon",
                    Rarity = "Uncommon",
                    Slot = "Weapon",
                    BaseValue = 200,
                    MaxStack = 1,
                    IsActive = true,
                };
                var armor = new Item
                {
                    Name = "[ELF] Leather Armor",
                    Description = "Áo da nhẹ, tăng chút phòng thủ.",
                    Type = "Armor",
                    Rarity = "Common",
                    Slot = "Armor",
                    BaseValue = 180,
                    MaxStack = 1,
                    IsActive = true,
                };

                _ctx.Items.AddRange(potion, sword, armor);
                await _ctx.SaveChangesAsync();

                // equipment stats
                _ctx.EquipmentStats.Add(new EquipmentStats
                {
                    ItemId = sword.ItemId,
                    BaseHp = 0,
                    BaseAtk = 30,
                    BaseDef = 0,
                    BonusHp = 0,
                    BonusAtk = 5,
                    BonusDef = 0,
                });
                _ctx.EquipmentStats.Add(new EquipmentStats
                {
                    ItemId = armor.ItemId,
                    BaseHp = 50,
                    BaseAtk = 0,
                    BaseDef = 12,
                    BonusHp = 10,
                    BonusAtk = 0,
                    BonusDef = 2,
                });
                await _ctx.SaveChangesAsync();

                // 2. Skins
                var existingSkins = await _ctx.Skins.Where(s => EF.Functions.Like(s.Name, "[ELF]%")).ToListAsync();
                _ctx.Skins.RemoveRange(existingSkins);
                await _ctx.SaveChangesAsync();

                var skinDefault = new Skin
                {
                    Name = "[ELF] ElfForest Default",
                    Description = "Skin mặc định khu rừng Elf.",
                    Type = "FullSet",
                    Rarity = "Common",
                    IsForSale = false,
                    IsActive = true,
                };
                var skinAlt = new Skin
                {
                    Name = "[ELF] Ranger Cloak",
                    Description = "Áo choàng của cung thủ rừng.",
                    Type = "Cloak",
                    Rarity = "Rare",
                    IsForSale = false,
                    IsActive = true,
                };
                _ctx.Skins.AddRange(skinDefault, skinAlt);
                await _ctx.SaveChangesAsync();

                // 3. Quests ElfForest 1-15
                var existingQuests = await _ctx.Quests.Where(q => EF.Functions.Like(q.Title, "[ELFFOREST]%")).ToListAsync();
                _ctx.Quests.RemoveRange(existingQuests);
                await _ctx.SaveChangesAsync();

                var elfQuestConfigs = new (int TargetAmount, decimal RewardGold, int RewardExperience, decimal RewardGems)[]
                {
                    (5, 500, 200, 0),
                    (3, 300, 150, 0),
                    (10, 800, 400, 5),
                    (1, 200, 100, 0),
                    (8, 600, 300, 0),
                    (5, 500, 250, 0),
                    (3, 400, 200, 5),
                    (12, 1000, 500, 10),
                    (1, 300, 150, 0),
                    (6, 700, 350, 5),
                    (4, 500, 250, 0),
                    (8, 800, 400, 0),
                    (2, 300, 150, 0),
                    (5, 600, 300, 5),
                    (1, 1500, 800, 20),
                };

                var quests = new List<Quest>();
                var elfQuestObjectives = new (string ObjectiveType, string ObjectiveTarget, string ObjectiveLocation, string QuestGiverName)[]
                {
                    ("Defeat", "Forest Slime", "Mossy Gate", "Elder Rowan"),
                    ("Collect", "Moonleaf", "Moonwell Grove", "Healer Lyria"),
                    ("Defeat", "Wild Treant", "Ancient Roots", "Scout Elian"),
                    ("Talk", "Merchant Mira", "Village Market", "Elder Rowan"),
                    ("Interact", "Ancient Totem", "Old Shrine", "Scout Elian"),
                    ("Defeat", "Wolf Pack", "North Trail", "Scout Elian"),
                    ("Collect", "Crystal Dew", "Silver Pond", "Healer Lyria"),
                    ("Defeat", "Forest Guardian", "Heartwood Clearing", "Elder Rowan"),
                    ("OpenChest", "Random Chest", "ElfForest", "Merchant Mira"),
                    ("Interact", "Lost Footprints", "Old Mine Road", "Scout Elian"),
                    ("Collect", "Broken Charm", "Abandoned Camp", "Elder Rowan"),
                    ("Defeat", "Dark Sprout", "Shadow Thicket", "Healer Lyria"),
                    ("Talk", "Scout Elian", "Watch Post", "Elder Rowan"),
                    ("Interact", "Sealed Root", "Ancient Roots", "Elder Rowan"),
                    ("Defeat", "Corrupted Ancient", "Heartwood Clearing", "Elder Rowan"),
                };
                for (int i = 1; i <= 15; i++)
                {
                    var config = elfQuestConfigs[i - 1];
                    var objective = elfQuestObjectives[i - 1];
                    quests.Add(new Quest
                    {
                        Title = $"[ELFFOREST] ElfForest - Quest {i}",
                        Description = $"ElfForest quest stage {i}.",
                        Type = "Main",
                        DefaultStatus = "NotStarted",
                        MapName = "ElfForest",
                        RegionName = "Greenwood",
                        ObjectiveType = objective.ObjectiveType,
                        ObjectiveTarget = objective.ObjectiveTarget,
                        ObjectiveLocation = objective.ObjectiveLocation,
                        QuestGiverName = objective.QuestGiverName,
                        RequiredLevel = Math.Max(1, i),
                        TargetAmount = config.TargetAmount,
                        RewardExperience = config.RewardExperience,
                        RewardGold = config.RewardGold,
                        RewardGems = config.RewardGems,
                        IsActive = true,
                    });
                }
                _ctx.Quests.AddRange(quests);
                await _ctx.SaveChangesAsync();

                var existingNpcs = await _ctx.NPCs
                    .Where(n => n.MapName == "ElfForest")
                    .ToListAsync();
                _ctx.NPCs.RemoveRange(existingNpcs);
                await _ctx.SaveChangesAsync();

                var elderRowan = new NPC
                {
                    Name = "Elder Rowan",
                    Description = "Village elder and main quest giver.",
                    Type = "QuestGiver",
                    MapName = "ElfForest",
                    PositionX = -2.5,
                    PositionY = 1.25,
                    InteractionRadius = 2.25f,
                    IsActive = true,
                };
                var healerLyria = new NPC
                {
                    Name = "Healer Lyria",
                    Description = "Forest healer who helps adventurers recover.",
                    Type = "Healer",
                    MapName = "ElfForest",
                    PositionX = 3.75,
                    PositionY = 1.5,
                    InteractionRadius = 2f,
                    IsActive = true,
                };
                var merchantMira = new NPC
                {
                    Name = "Merchant Mira",
                    Description = "Village merchant with supplies and rumors.",
                    Type = "Merchant",
                    MapName = "ElfForest",
                    PositionX = 1.5,
                    PositionY = -3.25,
                    InteractionRadius = 2f,
                    IsActive = true,
                };
                var scoutElian = new NPC
                {
                    Name = "Scout Elian",
                    Description = "Scout watching the dangerous forest paths.",
                    Type = "Information",
                    MapName = "ElfForest",
                    PositionX = -5.25,
                    PositionY = -2.75,
                    InteractionRadius = 2f,
                    IsActive = true,
                };

                _ctx.NPCs.AddRange(elderRowan, healerLyria, merchantMira, scoutElian);
                await _ctx.SaveChangesAsync();

                _ctx.NPCDialogues.AddRange(
                    new NPCDialogue
                    {
                        NPCId = elderRowan.NPCId,
                        Content = "The forest is restless. Start with the creatures near the Mossy Gate.",
                        ResponseType = "Quest",
                        LinkedQuestId = quests[0].QuestId,
                        DisplayOrder = 1,
                        IsActive = true,
                    },
                    new NPCDialogue
                    {
                        NPCId = healerLyria.NPCId,
                        Content = "Bring me Moonleaf from the grove and I can prepare medicine.",
                        ResponseType = "Quest",
                        LinkedQuestId = quests[1].QuestId,
                        DisplayOrder = 1,
                        IsActive = true,
                    },
                    new NPCDialogue
                    {
                        NPCId = merchantMira.NPCId,
                        Content = "I can trade supplies, and sometimes random chests turn up after fights.",
                        ResponseType = "Shop",
                        LinkedQuestId = quests[8].QuestId,
                        DisplayOrder = 1,
                        IsActive = true,
                    },
                    new NPCDialogue
                    {
                        NPCId = scoutElian.NPCId,
                        Content = "I found fresh tracks toward the Ancient Roots. Stay sharp.",
                        ResponseType = "Quest",
                        LinkedQuestId = quests[2].QuestId,
                        DisplayOrder = 1,
                        IsActive = true,
                    },
                    new NPCDialogue
                    {
                        NPCId = scoutElian.NPCId,
                        Content = "The old mine road is unsafe, but it may reveal what corrupted the forest.",
                        ResponseType = "Quest",
                        LinkedQuestId = quests[9].QuestId,
                        DisplayOrder = 2,
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

                // 4. Create 2 accounts with inventory
                async Task<int> CreatePlayer(string username, string email, string displayName, string cls)
                {
                    // remove existing
                    var acc = await _ctx.Accounts.Include(a => a.PlayerProfile).FirstOrDefaultAsync(a => a.Email == email);
                    if (acc != null)
                    {
                        var pp = acc.PlayerProfile;
                        if (pp != null)
                        {
                            _ctx.InventoryItems.RemoveRange(_ctx.InventoryItems.Where(x => x.PlayerProfileId == pp.PlayerProfileId));
                            _ctx.PlayerSkins.RemoveRange(_ctx.PlayerSkins.Where(x => x.PlayerProfileId == pp.PlayerProfileId));
                            _ctx.PlayerQuests.RemoveRange(_ctx.PlayerQuests.Where(x => x.PlayerProfileId == pp.PlayerProfileId));
                            _ctx.PlayerChests.RemoveRange(_ctx.PlayerChests.Where(x => x.PlayerProfileId == pp.PlayerProfileId));
                            _ctx.PlayerDailyLogins.RemoveRange(_ctx.PlayerDailyLogins.Where(x => x.PlayerProfileId == pp.PlayerProfileId));
                            _ctx.PlayerStats.RemoveRange(_ctx.PlayerStats.Where(x => x.PlayerProfileId == pp.PlayerProfileId));
                            _ctx.PlayerStatsSnapshots.RemoveRange(_ctx.PlayerStatsSnapshots.Where(x => x.PlayerProfileId == pp.PlayerProfileId));
                            await _ctx.SaveChangesAsync();
                            _ctx.PlayerProfiles.Remove(pp);
                        }
                        _ctx.Accounts.Remove(acc);
                        await _ctx.SaveChangesAsync();
                    }

                    var account = new Account
                    {
                        UserName = username,
                        Email = email,
                        HashPassword = HashPassword("Abc@12345"),
                        RoleId = 1,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                    };
                    _ctx.Accounts.Add(account);
                    await _ctx.SaveChangesAsync();

                    var profile = new PlayerProfile
                    {
                        AccountId = account.AccountId,
                        DisplayName = displayName,
                        Class = cls,
                        Level = 2,
                        ExperiencePoints = 0,
                        Gold = 100,
                        Gems = 10,
                        Energy = 100,
                        LastMapName = "ElfForest",
                        PositionX = 0,
                        PositionY = -1,
                    };
                    _ctx.PlayerProfiles.Add(profile);
                    await _ctx.SaveChangesAsync();

                    int pid = profile.PlayerProfileId;

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
                    var elfQuests = await _ctx.Quests.Where(q => EF.Functions.Like(q.Title, "[ELFFOREST]%")).ToListAsync();
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

                var p1 = await CreatePlayer("elf_user1", "elf1@mystic.test", "Elf Ranger 1", "Archer");
                var p2 = await CreatePlayer("elf_user2", "elf2@mystic.test", "Elf Ranger 2", "Archer");

                await tx.CommitAsync();

                return Ok(new { success = true, message = "Seed ElfForest completed", players = new[] { p1, p2 } });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return StatusCode(500, new { error = ex.Message, detail = ex.InnerException?.Message });
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

                return Ok(new { success = true, message = "Xoá seed data thành công." });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // ─────────────────────────────────────────────────────────────────────────
        private static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
