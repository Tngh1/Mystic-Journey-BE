using BLL.DTOs;
using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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
    // POST /api/seed/mysticjourney → Seed toàn bộ dữ liệu mẫu (endpoint DUY NHẤT)
    //
    // Dữ liệu tạo ra:
    //   - Skills/monsters/drops/spawns cho ElfForest, 6 Dungeon, Content mẫu
    //   - 5 account test elf1..elf5@mystic.test, mỗi account đứng ở CUỐI chương
    //     tương ứng (elf1 = cuối chương 1 ... elf5 = cuối chương 5/hết game),
    //     đứng tại spawn point của map chương đó, đã nhận đủ item/skill/gold/exp
    //     mà một lượt chơi thật sự sẽ cấp tới điểm đó (xem ReplayChapterRewards).
    //   - 1 account admin@mysticjourney.com (role Admin) cho FE admin portal
    //   - 30 ngày DailyLoginReward
    //   - Lịch sử mua hàng mẫu (PurchaseHistory) cho 5 account trên, từ catalogue
    //     ShopItem đã có sẵn qua migration HasData
    //
    // =============================================================================
    [Route("api/[controller]")]
    [ApiController]
    // Toàn bộ seeder ghi thẳng vào DB (chèn item, monster, dungeon, guild,
    // transaction... và DELETE /api/seed/inventory xoá sạch dữ liệu mẫu), và mật
    // khẩu của các account test nằm hardcode trong source. Trước đây không có
    // attribute nào nên bất kỳ ai biết URL đều gọi được.
    //
    // Khoá theo MÔI TRƯỜNG, không theo role: seed là việc chạy tay từ Swagger/CLI
    // nên đòi cookie admin chỉ làm nó trả 401 và không dùng được; ngược lại trên
    // production thì cả controller đơn giản là không tồn tại (404).
    [AllowAnonymous]
    public class SeedController : ControllerBase, IActionFilter
    {
        private readonly MysticJourneyDbContext _ctx;
        private readonly IWebHostEnvironment _env;

        public SeedController(MysticJourneyDbContext ctx, IWebHostEnvironment env)
        {
            _ctx = ctx;
            _env = env;
        }

        // Chặn mọi action của controller này khi không phải Development.
        // [NonAction] là bắt buộc: MVC coi MỌI method public trên controller là
        // một action, nên nếu thiếu thì Swagger đổ "Ambiguous HTTP method for
        // action - SeedController.OnActionExecuting".
        [NonAction]
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!_env.IsDevelopment())
                context.Result = NotFound();
        }

        [NonAction]
        public void OnActionExecuted(ActionExecutedContext context) { }

        // Mốc chương: quest cuối cùng đã Claimed + map/spawn nơi account đứng.
        // Spawn world-position xác nhận trực tiếp trên scene Unity (không suy ra
        // qua offset container) ngày 2026-08-05:
        //   ElfForest / AutumnPumpkin: container ở gốc (0,0) nên toạ độ NPC = world.
        //   FrozenMountain: PlayerSpawnRuntime (24.0889,-49.7661) + SpawnPoint_Tutorial
        //     local (11.9,17.8) => world (35.9889,-31.9661). Giá trị cũ (-13.1,-44.2)
        //     dùng nhầm toạ độ PlayerSpawnRuntime trước khi nó bị dời trong các fix map
        //     sau này — xem memory map-spawn-coords-drift-from-seed.
        //   AbandonedCastle: PlayerSpawner.spawnPoint -> PlayerSpawnPoint (-12.36,60.14).
        private sealed record ChapterMilestone(string Email, string DisplayName, int LastQuestId, string Map, double X, double Y);

        private static readonly ChapterMilestone[] ChapterMilestones =
        {
            new("elf1@mystic.test", "Tutorial Archer 1", 0,  "ElfForest",      11.9,   17.8),
            new("elf2@mystic.test", "Tutorial Archer 2", 8,  "AutumnPumpkin", -130.2,   37.8),
            new("elf3@mystic.test", "Tutorial Archer 3", 20, "FrozenMountain", 35.9889, -31.9661),
            new("elf4@mystic.test", "Tutorial Archer 4", 27, "AbandonedCastle", -12.36,  60.14),
            new("elf5@mystic.test", "Tutorial Archer 5", 40, "ElfForest",      11.9,   17.8),
        };

        // ─────────────────────────────────────────────────────────────────────────
        // POST /api/seed/mysticjourney → Seed toàn bộ dữ liệu mẫu (single source of truth)
        // ─────────────────────────────────────────────────────────────────────────
        [HttpPost("mysticjourney")]
        public async Task<IActionResult> SeedMysticJourney()
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                await EnsureElfForestSchema();

                // 1. Lookup system items từ migration (không tạo item mới trong seed)
                var potion        = await _ctx.Items.FirstOrDefaultAsync(i => i.Name == "Small Health Potion");
                var sword         = await _ctx.Items.FirstOrDefaultAsync(i => i.Name == "Iron Sword");
                var armor         = await _ctx.Items.FirstOrDefaultAsync(i => i.Name == "Leather Armor");
                var whiteFlower   = await _ctx.Items.FirstOrDefaultAsync(i => i.Name == "White Flower");
                var upgradeStone  = await _ctx.Items.FirstOrDefaultAsync(i => i.Name == "Skill Upgrade Stone");
                var swampBook     = await _ctx.Items.FirstOrDefaultAsync(i => i.Name == "Swamp Seal Book");
                var dragonBook    = await _ctx.Items.FirstOrDefaultAsync(i => i.Name == "Dragon Seal Book");
                var golemBook     = await _ctx.Items.FirstOrDefaultAsync(i => i.Name == "Golem Seal Book");
                var underKingBook = await _ctx.Items.FirstOrDefaultAsync(i => i.Name == "UnderKing Seal Book");

                var missingItems = new List<string>();
                if (potion == null)        missingItems.Add("Small Health Potion");
                if (sword == null)         missingItems.Add("Iron Sword");
                if (armor == null)         missingItems.Add("Leather Armor");
                if (whiteFlower == null)   missingItems.Add("White Flower");
                if (upgradeStone == null)  missingItems.Add("Skill Upgrade Stone");
                if (swampBook == null)     missingItems.Add("Swamp Seal Book");
                if (dragonBook == null)    missingItems.Add("Dragon Seal Book");
                if (golemBook == null)     missingItems.Add("Golem Seal Book");
                if (underKingBook == null) missingItems.Add("UnderKing Seal Book");

                if (missingItems.Count > 0)
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = $"System items not found in DB. Please run items migration first. Missing: {string.Join(", ", missingItems)}"
                    });

                // 2. Skins are seeded in exact order matching Unity's SkinDatabase.asset
                var (skinKnight, skinArcher, skinMage, skinArcherPremium, skinKnightPremium, skinMagePremium) = await EnsureBaseSkinsAsync();

                // 3. Upsert tutorial skills (2 hắc hóa dùng chung)
                var elfSkillNames = new[] {
                    "Dark Poison Zone", "Dark Explosion"
                };
                var removedSkillNames = new[] {
                    "AP_Skill", "Skill_Ad", "Skill_Knight Attack", "Skill_Mui_Ten_Bang", "Skill_Thap_AS"
                };
                var obsoleteSkills = await _ctx.Skills.Where(s => removedSkillNames.Contains(s.Name)).ToListAsync();
                if (obsoleteSkills.Any())
                {
                    _ctx.Skills.RemoveRange(obsoleteSkills);
                }

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
                        existingElfSkills.Add(s);
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

                UpsertSkill("Dark Poison Zone", "Creates a poisonous zone dealing AoE damage. Darkening +10.", "Active", "Magical", "Area", "All", 6, 135.0, 16.0, 3.5, 10f);
                UpsertSkill("Dark Explosion", "Creates an explosion dealing massive damage. Darkening +5.", "Active", "Magical", "Area", "All", 8, 175.0, 20.0, 3.5, 5f);

                await _ctx.SaveChangesAsync();

                // 4. Remove obsolete tutorial monsters for ElfForest if present
                var obsoleteMonsterNames = new[] { "[ELF] Shadow Sprout", "[ELF] Forest Wolf", "[ELF] Sprout King" };
                var obsoleteMonsters = await _ctx.Monsters.Where(m => obsoleteMonsterNames.Contains(m.Name)).ToListAsync();
                if (obsoleteMonsters.Any())
                {
                    var obsoleteMonsterIds = obsoleteMonsters.Select(m => m.MonsterId).ToList();
                    var obsoleteDrops = await _ctx.MonsterDrops.Where(d => obsoleteMonsterIds.Contains(d.MonsterId)).ToListAsync();
                    var obsoleteSpawns = await _ctx.MonsterSpawns.Where(s => obsoleteMonsterIds.Contains(s.MonsterId)).ToListAsync();
                    _ctx.MonsterSpawns.RemoveRange(obsoleteSpawns);
                    _ctx.MonsterDrops.RemoveRange(obsoleteDrops);
                    _ctx.Monsters.RemoveRange(obsoleteMonsters);
                    await _ctx.SaveChangesAsync();
                }

                // 7. Create/update the 5 chapter-milestone accounts + replay their rewards
                var mainQuests = await _ctx.Quests
                    .Where(q => q.Type == "Main" && q.IsActive)
                    .Include(q => q.RewardItems)
                    .Include(q => q.RewardSkills)
                    .OrderBy(q => q.QuestId)
                    .ToListAsync();

                var allSkills = await _ctx.Skills.ToListAsync();

                var playerIds = new List<int>();
                foreach (var milestone in ChapterMilestones)
                {
                    int pid = await UpsertChapterAccount(
                        milestone, mainQuests, allSkills,
                        potion, sword, armor, upgradeStone,
                        swampBook, dragonBook, golemBook, underKingBook,
                        skinKnight, skinArcher, skinMage);
                    playerIds.Add(pid);
                }
                await _ctx.SaveChangesAsync();

                await SeedGachaBaseDataAsync("elf1@mystic.test", 11);

                await tx.CommitAsync();

                // Independent sub-seeds (each self-contained; safe to run after the
                // core commit so a failure here doesn't roll back the accounts above)
                await SeedDungeons();
                await SeedContent();
                await SeedAdminAccount();
                await SeedDailyLoginRewards();
                await SeedShopPurchaseHistory(playerIds);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Seed MysticJourney completed successfully!",
                    Data = new { players = playerIds, milestones = ChapterMilestones.Select(m => new { m.Email, endOfChapterQuestId = m.LastQuestId, m.Map }) }
                });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return StatusCode(500, new ApiResponse<object> { Success = false, Message = ex.ToString(), ErrorCode = ErrorCodes.InternalError });
            }
        }

        // Tạo/ghi đè 1 account đứng ở cuối chương: quest 1..LastQuestId Claimed (đã
        // replay đủ gold/exp/gems/item/skill mà ClaimRewardCore thực sự cấp), quest
        // kế tiếp NotStarted, các quest sau không insert (để IsMainQuestUnlocked hoạt
        // động đúng như một account thật đang đứng ở mốc đó).
        private async Task<int> UpsertChapterAccount(
            ChapterMilestone milestone, List<Quest> mainQuests, List<Skill> allSkills,
            Item potion, Item sword, Item armor, Item upgradeStone,
            Item swampBook, Item dragonBook, Item golemBook, Item underKingBook,
            Skin skinKnight, Skin skinArcher, Skin skinMage)
        {
            var username = milestone.Email.Split('@')[0].Replace(".", "_");

            var account = await _ctx.Accounts
                .Include(a => a.PlayerProfile)
                .FirstOrDefaultAsync(a => a.Email == milestone.Email || a.UserName == username);

            if (account == null)
            {
                account = new Account { CreatedAt = DateTime.UtcNow };
                _ctx.Accounts.Add(account);
            }

            account.UserName = username;
            account.Email = milestone.Email;
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
                profile = new PlayerProfile { AccountId = account.AccountId, CreatedAt = DateTime.UtcNow };
                _ctx.PlayerProfiles.Add(profile);
                await _ctx.SaveChangesAsync();
            }

            int pid = profile.PlayerProfileId;

            // Rebuild toàn bộ dữ liệu phụ thuộc của account này (idempotent theo pid)
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
            _ctx.Mailboxes.RemoveRange(_ctx.Mailboxes.Where(x => x.PlayerProfileId == pid));
            _ctx.GuildMembers.RemoveRange(_ctx.GuildMembers.Where(x => x.PlayerProfileId == pid));
            await _ctx.SaveChangesAsync();

            const string cls = "Archer";
            profile.DisplayName = milestone.DisplayName;
            profile.Class = cls;
            profile.ExperiencePoints = 0;
            profile.Gold = 100;
            profile.Gems = 0;
            profile.CurrentEnergy = 100;
            profile.MaxEnergy = 100;
            profile.LastEnergyUpdateTime = DateTime.UtcNow;
            profile.LastMapName = milestone.Map;
            profile.PositionX = milestone.X;
            profile.PositionY = milestone.Y;
            profile.AvatarUrl = string.Empty;
            profile.UpdatedAt = DateTime.UtcNow;
            profile.Level = 1;
            await _ctx.SaveChangesAsync();

            // Quests: mọi quest QuestId <= LastQuestId = Claimed; quest kế tiếp = NotStarted.
            var claimedQuests = mainQuests.Where(q => q.QuestId <= milestone.LastQuestId).ToList();
            var nextQuest = mainQuests.FirstOrDefault(q => q.QuestId == milestone.LastQuestId + 1);

            foreach (var quest in claimedQuests)
            {
                var targetAmount = Math.Max(1, quest.TargetAmount);
                _ctx.PlayerQuests.Add(new PlayerQuest
                {
                    PlayerProfileId = pid,
                    QuestId = quest.QuestId,
                    Status = "Claimed",
                    TargetValue = targetAmount,
                    Progress = targetAmount,
                    AcceptedAt = DateTime.UtcNow,
                    CompletedAt = DateTime.UtcNow,
                    ClaimedAt = DateTime.UtcNow
                });
            }
            if (nextQuest != null)
            {
                _ctx.PlayerQuests.Add(new PlayerQuest
                {
                    PlayerProfileId = pid,
                    QuestId = nextQuest.QuestId,
                    Status = "NotStarted",
                    TargetValue = Math.Max(1, nextQuest.TargetAmount),
                    Progress = 0,
                    AcceptedAt = DateTime.UtcNow
                });
            }
            await _ctx.SaveChangesAsync();

            // Replay rewards for every claimed quest (gold/exp/gems/items/skills),
            // mirroring PlayerQuestService.ClaimRewardCore precedence rules.
            decimal goldTotal = 100m;
            decimal gemsTotal = 0m;
            int expTotal = 0;
            var grantedItemQty = new Dictionary<int, int>();

            foreach (var quest in claimedQuests)
            {
                goldTotal += quest.RewardGold;
                gemsTotal += quest.RewardGems;
                expTotal += quest.RewardExperience;

                var rewardItems = quest.RewardItems
                    .Where(ri => ri.ItemId > 0 && ri.Quantity > 0)
                    .ToList();
                if (rewardItems.Count > 0)
                {
                    foreach (var ri in rewardItems)
                        grantedItemQty[ri.ItemId] = grantedItemQty.GetValueOrDefault(ri.ItemId) + Math.Max(1, ri.Quantity);
                }
                else if (quest.RewardItemId.HasValue)
                {
                    grantedItemQty[quest.RewardItemId.Value] = grantedItemQty.GetValueOrDefault(quest.RewardItemId.Value) + 1;
                }
            }

            profile.Gold = goldTotal;
            profile.Gems = gemsTotal;
            profile.AddExperience(expTotal);
            await _ctx.SaveChangesAsync();

            // Base loadout
            _ctx.InventoryItems.Add(new InventoryItem { PlayerProfileId = pid, ItemId = sword.ItemId, Quantity = 1, IsEquipped = true, IsSkin = false, EquippedSlot = "Weapon" });
            _ctx.InventoryItems.Add(new InventoryItem { PlayerProfileId = pid, ItemId = armor.ItemId, Quantity = 1, IsEquipped = true, IsSkin = false, EquippedSlot = "Armor" });
            _ctx.InventoryItems.Add(new InventoryItem { PlayerProfileId = pid, ItemId = potion.ItemId, Quantity = 3, IsEquipped = false, IsSkin = false });
            _ctx.InventoryItems.Add(new InventoryItem { PlayerProfileId = pid, ItemId = upgradeStone.ItemId, Quantity = 99, IsEquipped = false, IsSkin = false });

            // Seal Books (story books of completed chapter boss quests)
            if (milestone.LastQuestId >= 6 && swampBook != null)
                _ctx.InventoryItems.Add(new InventoryItem { PlayerProfileId = pid, ItemId = swampBook.ItemId, Quantity = 1, IsEquipped = false, IsSkin = false });
            if (milestone.LastQuestId >= 19 && dragonBook != null)
                _ctx.InventoryItems.Add(new InventoryItem { PlayerProfileId = pid, ItemId = dragonBook.ItemId, Quantity = 1, IsEquipped = false, IsSkin = false });
            if (milestone.LastQuestId >= 26 && golemBook != null)
                _ctx.InventoryItems.Add(new InventoryItem { PlayerProfileId = pid, ItemId = golemBook.ItemId, Quantity = 1, IsEquipped = false, IsSkin = false });
            if (milestone.LastQuestId >= 39 && underKingBook != null)
                _ctx.InventoryItems.Add(new InventoryItem { PlayerProfileId = pid, ItemId = underKingBook.ItemId, Quantity = 1, IsEquipped = false, IsSkin = false });

            // Quest-granted items on top of the base loadout
            foreach (var (itemId, qty) in grantedItemQty)
            {
                if (itemId == sword.ItemId || itemId == armor.ItemId || itemId == potion.ItemId || itemId == upgradeStone.ItemId ||
                    (swampBook != null && itemId == swampBook.ItemId) ||
                    (dragonBook != null && itemId == dragonBook.ItemId) ||
                    (golemBook != null && itemId == golemBook.ItemId) ||
                    (underKingBook != null && itemId == underKingBook.ItemId))
                    continue; // already covered above
                _ctx.InventoryItems.Add(new InventoryItem { PlayerProfileId = pid, ItemId = itemId, Quantity = qty, IsEquipped = false, IsSkin = false });
            }

            // Default class skin
            _ctx.PlayerSkins.Add(new PlayerSkin { PlayerProfileId = pid, SkinId = skinArcher.SkinId, IsEquipped = true, UnlockedAt = DateTime.UtcNow });

            // Skills: claiming "[Chapter 1] Gather White Flowers" (Q2) triggers
            // PlayerQuestService's tutorial hack that unlocks EVERY skill in the
            // table — Q2 is Claimed for every milestone here, so every account ends
            // up owning the full skill list, matching a real playthrough.
            foreach (var skill in allSkills)
            {
                _ctx.PlayerSkills.Add(new PlayerSkill { PlayerProfileId = pid, SkillId = skill.SkillId, Level = 1, Experience = 0, UnlockedAt = DateTime.UtcNow });
            }

            // Stat baseline sized for QA/testing (not a balance reference) so every
            // milestone account can survive its chapter's content immediately.
            _ctx.PlayerStats.Add(new PlayerStat
            {
                PlayerProfileId = pid,
                CurrentHp = 2000,
                MaxHp = 2000,
                Atk = 2000,
                Def = 50,
                MoveSpeed = 50,
                AttackSpeed = 100,
                CritRate = 50,
                CritDamage = 150,
            });

            await _ctx.SaveChangesAsync();
            return pid;
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
                throw new InvalidOperationException($"System item '{name}' not found in DB. Please run items migration first.");
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

        // Seed 30 ngày DailyLoginReward (Gold/Energy/Gems/Item luân phiên, milestone
        // item ở ngày 7/14/21/28). Idempotent: xoá hết reward cũ rồi chèn lại.
        private async Task SeedDailyLoginRewards()
        {
            var existingRewards = await _ctx.DailyLoginRewards.ToListAsync();
            _ctx.DailyLoginRewards.RemoveRange(existingRewards);
            await _ctx.SaveChangesAsync();

            var potion = await _ctx.Items.FirstOrDefaultAsync(i => i.Name == "Small Health Potion");
            var sword  = await _ctx.Items.FirstOrDefaultAsync(i => i.Name == "Iron Sword");
            var helm   = await _ctx.Items.FirstOrDefaultAsync(i => i.Name == "Iron Helmet");

            var rewards = new List<DailyLoginReward>
            {
                new DailyLoginReward { DayNumber = 1,  RewardType = "Gold",   RewardValue = 100,  IsActive = true, CreatedAt = DateTime.UtcNow },
                new DailyLoginReward { DayNumber = 2,  RewardType = "Energy", RewardValue = 20,   IsActive = true, CreatedAt = DateTime.UtcNow },
                new DailyLoginReward { DayNumber = 3,  RewardType = "Gold",   RewardValue = 200,  IsActive = true, CreatedAt = DateTime.UtcNow },
                new DailyLoginReward { DayNumber = 4,  RewardType = "Gems",   RewardValue = 5,    IsActive = true, CreatedAt = DateTime.UtcNow },
                new DailyLoginReward { DayNumber = 5,  RewardType = "Gold",   RewardValue = 300,  IsActive = true, CreatedAt = DateTime.UtcNow },
                new DailyLoginReward { DayNumber = 6,  RewardType = "Energy", RewardValue = 30,   IsActive = true, CreatedAt = DateTime.UtcNow },
                new DailyLoginReward { DayNumber = 7,  RewardType = "Item", RewardValue = 0, RewardItemId = potion?.ItemId, RewardItemQuantity = potion != null ? 3 : 0, IsActive = true, CreatedAt = DateTime.UtcNow },

                new DailyLoginReward { DayNumber = 8,  RewardType = "Gold",   RewardValue = 400,  IsActive = true, CreatedAt = DateTime.UtcNow },
                new DailyLoginReward { DayNumber = 9,  RewardType = "Gems",   RewardValue = 10,   IsActive = true, CreatedAt = DateTime.UtcNow },
                new DailyLoginReward { DayNumber = 10, RewardType = "Gold",   RewardValue = 500,  IsActive = true, CreatedAt = DateTime.UtcNow },
                new DailyLoginReward { DayNumber = 11, RewardType = "Energy", RewardValue = 40,   IsActive = true, CreatedAt = DateTime.UtcNow },
                new DailyLoginReward { DayNumber = 12, RewardType = "Gold",   RewardValue = 600,  IsActive = true, CreatedAt = DateTime.UtcNow },
                new DailyLoginReward { DayNumber = 13, RewardType = "Gems",   RewardValue = 15,   IsActive = true, CreatedAt = DateTime.UtcNow },
                new DailyLoginReward { DayNumber = 14, RewardType = "Item", RewardValue = 0, RewardItemId = helm?.ItemId, RewardItemQuantity = helm != null ? 1 : 0, IsActive = true, CreatedAt = DateTime.UtcNow },

                new DailyLoginReward { DayNumber = 15, RewardType = "Gold",   RewardValue = 800,  IsActive = true, CreatedAt = DateTime.UtcNow },
                new DailyLoginReward { DayNumber = 16, RewardType = "Gems",   RewardValue = 20,   IsActive = true, CreatedAt = DateTime.UtcNow },
                new DailyLoginReward { DayNumber = 17, RewardType = "Energy", RewardValue = 50,   IsActive = true, CreatedAt = DateTime.UtcNow },
                new DailyLoginReward { DayNumber = 18, RewardType = "Gold",   RewardValue = 900,  IsActive = true, CreatedAt = DateTime.UtcNow },
                new DailyLoginReward { DayNumber = 19, RewardType = "Gems",   RewardValue = 25,   IsActive = true, CreatedAt = DateTime.UtcNow },
                new DailyLoginReward { DayNumber = 20, RewardType = "Gold",   RewardValue = 1000, IsActive = true, CreatedAt = DateTime.UtcNow },
                new DailyLoginReward { DayNumber = 21, RewardType = "Item", RewardValue = 0, RewardItemId = potion?.ItemId, RewardItemQuantity = potion != null ? 5 : 0, IsActive = true, CreatedAt = DateTime.UtcNow },

                new DailyLoginReward { DayNumber = 22, RewardType = "Gold",   RewardValue = 1100, IsActive = true, CreatedAt = DateTime.UtcNow },
                new DailyLoginReward { DayNumber = 23, RewardType = "Energy", RewardValue = 60,   IsActive = true, CreatedAt = DateTime.UtcNow },
                new DailyLoginReward { DayNumber = 24, RewardType = "Gems",   RewardValue = 30,   IsActive = true, CreatedAt = DateTime.UtcNow },
                new DailyLoginReward { DayNumber = 25, RewardType = "Gold",   RewardValue = 1200, IsActive = true, CreatedAt = DateTime.UtcNow },
                new DailyLoginReward { DayNumber = 26, RewardType = "Gems",   RewardValue = 35,   IsActive = true, CreatedAt = DateTime.UtcNow },
                new DailyLoginReward { DayNumber = 27, RewardType = "Energy", RewardValue = 70,   IsActive = true, CreatedAt = DateTime.UtcNow },
                new DailyLoginReward { DayNumber = 28, RewardType = "Item", RewardValue = 0, RewardItemId = sword?.ItemId, RewardItemQuantity = sword != null ? 1 : 0, IsActive = true, CreatedAt = DateTime.UtcNow },

                new DailyLoginReward { DayNumber = 29, RewardType = "Gems",   RewardValue = 50,   IsActive = true, CreatedAt = DateTime.UtcNow },
                new DailyLoginReward { DayNumber = 30, RewardType = "Gold",   RewardValue = 2000, IsActive = true, CreatedAt = DateTime.UtcNow },
            };

            _ctx.DailyLoginRewards.AddRange(rewards);
            await _ctx.SaveChangesAsync();
        }

        // Tạo/ghi đè account admin@mysticjourney.com cho FE admin portal.
        private async Task SeedAdminAccount()
        {
            const string adminEmail = "admin@mysticjourney.com";
            const string adminUsername = "admin";
            const string adminPassword = "AdminPassword123!";

            var adminAcc = await _ctx.Accounts
                .Include(a => a.PlayerProfile)
                .FirstOrDefaultAsync(a => a.Email == adminEmail || a.UserName == adminUsername);

            if (adminAcc == null)
            {
                adminAcc = new Account
                {
                    Email = adminEmail,
                    UserName = adminUsername,
                    HashPassword = HashPassword(adminPassword),
                    RoleId = 2, // Admin
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    PlayerProfile = new PlayerProfile
                    {
                        DisplayName = "System Admin",
                        Class = "Knight",
                        Level = 99,
                        Gold = 100000,
                        Gems = 10000,
                        CreatedAt = DateTime.UtcNow
                    }
                };
                _ctx.Accounts.Add(adminAcc);
            }
            else
            {
                adminAcc.HashPassword = HashPassword(adminPassword);
                adminAcc.IsActive = true;
                adminAcc.RoleId = 2;
                adminAcc.UpdatedAt = DateTime.UtcNow;
            }

            await _ctx.SaveChangesAsync();
        }

        // Lịch sử mua hàng mẫu cho các account chapter-milestone, rút từ catalogue
        // ShopItem đã seed sẵn qua migration HasData (không tạo item/shopitem giả).
        private async Task SeedShopPurchaseHistory(List<int> playerIds)
        {
            _ctx.PurchaseHistories.RemoveRange(_ctx.PurchaseHistories.Where(h => playerIds.Contains(h.PlayerProfileId)));
            await _ctx.SaveChangesAsync();

            var shopItems = await _ctx.ShopItems.Where(s => s.IsActive).ToListAsync();
            if (shopItems.Count == 0)
                return;

            var rnd = new Random();
            var histories = new List<PurchaseHistory>();
            foreach (var pid in playerIds)
            {
                for (int i = 0; i < 5; i++)
                {
                    var shopItem = shopItems[rnd.Next(shopItems.Count)];
                    int qty = rnd.Next(1, 4);
                    histories.Add(new PurchaseHistory
                    {
                        PlayerProfileId = pid,
                        ShopItemId = shopItem.ShopItemId,
                        Quantity = qty,
                        TotalPrice = shopItem.Price * qty,
                        PurchasedAt = DateTime.UtcNow.AddDays(-rnd.Next(0, 10)).AddHours(-rnd.Next(0, 24))
                    });
                }
            }

            _ctx.PurchaseHistories.AddRange(histories);
            await _ctx.SaveChangesAsync();
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
        // Seed 6 Dungeon mẫu để test luồng Game (không phải route riêng, gọi từ
        // SeedMysticJourney)
        // ─────────────────────────────────────────────────────────────────────────
        private async Task SeedDungeons()
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
            //  8   |  SlimeIce            | Normal
            //  9   |  IceDragon           | Normal
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

            // ponytail: giữ ĐỒNG BỘ với modelBuilder.Entity<Monster>().HasData trong
            // MysticJourneyDbContext. Trước đây hàm này chỉ nhận (hp, atk, def) nên mọi
            // quái do nó tạo ra có Level=0, MoveSpeed=0 và CritDamage=0 — tức là không
            // bao giờ đuổi được người chơi (EnemyBehaviour tính MoveSpeed/100*3.5) và
            // đòn "chí mạng" chỉ gây 0x sát thương. Nhận đủ chỉ số để tránh hai lỗi đó.
            void EnsureMonster(int id, string name, string type, int level, int hp, int atk, int def,
                               int moveSpeed, int attackSpeed, int critRate, int critDamage,
                               int expReward, decimal goldReward, decimal gemReward = 0m)
            {
                var m = _ctx.Monsters.Local.FirstOrDefault(x => x.MonsterId == id)
                     ?? _ctx.Monsters.Find(id);
                if (m == null)
                {
                    m = new Monster { MonsterId = id };
                    _ctx.Monsters.Add(m);
                }

                // Ghi đè có chủ ý: seed endpoint là nguồn sự thật duy nhất khi chạy lại,
                // nếu chỉ insert-khi-thiếu thì các hàng có số liệu cũ sẽ sống mãi.
                m.Name = name;
                m.Type = type;
                m.Level = level;
                m.MaxHp = hp;
                m.Atk = atk;
                m.Def = def;
                m.MoveSpeed = moveSpeed;
                m.AttackSpeed = attackSpeed;
                m.CritRate = critRate;
                m.CritDamage = critDamage;
                m.ExperienceReward = expReward;
                m.GoldReward = goldReward;
                m.GemReward = gemReward;
                m.IsActive = true;
            }

            // Normal monsters
            EnsureMonster(1,   "SlimeLittle",         "Normal",    1,   300,  30,   2,   70,  85,  5, 130,    4,     8m);
            EnsureMonster(3,   "WaterElemental",      "Normal",    3,   400,  39,   5,   80,  95,  8, 140,    4,     8m);
            EnsureMonster(4,   "Dragon",              "Normal",    6,   560,  47,  12,  110, 100, 15, 160,    6,    13m);
            EnsureMonster(5,   "BlueDragonFrost",     "Normal",    7,   580,  48,  14,  110, 100, 15, 160,    6,    13m);
            EnsureMonster(6,   "GreenDragonForest",   "Normal",    7,   590,  49,  15,  110, 105, 15, 160,    6,    13m);
            EnsureMonster(8,   "SlimeIce",            "Normal",    7,   620,  50,  15,   75,  90, 10, 150,   10,    19m);
            EnsureMonster(9,   "IceDragon",           "Normal",    9,   840,  55,  18,  115, 105, 20, 165,   10,    19m);
            EnsureMonster(11,  "OrcSkeleton",         "Normal",    9,   850,  61,  20,   95, 100, 15, 160,   13,    26m);
            EnsureMonster(12,  "SkeletonMelee",       "Normal",   11,  1050,  71,  22,  100, 105, 15, 160,   13,    26m);
            EnsureMonster(13,  "SkeletonArcher",      "Normal",   12,  1160,  78,  16,  100, 115, 22, 165,   13,    26m);
            EnsureMonster(14,  "Ghost",               "Normal",    4,   480,  42,  10,   95, 100, 15, 160,    6,    13m);
            EnsureMonster(16,  "Demon",               "Normal",    8,   730,  51,  18,   95, 100, 20, 165,   10,    19m);
            EnsureMonster(17,  "GoblinWarrior",       "Normal",    5,   530,  45,  13,   95, 100, 12, 150,    6,    13m);
            EnsureMonster(18,  "GoblinSpear",         "Normal",    5,   510,  44,  10,  100, 100, 10, 150,    6,    13m);
            EnsureMonster(23,  "NecromancerCast",     "Normal",    4,   500,  43,   7,   85,  90, 10, 155,    6,    13m);
            EnsureMonster(24,  "RobberArcher",        "Normal",    3,   440,  40,   6,  100, 110, 12, 150,    6,    13m);
            EnsureMonster(25,  "RobberAssassin",      "Normal",    3,   460,  41,   9,  105, 115, 18, 160,    6,    13m);
            EnsureMonster(26,  "RedGuard",            "Normal",    6,   540,  46,  15,   85,  95, 10, 150,    6,    13m);
            EnsureMonster(27,  "OrcSkeletonAfk",      "Normal",   10,   950,  65,  24,   90,  95, 15, 160,   13,    26m);

            // Boss monsters
            EnsureMonster(2,   "SwampDemon",          "Boss",      3,  1380,  32,  10,   90, 100, 12, 150,   22,   110m, 5m);
            EnsureMonster(7,   "DragonBossIdle",      "Boss",      7,  2930,  53,  22,    0, 100, 20, 175,   35,   176m, 10m);
            EnsureMonster(10,  "GolemBoss",           "Boss",      9,  4300,  65,  28,   80,  90, 20, 170,   53,   264m, 15m);
            EnsureMonster(15,  "UnderKing",           "Boss",     12,  6040,  94,  35,   95, 100, 25, 180,   70,   352m, 30m);
            EnsureMonster(19,  "Ogre",                "Boss",      7,  2560,  46,  19,   85,  90, 15, 165,   35,   176m, 10m);
            EnsureMonster(20,  "OrcWarlord",          "Boss",     12,  4490,  73,  30,   95, 100, 22, 175,   70,   352m, 30m);
            EnsureMonster(21,  "IceFairy",            "Boss",      9,  3230,  54,  16,  100, 100, 12, 150,   53,   264m, 15m);
            EnsureMonster(22,  "GoblinWarlord",       "Boss",      7,  2180,  41,  18,   95, 100, 18, 165,   35,   176m, 10m);

            await _ctx.SaveChangesAsync();

            // ─── 3. Tạo 6 Dungeon (ID phải khớp với Unity DungeonConfig) ────────
            var dungeons = new List<Dungeon>
            {
                new Dungeon { DungeonId = 1, Name = "Slime Swamp",          Description = "Realm of dangerous Slimes",           IsRepeatable = true },
                new Dungeon { DungeonId = 2, Name = "Dragon's Lair",        Description = "The den of ferocious dragons",        IsRepeatable = true },
                new Dungeon { DungeonId = 3, Name = "Frozen Palace",        Description = "Ice fortress of the giant Golem",     IsRepeatable = true },
                new Dungeon { DungeonId = 4, Name = "Shadow Graveyard",     Description = "Underground kingdom of the Bone King",IsRepeatable = true },
                new Dungeon { DungeonId = 5, Name = "Goblin Camp",          Description = "Stronghold of Goblins and Ogres",     IsRepeatable = true },
                new Dungeon { DungeonId = 6, Name = "Hell's Gate",          Description = "Portal to the realm of Demons and Orc Warriors", IsRepeatable = true },
            };
            _ctx.Dungeons.AddRange(dungeons);

            // ─── 4. Tạo Chest và DungeonConfig khớp với Unity ─────────────────────────────

            // Clear old chests associated with old dungeon configs
            var chestIdsToRemove = existingConfigs.Where(c => c.ChestId.HasValue).Select(c => c.ChestId.Value).ToList();
            if (chestIdsToRemove.Any())
            {
                var chestsToRemove = await _ctx.Chests.Where(c => chestIdsToRemove.Contains(c.ChestId)).ToListAsync();
                _ctx.Chests.RemoveRange(chestsToRemove);
                await _ctx.SaveChangesAsync();
            }

            // Create chests for each dungeon
            var chest1 = new Chest { Name = "Slime Swamp Chest", Description = "Slime Swamp reward", Type = "Normal", GoldMinReward = 50, GoldMaxReward = 100, ExperienceReward = 50, IsActive = true };
            var chest2 = new Chest { Name = "Dragon Lair Chest", Description = "Dragon Lair reward", Type = "Normal", GoldMinReward = 100, GoldMaxReward = 200, ExperienceReward = 150, IsActive = true };
            var chest3 = new Chest { Name = "Ice Palace Chest", Description = "Ice Palace reward", Type = "Normal", GoldMinReward = 150, GoldMaxReward = 300, ExperienceReward = 300, IsActive = true };
            var chest4 = new Chest { Name = "Dark Graveyard Chest", Description = "Dark Graveyard reward", Type = "Normal", GoldMinReward = 200, GoldMaxReward = 400, ExperienceReward = 450, IsActive = true };
            var chest5 = new Chest { Name = "Goblin Camp Chest", Description = "Goblin Camp reward", Type = "Normal", GoldMinReward = 150, GoldMaxReward = 300, ExperienceReward = 350, IsActive = true };
            var chest6 = new Chest { Name = "Hell Gate Chest", Description = "Hell Gate reward", Type = "Epic", GoldMinReward = 500, GoldMaxReward = 1000, ExperienceReward = 1000, IsActive = true };

            _ctx.Chests.AddRange(chest1, chest2, chest3, chest4, chest5, chest6);
            await _ctx.SaveChangesAsync();

            // Add item drops to these chests
            var hpPotion = await _ctx.Items.FirstOrDefaultAsync(i => i.Name == "Small Health Potion");
            var mpPotion = await _ctx.Items.FirstOrDefaultAsync(i => i.Name == "Small Mana Potion");
            var ironSword = await _ctx.Items.FirstOrDefaultAsync(i => i.Name == "Iron Sword");

            var chestItems = new List<ChestItem>();
            foreach (var chest in new[] { chest1, chest2, chest3, chest4, chest5, chest6 })
            {
                if (hpPotion != null)
                    chestItems.Add(new ChestItem { ChestId = chest.ChestId, ItemId = hpPotion.ItemId, DropRate = 80.0m, QuantityMin = 1, QuantityMax = 3 });
                if (mpPotion != null)
                    chestItems.Add(new ChestItem { ChestId = chest.ChestId, ItemId = mpPotion.ItemId, DropRate = 60.0m, QuantityMin = 1, QuantityMax = 2 });
                if (ironSword != null && chest.Type == "Epic") // Cổng địa ngục is Epic
                    chestItems.Add(new ChestItem { ChestId = chest.ChestId, ItemId = ironSword.ItemId, DropRate = 30.0m, QuantityMin = 1, QuantityMax = 1 });
            }
            if (chestItems.Any())
            {
                _ctx.ChestItems.AddRange(chestItems);
                await _ctx.SaveChangesAsync();
            }

            var dungeonConfigs = new List<DungeonConfig>
            {
                new DungeonConfig { DungeonConfigId = 1, Name = "Slime Swamp",      Description = "Realm of dangerous Slimes",           Type = "Normal", LevelRequirement = 1,  MaxMembers = 4, Difficulty = 1, EnergyCost = 10, RecommendedPower = 100,  IsActive = true, ChestId = chest1.ChestId },
                new DungeonConfig { DungeonConfigId = 2, Name = "Dragon's Lair",    Description = "The den of ferocious dragons",        Type = "Normal", LevelRequirement = 3,  MaxMembers = 4, Difficulty = 2, EnergyCost = 15, RecommendedPower = 300,  IsActive = true, ChestId = chest2.ChestId },
                new DungeonConfig { DungeonConfigId = 3, Name = "Frozen Palace",    Description = "Ice fortress of the giant Golem",     Type = "Normal", LevelRequirement = 10, MaxMembers = 4, Difficulty = 3, EnergyCost = 20, RecommendedPower = 600,  IsActive = true, ChestId = chest3.ChestId },
                new DungeonConfig { DungeonConfigId = 4, Name = "Shadow Graveyard", Description = "Underground kingdom of the Bone King",Type = "Normal", LevelRequirement = 15, MaxMembers = 4, Difficulty = 4, EnergyCost = 25, RecommendedPower = 900,  IsActive = true, ChestId = chest4.ChestId },
                new DungeonConfig { DungeonConfigId = 5, Name = "Goblin Camp",      Description = "Stronghold of Goblins and Ogres",     Type = "Normal", LevelRequirement = 10, MaxMembers = 4, Difficulty = 3, EnergyCost = 20, RecommendedPower = 700,  IsActive = true, ChestId = chest5.ChestId },
                new DungeonConfig { DungeonConfigId = 6, Name = "Hell's Gate",      Description = "Portal to the realm of Demons and Orc Warriors", Type = "Boss",   LevelRequirement = 20, MaxMembers = 4, Difficulty = 5, EnergyCost = 30, RecommendedPower = 1500, IsActive = true, ChestId = chest6.ChestId },
            };
            _ctx.DungeonConfigs.AddRange(dungeonConfigs);
            await _ctx.SaveChangesAsync();

            // ─── 5. Tạo MonsterSpawns cho từng Dungeon ───────────────────────────
            // MapName phải khớp với scene name trong Unity
            string mapName = "HollowCryptDungeon";

            var spawns = new List<MonsterSpawn>
            {
                // ── Dungeon 1: Đầm lầy Slime ─────────────────────────────────────
                new MonsterSpawn { DungeonId = 1, MonsterId = 1,  SpawnCount = 3, MapName = mapName, IsActive = true },
                new MonsterSpawn { DungeonId = 1, MonsterId = 8,  SpawnCount = 3, MapName = mapName, IsActive = true },
                new MonsterSpawn { DungeonId = 1, MonsterId = 2,  SpawnCount = 1, MapName = mapName, IsActive = true },

                // ── Dungeon 2: Sào huyệt Rồng ────────────────────────────────────
                new MonsterSpawn { DungeonId = 2, MonsterId = 4,  SpawnCount = 2, MapName = mapName, IsActive = true },
                new MonsterSpawn { DungeonId = 2, MonsterId = 5,  SpawnCount = 2, MapName = mapName, IsActive = true },
                new MonsterSpawn { DungeonId = 2, MonsterId = 6,  SpawnCount = 2, MapName = mapName, IsActive = true },
                new MonsterSpawn { DungeonId = 2, MonsterId = 7,  SpawnCount = 1, MapName = mapName, IsActive = true },

                // ── Dungeon 3: Cung điện Băng giá ────────────────────────────────
                new MonsterSpawn { DungeonId = 3, MonsterId = 8,  SpawnCount = 3, MapName = mapName, IsActive = true },
                new MonsterSpawn { DungeonId = 3, MonsterId = 9,  SpawnCount = 3, MapName = mapName, IsActive = true },
                new MonsterSpawn { DungeonId = 3, MonsterId = 10, SpawnCount = 1, MapName = mapName, IsActive = true },

                // ── Dungeon 4: Nghĩa địa Bóng tối ────────────────────────────────
                new MonsterSpawn { DungeonId = 4, MonsterId = 12, SpawnCount = 3, MapName = mapName, IsActive = true },
                new MonsterSpawn { DungeonId = 4, MonsterId = 13, SpawnCount = 2, MapName = mapName, IsActive = true },
                new MonsterSpawn { DungeonId = 4, MonsterId = 11, SpawnCount = 2, MapName = mapName, IsActive = true },
                new MonsterSpawn { DungeonId = 4, MonsterId = 15, SpawnCount = 1, MapName = mapName, IsActive = true },

                // ── Dungeon 5: Doanh trại Goblin ─────────────────────────────────
                new MonsterSpawn { DungeonId = 5, MonsterId = 17, SpawnCount = 3, MapName = mapName, IsActive = true },
                new MonsterSpawn { DungeonId = 5, MonsterId = 18, SpawnCount = 3, MapName = mapName, IsActive = true },
                new MonsterSpawn { DungeonId = 5, MonsterId = 19, SpawnCount = 1, MapName = mapName, IsActive = true },

                // ── Dungeon 6: Cổng địa ngục ─────────────────────────────────────
                new MonsterSpawn { DungeonId = 6, MonsterId = 14, SpawnCount = 3, MapName = mapName, IsActive = true },
                new MonsterSpawn { DungeonId = 6, MonsterId = 16, SpawnCount = 2, MapName = mapName, IsActive = true },
                new MonsterSpawn { DungeonId = 6, MonsterId = 11, SpawnCount = 2, MapName = mapName, IsActive = true },
                new MonsterSpawn { DungeonId = 6, MonsterId = 20, SpawnCount = 1, MapName = mapName, IsActive = true },
            };

            _ctx.MonsterSpawns.AddRange(spawns);
            await _ctx.SaveChangesAsync();
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Seed Content mẫu (không phải route riêng, gọi từ SeedMysticJourney)
        // ─────────────────────────────────────────────────────────────────────────
        private async Task SeedContent()
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

            var catElfForest = await _ctx.CategoryContents.FirstOrDefaultAsync(c => c.Slug == "elf-forest")
                ?? new CategoryContent { Name = "Elf Forest", Slug = "elf-forest", Description = "The ancient forest where Elves live. It was once protected by the Origin Tree before the curse fell.", IsActive = true, IconUrl = null };
            var catSealBooks = await _ctx.CategoryContents.FirstOrDefaultAsync(c => c.Slug == "seal-books")
                ?? new CategoryContent { Name = "Seal Books", Slug = "seal-books", Description = "A collection of four ancient seal books containing elemental power used to solve mysteries in the game.", IsActive = true, IconUrl = null };
            var catChronicle = await _ctx.CategoryContents.FirstOrDefaultAsync(c => c.Slug == "the-chronicle")
                ?? new CategoryContent { Name = "The Chronicle", Slug = "the-chronicle", Description = "A journal recording the legends, myths, and main story events happening across the lands.", IsActive = true, IconUrl = null };

            var catNews = new CategoryContent { Name = "[SEED] News", Slug = "seed-news", Description = "Game News", IsActive = true };
            var catGuides = new CategoryContent { Name = "[SEED] Guides", Slug = "seed-guides", Description = "Beginner Guides", IsActive = true };

            if (catElfForest.CategoryContentId == 0) _ctx.CategoryContents.Add(catElfForest);
            if (catSealBooks.CategoryContentId == 0) _ctx.CategoryContents.Add(catSealBooks);
            if (catChronicle.CategoryContentId == 0) _ctx.CategoryContents.Add(catChronicle);
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
                    new BlockContent { Title = "Combat Guide", BlockType = "Text", ContentData = "Use basic skills to defeat monsters.", SortOrder = 1 }
                }
            };

            _ctx.Contents.AddRange(content1, content2);
            await _ctx.SaveChangesAsync();
        }

        private async Task<(Skin skinKnight, Skin skinArcher, Skin skinMage, Skin skinArcherPremium, Skin skinKnightPremium, Skin skinMagePremium)> EnsureBaseSkinsAsync()
        {
            var skinKnight = await _ctx.Skins.FirstOrDefaultAsync(s => s.SkinId == 1 || s.Name == "Knight Default");
            if (skinKnight == null)
            {
                skinKnight = new Skin { Name = "Knight Default", Description = "Knight default skin", Type = "FullSet", Rarity = "Common", Currency = "Gems", Price = 0, IsForSale = false, IsActive = true, CreatedAt = DateTime.UtcNow };
                _ctx.Skins.Add(skinKnight);
                await _ctx.SaveChangesAsync();
            }
            else if (skinKnight.Name != "Knight Default")
            {
                skinKnight.Name = "Knight Default";
                skinKnight.Description = "Knight default skin";
                await _ctx.SaveChangesAsync();
            }

            var skinArcher = await _ctx.Skins.FirstOrDefaultAsync(s => s.SkinId == 2 || s.Name == "Archer Default");
            if (skinArcher == null)
            {
                skinArcher = new Skin { Name = "Archer Default", Description = "Archer default skin", Type = "FullSet", Rarity = "Common", Currency = "Gems", Price = 0, IsForSale = false, IsActive = true, CreatedAt = DateTime.UtcNow };
                _ctx.Skins.Add(skinArcher);
                await _ctx.SaveChangesAsync();
            }
            else if (skinArcher.Name != "Archer Default")
            {
                skinArcher.Name = "Archer Default";
                skinArcher.Description = "Archer default skin";
                await _ctx.SaveChangesAsync();
            }

            var skinMage = await _ctx.Skins.FirstOrDefaultAsync(s => s.SkinId == 3 || s.Name == "Mage Default");
            if (skinMage == null)
            {
                skinMage = new Skin { Name = "Mage Default", Description = "Mage default skin", Type = "FullSet", Rarity = "Common", Currency = "Gems", Price = 0, IsForSale = false, IsActive = true, CreatedAt = DateTime.UtcNow };
                _ctx.Skins.Add(skinMage);
                await _ctx.SaveChangesAsync();
            }
            else if (skinMage.Name != "Mage Default")
            {
                skinMage.Name = "Mage Default";
                skinMage.Description = "Mage default skin";
                await _ctx.SaveChangesAsync();
            }

            var skinArcherPremium = await _ctx.Skins.FirstOrDefaultAsync(s => s.SkinId == 4 || s.Name == "Archer Skin");
            if (skinArcherPremium == null)
            {
                skinArcherPremium = new Skin { Name = "Archer Skin", Description = "Archer premium skin", Type = "FullSet", Rarity = "Rare", Currency = "Gems", Price = 100, IsForSale = true, IsActive = true, CreatedAt = DateTime.UtcNow };
                _ctx.Skins.Add(skinArcherPremium);
                await _ctx.SaveChangesAsync();
            }
            else if (skinArcherPremium.Name != "Archer Skin")
            {
                skinArcherPremium.Name = "Archer Skin";
                skinArcherPremium.Description = "Archer premium skin";
                await _ctx.SaveChangesAsync();
            }

            var skinKnightPremium = await _ctx.Skins.FirstOrDefaultAsync(s => s.SkinId == 5 || s.Name == "Knight Skin");
            if (skinKnightPremium == null)
            {
                skinKnightPremium = new Skin { Name = "Knight Skin", Description = "Knight premium skin", Type = "FullSet", Rarity = "Rare", Currency = "Gems", Price = 100, IsForSale = true, IsActive = true, CreatedAt = DateTime.UtcNow };
                _ctx.Skins.Add(skinKnightPremium);
                await _ctx.SaveChangesAsync();
            }
            else if (skinKnightPremium.Name != "Knight Skin")
            {
                skinKnightPremium.Name = "Knight Skin";
                skinKnightPremium.Description = "Knight premium skin";
                await _ctx.SaveChangesAsync();
            }

            var skinMagePremium = await _ctx.Skins.FirstOrDefaultAsync(s => s.SkinId == 6 || s.Name == "Mage Skin");
            if (skinMagePremium == null)
            {
                skinMagePremium = new Skin { Name = "Mage Skin", Description = "Mage premium skin", Type = "FullSet", Rarity = "Rare", Currency = "Gems", Price = 100, IsForSale = true, IsActive = true, CreatedAt = DateTime.UtcNow };
                _ctx.Skins.Add(skinMagePremium);
                await _ctx.SaveChangesAsync();
            }
            else if (skinMagePremium.Name != "Mage Skin")
            {
                skinMagePremium.Name = "Mage Skin";
                skinMagePremium.Description = "Mage premium skin";
                await _ctx.SaveChangesAsync();
            }

            return (skinKnight, skinArcher, skinMage, skinArcherPremium, skinKnightPremium, skinMagePremium);
        }

        private static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
