using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Data
{
    public class MysticJourneyDbContext : DbContext
    {
        public MysticJourneyDbContext(DbContextOptions<MysticJourneyDbContext> options)
            : base(options)
        {
        }
        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<PlayerProfile> PlayerProfiles => Set<PlayerProfile>();
        public DbSet<PlayerStat> PlayerStats => Set<PlayerStat>();
        public DbSet<PlayerStatsSnapshot> PlayerStatsSnapshots => Set<PlayerStatsSnapshot>();
        public DbSet<PlayerBuff> PlayerBuffs => Set<PlayerBuff>();
        public DbSet<Item> Items => Set<Item>();
        public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
        public DbSet<EquipmentStats> EquipmentStats => Set<EquipmentStats>();
        public DbSet<Skill> Skills => Set<Skill>();
        public DbSet<PlayerSkill> PlayerSkills => Set<PlayerSkill>();
        public DbSet<Quest> Quests => Set<Quest>();
        public DbSet<QuestRewardItem> QuestRewardItems => Set<QuestRewardItem>();
        public DbSet<QuestRewardSkill> QuestRewardSkills => Set<QuestRewardSkill>();
        public DbSet<PlayerQuest> PlayerQuests => Set<PlayerQuest>();
        public DbSet<NPC> NPCs => Set<NPC>();
        public DbSet<NPCDialogue> NPCDialogues => Set<NPCDialogue>();
        public DbSet<Monster> Monsters => Set<Monster>();
        public DbSet<MonsterSpawn> MonsterSpawns => Set<MonsterSpawn>();
        public DbSet<PurchaseHistory> PurchaseHistories => Set<PurchaseHistory>();
        public DbSet<PlayerCurrencyLog> PlayerCurrencyLogs => Set<PlayerCurrencyLog>();
        public DbSet<ShopItem> ShopItems => Set<ShopItem>();
        public DbSet<PlayerShopRefreshState> PlayerShopRefreshStates => Set<PlayerShopRefreshState>();
        public DbSet<Mailbox> Mailboxes => Set<Mailbox>();
        public DbSet<MailboxRewardItem> MailboxRewardItems => Set<MailboxRewardItem>();
        public DbSet<Friend> Friends => Set<Friend>();
        public DbSet<FriendBlock> FriendBlocks => Set<FriendBlock>();
        public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
        public DbSet<WorldChatMessage> WorldChatMessages => Set<WorldChatMessage>();
        public DbSet<ChatModerationPenalty> ChatModerationPenalties => Set<ChatModerationPenalty>();
        public DbSet<MonsterDrop> MonsterDrops => Set<MonsterDrop>();
        public DbSet<GachaBanner> GachaBanners => Set<GachaBanner>();
        public DbSet<GachaBannerItem> GachaBannerItems => Set<GachaBannerItem>();
        public DbSet<GachaPullHistory> GachaPullHistories => Set<GachaPullHistory>();
        public DbSet<DungeonConfig> DungeonConfigs => Set<DungeonConfig>();
        public DbSet<DungeonSession> DungeonSessions => Set<DungeonSession>();
        public DbSet<DungeonProgress> DungeonProgresses => Set<DungeonProgress>();
        public DbSet<Dungeon> Dungeons => Set<Dungeon>();
        public DbSet<PlayerMonsterDiscovery> PlayerMonsterDiscoveries => Set<PlayerMonsterDiscovery>();
        public DbSet<Achievement> Achievements => Set<Achievement>();
        public DbSet<PlayerAchievement> PlayerAchievements => Set<PlayerAchievement>();
        public DbSet<Guild> Guilds => Set<Guild>();
        public DbSet<GuildMember> GuildMembers => Set<GuildMember>();
        public DbSet<GuildInvitation> GuildInvitations => Set<GuildInvitation>();
        public DbSet<GuildApplication> GuildApplications => Set<GuildApplication>();
        public DbSet<GuildChatMessage> GuildChatMessages => Set<GuildChatMessage>();
        public DbSet<GuildLog> GuildLogs => Set<GuildLog>();
        public DbSet<Skin> Skins => Set<Skin>();
        public DbSet<PlayerSkin> PlayerSkins => Set<PlayerSkin>();
        public DbSet<Chest> Chests => Set<Chest>();
        public DbSet<ChestItem> ChestItems => Set<ChestItem>();
        public DbSet<PlayerChest> PlayerChests => Set<PlayerChest>();
public DbSet<DailyLoginReward> DailyLoginRewards => Set<DailyLoginReward>();
        public DbSet<PlayerDailyLogin> PlayerDailyLogins => Set<PlayerDailyLogin>();
        public DbSet<GameAnnouncement> GameAnnouncements => Set<GameAnnouncement>();
        public DbSet<PlayerAnnouncement> PlayerAnnouncements => Set<PlayerAnnouncement>();
        public DbSet<CategoryContent> CategoryContents => Set<CategoryContent>();
        public DbSet<SubCategoryContent> SubCategoryContents => Set<SubCategoryContent>();
        public DbSet<Content> Contents => Set<Content>();
        public DbSet<BlockContent> BlockContents => Set<BlockContent>();
        public DbSet<ClassConfig> ClassConfigs => Set<ClassConfig>();   

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
            new Role { RoleId = 1, Name = "Player" },
            new Role { RoleId = 2, Name = "Admin" },
            new Role { RoleId = 3, Name = "SuperAdmin" });

            modelBuilder.Entity<ClassConfig>().HasData(
                new ClassConfig { ClassConfigId = 1, ClassName = "Knight", MaxHp = 620, Atk = 42, Def = 45, MoveSpeed = 100, AttackSpeed = 100, CritRate = 5, CritDamage = 150, DamageBonus = 0 },
                new ClassConfig { ClassConfigId = 2, ClassName = "Archer", MaxHp = 420, Atk = 52, Def = 26, MoveSpeed = 100, AttackSpeed = 100, CritRate = 5, CritDamage = 150, DamageBonus = 0 },
                new ClassConfig { ClassConfigId = 3, ClassName = "Mage",   MaxHp = 360, Atk = 46, Def = 20, MoveSpeed = 100, AttackSpeed = 100, CritRate = 5, CritDamage = 150, DamageBonus = 0 }
            );

            // ─────────────────────────────────────────────────────────────────────────
            // MONSTERS – tuned against the ClassConfig baselines above and the chapter
            // each monster is actually fought in (cross-check Quest.MapName below).
            //
            // Balance contract – keep these invariants when adding or editing a monster:
            //   • A fresh level-1 player deals ~120 damage per basic attack (ClassConfig
            //     + PlayerCombat.GetClassScaledDamage). Trash HP is therefore sized in
            //     whole "hits to kill": ~1 hit in Ch1, ~3 in Ch2, ~5 in Ch3, ~6 in Ch4.
            //   • Atk is near-literal incoming damage: PlayerEntity.TakeDamage only
            //     subtracts Def/5 and floors at 50% of the hit. Same-chapter trash should
            //     need 8+ hits to kill the player, a boss 5+.
            //   • CritDamage is a PERCENT multiplier (150 = 1.5x). NEVER set it below 100
            //     or a "crit" would hit softer than a normal swing (UnderKing shipped at
            //     20, which silently made its crits a 5x damage REDUCTION).
            //   • MoveSpeed is on the same 100 = normal scale as ClassConfig, because
            //     EnemyBehaviour.UpdateStatsFromAPI does (MoveSpeed / 100) * 3.5. The old
            //     values of 1-6 resolved to 0.03-0.21 Unity speed, i.e. every monster in
            //     the game was effectively unable to chase. 0 means stationary BY DESIGN
            //     (DragonBossIdle only).
            //   • ExperienceReward stays deliberately small: PlayerProfile levels every
            //     100 exp, so a 12-kill quest must not grant several levels at once.
            // ─────────────────────────────────────────────────────────────────────────
            var monsterSeededAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            modelBuilder.Entity<Monster>().HasData(
                new Monster { MonsterId = 1, Name = "SlimeLittle", Type = "Normal", Description = "A basic slime monster. The first thing a new player ever fights.", Level = 1, MaxHp = 300, Atk = 30, Def = 2, MoveSpeed = 70, AttackSpeed = 85, CritRate = 5, CritDamage = 130, ExperienceReward = 4, GoldReward = 8m, IsActive = true, CreatedAt = monsterSeededAt },
                new Monster { MonsterId = 2, Name = "SwampDemon", Type = "Boss", Description = "A dangerous swamp demon brooding over an old relic in the deep woods.", Level = 3, MaxHp = 1380, Atk = 32, Def = 10, MoveSpeed = 90, AttackSpeed = 100, CritRate = 12, CritDamage = 150, ExperienceReward = 22, GoldReward = 110m, GemReward = 5m, IsActive = true, CreatedAt = monsterSeededAt },
                new Monster { MonsterId = 3, Name = "WaterElemental", Type = "Normal", Description = "A water elemental monster from the forest marshes.", Level = 3, MaxHp = 400, Atk = 39, Def = 5, MoveSpeed = 80, AttackSpeed = 95, CritRate = 8, CritDamage = 140, ExperienceReward = 4, GoldReward = 8m, IsActive = true, CreatedAt = monsterSeededAt },
                new Monster { MonsterId = 4, Name = "Dragon", Type = "Normal", Description = "A fierce dragon nesting in the ruined city.", Level = 6, MaxHp = 560, Atk = 47, Def = 12, MoveSpeed = 110, AttackSpeed = 100, CritRate = 15, CritDamage = 160, ExperienceReward = 6, GoldReward = 13m, IsActive = true, CreatedAt = monsterSeededAt },
                new Monster { MonsterId = 5, Name = "BlueDragonFrost", Type = "Normal", Description = "A frosty blue dragon.", Level = 7, MaxHp = 580, Atk = 48, Def = 14, MoveSpeed = 110, AttackSpeed = 100, CritRate = 15, CritDamage = 160, ExperienceReward = 6, GoldReward = 13m, IsActive = true, CreatedAt = monsterSeededAt },
                new Monster { MonsterId = 6, Name = "GreenDragonForest", Type = "Normal", Description = "A forest green dragon.", Level = 7, MaxHp = 590, Atk = 49, Def = 15, MoveSpeed = 110, AttackSpeed = 105, CritRate = 15, CritDamage = 160, ExperienceReward = 6, GoldReward = 13m, IsActive = true, CreatedAt = monsterSeededAt },
                new Monster { MonsterId = 7, Name = "DragonBossIdle", Type = "Boss", Description = "The dragon that broke the city. It never leaves its nest, so MoveSpeed is 0 by design.", Level = 7, MaxHp = 2930, Atk = 53, Def = 22, MoveSpeed = 0, AttackSpeed = 100, CritRate = 20, CritDamage = 175, ExperienceReward = 35, GoldReward = 176m, GemReward = 10m, IsActive = true, CreatedAt = monsterSeededAt },
                new Monster { MonsterId = 8, Name = "SlimeIce", Type = "Normal", Description = "An icy slime that creeps onto the snow fields at night.", Level = 7, MaxHp = 620, Atk = 50, Def = 15, MoveSpeed = 75, AttackSpeed = 90, CritRate = 10, CritDamage = 150, ExperienceReward = 10, GoldReward = 19m, IsActive = true, CreatedAt = monsterSeededAt },
                new Monster { MonsterId = 9, Name = "IceDragon", Type = "Normal", Description = "An icy dragon driven down the mountain against the people below.", Level = 9, MaxHp = 840, Atk = 55, Def = 18, MoveSpeed = 115, AttackSpeed = 105, CritRate = 20, CritDamage = 165, ExperienceReward = 10, GoldReward = 19m, IsActive = true, CreatedAt = monsterSeededAt },
                new Monster { MonsterId = 10, Name = "GolemBoss", Type = "Boss", Description = "A giant stone golem sealed inside the Doomed Land of Snow.", Level = 9, MaxHp = 4300, Atk = 65, Def = 28, MoveSpeed = 80, AttackSpeed = 90, CritRate = 20, CritDamage = 170, ExperienceReward = 53, GoldReward = 264m, GemReward = 15m, IsActive = true, CreatedAt = monsterSeededAt },
                new Monster { MonsterId = 11, Name = "OrcSkeleton", Type = "Normal", Description = "An undead orc skeleton risen in the valley of Tide-Knell.", Level = 9, MaxHp = 850, Atk = 61, Def = 20, MoveSpeed = 95, AttackSpeed = 100, CritRate = 15, CritDamage = 160, ExperienceReward = 13, GoldReward = 26m, IsActive = true, CreatedAt = monsterSeededAt },
                new Monster { MonsterId = 12, Name = "SkeletonMelee", Type = "Normal", Description = "A melee skeleton warrior.", Level = 11, MaxHp = 1050, Atk = 71, Def = 22, MoveSpeed = 100, AttackSpeed = 105, CritRate = 15, CritDamage = 160, ExperienceReward = 13, GoldReward = 26m, IsActive = true, CreatedAt = monsterSeededAt },
                new Monster { MonsterId = 13, Name = "SkeletonArcher", Type = "Normal", Description = "A ranged skeleton archer. Glass cannon: highest Atk of the skeletons, lowest Def.", Level = 12, MaxHp = 1160, Atk = 78, Def = 16, MoveSpeed = 100, AttackSpeed = 115, CritRate = 22, CritDamage = 165, ExperienceReward = 13, GoldReward = 26m, IsActive = true, CreatedAt = monsterSeededAt },
                new Monster { MonsterId = 14, Name = "Ghost", Type = "Normal", Description = "A floating ghost haunting the ruined quarter.", Level = 4, MaxHp = 480, Atk = 42, Def = 10, MoveSpeed = 95, AttackSpeed = 100, CritRate = 15, CritDamage = 160, ExperienceReward = 6, GoldReward = 13m, IsActive = true, CreatedAt = monsterSeededAt },
                new Monster { MonsterId = 15, Name = "UnderKing", Type = "Boss", Description = "Once a great human king who accepted two Seal Books and imprisoned himself beneath the deserted island to spare the world their curse. Centuries of darkness eroded the hero into the UnderKing.", Level = 12, MaxHp = 6040, Atk = 94, Def = 35, MoveSpeed = 95, AttackSpeed = 100, CritRate = 25, CritDamage = 180, ExperienceReward = 70, GoldReward = 352m, GemReward = 30m, IsActive = true, CreatedAt = monsterSeededAt },
                new Monster { MonsterId = 16, Name = "Demon", Type = "Normal", Description = "A terrifying demon.", Level = 8, MaxHp = 730, Atk = 51, Def = 18, MoveSpeed = 95, AttackSpeed = 100, CritRate = 20, CritDamage = 165, ExperienceReward = 10, GoldReward = 19m, IsActive = true, CreatedAt = monsterSeededAt },
                new Monster { MonsterId = 17, Name = "GoblinWarrior", Type = "Normal", Description = "A strong goblin warrior.", Level = 5, MaxHp = 530, Atk = 45, Def = 13, MoveSpeed = 95, AttackSpeed = 100, CritRate = 12, CritDamage = 150, ExperienceReward = 6, GoldReward = 13m, IsActive = true, CreatedAt = monsterSeededAt },
                new Monster { MonsterId = 18, Name = "GoblinSpear", Type = "Normal", Description = "A goblin spearman.", Level = 5, MaxHp = 510, Atk = 44, Def = 10, MoveSpeed = 100, AttackSpeed = 100, CritRate = 10, CritDamage = 150, ExperienceReward = 6, GoldReward = 13m, IsActive = true, CreatedAt = monsterSeededAt },
                new Monster { MonsterId = 19, Name = "Ogre", Type = "Boss", Description = "The brutal ogre holding the Goblin barracks. Dungeon 5 boss.", Level = 7, MaxHp = 2560, Atk = 46, Def = 19, MoveSpeed = 85, AttackSpeed = 90, CritRate = 15, CritDamage = 165, ExperienceReward = 35, GoldReward = 176m, GemReward = 10m, IsActive = true, CreatedAt = monsterSeededAt },
                new Monster { MonsterId = 20, Name = "OrcWarlord", Type = "Boss", Description = "A formidable orc warlord guarding the gate to the underworld. Dungeon 6 boss.", Level = 12, MaxHp = 4490, Atk = 73, Def = 30, MoveSpeed = 95, AttackSpeed = 100, CritRate = 22, CritDamage = 175, ExperienceReward = 70, GoldReward = 352m, GemReward = 30m, IsActive = true, CreatedAt = monsterSeededAt },
                new Monster { MonsterId = 21, Name = "IceFairy", Type = "Boss", Description = "The spirit that never leaves the golem's side. Fought together with GolemBoss.", Level = 9, MaxHp = 3230, Atk = 54, Def = 16, MoveSpeed = 100, AttackSpeed = 100, CritRate = 12, CritDamage = 150, ExperienceReward = 53, GoldReward = 264m, GemReward = 15m, IsActive = true, CreatedAt = monsterSeededAt },
                new Monster { MonsterId = 22, Name = "GoblinWarlord", Type = "Boss", Description = "A fierce goblin warlord holding the Goblin Grounds.", Level = 7, MaxHp = 2180, Atk = 41, Def = 18, MoveSpeed = 95, AttackSpeed = 100, CritRate = 18, CritDamage = 165, ExperienceReward = 35, GoldReward = 176m, GemReward = 10m, IsActive = true, CreatedAt = monsterSeededAt },
                new Monster { MonsterId = 23, Name = "NecromancerCast", Type = "Normal", Description = "A dark necromancer casting dark spells.", Level = 4, MaxHp = 500, Atk = 43, Def = 7, MoveSpeed = 85, AttackSpeed = 90, CritRate = 10, CritDamage = 155, ExperienceReward = 6, GoldReward = 13m, IsActive = true, CreatedAt = monsterSeededAt },
                new Monster { MonsterId = 24, Name = "RobberArcher", Type = "Normal", Description = "A rogue robber archer wielding a crossbow.", Level = 3, MaxHp = 440, Atk = 40, Def = 6, MoveSpeed = 100, AttackSpeed = 110, CritRate = 12, CritDamage = 150, ExperienceReward = 6, GoldReward = 13m, IsActive = true, CreatedAt = monsterSeededAt },
                new Monster { MonsterId = 25, Name = "RobberAssassin", Type = "Normal", Description = "A stealthy robber assassin wielding a sword and shield.", Level = 3, MaxHp = 460, Atk = 41, Def = 9, MoveSpeed = 105, AttackSpeed = 115, CritRate = 18, CritDamage = 160, ExperienceReward = 6, GoldReward = 13m, IsActive = true, CreatedAt = monsterSeededAt },
                new Monster { MonsterId = 26, Name = "RedGuard", Type = "Normal", Description = "A heavy red guard soldier carrying a mace and shield.", Level = 6, MaxHp = 540, Atk = 46, Def = 15, MoveSpeed = 85, AttackSpeed = 95, CritRate = 10, CritDamage = 150, ExperienceReward = 6, GoldReward = 13m, IsActive = true, CreatedAt = monsterSeededAt },
                new Monster { MonsterId = 27, Name = "OrcSkeletonAfk", Type = "Normal", Description = "An orc skeleton standing watch in the valley of Tide-Knell. Slower and tougher than its roaming kin.", Level = 10, MaxHp = 950, Atk = 65, Def = 24, MoveSpeed = 90, AttackSpeed = 95, CritRate = 15, CritDamage = 160, ExperienceReward = 13, GoldReward = 26m, IsActive = true, CreatedAt = monsterSeededAt }
            );

            modelBuilder.Entity<Item>().HasData(
                new Item
                {
                    ItemId = 901,
                    Name = "Swamp Sword",
                    Description = "A sharp sword dropped by the SwampDemon.",
                    Type = "Weapon",
                    Rarity = "Rare",
                    Slot = "Weapon",
                    BaseValue = 100m,
                    MaxStack = 1,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Item
                {
                    ItemId = 902,
                    Name = "Swamp Armor",
                    Description = "A sturdy armor dropped by the SwampDemon.",
                    Type = "Armor",
                    Rarity = "Rare",
                    Slot = "Armor",
                    BaseValue = 150m,
                    MaxStack = 1,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Item
                {
                    ItemId = 903,
                    Name = "Dragon Boss Sword",
                    Description = "A legendary sword dropped by DragonBossIdle.",
                    Type = "Weapon",
                    Rarity = "Legendary",
                    Slot = "Weapon",
                    BaseValue = 500m,
                    MaxStack = 1,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Item
                {
                    ItemId = 904,
                    Name = "Dragon Boss Armor",
                    Description = "A legendary armor dropped by DragonBossIdle.",
                    Type = "Armor",
                    Rarity = "Legendary",
                    Slot = "Armor",
                    BaseValue = 600m,
                    MaxStack = 1,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Item
                {
                    ItemId = 905,
                    Name = "Golem Boss Gloves",
                    Description = "Heavy stone gloves dropped by GolemBoss.",
                    Type = "Armor",
                    Rarity = "Legendary",
                    Slot = "Gloves",
                    BaseValue = 800m,
                    MaxStack = 1,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Item
                {
                    ItemId = 906,
                    Name = "Golem Boss Armor",
                    Description = "A massive stone armor dropped by GolemBoss.",
                    Type = "Armor",
                    Rarity = "Legendary",
                    Slot = "Armor",
                    BaseValue = 1000m,
                    MaxStack = 1,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Item
                {
                    ItemId = 907,
                    Name = "UnderKing Sword",
                    Description = "A royal cursed sword dropped by UnderKing.",
                    Type = "Weapon",
                    Rarity = "Legendary",
                    Slot = "Weapon",
                    BaseValue = 1500m,
                    MaxStack = 1,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Item
                {
                    ItemId = 908,
                    Name = "UnderKing Crown",
                    Description = "The crown of the UnderKing.",
                    Type = "Armor",
                    Rarity = "Legendary",
                    Slot = "Helmet",
                    BaseValue = 2000m,
                    MaxStack = 1,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Item
                {
                    ItemId = 909,
                    Name = "Swamp Seal Book",
                    Description = "A magic book containing the power to seal the Origin Tree, guarded by SwampDemon.",
                    Type = "QuestItem",
                    Rarity = "Legendary",
                    Slot = "None",
                    BaseValue = 0m,
                    MaxStack = 1,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Item
                {
                    ItemId = 910,
                    Name = "Dragon Seal Book",
                    Description = "A magic book containing the power to seal the Origin Tree, guarded by DragonBossIdle.",
                    Type = "QuestItem",
                    Rarity = "Legendary",
                    Slot = "None",
                    BaseValue = 0m,
                    MaxStack = 1,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Item
                {
                    ItemId = 911,
                    Name = "Golem Seal Book",
                    Description = "A magic book containing the power to seal the Origin Tree, guarded by GolemBoss.",
                    Type = "QuestItem",
                    Rarity = "Legendary",
                    Slot = "None",
                    BaseValue = 0m,
                    MaxStack = 1,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Item
                {
                    ItemId = 912,
                    Name = "UnderKing Seal Book",
                    Description = "The final magic book to seal the Origin Tree, guarded by UnderKing.",
                    Type = "QuestItem",
                    Rarity = "Legendary",
                    Slot = "None",
                    BaseValue = 0m,
                    MaxStack = 1,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );


            // ─────────────────────────────────────────────────────────────────────────
            // EQUIPMENT STATS – one canonical block for every equippable item.
            //
            // Gear is the ONLY thing that makes the player stronger as chapters advance:
            // ClassConfig is flat, and a level-up grants a single stat point (+20 HP or
            // +3 Atk), so without a gear curve the player at Chapter 4 fights 800 HP
            // skeletons with Chapter 1 numbers.
            //
            // Tiers below map 1:1 onto the quest chain (see Quest seed further down):
            //   T1  Ch1  lvl 1-2   granted at character creation + Q6/Q8
            //   T2  Ch2  lvl 3-5   SwampDemon drops + Q14/Q19/Q20
            //   T3  Ch3  lvl 6-8   DragonBossIdle drops + Q26/Q27
            //   T4  Ch4  lvl 9-12  GolemBoss drops + Q38
            //   T5  end            UnderKing drops + Q45
            //
            // Invariants:
            //   • BonusCritRate is a FLAT PERCENT (InventoryService adds it straight into
            //     the snapshot, PlayerCombat compares Random(0,100) <= critRate). Total
            //     stackable crit across best-in-slot must stay well under 100, otherwise
            //     every hit crits and CritDamage stops being a trade-off. Shadow Hood
            //     shipped at 80 and Elven Blade at 60 -> permanent guaranteed crit.
            //   • Def feeds a (Def / 5) flat reduction with a 50% floor on both sides, so
            //     Def scales much harder than it looks. Keep armour Def under ~50/piece.
            //   • Each stat is split Base (70%) / Bonus (30%) so enhancement levels
            //     (HP +10, Atk +2, Def +1 per level) stay a visible fraction of the item.
            // ─────────────────────────────────────────────────────────────────────────
            modelBuilder.Entity<EquipmentStats>().HasData(
                new EquipmentStats { EquipmentStatsId = 5, ItemId = 5, BaseHp = 0, BaseAtk = 7, BaseDef = 0, BonusHp = 0, BonusAtk = 3, BonusDef = 0, BonusMoveSpeed = 0, BonusAttackSpeed = 0, BonusCritRate = 3, BonusCritDamage = 15, BonusDamageBonus = 0 },
                new EquipmentStats { EquipmentStatsId = 6, ItemId = 6, BaseHp = 0, BaseAtk = 6, BaseDef = 0, BonusHp = 0, BonusAtk = 2, BonusDef = 0, BonusMoveSpeed = 0, BonusAttackSpeed = 6, BonusCritRate = 6, BonusCritDamage = 10, BonusDamageBonus = 0 },
                new EquipmentStats { EquipmentStatsId = 7, ItemId = 7, BaseHp = 0, BaseAtk = 6, BaseDef = 0, BonusHp = 0, BonusAtk = 3, BonusDef = 0, BonusMoveSpeed = 0, BonusAttackSpeed = 0, BonusCritRate = 2, BonusCritDamage = 20, BonusDamageBonus = 2 },
                new EquipmentStats { EquipmentStatsId = 8, ItemId = 8, BaseHp = 0, BaseAtk = 29, BaseDef = 0, BonusHp = 0, BonusAtk = 13, BonusDef = 0, BonusMoveSpeed = 0, BonusAttackSpeed = 4, BonusCritRate = 10, BonusCritDamage = 30, BonusDamageBonus = 4 },
                new EquipmentStats { EquipmentStatsId = 9, ItemId = 9, BaseHp = 31, BaseAtk = 0, BaseDef = 6, BonusHp = 14, BonusAtk = 0, BonusDef = 2, BonusMoveSpeed = 0, BonusAttackSpeed = 0, BonusCritRate = 0, BonusCritDamage = 0, BonusDamageBonus = 0 },
                new EquipmentStats { EquipmentStatsId = 10, ItemId = 10, BaseHp = 21, BaseAtk = 0, BaseDef = 4, BonusHp = 9, BonusAtk = 0, BonusDef = 2, BonusMoveSpeed = 0, BonusAttackSpeed = 0, BonusCritRate = 0, BonusCritDamage = 0, BonusDamageBonus = 0 },
                new EquipmentStats { EquipmentStatsId = 11, ItemId = 11, BaseHp = 0, BaseAtk = 0, BaseDef = 4, BonusHp = 0, BonusAtk = 0, BonusDef = 1, BonusMoveSpeed = 8, BonusAttackSpeed = 0, BonusCritRate = 0, BonusCritDamage = 0, BonusDamageBonus = 0 },
                new EquipmentStats { EquipmentStatsId = 12, ItemId = 12, BaseHp = 196, BaseAtk = 0, BaseDef = 32, BonusHp = 84, BonusAtk = 0, BonusDef = 14, BonusMoveSpeed = 0, BonusAttackSpeed = 0, BonusCritRate = 0, BonusCritDamage = 0, BonusDamageBonus = 0 },
                new EquipmentStats { EquipmentStatsId = 13, ItemId = 13, BaseHp = 84, BaseAtk = 0, BaseDef = 14, BonusHp = 36, BonusAtk = 0, BonusDef = 6, BonusMoveSpeed = 6, BonusAttackSpeed = 0, BonusCritRate = 0, BonusCritDamage = 0, BonusDamageBonus = 0 },
                new EquipmentStats { EquipmentStatsId = 14, ItemId = 14, BaseHp = 0, BaseAtk = 0, BaseDef = 7, BonusHp = 0, BonusAtk = 0, BonusDef = 3, BonusMoveSpeed = 0, BonusAttackSpeed = 0, BonusCritRate = 8, BonusCritDamage = 25, BonusDamageBonus = 0 },
                new EquipmentStats { EquipmentStatsId = 15, ItemId = 15, BaseHp = 0, BaseAtk = 4, BaseDef = 3, BonusHp = 0, BonusAtk = 2, BonusDef = 1, BonusMoveSpeed = 0, BonusAttackSpeed = 0, BonusCritRate = 0, BonusCritDamage = 0, BonusDamageBonus = 2 },
                new EquipmentStats { EquipmentStatsId = 16, ItemId = 16, BaseHp = 0, BaseAtk = 3, BaseDef = 2, BonusHp = 0, BonusAtk = 1, BonusDef = 1, BonusMoveSpeed = 3, BonusAttackSpeed = 3, BonusCritRate = 0, BonusCritDamage = 0, BonusDamageBonus = 0 },
                new EquipmentStats { EquipmentStatsId = 17, ItemId = 17, BaseHp = 18, BaseAtk = 2, BaseDef = 0, BonusHp = 7, BonusAtk = 1, BonusDef = 0, BonusMoveSpeed = 0, BonusAttackSpeed = 0, BonusCritRate = 3, BonusCritDamage = 6, BonusDamageBonus = 0 },
                new EquipmentStats { EquipmentStatsId = 18, ItemId = 18, BaseHp = 35, BaseAtk = 0, BaseDef = 4, BonusHp = 15, BonusAtk = 0, BonusDef = 1, BonusMoveSpeed = 0, BonusAttackSpeed = 0, BonusCritRate = 0, BonusCritDamage = 0, BonusDamageBonus = 0 },
                new EquipmentStats { EquipmentStatsId = 901, ItemId = 901, BaseHp = 0, BaseAtk = 13, BaseDef = 0, BonusHp = 0, BonusAtk = 5, BonusDef = 0, BonusMoveSpeed = 0, BonusAttackSpeed = 0, BonusCritRate = 5, BonusCritDamage = 18, BonusDamageBonus = 0 },
                new EquipmentStats { EquipmentStatsId = 902, ItemId = 902, BaseHp = 56, BaseAtk = 0, BaseDef = 11, BonusHp = 24, BonusAtk = 0, BonusDef = 5, BonusMoveSpeed = 0, BonusAttackSpeed = 0, BonusCritRate = 0, BonusCritDamage = 0, BonusDamageBonus = 0 },
                new EquipmentStats { EquipmentStatsId = 903, ItemId = 903, BaseHp = 0, BaseAtk = 22, BaseDef = 0, BonusHp = 0, BonusAtk = 10, BonusDef = 0, BonusMoveSpeed = 0, BonusAttackSpeed = 0, BonusCritRate = 8, BonusCritDamage = 25, BonusDamageBonus = 0 },
                new EquipmentStats { EquipmentStatsId = 904, ItemId = 904, BaseHp = 112, BaseAtk = 0, BaseDef = 21, BonusHp = 48, BonusAtk = 0, BonusDef = 9, BonusMoveSpeed = 0, BonusAttackSpeed = 0, BonusCritRate = 0, BonusCritDamage = 0, BonusDamageBonus = 0 },
                new EquipmentStats { EquipmentStatsId = 905, ItemId = 905, BaseHp = 49, BaseAtk = 8, BaseDef = 11, BonusHp = 21, BonusAtk = 4, BonusDef = 5, BonusMoveSpeed = 0, BonusAttackSpeed = 0, BonusCritRate = 4, BonusCritDamage = 10, BonusDamageBonus = 0 },
                new EquipmentStats { EquipmentStatsId = 906, ItemId = 906, BaseHp = 147, BaseAtk = 0, BaseDef = 28, BonusHp = 63, BonusAtk = 0, BonusDef = 12, BonusMoveSpeed = 0, BonusAttackSpeed = 0, BonusCritRate = 0, BonusCritDamage = 0, BonusDamageBonus = 0 },
                new EquipmentStats { EquipmentStatsId = 907, ItemId = 907, BaseHp = 0, BaseAtk = 38, BaseDef = 0, BonusHp = 0, BonusAtk = 17, BonusDef = 0, BonusMoveSpeed = 0, BonusAttackSpeed = 0, BonusCritRate = 12, BonusCritDamage = 35, BonusDamageBonus = 0 },
                new EquipmentStats { EquipmentStatsId = 908, ItemId = 908, BaseHp = 98, BaseAtk = 6, BaseDef = 24, BonusHp = 42, BonusAtk = 2, BonusDef = 10, BonusMoveSpeed = 0, BonusAttackSpeed = 0, BonusCritRate = 5, BonusCritDamage = 15, BonusDamageBonus = 0 }
            );

            modelBuilder.Entity<MonsterDrop>().HasData(
                new MonsterDrop
                {
                    MonsterDropId = 901,
                    MonsterId = 2, // Gắn với SwampDemon
                    ItemId = 901,  // Rớt ra Swamp Sword
                    DropRate = 100, // Tỉ lệ 100%
                    MinQuantity = 1,
                    MaxQuantity = 1,
                    IsGuaranteed = true,
                    IsActive = true
                },
                new MonsterDrop
                {
                    MonsterDropId = 902,
                    MonsterId = 2, // Gắn với SwampDemon
                    ItemId = 902,  // Rớt ra Swamp Armor
                    DropRate = 100, // Tỉ lệ 100%
                    MinQuantity = 1,
                    MaxQuantity = 1,
                    IsGuaranteed = true,
                    IsActive = true
                },
                new MonsterDrop
                {
                    MonsterDropId = 903,
                    MonsterId = 7, // Gắn với DragonBossIdle
                    ItemId = 903,  // Rớt ra Dragon Boss Sword
                    DropRate = 100, // Tỉ lệ 100%
                    MinQuantity = 1,
                    MaxQuantity = 1,
                    IsGuaranteed = true,
                    IsActive = true
                },
                new MonsterDrop
                {
                    MonsterDropId = 904,
                    MonsterId = 7, // Gắn với DragonBossIdle
                    ItemId = 904,  // Rớt ra Dragon Boss Armor
                    DropRate = 100, // Tỉ lệ 100%
                    MinQuantity = 1,
                    MaxQuantity = 1,
                    IsGuaranteed = true,
                    IsActive = true
                },
                new MonsterDrop
                {
                    MonsterDropId = 905,
                    MonsterId = 10, // Gắn với GolemBoss
                    ItemId = 905,  // Rớt ra Golem Boss Gloves
                    DropRate = 100, // Tỉ lệ 100%
                    MinQuantity = 1,
                    MaxQuantity = 1,
                    IsGuaranteed = true,
                    IsActive = true
                },
                new MonsterDrop
                {
                    MonsterDropId = 906,
                    MonsterId = 10, // Gắn với GolemBoss
                    ItemId = 906,  // Rớt ra Golem Boss Armor
                    DropRate = 100, // Tỉ lệ 100%
                    MinQuantity = 1,
                    MaxQuantity = 1,
                    IsGuaranteed = true,
                    IsActive = true
                },
                new MonsterDrop
                {
                    MonsterDropId = 907,
                    MonsterId = 15, // Gắn với UnderKing
                    ItemId = 907,  // Rớt ra UnderKing Sword
                    DropRate = 100, // Tỉ lệ 100%
                    MinQuantity = 1,
                    MaxQuantity = 1,
                    IsGuaranteed = true,
                    IsActive = true
                },
                new MonsterDrop
                {
                    MonsterDropId = 908,
                    MonsterId = 15, // Gắn với UnderKing
                    ItemId = 908,  // Rớt ra UnderKing Crown
                    DropRate = 100, // Tỉ lệ 100%
                    MinQuantity = 1,
                    MaxQuantity = 1,
                    IsGuaranteed = true,
                    IsActive = true
                },
                new MonsterDrop { MonsterDropId = 909, MonsterId = 2, ItemId = 909, DropRate = 100, MinQuantity = 1, MaxQuantity = 1, IsGuaranteed = true, IsActive = true },
                new MonsterDrop { MonsterDropId = 910, MonsterId = 7, ItemId = 910, DropRate = 100, MinQuantity = 1, MaxQuantity = 1, IsGuaranteed = true, IsActive = true },
                new MonsterDrop { MonsterDropId = 911, MonsterId = 10, ItemId = 911, DropRate = 100, MinQuantity = 1, MaxQuantity = 1, IsGuaranteed = true, IsActive = true },
                new MonsterDrop { MonsterDropId = 912, MonsterId = 15, ItemId = 912, DropRate = 100, MinQuantity = 1, MaxQuantity = 1, IsGuaranteed = true, IsActive = true },

                // Seed Skill Upgrade Stone (ItemId = 22) drop for all monsters & bosses directly in DbContext
                new MonsterDrop { MonsterDropId = 951, MonsterId = 1, ItemId = 22, DropRate = 100, MinQuantity = 1, MaxQuantity = 5, IsGuaranteed = true, IsActive = true },
                new MonsterDrop { MonsterDropId = 952, MonsterId = 2, ItemId = 22, DropRate = 100, MinQuantity = 1, MaxQuantity = 5, IsGuaranteed = true, IsActive = true },
                new MonsterDrop { MonsterDropId = 953, MonsterId = 3, ItemId = 22, DropRate = 100, MinQuantity = 1, MaxQuantity = 5, IsGuaranteed = true, IsActive = true },
                new MonsterDrop { MonsterDropId = 954, MonsterId = 4, ItemId = 22, DropRate = 100, MinQuantity = 1, MaxQuantity = 5, IsGuaranteed = true, IsActive = true },
                new MonsterDrop { MonsterDropId = 955, MonsterId = 5, ItemId = 22, DropRate = 100, MinQuantity = 1, MaxQuantity = 5, IsGuaranteed = true, IsActive = true },
                new MonsterDrop { MonsterDropId = 956, MonsterId = 6, ItemId = 22, DropRate = 100, MinQuantity = 1, MaxQuantity = 5, IsGuaranteed = true, IsActive = true },
                new MonsterDrop { MonsterDropId = 957, MonsterId = 7, ItemId = 22, DropRate = 100, MinQuantity = 1, MaxQuantity = 5, IsGuaranteed = true, IsActive = true },
                new MonsterDrop { MonsterDropId = 958, MonsterId = 8, ItemId = 22, DropRate = 100, MinQuantity = 1, MaxQuantity = 5, IsGuaranteed = true, IsActive = true },
                new MonsterDrop { MonsterDropId = 959, MonsterId = 9, ItemId = 22, DropRate = 100, MinQuantity = 1, MaxQuantity = 5, IsGuaranteed = true, IsActive = true },
                new MonsterDrop { MonsterDropId = 960, MonsterId = 10, ItemId = 22, DropRate = 100, MinQuantity = 1, MaxQuantity = 5, IsGuaranteed = true, IsActive = true },
                new MonsterDrop { MonsterDropId = 961, MonsterId = 11, ItemId = 22, DropRate = 100, MinQuantity = 1, MaxQuantity = 5, IsGuaranteed = true, IsActive = true },
                new MonsterDrop { MonsterDropId = 962, MonsterId = 12, ItemId = 22, DropRate = 100, MinQuantity = 1, MaxQuantity = 5, IsGuaranteed = true, IsActive = true },
                new MonsterDrop { MonsterDropId = 963, MonsterId = 13, ItemId = 22, DropRate = 100, MinQuantity = 1, MaxQuantity = 5, IsGuaranteed = true, IsActive = true },
                new MonsterDrop { MonsterDropId = 964, MonsterId = 14, ItemId = 22, DropRate = 100, MinQuantity = 1, MaxQuantity = 5, IsGuaranteed = true, IsActive = true },
                new MonsterDrop { MonsterDropId = 965, MonsterId = 15, ItemId = 22, DropRate = 100, MinQuantity = 1, MaxQuantity = 5, IsGuaranteed = true, IsActive = true }
            );


            modelBuilder.Entity<ShopItem>()
                .Property(s => s.ShopSection)
                .HasMaxLength(30)
                .HasDefaultValue(ShopSections.Fixed);

            modelBuilder.Entity<ShopItem>()
                .HasIndex(s => s.ShopSection);
            modelBuilder.Entity<PlayerShopRefreshState>()
                .HasIndex(s => new { s.PlayerProfileId, s.ShopDateUtc })
                .IsUnique();

            modelBuilder.Entity<PlayerShopRefreshState>()
                .HasOne(s => s.PlayerProfile)
                .WithMany()
                .HasForeignKey(s => s.PlayerProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlayerMonsterDiscovery>()
                .HasIndex(d => new { d.PlayerProfileId, d.MonsterId })
                .IsUnique();

            modelBuilder.Entity<PlayerMonsterDiscovery>()
                .HasOne(d => d.PlayerProfile)
                .WithMany()
                .HasForeignKey(d => d.PlayerProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlayerMonsterDiscovery>()
                .HasOne(d => d.Monster)
                .WithMany()
                .HasForeignKey(d => d.MonsterId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MonsterSpawn>()
                .HasOne(s => s.Monster)
                .WithMany()
                .HasForeignKey(s => s.MonsterId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MonsterSpawn>()
                .HasOne(s => s.Dungeon)
                .WithMany(d => d.Spawns)
                .HasForeignKey(s => s.DungeonId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Quest>()
                .HasOne(q => q.BossMonster)
                .WithMany()
                .HasForeignKey(q => q.BossMonsterId)
                .OnDelete(DeleteBehavior.SetNull);


            // ─────────────────────────────────────────────────────────────────────────
            // QUEST REWARD GEAR – the gear curve that makes player power track story
            // progress. Before this, 13 of 14 equipment items were unreachable: there is
            // no starting equipment (CreateCharacter grants none) and no shop seed, so the
            // only gear in the game came from 4 boss drops.
            //
            // One piece per milestone (chapter boss / chapter end), so the power gained is
            // paced by the quest chain instead of arriving all at once.
            //
            // ClaimRewardCore prefers this collection over Quest.RewardItemId, so quests
            // 14/22/30/33 are deliberately NOT listed here — their RewardItemId carries a
            // story-critical item (Silver Necklace, Magic Flour, Spirit Skull, Mystic Key)
            // that adding a row here would silently replace.
            // ─────────────────────────────────────────────────────────────────────────
            modelBuilder.Entity<QuestRewardItem>().HasData(
                new QuestRewardItem { QuestRewardItemId = 1, QuestId = 6, ItemId = 10, Quantity = 1 },
                new QuestRewardItem { QuestRewardItemId = 2, QuestId = 8, ItemId = 15, Quantity = 1 },
                new QuestRewardItem { QuestRewardItemId = 3, QuestId = 18, ItemId = 17, Quantity = 1 },
                new QuestRewardItem { QuestRewardItemId = 4, QuestId = 19, ItemId = 11, Quantity = 1 },
                new QuestRewardItem { QuestRewardItemId = 5, QuestId = 26, ItemId = 14, Quantity = 1 },
                new QuestRewardItem { QuestRewardItemId = 6, QuestId = 27, ItemId = 13, Quantity = 1 },
                new QuestRewardItem { QuestRewardItemId = 7, QuestId = 39, ItemId = 8, Quantity = 1 },
                new QuestRewardItem { QuestRewardItemId = 8, QuestId = 45, ItemId = 12, Quantity = 1 }
            );

            // ─────────────────────────────────────────────────────────────────────────
            // SHOP – the gold sink. Quest rewards pay out ~2.3k gold across the campaign
            // and nothing was buyable, so gold was a dead currency.
            //
            // Prices are set against that curve: the class starter weapons and the basic
            // armor set are affordable inside chapter 1-2, mid gear lands around chapter 3,
            // and Dragon Scale Armor stays a late-campaign purchase. Every row is
            // ShopSections.Fixed — PlayerShopRepository only reads Fixed for the permanent
            // catalogue (DailyDeal rows are rolled per player, not seeded).
            // ─────────────────────────────────────────────────────────────────────────
            modelBuilder.Entity<ShopItem>().HasData(
                new ShopItem { ShopItemId = 1, ItemId = 19, ShopSection = ShopSections.Fixed, Currency = "Gold", Price = 25m, Stock = -1, IsActive = true },
                new ShopItem { ShopItemId = 2, ItemId = 20, ShopSection = ShopSections.Fixed, Currency = "Gold", Price = 70m, Stock = -1, IsActive = true },
                new ShopItem { ShopItemId = 3, ItemId = 21, ShopSection = ShopSections.Fixed, Currency = "Gold", Price = 50m, Stock = -1, IsActive = true },
                new ShopItem { ShopItemId = 4, ItemId = 22, ShopSection = ShopSections.Fixed, Currency = "Gold", Price = 40m, Stock = -1, IsActive = true },
                new ShopItem { ShopItemId = 5, ItemId = 5, ShopSection = ShopSections.Fixed, Currency = "Gold", Price = 120m, Stock = -1, IsActive = true },
                new ShopItem { ShopItemId = 6, ItemId = 6, ShopSection = ShopSections.Fixed, Currency = "Gold", Price = 120m, Stock = -1, IsActive = true },
                new ShopItem { ShopItemId = 7, ItemId = 7, ShopSection = ShopSections.Fixed, Currency = "Gold", Price = 120m, Stock = -1, IsActive = true },
                new ShopItem { ShopItemId = 8, ItemId = 9, ShopSection = ShopSections.Fixed, Currency = "Gold", Price = 100m, Stock = -1, IsActive = true },
                new ShopItem { ShopItemId = 9, ItemId = 10, ShopSection = ShopSections.Fixed, Currency = "Gold", Price = 85m, Stock = -1, IsActive = true },
                new ShopItem { ShopItemId = 10, ItemId = 16, ShopSection = ShopSections.Fixed, Currency = "Gold", Price = 80m, Stock = -1, IsActive = true },
                new ShopItem { ShopItemId = 11, ItemId = 15, ShopSection = ShopSections.Fixed, Currency = "Gold", Price = 110m, Stock = -1, IsActive = true },
                new ShopItem { ShopItemId = 12, ItemId = 17, ShopSection = ShopSections.Fixed, Currency = "Gold", Price = 70m, Stock = -1, IsActive = true },
                new ShopItem { ShopItemId = 13, ItemId = 11, ShopSection = ShopSections.Fixed, Currency = "Gold", Price = 160m, Stock = -1, IsActive = true },
                new ShopItem { ShopItemId = 14, ItemId = 18, ShopSection = ShopSections.Fixed, Currency = "Gold", Price = 170m, Stock = -1, IsActive = true },
                new ShopItem { ShopItemId = 15, ItemId = 14, ShopSection = ShopSections.Fixed, Currency = "Gold", Price = 450m, Stock = -1, IsActive = true },
                new ShopItem { ShopItemId = 16, ItemId = 13, ShopSection = ShopSections.Fixed, Currency = "Gold", Price = 800m, Stock = -1, IsActive = true },
                new ShopItem { ShopItemId = 17, ItemId = 8, ShopSection = ShopSections.Fixed, Currency = "Gold", Price = 700m, Stock = -1, IsActive = true },
                new ShopItem { ShopItemId = 18, ItemId = 12, ShopSection = ShopSections.Fixed, Currency = "Gold", Price = 1800m, Stock = -1, IsActive = true },
                new ShopItem { ShopItemId = 19, ItemId = 4, ShopSection = ShopSections.Fixed, Currency = "Gems", Price = 100m, Stock = -1, IsActive = true }
            );

            modelBuilder.Entity<QuestRewardItem>()
                .HasOne(r => r.Quest)
                .WithMany(q => q.RewardItems)
                .HasForeignKey(r => r.QuestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<QuestRewardItem>()
                .HasOne(r => r.Item)
                .WithMany()
                .HasForeignKey(r => r.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<QuestRewardItem>()
                .HasIndex(r => new { r.QuestId, r.ItemId })
                .IsUnique();

            modelBuilder.Entity<PlayerQuest>()
                .HasIndex(pq => new { pq.PlayerProfileId, pq.QuestId })
                .IsUnique();

            modelBuilder.Entity<QuestRewardSkill>()
                .HasOne(r => r.Quest)
                .WithMany(q => q.RewardSkills)
                .HasForeignKey(r => r.QuestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<QuestRewardSkill>()
                .HasOne(r => r.Skill)
                .WithMany()
                .HasForeignKey(r => r.SkillId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<QuestRewardSkill>()
                .HasIndex(r => new { r.QuestId, r.SkillId })
                .IsUnique();
            modelBuilder.Entity<ChatMessage>()
                .HasOne(m => m.ReportedBy)
                .WithMany()
                .HasForeignKey(m => m.ReportedById)
                .OnDelete(DeleteBehavior.SetNull);


            modelBuilder.Entity<ChatMessage>()
                .Property(m => m.ReportReason)
                .HasMaxLength(500);


            modelBuilder.Entity<ChatMessage>()
                .HasIndex(m => m.ReportedById);

            modelBuilder.Entity<WorldChatMessage>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WorldChatMessage>()
                .HasOne(m => m.ReportedBy)
                .WithMany()
                .HasForeignKey(m => m.ReportedById)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<WorldChatMessage>()
                .Property(m => m.ReportReason)
                .HasMaxLength(500);

            // Index phủ đúng truy vấn phân trang world chat:
            // WHERE IsHidden = false ORDER BY SentAt DESC, WorldChatMessageId DESC.
            // Chỉ index SentAt thì Postgres vẫn phải sort lại và filter IsHidden riêng.
            modelBuilder.Entity<WorldChatMessage>()
                .HasIndex(m => new { m.IsHidden, m.SentAt, m.WorldChatMessageId })
                .HasDatabaseName("IX_WorldChatMessages_Feed")
                .IsDescending(false, true, true);

            modelBuilder.Entity<WorldChatMessage>()
                .HasIndex(m => m.SenderId);

            modelBuilder.Entity<WorldChatMessage>()
                .HasIndex(m => m.ReportedById);

            modelBuilder.Entity<ChatModerationPenalty>()
                .HasOne(p => p.PlayerProfile)
                .WithMany()
                .HasForeignKey(p => p.PlayerProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ChatModerationPenalty>()
                .HasOne(p => p.Reporter)
                .WithMany()
                .HasForeignKey(p => p.ReporterId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ChatModerationPenalty>()
                .HasOne(p => p.ChatMessage)
                .WithMany()
                .HasForeignKey(p => p.ChatMessageId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ChatModerationPenalty>()
                .HasOne(p => p.WorldChatMessage)
                .WithMany()
                .HasForeignKey(p => p.WorldChatMessageId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ChatModerationPenalty>()
                .Property(p => p.Channel)
                .HasMaxLength(30);

            modelBuilder.Entity<ChatModerationPenalty>()
                .Property(p => p.ContentSnapshot)
                .HasMaxLength(500);

            modelBuilder.Entity<ChatModerationPenalty>()
                .Property(p => p.ReportReason)
                .HasMaxLength(500);

            modelBuilder.Entity<ChatModerationPenalty>()
                .Property(p => p.MatchedTerms)
                .HasMaxLength(500);

            modelBuilder.Entity<ChatModerationPenalty>()
                .HasIndex(p => p.PlayerProfileId);

            modelBuilder.Entity<ChatModerationPenalty>()
                .HasIndex(p => p.ReporterId);

            modelBuilder.Entity<ChatModerationPenalty>()
                .HasIndex(p => p.ChatMessageId);

            modelBuilder.Entity<ChatModerationPenalty>()
                .HasIndex(p => p.WorldChatMessageId);

            modelBuilder.Entity<ChatModerationPenalty>()
                .HasIndex(p => p.LockedUntil);

            modelBuilder.Entity<FriendBlock>()
                .HasOne(fb => fb.Blocker)
                .WithMany()
                .HasForeignKey(fb => fb.BlockerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FriendBlock>()
                .HasOne(fb => fb.Blocked)
                .WithMany()
                .HasForeignKey(fb => fb.BlockedId)
                .OnDelete(DeleteBehavior.Restrict);

            // Guild relationships
            modelBuilder.Entity<Guild>()
                .HasOne(g => g.Leader)
                .WithMany()
                .HasForeignKey(g => g.LeaderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Guild>()
                .HasOne(g => g.CreatedBy)
                .WithMany()
                .HasForeignKey(g => g.CreatedByProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Guild>()
                .Ignore(g => g.MaxMembers)
                .Ignore(g => g.ExpToNextLevel);

            modelBuilder.Entity<GuildMember>()
                .HasIndex(m => m.PlayerProfileId)
                .IsUnique();

            modelBuilder.Entity<GuildLog>()
                .HasOne(l => l.Guild)
                .WithMany(g => g.Logs)
                .HasForeignKey(l => l.GuildId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GuildLog>()
                .HasOne(l => l.Actor)
                .WithMany()
                .HasForeignKey(l => l.ActorProfileId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<GuildLog>()
                .HasOne(l => l.Target)
                .WithMany()
                .HasForeignKey(l => l.TargetProfileId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<GuildLog>()
                .HasIndex(l => l.GuildId);

            modelBuilder.Entity<Skill>().HasData(
                new Skill { SkillId = 1, Name = "Accelerationarrow", Description = "Automatically fires in the direction the archer is facing.", Type = "Active", DamageType = "Physical", TargetType = "SingleTarget", ClassRequirement = "Archer", CooldownSeconds = 2, BaseDamage = 55.0, DamagePerLevel = 8.0, DamageGrowthPercent = 3.0, UnlockLevel = 1, CorruptionCost = 0, IsActive = true },
                new Skill { SkillId = 2, Name = "ArrowofLight", Description = "Automatically fires in the direction the archer is facing.", Type = "Active", DamageType = "Physical", TargetType = "SingleTarget", ClassRequirement = "Archer", CooldownSeconds = 5, BaseDamage = 115.0, DamagePerLevel = 14.0, DamageGrowthPercent = 3.5, UnlockLevel = 1, CorruptionCost = 0, IsActive = true },
                new Skill { SkillId = 3, Name = "Holymagic", Description = "Heals allies within range.", Type = "Buff", DamageType = "Magical", TargetType = "Ally", ClassRequirement = "Mage", CooldownSeconds = 4, BaseDamage = 0, DamagePerLevel = 0, DamageGrowthPercent = 0, UnlockLevel = 1, CorruptionCost = 0, IsActive = true },
                new Skill { SkillId = 4, Name = "Purification", Description = "Casts a spell in the direction the character is facing.", Type = "Active", DamageType = "Magical", TargetType = "SingleTarget", ClassRequirement = "Mage", CooldownSeconds = 3, BaseDamage = 75.0, DamagePerLevel = 10.0, DamageGrowthPercent = 3.0, UnlockLevel = 1, CorruptionCost = 0, IsActive = true },
                new Skill { SkillId = 5, Name = "Stardust", Description = "Selects and attacks a random monster within range.", Type = "Active", DamageType = "Magical", TargetType = "SingleTarget", ClassRequirement = "Mage", CooldownSeconds = 3, BaseDamage = 75.0, DamagePerLevel = 10.0, DamageGrowthPercent = 3.0, UnlockLevel = 1, CorruptionCost = 0, IsActive = true },
                new Skill { SkillId = 6, Name = "Lightsabers", Description = "Selects a target with the monster tag to attack.", Type = "Active", DamageType = "Physical", TargetType = "SingleTarget", ClassRequirement = "Knight", CooldownSeconds = 5, BaseDamage = 115.0, DamagePerLevel = 14.0, DamageGrowthPercent = 3.5, UnlockLevel = 1, CorruptionCost = 0, IsActive = true },
                new Skill { SkillId = 7, Name = "LightWaves", Description = "Casts a spell in the direction the character is facing.", Type = "Active", DamageType = "Physical", TargetType = "Area", ClassRequirement = "Knight", CooldownSeconds = 4, BaseDamage = 95.0, DamagePerLevel = 12.0, DamageGrowthPercent = 3.5, UnlockLevel = 1, CorruptionCost = 0, IsActive = true },
                new Skill { SkillId = 8, Name = "ProtectiveShield", Description = "Protects all allies within range.", Type = "Buff", DamageType = "Magical", TargetType = "Ally", ClassRequirement = "Knight", CooldownSeconds = 8, BaseDamage = 0, DamagePerLevel = 0, DamageGrowthPercent = 0, UnlockLevel = 1, CorruptionCost = 0, IsActive = true },
                new Skill { SkillId = 9, Name = "DarkExplosion", Description = "Shared among all classes. Deals damage equal to 3x base damage. Increases corruption points by 15.", Type = "Active", DamageType = "Magical", TargetType = "Area", ClassRequirement = "All", CooldownSeconds = 8, BaseDamage = 180.0, DamagePerLevel = 22.0, DamageGrowthPercent = 4.0, UnlockLevel = 1, CorruptionCost = 15, IsActive = true },
                new Skill { SkillId = 10, Name = "DarkPoisonZone", Description = "Shared among all classes. Deals damage equal to 2x base damage. Increases corruption points by 10.", Type = "Active", DamageType = "Magical", TargetType = "Area", ClassRequirement = "All", CooldownSeconds = 6, BaseDamage = 140.0, DamagePerLevel = 18.0, DamageGrowthPercent = 4.0, UnlockLevel = 1, CorruptionCost = 10, IsActive = true },
                new Skill { SkillId = 11, Name = "DeadlyCurse", Description = "Automatically fires in the direction the archer is facing.", Type = "Active", DamageType = "Physical", TargetType = "SingleTarget", ClassRequirement = "Archer", CooldownSeconds = 5, BaseDamage = 115.0, DamagePerLevel = 14.0, DamageGrowthPercent = 3.5, UnlockLevel = 1, CorruptionCost = 0, IsActive = true },
                new Skill { SkillId = 12, Name = "NightMagic", Description = "Selects an area within range to attack.", Type = "Active", DamageType = "Magical", TargetType = "Area", ClassRequirement = "Mage", CooldownSeconds = 2, BaseDamage = 55.0, DamagePerLevel = 8.0, DamageGrowthPercent = 3.0, UnlockLevel = 1, CorruptionCost = 0, IsActive = true },
                new Skill { SkillId = 13, Name = "DeadlyExplosion", Description = "Shared among all classes. Deals damage equal to 3x base damage. Increases corruption points by 8.", Type = "Active", DamageType = "Magical", TargetType = "SingleTarget", ClassRequirement = "All", CooldownSeconds = 6, BaseDamage = 140.0, DamagePerLevel = 18.0, DamageGrowthPercent = 4.0, UnlockLevel = 1, CorruptionCost = 8, IsActive = true },
                new Skill { SkillId = 14, Name = "BloodySlash", Description = "A short-range slash in the direction the knight is facing.", Type = "Active", DamageType = "Physical", TargetType = "SingleTarget", ClassRequirement = "Knight", CooldownSeconds = 2, BaseDamage = 55.0, DamagePerLevel = 8.0, DamageGrowthPercent = 3.0, UnlockLevel = 1, CorruptionCost = 0, IsActive = true },
                new Skill { SkillId = 15, Name = "FrozenSash", Description = "Selects an area within range to unleash an icy slash.", Type = "Active", DamageType = "Physical", TargetType = "Area", ClassRequirement = "Knight", CooldownSeconds = 3, BaseDamage = 75.0, DamagePerLevel = 10.0, DamageGrowthPercent = 3.0, UnlockLevel = 1, CorruptionCost = 0, IsActive = true },
                new Skill { SkillId = 16, Name = "PumpkinMagic", Description = "Summons a magical pumpkin trap that lasts 5 seconds. Explodes when touched by monsters or when duration expires, dealing AoE physical damage.", Type = "Active", DamageType = "Physical", TargetType = "Area", ClassRequirement = "Archer", CooldownSeconds = 5, BaseDamage = 115.0, DamagePerLevel = 14.0, DamageGrowthPercent = 3.5, UnlockLevel = 1, CorruptionCost = 0, IsActive = true },
                new Skill { SkillId = 17, Name = "PumpkinThrow", Description = "Throws an explosive pumpkin in a parabolic arc. Explodes on impact with any object, dealing AoE physical damage to monsters.", Type = "Active", DamageType = "Physical", TargetType = "Area", ClassRequirement = "Knight", CooldownSeconds = 5, BaseDamage = 115.0, DamagePerLevel = 14.0, DamageGrowthPercent = 3.5, UnlockLevel = 1, CorruptionCost = 0, IsActive = true },
                new Skill { SkillId = 18, Name = "PumpkinSlash", Description = "A short-range pumpkin slash in the direction the knight is facing.", Type = "Active", DamageType = "Physical", TargetType = "SingleTarget", ClassRequirement = "Knight", CooldownSeconds = 2, BaseDamage = 55.0, DamagePerLevel = 8.0, DamageGrowthPercent = 3.0, UnlockLevel = 1, CorruptionCost = 0, IsActive = true },
                new Skill { SkillId = 19, Name = "BoomBoomPumpkin", Description = "Summons a magic pumpkin that explodes immediately at the target location, dealing light magical AoE damage with a short cooldown.", Type = "Active", DamageType = "Magical", TargetType = "Area", ClassRequirement = "Mage", CooldownSeconds = 2, BaseDamage = 55.0, DamagePerLevel = 8.0, DamageGrowthPercent = 3.0, UnlockLevel = 1, CorruptionCost = 0, IsActive = true }
            );

            // ─────────────────────────────────────────────────────────────────────────
            // ITEMS – System items with fixed IDs so EF snapshot includes them.
            // ID 1-29: from SeedSystemItems | ID 30-33: from SeedGameStoryQuests
            // ID 901-912: boss-drop items (already seeded above)
            // ─────────────────────────────────────────────────────────────────────────
            var utc2024 = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            modelBuilder.Entity<Item>().HasData(
                // ── Currency ─────────────────────────────────────────────────────────
                new Item { ItemId = 1,  Name = "Gold",                 Description = "In-game gold currency.",                                                                      Type = "Currency",   Rarity = "Common",    Slot = "None",    BaseValue = 1m,    MaxStack = 2147483647, IsActive = true, CorruptionReduction = 0, CreatedAt = utc2024 },
                new Item { ItemId = 2,  Name = "Exp",                  Description = "Experience points for leveling up.",                                                          Type = "Currency",   Rarity = "Common",    Slot = "None",    BaseValue = 1m,    MaxStack = 2147483647, IsActive = true, CorruptionReduction = 0, CreatedAt = utc2024 },
                new Item { ItemId = 3,  Name = "Gem",                  Description = "Premium gem used to purchase high-tier items.",                                              Type = "Currency",   Rarity = "Rare",      Slot = "None",    BaseValue = 5m,    MaxStack = 2147483647, IsActive = true, CorruptionReduction = 0, CreatedAt = utc2024 },
                new Item { ItemId = 4,  Name = "Lucky Ticket",         Description = "Lucky ticket used to spin the gacha banner.",                                               Type = "Consumable", Rarity = "Rare",      Slot = "None",    BaseValue = 1m,    MaxStack = 99,         IsActive = true, CorruptionReduction = 0, CreatedAt = utc2024 },
                // ── Weapon ───────────────────────────────────────────────────────────
                new Item { ItemId = 5,  Name = "Iron Sword",           Description = "Basic iron sword for beginner warriors.",                                                    Type = "Weapon",     Rarity = "Common",    Slot = "Weapon",  BaseValue = 150m,  MaxStack = 1,          IsActive = true, CorruptionReduction = 0, CreatedAt = utc2024 },
                new Item { ItemId = 6,  Name = "Hunter Bow",           Description = "A forest hunter bow, light and accurate.",                                                  Type = "Weapon",     Rarity = "Common",    Slot = "Weapon",  BaseValue = 150m,  MaxStack = 1,          IsActive = true, CorruptionReduction = 0, CreatedAt = utc2024 },
                new Item { ItemId = 7,  Name = "Apprentice Staff",     Description = "A novice magic staff for casting light spells.",                                            Type = "Weapon",     Rarity = "Common",    Slot = "Weapon",  BaseValue = 150m,  MaxStack = 1,          IsActive = true, CorruptionReduction = 0, CreatedAt = utc2024 },
                new Item { ItemId = 8,  Name = "Elven Blade",          Description = "A glowing elven blade, forged deep in the ancient forest.",                               Type = "Weapon",     Rarity = "Epic",      Slot = "Weapon",  BaseValue = 800m,  MaxStack = 1,          IsActive = true, CorruptionReduction = 0, CreatedAt = utc2024 },
                // ── Armor ────────────────────────────────────────────────────────────
                new Item { ItemId = 9,  Name = "Leather Armor",        Description = "Light leather armor that provides basic defense.",                                           Type = "Armor",      Rarity = "Common",    Slot = "Armor",   BaseValue = 120m,  MaxStack = 1,          IsActive = true, CorruptionReduction = 0, CreatedAt = utc2024 },
                new Item { ItemId = 10, Name = "Iron Helmet",          Description = "Sturdy iron helmet that protects the head from damage.",                                    Type = "Armor",      Rarity = "Common",    Slot = "Helmet",  BaseValue = 100m,  MaxStack = 1,          IsActive = true, CorruptionReduction = 0, CreatedAt = utc2024 },
                new Item { ItemId = 11, Name = "Wind Boots",           Description = "Wind-infused boots that increase movement speed.",                                          Type = "Armor",      Rarity = "Uncommon",  Slot = "Boots",   BaseValue = 200m,  MaxStack = 1,          IsActive = true, CorruptionReduction = 0, CreatedAt = utc2024 },
                new Item { ItemId = 12, Name = "Dragon Scale Armor",   Description = "Legendary dragon scale armor offering supreme defense.",                                    Type = "Armor",      Rarity = "Legendary", Slot = "Armor",   BaseValue = 2000m, MaxStack = 1,          IsActive = true, CorruptionReduction = 0, CreatedAt = utc2024 },
                new Item { ItemId = 13, Name = "Phantom Cloak",        Description = "Shadow cloak that boosts speed and evasion.",                                               Type = "Armor",      Rarity = "Epic",      Slot = "Armor",   BaseValue = 900m,  MaxStack = 1,          IsActive = true, CorruptionReduction = 0, CreatedAt = utc2024 },
                new Item { ItemId = 14, Name = "Shadow Hood",          Description = "Dark hood that increases critical strike damage.",                                           Type = "Armor",      Rarity = "Rare",      Slot = "Helmet",  BaseValue = 500m,  MaxStack = 1,          IsActive = true, CorruptionReduction = 0, CreatedAt = utc2024 },
                new Item { ItemId = 15, Name = "Iron Gauntlets",       Description = "Iron gauntlets that increase physical damage.",                                              Type = "Armor",      Rarity = "Common",    Slot = "Gloves",  BaseValue = 120m,  MaxStack = 1,          IsActive = true, CorruptionReduction = 0, CreatedAt = utc2024 },
                new Item { ItemId = 16, Name = "Leather Gauntlets",    Description = "Soft leather gauntlets that allow flexible combat.",                                        Type = "Armor",      Rarity = "Common",    Slot = "Gloves",  BaseValue = 100m,  MaxStack = 1,          IsActive = true, CorruptionReduction = 0, CreatedAt = utc2024 },
                new Item { ItemId = 17, Name = "Copper Ring",          Description = "Basic copper ring that slightly boosts stats.",                                              Type = "Armor",      Rarity = "Common",    Slot = "Ring",    BaseValue = 80m,   MaxStack = 1,          IsActive = true, CorruptionReduction = 0, CreatedAt = utc2024 },
                new Item { ItemId = 18, Name = "Silver Necklace",      Description = "Silver necklace that increases maximum energy.",                                            Type = "Armor",      Rarity = "Uncommon",  Slot = "Necklace",BaseValue = 200m,  MaxStack = 1,          IsActive = true, CorruptionReduction = 0, CreatedAt = utc2024 },
                // ── Consumable ───────────────────────────────────────────────────────
                new Item { ItemId = 19, Name = "Small Health Potion",  Description = "Small health potion that restores 80 HP.",                                                  Type = "Consumable", Rarity = "Common",    Slot = "None",    BaseValue = 30m,   MaxStack = 99,         IsActive = true, CorruptionReduction = 0, CreatedAt = utc2024 },
                new Item { ItemId = 20, Name = "Large Health Potion",  Description = "Large health potion that restores 200 HP.",                                                 Type = "Consumable", Rarity = "Uncommon",  Slot = "None",    BaseValue = 80m,   MaxStack = 99,         IsActive = true, CorruptionReduction = 0, CreatedAt = utc2024 },
                new Item { ItemId = 21, Name = "Energy Elixir",        Description = "Energy elixir that restores 60 Energy.",                                                   Type = "Consumable", Rarity = "Uncommon",  Slot = "None",    BaseValue = 60m,   MaxStack = 99,         IsActive = true, CorruptionReduction = 0, CreatedAt = utc2024 },
                // ── Material ─────────────────────────────────────────────────────────
                new Item { ItemId = 22, Name = "Skill Upgrade Stone",  Description = "Magic stone used to upgrade player skills.",                                                 Type = "Material",   Rarity = "Rare",      Slot = "None",    BaseValue = 50m,   MaxStack = 999,        IsActive = true, CorruptionReduction = 0, CreatedAt = utc2024 },
                // ── Quest Items ───────────────────────────────────────────────────────
                new Item { ItemId = 23, Name = "White Flower",         Description = "White flower collected in the fairy forest.",                                               Type = "QuestItem",  Rarity = "Common",    Slot = "None",    BaseValue = 0m,    MaxStack = 99,         IsActive = true, CorruptionReduction = 0, CreatedAt = utc2024 },
                new Item { ItemId = 24, Name = "Wood Logs",            Description = "Logs collected from the ancient forest.",                                                   Type = "QuestItem",  Rarity = "Common",    Slot = "None",    BaseValue = 0m,    MaxStack = 99,         IsActive = true, CorruptionReduction = 0, CreatedAt = utc2024 },
                new Item { ItemId = 25, Name = "Ancient Leaves",       Description = "Ancient tree leaves collected from the fairy forest.",                                      Type = "QuestItem",  Rarity = "Common",    Slot = "None",    BaseValue = 0m,    MaxStack = 99,         IsActive = true, CorruptionReduction = 0, CreatedAt = utc2024 },
                // ── Seal Books (QuestItem – dropped by Bosses) ────────────────────────
                new Item { ItemId = 26, Name = "Dragon Seal Book",     Description = "Dragon Seal Book. Dropped by DragonBossIdle. Collect all 4 seal books to save the World Tree.", Type = "QuestItem", Rarity = "Epic", Slot = "None", BaseValue = 0m, MaxStack = 1, IsActive = true, CorruptionReduction = 0, CreatedAt = utc2024 },
                new Item { ItemId = 27, Name = "Golem Seal Book",      Description = "Golem Seal Book. Dropped by GolemBoss.",                                                   Type = "QuestItem",  Rarity = "Epic",      Slot = "None",    BaseValue = 0m,    MaxStack = 1,          IsActive = true, CorruptionReduction = 0, CreatedAt = utc2024 },
                new Item { ItemId = 28, Name = "UnderKing Seal Book",  Description = "UnderKing Seal Book. Dropped by the UnderKing boss.",                                      Type = "QuestItem",  Rarity = "Epic",      Slot = "None",    BaseValue = 0m,    MaxStack = 1,          IsActive = true, CorruptionReduction = 0, CreatedAt = utc2024 },
                new Item { ItemId = 29, Name = "Swamp Seal Book",      Description = "Swamp Demon Seal Book. Dropped by SwampDemon boss.",                                       Type = "QuestItem",  Rarity = "Epic",      Slot = "None",    BaseValue = 0m,    MaxStack = 1,          IsActive = true, CorruptionReduction = 0, CreatedAt = utc2024 },
                // ── Quest Items from Chapter 2-4 ──────────────────────────────────────
                new Item { ItemId = 30, Name = "Enchanted Pumpkin",    Description = "A magical pumpkin glowing with autumn energy.",                                             Type = "QuestItem",  Rarity = "Common",    Slot = "None",    BaseValue = 0m,    MaxStack = 99,         IsActive = true, CorruptionReduction = 0, CreatedAt = utc2024 },
                new Item { ItemId = 31, Name = "Magic Flour",          Description = "Mystical flour imbued with purifying magic. Reduces your corruption by 50% when consumed.",  Type = "Consumable", Rarity = "Uncommon", Slot = "None",    BaseValue = 50m,   MaxStack = 99,         IsActive = true, CorruptionReduction = 0.5f, CreatedAt = utc2024 },
                new Item { ItemId = 32, Name = "Spirit Skull",         Description = "A skull radiating with ghostly presence.",                                                 Type = "QuestItem",  Rarity = "Common",    Slot = "None",    BaseValue = 0m,    MaxStack = 99,         IsActive = true, CorruptionReduction = 0, CreatedAt = utc2024 },
                new Item { ItemId = 33, Name = "Mystic Key",           Description = "A key that opens the castle on the deserted island.",                                       Type = "QuestItem",  Rarity = "Epic",      Slot = "None",    BaseValue = 0m,    MaxStack = 1,          IsActive = true, CorruptionReduction = 0, CreatedAt = utc2024 },
                new Item { ItemId = 34, Name = "Tide-Knell Remembrance", Description = "A remembrance token recovered from the dead of Tide-Knell.",                              Type = "QuestItem",  Rarity = "Common",    Slot = "None",    BaseValue = 0m,    MaxStack = 99,         IsActive = true, CorruptionReduction = 0, CreatedAt = utc2024 },
                new Item { ItemId = 35, Name = "Natalie's Memory",      Description = "A keepsake carrying a fragment of Natalie's family memories.",                               Type = "QuestItem",  Rarity = "Common",    Slot = "None",    BaseValue = 0m,    MaxStack = 99,         IsActive = true, CorruptionReduction = 0, CreatedAt = utc2024 },
                new Item { ItemId = 36, Name = "Warden Relic",          Description = "A relic left by the wardens who sealed King Aderyn beneath the island.",                     Type = "QuestItem",  Rarity = "Common",    Slot = "None",    BaseValue = 0m,    MaxStack = 99,         IsActive = true, CorruptionReduction = 0, CreatedAt = utc2024 }
            );

            // ─────────────────────────────────────────────────────────────────────────
            // EQUIPMENT STATS – for system weapon/armor items (IDs match their ItemId)
            // ─────────────────────────────────────────────────────────────────────────

            // ─────────────────────────────────────────────────────────────────────────
            // NPCs – Fixed IDs so Quest/Dialogue FK references are stable
            //
            // PositionX/Y are LOCAL to the scene's "NPC" container object, not world
            // coordinates: WorldNpcSpawnerRuntime assigns them to transform.localPosition
            // under npcContainer. Those containers are not at the origin, so a world
            // coordinate pasted in here gets the container offset added on top and the NPC
            // lands far off-map (looks like "the NPC never spawned").
            // Container offsets (đọc từ transform của GameObject "NPC" mà WorldNpcSpawnerRuntime
            // trỏ tới qua field npcContainer, trong từng file .unity):
            //   ElfForest (0, 0) · AutumnPumpkin (0, 0)
            //   FrozenMountain (-8.12395, 26.35298) · AbandonedCastle (18.19, 5.05)
            // To convert a position read off the Unity inspector: subtract the offset.
            // ElfForest + AutumnPumpkin ở gốc nên toạ độ dưới đây = toạ độ world luôn (ví dụ Arthur
            // (-32, 58) đúng bằng chỗ anh ta đứng trong scene). Trước đây comment này ghi
            // AutumnPumpkin (-108.53592, 135.28775) — sai, trừ theo đó là NPC bay ra khỏi map.
            // ─────────────────────────────────────────────────────────────────────────
            modelBuilder.Entity<NPC>().HasData(
                new NPC { NPCId = 1,  Name = "Elder Rowan",           Description = "The wise guide of the Elf Forest.",              Type = "QuestGiver", MapName = "ElfForest",      PositionX = -0.7,    PositionY = 18.5,    InteractionRadius = 2.5f, IsActive = true },
                new NPC { NPCId = 2,  Name = "Lyra",                  Description = "A spirit of the forest.",                        Type = "QuestGiver", MapName = "ElfForest",      PositionX = 30,   PositionY = -6,   InteractionRadius = 2.5f, IsActive = true },
                new NPC { NPCId = 3,  Name = "Mysterious Figure",     Description = "A mysterious figure in a cloak.",               Type = "QuestGiver", MapName = "ElfForest",      PositionX = 14,   PositionY = -47.5,   InteractionRadius = 2.5f, IsActive = true },
                new NPC { NPCId = 4,  Name = "Drake",                 Description = "A weathered guide in the pumpkin town.",         Type = "QuestGiver", MapName = "AutumnPumpkin",  PositionX = -105.5,   PositionY = 40.7,   InteractionRadius = 2.5f, IsActive = true },
                new NPC { NPCId = 5,  Name = "Tristan",               Description = "The city gate guard.",                          Type = "QuestGiver", MapName = "AutumnPumpkin",  PositionX = -97.3 ,  PositionY = 21.4,    InteractionRadius = 2.5f, IsActive = true },
                new NPC { NPCId = 6,  Name = "Arthur",                Description = "The silver knight.",                            Type = "QuestGiver", MapName = "AutumnPumpkin",  PositionX = -32,  PositionY = 58,    InteractionRadius = 2.5f, IsActive = true },
                new NPC { NPCId = 7,  Name = "Fa",                    Description = "A farmer collecting enchanted pumpkins.",       Type = "QuestGiver", MapName = "AutumnPumpkin",  PositionX = -101,  PositionY = -26,   InteractionRadius = 2.5f, IsActive = true },
                new NPC { NPCId = 8,  Name = "Roselyn Aurora Queen",  Description = "Queen of the frozen lands.",                    Type = "QuestGiver", MapName = "FrozenMountain", PositionX = 146.8143,   PositionY = -11.63209,    InteractionRadius = 2.5f, IsActive = true },
                new NPC { NPCId = 9,  Name = "Zephyr",                Description = "The witch and disguised priest.",               Type = "QuestGiver", MapName = "FrozenMountain", PositionX = -4.87365,   PositionY = 25.72625,  InteractionRadius = 2.5f, IsActive = true },
                new NPC { NPCId = 10, Name = "Roland",                Description = "The forbidden zone guard.",                     Type = "QuestGiver", MapName = "FrozenMountain", PositionX = 130.43,   PositionY = 28.79,    InteractionRadius = 2.5f, IsActive = true },
                new NPC { NPCId = 11, Name = "Valiant Warrior",       Description = "A battle-worn soldier who returned to Tide-Knell too late — Natalie's father, still guarding the valley from the dead.",           Type = "QuestGiver", MapName = "AbandonedCastle",PositionX = 29.82,  PositionY = 129.1,    InteractionRadius = 2.5f, IsActive = true },
                new NPC { NPCId = 12, Name = "Natalie",               Description = "The ghost of a lonely girl whose desperate wish for friends doomed Tide-Knell.",                    Type = "QuestGiver", MapName = "AbandonedCastle",PositionX = -10.65,  PositionY = 58.85,   InteractionRadius = 2.5f, IsActive = true },
                new NPC { NPCId = 13, Name = "Elf Guard",             Description = "The lone guard of the deserted island.",        Type = "QuestGiver", MapName = "AbandonedCastle",PositionX = -101.83,   PositionY = 32.9,    InteractionRadius = 2.5f, IsActive = true },
                // Cedric đứng ngay chỗ thuyền thả người chơi xuống FrozenMountain (SpawnPoint_Tutorial
                // = world (-13.1, -44.2)), lệch ~4m để không đè lên player. Local = world - offset
                // container (-8.12395, 26.35298). Anh ta là NPC ĐẦU TIÊN của chương 3 nên phải nằm
                // trong tầm mắt lúc vừa cập bờ, không phải ở citadel cùng Nữ hoàng.
                new NPC { NPCId = 14, Name = "Cedric",                Description = "Captain of the snow-field militia.",            Type = "QuestGiver", MapName = "FrozenMountain", PositionX = 5.53,      PositionY = -8.62,      InteractionRadius = 2.5f, IsActive = true },
                new NPC { NPCId = 15, Name = "Brother Cael",          Description = "The last keeper of King Aderyn's history, living among the island ruins.", Type = "QuestGiver", MapName = "AbandonedCastle", PositionX = -119.29, PositionY = -10.89, InteractionRadius = 2.5f, IsActive = true }
            );

            // ─────────────────────────────────────────────────────────────────────────
            // QUESTS – 46 main story quests with fixed IDs, in play order.
            //
            // Rules for this block (keep them when editing):
            //   • Title must describe THIS quest's own objective, never the next step.
            //   • Description is player-facing prose: no monster/prefab/dungeon ids.
            //   • ObjectiveTarget is matched at runtime by the client (monster prefab
            //     name, item name, interactable key) — do NOT prettify these strings.
            //     Multi-monster objectives use a '/'-separated list (see Q15).
            //   • RequiredLevel and RewardExperience rise monotonically with QuestId:
            //     the client sorts the main chain by RequiredLevel then QuestId.
            // ─────────────────────────────────────────────────────────────────────────
            modelBuilder.Entity<Quest>().HasData(
                // ── MAP 1: Elf Forest ────────────────────────────────────────────────
                new Quest { QuestId = 1,  Title = "[Chapter 1] A Word with Elder Rowan",      Description = "You wake at the edge of the Elf Forest with no memory of how you arrived. Elder Rowan is waiting by the great roots — go to him and hear why the forest called you here.",                Type = "Main", DefaultStatus = "NotStarted", MapName = "ElfForest",      RequiredLevel = 1,  TargetAmount = 1,  RewardExperience = 15,    RewardGold = 20m,   RewardGems = 3m, ObjectiveType = "Talk",       ObjectiveTarget = "Elder Rowan",    ObjectiveLocation = "Elf Forest",       QuestGiverName = "Elder Rowan",          IsActive = true },
                new Quest { QuestId = 2,  Title = "[Chapter 1] Gather White Flowers",         Description = "The elders brew their healing draught from white flowers that only bloom in the shade of the old woods. Search the clearings and gather 3 White Flowers for Elder Rowan.",              Type = "Main", DefaultStatus = "NotStarted", MapName = "ElfForest",      RequiredLevel = 1,  TargetAmount = 3,  RewardExperience = 15,   RewardGold = 20m,    RewardGems = 3m, ObjectiveType = "Collect",    ObjectiveTarget = "White Flower",   ObjectiveLocation = "Elf Forest",       QuestGiverName = "Elder Rowan",          IsActive = true },
                new Quest { QuestId = 3,  Title = "[Chapter 1] Deliver the White Flowers",    Description = "Bring the gathered flowers back to Elder Rowan. In return he will teach you the first strike an elf ever learns.",                                                                     Type = "Main", DefaultStatus = "NotStarted", MapName = "ElfForest",      RequiredLevel = 1,  TargetAmount = 1,  RewardExperience = 15,    RewardGold = 20m,    RewardGems = 3m, ObjectiveType = "Talk",       ObjectiveTarget = "Elder Rowan",    ObjectiveLocation = "Elf Forest",       QuestGiverName = "Elder Rowan",          IsActive = true, RewardSkillId = 10 },
                new Quest { QuestId = 4,  Title = "[Chapter 1] Equip Your First Skill",       Description = "A skill is useless until it sits in your hand. Open the Skill panel and equip the technique Elder Rowan just taught you.",                                                             Type = "Main", DefaultStatus = "NotStarted", MapName = "ElfForest",      RequiredLevel = 1,  TargetAmount = 1,  RewardExperience = 15,   RewardGold = 20m,   RewardGems = 3m, ObjectiveType = "EquipSkill", ObjectiveTarget = "Skill Panel",    ObjectiveLocation = "Elf Forest",       QuestGiverName = "Elder Rowan",          IsActive = true },
                new Quest { QuestId = 5,  Title = "[Chapter 1] Cull the Little Slimes",       Description = "Little slimes have crept out of the marsh and are eating the flower beds. Put your new skill to work and defeat 3 of them.",                                                           Type = "Main", DefaultStatus = "NotStarted", MapName = "ElfForest",      RequiredLevel = 1,  TargetAmount = 3,  RewardExperience = 15,   RewardGold = 20m,   RewardGems = 3m, ObjectiveType = "Defeat",     ObjectiveTarget = "Slime Little",    ObjectiveLocation = "Elf Forest",       QuestGiverName = "Elder Rowan",          IsActive = true },
                new Quest { QuestId = 6,  Title = "[Chapter 1] Slay the Swamp Demon",         Description = "The slimes were only fleeing something worse. A Swamp Demon broods in the deep woods over some old relic, and the water rots around it. Kill it and take whatever it is guarding.",                                       Type = "Main", DefaultStatus = "NotStarted", MapName = "ElfForest",      RequiredLevel = 2,  TargetAmount = 1,  RewardExperience = 65,   RewardGold = 60m,   RewardGems = 8m, ObjectiveType = "Defeat",     ObjectiveTarget = "Swamp Demon",     ObjectiveLocation = "Deep Woods",       QuestGiverName = "Elder Rowan",          IsActive = true, BossMonsterId = 2 },
                new Quest { QuestId = 7,  Title = "[Chapter 1] Lyra and the Origin Tree",     Description = "Rowan cannot name the relic you took from the swamp. Carry it to Lyra at the Origin Tree — she is older than every elf alive, and she will know what you are holding.",                              Type = "Main", DefaultStatus = "NotStarted", MapName = "ElfForest",      RequiredLevel = 2,  TargetAmount = 1,  RewardExperience = 15,   RewardGold = 20m,   RewardGems = 3m, ObjectiveType = "Talk",       ObjectiveTarget = "Lyra",           ObjectiveLocation = "Origin Tree",      QuestGiverName = "Lyra",                 IsActive = true },
                new Quest { QuestId = 8,  Title = "[Chapter 1] Follow the Cloaked Figure",    Description = "A cloaked figure has been watching you since you woke, and now walks into a portal at the forest edge. Step through it before the way closes.",                                        Type = "Main", DefaultStatus = "NotStarted", MapName = "ElfForest",      RequiredLevel = 2,  TargetAmount = 1,  RewardExperience = 15,    RewardGold = 55m,    RewardGems = 7m, ObjectiveType = "Explore",    ObjectiveTarget = "Portal",         ObjectiveLocation = "Elf Forest",       QuestGiverName = "Mysterious Figure",    IsActive = true },
                // ── MAP 2: Autumn Pumpkin ────────────────────────────────────────────
                new Quest { QuestId = 9,  Title = "[Chapter 2] Ask Where You Are",            Description = "The portal spits you onto a cold beach under an autumn sky. Climb to the castle and find Drake, the one soul here willing to speak to a stranger, and ask what land this is.",                    Type = "Main", DefaultStatus = "NotStarted", MapName = "AutumnPumpkin",  RequiredLevel = 3,  TargetAmount = 1,  RewardExperience = 10,  RewardGold = 30m,    RewardGems = 4m, ObjectiveType = "Talk",       ObjectiveTarget = "Drake",          ObjectiveLocation = "Autumn Pumpkin",   QuestGiverName = "Drake",                IsActive = true },
                new Quest { QuestId = 10, Title = "[Chapter 2] Harvest for Your Supper",      Description = "You have no coin in this land and no one gives bread away. Farmer Fa will trade a meal for labour: pick 8 Enchanted Pumpkins from his field.",                                        Type = "Main", DefaultStatus = "NotStarted", MapName = "AutumnPumpkin",  RequiredLevel = 3,  TargetAmount = 8,  RewardExperience = 10,  RewardGold = 30m,   RewardGems = 4m, ObjectiveType = "Collect",    ObjectiveTarget = "Enchanted Pumpkin",ObjectiveLocation = "Pumpkin Town",   QuestGiverName = "Fa",                   IsActive = true },
                new Quest { QuestId = 11, Title = "[Chapter 2] Deliver the Harvest",          Description = "Fa is too old to make the road alone. Carry the harvest to the city gate and hand it to the guard Tristan.",                                                                          Type = "Main", DefaultStatus = "NotStarted", MapName = "AutumnPumpkin",  RequiredLevel = 3,  TargetAmount = 1,  RewardExperience = 10,  RewardGold = 30m,    RewardGems = 4m, ObjectiveType = "Talk",       ObjectiveTarget = "Tristan",        ObjectiveLocation = "City Gate",        QuestGiverName = "Fa",                   IsActive = true },
                new Quest { QuestId = 12, Title = "[Chapter 2] Examine the Fallen",           Description = "Beyond the gate the city is silent and the streets are full of the dead. Examine 5 of the bodies and learn what killed them.",                                                        Type = "Main", DefaultStatus = "NotStarted", MapName = "AutumnPumpkin",  RequiredLevel = 3,  TargetAmount = 5,  RewardExperience = 10,  RewardGold = 30m,    RewardGems = 4m, ObjectiveType = "Interact",   ObjectiveTarget = "Corpse",         ObjectiveLocation = "Ruined City",      QuestGiverName = "Tristan",              IsActive = true },
                new Quest { QuestId = 13, Title = "[Chapter 2] Seek the Silver Knight",       Description = "Tristan pales at your report: only one man ever held these ruins. Search the city for the silver knight Arthur and ask for his help.",                                               Type = "Main", DefaultStatus = "NotStarted", MapName = "AutumnPumpkin",  RequiredLevel = 3,  TargetAmount = 1,  RewardExperience = 10,  RewardGold = 30m,    RewardGems = 4m, ObjectiveType = "Talk",       ObjectiveTarget = "Arthur",         ObjectiveLocation = "Ruined City",      QuestGiverName = "Tristan",              IsActive = true },
                new Quest { QuestId = 14, Title = "[Chapter 2] Train in the Old Dungeon",     Description = "Arthur's wounds run deeper than his armour and his power is sealed away; he cannot fight for the city. He can, however, make you strong enough to. Clear his training dungeon.",       Type = "Main", DefaultStatus = "NotStarted", MapName = "AutumnPumpkin",  RequiredLevel = 4,  TargetAmount = 1,  RewardExperience = 10,  RewardGold = 30m,   RewardGems = 4m, ObjectiveType = "Explore",    ObjectiveTarget = "Dungeon_2",      ObjectiveLocation = "Dungeon",          QuestGiverName = "Arthur",               IsActive = true, RewardSkillId = 9, RewardItemId = 18 },
                new Quest { QuestId = 15, Title = "[Chapter 2] Trial I: The Robber Camp",      Description = "Arthur will not send you at a dragon on faith. He sets four trials, and the first is the robbers holding the eastern camp. Cut down 6 of them.",                       Type = "Main", DefaultStatus = "NotStarted", MapName = "AutumnPumpkin",  RequiredLevel = 4,  TargetAmount = 6, RewardExperience = 10,  RewardGold = 30m,   RewardGems = 4m, ObjectiveType = "Defeat",     ObjectiveTarget = "Robber", ObjectiveLocation = "Robber Camp", QuestGiverName = "Arthur", IsActive = true },
                new Quest { QuestId = 16, Title = "[Chapter 2] Trial II: The Haunted Quarter", Description = "One trial stands to your name. The second is the haunted quarter - ghosts, necromancers, and the red guard who died at their posts. Put down 10.",                     Type = "Main", DefaultStatus = "NotStarted", MapName = "AutumnPumpkin",  RequiredLevel = 4,  TargetAmount = 10, RewardExperience = 10,  RewardGold = 30m,   RewardGems = 4m, ObjectiveType = "Defeat",     ObjectiveTarget = "Ghost", ObjectiveLocation = "Haunted Quarter", QuestGiverName = "Arthur", IsActive = true },
                new Quest { QuestId = 17, Title = "[Chapter 2] Trial III: The Goblin Grounds", Description = "Two trials done. The third lies south of the ruins, where goblin spear and axe bands have dug in. Break 3 of them.",                                                   Type = "Main", DefaultStatus = "NotStarted", MapName = "AutumnPumpkin",  RequiredLevel = 4,  TargetAmount = 3, RewardExperience = 10,  RewardGold = 30m,   RewardGems = 4m, ObjectiveType = "Defeat",     ObjectiveTarget = "Goblin", ObjectiveLocation = "Goblin Grounds", QuestGiverName = "Arthur", IsActive = true },
                new Quest { QuestId = 18, Title = "[Chapter 2] Trial IV: The Goblin Warlord",  Description = "The goblins you broke were only a warband, and every warband answers to someone. Their warlord still holds the Goblin Grounds. Kill him and the last trial is yours.", Type = "Main", DefaultStatus = "NotStarted", MapName = "AutumnPumpkin",  RequiredLevel = 4,  TargetAmount = 1, RewardExperience = 35,  RewardGold = 120m,   RewardGems = 16m, ObjectiveType = "Defeat",     ObjectiveTarget = "Goblin Warlord", ObjectiveLocation = "Goblin Grounds", QuestGiverName = "Arthur", IsActive = true, BossMonsterId = 22 },
                new Quest { QuestId = 19, Title = "[Chapter 2] Slay the Dragon",              Description = "Arthur admits you now fight as well as he once did — and tells you what truly broke the city. A dragon nests in the ruins. End it.",                                                 Type = "Main", DefaultStatus = "NotStarted", MapName = "AutumnPumpkin",  RequiredLevel = 5,  TargetAmount = 1,  RewardExperience = 10,  RewardGold = 120m,  RewardGems = 16m, ObjectiveType = "Defeat",     ObjectiveTarget = "Red Dragon", ObjectiveLocation = "Ruined City",      QuestGiverName = "Arthur",               IsActive = true, BossMonsterId = 7 },
                new Quest { QuestId = 20, Title = "[Chapter 2] Arthur's Parting Words",       Description = "Return to Arthur for the knight's thanks and ask where the cursed codex came from. He points north, to a kingdom the codex froze solid.",                                            Type = "Main", DefaultStatus = "NotStarted", MapName = "AutumnPumpkin",  RequiredLevel = 5,  TargetAmount = 1,  RewardExperience = 10,  RewardGold = 80m,   RewardGems = 10m, ObjectiveType = "Talk",       ObjectiveTarget = "Arthur",         ObjectiveLocation = "Ruined City",      QuestGiverName = "Arthur",               IsActive = true },
                // ── MAP 3: Frozen Mountain ───────────────────────────────────────────
                new Quest { QuestId = 21, Title = "[Chapter 3] The Ice Slimes",               Description = "Cedric holds the snow fields with farmers and borrowed spears, and he has no reason to trust a stranger off the ice road. The slimes are on his fields tonight. Defeat 8 of them and he will hear you out.", Type = "Main", DefaultStatus = "NotStarted", MapName = "FrozenMountain", RequiredLevel = 6,  TargetAmount = 8,  RewardExperience = 15,  RewardGold = 40m,   RewardGems = 5m, ObjectiveType = "Defeat",     ObjectiveTarget = "Slime Ice",      ObjectiveLocation = "Snow Fields",      QuestGiverName = "Cedric", IsActive = true },
                new Quest { QuestId = 22, Title = "[Chapter 3] A Word to the Queen",          Description = "The fields are clear, and Cedric has stopped calling you stranger. He says the Queen has been searching for someone with the strength to stand against what is coming, and that he intends to give her your name. Speak with Roselyn Aurora at the citadel.", Type = "Main", DefaultStatus = "NotStarted", MapName = "FrozenMountain", RequiredLevel = 6,  TargetAmount = 1,  RewardExperience = 15,  RewardGold = 40m,   RewardGems = 5m, ObjectiveType = "Talk",       ObjectiveTarget = "Roselyn Aurora Queen", ObjectiveLocation = "Snow Fields",   QuestGiverName = "Cedric", IsActive = true, RewardItemId = 31 },
                new Quest { QuestId = 23, Title = "[Chapter 3] Magic Flour for the Priest",   Description = "The Queen speaks of the ancient king whose statue this kingdom still honours, and of a priest who studies the old magics. Deliver her Magic Flour to Zephyr and ask him what she could not answer.", Type = "Main", DefaultStatus = "NotStarted", MapName = "FrozenMountain", RequiredLevel = 6,  TargetAmount = 1,  RewardExperience = 15,  RewardGold = 40m,   RewardGems = 5m, ObjectiveType = "Talk",       ObjectiveTarget = "Zephyr",         ObjectiveLocation = "Frozen Mountain",  QuestGiverName = "Roselyn Aurora Queen", IsActive = true },
                new Quest { QuestId = 24, Title = "[Chapter 3] Dragons of Snow",              Description = "Zephyr has studied the vanished seal books for thirty years. Something is driving the ice dragons against the people below. Bring down 5 of them on the mountain and report what you saw.",       Type = "Main", DefaultStatus = "NotStarted", MapName = "FrozenMountain", RequiredLevel = 7,  TargetAmount = 5,  RewardExperience = 15,  RewardGold = 40m,   RewardGems = 5m, ObjectiveType = "Defeat",     ObjectiveTarget = "Ice Dragon",     ObjectiveLocation = "Frozen Mountain",  QuestGiverName = "Zephyr",               IsActive = true },
                new Quest { QuestId = 25, Title = "[Chapter 3] The Forbidden Zone",           Description = "Zephyr shares what he suspects: the codex may have been corrupted, not born evil. The rest lies in the sealed north, The Doomed Land of Snow. Find the guard Roland and ask for passage.",         Type = "Main", DefaultStatus = "NotStarted", MapName = "FrozenMountain", RequiredLevel = 7,  TargetAmount = 1,  RewardExperience = 15,  RewardGold = 40m,   RewardGems = 5m, ObjectiveType = "Talk",       ObjectiveTarget = "Roland",         ObjectiveLocation = "Forbidden Zone",   QuestGiverName = "Roland",               IsActive = true },
                new Quest { QuestId = 26, Title = "[Chapter 3] The Sealed Guardians",         Description = "Two ancient things wait inside the ban: a giant of stone, and the spirit that never leaves his side. Defeat them both and take the Golem Seal Book.",                                        Type = "Main", DefaultStatus = "NotStarted", MapName = "FrozenMountain", RequiredLevel = 8,  TargetAmount = 2,  RewardExperience = 15,  RewardGold = 180m,  RewardGems = 24m, ObjectiveType = "Defeat",     ObjectiveTarget = "Golem Boss / Ice Fairy", ObjectiveLocation = "Forbidden Zone", QuestGiverName = "Roland",               IsActive = true, BossMonsterId = 10 },
                new Quest { QuestId = 27, Title = "[Chapter 3] Truth of the Codex",           Description = "Roland is waiting where you left him, and what you carry out of the ban is heavier than a book. Speak with him and put together what was really done to the guardians.",              Type = "Main", DefaultStatus = "NotStarted", MapName = "FrozenMountain", RequiredLevel = 8,  TargetAmount = 1,  RewardExperience = 15,  RewardGold = 105m,   RewardGems = 13m, ObjectiveType = "Talk",       ObjectiveTarget = "Roland",             ObjectiveLocation = "Forbidden Zone", QuestGiverName = "Roland",               IsActive = true },
                // ── MAP 4: Abandoned Castle ──────────────────────────────────────────
                new Quest { QuestId = 28, Title = "[Chapter 4] Break the Skeleton Army", Description = "The Valiant Warrior is Natalie's father, returned from war to find Tide-Knell dead. Help him put down 12 skeletons and hold the valley.", Type = "Main", DefaultStatus = "NotStarted", MapName = "AbandonedCastle", RequiredLevel = 9, TargetAmount = 12, RewardExperience = 15, RewardGold = 50m, RewardGems = 6m, ObjectiveType = "Defeat", ObjectiveTarget = "Skeleton", ObjectiveLocation = "Valley", QuestGiverName = "Valiant Warrior", IsActive = true },
                new Quest { QuestId = 29, Title = "[Chapter 4] Names Beneath the Bone", Description = "Recover 5 remembrance tokens so the Valiant Warrior can name the people he is forced to fight.", Type = "Main", DefaultStatus = "NotStarted", MapName = "AbandonedCastle", RequiredLevel = 9, TargetAmount = 5, RewardExperience = 15, RewardGold = 50m, RewardGems = 6m, ObjectiveType = "Collect", ObjectiveTarget = "Tide-Knell Remembrance", ObjectiveLocation = "Tide-Knell", QuestGiverName = "Valiant Warrior", IsActive = true },
                new Quest { QuestId = 30, Title = "[Chapter 4] The Skull by the Well", Description = "Natalie's ghost asks you to dig beside the old well and recover the skull buried there.", Type = "Main", DefaultStatus = "NotStarted", MapName = "AbandonedCastle", RequiredLevel = 9, TargetAmount = 1, RewardExperience = 15, RewardGold = 50m, RewardGems = 6m, ObjectiveType = "Interact", ObjectiveTarget = "Skull", ObjectiveLocation = "Tide-Knell", QuestGiverName = "Natalie", IsActive = true, RewardItemId = 32 },
                new Quest { QuestId = 31, Title = "[Chapter 4] The Voice Beneath the Well", Description = "Find 3 traces of the old seal around the cursed well and force its promise into the open.", Type = "Main", DefaultStatus = "NotStarted", MapName = "AbandonedCastle", RequiredLevel = 10, TargetAmount = 3, RewardExperience = 15, RewardGold = 50m, RewardGems = 6m, ObjectiveType = "Interact", ObjectiveTarget = "Cursed Well", ObjectiveLocation = "Tide-Knell", QuestGiverName = "Natalie", IsActive = true },
                new Quest { QuestId = 32, Title = "[Chapter 4] The Father's Last Letter", Description = "Find 3 memories left by Natalie's father and let his daughter hear the truth.", Type = "Main", DefaultStatus = "NotStarted", MapName = "AbandonedCastle", RequiredLevel = 10, TargetAmount = 3, RewardExperience = 15, RewardGold = 50m, RewardGems = 6m, ObjectiveType = "Collect", ObjectiveTarget = "Natalie's Memory", ObjectiveLocation = "Tide-Knell", QuestGiverName = "Valiant Warrior", IsActive = true },
                new Quest { QuestId = 33, Title = "[Chapter 4] Lay Natalie to Rest", Description = "Bury Natalie beneath the ivy tree and forgive the lonely child who opened the seal.", Type = "Main", DefaultStatus = "NotStarted", MapName = "AbandonedCastle", RequiredLevel = 10, TargetAmount = 1, RewardExperience = 15, RewardGold = 50m, RewardGems = 6m, ObjectiveType = "Interact", ObjectiveTarget = "Ivy Tree", ObjectiveLocation = "Tide-Knell", QuestGiverName = "Natalie", IsActive = true, RewardItemId = 33 },
                new Quest { QuestId = 34, Title = "[Chapter 4] The Key to the Island", Description = "Use Natalie's Mystic Key at the bridge gate and open the road to the deserted island.", Type = "Main", DefaultStatus = "NotStarted", MapName = "AbandonedCastle", RequiredLevel = 10, TargetAmount = 1, RewardExperience = 15, RewardGold = 50m, RewardGems = 6m, ObjectiveType = "Interact", ObjectiveTarget = "Locked Bridge Gate", ObjectiveLocation = "Bridge", QuestGiverName = "Valiant Warrior", IsActive = true },
                new Quest { QuestId = 35, Title = "[Chapter 4] Ancient Leaves of the Isle", Description = "Gather 5 Ancient Leaves to restore the old rite and open King Aderyn's prison.", Type = "Main", DefaultStatus = "NotStarted", MapName = "AbandonedCastle", RequiredLevel = 10, TargetAmount = 5, RewardExperience = 15, RewardGold = 50m, RewardGems = 6m, ObjectiveType = "Collect", ObjectiveTarget = "Ancient Leaves", ObjectiveLocation = "Northern Plateau", QuestGiverName = "Elf Guard", IsActive = true },
                new Quest { QuestId = 36, Title = "[Chapter 4] The Warden's Oath", Description = "Recover 4 relics from the old sealing party and confront the Elf Guard's guilt.", Type = "Main", DefaultStatus = "NotStarted", MapName = "AbandonedCastle", RequiredLevel = 11, TargetAmount = 4, RewardExperience = 15, RewardGold = 50m, RewardGems = 6m, ObjectiveType = "Collect", ObjectiveTarget = "Warden Relic", ObjectiveLocation = "Deserted Island", QuestGiverName = "Elf Guard", IsActive = true },
                new Quest { QuestId = 37, Title = "[Chapter 4] The King's Garden", Description = "Cleanse 3 cursed roots in King Aderyn's abandoned garden.", Type = "Main", DefaultStatus = "NotStarted", MapName = "AbandonedCastle", RequiredLevel = 11, TargetAmount = 3, RewardExperience = 15, RewardGold = 50m, RewardGems = 6m, ObjectiveType = "Interact", ObjectiveTarget = "Cursed Root", ObjectiveLocation = "Northern Plateau", QuestGiverName = "Brother Cael", IsActive = true },
                new Quest { QuestId = 38, Title = "[Chapter 4] The Man Beneath the Crown", Description = "Read 3 memory fragments and learn why King Aderyn chose imprisonment before entering the crypt.", Type = "Main", DefaultStatus = "NotStarted", MapName = "AbandonedCastle", RequiredLevel = 11, TargetAmount = 3, RewardExperience = 15, RewardGold = 50m, RewardGems = 6m, ObjectiveType = "Interact", ObjectiveTarget = "Aderyn Memory", ObjectiveLocation = "Deserted Island", QuestGiverName = "Brother Cael", IsActive = true },
                new Quest { QuestId = 39, Title = "[Chapter 4] Free the UnderKing", Description = "Defeat the UnderKing and release the hero beneath the crown.", Type = "Main", DefaultStatus = "NotStarted", MapName = "AbandonedCastle", RequiredLevel = 12, TargetAmount = 1, RewardExperience = 15, RewardGold = 240m, RewardGems = 32m, ObjectiveType = "Defeat", ObjectiveTarget = "UnderKing", ObjectiveLocation = "Deserted Island", QuestGiverName = "Elf Guard", IsActive = true, BossMonsterId = 15 },
                new Quest { QuestId = 40, Title = "[Chapter 4] Ask for the Way Home", Description = "Hear the Elf Guard's farewell to his old friend, then open the portal back to the Elf Forest.", Type = "Main", DefaultStatus = "NotStarted", MapName = "AbandonedCastle", RequiredLevel = 12, TargetAmount = 1, RewardExperience = 15, RewardGold = 130m, RewardGems = 16m, ObjectiveType = "Talk", ObjectiveTarget = "Elf Guard", ObjectiveLocation = "Deserted Island", QuestGiverName = "Elf Guard", IsActive = true },
                // ── FINALE: back to the Elf Forest ───────────────────────────────────
                new Quest { QuestId = 41, Title = "[Chapter 5] Return with the Seals", Description = "You are home, and the Origin Tree is worse than you left it. Bring all four Seal Books to Lyra.", Type = "Main", DefaultStatus = "NotStarted", MapName = "ElfForest", RequiredLevel = 12, TargetAmount = 1, RewardExperience = 15, RewardGold = 60m, RewardGems = 7m, ObjectiveType = "Talk", ObjectiveTarget = "Lyra", ObjectiveLocation = "Origin Tree", QuestGiverName = "Lyra", IsActive = true },
                new Quest { QuestId = 42, Title = "[Chapter 5] The Forest Remembers", Description = "Return to Elder Rowan. The forest still remembers the first healing flowers and the people they saved.", Type = "Main", DefaultStatus = "NotStarted", MapName = "ElfForest", RequiredLevel = 12, TargetAmount = 1, RewardExperience = 15, RewardGold = 60m, RewardGems = 7m, ObjectiveType = "Talk", ObjectiveTarget = "Elder Rowan", ObjectiveLocation = "Elf Forest", QuestGiverName = "Lyra", IsActive = true },
                new Quest { QuestId = 43, Title = "[Chapter 5] Flowers Before Dawn", Description = "Gather 3 White Flowers from the old clearing so Elder Rowan can brew the last healing draught.", Type = "Main", DefaultStatus = "NotStarted", MapName = "ElfForest", RequiredLevel = 12, TargetAmount = 3, RewardExperience = 15, RewardGold = 60m, RewardGems = 7m, ObjectiveType = "Collect", ObjectiveTarget = "White Flower", ObjectiveLocation = "Elf Forest", QuestGiverName = "Elder Rowan", IsActive = true },
                new Quest { QuestId = 44, Title = "[Chapter 5] The Last Healing Draught", Description = "Bring the flowers to Elder Rowan, then return to Lyra with the finished draught.", Type = "Main", DefaultStatus = "NotStarted", MapName = "ElfForest", RequiredLevel = 12, TargetAmount = 1, RewardExperience = 15, RewardGold = 60m, RewardGems = 7m, ObjectiveType = "Talk", ObjectiveTarget = "Lyra", ObjectiveLocation = "Origin Tree", QuestGiverName = "Elder Rowan", IsActive = true },
                new Quest { QuestId = 45, Title = "[Chapter 5] Heal the Origin Tree", Description = "Set the four Seal Books and the last healing draught upon the Origin Tree and break the curse.", Type = "Main", DefaultStatus = "NotStarted", MapName = "ElfForest", RequiredLevel = 12, TargetAmount = 1, RewardExperience = 15, RewardGold = 300m, RewardGems = 40m, ObjectiveType = "Interact", ObjectiveTarget = "Origin Tree", ObjectiveLocation = "Origin Tree", QuestGiverName = "Lyra", IsActive = true },
                new Quest { QuestId = 46, Title = "[Chapter 5] A New Dawn", Description = "Speak with Lyra one last time and learn what still waits beyond the healed forest.", Type = "Main", DefaultStatus = "NotStarted", MapName = "ElfForest", RequiredLevel = 12, TargetAmount = 1, RewardExperience = 15, RewardGold = 300m, RewardGems = 40m, ObjectiveType = "Talk", ObjectiveTarget = "Lyra", ObjectiveLocation = "Origin Tree", QuestGiverName = "Lyra", IsActive = true }
            );

            // ─────────────────────────────────────────────────────────────────────────
            // NPC DIALOGUES – Fixed IDs with NPCId and LinkedQuestId referencing
            // the fixed NPC and Quest IDs above.
            // ─────────────────────────────────────────────────────────────────────────
            modelBuilder.Entity<NPCDialogue>().HasData(
                // ── [Chapter 1] A Word with Elder Rowan (QuestId=1, NPCId=1=Elder Rowan) ──
                // Rowan mở đầu bằng LÝ DO người chơi là người được chọn (rừng tự đánh thức,
                // chỉ mình họ nghe được tiếng gọi của Origin Tree) — nếu thiếu, người chơi
                // không có động cơ nào để trở thành nhân vật chính.
                new NPCDialogue { NPCDialogueId = 1, NPCId = 1, LinkedQuestId = 1, ResponseType = "None", Content = "Ah... a new face, and not one born of these woods. Welcome to the Elf Forest, traveler.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 127, NPCId = 1, LinkedQuestId = 1, ResponseType = "None", Content = "You did not wander in here. The forest awakened you at its edge, and the forest does not wake strangers.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 2, NPCId = 1, LinkedQuestId = 1, ResponseType = "None", Content = "For a thousand years this forest kept itself in peace. Now something gathers in the dark beneath the roots.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 94, NPCId = 1, LinkedQuestId = 1, ResponseType = "None", Content = "The Origin Tree at our heart is sickening. Its leaves fall in high summer, and the animals no longer sleep here.", DisplayOrder = 4, IsActive = true },
                new NPCDialogue { NPCDialogueId = 128, NPCId = 1, LinkedQuestId = 1, ResponseType = "None", Content = "A hundred of my people have walked past those roots and heard nothing. You heard it call before you opened your eyes. No one else could.", DisplayOrder = 5, IsActive = true },
                new NPCDialogue { NPCDialogueId = 3, NPCId = 1, LinkedQuestId = 1, ResponseType = "Quest", Content = "So the Origin Tree chose you, and I must trust its choosing. I am Elder Rowan. Speak with me when you are ready to begin.", DisplayOrder = 6, IsActive = true },
                // ── [Chapter 1] Gather White Flowers (QuestId=2, NPCId=1=Elder Rowan) ──
                // Động cơ cụ thể cho quest hái hoa: hoa là NGUYÊN LIỆU CHỮA THƯƠNG và Rowan
                // cần chúng để cứu dân làng đang hấp hối — không còn là việc sai vặt vô cớ.
                new NPCDialogue { NPCDialogueId = 4, NPCId = 1, LinkedQuestId = 2, ResponseType = "None", Content = "Before anything else, I must beg medicine of you. Eleven of my village lie in the healing hall, and my stores are almost gone.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 129, NPCId = 1, LinkedQuestId = 2, ResponseType = "None", Content = "The rot came up through the well water. The children fell first, then whoever carried them. Salve buys them days, no more than that.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 5, NPCId = 1, LinkedQuestId = 2, ResponseType = "None", Content = "By the old willow clearing grows a white flower that only opens where the air is still clean. Crushed with spring water, it is the one salve that answers this sickness.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 95, NPCId = 1, LinkedQuestId = 2, ResponseType = "None", Content = "Where those flowers still bloom, the curse has not yet reached. They are medicine and warning both.", DisplayOrder = 4, IsActive = true },
                new NPCDialogue { NPCDialogueId = 130, NPCId = 1, LinkedQuestId = 2, ResponseType = "None", Content = "My knees will not carry me that far anymore. Bring me the flowers and my people live through the week. That is the whole of it.", DisplayOrder = 5, IsActive = true },
                new NPCDialogue { NPCDialogueId = 6, NPCId = 1, LinkedQuestId = 2, ResponseType = "Quest", Content = "Go to the clearing and gather 3 White Flowers for me. Take care, even slimes wander there now.", DisplayOrder = 6, IsActive = true },
                // ── [Chapter 1] Deliver the White Flowers (QuestId=3, NPCId=1=Elder Rowan) ──
                // Kỹ năng (RewardSkillId=10) giờ là PHẦN THƯỞNG CHO VIỆC ĐÃ CHỨNG TỎ BẢN THÂN:
                // Rowan phá lệ truyền dạy vì người chơi đã cứu dân làng, thay vì "cầm lấy đi".
                new NPCDialogue { NPCDialogueId = 7, NPCId = 1, LinkedQuestId = 3, ResponseType = "None", Content = "Back already? Let me see your hands... ah, you found them.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 8, NPCId = 1, LinkedQuestId = 3, ResponseType = "None", Content = "Not a petal bruised. Three flowers, three doses. The healing hall will have them before nightfall, and the children will sleep.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 131, NPCId = 1, LinkedQuestId = 3, ResponseType = "None", Content = "I sent a stranger into cursed woods for people you had never met, and you went without asking payment. You have proven yourself.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 96, NPCId = 1, LinkedQuestId = 3, ResponseType = "None", Content = "We do not teach our craft outside the bloodline. For you I will break that rule. Let me teach you the First Elven Technique.", DisplayOrder = 4, IsActive = true },
                new NPCDialogue { NPCDialogueId = 9, NPCId = 1, LinkedQuestId = 3, ResponseType = "Reward", Content = "Hold still. Breathe with the roots, as we do... there. It is yours now, with an old elf's thanks.", DisplayOrder = 5, IsActive = true },
                // ── [Chapter 1] Equip Your First Skill (QuestId=4, NPCId=1=Elder Rowan) ──
                // Nối liền với Q3: kỹ năng vừa được DẠY, giờ phải đặt vào tay mới dùng được.
                new NPCDialogue { NPCDialogueId = 10, NPCId = 1, LinkedQuestId = 4, ResponseType = "None", Content = "The technique is in you now, but a technique you have not called upon is no better than one you never learned.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 11, NPCId = 1, LinkedQuestId = 4, ResponseType = "None", Content = "Every warrior in this world channels power through learned technique. Bare fists will not answer what waits out there.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 97, NPCId = 1, LinkedQuestId = 4, ResponseType = "None", Content = "Set it where your hand can reach it without thinking. In a fight you will not have time to remember.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 12, NPCId = 1, LinkedQuestId = 4, ResponseType = "Quest", Content = "Open your Skill Panel and equip the First Elven Technique. Do not step past the treeline without it.", DisplayOrder = 4, IsActive = true },
                // ── [Chapter 1] Cull the Little Slimes (QuestId=5, NPCId=1=Elder Rowan) ──
                new NPCDialogue { NPCDialogueId = 13, NPCId = 1, LinkedQuestId = 5, ResponseType = "None", Content = "Good. I can feel the power settled in you now. It must be tested before it is trusted.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 14, NPCId = 1, LinkedQuestId = 5, ResponseType = "None", Content = "The outskirts crawl with little slimes. They were harmless once, now they hunt in packs.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 98, NPCId = 1, LinkedQuestId = 5, ResponseType = "None", Content = "They are the curse's smallest children. Where they spread, the soil dies behind them.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 15, NPCId = 1, LinkedQuestId = 5, ResponseType = "Quest", Content = "Go out and defeat 3 little slimes, then return and tell me what you felt out there.", DisplayOrder = 4, IsActive = true },
                // ── [Chapter 1] Slay the Swamp Demon (QuestId=6, NPCId=1=Elder Rowan) ──
                // Rowan CHỈ biết hai điều: Swamp Demon đang gây ô nhiễm, và trong đầm lầy có
                // một cổ vật/phong ấn. Ông KHÔNG biết nó tên "Seal Book", KHÔNG biết có 4 quyển,
                // KHÔNG biết chúng là chìa khóa cứu Origin Tree — đó là phần Lyra tiết lộ ở Q7.
                new NPCDialogue { NPCDialogueId = 16, NPCId = 1, LinkedQuestId = 6, ResponseType = "None", Content = "You handled them cleanly. But the slimes are only spillage from something far worse.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 17, NPCId = 1, LinkedQuestId = 6, ResponseType = "None", Content = "Deep in the swamp lies a Demon. The water rots around it, and the corruption creeps closer each night.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 99, NPCId = 1, LinkedQuestId = 6, ResponseType = "None", Content = "Our scouts say the beast broods over some old relic down there. A seal of some kind, they think. I am no scholar, and none of them got close enough to be sure.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 132, NPCId = 1, LinkedQuestId = 6, ResponseType = "None", Content = "What it is, and whether it matters, I cannot tell you. Lyra at the Origin Tree is older than every elf alive. Bring it to her and she will know.", DisplayOrder = 4, IsActive = true },
                new NPCDialogue { NPCDialogueId = 18, NPCId = 1, LinkedQuestId = 6, ResponseType = "Quest", Content = "Destroy the Swamp Demon and take whatever it is guarding. Stopping the rot at its source is what matters.", DisplayOrder = 5, IsActive = true },
                // ── [Chapter 1] Lyra and the Origin Tree (QuestId=7, NPCId=2=Lyra) ──
                // Lyra là người TIẾT LỘ: đây là Seal Book đầu tiên, có 4 quyển, và chúng là
                // chìa khóa cứu Origin Tree. Toàn bộ lore này đã bị lấy khỏi lời Rowan ở Q6.
                new NPCDialogue { NPCDialogueId = 19, NPCId = 2, LinkedQuestId = 7, ResponseType = "None", Content = "Come closer, brave one. I am Lyra, not elf and not ghost. I am the spirit of the Origin Tree itself.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 20, NPCId = 2, LinkedQuestId = 7, ResponseType = "None", Content = "Look at my bark. The curse has reached my heartwood, and I am dying slowly, from the inside outward.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 133, NPCId = 2, LinkedQuestId = 7, ResponseType = "None", Content = "Now show me what you took from the swamp... ah. Rowan sent you here not knowing, did he. Hold it up, child.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 134, NPCId = 2, LinkedQuestId = 7, ResponseType = "None", Content = "This is a Seal Book. The first of them to see daylight in an age, and no living elf has ever held one.", DisplayOrder = 4, IsActive = true },
                new NPCDialogue { NPCDialogueId = 100, NPCId = 2, LinkedQuestId = 7, ResponseType = "None", Content = "Long ago the elders bound an ancient power into four such books. That binding has broken, and the leak is what poisons me.", DisplayOrder = 5, IsActive = true },
                new NPCDialogue { NPCDialogueId = 135, NPCId = 2, LinkedQuestId = 7, ResponseType = "None", Content = "Four books, scattered and guarded. They are not treasure, they are the lock on my heartwood. Nothing else will save me.", DisplayOrder = 6, IsActive = true },
                new NPCDialogue { NPCDialogueId = 21, NPCId = 2, LinkedQuestId = 7, ResponseType = "Quest", Content = "Only the 4 Seal Books can cleanse me. You hold the first already, find the remaining three, and hurry!", DisplayOrder = 7, IsActive = true },
                // ── [Chapter 1] Follow the Cloaked Figure (QuestId=8, NPCId=3=Mysterious Figure) ──
                new NPCDialogue { NPCDialogueId = 22, NPCId = 3, LinkedQuestId = 8, ResponseType = "None", Content = "Heh... so you are the little errand-runner gathering up the Seal Books.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 23, NPCId = 3, LinkedQuestId = 8, ResponseType = "None", Content = "You carry them and do not even know what they are, or whose hand cursed that tree.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 101, NPCId = 3, LinkedQuestId = 8, ResponseType = "None", Content = "The elves told you a story with the ugly parts cut out. I can show you what they buried.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 24, NPCId = 3, LinkedQuestId = 8, ResponseType = "Quest", Content = "The truth waits through this portal. Follow me, or stay and keep watering a dying tree.", DisplayOrder = 4, IsActive = true },
                // ── [Chapter 2] Ask Where You Are (QuestId=9, NPCId=4=Drake) ──
                new NPCDialogue { NPCDialogueId = 25, NPCId = 4, LinkedQuestId = 9, ResponseType = "None", Content = "Steady, traveler. That portal spat us both out here on the beach, and the cloaked one is long gone.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 26, NPCId = 4, LinkedQuestId = 9, ResponseType = "None", Content = "We are far from the forest now, with no coin between us and no way back that I can see.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 102, NPCId = 4, LinkedQuestId = 9, ResponseType = "None", Content = "This is farming country. Folk here trade a day of work for supper, and honest work is easy to find.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 27, NPCId = 4, LinkedQuestId = 9, ResponseType = "Quest", Content = "Go and speak with Fa, the farmer just up the path. He always needs hands.", DisplayOrder = 4, IsActive = true },
                // ── [Chapter 2] Harvest for Your Supper (QuestId=10, NPCId=7=Fa) ──
                new NPCDialogue { NPCDialogueId = 28, NPCId = 7, LinkedQuestId = 10, ResponseType = "None", Content = "Drake sent you? Good timing, stranger. My back is not what it was.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 29, NPCId = 7, LinkedQuestId = 10, ResponseType = "None", Content = "The whole field came ripe at once, and the harvest cart leaves for the city at dusk.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 103, NPCId = 7, LinkedQuestId = 10, ResponseType = "None", Content = "Mind the ones that glow faintly. An enchanted pumpkin keeps a lantern lit all winter, that is why the city pays.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 30, NPCId = 7, LinkedQuestId = 10, ResponseType = "Quest", Content = "Collect 8 Enchanted Pumpkins for me and I will see you fed tonight.", DisplayOrder = 4, IsActive = true },
                // ── [Chapter 2] Deliver the Harvest (QuestId=11, NPCId=7=Fa) ──
                new NPCDialogue { NPCDialogueId = 31, NPCId = 7, LinkedQuestId = 11, ResponseType = "None", Content = "Eight, and not one bruised. You work like a farmhand born, not a wanderer.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 32, NPCId = 7, LinkedQuestId = 11, ResponseType = "None", Content = "Now the hard half of the job. These are owed at the city gate before nightfall.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 104, NPCId = 7, LinkedQuestId = 11, ResponseType = "None", Content = "I would carry them myself, but no one from this farm has come back from that road in a week.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 33, NPCId = 7, LinkedQuestId = 11, ResponseType = "Quest", Content = "Take them to the guard Tristan at the ruined city, and tell him Fa sent you.", DisplayOrder = 4, IsActive = true },
                // ── [Chapter 2] Examine the Fallen (QuestId=12, NPCId=5=Tristan) ──
                new NPCDialogue { NPCDialogueId = 34, NPCId = 5, LinkedQuestId = 12, ResponseType = "None", Content = "Halt! Who goes... ah, pumpkins from Fa. Set them down, you may be the last delivery this gate sees.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 35, NPCId = 5, LinkedQuestId = 12, ResponseType = "None", Content = "Something is wrong inside. No bells, no market noise, no smoke from a single chimney since dawn.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 105, NPCId = 5, LinkedQuestId = 12, ResponseType = "None", Content = "I am Tristan, and my orders bind me to this gate. I cannot take one step past it, even now.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 36, NPCId = 5, LinkedQuestId = 12, ResponseType = "Quest", Content = "Go in and look at the fallen with your own eyes. Then come back and tell me the truth of it.", DisplayOrder = 4, IsActive = true },
                // ── [Chapter 2] Seek the Silver Knight (QuestId=13, NPCId=5=Tristan) ──
                new NPCDialogue { NPCDialogueId = 37, NPCId = 5, LinkedQuestId = 13, ResponseType = "None", Content = "All of them? Every soul in the city, cut down where they stood? Gods, I stood here and heard nothing.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 38, NPCId = 5, LinkedQuestId = 13, ResponseType = "None", Content = "No bandit crew does this in one night. Whatever walked in there was not a man with a sword.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 106, NPCId = 5, LinkedQuestId = 13, ResponseType = "None", Content = "There is one person left who might stand against it. Arthur, the silver knight, camped in the old ruins.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 39, NPCId = 5, LinkedQuestId = 13, ResponseType = "Quest", Content = "Find Arthur and report what you saw. Go, before whatever did this moves on to the next town.", DisplayOrder = 4, IsActive = true },
                // ── [Chapter 2] Train in the Old Dungeon (QuestId=14, NPCId=6=Arthur) ──
                new NPCDialogue { NPCDialogueId = 40, NPCId = 6, LinkedQuestId = 14, ResponseType = "None", Content = "Lower your guard, I am no enemy. I am Arthur, once called the silver knight of this city.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 41, NPCId = 6, LinkedQuestId = 14, ResponseType = "None", Content = "I met the thing that emptied these streets. It broke something inside me and sealed my power away.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 107, NPCId = 6, LinkedQuestId = 14, ResponseType = "None", Content = "I cannot lift my blade again. But a blade is only steel, what matters is the hand that learns to swing it.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 42, NPCId = 6, LinkedQuestId = 14, ResponseType = "Quest", Content = "Clear my old training dungeon. Survive it, and I will give you everything I have left. Go!", DisplayOrder = 4, IsActive = true },
                // -- Q15: Trial I: The Robber Camp (QuestId=15, NPCId=6=Arthur) --
                new NPCDialogue { NPCDialogueId = 136, NPCId = 6, LinkedQuestId = 15, ResponseType = "None", Content = "The streets are quieter. But quiet is not the same as ready, and a dragon is not a ghoul in an alley.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 137, NPCId = 6, LinkedQuestId = 15, ResponseType = "None", Content = "I fought one once believing I was ready. You have seen what is left of me. So you will earn it in four trials.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 138, NPCId = 6, LinkedQuestId = 15, ResponseType = "None", Content = "The robbers took the eastern camp the night the city fell. They prey on whoever still crawls out of here alive.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 139, NPCId = 6, LinkedQuestId = 15, ResponseType = "Quest", Content = "Clear 6 of them from the Robber Camp. That is the first trial. Go.", DisplayOrder = 4, IsActive = true },
                // -- Q16: Trial II: The Haunted Quarter (QuestId=16, NPCId=6=Arthur) --
                new NPCDialogue { NPCDialogueId = 140, NPCId = 6, LinkedQuestId = 16, ResponseType = "None", Content = "One trial down, and you came back on your own feet again. Good.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 141, NPCId = 6, LinkedQuestId = 16, ResponseType = "None", Content = "The second is the haunted quarter - ghosts, necromancers, and the red guard. My own men, still standing their posts, still dead.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 142, NPCId = 6, LinkedQuestId = 16, ResponseType = "None", Content = "I could never walk that street again. A dragon will not care how brave you felt in daylight.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 143, NPCId = 6, LinkedQuestId = 16, ResponseType = "Quest", Content = "Put down 10 in the Haunted Quarter. Second trial. Move.", DisplayOrder = 4, IsActive = true },
                // -- Q17: Trial III: The Goblin Grounds (QuestId=17, NPCId=6=Arthur) --
                new NPCDialogue { NPCDialogueId = 144, NPCId = 6, LinkedQuestId = 17, ResponseType = "None", Content = "Two trials, two returns. I am beginning to believe the city might keep you.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 145, NPCId = 6, LinkedQuestId = 17, ResponseType = "None", Content = "The third is the ground south of the ruins. Goblins hold it, spear and axe together, and they fight as a pack.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 146, NPCId = 6, LinkedQuestId = 17, ResponseType = "None", Content = "A dragon will not come at you alone either. Learn to hold when more than one thing wants you dead.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 147, NPCId = 6, LinkedQuestId = 17, ResponseType = "Quest", Content = "Break 3 of them in the Goblin Grounds. Third trial.", DisplayOrder = 4, IsActive = true },
                // -- Q18: Trial IV: The Goblin Warlord (QuestId=18, NPCId=6=Arthur) --
                new NPCDialogue { NPCDialogueId = 148, NPCId = 6, LinkedQuestId = 18, ResponseType = "None", Content = "Three trials done. You have fought packs, and ghosts, and men. One thing is left that you have not fought.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 149, NPCId = 6, LinkedQuestId = 18, ResponseType = "None", Content = "The warband you broke answers to a warlord, and he is still down there holding what is left of them.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 150, NPCId = 6, LinkedQuestId = 18, ResponseType = "None", Content = "One enemy, bigger than you, who does not retreat and does not tire. That is the shape of a dragon fight. Learn it here where I can still pull you out.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 151, NPCId = 6, LinkedQuestId = 18, ResponseType = "Quest", Content = "Kill the Goblin Warlord. Finish the last trial and I will tell you everything.", DisplayOrder = 4, IsActive = true },
                // ── [Chapter 2] Slay the Dragon (QuestId=19, NPCId=6=Arthur) ──
                new NPCDialogue { NPCDialogueId = 46, NPCId = 6, LinkedQuestId = 19, ResponseType = "None", Content = "You came back quieter than you left. That is how I know the fighting took hold in you.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 47, NPCId = 6, LinkedQuestId = 19, ResponseType = "None", Content = "Then hear the rest of it. The monsters were never the cause. Something older nests above the ruins.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 109, NPCId = 6, LinkedQuestId = 19, ResponseType = "None", Content = "A dragon. It is the thing that broke this city, and the thing that broke me. I have carried that shame for years.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 48, NPCId = 6, LinkedQuestId = 19, ResponseType = "Quest", Content = "Finish what I could not. Climb to its nest and slay the dragon!", DisplayOrder = 4, IsActive = true },
                // ── [Chapter 2] Arthur's Parting Words (QuestId=20, NPCId=6=Arthur) ──
                new NPCDialogue { NPCDialogueId = 49, NPCId = 6, LinkedQuestId = 20, ResponseType = "None", Content = "The dragon is dead. I felt it go — the whole city breathed out at once. Thank you.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 50, NPCId = 6, LinkedQuestId = 20, ResponseType = "None", Content = "You want to know about the cloaked one. Yes. He passed through here before the dragon ever came.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 110, NPCId = 6, LinkedQuestId = 20, ResponseType = "None", Content = "He carries something that should have stayed sealed. Wherever he walks, the land sickens behind him.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 51, NPCId = 6, LinkedQuestId = 20, ResponseType = "Quest", Content = "He went north, into the frozen lands. Follow him to the Frozen Mountains. I will hold this city.", DisplayOrder = 4, IsActive = true },
                // ── [Chapter 3] The Ice Slimes (QuestId=21, NPCId=14=Cedric) ──
                // Mở chương 3 bằng Cedric, KHÔNG phải Nữ hoàng: người lạ vừa lên bờ thì gặp lính giữ
                // ruộng, phải chứng minh mình trước rồi mới được tiến cử vào citadel. Truyền thuyết
                // vùng đất tuyết dời sang thoại của chính Nữ hoàng ở "Magic Flour for the Priest".
                // Dialogue 54/55 vốn là thoại Nữ hoàng cho quest diệt slime — nay đổi chủ sang Cedric
                // (UpdateData, không xoá) vì nhiệm vụ đó giờ là của anh ta.
                new NPCDialogue { NPCDialogueId = 173, NPCId = 14, LinkedQuestId = 21, ResponseType = "None", Content = "Far enough. I am Cedric — captain, if you are being generous. These fields are mine to hold until the Queen can spare someone better.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 174, NPCId = 14, LinkedQuestId = 21, ResponseType = "None", Content = "You came up the ice road, so you saw the state of it. Something crawled out of the snow after the war and it has been eating this valley one field at a time.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 175, NPCId = 14, LinkedQuestId = 21, ResponseType = "None", Content = "The ice slimes come further in every night and freeze whatever they touch. Half my company are farmers holding spears they do not know how to use. I have buried four of them this month.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 54, NPCId = 14, LinkedQuestId = 21, ResponseType = "Quest", Content = "You want me to take you seriously, stranger? They are out on the fields right now. Kill 8 of them. Then we can talk about who you are.", DisplayOrder = 4, IsActive = true },
                new NPCDialogue { NPCDialogueId = 55, NPCId = 14, LinkedQuestId = 21, ResponseType = "Reward", Content = "The fields are quiet. My people walked out to the grain stores without an escort for the first time in a month. Come here — I owe you a word.", DisplayOrder = 5, IsActive = true },
                // ── [Chapter 3] A Word to the Queen (QuestId=22, NPCId=14=Cedric + NPCId=8=Roselyn Aurora Queen) ──
                // Đây là quest "được tiến cử": Cedric thừa nhận đã nhìn sai người, kể ra việc Nữ hoàng
                // đang âm thầm tìm người đủ sức chống lại, rồi tự đứng ra gửi tên người chơi lên.
                // Thoại "quay lại sau khi diệt slime" phải nằm ở quest KẾ TIẾP của cùng NPC, vì engine
                // chỉ hiện thoại của quest đang tới lượt (PickQuestDialogue).
                new NPCDialogue { NPCDialogueId = 176, NPCId = 14, LinkedQuestId = 22, ResponseType = "None", Content = "I had you down as one more sellsword chasing coin up the ice road. It seems you are not. I have watched men with ten years of service do less than you did out there.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 177, NPCId = 14, LinkedQuestId = 22, ResponseType = "None", Content = "So I will tell you what I would not have told you this morning. The Queen has been searching — quietly — for someone with the strength to stand against what is coming for this kingdom.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 178, NPCId = 14, LinkedQuestId = 22, ResponseType = "None", Content = "She has asked every captain on this mountain, and every captain has sent back the same answer: no one. I am tired of writing that answer.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 179, NPCId = 14, LinkedQuestId = 22, ResponseType = "Quest", Content = "Go up to the citadel and stand in front of Roselyn Aurora. I am sending a runner ahead of you — for once with a name in it.", DisplayOrder = 4, IsActive = true },
                new NPCDialogue { NPCDialogueId = 180, NPCId = 8, LinkedQuestId = 22, ResponseType = "Reward", Content = "Cedric's runner reached me an hour before you did. That man does not praise people, so I read it twice. Stay a moment — there are things you should hear from me and not from a soldier.", DisplayOrder = 1, IsActive = true },
                // ── [Chapter 3] Magic Flour for the Priest (QuestId=23, NPCId=8=Roselyn Aurora Queen + NPCId=9=Zephyr) ──
                // Đây mới là chỗ Nữ hoàng kể truyền thuyết vùng đất tuyết: dialogue 52/53/111/152 dời
                // sang từ "The Ice Slimes" (quest đó giờ thuộc Cedric). Thứ tự: thuở bình yên -> codex
                // cướp 4 sách phong ấn + sức mạnh cây khởi nguyên -> chỉ còn tàn dư -> tượng vua
                // Aurelian -> nhờ mang bột cho tu sĩ Zephyr. Dialogue 55 cũ ("The fields are quiet
                // tonight") KHÔNG còn ở đây: câu đó giờ là lời Cedric ở dòng Reward của "The Ice
                // Slimes", vì anh ta mới là người giao việc diệt slime.
                new NPCDialogue { NPCDialogueId = 52, NPCId = 8, LinkedQuestId = 23, ResponseType = "None", Content = "So you are the one Cedric put a name to. I am Roselyn Aurora, and what is left of this kingdom is mine to hold.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 53, NPCId = 8, LinkedQuestId = 23, ResponseType = "None", Content = "It was not always like this. Once these were the quiet lands — snow fell all winter and killed nothing, and no one here carried a sword.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 111, NPCId = 8, LinkedQuestId = 23, ResponseType = "None", Content = "Then the codex came. It took the four Seal Books and drank the strength of the Origin Tree, and everything it passed turned wrong. What you are standing in is only what that war left behind.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 152, NPCId = 8, LinkedQuestId = 23, ResponseType = "None", Content = "Cedric's company is the whole of my army. Farmers holding spears, and a captain who has stopped asking me for reinforcements because he knows there are none.", DisplayOrder = 4, IsActive = true },
                new NPCDialogue { NPCDialogueId = 56, NPCId = 8, LinkedQuestId = 23, ResponseType = "None", Content = "You passed the statue at my gate. King Aurelian — the ancient king this whole kingdom still honours, and the reason any of it is still standing.", DisplayOrder = 5, IsActive = true },
                new NPCDialogue { NPCDialogueId = 112, NPCId = 8, LinkedQuestId = 23, ResponseType = "None", Content = "He spent his life holding back the codex's leavings so that this little peace would outlive him. It did. Barely.", DisplayOrder = 6, IsActive = true },
                new NPCDialogue { NPCDialogueId = 153, NPCId = 8, LinkedQuestId = 23, ResponseType = "None", Content = "I cannot answer what you really came to ask. But there is a priest near here — Zephyr. He studies the old magics, and he has studied them longer than I have been queen.", DisplayOrder = 7, IsActive = true },
                new NPCDialogue { NPCDialogueId = 57, NPCId = 8, LinkedQuestId = 23, ResponseType = "Quest", Content = "Carry the Magic Flour I gave you to him — and ask him everything you have been asking me.", DisplayOrder = 8, IsActive = true },
                new NPCDialogue { NPCDialogueId = 58, NPCId = 9, LinkedQuestId = 23, ResponseType = "Reward", Content = "The Queen's flour, and a courier still breathing. Welcome, hero from far away. I have been on this mountain thirty years chasing one question: how four holy books vanished out of a sealed world.", DisplayOrder = 1, IsActive = true },
                // ── [Chapter 3] Dragons of Snow (QuestId=24, NPCId=9=Zephyr) ──
                new NPCDialogue { NPCDialogueId = 59, NPCId = 9, LinkedQuestId = 24, ResponseType = "None", Content = "Thirty years, and the answer keeps moving. But there is a nearer trouble, and it will not wait for my research.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 89, NPCId = 9, LinkedQuestId = 24, ResponseType = "None", Content = "The ice dragons have stopped behaving like animals. Something is steering them — and they have begun coming down on the people below.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 113, NPCId = 9, LinkedQuestId = 24, ResponseType = "None", Content = "Five of them circle the peak. Young, all of them. Whatever holds their reins made them hungry in a way no beast should be.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 60, NPCId = 9, LinkedQuestId = 24, ResponseType = "Quest", Content = "Climb the peak and put down all 5. May I ask that of a stranger?", DisplayOrder = 4, IsActive = true },
                new NPCDialogue { NPCDialogueId = 154, NPCId = 9, LinkedQuestId = 24, ResponseType = "Reward", Content = "The peak is silent. Sit down, hero — I will tell you the part I have never told the Queen.", DisplayOrder = 5, IsActive = true },
                // ── [Chapter 3] The Forbidden Zone (QuestId=25, NPCId=9=Zephyr + NPCId=10=Roland) ──
                // Thoại của Zephyr ở quest này là đoạn "trở về bàn giao cho tu sĩ": bí mật codex bị
                // tha hoá, rồi chỉ đường tới cấm địa và tới Roland. Mỗi NPC chỉ hiện thoại của chính
                // nó, nên Zephyr kể bí mật còn Roland giữ dòng "Quest" mở đường.
                new NPCDialogue { NPCDialogueId = 155, NPCId = 9, LinkedQuestId = 25, ResponseType = "None", Content = "The codex may not have been evil to begin with. I think it was something else once, and a power turned it.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 156, NPCId = 9, LinkedQuestId = 25, ResponseType = "None", Content = "Dark magic. Strength that gives freely and takes greed as its price. Am I certain? No. I am not certain of any of it.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 157, NPCId = 9, LinkedQuestId = 25, ResponseType = "None", Content = "But look at the beasts here. I studied them before the great war — gentle things, no harm in them. After it, not one of them was itself. Something settled into them, like a taint in the blood.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 158, NPCId = 9, LinkedQuestId = 25, ResponseType = "None", Content = "You want somewhere else to look? There is one place. Dangerous, and cut off from the capital long ago — the forbidden land north of here, The Doomed Land of Snow. Find the guard Roland at the boundary stones. He is a friend of mine.", DisplayOrder = 4, IsActive = true },
                new NPCDialogue { NPCDialogueId = 61, NPCId = 10, LinkedQuestId = 25, ResponseType = "None", Content = "Halt! This ground is under ban and no one goes in. Who are you?", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 62, NPCId = 10, LinkedQuestId = 25, ResponseType = "None", Content = "A knight sent to recover the stolen books... I see. Then you did not climb all this way for the view. You mean to go inside the ban.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 114, NPCId = 10, LinkedQuestId = 25, ResponseType = "None", Content = "Then I will not stand in your way. But hear this: two ancient things still live in there, and both of them are dangerous.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 159, NPCId = 10, LinkedQuestId = 25, ResponseType = "None", Content = "What are they? I do not truly know. The legend says one is a giant made of stone. The other is a mystery — since the old hero sealed this place, no one has dared walk in far enough to find out.", DisplayOrder = 4, IsActive = true },
                new NPCDialogue { NPCDialogueId = 63, NPCId = 10, LinkedQuestId = 25, ResponseType = "Quest", Content = "Go, then. The way is open, and I will keep the road behind you.", DisplayOrder = 5, IsActive = true },
                // ── [Chapter 3] The Sealed Guardians (QuestId=26, NPCId=10=Roland) ──
                // Reward line = phần payoff của item 21: sự thật về GolemBoss và IceFairy.
                new NPCDialogue { NPCDialogueId = 64, NPCId = 10, LinkedQuestId = 26, ResponseType = "None", Content = "So it was here all along. Now I know why my order was told to guard this place and never once to enter it.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 65, NPCId = 10, LinkedQuestId = 26, ResponseType = "None", Content = "And the seal still holds. One of the four old Seal Books lies at the heart of the ban — the Golem Seal Book.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 115, NPCId = 10, LinkedQuestId = 26, ResponseType = "None", Content = "Two guardians stand over it: the stone giant, and the spirit that never leaves his side. The elders left them there to keep every hand off that book, mine included.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 66, NPCId = 10, LinkedQuestId = 26, ResponseType = "Quest", Content = "Put down both of them and take the Golem Seal Book. It is worth more in your hands than under my ban.", DisplayOrder = 4, IsActive = true },
                new NPCDialogue { NPCDialogueId = 160, NPCId = 10, LinkedQuestId = 26, ResponseType = "Reward", Content = "You have the book. But you did not come out of there looking like a man who won a fight — come here and tell me what you saw.", DisplayOrder = 5, IsActive = true },
                // ── [Chapter 3] Truth of the Codex (QuestId=27, NPCId=10=Roland) ──
                // Toàn bộ sự thật về Golem & IceFairy kể ở đây, chia thành nhiều dòng thoại để người
                // chơi bấm đọc từng đoạn (nhồi vào 1 dòng Reward của "The Sealed Guardians" thì
                // thành một khối chữ dài).
                new NPCDialogue { NPCDialogueId = 161, NPCId = 10, LinkedQuestId = 27, ResponseType = "None", Content = "So that is what was sleeping under my ban. Say it again, slowly — I want to be sure I have it right before I write it down.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 162, NPCId = 10, LinkedQuestId = 27, ResponseType = "None", Content = "The golem was gentle once. Not a weapon, not a guardian — he lived close to people and helped anyone who asked him.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 163, NPCId = 10, LinkedQuestId = 27, ResponseType = "None", Content = "And the fairy — he pulled her out of the hands of spirit-traders, men who sold her kind by weight. She never forgot it. She stayed at his side from that day to repay him.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 164, NPCId = 10, LinkedQuestId = 27, ResponseType = "None", Content = "Then the darkness came down on these lands, and he stood against it — for the villages, not for himself. It was stronger. It did not kill him; it put him into a sleep that went on for years.", DisplayOrder = 4, IsActive = true },
                new NPCDialogue { NPCDialogueId = 165, NPCId = 10, LinkedQuestId = 27, ResponseType = "None", Content = "The fairy was terrified. She went to the hero of that age and begged him for help — that is the hero whose statue the Queen still keeps at her gate.", DisplayOrder = 5, IsActive = true },
                new NPCDialogue { NPCDialogueId = 166, NPCId = 10, LinkedQuestId = 27, ResponseType = "None", Content = "Two months after the codex fled this place, the golem finally woke — and it was her doing. She had just learned a blessing, and she spent it on him.", DisplayOrder = 6, IsActive = true },
                new NPCDialogue { NPCDialogueId = 167, NPCId = 10, LinkedQuestId = 27, ResponseType = "None", Content = "But what woke was not what fell asleep. He behaved wrongly. He struck at the people he used to carry water for.", DisplayOrder = 7, IsActive = true },
                new NPCDialogue { NPCDialogueId = 168, NPCId = 10, LinkedQuestId = 27, ResponseType = "None", Content = "And yet he was clear-headed sometimes. Clear enough to understand what was creeping through him — and to decide what to do about it.", DisplayOrder = 8, IsActive = true },
                new NPCDialogue { NPCDialogueId = 169, NPCId = 10, LinkedQuestId = 27, ResponseType = "None", Content = "He walked into the forbidden land himself. Shut himself in. Wiped out every road and marker so no one could follow him in. That is the ban I have been standing guard over for eleven years — a cage a man built for himself.", DisplayOrder = 9, IsActive = true },
                new NPCDialogue { NPCDialogueId = 170, NPCId = 10, LinkedQuestId = 27, ResponseType = "None", Content = "And she went in with him. Knowing what he had become. She never left his side, right up until you.", DisplayOrder = 10, IsActive = true },
                new NPCDialogue { NPCDialogueId = 171, NPCId = 10, LinkedQuestId = 27, ResponseType = "Quest", Content = "Then it is confirmed, and I will carry it to the Queen: no demon, no evil born evil. Someone was corrupted by a dark power — and everything since has followed from that.", DisplayOrder = 11, IsActive = true },
                new NPCDialogue { NPCDialogueId = 172, NPCId = 10, LinkedQuestId = 27, ResponseType = "Reward", Content = "You said you would come back for those two and set them right. Hold to that, hero. I will keep the ban open for the day you do.", DisplayOrder = 12, IsActive = true },
                // ── [Chapter 4] Main chain ──
                new NPCDialogue { NPCDialogueId = 67, NPCId = 11, LinkedQuestId = 28, ResponseType = "None", Content = "Those bones were my neighbours once. I left Tide-Knell for the king's army, and returned to find every soul walking without flesh.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 68, NPCId = 11, LinkedQuestId = 28, ResponseType = "None", Content = "I have guarded this valley for years, cutting down friends who rise again by moonrise. Help me put 12 of them down.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 69, NPCId = 11, LinkedQuestId = 29, ResponseType = "Quest", Content = "Recover five keepsakes from Tide-Knell. Let me remember the people I am forced to fight.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 70, NPCId = 11, LinkedQuestId = 32, ResponseType = "Quest", Content = "Find the memories and my last letter. Natalie deserves to know I came home too late, not that I abandoned her.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 71, NPCId = 12, LinkedQuestId = 30, ResponseType = "None", Content = "My mother died, my father went to war, and Tide-Knell called an orphan bad luck. Then a voice beneath the well called me by name.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 72, NPCId = 12, LinkedQuestId = 30, ResponseType = "Quest", Content = "It promised friends who would never abandon me. I believed it. Please dig beside the well and lift out my skull.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 73, NPCId = 12, LinkedQuestId = 31, ResponseType = "Quest", Content = "The voice still whispers. Find three traces around the well and make it answer for the promise it made.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 74, NPCId = 12, LinkedQuestId = 33, ResponseType = "Quest", Content = "I was lonely, but the choice was mine. If you can still pity me, bury me beneath the ivy tree.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 75, NPCId = 12, LinkedQuestId = 33, ResponseType = "Reward", Content = "If the earth accepts me, Tide-Knell may sleep. Take my Mystic Key and go to the island.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 76, NPCId = 11, LinkedQuestId = 34, ResponseType = "Quest", Content = "Natalie's key opens the bridge. Use it at the gate, then let the Elf Guard finish what I could not.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 77, NPCId = 13, LinkedQuestId = 35, ResponseType = "None", Content = "The prisoner was King Aderyn, my closest friend. He accepted two Seal Books so the forests would not bear the whole curse.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 78, NPCId = 13, LinkedQuestId = 35, ResponseType = "Quest", Content = "Gather five Ancient Leaves. They may open the crypt without destroying what remains of him.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 79, NPCId = 13, LinkedQuestId = 36, ResponseType = "Quest", Content = "Recover four relics from the old sealing party. I have called my guilt duty for centuries.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 80, NPCId = 13, LinkedQuestId = 39, ResponseType = "Quest", Content = "Aderyn was a hero before darkness ate his mind. Defeat the UnderKing and free the man beneath the crown.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 81, NPCId = 13, LinkedQuestId = 40, ResponseType = "Reward", Content = "For one breath, I heard Aderyn thank you. Go home and tell the forest that he is finally free.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 82, NPCId = 15, LinkedQuestId = 37, ResponseType = "Quest", Content = "Cleanse three cursed roots in the king's garden. His last living seed is still below the island.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 83, NPCId = 15, LinkedQuestId = 38, ResponseType = "Quest", Content = "Read three memory fragments. Aderyn chose imprisonment to protect the world; the records must survive him.", DisplayOrder = 1, IsActive = true },
                // ── [Chapter 5] Main chain ──
                new NPCDialogue { NPCDialogueId = 85, NPCId = 2, LinkedQuestId = 41, ResponseType = "Quest", Content = "You came back carrying all four seals. The forest is still breathing, but only just. Bring them to the roots and I will show you what remains.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 91, NPCId = 2, LinkedQuestId = 42, ResponseType = "Quest", Content = "The books cannot heal a memory they do not understand. Return to Elder Rowan; he remembers the first flowers and the lives they saved.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 181, NPCId = 1, LinkedQuestId = 42, ResponseType = "Reward", Content = "You came back to the old roots. The villagers who survived still carry the scent of those first flowers in their homes. There is enough life left for one final draught.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 182, NPCId = 1, LinkedQuestId = 43, ResponseType = "Quest", Content = "Gather 3 White Flowers from the old clearing. This time they will not only keep a few people alive; they will give the Origin Tree something clean to remember.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 183, NPCId = 1, LinkedQuestId = 44, ResponseType = "Reward", Content = "Three flowers, as before. I have brewed the last draught. Take it to Lyra at the Origin Tree; the rest belongs to the one who called you here.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 184, NPCId = 2, LinkedQuestId = 44, ResponseType = "Quest", Content = "I can feel Rowan's draught in your hands. Bring it to the roots, and set the four books where the wound first opened.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 185, NPCId = 2, LinkedQuestId = 45, ResponseType = "Quest", Content = "Set the four Seal Books and the last healing draught upon the Origin Tree. If the forest accepts them, the curse will break.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 186, NPCId = 2, LinkedQuestId = 46, ResponseType = "None", Content = "The forest remembers every hand that carried these seals: Rowan, the silent cities, the frozen guardians, Natalie, and Aderyn. You have given them all another dawn.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 187, NPCId = 2, LinkedQuestId = 46, ResponseType = "Reward", Content = "But the codex was not masterless. Something taught it to drink from the Origin Tree, and that presence is still somewhere beyond these woods.", DisplayOrder = 2, IsActive = true }
            );

            modelBuilder.Entity<CategoryContent>().HasData(
                new CategoryContent
                {
                    CategoryContentId = 1,
                    Name = "Elf Forest",
                    Slug = "elf-forest",
                    Description = "An ancient woodland inhabited by the Elven race, once protected by the Origin Tree before the curse befell the land.",
                    IconUrl = null,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new CategoryContent
                {
                    CategoryContentId = 2,
                    Name = "Seal Books",
                    Slug = "seal-books",
                    Description = "A collection of four ancient elemental seal books containing mysterious powers needed to unlock the realm's secrets.",
                    IconUrl = null,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new CategoryContent
                {
                    CategoryContentId = 3,
                    Name = "The Chronicle",
                    Slug = "the-chronicle",
                    Description = "A journal recording legends, lore, and key storyline events unfolding across the realms.",
                    IconUrl = null,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            modelBuilder.Entity<Content>().HasData(
                new Content
                {
                    ContentId = 1,
                    Title = "Secrets of the Origin Tree in Elf Forest",
                    Slug = "secrets-of-the-origin-tree-in-elf-forest",
                    Summary = "Discover the source of life power for the Elven race and the rising threat of dark forces surrounding the ancient forest.",
                    ThumbnailUrl = null,
                    CategoryContentId = 1,
                    IsPublished = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    PublishedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    CreatedByAccountId = Guid.Empty
                },
                new Content
                {
                    ContentId = 2,
                    Title = "Legend of the Fire Elemental Seal Book",
                    Slug = "legend-of-the-fire-elemental-seal-book",
                    Summary = "Details on the location and decryption of the first Seal Book to unlock Fire Magic skills.",
                    ThumbnailUrl = null,
                    CategoryContentId = 2,
                    IsPublished = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    PublishedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    CreatedByAccountId = Guid.Empty
                },
                new Content
                {
                    ContentId = 3,
                    Title = "Chapter 1: Awakening in the Deep Woods",
                    Slug = "chapter-1-awakening-in-the-deep-woods",
                    Summary = "The beginning of the protagonist's journey — waking up with no memories and the 4 ancient books as the sole clue.",
                    ThumbnailUrl = null,
                    CategoryContentId = 3,
                    IsPublished = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    PublishedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    CreatedByAccountId = Guid.Empty
                },
                new Content
                {
                    ContentId = 4,
                    Title = "Guide to Collecting All 4 Seal Books",
                    Slug = "guide-to-collecting-all-4-seal-books",
                    Summary = "Overview of requirements, minimum levels, and boss encounters required to complete the ancient book collection.",
                    ThumbnailUrl = null,
                    CategoryContentId = 2,
                    IsPublished = false,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    PublishedAt = null,
                    CreatedByAccountId = Guid.Empty
                },
                new Content
                {
                    ContentId = 5,
                    Title = "Ecosystem and Monsters in Elf Forest",
                    Slug = "ecosystem-and-monsters-in-elf-forest",
                    Summary = "A list of mystical creatures and monster stats that players will encounter throughout the Elf Forest region.",
                    ThumbnailUrl = null,
                    CategoryContentId = 1,
                    IsPublished = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    PublishedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    CreatedByAccountId = Guid.Empty
                }
            );

            modelBuilder.Entity<BlockContent>().HasData(
                new BlockContent
                {
                    Id = 1,
                    ContentId = 1,
                    BlockType = "Text",
                    ContentData = "Located at the heart of the Elf Forest, the Origin Tree once provided magical energy to all living beings. However, an ancient curse is causing its leaves to wither away...",
                    MediaUrl = null,
                    Caption = null,
                    SortOrder = 1,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new BlockContent
                {
                    Id = 2,
                    ContentId = 1,
                    BlockType = "Image",
                    ContentData = null,
                    MediaUrl = null,
                    Caption = "origin_tree_pixel.png",
                    SortOrder = 2,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new BlockContent
                {
                    Id = 3,
                    ContentId = 2,
                    BlockType = "Text",
                    ContentData = "The four Seal Books contain remnants of ancient power. The Fire elemental book is currently sealed deep within the abandoned fortress of Autumn Pumpkin...",
                    MediaUrl = null,
                    Caption = null,
                    SortOrder = 1,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new BlockContent
                {
                    Id = 4,
                    ContentId = 3,
                    BlockType = "Text",
                    ContentData = "You awaken in a cursed forest with no memories. Four Seal Books, four realms, and a fading Origin Tree — this is the only path forward.",
                    MediaUrl = null,
                    Caption = null,
                    SortOrder = 1,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new BlockContent
                {
                    Id = 5,
                    ContentId = 3,
                    BlockType = "Image",
                    ContentData = null,
                    MediaUrl = null,
                    Caption = "awakening_scene.png",
                    SortOrder = 2,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new BlockContent
                {
                    Id = 6,
                    ContentId = 4,
                    BlockType = "Text",
                    ContentData = "Each Seal Book corresponds to a realm on the map: Elf Forest (Earth), Frozen Mountain (Ice), Autumn Pumpkin (Fire), and Abandoned Castle (Shadow)...",
                    MediaUrl = null,
                    Caption = null,
                    SortOrder = 1,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new BlockContent
                {
                    Id = 7,
                    ContentId = 5,
                    BlockType = "Text",
                    ContentData = "Although a starter area, Elf Forest hides many dangers from corrupted forest spirits...",
                    MediaUrl = null,
                    Caption = null,
                    SortOrder = 1,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new BlockContent
                {
                    Id = 8,
                    ContentId = 5,
                    BlockType = "Image",
                    ContentData = null,
                    MediaUrl = null,
                    Caption = "monster_list_pixel.png",
                    SortOrder = 2,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            modelBuilder.Entity<Achievement>().HasData(
                new Achievement
                {
                    AchievementId = 1,
                    Name = "Pioneer",
                    Description = "Complete the first chapter",
                    BuffDescription = "+2% Max HP",
                    RequiredValue = 1,
                    Type = "Progression",
                    IconUrl = "pioneer",
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Achievement
                {
                    AchievementId = 2,
                    Name = "Monster Hunter",
                    Description = "Defeat 1,000 monsters",
                    BuffDescription = "+2% Attack",
                    RequiredValue = 1000,
                    Type = "Combat",
                    IconUrl = "monster_hunter",
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Achievement
                {
                    AchievementId = 3,
                    Name = "Deadeye",
                    Description = "Reach the required cumulative Critical Rate",
                    BuffDescription = "+2% Critical Rate",
                    RequiredValue = 100,
                    Type = "Progression",
                    IconUrl = "deadeye",
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Achievement
                {
                    AchievementId = 4,
                    Name = "The Unyielding",
                    Description = "Die fewer than 10 times before Level 30",
                    BuffDescription = "+3% Defense",
                    RequiredValue = 1,
                    Type = "Progression",
                    IconUrl = "unyielding",
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Achievement
                {
                    AchievementId = 5,
                    Name = "Swift Wanderer",
                    Description = "Explore every region on the map",
                    BuffDescription = "+3% Movement Speed",
                    RequiredValue = 1,
                    Type = "Exploration",
                    IconUrl = "swift_wanderer",
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Achievement
                {
                    AchievementId = 6,
                    Name = "Treasure Seeker",
                    Description = "Open 500 treasure chests",
                    BuffDescription = "+5% Gold Gain",
                    RequiredValue = 500,
                    Type = "Collection",
                    IconUrl = "treasure_seeker",
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Achievement
                {
                    AchievementId = 7,
                    Name = "Adventurer",
                    Description = "Complete 100 quests",
                    BuffDescription = "+3% EXP Gain",
                    RequiredValue = 100,
                    Type = "Progression",
                    IconUrl = "adventurer",
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Achievement
                {
                    AchievementId = 8,
                    Name = "Faithful Companion",
                    Description = "Complete 100 co-op dungeons",
                    BuffDescription = "+2% Max HP, +2% Defense",
                    RequiredValue = 100,
                    Type = "Social",
                    IconUrl = "faithful_companion",
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Achievement
                {
                    AchievementId = 9,
                    Name = "Conqueror",
                    Description = "Defeat every Boss at least once",
                    BuffDescription = "+3% Damage to Bosses",
                    RequiredValue = 1,
                    Type = "Combat",
                    IconUrl = "conqueror",
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Achievement
                {
                    AchievementId = 10,
                    Name = "Legend of Elarion",
                    Description = "Reach the maximum level and complete the main storyline",
                    BuffDescription = "+2% to All Stats (HP, ATK, DEF)",
                    RequiredValue = 1,
                    Type = "Progression",
                    IconUrl = "legend_elarion",
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
