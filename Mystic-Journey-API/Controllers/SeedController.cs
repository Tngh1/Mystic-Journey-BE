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
        // ─────────────────────────────────────────────────────────────────────────
        // POST /api/seed/inventory  → Seed toàn bộ dữ liệu mẫu UC 20
        // Dùng item từ system migration (không tạo mới item trong seed)
        // ─────────────────────────────────────────────────────────────────────────
        [HttpPost("inventory")]
        public async Task<IActionResult> SeedInventory()
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                // ── 1. Lookup system items (từ migration, không tự tạo) ──────────
                var potion = await _ctx.Items.FirstOrDefaultAsync(i => i.Name == "Small Health Potion");
                var sword  = await _ctx.Items.FirstOrDefaultAsync(i => i.Name == "Iron Sword");
                var helm   = await _ctx.Items.FirstOrDefaultAsync(i => i.Name == "Iron Helmet");

                if (potion == null || sword == null || helm == null)
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "System items chưa có trong DB. Hãy chạy migration trước: 'Small Health Potion', 'Iron Sword', 'Iron Helmet'."
                    });

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

                // ── 4. Account + PlayerProfile test ──────────────────────────
                const string TEST_EMAIL    = "testplayer@mystic.test";
                const string TEST_USERNAME = "testplayer";

                // Clean up any existing friend relationships first
                _ctx.Friends.RemoveRange(_ctx.Friends);
                await _ctx.SaveChangesAsync();

                // Xoá friend accounts nếu đã tồn tại
                var friendEmails = new[] { "friend1@mystic.test", "friend2@mystic.test", "friend3@mystic.test" };
                var existingFriends = await _ctx.Accounts
                    .Include(a => a.PlayerProfile)
                    .Where(a => friendEmails.Contains(a.Email))
                    .ToListAsync();
                foreach (var acc in existingFriends)
                {
                    var pp = acc.PlayerProfile;
                    if (pp != null)
                    {
                        _ctx.InventoryItems.RemoveRange(_ctx.InventoryItems.Where(x => x.PlayerProfileId == pp.PlayerProfileId));
                        _ctx.PlayerSkins.RemoveRange(_ctx.PlayerSkins.Where(x => x.PlayerProfileId == pp.PlayerProfileId));
                        _ctx.PlayerQuests.RemoveRange(_ctx.PlayerQuests.Where(x => x.PlayerProfileId == pp.PlayerProfileId));
                        _ctx.PlayerStats.RemoveRange(_ctx.PlayerStats.Where(x => x.PlayerProfileId == pp.PlayerProfileId));
                        _ctx.PlayerStatsSnapshots.RemoveRange(_ctx.PlayerStatsSnapshots.Where(x => x.PlayerProfileId == pp.PlayerProfileId));
                        await _ctx.SaveChangesAsync();
                        _ctx.PlayerProfiles.Remove(pp);
                    }
                    _ctx.Accounts.Remove(acc);
                    await _ctx.SaveChangesAsync();
                }

                // Xoá nếu đã tồn tại testplayer
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
                    CurrentEnergy = 100,
                    MaxEnergy    = 100,
                    LastEnergyUpdateTime = DateTime.UtcNow,
                    LastMapName  = "ElfForest",
                    PositionX    = 11.9,
                    PositionY    = 17.8,
                    AvatarUrl    = "",
                };
                _ctx.PlayerProfiles.Add(profile);
                await _ctx.SaveChangesAsync();

                int pid = profile.PlayerProfileId;
                var gachaSeed = await SeedGachaBaseDataAsync(TEST_EMAIL, 11);

                // Tạo friend accounts
                 var f1Account = new Account { UserName = "friend1", Email = "friend1@mystic.test", HashPassword = HashPassword("Abc@12345"), RoleId = 1, IsActive = true, CreatedAt = DateTime.UtcNow };
                 var f2Account = new Account { UserName = "friend2", Email = "friend2@mystic.test", HashPassword = HashPassword("Abc@12345"), RoleId = 1, IsActive = true, CreatedAt = DateTime.UtcNow };
                 var f3Account = new Account { UserName = "friend3", Email = "friend3@mystic.test", HashPassword = HashPassword("Abc@12345"), RoleId = 1, IsActive = true, CreatedAt = DateTime.UtcNow };
                 _ctx.Accounts.AddRange(f1Account, f2Account, f3Account);
                 await _ctx.SaveChangesAsync();

                 // Tạo friend profiles
                 var f1Profile = new PlayerProfile { AccountId = f1Account.AccountId, DisplayName = "Arthur", Class = "Knight", Level = 10, Gold = 1000, Gems = 100, CurrentEnergy = 100, MaxEnergy = 100, LastEnergyUpdateTime = DateTime.UtcNow, LastMapName = "ElfForest", PositionX = 0, PositionY = 0, AvatarUrl = "" };
                 var f2Profile = new PlayerProfile { AccountId = f2Account.AccountId, DisplayName = "Gwen", Class = "Archer", Level = 12, Gold = 1200, Gems = 150, CurrentEnergy = 100, MaxEnergy = 100, LastEnergyUpdateTime = DateTime.UtcNow, LastMapName = "ElfForest", PositionX = 0, PositionY = 0, AvatarUrl = "" };
                 var f3Profile = new PlayerProfile { AccountId = f3Account.AccountId, DisplayName = "Merlin", Class = "Mage", Level = 15, Gold = 2000, Gems = 200, CurrentEnergy = 100, MaxEnergy = 100, LastEnergyUpdateTime = DateTime.UtcNow, LastMapName = "ElfForest", PositionX = 0, PositionY = 0, AvatarUrl = "" };
                 _ctx.PlayerProfiles.AddRange(f1Profile, f2Profile, f3Profile);
                 await _ctx.SaveChangesAsync();

                 // Thiết lập quan hệ bạn bè (Accepted) với testplayer
                 _ctx.Friends.AddRange(
                     new Friend { RequesterId = pid, AddresseeId = f1Profile.PlayerProfileId, Status = "Accepted", CreatedAt = DateTime.UtcNow },
                     new Friend { RequesterId = pid, AddresseeId = f2Profile.PlayerProfileId, Status = "Accepted", CreatedAt = DateTime.UtcNow },
                     new Friend { RequesterId = f3Profile.PlayerProfileId, AddresseeId = pid, Status = "Accepted", CreatedAt = DateTime.UtcNow }
                 );
                 await _ctx.SaveChangesAsync();

                 // Tạo PlayerStats (base stats lv 3)
                _ctx.PlayerStats.Add(new PlayerStat
                {
                    PlayerProfileId = pid,
                    CurrentHp = 350, MaxHp = 350,
                    Atk = 45, Def = 20,
                    MoveSpeed = 50, AttackSpeed = 100,
                    CritRate = 50, CritDamage = 150,
                    DamageBonus = 0,
                    SkillPoints = 3,
                    TotalWins = 5, TotalLosses = 2, TotalKills = 23, TotalDeaths = 4
                });
                await _ctx.SaveChangesAsync();

                // ── 5. Inventory của player test (dùng system items) ──────────
                // 5a. Iron Helmet – EQUIPPED (Helmet)
                if (helm != null)
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

                // 5b. Small Health Potion – 2 bình (trong túi)
                if (potion != null)
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
                        level           = 1,
                        playerClass     = "Knight",
                        items = new[]
                        {
                            new { name = helm?.Name   ?? "(không tìm thấy)", type = "Armor (Helmet)", status = "EQUIPPED" },
                            new { name = potion?.Name ?? "(không tìm thấy)", type = "Consumable",     status = "BAG x2"  },
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
                        systemItemsUsed = new[]
                        {
                            new { name = potion?.Name ?? "(null)", itemId = potion?.ItemId },
                            new { name = sword?.Name  ?? "(null)", itemId = sword?.ItemId  },
                            new { name = helm?.Name   ?? "(null)", itemId = helm?.ItemId   },
                        },
                        gacha = new
                        {
                            bannerId     = gachaSeed.BannerId,
                            ticketItemId = gachaSeed.TicketItemId,
                            ticketCount  = 11,
                            targetEmail  = TEST_EMAIL,
                        },
                    }
                });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                // Return full exception details for debugging (includes inner exception)
                return StatusCode(500, new ApiResponse<object> { Success = false, Message = ex.ToString(), ErrorCode = ErrorCodes.InternalError });
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
                return StatusCode(500, new ApiResponse<object> { Success = false, Message = ex.ToString(), ErrorCode = ErrorCodes.InternalError });
            }
        }

        // ─────────────────────────────────────────────────────────────────────────
        // POST /api/seed/achievements  → Upsert mẫu Achievements
        // Dùng để tạo các danh hiệu (Achievements) test
        // ─────────────────────────────────────────────────────────────────────────
        [HttpPost("achievements")]
        public async Task<IActionResult> SeedAchievements()
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                var achievementList = new List<Achievement>
                {
                    new Achievement { Name = "Pioneer", Description = "Complete the first chapter", BuffDescription = "+2% Max HP", RequiredValue = 1, Type = "Progression", IconUrl = "pioneer" },
                    new Achievement { Name = "Monster Hunter", Description = "Defeat 1,000 monsters", BuffDescription = "+2% Attack", RequiredValue = 1000, Type = "Combat", IconUrl = "monster_hunter" },
                    new Achievement { Name = "Deadeye", Description = "Reach the required cumulative Critical Rate", BuffDescription = "+2% Critical Rate", RequiredValue = 100, Type = "Progression", IconUrl = "deadeye" },
                    new Achievement { Name = "The Unyielding", Description = "Die fewer than 10 times before Level 30", BuffDescription = "+3% Defense", RequiredValue = 1, Type = "Progression", IconUrl = "unyielding" },
                    new Achievement { Name = "Swift Wanderer", Description = "Explore every region on the map", BuffDescription = "+3% Movement Speed", RequiredValue = 1, Type = "Exploration", IconUrl = "swift_wanderer" },
                    new Achievement { Name = "Treasure Seeker", Description = "Open 500 treasure chests", BuffDescription = "+5% Gold Gain", RequiredValue = 500, Type = "Collection", IconUrl = "treasure_seeker" },
                    new Achievement { Name = "Adventurer", Description = "Complete 100 quests", BuffDescription = "+3% EXP Gain", RequiredValue = 100, Type = "Progression", IconUrl = "adventurer" },
                    new Achievement { Name = "Faithful Companion", Description = "Complete 100 co-op dungeons", BuffDescription = "+2% Max HP, +2% Defense", RequiredValue = 100, Type = "Social", IconUrl = "faithful_companion" },
                    new Achievement { Name = "Conqueror", Description = "Defeat every Boss at least once", BuffDescription = "+3% Damage to Bosses", RequiredValue = 1, Type = "Combat", IconUrl = "conqueror" },
                    new Achievement { Name = "Legend of Elarion", Description = "Reach the maximum level and complete the main storyline", BuffDescription = "+2% to All Stats (HP, ATK, DEF)", RequiredValue = 1, Type = "Progression", IconUrl = "legend_elarion" }
                };

                foreach (var a in achievementList)
                {
                    var existing = await _ctx.Achievements.FirstOrDefaultAsync(x => x.Name == a.Name);
                    if (existing == null)
                    {
                        _ctx.Achievements.Add(a);
                    }
                    else
                    {
                        existing.Description = a.Description;
                        existing.BuffDescription = a.BuffDescription;
                        existing.IconUrl = a.IconUrl;
                        existing.RequiredValue = a.RequiredValue;
                        existing.Type = a.Type;
                        existing.IsActive = a.IsActive;
                    }
                }

                await _ctx.SaveChangesAsync();
                await tx.CommitAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Seed achievements thành công",
                    Data = new { count = achievementList.Count }
                });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return StatusCode(500, new ApiResponse<object> { Success = false, Message = ex.ToString(), ErrorCode = ErrorCodes.InternalError });
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

                // 1. Lookup system items từ migration (không tạo item mới trong seed)
                var potion      = await _ctx.Items.FirstOrDefaultAsync(i => i.Name == "Small Health Potion");
                var sword       = await _ctx.Items.FirstOrDefaultAsync(i => i.Name == "Iron Sword");
                var armor       = await _ctx.Items.FirstOrDefaultAsync(i => i.Name == "Leather Armor");
                var whiteFlower = await _ctx.Items.FirstOrDefaultAsync(i => i.Name == "White Flower");
                var upgradeStone = await _ctx.Items.FirstOrDefaultAsync(i => i.Name == "Skill Upgrade Stone");

                var missingItems = new List<string>();
                if (potion == null)      missingItems.Add("Small Health Potion");
                if (sword == null)       missingItems.Add("Iron Sword");
                if (armor == null)       missingItems.Add("Leather Armor");
                if (whiteFlower == null) missingItems.Add("White Flower");
                if (upgradeStone == null) missingItems.Add("Skill Upgrade Stone");

                if (missingItems.Count > 0)
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = $"System items chưa có trong DB. Hãy chạy migration items trước. Thiếu: {string.Join(", ", missingItems)}"
                    });

                // EquipmentStats đã được tạo sẵn qua migration cùng với item
                // Không cần upsert lại ở đây

                // 2. Upsert skins, same reason as items: player skin ownership can reference them.
                var skinNames = new[] { 
                    "[ELF] ElfForest Default", 
                    "[ELF] Ranger Cloak",
                    "[ELF] Elven Blade",
                    "[ELF] Leaf Crown",
                    "[ELF] Guardian Plate"
                };
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
                var skinSword = UpsertSkin("[ELF] Elven Blade", "A beautiful blade glowing with forest magic.", "Weapon", "Epic");
                var skinHelm = UpsertSkin("[ELF] Leaf Crown", "A crown made of mystical leaves.", "Helmet", "Uncommon");
                var skinArmor = UpsertSkin("[ELF] Guardian Plate", "Sturdy plate armor worn by the forest guardians.", "Armor", "Rare");
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

                // Upsert 3 tutorial skills rewarded when player delivers 3 White Flowers
                var elfSkillNames = new[] { 
                    "Dark Poison Zone", "Dark Explosion",
                    "AP_Skill", "Skill_Ad", "Skill_Knight Attack", "Skill_Mui_Ten_Bang", "Skill_Thap_AS"
                };
                var existingElfSkills = await _ctx.Skills
                    .Where(s => elfSkillNames.Contains(s.Name))
                    .ToListAsync();

                Skill UpsertSkill(string name, string description, string type, string damageType,
                    string targetType, string classReq, int cooldown, double baseDmg, double dmgPerLv, double growthPct, float corruptionCost = 0f)
                {
                    var s = existingElfSkills.FirstOrDefault(x => x.Name == name);
                    if (s == null)
                    {
                        s = new Skill { Name = name };
                        _ctx.Skills.Add(s);
                        existingElfSkills.Add(s); // keep local list in sync
                    }
                    s.Description         = description;
                    s.Type                = type;
                    s.DamageType          = damageType;
                    s.TargetType          = targetType;
                    s.ClassRequirement    = classReq;
                    s.CooldownSeconds     = cooldown;
                    s.BaseDamage          = baseDmg;
                    s.DamagePerLevel      = dmgPerLv;
                    s.DamageGrowthPercent = growthPct;
                    s.CorruptionCost      = corruptionCost;
                    s.UnlockLevel         = 1;
                    s.IsActive            = true;
                    return s;
                }

                var poisonZoneSkill = UpsertSkill(
                    "Dark Poison Zone",
                    "Tạo bãi độc gây sát thương diện rộng. Hắc hóa +10.",
                    "Active", "Magical", "Area", "All", 90, 80.0, 10.0, 3.0, 10f);

                var explosionSkill = UpsertSkill(
                    "Dark Explosion",
                    "Tạo vụ nổ gây sát thương khủng khiếp. Hắc hóa +5.",
                    "Active", "Magical", "Area", "All", 60, 150.0, 20.0, 5.0, 5f);

                // Add 5 custom skills
                var apSkill = UpsertSkill("AP_Skill", "Mage Buff/Explosion skill", "Active", "Magical", "Area", "Mage", 12, 100.0, 15.0, 3.0);
                var skillAd = UpsertSkill("Skill_Ad", "Archer normal arrow", "Active", "Physical", "SingleTarget", "Archer", 5, 45.0, 8.0, 2.0);
                var skillKnightAttack = UpsertSkill("Skill_Knight Attack", "Knight heavy attack", "Active", "Physical", "Area", "Knight", 8, 80.0, 12.0, 2.5);
                var skillMuiTenBang = UpsertSkill("Skill_Mui_Ten_Bang", "Archer light arrow", "Active", "Physical", "SingleTarget", "Archer", 6, 60.0, 10.0, 2.0);
                var skillThapAS = UpsertSkill("Skill_Thap_AS", "Mage light explosion", "Active", "Magical", "Area", "Mage", 15, 120.0, 20.0, 4.0);

                await _ctx.SaveChangesAsync();

                // 3a. Upsert tutorial monsters for ElfForest
                var monsterNames = new[] { "[ELF] Shadow Sprout", "[ELF] Forest Wolf", "[ELF] Sprout King" };
                var existingMonsters = await _ctx.Monsters.Where(m => monsterNames.Contains(m.Name)).ToListAsync();

                Monster UpsertMonster(string name, string type, string description, int level, int maxHp, int atk, int def, int moveSpeed, int attackSpeed, int critRate, int critDamage, int expReward, decimal goldReward)
                {
                    var m = existingMonsters.FirstOrDefault(x => x.Name == name);
                    if (m == null)
                    {
                        m = new Monster { Name = name };
                        _ctx.Monsters.Add(m);
                        existingMonsters.Add(m);
                    }
                    m.Type = type;
                    m.Description = description;
                    m.Level = level;
                    m.MaxHp = maxHp;
                    m.Atk = atk;
                    m.Def = def;
                    m.MoveSpeed = moveSpeed;
                    m.AttackSpeed = attackSpeed;
                    m.CritRate = critRate;
                    m.CritDamage = critDamage;
                    m.ExperienceReward = expReward;
                    m.GoldReward = goldReward;
                    m.IsActive = true;
                    return m;
                }

                var sprout = UpsertMonster(
                    "[ELF] Shadow Sprout", "Normal", "A small sprout that lurks near the clearing.", 1, 80, 8, 2, 90, 100, 2, 120, 8, 2);

                var wolf = UpsertMonster(
                    "[ELF] Forest Wolf", "Normal", "A hungry wolf roaming the forest edge.", 2, 160, 18, 4, 110, 100, 5, 140, 18, 6);

                var sproutKing = UpsertMonster(
                    "[ELF] Sprout King", "Boss", "The corrupted guardian of the willow clearing.", 5, 1200, 60, 20, 80, 90, 10, 150, 600, 120);

                await _ctx.SaveChangesAsync();

                // 3b. Upsert drops
                var monsterIds = existingMonsters.Select(m => m.MonsterId).ToList();
                var existingDrops = await _ctx.MonsterDrops.Where(d => monsterIds.Contains(d.MonsterId)).ToListAsync();

                MonsterDrop UpsertDrop(Monster m, int itemId, double dropRate, int minQ = 1, int maxQ = 1, bool isGuaranteed = false)
                {
                    var ex = existingDrops.FirstOrDefault(x => x.MonsterId == m.MonsterId && x.ItemId == itemId);
                    if (ex == null)
                    {
                        ex = new MonsterDrop { MonsterId = m.MonsterId, ItemId = itemId };
                        _ctx.MonsterDrops.Add(ex);
                        existingDrops.Add(ex);
                    }
                    ex.DropRate = dropRate;
                    ex.MinQuantity = minQ;
                    ex.MaxQuantity = maxQ;
                    ex.IsGuaranteed = isGuaranteed;
                    ex.IsActive = true;
                    return ex;
                }

                if (potion != null)  UpsertDrop(sprout,     potion.ItemId, 40);
                if (sword != null)   UpsertDrop(wolf,        sword.ItemId,  20);
                if (sword != null)   UpsertDrop(sproutKing,  sword.ItemId,  100, 1, 1, true);
                await _ctx.SaveChangesAsync();

                // 3c. Upsert spawns
                var existingSpawns = await _ctx.MonsterSpawns.Where(s => monsterIds.Contains(s.MonsterId)).ToListAsync();

                MonsterSpawn UpsertSpawn(Monster m, string mapName, string region, string location, int count, int respawn)
                {
                    var ex = existingSpawns.FirstOrDefault(x => x.MonsterId == m.MonsterId && x.MapName == mapName && x.RegionName == region);
                    if (ex == null)
                    {
                        ex = new MonsterSpawn { MonsterId = m.MonsterId, MapName = mapName, RegionName = region };
                        _ctx.MonsterSpawns.Add(ex);
                        existingSpawns.Add(ex);
                    }
                    ex.Location = location;
                    ex.SpawnCount = count;
                    ex.RespawnSeconds = respawn;
                    ex.IsActive = true;
                    return ex;
                }

                UpsertSpawn(sprout, "ElfForest", "Forest Edge", "Forest Edge - North", 4, 30);
                UpsertSpawn(wolf, "ElfForest", "Forest Edge", "Forest Edge - South", 2, 45);
                UpsertSpawn(sproutKing, "ElfForest", "Old Willow Clearing", "Willow Throne", 1, 0);
                await _ctx.SaveChangesAsync();

                // 3e. (Optional) seed discovery entries for test players (leave undiscovered by default)

                await _ctx.SaveChangesAsync();



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
                    profile.CurrentEnergy = 100;
                    profile.MaxEnergy = 100;
                    profile.LastEnergyUpdateTime = DateTime.UtcNow;
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
                        AttackSpeed = 100,
                        CritRate = 50,
                        CritDamage = 150,
                    });
                    await _ctx.SaveChangesAsync();

                    // inventory: dùng system items từ migration
                    if (sword != null)
                        _ctx.InventoryItems.Add(new InventoryItem
                        {
                            PlayerProfileId = pid,
                            ItemId = sword.ItemId,
                            Quantity = 1,
                            IsEquipped = true,
                            IsSkin = false,
                            EquippedSlot = "Weapon",
                        });
                    if (armor != null)
                        _ctx.InventoryItems.Add(new InventoryItem
                        {
                            PlayerProfileId = pid,
                            ItemId = armor.ItemId,
                            Quantity = 1,
                            IsEquipped = true,
                            IsSkin = false,
                            EquippedSlot = "Armor",
                        });
                    if (potion != null)
                        _ctx.InventoryItems.Add(new InventoryItem
                        {
                            PlayerProfileId = pid,
                            ItemId = potion.ItemId,
                            Quantity = 3,
                            IsEquipped = false,
                            IsSkin = false,
                        });
                    if (upgradeStone != null)
                        _ctx.InventoryItems.Add(new InventoryItem
                        {
                            PlayerProfileId = pid,
                            ItemId = upgradeStone.ItemId,
                            Quantity = 99,
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

                    // Unlock 3 skills rewarded from the Gather White Flowers quest
                    // [DA UPDATE] Đã comment đoạn này lại để testplayer KHÔNG có sẵn skill,
                    // giúp test tính năng: Làm xong Quest nhận thưởng mới được mở khóa skill.
                    /*
                    foreach (var skill in new[] { apSkill, skillAd, skillKnightAttack })
                    {
                        var alreadyHas = await _ctx.PlayerSkills.AnyAsync(
                            ps => ps.PlayerProfileId == pid && ps.SkillId == skill.SkillId);
                        if (!alreadyHas)
                        {
                            _ctx.PlayerSkills.Add(new PlayerSkill
                            {
                                PlayerProfileId = pid,
                                SkillId        = skill.SkillId,
                                Level          = 1,
                                Experience     = 0,
                                EquippedSlot   = null, // chưa trang bị, player tự chọn
                                UnlockedAt     = DateTime.UtcNow,
                            });
                        }
                    }
                    await _ctx.SaveChangesAsync();
                    */

                    return pid;
                }

                var p1 = await CreatePlayer("elf_user1", "elf1@mystic.test", "Tutorial Knight 1", "Knight");
                
                // Cấp sẵn 7 kỹ năng cho elf1 (2 hắc hóa + 5 kỹ năng custom)
                foreach (var skillId in new[] { 
                    poisonZoneSkill.SkillId, 
                    explosionSkill.SkillId, 
                    apSkill.SkillId, 
                    skillAd.SkillId, 
                    skillKnightAttack.SkillId, 
                    skillMuiTenBang.SkillId, 
                    skillThapAS.SkillId 
                })
                {
                    _ctx.PlayerSkills.Add(new PlayerSkill
                    {
                        PlayerProfileId = p1,
                        SkillId = skillId,
                        Level = 1,
                        Experience = 0,
                        UnlockedAt = DateTime.UtcNow
                    });
                }
                await _ctx.SaveChangesAsync();

                await SeedGachaBaseDataAsync("elf1@mystic.test", 11);
                var p2 = await CreatePlayer("elf_user2", "elf2@mystic.test", "Tutorial Knight 2", "Knight");

                await tx.CommitAsync();

                return Ok(new ApiResponse<object> { Success = true, Message = "Seed ElfForest completed", Data = new { players = new[] { p1, p2 }, gacha = new { bannerName = "[SEED] Test Gacha Banner", grantedToEmail = "elf1@mystic.test", ticketCount = 11 } } });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return StatusCode(500, new ApiResponse<object> { Success = false, Message = ex.ToString(), ErrorCode = ErrorCodes.InternalError });
            }
        }

        private async Task<(int BannerId, int TicketItemId)> SeedGachaBaseDataAsync(string? targetEmail = null, int ticketQuantity = 11)
        {
            var now = DateTime.UtcNow;

            var existingBanner = await _ctx.GachaBanners
                .FirstOrDefaultAsync(b => b.Name == "[SEED] Test Gacha Banner");

            if (existingBanner != null)
            {
                _ctx.GachaPullHistories.RemoveRange(_ctx.GachaPullHistories.Where(h => h.GachaBannerId == existingBanner.GachaBannerId));
                _ctx.GachaBannerItems.RemoveRange(_ctx.GachaBannerItems.Where(i => i.GachaBannerId == existingBanner.GachaBannerId));
                await _ctx.SaveChangesAsync();
            }

            // Lookup system items từ migration (không tạo item mới trong gacha seed)
            var systemItems = await _ctx.Items
                .Where(i => new[]
                {
                    "Lucky Ticket",
                    "Elven Blade",
                    "Phantom Cloak",
                    "Skill Upgrade Stone",
                    "Gem",
                    "Small Health Potion",
                    "Gold"
                }.Contains(i.Name))
                .ToDictionaryAsync(i => i.Name);

            Item GetItem(string name)
            {
                if (systemItems.TryGetValue(name, out var item))
                    return item;
                throw new InvalidOperationException($"System item '{name}' chưa có trong DB. Hãy chạy migration items trước.");
            }

            var ticketItem   = GetItem("Lucky Ticket");
            var featuredItem = GetItem("Elven Blade");
            var cloakItem    = GetItem("Phantom Cloak");
            var runeItem     = GetItem("Skill Upgrade Stone");
            var shardItem    = GetItem("Gem");
            var potionItem   = GetItem("Small Health Potion");
            var goldItem     = GetItem("Gold");

            await _ctx.SaveChangesAsync();

            if (existingBanner == null)
            {
                existingBanner = new GachaBanner
                {
                    Name = "[SEED] Test Gacha Banner",
                    Type = "Limited",
                    PullCost = 1,
                    CostItemId = ticketItem.ItemId,
                    PityLimit = 90,
                    IsActive = true,
                    StartAt = now.AddDays(-1),
                    EndAt = now.AddYears(1)
                };
                _ctx.GachaBanners.Add(existingBanner);
                await _ctx.SaveChangesAsync();
            }
            else
            {
                existingBanner.Type = "Limited";
                existingBanner.PullCost = 1;
                existingBanner.CostItemId = ticketItem.ItemId;
                existingBanner.PityLimit = 90;
                existingBanner.IsActive = true;
                existingBanner.StartAt = now.AddDays(-1);
                existingBanner.EndAt = now.AddYears(1);
                await _ctx.SaveChangesAsync();
            }

            _ctx.GachaBannerItems.AddRange(
                new GachaBannerItem { GachaBannerId = existingBanner.GachaBannerId, ItemId = featuredItem.ItemId, DropRate = 1m, IsFeatured = true },
                new GachaBannerItem { GachaBannerId = existingBanner.GachaBannerId, ItemId = cloakItem.ItemId, DropRate = 15m, IsFeatured = false },
                new GachaBannerItem { GachaBannerId = existingBanner.GachaBannerId, ItemId = runeItem.ItemId, DropRate = 20m, IsFeatured = false },
                new GachaBannerItem { GachaBannerId = existingBanner.GachaBannerId, ItemId = shardItem.ItemId, DropRate = 20m, IsFeatured = false },
                new GachaBannerItem { GachaBannerId = existingBanner.GachaBannerId, ItemId = potionItem.ItemId, DropRate = 24m, IsFeatured = false },
                new GachaBannerItem { GachaBannerId = existingBanner.GachaBannerId, ItemId = goldItem.ItemId, DropRate = 20m, IsFeatured = false }
            );
            await _ctx.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(targetEmail))
            {
                var targetAccount = await _ctx.Accounts
                    .Include(a => a.PlayerProfile)
                    .FirstOrDefaultAsync(a => a.Email == targetEmail);

                if (targetAccount?.PlayerProfile != null)
                {
                    var targetInventoryItem = await _ctx.InventoryItems
                        .FirstOrDefaultAsync(x => x.PlayerProfileId == targetAccount.PlayerProfile.PlayerProfileId && x.ItemId == ticketItem.ItemId);

                    if (targetInventoryItem != null)
                    {
                        targetInventoryItem.Quantity = ticketQuantity;
                    }
                    else
                    {
                        _ctx.InventoryItems.Add(new InventoryItem
                        {
                            PlayerProfileId = targetAccount.PlayerProfile.PlayerProfileId,
                            ItemId = ticketItem.ItemId,
                            Quantity = ticketQuantity,
                            IsEquipped = false,
                            IsSkin = false,
                            EnhancementLevel = 0,
                        });
                    }

                    await _ctx.SaveChangesAsync();
                }
            }

            return (existingBanner.GachaBannerId, ticketItem.ItemId);
        }

        // ─────────────────────────────────────────────────────────────────────────
        // POST /api/seed/dailylogin  → Seed 30 ngày DailyLoginRewards
        // Xoá reward cũ rồi chèn mới đủ 30 ngày với nhiều loại phần thưởng.
        // Các ngày có Item reward sẽ tự động dùng item [SEED] / [ELF] nếu tồn tại.
        // ─────────────────────────────────────────────────────────────────────────
        [HttpPost("dailylogin")]
        public async Task<IActionResult> SeedDailyLogin()
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                // Xoá toàn bộ reward cũ để seed lại sạch
                var existingRewards = await _ctx.DailyLoginRewards.ToListAsync();
                _ctx.DailyLoginRewards.RemoveRange(existingRewards);
                await _ctx.SaveChangesAsync();

                // Dùng system items từ migration làm phần thưởng
                var potion = await _ctx.Items.FirstOrDefaultAsync(i => i.Name == "Small Health Potion");
                var sword  = await _ctx.Items.FirstOrDefaultAsync(i => i.Name == "Iron Sword");
                var helm   = await _ctx.Items.FirstOrDefaultAsync(i => i.Name == "Iron Helmet");

                // Định nghĩa phần thưởng cho 30 ngày
                // RewardType: Gold | Gems | Energy | Item
                var rewards = new List<DailyLoginReward>
                {
                    // Tuần 1 – khởi động
                    new DailyLoginReward { DayNumber = 1,  RewardType = "Gold",   RewardValue = 100,  IsActive = true, CreatedAt = DateTime.UtcNow },
                    new DailyLoginReward { DayNumber = 2,  RewardType = "Energy", RewardValue = 20,   IsActive = true, CreatedAt = DateTime.UtcNow },
                    new DailyLoginReward { DayNumber = 3,  RewardType = "Gold",   RewardValue = 200,  IsActive = true, CreatedAt = DateTime.UtcNow },
                    new DailyLoginReward { DayNumber = 4,  RewardType = "Gems",   RewardValue = 5,    IsActive = true, CreatedAt = DateTime.UtcNow },
                    new DailyLoginReward { DayNumber = 5,  RewardType = "Gold",   RewardValue = 300,  IsActive = true, CreatedAt = DateTime.UtcNow },
                    new DailyLoginReward { DayNumber = 6,  RewardType = "Energy", RewardValue = 30,   IsActive = true, CreatedAt = DateTime.UtcNow },
                    new DailyLoginReward
                    {
                        DayNumber = 7, RewardType = "Item", RewardValue = 0,
                        RewardItemId = potion?.ItemId, RewardItemQuantity = potion != null ? 3 : 0,
                        IsActive = true, CreatedAt = DateTime.UtcNow
                    },

                    // Tuần 2 – tăng dần
                    new DailyLoginReward { DayNumber = 8,  RewardType = "Gold",   RewardValue = 400,  IsActive = true, CreatedAt = DateTime.UtcNow },
                    new DailyLoginReward { DayNumber = 9,  RewardType = "Gems",   RewardValue = 10,   IsActive = true, CreatedAt = DateTime.UtcNow },
                    new DailyLoginReward { DayNumber = 10, RewardType = "Gold",   RewardValue = 500,  IsActive = true, CreatedAt = DateTime.UtcNow },
                    new DailyLoginReward { DayNumber = 11, RewardType = "Energy", RewardValue = 40,   IsActive = true, CreatedAt = DateTime.UtcNow },
                    new DailyLoginReward { DayNumber = 12, RewardType = "Gold",   RewardValue = 600,  IsActive = true, CreatedAt = DateTime.UtcNow },
                    new DailyLoginReward { DayNumber = 13, RewardType = "Gems",   RewardValue = 15,   IsActive = true, CreatedAt = DateTime.UtcNow },
                    new DailyLoginReward
                    {
                        DayNumber = 14, RewardType = "Item", RewardValue = 0,
                        RewardItemId = helm?.ItemId, RewardItemQuantity = helm != null ? 1 : 0,
                        IsActive = true, CreatedAt = DateTime.UtcNow
                    },

                    // Tuần 3 – milestone giữa tháng
                    new DailyLoginReward { DayNumber = 15, RewardType = "Gold",   RewardValue = 800,  IsActive = true, CreatedAt = DateTime.UtcNow },
                    new DailyLoginReward { DayNumber = 16, RewardType = "Gems",   RewardValue = 20,   IsActive = true, CreatedAt = DateTime.UtcNow },
                    new DailyLoginReward { DayNumber = 17, RewardType = "Energy", RewardValue = 50,   IsActive = true, CreatedAt = DateTime.UtcNow },
                    new DailyLoginReward { DayNumber = 18, RewardType = "Gold",   RewardValue = 900,  IsActive = true, CreatedAt = DateTime.UtcNow },
                    new DailyLoginReward { DayNumber = 19, RewardType = "Gems",   RewardValue = 25,   IsActive = true, CreatedAt = DateTime.UtcNow },
                    new DailyLoginReward { DayNumber = 20, RewardType = "Gold",   RewardValue = 1000, IsActive = true, CreatedAt = DateTime.UtcNow },
                    new DailyLoginReward
                    {
                        DayNumber = 21, RewardType = "Item", RewardValue = 0,
                        RewardItemId = potion?.ItemId, RewardItemQuantity = potion != null ? 5 : 0,
                        IsActive = true, CreatedAt = DateTime.UtcNow
                    },

                    // Tuần 4 – hướng tới cuối tháng
                    new DailyLoginReward { DayNumber = 22, RewardType = "Gold",   RewardValue = 1100, IsActive = true, CreatedAt = DateTime.UtcNow },
                    new DailyLoginReward { DayNumber = 23, RewardType = "Energy", RewardValue = 60,   IsActive = true, CreatedAt = DateTime.UtcNow },
                    new DailyLoginReward { DayNumber = 24, RewardType = "Gems",   RewardValue = 30,   IsActive = true, CreatedAt = DateTime.UtcNow },
                    new DailyLoginReward { DayNumber = 25, RewardType = "Gold",   RewardValue = 1200, IsActive = true, CreatedAt = DateTime.UtcNow },
                    new DailyLoginReward { DayNumber = 26, RewardType = "Gems",   RewardValue = 35,   IsActive = true, CreatedAt = DateTime.UtcNow },
                    new DailyLoginReward { DayNumber = 27, RewardType = "Energy", RewardValue = 70,   IsActive = true, CreatedAt = DateTime.UtcNow },
                    new DailyLoginReward
                    {
                        DayNumber = 28, RewardType = "Item", RewardValue = 0,
                        RewardItemId = sword?.ItemId, RewardItemQuantity = sword != null ? 1 : 0,
                        IsActive = true, CreatedAt = DateTime.UtcNow
                    },

                    // Ngày 29–30 – phần thưởng lớn cuối tháng
                    new DailyLoginReward { DayNumber = 29, RewardType = "Gems",   RewardValue = 50,   IsActive = true, CreatedAt = DateTime.UtcNow },
                    new DailyLoginReward { DayNumber = 30, RewardType = "Gold",   RewardValue = 2000, IsActive = true, CreatedAt = DateTime.UtcNow },
                };

                _ctx.DailyLoginRewards.AddRange(rewards);
                await _ctx.SaveChangesAsync();
                await tx.CommitAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = $"Seed {rewards.Count} daily login rewards thành công!",
                    Data = new
                    {
                        totalDays = rewards.Count,
                        itemRewardDays = new[] { 7, 14, 21, 28 },
                        itemsUsed = new
                        {
                            potion = potion != null ? $"{potion.Name} (Id={potion.ItemId})" : "Không tìm thấy – ngày Item sẽ có Quantity=0",
                            helm   = helm   != null ? $"{helm.Name} (Id={helm.ItemId})"     : "Không tìm thấy",
                            sword  = sword  != null ? $"{sword.Name} (Id={sword.ItemId})"   : "Không tìm thấy",
                        },
                        tip = "Chạy POST /api/seed/inventory trước để có item [SEED] rồi mới seed daily login."
                    }
                });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return StatusCode(500, new ApiResponse<object> { Success = false, Message = ex.ToString(), ErrorCode = ErrorCodes.InternalError });
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
                return StatusCode(500, new ApiResponse<object> { Success = false, Message = ex.ToString(), ErrorCode = ErrorCodes.InternalError });
            }
        }

        // ─────────────────────────────────────────────────────────────────────────
        // POST /api/seed/monsters -> Seed various monsters and their spawns
        // ─────────────────────────────────────────────────────────────────────────
        [HttpPost("monsters")]
        public async Task<IActionResult> SeedMonsters()
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                var monsterNames = new[] { "Slime", "SkeletonMelee", "GoblinSpear", "GoblinWarrior", "Ogre", "Dragon", "DragonForest", "Demon" };
                var existingMonsters = await _ctx.Monsters.Where(m => monsterNames.Contains(m.Name)).ToListAsync();

                Monster UpsertMonster(string name, string type, string description, int level, int maxHp, int atk, int def, int moveSpeed, int attackSpeed, int critRate, int critDamage, int expReward, decimal goldReward)
                {
                    var m = existingMonsters.FirstOrDefault(x => x.Name == name);
                    if (m == null)
                    {
                        m = new Monster { Name = name };
                        _ctx.Monsters.Add(m);
                        existingMonsters.Add(m);
                    }
                    m.Type = type;
                    m.Description = description;
                    m.Level = level;
                    m.MaxHp = maxHp;
                    m.Atk = atk;
                    m.Def = def;
                    m.MoveSpeed = moveSpeed;
                    m.AttackSpeed = attackSpeed;
                    m.CritRate = critRate;
                    m.CritDamage = critDamage;
                    m.ExperienceReward = expReward;
                    m.GoldReward = goldReward;
                    m.IsActive = true;
                    return m;
                }

                var slime = UpsertMonster("Slime", "Normal", "A normal slime monster found outside.", 3, 200, 15, 5, 100, 100, 5, 150, 20, 10);
                var skeleton = UpsertMonster("SkeletonMelee", "Normal", "A skeleton warrior from the dark dungeon.", 5, 350, 25, 10, 110, 100, 5, 150, 40, 25);
                var goblinSpear = UpsertMonster("GoblinSpear", "Normal", "A goblin with a spear.", 4, 300, 20, 8, 115, 100, 5, 150, 30, 20);
                var goblinWarrior = UpsertMonster("GoblinWarrior", "Normal", "A tough goblin warrior.", 6, 400, 30, 15, 105, 100, 5, 150, 45, 30);
                var ogre = UpsertMonster("Ogre", "Boss", "The brutal boss of the dungeon.", 10, 2500, 100, 40, 80, 80, 10, 200, 1000, 500);
                var dragon = UpsertMonster("Dragon", "Normal", "A fearsome dragon.", 15, 5000, 150, 60, 120, 90, 15, 200, 2500, 1000);
                var dragonForest = UpsertMonster("DragonForest", "Normal", "A dragon from the deep forest.", 12, 4000, 120, 50, 110, 90, 10, 200, 1800, 800);
                var demon = UpsertMonster("Demon", "Normal", "A terrifying demon from the underworld.", 20, 8000, 200, 80, 130, 100, 20, 250, 4000, 2000);

                await _ctx.SaveChangesAsync();

                // Setup Dungeon Boss Chest
                var bossChest = await _ctx.Chests.FirstOrDefaultAsync(c => c.Name == "Abandoned Mines Reward Chest");
                if (bossChest == null)
                {
                    bossChest = new Chest
                    {
                        Name = "Abandoned Mines Reward Chest",
                        GoldMinReward = 150,
                        GoldMaxReward = 300,
                        ExperienceReward = 200
                    };
                    _ctx.Chests.Add(bossChest);
                    await _ctx.SaveChangesAsync();
                }

                // Add some items to this chest – dùng system items từ migration
                var healthPotion = await _ctx.Items.FirstOrDefaultAsync(i => i.Name == "Small Health Potion");
                var basicSword   = await _ctx.Items.FirstOrDefaultAsync(i => i.Name == "Iron Sword");
                
                if (healthPotion != null)
                {
                    var existingHp = await _ctx.ChestItems.FirstOrDefaultAsync(ci => ci.ChestId == bossChest.ChestId && ci.ItemId == healthPotion.ItemId);
                    if (existingHp == null)
                    {
                        _ctx.ChestItems.Add(new ChestItem
                        {
                            ChestId = bossChest.ChestId,
                            ItemId = healthPotion.ItemId,
                            DropRate = 100.0m, // 100%
                            QuantityMin = 1,
                            QuantityMax = 3
                        });
                    }
                    else
                    {
                        existingHp.DropRate = 100.0m;
                    }
                }
                
                if (basicSword != null)
                {
                    var existingSword = await _ctx.ChestItems.FirstOrDefaultAsync(ci => ci.ChestId == bossChest.ChestId && ci.ItemId == basicSword.ItemId);
                    if (existingSword == null)
                    {
                        _ctx.ChestItems.Add(new ChestItem
                        {
                            ChestId = bossChest.ChestId,
                            ItemId = basicSword.ItemId,
                            DropRate = 50.0m, // 50%
                            QuantityMin = 1,
                            QuantityMax = 1
                        });
                    }
                    else
                    {
                        existingSword.DropRate = 50.0m;
                    }
                }
                await _ctx.SaveChangesAsync();

                // Setup DungeonConfig
                var dungeonConfig = await _ctx.DungeonConfigs.FirstOrDefaultAsync(d => d.Name == "Abandoned Mines" || d.Name == "Goblin Dungeon");
                if (dungeonConfig == null)
                {
                    dungeonConfig = new DungeonConfig 
                    { 
                        Name = "Abandoned Mines", 
                        Description = "A dark dungeon filled with goblins and skeletons.", 
                        LevelRequirement = 5,
                        EnergyCost = 20,
                        Type = "Normal",
                        IsActive = true,
                        ChestId = bossChest.ChestId
                    };
                    _ctx.DungeonConfigs.Add(dungeonConfig);
                    await _ctx.SaveChangesAsync();
                }
                else
                {
                    dungeonConfig.Name = "Abandoned Mines";
                    dungeonConfig.EnergyCost = 20;
                    dungeonConfig.ChestId = bossChest.ChestId;
                    _ctx.DungeonConfigs.Update(dungeonConfig);
                    await _ctx.SaveChangesAsync();
                }

                // Setup Dungeon (for Monster Spawns)
                var dungeon = await _ctx.Dungeons.FirstOrDefaultAsync(d => d.DungeonId == 1);
                if (dungeon == null)
                {
                    dungeon = new Dungeon { Name = "Abandoned Mines", Description = "A dark dungeon filled with goblins and skeletons.", IsRepeatable = true };
                    _ctx.Dungeons.Add(dungeon);
                    await _ctx.SaveChangesAsync();
                }

                var monsterIds = existingMonsters.Select(m => m.MonsterId).ToList();
                var existingSpawns = await _ctx.MonsterSpawns.Where(s => monsterIds.Contains(s.MonsterId)).ToListAsync();

                MonsterSpawn UpsertSpawn(Monster m, string mapName, string region, string location, int count, int respawn, int? dungeonId)
                {
                    var ex = existingSpawns.FirstOrDefault(x => x.MonsterId == m.MonsterId && x.MapName == mapName && x.RegionName == region);
                    if (ex == null)
                    {
                        ex = new MonsterSpawn { MonsterId = m.MonsterId, MapName = mapName, RegionName = region };
                        _ctx.MonsterSpawns.Add(ex);
                        existingSpawns.Add(ex);
                    }
                    ex.Location = location;
                    ex.SpawnCount = count;
                    ex.RespawnSeconds = respawn;
                    ex.DungeonId = dungeonId;
                    ex.IsActive = true;
                    return ex;
                }

                // Slime: normal monster outside, related to quest
                UpsertSpawn(slime, "Map1", "Slime Field", "Slime Field", 5, 30, null);
                
                // Dungeon Monsters
                UpsertSpawn(skeleton, "Dungeon_Goblin", "Skeleton Zone", "Skeleton Zone", 3, 45, dungeon.DungeonId);
                UpsertSpawn(goblinSpear, "Dungeon_Goblin", "Goblin Camp", "Goblin Camp", 3, 45, dungeon.DungeonId);
                UpsertSpawn(goblinWarrior, "Dungeon_Goblin", "Goblin Camp", "Goblin Camp", 2, 45, dungeon.DungeonId);
                UpsertSpawn(ogre, "Dungeon_Goblin", "Ogre Lair", "Ogre Lair", 1, 300, dungeon.DungeonId);
                
                // Other normal monsters outside
                UpsertSpawn(dragon, "Map3", "Dragon Peak", "Dragon Peak", 1, 300, null);
                UpsertSpawn(dragonForest, "Map2", "Deep Forest", "Deep Forest", 1, 300, null);
                UpsertSpawn(demon, "Map4", "Underworld", "Underworld", 1, 300, null);

                await _ctx.SaveChangesAsync();
                await tx.CommitAsync();

                return Ok(new ApiResponse<object> { Success = true, Message = "Seed Monsters completed", Data = existingMonsters.Select(m => m.Name) });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return StatusCode(500, new ApiResponse<object> { Success = false, Message = ex.ToString(), ErrorCode = ErrorCodes.InternalError });
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
        // POST /api/seed/dungeons → Tạo dữ liệu 3 Dungeon mẫu để test luồng Game
        // ─────────────────────────────────────────────────────────────────────────
        [HttpPost("dungeons")]
        public async Task<IActionResult> SeedDungeons()
        {
            try
            {
                // ─── 1. Xoá dữ liệu dungeon cũ ───────────────────────────────────────
                var existingSpawns = await _ctx.MonsterSpawns.Where(ms => ms.DungeonId != null).ToListAsync();
                _ctx.MonsterSpawns.RemoveRange(existingSpawns);

                var existingDungeons = await _ctx.Dungeons.ToListAsync();
                _ctx.Dungeons.RemoveRange(existingDungeons);

                var existingConfigs = await _ctx.DungeonConfigs.ToListAsync();
                _ctx.DungeonConfigs.RemoveRange(existingConfigs);

                await _ctx.SaveChangesAsync();

                // ─── 2. Đảm bảo tất cả monsters tồn tại (theo MonsterDatabaseSO) ────
                //  ID  |  Tên                  | Type
                //  1   |  SlimeLittle          | Normal
                //  2   |  SwampDemon           | Boss
                //  3   |  WaterElemental       | Normal
                //  4   |  Dragon               | Normal
                //  5   |  BlueDragonFrost      | Normal
                //  6   |  GreenDragonForest    | Normal
                //  7   |  DragonBossIdle       | Boss
                //  8   |  Slime_ice            | Normal
                //  9   |  Ice_Dragon           | Normal
                //  10  |  GolemBoss            | Boss
                //  11  |  OrcSkeleton          | Normal
                //  12  |  SkeletonMelee        | Normal
                //  13  |  SkeletonArcher       | Normal
                //  14  |  Ghost                | Normal
                //  15  |  UnderKing            | Boss
                //  16  |  Demon                | Normal
                //  17  |  GoblinWarrior        | Normal
                //  18  |  GoblinSpear          | Normal
                //  19  |  Ogre                 | Boss
                //  20  |  OrcWarlord           | Boss

                void EnsureMonster(int id, string name, string type, int hp, int atk, int def)
                {
                    var m = _ctx.Monsters.Local.FirstOrDefault(x => x.MonsterId == id)
                         ?? _ctx.Monsters.Find(id);
                    if (m == null)
                    {
                        _ctx.Monsters.Add(new Monster
                        {
                            MonsterId = id, Name = name, Type = type,
                            MaxHp = hp, Atk = atk, Def = def, IsActive = true
                        });
                    }
                }

                // Normal monsters
                EnsureMonster(1,  "SlimeLittle",       "Normal", 80,   8,  3);
                EnsureMonster(3,  "WaterElemental",    "Normal", 150, 12,  6);
                EnsureMonster(4,  "Dragon",            "Normal", 500, 40, 20);
                EnsureMonster(5,  "BlueDragonFrost",   "Normal", 450, 35, 18);
                EnsureMonster(6,  "GreenDragonForest", "Normal", 400, 30, 15);
                EnsureMonster(8,  "Slime_ice",         "Normal", 120, 10,  5);
                EnsureMonster(9,  "Ice_Dragon",        "Normal", 600, 50, 25);
                EnsureMonster(11, "OrcSkeleton",       "Normal", 200, 20,  8);
                EnsureMonster(12, "SkeletonMelee",     "Normal", 180, 18,  7);
                EnsureMonster(13, "SkeletonArcher",    "Normal", 140, 22,  5);
                EnsureMonster(14, "Ghost",             "Normal", 160, 25,  4);
                EnsureMonster(16, "Demon",             "Normal", 800, 70, 30);
                EnsureMonster(17, "GoblinWarrior",     "Normal", 220, 22, 10);
                EnsureMonster(18, "GoblinSpear",       "Normal", 180, 18,  8);

                // Boss monsters
                EnsureMonster(2,  "SwampDemon",   "Boss", 1500, 80,  30);
                EnsureMonster(7,  "DragonBossIdle","Boss",3000, 150, 60);
                EnsureMonster(10, "GolemBoss",    "Boss", 2500, 120, 80);
                EnsureMonster(15, "UnderKing",    "Boss", 2800, 130, 50);
                EnsureMonster(19, "Ogre",         "Boss", 2000, 100, 40);
                EnsureMonster(20, "OrcWarlord",   "Boss", 3500, 160, 70);

                await _ctx.SaveChangesAsync();

                // ─── 3. Tạo 6 Dungeon (ID phải khớp với Unity DungeonConfig) ────────
                var dungeons = new List<Dungeon>
                {
                    new Dungeon { DungeonId = 1, Name = "Đầm lầy Slime",          Description = "Vương quốc của những con Slime nguy hiểm",           IsRepeatable = true },
                    new Dungeon { DungeonId = 2, Name = "Sào huyệt Rồng",          Description = "Hang ổ của những con rồng hung tàn",                  IsRepeatable = true },
                    new Dungeon { DungeonId = 3, Name = "Cung điện Băng giá",      Description = "Pháo đài băng của Golem khổng lồ",                    IsRepeatable = true },
                    new Dungeon { DungeonId = 4, Name = "Nghĩa địa Bóng tối",     Description = "Vương quốc ngầm của Vua Xương",                       IsRepeatable = true },
                    new Dungeon { DungeonId = 5, Name = "Doanh trại Goblin",       Description = "Pháo đài kiên cố của bầy Goblin và Ogre",             IsRepeatable = true },
                    new Dungeon { DungeonId = 6, Name = "Cổng địa ngục",           Description = "Cánh cổng dẫn đến thế giới của Quỷ và Chiến binh Orc", IsRepeatable = true },
                };
                _ctx.Dungeons.AddRange(dungeons);

                // ─── 4. Tạo DungeonConfig khớp với Unity ─────────────────────────────
                var dungeonConfigs = new List<DungeonConfig>
                {
                    new DungeonConfig { DungeonConfigId = 1, Name = "Đầm lầy Slime",      Description = "Vương quốc của những con Slime nguy hiểm",           Type = "Normal", LevelRequirement = 1,  MaxMembers = 4, Difficulty = 1, EnergyCost = 10, RecommendedPower = 100,  IsActive = true },
                    new DungeonConfig { DungeonConfigId = 2, Name = "Sào huyệt Rồng",      Description = "Hang ổ của những con rồng hung tàn",                  Type = "Normal", LevelRequirement = 5,  MaxMembers = 4, Difficulty = 2, EnergyCost = 15, RecommendedPower = 300,  IsActive = true },
                    new DungeonConfig { DungeonConfigId = 3, Name = "Cung điện Băng giá",  Description = "Pháo đài băng của Golem khổng lồ",                    Type = "Normal", LevelRequirement = 10, MaxMembers = 4, Difficulty = 3, EnergyCost = 20, RecommendedPower = 600,  IsActive = true },
                    new DungeonConfig { DungeonConfigId = 4, Name = "Nghĩa địa Bóng tối", Description = "Vương quốc ngầm của Vua Xương",                       Type = "Normal", LevelRequirement = 15, MaxMembers = 4, Difficulty = 4, EnergyCost = 25, RecommendedPower = 900,  IsActive = true },
                    new DungeonConfig { DungeonConfigId = 5, Name = "Doanh trại Goblin",   Description = "Pháo đài kiên cố của bầy Goblin và Ogre",             Type = "Normal", LevelRequirement = 10, MaxMembers = 4, Difficulty = 3, EnergyCost = 20, RecommendedPower = 700,  IsActive = true },
                    new DungeonConfig { DungeonConfigId = 6, Name = "Cổng địa ngục",       Description = "Cánh cổng dẫn đến thế giới của Quỷ và Chiến binh Orc", Type = "Boss",   LevelRequirement = 20, MaxMembers = 4, Difficulty = 5, EnergyCost = 30, RecommendedPower = 1500, IsActive = true },
                };
                _ctx.DungeonConfigs.AddRange(dungeonConfigs);
                await _ctx.SaveChangesAsync();

                // ─── 5. Tạo MonsterSpawns cho từng Dungeon ───────────────────────────
                // MapName phải khớp với scene name trong Unity
                string mapName = "HollowCryptDungeon";

                var spawns = new List<MonsterSpawn>
                {
                    // ── Dungeon 1: Đầm lầy Slime ─────────────────────────────────────
                    // Quái thường: SlimeLittle (1) + WaterElemental (3)
                    // Boss: SwampDemon (2)
                    new MonsterSpawn { DungeonId = 1, MonsterId = 1,  SpawnCount = 3, MapName = mapName, IsActive = true },
                    new MonsterSpawn { DungeonId = 1, MonsterId = 3,  SpawnCount = 3, MapName = mapName, IsActive = true },
                    new MonsterSpawn { DungeonId = 1, MonsterId = 2,  SpawnCount = 1, MapName = mapName, IsActive = true },

                    // ── Dungeon 2: Sào huyệt Rồng ────────────────────────────────────
                    // Quái thường: Dragon (4) + BlueDragonFrost (5) + GreenDragonForest (6)
                    // Boss: DragonBossIdle (7)
                    new MonsterSpawn { DungeonId = 2, MonsterId = 4,  SpawnCount = 2, MapName = mapName, IsActive = true },
                    new MonsterSpawn { DungeonId = 2, MonsterId = 5,  SpawnCount = 2, MapName = mapName, IsActive = true },
                    new MonsterSpawn { DungeonId = 2, MonsterId = 6,  SpawnCount = 2, MapName = mapName, IsActive = true },
                    new MonsterSpawn { DungeonId = 2, MonsterId = 7,  SpawnCount = 1, MapName = mapName, IsActive = true },

                    // ── Dungeon 3: Cung điện Băng giá ────────────────────────────────
                    // Quái thường: Slime_ice (8) + Ice_Dragon (9)
                    // Boss: GolemBoss (10)
                    new MonsterSpawn { DungeonId = 3, MonsterId = 8,  SpawnCount = 3, MapName = mapName, IsActive = true },
                    new MonsterSpawn { DungeonId = 3, MonsterId = 9,  SpawnCount = 3, MapName = mapName, IsActive = true },
                    new MonsterSpawn { DungeonId = 3, MonsterId = 10, SpawnCount = 1, MapName = mapName, IsActive = true },

                    // ── Dungeon 4: Nghĩa địa Bóng tối ────────────────────────────────
                    // Quái thường: SkeletonMelee (12) + SkeletonArcher (13) + OrcSkeleton (11)
                    // Boss: UnderKing (15)
                    new MonsterSpawn { DungeonId = 4, MonsterId = 12, SpawnCount = 3, MapName = mapName, IsActive = true },
                    new MonsterSpawn { DungeonId = 4, MonsterId = 13, SpawnCount = 2, MapName = mapName, IsActive = true },
                    new MonsterSpawn { DungeonId = 4, MonsterId = 11, SpawnCount = 2, MapName = mapName, IsActive = true },
                    new MonsterSpawn { DungeonId = 4, MonsterId = 15, SpawnCount = 1, MapName = mapName, IsActive = true },

                    // ── Dungeon 5: Doanh trại Goblin ─────────────────────────────────
                    // Quái thường: GoblinWarrior (17) + GoblinSpear (18)
                    // Boss: Ogre (19)
                    new MonsterSpawn { DungeonId = 5, MonsterId = 17, SpawnCount = 3, MapName = mapName, IsActive = true },
                    new MonsterSpawn { DungeonId = 5, MonsterId = 18, SpawnCount = 3, MapName = mapName, IsActive = true },
                    new MonsterSpawn { DungeonId = 5, MonsterId = 19, SpawnCount = 1, MapName = mapName, IsActive = true },

                    // ── Dungeon 6: Cổng địa ngục ─────────────────────────────────────
                    // Quái thường: Ghost (14) + Demon (16) + OrcSkeleton (11)
                    // Boss: OrcWarlord (20)
                    new MonsterSpawn { DungeonId = 6, MonsterId = 14, SpawnCount = 3, MapName = mapName, IsActive = true },
                    new MonsterSpawn { DungeonId = 6, MonsterId = 16, SpawnCount = 2, MapName = mapName, IsActive = true },
                    new MonsterSpawn { DungeonId = 6, MonsterId = 11, SpawnCount = 2, MapName = mapName, IsActive = true },
                    new MonsterSpawn { DungeonId = 6, MonsterId = 20, SpawnCount = 1, MapName = mapName, IsActive = true },
                };

                _ctx.MonsterSpawns.AddRange(spawns);
                await _ctx.SaveChangesAsync();

                return Ok(new
                {
                    message = "Đã seed 6 Dungeons thành công!",
                    dungeons = new[]
                    {
                        "D1: Đầm lầy Slime      → SlimeLittle×3, Slime_ice×3,     Boss: SwampDemon",
                        "D2: Sào huyệt Rồng     → Dragon×2, BlueDragon×2, GreenDragon×2, Boss: DragonBossIdle",
                        "D3: Cung điện Băng giá → WaterElemental×3, Ice_Dragon×3,  Boss: GolemBoss",
                        "D4: Nghĩa địa Bóng tối → SkeletonMelee×3, Archer×2, Orc×2, Boss: UnderKing",
                        "D5: Doanh trại Goblin  → GoblinWarrior×3, GoblinSpear×3, Boss: Ogre",
                        "D6: Cổng địa ngục      → Ghost×3, Demon×2, OrcSkeleton×2, Boss: OrcWarlord"
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi seed Dungeon: " + ex.Message, details = ex.InnerException?.Message });
            }
        }

        // ─────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────────────────────────────────────────────────
        // POST /api/seed/content -> Seed Content
        // ─────────────────────────────────────────────────────────────────────────
        [HttpPost("content")]
        public async Task<IActionResult> SeedContent()
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                var adminAcc = await _ctx.Accounts.FirstOrDefaultAsync(a => a.Email == "admin@mystic.test");
                if (adminAcc == null)
                {
                    adminAcc = new Account
                    {
                        UserName = "admin_seed",
                        Email = "admin@mystic.test",
                        HashPassword = HashPassword("Abc@12345"),
                        RoleId = 2,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    _ctx.Accounts.Add(adminAcc);
                    await _ctx.SaveChangesAsync();
                }

                var existingContents = await _ctx.Contents.Where(c => c.Title.StartsWith("[SEED]")).ToListAsync();
                if (existingContents.Any())
                {
                    _ctx.Contents.RemoveRange(existingContents);
                    await _ctx.SaveChangesAsync();
                }
                
                var existingCategories = await _ctx.CategoryContents.Where(c => c.Name.StartsWith("[SEED]")).ToListAsync();
                if (existingCategories.Any())
                {
                    _ctx.CategoryContents.RemoveRange(existingCategories);
                    await _ctx.SaveChangesAsync();
                }

                var catNews = new CategoryContent { Name = "[SEED] News", Slug = "seed-news", Description = "Game News", IsActive = true };
                var catGuides = new CategoryContent { Name = "[SEED] Guides", Slug = "seed-guides", Description = "Beginner Guides", IsActive = true };
                _ctx.CategoryContents.AddRange(catNews, catGuides);
                await _ctx.SaveChangesAsync();

                var content1 = new Content
                {
                    Title = "[SEED] Welcome to Mystic Journey",
                    Slug = "seed-welcome-to-mystic-journey",
                    Summary = "Welcome to the world of Mystic Journey. Explore 4 mystical lands.",
                    ThumbnailUrl = null,
                    CategoryContentId = catNews.CategoryContentId,
                    IsPublished = true,
                    CreatedAt = DateTime.UtcNow,
                    PublishedAt = DateTime.UtcNow,
                    CreatedByAccountId = Guid.Empty,
                    BlockContents = new List<BlockContent>
                    {
                        new BlockContent { Title = "Introduction", BlockType = "Text", ContentData = "Mystic Journey is an open-world RPG featuring 4 distinct lands...", SortOrder = 1 },
                        new BlockContent { Title = "Lands", BlockType = "Text", ContentData = "Includes Elf Forest, Autumn Pumpkin, Frozen Mountains, and Vestige of an Era.", SortOrder = 2 }
                    }
                };
                
                var content2 = new Content
                {
                    Title = "[SEED] Beginner Guide - Chapter 1",
                    Slug = "seed-beginner-guide",
                    Summary = "A survival handbook in Elf Forest for beginners.",
                    ThumbnailUrl = null,
                    CategoryContentId = catGuides.CategoryContentId,
                    IsPublished = true,
                    CreatedAt = DateTime.UtcNow,
                    PublishedAt = DateTime.UtcNow,
                    CreatedByAccountId = Guid.Empty,
                    BlockContents = new List<BlockContent>
                    {
                        new BlockContent { Title = "Combat Guide", BlockType = "Text", ContentData = "Use basic skills to defeat Shadow Sprout.", SortOrder = 1 }
                    }
                };

                _ctx.Contents.AddRange(content1, content2);
                await _ctx.SaveChangesAsync();
                await tx.CommitAsync();

                return Ok(new ApiResponse<object> { Success = true, Message = "Seed content successfully." });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return StatusCode(500, new ApiResponse<object> { Success = false, Message = ex.ToString(), ErrorCode = ErrorCodes.InternalError });
            }
        }

        [HttpPost("friends")]
        public async Task<IActionResult> SeedFriends()
        {
            using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                const string TEST_EMAIL = "elf1@mystic.test";
                var mainAcc = await _ctx.Accounts
                    .Include(a => a.PlayerProfile)
                    .FirstOrDefaultAsync(a => a.Email == TEST_EMAIL);

                if (mainAcc == null || mainAcc.PlayerProfile == null)
                {
                    return BadRequest(new ApiResponse<object> { Success = false, Message = $"{TEST_EMAIL} not found. Please create or login with this account first." });
                }

                int mainPid = mainAcc.PlayerProfile.PlayerProfileId;

                // Xóa bots cũ
                var botEmails = Enumerable.Range(1, 15).Select(i => $"bot{i}@mystic.test").ToList();
                var existingBots = await _ctx.Accounts
                    .Include(a => a.PlayerProfile)
                    .Where(a => botEmails.Contains(a.Email))
                    .ToListAsync();
                
                foreach (var acc in existingBots)
                {
                    if (acc.PlayerProfile != null)
                    {
                        _ctx.Friends.RemoveRange(_ctx.Friends.Where(f => f.RequesterId == acc.PlayerProfile.PlayerProfileId || f.AddresseeId == acc.PlayerProfile.PlayerProfileId));
                        await _ctx.SaveChangesAsync();
                        _ctx.PlayerProfiles.Remove(acc.PlayerProfile);
                    }
                    _ctx.Accounts.Remove(acc);
                }
                await _ctx.SaveChangesAsync();

                // Tạo 15 Bots
                var botProfiles = new List<PlayerProfile>();
                var classes = new[] { "Knight", "Mage", "Archer" };
                var names = new[] { "Alex", "Bob", "Charlie", "David", "Eve", "Fiona", "George", "Hannah", "Ian", "Jane", "Kevin", "Luna", "Mike", "Nina", "Oscar" };

                for (int i = 1; i <= 15; i++)
                {
                    var botAcc = new Account
                    {
                        UserName = $"bot{i}",
                        Email = $"bot{i}@mystic.test",
                        HashPassword = HashPassword("Abc@12345"),
                        RoleId = 1,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    _ctx.Accounts.Add(botAcc);
                    await _ctx.SaveChangesAsync();

                    var botProf = new PlayerProfile
                    {
                        AccountId = botAcc.AccountId,
                        DisplayName = names[i - 1],
                        Class = classes[i % 3],
                        Level = 5 + i,
                        Gold = 1000,
                        Gems = 100,
                        CurrentEnergy = 100,
                        MaxEnergy = 100,
                        LastEnergyUpdateTime = DateTime.UtcNow,
                        LastMapName = "ElfForest",
                        PositionX = 0, PositionY = 0,
                        AvatarUrl = ""
                    };
                    _ctx.PlayerProfiles.Add(botProf);
                    await _ctx.SaveChangesAsync();
                    botProfiles.Add(botProf);
                }

                // Gán Relationship
                var friendsList = new List<Friend>();

                // 5 Accepted (Bạn bè)
                for (int i = 0; i < 5; i++)
                {
                    friendsList.Add(new Friend { RequesterId = mainPid, AddresseeId = botProfiles[i].PlayerProfileId, Status = "Accepted", CreatedAt = DateTime.UtcNow });
                }

                // 5 Pending (Lời mời kết bạn GỬI ĐẾN mainPid)
                for (int i = 5; i < 10; i++)
                {
                    friendsList.Add(new Friend { RequesterId = botProfiles[i].PlayerProfileId, AddresseeId = mainPid, Status = "Pending", CreatedAt = DateTime.UtcNow });
                }

                // 3 Blocked (mainPid CHẶN bot)
                for (int i = 10; i < 13; i++)
                {
                    friendsList.Add(new Friend { RequesterId = mainPid, AddresseeId = botProfiles[i].PlayerProfileId, Status = "Blocked", CreatedAt = DateTime.UtcNow });
                }

                // 2 Không có quan hệ (bot 14, 15) -> Sẽ hiện nút Add khi Search

                _ctx.Friends.AddRange(friendsList);
                await _ctx.SaveChangesAsync();
                
                await tx.CommitAsync();

                return Ok(new ApiResponse<object> { Success = true, Message = "Seed 15 friends successfully." });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return StatusCode(500, new ApiResponse<object> { Success = false, Message = ex.ToString(), ErrorCode = ErrorCodes.InternalError });
            }
        }

        // ─────────────────────────────────────────────────────────────────────────
        // POST /api/seed/guilds → Seed dữ liệu Bang hội (Guild System v3)
        // ─────────────────────────────────────────────────────────────────────────
        [HttpPost("guilds")]
        public async Task<IActionResult> SeedGuilds()
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                // 1. Xóa các Guild cũ do bot tạo
                var botAccounts = await _ctx.Accounts.Where(a => a.Email.StartsWith("guildbot")).Select(a => a.AccountId).ToListAsync();
                var oldBotProfiles = await _ctx.PlayerProfiles.Where(p => botAccounts.Contains(p.AccountId)).Select(p => p.PlayerProfileId).ToListAsync();
                var existingGuilds = await _ctx.Guilds.Where(g => oldBotProfiles.Contains(g.CreatedByProfileId)).ToListAsync();
                if (existingGuilds.Any())
                {
                    _ctx.Guilds.RemoveRange(existingGuilds);
                    await _ctx.SaveChangesAsync();
                }

                // 2. Tạo 10 user giả để làm Leader / Member
                var botProfiles = new List<PlayerProfile>();
                for (int i = 1; i <= 10; i++)
                {
                    string email = $"guildbot{i}@mystic.test";
                    var acc = await _ctx.Accounts.FirstOrDefaultAsync(a => a.Email == email);
                    if (acc == null)
                    {
                        acc = new Account { UserName = $"guildbot{i}", Email = email, HashPassword = HashPassword("123"), IsActive = true, CreatedAt = DateTime.UtcNow, RoleId = 1 };
                        _ctx.Accounts.Add(acc);
                        await _ctx.SaveChangesAsync();
                    }

                    var prof = await _ctx.PlayerProfiles.FirstOrDefaultAsync(p => p.AccountId == acc.AccountId);
                    if (prof == null)
                    {
                        prof = new PlayerProfile { AccountId = acc.AccountId, DisplayName = $"GuildBot {i}", Level = i * 5, Class = "Knight", CreatedAt = DateTime.UtcNow };
                        _ctx.PlayerProfiles.Add(prof);
                    }
                    else
                    {
                        // Cập nhật lại tên nếu account đã tồn tại từ lần seed trước
                        prof.DisplayName = $"GuildBot {i}";
                    }
                    await _ctx.SaveChangesAsync();
                    botProfiles.Add(prof);
                }

                // 3. Tạo 3 Guilds
                var guild1 = new Guild
                {
                    Name = "Dragon Slayer",
                    Notice = "Bang hội săn rồng, tuyển anh em onl thường xuyên!",
                    IconId = 1,
                    BannerId = 1,
                    Level = 7,
                    GuildExp = 50000,
                    JoinPolicy = DAL.Models.GuildJoinPolicy.Approval,
                    RequiredLevel = 20,
                    LeaderId = botProfiles[0].PlayerProfileId,
                    CreatedByProfileId = botProfiles[0].PlayerProfileId,
                    TotalMedals = 150000,
                    CreatedAt = DateTime.UtcNow.AddDays(-30)
                };

                var guild2 = new Guild
                {
                    Name = "Noob House",
                    Notice = "Vui vẻ là chính, không quan trọng cấp độ.",
                    IconId = 2,
                    BannerId = 2,
                    Level = 2,
                    GuildExp = 1500,
                    JoinPolicy = DAL.Models.GuildJoinPolicy.Open,
                    RequiredLevel = 1,
                    LeaderId = botProfiles[1].PlayerProfileId,
                    CreatedByProfileId = botProfiles[1].PlayerProfileId,
                    TotalMedals = 5000,
                    CreatedAt = DateTime.UtcNow.AddDays(-5)
                };

                var guild3 = new Guild
                {
                    Name = "Solo Leveling",
                    Notice = "Cày chay không nạp.",
                    IconId = 3,
                    BannerId = 3,
                    Level = 5,
                    GuildExp = 25000,
                    JoinPolicy = DAL.Models.GuildJoinPolicy.InviteOnly,
                    RequiredLevel = 50,
                    LeaderId = botProfiles[2].PlayerProfileId,
                    CreatedByProfileId = botProfiles[2].PlayerProfileId,
                    TotalMedals = 75000,
                    CreatedAt = DateTime.UtcNow.AddDays(-15)
                };

                _ctx.Guilds.AddRange(guild1, guild2, guild3);
                await _ctx.SaveChangesAsync();

                // 4. Thêm thành viên vào Guild 1
                var members = new List<GuildMember>
                {
                    new GuildMember { GuildId = guild1.GuildId, PlayerProfileId = botProfiles[0].PlayerProfileId, Role = DAL.Models.GuildRole.Leader, Feats = 10000, JoinedAt = DateTime.UtcNow.AddDays(-30) },
                    new GuildMember { GuildId = guild1.GuildId, PlayerProfileId = botProfiles[3].PlayerProfileId, Role = DAL.Models.GuildRole.Officer, Feats = 5000, JoinedAt = DateTime.UtcNow.AddDays(-20) },
                    new GuildMember { GuildId = guild1.GuildId, PlayerProfileId = botProfiles[4].PlayerProfileId, Role = DAL.Models.GuildRole.Member, Feats = 200, JoinedAt = DateTime.UtcNow.AddDays(-5) }
                };

                // Guild 2
                members.Add(new GuildMember { GuildId = guild2.GuildId, PlayerProfileId = botProfiles[1].PlayerProfileId, Role = DAL.Models.GuildRole.Leader, Feats = 500, JoinedAt = DateTime.UtcNow.AddDays(-5) });
                members.Add(new GuildMember { GuildId = guild2.GuildId, PlayerProfileId = botProfiles[5].PlayerProfileId, Role = DAL.Models.GuildRole.Member, Feats = 10, JoinedAt = DateTime.UtcNow.AddDays(-1) });

                // Guild 3 (Chỉ có 1 leader cô đơn)
                members.Add(new GuildMember { GuildId = guild3.GuildId, PlayerProfileId = botProfiles[2].PlayerProfileId, Role = DAL.Models.GuildRole.Leader, Feats = 99999, JoinedAt = DateTime.UtcNow.AddDays(-15) });

                _ctx.GuildMembers.AddRange(members);
                await _ctx.SaveChangesAsync();

                await tx.CommitAsync();
                return Ok(new ApiResponse<object> { Success = true, Message = "Seeded 3 Guilds successfully!" });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return StatusCode(500, new ApiResponse<object> { Success = false, Message = ex.ToString(), ErrorCode = ErrorCodes.InternalError });
            }
        }

        // ─────────────────────────────────────────────────────────────────────────
        // POST /api/seed/testcoopskill
        // ─────────────────────────────────────────────────────────────────────────
        [HttpPost("testcoopskill")]
        public async Task<IActionResult> SeedTestCoopSkill()
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                var targetEmails = new[] { "elf3@mystic.test", "elf4@mystic.test" };
                
                // Cleanup existing
                var existingAccounts = await _ctx.Accounts
                    .Include(a => a.PlayerProfile)
                    .Where(a => targetEmails.Contains(a.Email))
                    .ToListAsync();
                
                foreach (var acc in existingAccounts)
                {
                    var pp = acc.PlayerProfile;
                    if (pp != null)
                    {
                        _ctx.PlayerSkills.RemoveRange(_ctx.PlayerSkills.Where(x => x.PlayerProfileId == pp.PlayerProfileId));
                        _ctx.PlayerSkins.RemoveRange(_ctx.PlayerSkins.Where(x => x.PlayerProfileId == pp.PlayerProfileId));
                        _ctx.PlayerStats.RemoveRange(_ctx.PlayerStats.Where(x => x.PlayerProfileId == pp.PlayerProfileId));
                        _ctx.PlayerProfiles.Remove(pp);
                    }
                    _ctx.Accounts.Remove(acc);
                }
                await _ctx.SaveChangesAsync();

                // Create Elf3 (Mage)
                var elf3Account = new Account { UserName = "elf3", Email = "elf3@mystic.test", HashPassword = HashPassword("Abc@12345"), RoleId = 1, IsActive = true, CreatedAt = DateTime.UtcNow };
                _ctx.Accounts.Add(elf3Account);
                await _ctx.SaveChangesAsync();

                var elf3Profile = new PlayerProfile { AccountId = elf3Account.AccountId, DisplayName = "Elf 3 Mage", Class = "Mage", Level = 10, Gold = 5000, Gems = 500, CurrentEnergy = 100, MaxEnergy = 100, LastEnergyUpdateTime = DateTime.UtcNow, LastMapName = "ElfForest", PositionX = 0, PositionY = 0, AvatarUrl = "" };
                _ctx.PlayerProfiles.Add(elf3Profile);
                await _ctx.SaveChangesAsync();

                _ctx.PlayerStats.Add(new PlayerStat
                {
                    PlayerProfileId = elf3Profile.PlayerProfileId,
                    CurrentHp = 500, MaxHp = 500,
                    Atk = 60, Def = 30,
                    MoveSpeed = 50, AttackSpeed = 100,
                    CritRate = 50, CritDamage = 150,
                    DamageBonus = 0,
                    SkillPoints = 10,
                });
                await _ctx.SaveChangesAsync();

                // Create Elf4 (Archer)
                var elf4Account = new Account { UserName = "elf4", Email = "elf4@mystic.test", HashPassword = HashPassword("Abc@12345"), RoleId = 1, IsActive = true, CreatedAt = DateTime.UtcNow };
                _ctx.Accounts.Add(elf4Account);
                await _ctx.SaveChangesAsync();

                var elf4Profile = new PlayerProfile { AccountId = elf4Account.AccountId, DisplayName = "Elf 4 Knight", Class = "Knight", Level = 10, Gold = 5000, Gems = 500, CurrentEnergy = 100, MaxEnergy = 100, LastEnergyUpdateTime = DateTime.UtcNow, LastMapName = "ElfForest", PositionX = 0, PositionY = 0, AvatarUrl = "" };
                _ctx.PlayerProfiles.Add(elf4Profile);
                await _ctx.SaveChangesAsync();

                _ctx.PlayerStats.Add(new PlayerStat
                {
                    PlayerProfileId = elf4Profile.PlayerProfileId,
                    CurrentHp = 500, MaxHp = 500,
                    Atk = 60, Def = 30,
                    MoveSpeed = 50, AttackSpeed = 100,
                    CritRate = 50, CritDamage = 150,
                    DamageBonus = 0,
                    SkillPoints = 10,
                });
                await _ctx.SaveChangesAsync();

                // Add skills for Mage (Elf 3)
                var mageSkills = await _ctx.Skills.Where(s => s.ClassRequirement == "Mage" || s.ClassRequirement == "All").ToListAsync();
                int slot = 0;
                foreach (var skill in mageSkills)
                {
                    _ctx.PlayerSkills.Add(new PlayerSkill
                    {
                        PlayerProfileId = elf3Profile.PlayerProfileId,
                        SkillId = skill.SkillId,
                        Level = 1,
                        EquippedSlot = slot < 3 ? slot : (int?)null
                    });
                    slot++;
                }

                // Add skins for Knight (Elf 4)
                var knightSkills = await _ctx.Skills.Where(s => s.ClassRequirement == "Knight" || s.ClassRequirement == "All").ToListAsync();
                slot = 0;
                foreach (var skill in knightSkills)
                {
                    _ctx.PlayerSkills.Add(new PlayerSkill
                    {
                        PlayerProfileId = elf4Profile.PlayerProfileId,
                        SkillId = skill.SkillId,
                        Level = 1,
                        EquippedSlot = slot < 3 ? slot : (int?)null
                    });
                    slot++;
                }

                // Equip skin for Mage
                var mageSkin = await _ctx.Skins.FirstOrDefaultAsync(s => s.Name.Contains("Mage") || s.Name.Contains("Elven Blade"));
                if (mageSkin != null)
                {
                    _ctx.PlayerSkins.Add(new PlayerSkin
                    {
                        PlayerProfileId = elf3Profile.PlayerProfileId,
                        SkinId = mageSkin.SkinId,
                        IsEquipped = true,
                        UnlockedAt = DateTime.UtcNow
                    });
                }

                // Equip skin for Knight
                var knightSkin = await _ctx.Skins.FirstOrDefaultAsync(s => s.Name.Contains("Knight") || s.Name.Contains("ElfForest Default"));
                if (knightSkin != null)
                {
                    _ctx.PlayerSkins.Add(new PlayerSkin
                    {
                        PlayerProfileId = elf4Profile.PlayerProfileId,
                        SkinId = knightSkin.SkinId,
                        IsEquipped = true,
                        UnlockedAt = DateTime.UtcNow
                    });
                }

                await _ctx.SaveChangesAsync();
                await tx.CommitAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Seeded elf3 (Mage) and elf4 (Knight) successfully with full skills!"
                });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return StatusCode(500, new ApiResponse<object> { Success = false, Message = ex.ToString(), ErrorCode = ErrorCodes.InternalError });
            }
        }

        private static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}

