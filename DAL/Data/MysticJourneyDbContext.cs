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
        public DbSet<Mail> Mails => Set<Mail>();
        public DbSet<MailRewardItem> MailRewardItems => Set<MailRewardItem>();
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
        public DbSet<GameSetting> GameSettings => Set<GameSetting>();
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
                new ClassConfig { ClassConfigId = 1, ClassName = "Knight", MaxHp = 500, Atk = 50, Def = 40, MoveSpeed = 100, AttackSpeed = 100, CritRate = 5, CritDamage = 150, DamageBonus = 0 },
                new ClassConfig { ClassConfigId = 2, ClassName = "Archer", MaxHp = 350, Atk = 70, Def = 20, MoveSpeed = 100, AttackSpeed = 100, CritRate = 5, CritDamage = 150, DamageBonus = 0 },
                new ClassConfig { ClassConfigId = 3, ClassName = "Mage", MaxHp = 300, Atk = 90, Def = 15, MoveSpeed = 100, AttackSpeed = 100, CritRate = 5, CritDamage = 150, DamageBonus = 0 }
            );

            modelBuilder.Entity<Monster>().HasData(
                new Monster
                {
                    MonsterId = 1,
                    Name = "SlimeLittle",
                    Type = "Normal",
                    Description = "A basic slime monster.",
                    Level = 1,
                    MaxHp = 50,
                    Atk = 5,
                    Def = 2,
                    MoveSpeed = 1,
                    AttackSpeed = 1,
                    CritRate = 10,
                    CritDamage = 187,
                    ExperienceReward = 5,
                    GoldReward = 15m, // Trung bình của 10-20
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Monster
                {
                    MonsterId = 2,
                    Name = "SwampDemon",
                    Type = "Boss",
                    Description = "A dangerous swamp demon.",
                    Level = 10,
                    MaxHp = 500,
                    Atk = 20,
                    Def = 10,
                    MoveSpeed = 1,
                    AttackSpeed = 1,
                    CritRate = 20,
                    CritDamage = 130,
                    ExperienceReward = 100,
                    GoldReward = 200m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Monster
                {
                    MonsterId = 3,
                    Name = "WaterElemental",
                    Type = "Normal",
                    Description = "A water elemental monster.",
                    Level = 5,
                    MaxHp = 80,
                    Atk = 15,
                    Def = 5,
                    MoveSpeed = 1,
                    AttackSpeed = 1,
                    CritRate = 10,
                    CritDamage = 183,
                    ExperienceReward = 10,
                    GoldReward = 30m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Monster
                {
                    MonsterId = 4,
                    Name = "Dragon",
                    Type = "Normal",
                    Description = "A fierce dragon.",
                    Level = 5,
                    MaxHp = 200,
                    Atk = 30,
                    Def = 15,
                    MoveSpeed = 1,
                    AttackSpeed = 1,
                    CritRate = 20,
                    CritDamage = 180,
                    ExperienceReward = 20,
                    GoldReward = 50m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Monster
                {
                    MonsterId = 5,
                    Name = "BlueDragonFrost",
                    Type = "Normal",
                    Description = "A frosty blue dragon.",
                    Level = 6,
                    MaxHp = 250,
                    Atk = 35,
                    Def = 20,
                    MoveSpeed = 5,
                    AttackSpeed = 1,
                    CritRate = 20,
                    CritDamage = 156,
                    ExperienceReward = 22,
                    GoldReward = 55m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Monster
                {
                    MonsterId = 6,
                    Name = "GreenDragonForest",
                    Type = "Normal",
                    Description = "A forest green dragon.",
                    Level = 7,
                    MaxHp = 270,
                    Atk = 37,
                    Def = 25,
                    MoveSpeed = 6,
                    AttackSpeed = 2,
                    CritRate = 12,
                    CritDamage = 160,
                    ExperienceReward = 25,
                    GoldReward = 62m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Monster
                {
                    MonsterId = 7,
                    Name = "DragonBossIdle",
                    Type = "Boss",
                    Description = "A terrifying boss dragon.",
                    Level = 20,
                    MaxHp = 1000,
                    Atk = 50,
                    Def = 35,
                    MoveSpeed = 0,
                    AttackSpeed = 1,
                    CritRate = 30,
                    CritDamage = 250,
                    ExperienceReward = 300,
                    GoldReward = 1000m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Monster
                {
                    MonsterId = 8,
                    Name = "Slime_ice",
                    Type = "Normal",
                    Description = "An icy slime.",
                    Level = 8,
                    MaxHp = 300,
                    Atk = 25,
                    Def = 50,
                    MoveSpeed = 1,
                    AttackSpeed = 1,
                    CritRate = 12,
                    CritDamage = 160,
                    ExperienceReward = 30,
                    GoldReward = 70m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Monster
                {
                    MonsterId = 9,
                    Name = "Ice_Dragon",
                    Type = "Normal",
                    Description = "An icy dragon.",
                    Level = 9,
                    MaxHp = 350,
                    Atk = 50,
                    Def = 70,
                    MoveSpeed = 2,
                    AttackSpeed = 1,
                    CritRate = 25,
                    CritDamage = 170,
                    ExperienceReward = 32,
                    GoldReward = 100m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Monster
                {
                    MonsterId = 10,
                    Name = "GolemBoss",
                    Type = "Boss",
                    Description = "A giant stone golem boss.",
                    Level = 15,
                    MaxHp = 3000,
                    Atk = 150,
                    Def = 70,
                    MoveSpeed = 3,
                    AttackSpeed = 1,
                    CritRate = 30,
                    CritDamage = 150,
                    ExperienceReward = 1500,
                    GoldReward = 2000m,
                    GemReward = 10m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Monster
                {
                    MonsterId = 11,
                    Name = "OrcSkeleton",
                    Type = "Normal",
                    Description = "An undead orc skeleton.",
                    Level = 5,
                    MaxHp = 400,
                    Atk = 50,
                    Def = 100,
                    MoveSpeed = 2,
                    AttackSpeed = 1,
                    CritRate = 25,
                    CritDamage = 166,
                    ExperienceReward = 40,
                    GoldReward = 70m,
                    GemReward = 0m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Monster
                {
                    MonsterId = 12,
                    Name = "SkeletonMelee",
                    Type = "Normal",
                    Description = "A melee skeleton warrior.",
                    Level = 6,
                    MaxHp = 350,
                    Atk = 70,
                    Def = 70,
                    MoveSpeed = 3,
                    AttackSpeed = 2,
                    CritRate = 15,
                    CritDamage = 164,
                    ExperienceReward = 42,
                    GoldReward = 74m,
                    GemReward = 0m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Monster
                {
                    MonsterId = 13,
                    Name = "SkeletonArcher",
                    Type = "Normal",
                    Description = "A ranged skeleton archer.",
                    Level = 6,
                    MaxHp = 250,
                    Atk = 100,
                    Def = 30,
                    MoveSpeed = 3,
                    AttackSpeed = 3,
                    CritRate = 25,
                    CritDamage = 163,
                    ExperienceReward = 38,
                    GoldReward = 78m,
                    GemReward = 0m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Monster
                {
                    MonsterId = 14,
                    Name = "Ghost",
                    Type = "Normal",
                    Description = "A floating ghost.",
                    Level = 5,
                    MaxHp = 300,
                    Atk = 90,
                    Def = 150,
                    MoveSpeed = 4,
                    AttackSpeed = 2,
                    CritRate = 30,
                    CritDamage = 185,
                    ExperienceReward = 45,
                    GoldReward = 85m,
                    GemReward = 0m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Monster
                {
                    MonsterId = 15,
                    Name = "UnderKing",
                    Type = "Boss",
                    Description = "The supreme skeleton king.",
                    Level = 20,
                    MaxHp = 10000,
                    Atk = 200,
                    Def = 300,
                    MoveSpeed = 4,
                    AttackSpeed = 2,
                    CritRate = 40,
                    CritDamage = 20,
                    ExperienceReward = 900,
                    GoldReward = 2500m,
                    GemReward = 500m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Monster
                {
                    MonsterId = 16,
                    Name = "Demon",
                    Type = "Normal",
                    Description = "A terrifying demon.",
                    Level = 8,
                    MaxHp = 500,
                    Atk = 70,
                    Def = 30,
                    MoveSpeed = 2,
                    AttackSpeed = 1,
                    CritRate = 40,
                    CritDamage = 150,
                    ExperienceReward = 50,
                    GoldReward = 100m,
                    GemReward = 0m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Monster
                {
                    MonsterId = 17,
                    Name = "GoblinWarrior",
                    Type = "Normal",
                    Description = "A strong goblin warrior.",
                    Level = 6,
                    MaxHp = 450,
                    Atk = 70,
                    Def = 80,
                    MoveSpeed = 3,
                    AttackSpeed = 1,
                    CritRate = 20,
                    CritDamage = 150,
                    ExperienceReward = 50,
                    GoldReward = 100m,
                    GemReward = 0m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Monster
                {
                    MonsterId = 18,
                    Name = "GoblinSpear",
                    Type = "Normal",
                    Description = "A goblin spearman.",
                    Level = 6,
                    MaxHp = 450,
                    Atk = 50,
                    Def = 40,
                    MoveSpeed = 3,
                    AttackSpeed = 1,
                    CritRate = 20,
                    CritDamage = 150,
                    ExperienceReward = 50,
                    GoldReward = 100m,
                    GemReward = 0m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Monster
                {
                    MonsterId = 19,
                    Name = "Ogre",
                    Type = "Normal",
                    Description = "A fierce ogre.",
                    Level = 6,
                    MaxHp = 450,
                    Atk = 50,
                    Def = 40,
                    MoveSpeed = 3,
                    AttackSpeed = 1,
                    CritRate = 20,
                    CritDamage = 150,
                    ExperienceReward = 50,
                    GoldReward = 100m,
                    GemReward = 0m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Monster
                {
                    MonsterId = 20,
                    Name = "OrcWarlord",
                    Type = "Normal",
                    Description = "A formidable orc warlord.",
                    Level = 7,
                    MaxHp = 600,
                    Atk = 100,
                    Def = 80,
                    MoveSpeed = 3,
                    AttackSpeed = 1,
                    CritRate = 28,
                    CritDamage = 175,
                    ExperienceReward = 55,
                    GoldReward = 100m,
                    GemReward = 0m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
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

            modelBuilder.Entity<EquipmentStats>().HasData(
                new EquipmentStats { EquipmentStatsId = 901, ItemId = 901, BaseAtk = 15, BaseHp = 0, BaseDef = 0, BonusCritRate = 5, BonusCritDamage = 10 },
                new EquipmentStats { EquipmentStatsId = 902, ItemId = 902, BaseAtk = 0, BaseHp = 100, BaseDef = 20, BonusCritRate = 0, BonusCritDamage = 0 },
                new EquipmentStats { EquipmentStatsId = 903, ItemId = 903, BaseAtk = 100, BaseHp = 0, BaseDef = 0, BonusCritRate = 15, BonusCritDamage = 20 },
                new EquipmentStats { EquipmentStatsId = 904, ItemId = 904, BaseAtk = 0, BaseHp = 500, BaseDef = 100, BonusCritRate = 0, BonusCritDamage = 0 },
                new EquipmentStats { EquipmentStatsId = 905, ItemId = 905, BaseAtk = 50, BaseHp = 200, BaseDef = 50, BonusCritRate = 5, BonusCritDamage = 5 },
                new EquipmentStats { EquipmentStatsId = 906, ItemId = 906, BaseAtk = 0, BaseHp = 1000, BaseDef = 200, BonusCritRate = 0, BonusCritDamage = 0 },
                new EquipmentStats { EquipmentStatsId = 907, ItemId = 907, BaseAtk = 200, BaseHp = 0, BaseDef = 0, BonusCritRate = 20, BonusCritDamage = 20 },
                new EquipmentStats { EquipmentStatsId = 908, ItemId = 908, BaseAtk = 50, BaseHp = 1000, BaseDef = 300, BonusCritRate = 10, BonusCritDamage = 10 }
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
                new MonsterDrop { MonsterDropId = 912, MonsterId = 15, ItemId = 912, DropRate = 100, MinQuantity = 1, MaxQuantity = 1, IsGuaranteed = true, IsActive = true }
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

            modelBuilder.Entity<WorldChatMessage>()
                .HasIndex(m => m.SentAt);

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
                new Skill { SkillId = 1, Name = "Accelerationarrow", Description = "Automatically fires in the direction the archer is facing.", Type = "Active", DamageType = "Physical", TargetType = "SingleTarget", ClassRequirement = "Archer", CooldownSeconds = 15, BaseDamage = 0, DamagePerLevel = 0, DamageGrowthPercent = 0, UnlockLevel = 1, CorruptionCost = 0, IsActive = true },
                new Skill { SkillId = 2, Name = "ArrowofLight", Description = "Automatically fires in the direction the archer is facing.", Type = "Active", DamageType = "Physical", TargetType = "SingleTarget", ClassRequirement = "Archer", CooldownSeconds = 20, BaseDamage = 0, DamagePerLevel = 0, DamageGrowthPercent = 0, UnlockLevel = 1, CorruptionCost = 0, IsActive = true },
                new Skill { SkillId = 3, Name = "Holymagic", Description = "Heals allies within range.", Type = "Buff", DamageType = "Magical", TargetType = "Ally", ClassRequirement = "Mage", CooldownSeconds = 10, BaseDamage = 0, DamagePerLevel = 0, DamageGrowthPercent = 0, UnlockLevel = 1, CorruptionCost = 0, IsActive = true },
                new Skill { SkillId = 4, Name = "Purification", Description = "Casts a spell in the direction the character is facing.", Type = "Active", DamageType = "Magical", TargetType = "SingleTarget", ClassRequirement = "Mage", CooldownSeconds = 10, BaseDamage = 0, DamagePerLevel = 0, DamageGrowthPercent = 0, UnlockLevel = 1, CorruptionCost = 0, IsActive = true },
                new Skill { SkillId = 5, Name = "Stardust", Description = "Selects and attacks a random monster within range.", Type = "Active", DamageType = "Magical", TargetType = "SingleTarget", ClassRequirement = "Mage", CooldownSeconds = 10, BaseDamage = 0, DamagePerLevel = 0, DamageGrowthPercent = 0, UnlockLevel = 1, CorruptionCost = 0, IsActive = true },
                new Skill { SkillId = 6, Name = "Lightsabers", Description = "Selects a target with the monster tag to attack.", Type = "Active", DamageType = "Physical", TargetType = "SingleTarget", ClassRequirement = "Knight", CooldownSeconds = 20, BaseDamage = 0, DamagePerLevel = 0, DamageGrowthPercent = 0, UnlockLevel = 1, CorruptionCost = 0, IsActive = true },
                new Skill { SkillId = 7, Name = "LightWaves", Description = "Casts a spell in the direction the character is facing.", Type = "Active", DamageType = "Physical", TargetType = "Area", ClassRequirement = "Knight", CooldownSeconds = 15, BaseDamage = 0, DamagePerLevel = 0, DamageGrowthPercent = 0, UnlockLevel = 1, CorruptionCost = 0, IsActive = true },
                new Skill { SkillId = 8, Name = "ProtectiveShield", Description = "Protects all allies within range.", Type = "Buff", DamageType = "Magical", TargetType = "Ally", ClassRequirement = "Knight", CooldownSeconds = 20, BaseDamage = 0, DamagePerLevel = 0, DamageGrowthPercent = 0, UnlockLevel = 1, CorruptionCost = 0, IsActive = true },
                new Skill { SkillId = 9, Name = "DarkExplosion", Description = "Shared among all classes. Deals damage equal to 3x base damage. Increases corruption points by 15.", Type = "Active", DamageType = "Magical", TargetType = "Area", ClassRequirement = "All", CooldownSeconds = 90, BaseDamage = 0, DamagePerLevel = 0, DamageGrowthPercent = 0, UnlockLevel = 1, CorruptionCost = 15, IsActive = true },
                new Skill { SkillId = 10, Name = "DarkPoisonZone", Description = "Shared among all classes. Deals damage equal to 2x base damage. Increases corruption points by 10.", Type = "Active", DamageType = "Magical", TargetType = "Area", ClassRequirement = "All", CooldownSeconds = 60, BaseDamage = 0, DamagePerLevel = 0, DamageGrowthPercent = 0, UnlockLevel = 1, CorruptionCost = 10, IsActive = true },
                new Skill { SkillId = 11, Name = "DeadlyCurse", Description = "Automatically fires in the direction the archer is facing.", Type = "Active", DamageType = "Physical", TargetType = "SingleTarget", ClassRequirement = "Archer", CooldownSeconds = 20, BaseDamage = 0, DamagePerLevel = 0, DamageGrowthPercent = 0, UnlockLevel = 1, CorruptionCost = 0, IsActive = true },
                new Skill { SkillId = 12, Name = "NightMagic", Description = "Selects an area within range to attack.", Type = "Active", DamageType = "Magical", TargetType = "Area", ClassRequirement = "Mage", CooldownSeconds = 5, BaseDamage = 0, DamagePerLevel = 0, DamageGrowthPercent = 0, UnlockLevel = 1, CorruptionCost = 0, IsActive = true },
                new Skill { SkillId = 13, Name = "DeadlyExplosion", Description = "Shared among all classes. Deals damage equal to 3x base damage. Increases corruption points by 8.", Type = "Active", DamageType = "Magical", TargetType = "SingleTarget", ClassRequirement = "All", CooldownSeconds = 30, BaseDamage = 200, DamagePerLevel = 0, DamageGrowthPercent = 0, UnlockLevel = 1, CorruptionCost = 8, IsActive = true },
                new Skill { SkillId = 14, Name = "BloodySlash", Description = "A short-range slash in the direction the knight is facing.", Type = "Active", DamageType = "Physical", TargetType = "SingleTarget", ClassRequirement = "Knight", CooldownSeconds = 6, BaseDamage = 0, DamagePerLevel = 0, DamageGrowthPercent = 0, UnlockLevel = 1, CorruptionCost = 0, IsActive = true },
                new Skill { SkillId = 15, Name = "FrozenSash", Description = "A short-range slash in the direction the character is facing.", Type = "Active", DamageType = "Physical", TargetType = "SingleTarget", ClassRequirement = "Fighter", CooldownSeconds = 8, BaseDamage = 38, DamagePerLevel = 11, DamageGrowthPercent = 4, UnlockLevel = 1, CorruptionCost = 0, IsActive = true }
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
                new Item { ItemId = 31, Name = "Magic Flour",          Description = "Mystical flour used for special spells.",                                                   Type = "QuestItem",  Rarity = "Common",    Slot = "None",    BaseValue = 0m,    MaxStack = 99,         IsActive = true, CorruptionReduction = 0, CreatedAt = utc2024 },
                new Item { ItemId = 32, Name = "Spirit Skull",         Description = "A skull radiating with ghostly presence.",                                                 Type = "QuestItem",  Rarity = "Common",    Slot = "None",    BaseValue = 0m,    MaxStack = 99,         IsActive = true, CorruptionReduction = 0, CreatedAt = utc2024 },
                new Item { ItemId = 33, Name = "Mystic Key",           Description = "A key that opens the castle on the deserted island.",                                       Type = "QuestItem",  Rarity = "Epic",      Slot = "None",    BaseValue = 0m,    MaxStack = 1,          IsActive = true, CorruptionReduction = 0, CreatedAt = utc2024 }
            );

            // ─────────────────────────────────────────────────────────────────────────
            // EQUIPMENT STATS – for system weapon/armor items (IDs match their ItemId)
            // ─────────────────────────────────────────────────────────────────────────
            modelBuilder.Entity<EquipmentStats>().HasData(
                // Weapons
                new EquipmentStats { EquipmentStatsId = 5,  ItemId = 5,  BaseHp = 0,   BaseAtk = 35,  BaseDef = 0,   BonusHp = 0,   BonusAtk = 8,  BonusDef = 0,  BonusMoveSpeed = 0,  BonusAttackSpeed = 0,  BonusCritRate = 30, BonusCritDamage = 50,  BonusDamageBonus = 0 },
                new EquipmentStats { EquipmentStatsId = 6,  ItemId = 6,  BaseHp = 0,   BaseAtk = 30,  BaseDef = 0,   BonusHp = 0,   BonusAtk = 6,  BonusDef = 0,  BonusMoveSpeed = 0,  BonusAttackSpeed = 10, BonusCritRate = 40, BonusCritDamage = 30,  BonusDamageBonus = 0 },
                new EquipmentStats { EquipmentStatsId = 7,  ItemId = 7,  BaseHp = 0,   BaseAtk = 28,  BaseDef = 0,   BonusHp = 0,   BonusAtk = 5,  BonusDef = 0,  BonusMoveSpeed = 0,  BonusAttackSpeed = 0,  BonusCritRate = 20, BonusCritDamage = 80,  BonusDamageBonus = 10 },
                new EquipmentStats { EquipmentStatsId = 8,  ItemId = 8,  BaseHp = 0,   BaseAtk = 80,  BaseDef = 0,   BonusHp = 0,   BonusAtk = 20, BonusDef = 0,  BonusMoveSpeed = 0,  BonusAttackSpeed = 5,  BonusCritRate = 60, BonusCritDamage = 100, BonusDamageBonus = 15 },
                // Armors
                new EquipmentStats { EquipmentStatsId = 9,  ItemId = 9,  BaseHp = 50,  BaseAtk = 0,   BaseDef = 12,  BonusHp = 10,  BonusAtk = 0,  BonusDef = 3,  BonusMoveSpeed = 0,  BonusAttackSpeed = 0,  BonusCritRate = 0,  BonusCritDamage = 0,   BonusDamageBonus = 0 },
                new EquipmentStats { EquipmentStatsId = 10, ItemId = 10, BaseHp = 100, BaseAtk = 0,   BaseDef = 30,  BonusHp = 20,  BonusAtk = 0,  BonusDef = 8,  BonusMoveSpeed = 0,  BonusAttackSpeed = 0,  BonusCritRate = 0,  BonusCritDamage = 0,   BonusDamageBonus = 0 },
                new EquipmentStats { EquipmentStatsId = 11, ItemId = 11, BaseHp = 0,   BaseAtk = 0,   BaseDef = 5,   BonusHp = 0,   BonusAtk = 0,  BonusDef = 2,  BonusMoveSpeed = 20, BonusAttackSpeed = 0,  BonusCritRate = 0,  BonusCritDamage = 0,   BonusDamageBonus = 0 },
                new EquipmentStats { EquipmentStatsId = 12, ItemId = 12, BaseHp = 500, BaseAtk = 0,   BaseDef = 120, BonusHp = 100, BonusAtk = 0,  BonusDef = 30, BonusMoveSpeed = 0,  BonusAttackSpeed = 0,  BonusCritRate = 0,  BonusCritDamage = 0,   BonusDamageBonus = 0 },
                new EquipmentStats { EquipmentStatsId = 13, ItemId = 13, BaseHp = 0,   BaseAtk = 0,   BaseDef = 60,  BonusHp = 0,   BonusAtk = 0,  BonusDef = 15, BonusMoveSpeed = 15, BonusAttackSpeed = 0,  BonusCritRate = 0,  BonusCritDamage = 0,   BonusDamageBonus = 0 },
                new EquipmentStats { EquipmentStatsId = 14, ItemId = 14, BaseHp = 0,   BaseAtk = 0,   BaseDef = 20,  BonusHp = 0,   BonusAtk = 0,  BonusDef = 5,  BonusMoveSpeed = 0,  BonusAttackSpeed = 0,  BonusCritRate = 80, BonusCritDamage = 120, BonusDamageBonus = 0 },
                new EquipmentStats { EquipmentStatsId = 15, ItemId = 15, BaseHp = 0,   BaseAtk = 20,  BaseDef = 5,   BonusHp = 0,   BonusAtk = 5,  BonusDef = 2,  BonusMoveSpeed = 0,  BonusAttackSpeed = 0,  BonusCritRate = 0,  BonusCritDamage = 0,   BonusDamageBonus = 5 },
                new EquipmentStats { EquipmentStatsId = 16, ItemId = 16, BaseHp = 0,   BaseAtk = 15,  BaseDef = 3,   BonusHp = 0,   BonusAtk = 3,  BonusDef = 1,  BonusMoveSpeed = 5,  BonusAttackSpeed = 5,  BonusCritRate = 0,  BonusCritDamage = 0,   BonusDamageBonus = 0 },
                new EquipmentStats { EquipmentStatsId = 17, ItemId = 17, BaseHp = 30,  BaseAtk = 5,   BaseDef = 3,   BonusHp = 5,   BonusAtk = 2,  BonusDef = 1,  BonusMoveSpeed = 0,  BonusAttackSpeed = 0,  BonusCritRate = 10, BonusCritDamage = 10,  BonusDamageBonus = 0 },
                new EquipmentStats { EquipmentStatsId = 18, ItemId = 18, BaseHp = 80,  BaseAtk = 0,   BaseDef = 5,   BonusHp = 20,  BonusAtk = 0,  BonusDef = 2,  BonusMoveSpeed = 0,  BonusAttackSpeed = 0,  BonusCritRate = 0,  BonusCritDamage = 0,   BonusDamageBonus = 0 }
            );

            // ─────────────────────────────────────────────────────────────────────────
            // NPCs – Fixed IDs so Quest/Dialogue FK references are stable
            // ─────────────────────────────────────────────────────────────────────────
            modelBuilder.Entity<NPC>().HasData(
                new NPC { NPCId = 1,  Name = "Elder Rowan",           Description = "The wise guide of the Elf Forest.",              Type = "QuestGiver", MapName = "ElfForest",      PositionX = 12.4932,    PositionY = 18.61223,    InteractionRadius = 2.5f, IsActive = true },
                new NPC { NPCId = 2,  Name = "Lyra",                  Description = "A spirit of the forest.",                        Type = "QuestGiver", MapName = "ElfForest",      PositionX = 41.94587,   PositionY = -27.18052,   InteractionRadius = 2.5f, IsActive = true },
                new NPC { NPCId = 3,  Name = "Mysterious Figure",     Description = "A mysterious figure in a cloak.",               Type = "QuestGiver", MapName = "ElfForest",      PositionX = 10.11194,   PositionY = -45.86301,   InteractionRadius = 2.5f, IsActive = true },
                new NPC { NPCId = 4,  Name = "Elder Rowan (Pumpkin)", Description = "The wise guide, now in the pumpkin town.",      Type = "QuestGiver", MapName = "AutumnPumpkin",  PositionX = 1.873512,   PositionY = -92.8158,    InteractionRadius = 2.5f, IsActive = true },
                new NPC { NPCId = 5,  Name = "Tristan",               Description = "The city gate guard.",                          Type = "QuestGiver", MapName = "AutumnPumpkin",  PositionX = 11.62283,   PositionY = -113.6158,   InteractionRadius = 2.5f, IsActive = true },
                new NPC { NPCId = 6,  Name = "Arthur",                Description = "The silver knight.",                            Type = "QuestGiver", MapName = "AutumnPumpkin",  PositionX = 77.54412,   PositionY = -77.44301,   InteractionRadius = 2.5f, IsActive = true },
                new NPC { NPCId = 7,  Name = "Fa",                    Description = "A farmer collecting enchanted pumpkins.",       Type = "QuestGiver", MapName = "AutumnPumpkin",  PositionX = 6.08,       PositionY = -161.9,      InteractionRadius = 2.5f, IsActive = true },
                new NPC { NPCId = 8,  Name = "Roselyn Aurora Queen",  Description = "Queen of the frozen lands.",                    Type = "QuestGiver", MapName = "FrozenMountain", PositionX = 160.8554,   PositionY = -35.6486,    InteractionRadius = 2.5f, IsActive = true },
                new NPC { NPCId = 9,  Name = "Zephyr",                Description = "The witch and disguised priest.",               Type = "QuestGiver", MapName = "FrozenMountain", PositionX = 6.996814,   PositionY = -0.2094555,  InteractionRadius = 2.5f, IsActive = true },
                new NPC { NPCId = 10, Name = "Roland",                Description = "The forbidden zone guard.",                     Type = "QuestGiver", MapName = "FrozenMountain", PositionX = 70.45686,   PositionY = 18.80354,    InteractionRadius = 2.5f, IsActive = true },
                new NPC { NPCId = 11, Name = "Valiant Warrior",       Description = "A brave warrior fighting skeletons.",           Type = "QuestGiver", MapName = "AbandonedCastle",PositionX = -10.66112,  PositionY = 54.92884,    InteractionRadius = 2.5f, IsActive = true },
                new NPC { NPCId = 12, Name = "Natalie",               Description = "The ghost of a young girl.",                    Type = "QuestGiver", MapName = "AbandonedCastle",PositionX = -48.92126,  PositionY = -21.12006,   InteractionRadius = 2.5f, IsActive = true },
                new NPC { NPCId = 13, Name = "Elf Guard",             Description = "The lone guard of the deserted island.",        Type = "QuestGiver", MapName = "AbandonedCastle",PositionX = -6.237758,  PositionY = -13.13438,   InteractionRadius = 2.5f, IsActive = true }
            );

            // ─────────────────────────────────────────────────────────────────────────
            // QUESTS – 29 main story quests with fixed IDs.
            // EXP values reflect ALL migrations:
            //   AdjustMainQuestsExpAndGems  → EXP /= 10, RewardGems = 5 for all Main
            //   AdjustMainQuestsExpAndSeedElf3 → Q9=100, Q10=300, Q11=200, Q12=250, Q13=250
            // ─────────────────────────────────────────────────────────────────────────
            modelBuilder.Entity<Quest>().HasData(
                // ── MAP 1: Elf Forest ────────────────────────────────────────────────
                new Quest { QuestId = 1,  Title = "[Chapter 1] Speak with Elder Rowan",     Description = "Talk to Elder Rowan in the Elf Forest.",                                                                                                               Type = "Main", DefaultStatus = "NotStarted", MapName = "ElfForest",      RequiredLevel = 1,  TargetAmount = 1,  RewardExperience = 5,    RewardGold = 10m,   RewardGems = 5m, ObjectiveType = "Talk",      ObjectiveTarget = "Elder Rowan",    ObjectiveLocation = "Elf Forest",       QuestGiverName = "Elder Rowan",          IsActive = true },
                new Quest { QuestId = 2,  Title = "[Chapter 1] Gather White Flowers",        Description = "Collect 3 White Flowers from the forest.",                                                                                                            Type = "Main", DefaultStatus = "NotStarted", MapName = "ElfForest",      RequiredLevel = 1,  TargetAmount = 3,  RewardExperience = 10,   RewardGold = 8m,    RewardGems = 5m, ObjectiveType = "Collect",   ObjectiveTarget = "White Flower",   ObjectiveLocation = "Elf Forest",       QuestGiverName = "Elder Rowan",          IsActive = true },
                new Quest { QuestId = 3,  Title = "[Chapter 1] Deliver White Flowers",       Description = "Deliver the gathered flowers to Elder Rowan.",                                                                                                        Type = "Main", DefaultStatus = "NotStarted", MapName = "ElfForest",      RequiredLevel = 1,  TargetAmount = 1,  RewardExperience = 5,    RewardGold = 5m,    RewardGems = 5m, ObjectiveType = "Talk",      ObjectiveTarget = "Elder Rowan",    ObjectiveLocation = "Elf Forest",       QuestGiverName = "Elder Rowan",          IsActive = true, RewardSkillId = 10 },
                new Quest { QuestId = 4,  Title = "[Chapter 1] Equip Your Skill",            Description = "Equip your first combat skill.",                                                                                                                      Type = "Main", DefaultStatus = "NotStarted", MapName = "ElfForest",      RequiredLevel = 1,  TargetAmount = 1,  RewardExperience = 10,   RewardGold = 10m,   RewardGems = 5m, ObjectiveType = "EquipSkill", ObjectiveTarget = "Skill Panel",   ObjectiveLocation = "Elf Forest",       QuestGiverName = "Elder Rowan",          IsActive = true },
                new Quest { QuestId = 5,  Title = "[Chapter 1] Defeat Slimes",               Description = "Kill 3 SlimeLittle monsters in the forest.",                                                                                                         Type = "Main", DefaultStatus = "NotStarted", MapName = "ElfForest",      RequiredLevel = 1,  TargetAmount = 3,  RewardExperience = 15,   RewardGold = 15m,   RewardGems = 5m, ObjectiveType = "Defeat",    ObjectiveTarget = "SlimeLittle",    ObjectiveLocation = "Elf Forest",       QuestGiverName = "Elder Rowan",          IsActive = true },
                new Quest { QuestId = 6,  Title = "[Chapter 1] The Swamp Demon",             Description = "Slay the Swamp Demon and obtain its Seal Book.",                                                                                                      Type = "Main", DefaultStatus = "NotStarted", MapName = "ElfForest",      RequiredLevel = 2,  TargetAmount = 1,  RewardExperience = 25,   RewardGold = 50m,   RewardGems = 5m, ObjectiveType = "Defeat",    ObjectiveTarget = "SwampDemon",     ObjectiveLocation = "Deep Woods",       QuestGiverName = "Elder Rowan",          IsActive = true, BossMonsterId = 2 },
                new Quest { QuestId = 7,  Title = "[Chapter 1] The Origin Tree",             Description = "Talk to Lyra about the cursed Origin Tree and the 4 Seal Books.",                                                                                     Type = "Main", DefaultStatus = "NotStarted", MapName = "ElfForest",      RequiredLevel = 2,  TargetAmount = 1,  RewardExperience = 10,   RewardGold = 10m,   RewardGems = 5m, ObjectiveType = "Talk",      ObjectiveTarget = "Lyra",           ObjectiveLocation = "Origin Tree",      QuestGiverName = "Lyra",                 IsActive = true },
                new Quest { QuestId = 8,  Title = "[Chapter 1] The Mysterious Figure",       Description = "Follow the cloaked figure through the portal to Autumn Pumpkin.",                                                                                     Type = "Main", DefaultStatus = "NotStarted", MapName = "ElfForest",      RequiredLevel = 2,  TargetAmount = 1,  RewardExperience = 5,    RewardGold = 5m,    RewardGems = 5m, ObjectiveType = "Explore",   ObjectiveTarget = "Portal",         ObjectiveLocation = "Elf Forest",       QuestGiverName = "Mysterious Figure",    IsActive = true },
                // ── MAP 2: Autumn Pumpkin ────────────────────────────────────────────
                // EXP for Q9-Q13 overridden by AdjustMainQuestsExpAndSeedElf3 migration
                new Quest { QuestId = 9,  Title = "[Chapter 2] Where Are We?",               Description = "Teleported onto the beach, proceed to the castle and ask Elder Rowan where this is. After introductions, realize you have no money and ask if there is work to earn food.",  Type = "Main", DefaultStatus = "NotStarted", MapName = "AutumnPumpkin",  RequiredLevel = 3,  TargetAmount = 1,  RewardExperience = 100,  RewardGold = 5m,    RewardGems = 5m, ObjectiveType = "Talk",      ObjectiveTarget = "Elder Rowan",    ObjectiveLocation = "Autumn Pumpkin",   QuestGiverName = "Elder Rowan",          IsActive = true },
                new Quest { QuestId = 10, Title = "[Chapter 2] Work for Food",               Description = "Collect 8 Enchanted Pumpkins from the field and hand them over to farmer Fa.",                                                                        Type = "Main", DefaultStatus = "NotStarted", MapName = "AutumnPumpkin",  RequiredLevel = 3,  TargetAmount = 8,  RewardExperience = 300,  RewardGold = 10m,   RewardGems = 5m, ObjectiveType = "Collect",   ObjectiveTarget = "Enchanted Pumpkin",ObjectiveLocation = "Pumpkin Town",     QuestGiverName = "Fa",                   IsActive = true },
                new Quest { QuestId = 11, Title = "[Chapter 2] Delivery to the City",        Description = "Help Fa deliver the harvested pumpkins to guard Tristan at the ruined city gate.",                                                                    Type = "Main", DefaultStatus = "NotStarted", MapName = "AutumnPumpkin",  RequiredLevel = 3,  TargetAmount = 1,  RewardExperience = 200,  RewardGold = 5m,    RewardGems = 5m, ObjectiveType = "Talk",      ObjectiveTarget = "Tristan",        ObjectiveLocation = "City Gate",        QuestGiverName = "Fa",                   IsActive = true },
                new Quest { QuestId = 12, Title = "[Chapter 2] The Ruined City",             Description = "Enter the city and investigate the dead bodies, then report back to guard Tristan.",                                                                    Type = "Main", DefaultStatus = "NotStarted", MapName = "AutumnPumpkin",  RequiredLevel = 3,  TargetAmount = 5,  RewardExperience = 250,  RewardGold = 5m,    RewardGems = 5m, ObjectiveType = "Interact",  ObjectiveTarget = "Corpse",         ObjectiveLocation = "Ruined City",      QuestGiverName = "Tristan",              IsActive = true },
                new Quest { QuestId = 13, Title = "[Chapter 2] Seek the Silver Knight",      Description = "Report the massacre to Tristan. He asks you to find the silver knight Arthur for help.",                                                               Type = "Main", DefaultStatus = "NotStarted", MapName = "AutumnPumpkin",  RequiredLevel = 3,  TargetAmount = 1,  RewardExperience = 250,  RewardGold = 5m,    RewardGems = 5m, ObjectiveType = "Talk",      ObjectiveTarget = "Arthur",         ObjectiveLocation = "Ruined City",      QuestGiverName = "Tristan",              IsActive = true },
                new Quest { QuestId = 14, Title = "[Chapter 2] The Silver Knight's Training", Description = "Speak with Arthur and learn about his internal injuries and sealed power. Enter Dungeon ID 2 to train and level up your strength.",                      Type = "Main", DefaultStatus = "NotStarted", MapName = "AutumnPumpkin",  RequiredLevel = 12, TargetAmount = 1,  RewardExperience = 15,   RewardGold = 15m,   RewardGems = 5m, ObjectiveType = "Explore",   ObjectiveTarget = "Dungeon_2",      ObjectiveLocation = "Dungeon",          QuestGiverName = "Arthur",               IsActive = true, RewardSkillId = 9, RewardItemId = 18 },
                new Quest { QuestId = 15, Title = "[Chapter 2] Defeat the Evil Monsters",    Description = "Receive the DarkExplosion skill and Silver Necklace from Arthur. Take his place to defeat 10 evil monsters in the Ruined City.",                       Type = "Main", DefaultStatus = "NotStarted", MapName = "AutumnPumpkin",  RequiredLevel = 12, TargetAmount = 10, RewardExperience = 20,   RewardGold = 20m,   RewardGems = 5m, ObjectiveType = "Defeat",    ObjectiveTarget = "Ghost/RobberAssassin/RedGuard/GoblinSpear/GoblinWarrior/RobberArcher/NecromancerCast", ObjectiveLocation = "Ruined City", QuestGiverName = "Arthur", IsActive = true },
                new Quest { QuestId = 16, Title = "[Chapter 2] Slay the Dragon",             Description = "Turn in the quest and get Arthur's recognition of your strength, receive quest to kill DragonBossIdle. Go kill dragon DragonBossIdle.",                 Type = "Main", DefaultStatus = "NotStarted", MapName = "AutumnPumpkin",  RequiredLevel = 5,  TargetAmount = 1,  RewardExperience = 50,   RewardGold = 100m,  RewardGems = 5m, ObjectiveType = "Defeat",    ObjectiveTarget = "DragonBossIdle",  ObjectiveLocation = "Ruined City",      QuestGiverName = "Arthur",               IsActive = true, BossMonsterId = 7 },
                new Quest { QuestId = 17, Title = "[Chapter 2] The Frozen Threat",           Description = "Talk to Arthur and receive the knight's thanks, ask about the whereabouts of the ??? and he directs you to the frozen land devastated by the codex, go to Frozen Mountains.", Type = "Main", DefaultStatus = "NotStarted", MapName = "AutumnPumpkin", RequiredLevel = 5, TargetAmount = 1, RewardExperience = 10, RewardGold = 10m, RewardGems = 5m, ObjectiveType = "Talk", ObjectiveTarget = "Arthur", ObjectiveLocation = "Ruined City", QuestGiverName = "Arthur", IsActive = true },
                // ── MAP 3: Frozen Mountain ───────────────────────────────────────────
                new Quest { QuestId = 18, Title = "[Chapter 3] The Ice Slimes",              Description = "Meet Queen Roselyn Aurora and defeat 8 Ice Slimes.",                                                                                                    Type = "Main", DefaultStatus = "NotStarted", MapName = "FrozenMountain", RequiredLevel = 6,  TargetAmount = 8,  RewardExperience = 30,   RewardGold = 30m,   RewardGems = 5m, ObjectiveType = "Defeat",    ObjectiveTarget = "slime_ice",      ObjectiveLocation = "Snow Fields",      QuestGiverName = "Roselyn Aurora Queen", IsActive = true, RewardItemId = 31 },
                new Quest { QuestId = 19, Title = "[Chapter 3] Magic Flour for the Priest",  Description = "Deliver Magic Flour (obtained from the Queen) to the Priest (Zephyr).",                                                                              Type = "Main", DefaultStatus = "NotStarted", MapName = "FrozenMountain", RequiredLevel = 6,  TargetAmount = 1,  RewardExperience = 15,   RewardGold = 15m,   RewardGems = 5m, ObjectiveType = "Talk",      ObjectiveTarget = "Zephyr",         ObjectiveLocation = "Frozen Mountain",  QuestGiverName = "Roselyn Aurora Queen", IsActive = true },
                new Quest { QuestId = 20, Title = "[Chapter 3] Dragons of Snow",             Description = "Meet Zephyr and slay 5 Ice Dragons on the mountain.",                                                                                                  Type = "Main", DefaultStatus = "NotStarted", MapName = "FrozenMountain", RequiredLevel = 7,  TargetAmount = 5,  RewardExperience = 40,   RewardGold = 40m,   RewardGems = 5m, ObjectiveType = "Defeat",    ObjectiveTarget = "Ice_Dragon",     ObjectiveLocation = "Frozen Mountain",  QuestGiverName = "Zephyr",               IsActive = true },
                new Quest { QuestId = 21, Title = "[Chapter 3] The Forbidden Zone",          Description = "Head to the forbidden zone and speak with Roland to explore it.",                                                                                       Type = "Main", DefaultStatus = "NotStarted", MapName = "FrozenMountain", RequiredLevel = 7,  TargetAmount = 1,  RewardExperience = 15,   RewardGold = 15m,   RewardGems = 5m, ObjectiveType = "Explore",   ObjectiveTarget = "Roland",         ObjectiveLocation = "Forbidden Zone",   QuestGiverName = "Roland",               IsActive = true },
                new Quest { QuestId = 22, Title = "[Chapter 3] Truth of the Codex",          Description = "Discover the truth of the codex and defeat GolemBoss to get the Golem Seal Book.",                                                                     Type = "Main", DefaultStatus = "NotStarted", MapName = "FrozenMountain", RequiredLevel = 8,  TargetAmount = 1,  RewardExperience = 80,   RewardGold = 150m,  RewardGems = 5m, ObjectiveType = "Defeat",    ObjectiveTarget = "GolemBoss",      ObjectiveLocation = "Forbidden Zone",   QuestGiverName = "Roland",               IsActive = true, BossMonsterId = 10 },
                // ── MAP 4: Abandoned Castle ──────────────────────────────────────────
                new Quest { QuestId = 23, Title = "[Chapter 4] Skeleton Army",               Description = "Defeat 12 skeletons in the valley for Valiant Warrior.",                                                                                               Type = "Main", DefaultStatus = "NotStarted", MapName = "AbandonedCastle",RequiredLevel = 9,  TargetAmount = 12, RewardExperience = 50,   RewardGold = 50m,   RewardGems = 5m, ObjectiveType = "Defeat",    ObjectiveTarget = "Skeleton",       ObjectiveLocation = "Valley",           QuestGiverName = "Valiant Warrior",      IsActive = true },
                new Quest { QuestId = 24, Title = "[Chapter 4] The Abandoned Village",       Description = "Go to Tide-Knell village, meet Natalie, and dig up the skull near the old well.",                                                                       Type = "Main", DefaultStatus = "NotStarted", MapName = "AbandonedCastle",RequiredLevel = 9,  TargetAmount = 1,  RewardExperience = 30,   RewardGold = 30m,   RewardGems = 5m, ObjectiveType = "Interact",  ObjectiveTarget = "Skull",          ObjectiveLocation = "Tide-Knell",       QuestGiverName = "Valiant Warrior",      IsActive = true },
                new Quest { QuestId = 25, Title = "[Chapter 4] Rest in Peace",               Description = "Read Natalie's suicide letter and bury her remains under the ivy tree. Receive Mystic Key.",                                                            Type = "Main", DefaultStatus = "NotStarted", MapName = "AbandonedCastle",RequiredLevel = 10, TargetAmount = 1,  RewardExperience = 40,   RewardGold = 40m,   RewardGems = 5m, ObjectiveType = "Talk",      ObjectiveTarget = "Natalie",        ObjectiveLocation = "Tide-Knell",       QuestGiverName = "Natalie",              IsActive = true, RewardItemId = 33 },
                new Quest { QuestId = 26, Title = "[Chapter 4] Deserted Island",             Description = "Talk to Elf Guard on the deserted island and collect 5 Ancient Leaves.",                                                                                Type = "Main", DefaultStatus = "NotStarted", MapName = "AbandonedCastle",RequiredLevel = 10, TargetAmount = 5,  RewardExperience = 45,   RewardGold = 45m,   RewardGems = 5m, ObjectiveType = "Collect",   ObjectiveTarget = "Ancient Leaves", ObjectiveLocation = "Northern Plateau", QuestGiverName = "Elf Guard",            IsActive = true },
                new Quest { QuestId = 27, Title = "[Chapter 4] The UnderKing",               Description = "Defeat the UnderKing to claim the final UnderKing Seal Book.",                                                                                         Type = "Main", DefaultStatus = "NotStarted", MapName = "AbandonedCastle",RequiredLevel = 11, TargetAmount = 1,  RewardExperience = 200,  RewardGold = 300m,  RewardGems = 5m, ObjectiveType = "Defeat",    ObjectiveTarget = "UnderKing",      ObjectiveLocation = "Deserted Island",  QuestGiverName = "Elf Guard",            IsActive = true, BossMonsterId = 15 },
                new Quest { QuestId = 28, Title = "[Chapter 4] Return to Elf Forest",        Description = "Talk to Elf Guard. He will open a portal back to the Elf Forest.",                                                                                     Type = "Main", DefaultStatus = "NotStarted", MapName = "AbandonedCastle",RequiredLevel = 12, TargetAmount = 1,  RewardExperience = 10,   RewardGold = 10m,   RewardGems = 5m, ObjectiveType = "Talk",      ObjectiveTarget = "Elf Guard",      ObjectiveLocation = "Deserted Island",  QuestGiverName = "Elf Guard",            IsActive = true },
                // ── Back to MAP 1: Final Quest ───────────────────────────────────────
                new Quest { QuestId = 29, Title = "[Chapter 1] Save the Origin Tree",        Description = "Talk to Lyra and use the 4 Seal Books to cleanse the tree. \"To be continued\".",                                                                     Type = "Main", DefaultStatus = "NotStarted", MapName = "ElfForest",      RequiredLevel = 12, TargetAmount = 1,  RewardExperience = 500,  RewardGold = 500m,  RewardGems = 5m, ObjectiveType = "Talk",      ObjectiveTarget = "Lyra",           ObjectiveLocation = "Origin Tree",      QuestGiverName = "Lyra",                 IsActive = true }
            );

            // ─────────────────────────────────────────────────────────────────────────
            // NPC DIALOGUES – Fixed IDs with NPCId and LinkedQuestId referencing
            // the fixed NPC and Quest IDs above.
            // ─────────────────────────────────────────────────────────────────────────
            modelBuilder.Entity<NPCDialogue>().HasData(
                // ── Q1: Speak with Elder Rowan (QuestId=1, NPCId=1) ──
                new NPCDialogue { NPCDialogueId = 1,  NPCId = 1, LinkedQuestId = 1,  ResponseType = "None",   Content = "Ah, a new traveler. Welcome to the Elf Forest.",                                                                   DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 2,  NPCId = 1, LinkedQuestId = 1,  ResponseType = "None",   Content = "This forest has been peaceful for centuries, but recently, dark forces have begun to gather.",                       DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 3,  NPCId = 1, LinkedQuestId = 1,  ResponseType = "Quest",  Content = "I need your help to protect this place. Come speak to me when you are ready to begin.",                            DisplayOrder = 3, IsActive = true },
                // ── Q2: Gather White Flowers (QuestId=2, NPCId=1) ──
                new NPCDialogue { NPCDialogueId = 4,  NPCId = 1, LinkedQuestId = 2,  ResponseType = "None",   Content = "Before we can confront the darkness, we need to prepare some basic remedies.",                                     DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 5,  NPCId = 1, LinkedQuestId = 2,  ResponseType = "None",   Content = "The old willow clearing has some magical herbs we can use.",                                                         DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 6,  NPCId = 1, LinkedQuestId = 2,  ResponseType = "Quest",  Content = "Please head over there and gather 3 White Flowers for me.",                                                          DisplayOrder = 3, IsActive = true },
                // ── Q3: Deliver White Flowers (QuestId=3, NPCId=1) ──
                new NPCDialogue { NPCDialogueId = 7,  NPCId = 1, LinkedQuestId = 3,  ResponseType = "None",   Content = "You have returned quickly. Did you find the flowers?",                                                               DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 8,  NPCId = 1, LinkedQuestId = 3,  ResponseType = "None",   Content = "Excellent, these are in perfect condition. They will make fine healing poultices.",                                  DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 9,  NPCId = 1, LinkedQuestId = 3,  ResponseType = "Reward", Content = "Thank you! Take this as a token of my appreciation.",                                                               DisplayOrder = 3, IsActive = true },
                // ── Q4: Equip Your Skill (QuestId=4, NPCId=1) ──
                new NPCDialogue { NPCDialogueId = 10, NPCId = 1, LinkedQuestId = 4,  ResponseType = "None",   Content = "Now that you have your reward, it is time to learn how to defend yourself.",                                        DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 11, NPCId = 1, LinkedQuestId = 4,  ResponseType = "None",   Content = "In this world, skills are essential for survival. You cannot fight with bare hands alone.",                         DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 12, NPCId = 1, LinkedQuestId = 4,  ResponseType = "Quest",  Content = "Open your Skill Panel and equip your first combat skill before you face real danger.",                              DisplayOrder = 3, IsActive = true },
                // ── Q5: Defeat Slimes (QuestId=5, NPCId=1) ──
                new NPCDialogue { NPCDialogueId = 13, NPCId = 1, LinkedQuestId = 5,  ResponseType = "None",   Content = "Good, you are armed and ready. It is time to test your newfound abilities.",                                         DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 14, NPCId = 1, LinkedQuestId = 5,  ResponseType = "None",   Content = "The outskirts of our forest have been overrun by strange, aggressive slimes.",                                      DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 15, NPCId = 1, LinkedQuestId = 5,  ResponseType = "Quest",  Content = "Head out and defeat 3 SlimeLittle monsters to prove your worth to the village.",                                   DisplayOrder = 3, IsActive = true },
                // ── Q6: The Swamp Demon (QuestId=6, NPCId=1) ──
                new NPCDialogue { NPCDialogueId = 16, NPCId = 1, LinkedQuestId = 6,  ResponseType = "None",   Content = "You handled those slimes well. But a much greater threat lurks in the deep woods.",                                  DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 17, NPCId = 1, LinkedQuestId = 6,  ResponseType = "None",   Content = "A terrible Swamp Demon has made its lair there, corrupting the land with its presence.",                            DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 18, NPCId = 1, LinkedQuestId = 6,  ResponseType = "Quest",  Content = "You must destroy the Swamp Demon and claim the Swamp Seal Book it guards. We are counting on you!",                 DisplayOrder = 3, IsActive = true },
                // ── Q7: The Origin Tree (QuestId=7, NPCId=2=Lyra) ──
                new NPCDialogue { NPCDialogueId = 19, NPCId = 2, LinkedQuestId = 7,  ResponseType = "None",   Content = "Greetings, brave warrior. I am Lyra, the spirit of the Origin Tree.",                                               DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 20, NPCId = 2, LinkedQuestId = 7,  ResponseType = "None",   Content = "As you can see, the tree has been cursed and is slowly dying.",                                                      DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 21, NPCId = 2, LinkedQuestId = 7,  ResponseType = "Quest",  Content = "Only the 4 Seal Books can cleanse it. You have one, but you must find the remaining three!",                        DisplayOrder = 3, IsActive = true },
                // ── Q8: The Mysterious Figure (QuestId=8, NPCId=3) ──
                new NPCDialogue { NPCDialogueId = 22, NPCId = 3, LinkedQuestId = 8,  ResponseType = "None",   Content = "Heh... So you are the one collecting the Seal Books?",                                                              DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 23, NPCId = 3, LinkedQuestId = 8,  ResponseType = "None",   Content = "You know nothing of the true history of this world, or why the tree was cursed.",                                   DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 24, NPCId = 3, LinkedQuestId = 8,  ResponseType = "Quest",  Content = "If you want the truth, follow me through this portal. Don't keep me waiting.",                                     DisplayOrder = 3, IsActive = true },
                // ── Q9: Where Are We? (QuestId=9, NPCId=4=Elder Rowan Pumpkin) ──
                new NPCDialogue { NPCDialogueId = 25, NPCId = 4, LinkedQuestId = 9,  ResponseType = "None",   Content = "Welcome to the beach. We were teleported here by the portal.",                                                      DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 26, NPCId = 4, LinkedQuestId = 9,  ResponseType = "None",   Content = "You seem to have no money for food. Why don't you look for some work?",                                              DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 27, NPCId = 4, LinkedQuestId = 9,  ResponseType = "Quest",  Content = "Go talk to Fa, he is nearby and might need some help.",                                                             DisplayOrder = 3, IsActive = true },
                // ── Q10: Work for Food (QuestId=10, NPCId=7=Fa) ──
                new NPCDialogue { NPCDialogueId = 28, NPCId = 7, LinkedQuestId = 10, ResponseType = "None",   Content = "Ah, Elder Rowan sent you? Good timing.",                                                                           DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 29, NPCId = 7, LinkedQuestId = 10, ResponseType = "None",   Content = "I need someone to help me harvest the fields.",                                                                      DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 30, NPCId = 7, LinkedQuestId = 10, ResponseType = "Quest",  Content = "Please collect 8 Enchanted Pumpkins for me.",                                                                       DisplayOrder = 3, IsActive = true },
                // ── Q11: Delivery to the City (QuestId=11, NPCId=7=Fa) ──
                new NPCDialogue { NPCDialogueId = 31, NPCId = 7, LinkedQuestId = 11, ResponseType = "None",   Content = "Great job with the pumpkins! You are a hard worker.",                                                               DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 32, NPCId = 7, LinkedQuestId = 11, ResponseType = "None",   Content = "Now, I need these delivered to the city gate.",                                                                     DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 33, NPCId = 7, LinkedQuestId = 11, ResponseType = "Quest",  Content = "Please take them to guard Tristan at the ruined city.",                                                             DisplayOrder = 3, IsActive = true },
                // ── Q12: The Ruined City (QuestId=12, NPCId=5=Tristan) ──
                new NPCDialogue { NPCDialogueId = 34, NPCId = 5, LinkedQuestId = 12, ResponseType = "None",   Content = "Halt! Who goes there? Ah, you brought pumpkins from Fa?",                                                          DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 35, NPCId = 5, LinkedQuestId = 12, ResponseType = "None",   Content = "Something is wrong in the city... It is too quiet.",                                                               DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 36, NPCId = 5, LinkedQuestId = 12, ResponseType = "Quest",  Content = "Please go inside and investigate. Let me know if you find anything suspicious.",                                    DisplayOrder = 3, IsActive = true },
                // ── Q13: Seek the Silver Knight (QuestId=13, NPCId=5=Tristan) ──
                new NPCDialogue { NPCDialogueId = 37, NPCId = 5, LinkedQuestId = 13, ResponseType = "None",   Content = "What?! The people inside have all been massacred? Corpses everywhere?",                                            DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 38, NPCId = 5, LinkedQuestId = 13, ResponseType = "None",   Content = "This is a disaster. We need someone strong to handle this.",                                                        DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 39, NPCId = 5, LinkedQuestId = 13, ResponseType = "Quest",  Content = "Please, go find the silver knight Arthur and report this!",                                                         DisplayOrder = 3, IsActive = true },
                // ── Q14: Silver Knight's Training (QuestId=14, NPCId=6=Arthur) ──
                new NPCDialogue { NPCDialogueId = 40, NPCId = 6, LinkedQuestId = 14, ResponseType = "None",   Content = "Greetings, warrior. I am Arthur, the silver knight.",                                                               DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 41, NPCId = 6, LinkedQuestId = 14, ResponseType = "None",   Content = "I suffered severe internal injuries and my power has been sealed away.",                                             DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 42, NPCId = 6, LinkedQuestId = 14, ResponseType = "Quest",  Content = "You must train in Dungeon 2 to level up and unlock your true potential. Go!",                                      DisplayOrder = 3, IsActive = true },
                // ── Q15: Defeat the Evil Monsters (QuestId=15, NPCId=6=Arthur) ──
                new NPCDialogue { NPCDialogueId = 43, NPCId = 6, LinkedQuestId = 15, ResponseType = "None",   Content = "Splendid! You have trained well and cleared the dungeon.",                                                         DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 44, NPCId = 6, LinkedQuestId = 15, ResponseType = "None",   Content = "As promised, take this DarkExplosion skill and Silver Necklace.",                                                  DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 45, NPCId = 6, LinkedQuestId = 15, ResponseType = "Quest",  Content = "Now, use your power to defeat 10 evil monsters in the Ruined City!",                                               DisplayOrder = 3, IsActive = true },
                // ── Q16: Slay the Dragon (QuestId=16, NPCId=6=Arthur) ──
                new NPCDialogue { NPCDialogueId = 46, NPCId = 6, LinkedQuestId = 16, ResponseType = "None",   Content = "You have returned, and you are stronger than before.",                                                              DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 47, NPCId = 6, LinkedQuestId = 16, ResponseType = "None",   Content = "I recognize your true strength now. You are ready for the ultimate challenge.",                                    DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 48, NPCId = 6, LinkedQuestId = 16, ResponseType = "Quest",  Content = "A terrible dragon threatens our existence. Go and slay the DragonBossIdle!",                                       DisplayOrder = 3, IsActive = true },
                // ── Q17: The Frozen Threat (QuestId=17, NPCId=6=Arthur) ──
                new NPCDialogue { NPCDialogueId = 49, NPCId = 6, LinkedQuestId = 17, ResponseType = "None",   Content = "You did it! The dragon is slain. I cannot thank you enough.",                                                      DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 50, NPCId = 6, LinkedQuestId = 17, ResponseType = "None",   Content = "You ask about the mysterious figure? The one who did this?",                                                        DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 51, NPCId = 6, LinkedQuestId = 17, ResponseType = "Quest",  Content = "He went towards the frozen lands devastated by the codex. Head to the Frozen Mountains next.",                     DisplayOrder = 3, IsActive = true },
                // ── Q18: The Ice Slimes (QuestId=18, NPCId=8=Roselyn Aurora Queen) ──
                new NPCDialogue { NPCDialogueId = 52, NPCId = 8, LinkedQuestId = 18, ResponseType = "None",   Content = "Ah, a survivor from the ruins. I am Queen Roselyn Aurora.",                                                        DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 53, NPCId = 8, LinkedQuestId = 18, ResponseType = "None",   Content = "This land is devastated by the codex. Only volunteers remain to defend it.",                                         DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 54, NPCId = 8, LinkedQuestId = 18, ResponseType = "Quest",  Content = "Please, clear out 8 ice slimes from the Snow Fields to help us.",                                                  DisplayOrder = 3, IsActive = true },
                // ── Q19: Magic Flour for the Priest (QuestId=19, NPCId=8=Queen + NPCId=9=Zephyr) ──
                new NPCDialogue { NPCDialogueId = 55, NPCId = 8, LinkedQuestId = 19, ResponseType = "None",   Content = "Your efforts have not gone unnoticed. The slimes are thinning out.",                                               DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 56, NPCId = 8, LinkedQuestId = 19, ResponseType = "None",   Content = "However, our Priest Zephyr requires supplies for a ritual.",                                                        DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 57, NPCId = 8, LinkedQuestId = 19, ResponseType = "Quest",  Content = "Take this Magic Flour and deliver it to him at the mountain peak.",                                               DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 58, NPCId = 9, LinkedQuestId = 19, ResponseType = "None",   Content = "Ah, the flour from the Queen! Thank you, traveler.",                                                               DisplayOrder = 1, IsActive = true },
                // ── Q20: Dragons of Snow (QuestId=20, NPCId=9=Zephyr) ──
                new NPCDialogue { NPCDialogueId = 59, NPCId = 9, LinkedQuestId = 20, ResponseType = "None",   Content = "The codex has warped the creatures here. The beasts have become feral and dangerous.",                             DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 60, NPCId = 9, LinkedQuestId = 20, ResponseType = "Quest",  Content = "To secure our borders, go slay 5 Ice Dragons on the mountain.",                                                   DisplayOrder = 2, IsActive = true },
                // ── Q21: The Forbidden Zone (QuestId=21, NPCId=10=Roland) ──
                new NPCDialogue { NPCDialogueId = 61, NPCId = 10, LinkedQuestId = 21, ResponseType = "None",  Content = "Halt! This is the forbidden zone. None may enter.",                                                               DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 62, NPCId = 10, LinkedQuestId = 21, ResponseType = "None",  Content = "Wait... you have the aura of one who has fought the Ice Dragons.",                                               DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 63, NPCId = 10, LinkedQuestId = 21, ResponseType = "Quest", Content = "Since you made it this far, help me explore this dangerous area.",                                               DisplayOrder = 3, IsActive = true },
                // ── Q22: Truth of the Codex (QuestId=22, NPCId=10=Roland) ──
                new NPCDialogue { NPCDialogueId = 64, NPCId = 10, LinkedQuestId = 22, ResponseType = "None",  Content = "We have uncovered the origin of the codex... The truth is terrifying.",                                          DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 65, NPCId = 10, LinkedQuestId = 22, ResponseType = "None",  Content = "A massive ancient golem guards the final piece of the puzzle.",                                                  DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 66, NPCId = 10, LinkedQuestId = 22, ResponseType = "Quest", Content = "Defeat the giant GolemBoss to claim the Golem Seal Book! Do not fail us.",                                      DisplayOrder = 3, IsActive = true },
                // ── Q23: Skeleton Army (QuestId=23, NPCId=11=Valiant Warrior) ──
                new NPCDialogue { NPCDialogueId = 67, NPCId = 11, LinkedQuestId = 23, ResponseType = "None",  Content = "Stay back! The undead are relentless today.",                                                                     DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 68, NPCId = 11, LinkedQuestId = 23, ResponseType = "None",  Content = "An ancient power is leaking, causing skeletons to multiply out of control.",                                    DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 69, NPCId = 11, LinkedQuestId = 23, ResponseType = "Quest", Content = "I can't hold them off alone. Defeat 12 of them in the valley!",                                                 DisplayOrder = 3, IsActive = true },
                // ── Q24: The Abandoned Village (QuestId=24, NPCId=11=Valiant + NPCId=12=Natalie) ──
                new NPCDialogue { NPCDialogueId = 70, NPCId = 11, LinkedQuestId = 24, ResponseType = "Quest", Content = "The animals are fleeing from the abandoned village Tide-Knell. Investigate it and find Natalie.",              DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 71, NPCId = 12, LinkedQuestId = 24, ResponseType = "None",  Content = "Are you here to help me? I cannot leave this place...",                                                          DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 72, NPCId = 12, LinkedQuestId = 24, ResponseType = "Quest", Content = "Please... dig up what is buried under the small tree near the old well.",                                      DisplayOrder = 3, IsActive = true },
                // ── Q25: Rest in Peace (QuestId=25, NPCId=12=Natalie) ──
                new NPCDialogue { NPCDialogueId = 73, NPCId = 12, LinkedQuestId = 25, ResponseType = "None",  Content = "Thank you for finding my remains. Now I can finally rest in peace.",                                            DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 74, NPCId = 12, LinkedQuestId = 25, ResponseType = "None",  Content = "The ancient power leak was my doing. I am so sorry for the chaos.",                                             DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 75, NPCId = 12, LinkedQuestId = 25, ResponseType = "Reward",Content = "Take this key. It will unlock the doors to the island castle. Farewell.",                                       DisplayOrder = 3, IsActive = true },
                // ── Q26: Deserted Island (QuestId=26, NPCId=13=Elf Guard) ──
                new NPCDialogue { NPCDialogueId = 76, NPCId = 13, LinkedQuestId = 26, ResponseType = "None",  Content = "You actually survived the waves and made it to this deserted island.",                                          DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 77, NPCId = 13, LinkedQuestId = 26, ResponseType = "None",  Content = "I need your assistance to prepare a ritual of return.",                                                           DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 78, NPCId = 13, LinkedQuestId = 26, ResponseType = "Quest", Content = "Help me collect 5 Ancient Leaves from the Northern Plateau.",                                                   DisplayOrder = 3, IsActive = true },
                // ── Q27: The UnderKing (QuestId=27, NPCId=13=Elf Guard) ──
                new NPCDialogue { NPCDialogueId = 79, NPCId = 13, LinkedQuestId = 27, ResponseType = "None",  Content = "We have everything we need. But a dark presence blocks our path.",                                              DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 80, NPCId = 13, LinkedQuestId = 27, ResponseType = "None",  Content = "The UnderKing himself has awakened, and he guards the final Seal Book.",                                        DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 81, NPCId = 13, LinkedQuestId = 27, ResponseType = "Quest", Content = "You must end his reign! Defeat the UnderKing and claim the book!",                                             DisplayOrder = 3, IsActive = true },
                // ── Q28: Return to Elf Forest (QuestId=28, NPCId=13=Elf Guard) ──
                new NPCDialogue { NPCDialogueId = 82, NPCId = 13, LinkedQuestId = 28, ResponseType = "None",  Content = "It is done. The UnderKing is defeated, and you have all 4 Seal Books.",                                         DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 83, NPCId = 13, LinkedQuestId = 28, ResponseType = "None",  Content = "The fate of the Origin Tree now rests entirely in your hands.",                                                DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 84, NPCId = 13, LinkedQuestId = 28, ResponseType = "Reward",Content = "Farewell, hero. I will use my power to open a portal back to the Elf Forest. Save the tree!",                DisplayOrder = 3, IsActive = true },
                // ── Q29: Save the Origin Tree (QuestId=29, NPCId=2=Lyra) ──
                new NPCDialogue { NPCDialogueId = 85, NPCId = 2, LinkedQuestId = 29, ResponseType = "None",   Content = "You have returned! And I can sense the power of the 4 Seal Books.",                                              DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 86, NPCId = 2, LinkedQuestId = 29, ResponseType = "None",   Content = "The curse is breaking... The Origin Tree is finally healing!",                                                  DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 87, NPCId = 2, LinkedQuestId = 29, ResponseType = "Reward", Content = "Thank you! The Origin Tree is saved. But this is not the end... To be continued.",                            DisplayOrder = 3, IsActive = true }
            );
        }
    }
}
