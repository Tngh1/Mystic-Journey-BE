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
                    Name = "SlimeIce",
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
                    Name = "IceDragon",
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
                },
                new Monster
                {
                    MonsterId = 21,
                    Name = "IceFairy",
                    Type = "Boss",
                    Description = "A support boss fairy in Frozen Mountain.",
                    Level = 10,
                    MaxHp = 2500,
                    Atk = 40,
                    Def = 30,
                    MoveSpeed = 4,
                    AttackSpeed = 1,
                    CritRate = 10,
                    CritDamage = 150,
                    ExperienceReward = 200,
                    GoldReward = 500m,
                    GemReward = 50m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Monster
                {
                    MonsterId = 22,
                    Name = "GoblinWarlord",
                    Type = "Boss",
                    Description = "A fierce goblin warlord boss.",
                    Level = 12,
                    MaxHp = 2000,
                    Atk = 60,
                    Def = 40,
                    MoveSpeed = 3,
                    AttackSpeed = 1,
                    CritRate = 20,
                    CritDamage = 150,
                    ExperienceReward = 300,
                    GoldReward = 800m,
                    GemReward = 80m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Monster
                {
                    MonsterId = 23,
                    Name = "NecromancerCast",
                    Type = "Normal",
                    Description = "A dark necromancer casting dark spells.",
                    Level = 5,
                    MaxHp = 520,
                    Atk = 45,
                    Def = 20,
                    MoveSpeed = 2,
                    AttackSpeed = 1,
                    CritRate = 15,
                    CritDamage = 150,
                    ExperienceReward = 40,
                    GoldReward = 80m,
                    GemReward = 0m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Monster
                {
                    MonsterId = 24,
                    Name = "RobberArcher",
                    Type = "Normal",
                    Description = "A rogue robber archer wielding a crossbow.",
                    Level = 5,
                    MaxHp = 500,
                    Atk = 50,
                    Def = 25,
                    MoveSpeed = 3,
                    AttackSpeed = 1,
                    CritRate = 20,
                    CritDamage = 160,
                    ExperienceReward = 40,
                    GoldReward = 75m,
                    GemReward = 0m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Monster
                {
                    MonsterId = 25,
                    Name = "RobberAssassin",
                    Type = "Normal",
                    Description = "A stealthy robber assassin wielding a sword and shield.",
                    Level = 6,
                    MaxHp = 550,
                    Atk = 55,
                    Def = 35,
                    MoveSpeed = 3,
                    AttackSpeed = 1,
                    CritRate = 25,
                    CritDamage = 170,
                    ExperienceReward = 45,
                    GoldReward = 90m,
                    GemReward = 0m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Monster
                {
                    MonsterId = 26,
                    Name = "RedGuard",
                    Type = "Normal",
                    Description = "A heavy red guard soldier carrying a mace and shield.",
                    Level = 6,
                    MaxHp = 600,
                    Atk = 60,
                    Def = 50,
                    MoveSpeed = 2,
                    AttackSpeed = 1,
                    CritRate = 15,
                    CritDamage = 150,
                    ExperienceReward = 50,
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
                new Skill { SkillId = 1, Name = "Accelerationarrow", Description = "Automatically fires in the direction the archer is facing.", Type = "Active", DamageType = "Physical", TargetType = "SingleTarget", ClassRequirement = "Archer", CooldownSeconds = 2, BaseDamage = 0, DamagePerLevel = 0, DamageGrowthPercent = 0, UnlockLevel = 1, CorruptionCost = 0, IsActive = true },
                new Skill { SkillId = 2, Name = "ArrowofLight", Description = "Automatically fires in the direction the archer is facing.", Type = "Active", DamageType = "Physical", TargetType = "SingleTarget", ClassRequirement = "Archer", CooldownSeconds = 5, BaseDamage = 0, DamagePerLevel = 0, DamageGrowthPercent = 0, UnlockLevel = 1, CorruptionCost = 0, IsActive = true },
                new Skill { SkillId = 3, Name = "Holymagic", Description = "Heals allies within range.", Type = "Buff", DamageType = "Magical", TargetType = "Ally", ClassRequirement = "Mage", CooldownSeconds = 4, BaseDamage = 0, DamagePerLevel = 0, DamageGrowthPercent = 0, UnlockLevel = 1, CorruptionCost = 0, IsActive = true },
                new Skill { SkillId = 4, Name = "Purification", Description = "Casts a spell in the direction the character is facing.", Type = "Active", DamageType = "Magical", TargetType = "SingleTarget", ClassRequirement = "Mage", CooldownSeconds = 3, BaseDamage = 0, DamagePerLevel = 0, DamageGrowthPercent = 0, UnlockLevel = 1, CorruptionCost = 0, IsActive = true },
                new Skill { SkillId = 5, Name = "Stardust", Description = "Selects and attacks a random monster within range.", Type = "Active", DamageType = "Magical", TargetType = "SingleTarget", ClassRequirement = "Mage", CooldownSeconds = 3, BaseDamage = 0, DamagePerLevel = 0, DamageGrowthPercent = 0, UnlockLevel = 1, CorruptionCost = 0, IsActive = true },
                new Skill { SkillId = 6, Name = "Lightsabers", Description = "Selects a target with the monster tag to attack.", Type = "Active", DamageType = "Physical", TargetType = "SingleTarget", ClassRequirement = "Knight", CooldownSeconds = 5, BaseDamage = 0, DamagePerLevel = 0, DamageGrowthPercent = 0, UnlockLevel = 1, CorruptionCost = 0, IsActive = true },
                new Skill { SkillId = 7, Name = "LightWaves", Description = "Casts a spell in the direction the character is facing.", Type = "Active", DamageType = "Physical", TargetType = "Area", ClassRequirement = "Knight", CooldownSeconds = 4, BaseDamage = 0, DamagePerLevel = 0, DamageGrowthPercent = 0, UnlockLevel = 1, CorruptionCost = 0, IsActive = true },
                new Skill { SkillId = 8, Name = "ProtectiveShield", Description = "Protects all allies within range.", Type = "Buff", DamageType = "Magical", TargetType = "Ally", ClassRequirement = "Knight", CooldownSeconds = 8, BaseDamage = 0, DamagePerLevel = 0, DamageGrowthPercent = 0, UnlockLevel = 1, CorruptionCost = 0, IsActive = true },
                new Skill { SkillId = 9, Name = "DarkExplosion", Description = "Shared among all classes. Deals damage equal to 3x base damage. Increases corruption points by 15.", Type = "Active", DamageType = "Magical", TargetType = "Area", ClassRequirement = "All", CooldownSeconds = 8, BaseDamage = 0, DamagePerLevel = 0, DamageGrowthPercent = 0, UnlockLevel = 1, CorruptionCost = 15, IsActive = true },
                new Skill { SkillId = 10, Name = "DarkPoisonZone", Description = "Shared among all classes. Deals damage equal to 2x base damage. Increases corruption points by 10.", Type = "Active", DamageType = "Magical", TargetType = "Area", ClassRequirement = "All", CooldownSeconds = 6, BaseDamage = 0, DamagePerLevel = 0, DamageGrowthPercent = 0, UnlockLevel = 1, CorruptionCost = 10, IsActive = true },
                new Skill { SkillId = 11, Name = "DeadlyCurse", Description = "Automatically fires in the direction the archer is facing.", Type = "Active", DamageType = "Physical", TargetType = "SingleTarget", ClassRequirement = "Archer", CooldownSeconds = 5, BaseDamage = 0, DamagePerLevel = 0, DamageGrowthPercent = 0, UnlockLevel = 1, CorruptionCost = 0, IsActive = true },
                new Skill { SkillId = 12, Name = "NightMagic", Description = "Selects an area within range to attack.", Type = "Active", DamageType = "Magical", TargetType = "Area", ClassRequirement = "Mage", CooldownSeconds = 2, BaseDamage = 0, DamagePerLevel = 0, DamageGrowthPercent = 0, UnlockLevel = 1, CorruptionCost = 0, IsActive = true },
                new Skill { SkillId = 13, Name = "DeadlyExplosion", Description = "Shared among all classes. Deals damage equal to 3x base damage. Increases corruption points by 8.", Type = "Active", DamageType = "Magical", TargetType = "SingleTarget", ClassRequirement = "All", CooldownSeconds = 6, BaseDamage = 200, DamagePerLevel = 0, DamageGrowthPercent = 0, UnlockLevel = 1, CorruptionCost = 8, IsActive = true },
                new Skill { SkillId = 14, Name = "BloodySlash", Description = "A short-range slash in the direction the knight is facing.", Type = "Active", DamageType = "Physical", TargetType = "SingleTarget", ClassRequirement = "Knight", CooldownSeconds = 2, BaseDamage = 0, DamagePerLevel = 0, DamageGrowthPercent = 0, UnlockLevel = 1, CorruptionCost = 0, IsActive = true },
                new Skill { SkillId = 15, Name = "FrozenSash", Description = "Selects an area within range to unleash an icy slash.", Type = "Active", DamageType = "Physical", TargetType = "Area", ClassRequirement = "Knight", CooldownSeconds = 3, BaseDamage = 38, DamagePerLevel = 11, DamageGrowthPercent = 4, UnlockLevel = 1, CorruptionCost = 0, IsActive = true },
                new Skill { SkillId = 16, Name = "PumpkinMagic", Description = "Summons a magical pumpkin trap that lasts 5 seconds. Explodes when touched by monsters or when duration expires, dealing AoE physical damage.", Type = "Active", DamageType = "Physical", TargetType = "Area", ClassRequirement = "Archer", CooldownSeconds = 5, BaseDamage = 50, DamagePerLevel = 12, DamageGrowthPercent = 4, UnlockLevel = 1, CorruptionCost = 0, IsActive = true },
                new Skill { SkillId = 17, Name = "PumpkinThrow", Description = "Throws an explosive pumpkin in a parabolic arc. Explodes on impact with any object, dealing AoE physical damage to monsters.", Type = "Active", DamageType = "Physical", TargetType = "Area", ClassRequirement = "Knight", CooldownSeconds = 5, BaseDamage = 45, DamagePerLevel = 10, DamageGrowthPercent = 4, UnlockLevel = 1, CorruptionCost = 0, IsActive = true },
                new Skill { SkillId = 18, Name = "PumpkinSlash", Description = "A short-range pumpkin slash in the direction the knight is facing.", Type = "Active", DamageType = "Physical", TargetType = "SingleTarget", ClassRequirement = "Knight", CooldownSeconds = 2, BaseDamage = 40, DamagePerLevel = 9, DamageGrowthPercent = 3, UnlockLevel = 1, CorruptionCost = 0, IsActive = true },
                new Skill { SkillId = 19, Name = "BoomBoomPumpkin", Description = "Summons a magic pumpkin that explodes immediately at the target location, dealing light magical AoE damage with a short cooldown.", Type = "Active", DamageType = "Magical", TargetType = "Area", ClassRequirement = "Mage", CooldownSeconds = 2, BaseDamage = 30, DamagePerLevel = 8, DamageGrowthPercent = 3, UnlockLevel = 1, CorruptionCost = 0, IsActive = true }
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
            //
            // PositionX/Y are LOCAL to the scene's "NPC" container object, not world
            // coordinates: WorldNpcSpawnerRuntime assigns them to transform.localPosition
            // under npcContainer. Those containers are not at the origin, so a world
            // coordinate pasted in here gets the container offset added on top and the NPC
            // lands far off-map (looks like "the NPC never spawned").
            // Container offsets: ElfForest (0, 0) · AutumnPumpkin (-108.53592, 135.28775)
            //                    FrozenMountain (-8.12395, 26.35298) · AbandonedCastle (18.19, 5.05)
            // To convert a position read off the Unity inspector: subtract the offset.
            // ─────────────────────────────────────────────────────────────────────────
            modelBuilder.Entity<NPC>().HasData(
                new NPC { NPCId = 1,  Name = "Elder Rowan",           Description = "The wise guide of the Elf Forest.",              Type = "QuestGiver", MapName = "ElfForest",      PositionX = -0.7,    PositionY = 18.5,    InteractionRadius = 2.5f, IsActive = true },
                new NPC { NPCId = 2,  Name = "Lyra",                  Description = "A spirit of the forest.",                        Type = "QuestGiver", MapName = "ElfForest",      PositionX = 30,   PositionY = -6,   InteractionRadius = 2.5f, IsActive = true },
                new NPC { NPCId = 3,  Name = "Mysterious Figure",     Description = "A mysterious figure in a cloak.",               Type = "QuestGiver", MapName = "ElfForest",      PositionX = 14,   PositionY = -47.5,   InteractionRadius = 2.5f, IsActive = true },
                new NPC { NPCId = 4,  Name = "Drake",                 Description = "A weathered guide in the pumpkin town.",         Type = "QuestGiver", MapName = "AutumnPumpkin",  PositionX = -105.5,   PositionY = 40.7,   InteractionRadius = 2.5f, IsActive = true },
                new NPC { NPCId = 5,  Name = "Tristan",               Description = "The city gate guard.",                          Type = "QuestGiver", MapName = "AutumnPumpkin",  PositionX = -97.3 ,  PositionY = 21.4,    InteractionRadius = 2.5f, IsActive = true },
                new NPC { NPCId = 6,  Name = "Arthur",                Description = "The silver knight.",                            Type = "QuestGiver", MapName = "AutumnPumpkin",  PositionX = -32,  PositionY = 58,    InteractionRadius = 2.5f, IsActive = true },
                new NPC { NPCId = 7,  Name = "Fa",                    Description = "A farmer collecting enchanted pumpkins.",       Type = "QuestGiver", MapName = "AutumnPumpkin",  PositionX = -101,  PositionY = -26,   InteractionRadius = 2.5f, IsActive = true },
                new NPC { NPCId = 8,  Name = "Roselyn Aurora Queen",  Description = "Queen of the frozen lands.",                    Type = "QuestGiver", MapName = "FrozenMountain", PositionX = 160.8554,   PositionY = -35.6486,    InteractionRadius = 2.5f, IsActive = true },
                new NPC { NPCId = 9,  Name = "Zephyr",                Description = "The witch and disguised priest.",               Type = "QuestGiver", MapName = "FrozenMountain", PositionX = 6.996814,   PositionY = -0.2094555,  InteractionRadius = 2.5f, IsActive = true },
                new NPC { NPCId = 10, Name = "Roland",                Description = "The forbidden zone guard.",                     Type = "QuestGiver", MapName = "FrozenMountain", PositionX = 70.45686,   PositionY = 18.80354,    InteractionRadius = 2.5f, IsActive = true },
                new NPC { NPCId = 11, Name = "Valiant Warrior",       Description = "A brave warrior fighting skeletons.",           Type = "QuestGiver", MapName = "AbandonedCastle",PositionX = -10.66112,  PositionY = 54.92884,    InteractionRadius = 2.5f, IsActive = true },
                new NPC { NPCId = 12, Name = "Natalie",               Description = "The ghost of a young girl.",                    Type = "QuestGiver", MapName = "AbandonedCastle",PositionX = -48.92126,  PositionY = -21.12006,   InteractionRadius = 2.5f, IsActive = true },
                new NPC { NPCId = 13, Name = "Elf Guard",             Description = "The lone guard of the deserted island.",        Type = "QuestGiver", MapName = "AbandonedCastle",PositionX = -104.8f,   PositionY = -4.776f,    InteractionRadius = 2.5f, IsActive = true }
            );

            // ─────────────────────────────────────────────────────────────────────────
            // QUESTS – 31 main story quests with fixed IDs, in play order.
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
                new Quest { QuestId = 1,  Title = "[Chapter 1] A Word with Elder Rowan",      Description = "You wake at the edge of the Elf Forest with no memory of how you arrived. Elder Rowan is waiting by the great roots — go to him and hear why the forest called you here.",                Type = "Main", DefaultStatus = "NotStarted", MapName = "ElfForest",      RequiredLevel = 1,  TargetAmount = 1,  RewardExperience = 5,    RewardGold = 10m,   RewardGems = 5m, ObjectiveType = "Talk",       ObjectiveTarget = "Elder Rowan",    ObjectiveLocation = "Elf Forest",       QuestGiverName = "Elder Rowan",          IsActive = true },
                new Quest { QuestId = 2,  Title = "[Chapter 1] Gather White Flowers",         Description = "The elders brew their healing draught from white flowers that only bloom in the shade of the old woods. Search the clearings and gather 3 White Flowers for Elder Rowan.",              Type = "Main", DefaultStatus = "NotStarted", MapName = "ElfForest",      RequiredLevel = 1,  TargetAmount = 3,  RewardExperience = 10,   RewardGold = 8m,    RewardGems = 5m, ObjectiveType = "Collect",    ObjectiveTarget = "White Flower",   ObjectiveLocation = "Elf Forest",       QuestGiverName = "Elder Rowan",          IsActive = true },
                new Quest { QuestId = 3,  Title = "[Chapter 1] Deliver the White Flowers",    Description = "Bring the gathered flowers back to Elder Rowan. In return he will teach you the first strike an elf ever learns.",                                                                     Type = "Main", DefaultStatus = "NotStarted", MapName = "ElfForest",      RequiredLevel = 1,  TargetAmount = 1,  RewardExperience = 5,    RewardGold = 5m,    RewardGems = 5m, ObjectiveType = "Talk",       ObjectiveTarget = "Elder Rowan",    ObjectiveLocation = "Elf Forest",       QuestGiverName = "Elder Rowan",          IsActive = true, RewardSkillId = 10 },
                new Quest { QuestId = 4,  Title = "[Chapter 1] Equip Your First Skill",       Description = "A skill is useless until it sits in your hand. Open the Skill panel and equip the technique Elder Rowan just taught you.",                                                             Type = "Main", DefaultStatus = "NotStarted", MapName = "ElfForest",      RequiredLevel = 1,  TargetAmount = 1,  RewardExperience = 10,   RewardGold = 10m,   RewardGems = 5m, ObjectiveType = "EquipSkill", ObjectiveTarget = "Skill Panel",    ObjectiveLocation = "Elf Forest",       QuestGiverName = "Elder Rowan",          IsActive = true },
                new Quest { QuestId = 5,  Title = "[Chapter 1] Cull the Little Slimes",       Description = "Little slimes have crept out of the marsh and are eating the flower beds. Put your new skill to work and defeat 3 of them.",                                                           Type = "Main", DefaultStatus = "NotStarted", MapName = "ElfForest",      RequiredLevel = 1,  TargetAmount = 3,  RewardExperience = 15,   RewardGold = 15m,   RewardGems = 5m, ObjectiveType = "Defeat",     ObjectiveTarget = "Slime Little",    ObjectiveLocation = "Elf Forest",       QuestGiverName = "Elder Rowan",          IsActive = true },
                new Quest { QuestId = 6,  Title = "[Chapter 1] Slay the Swamp Demon",         Description = "The slimes were only fleeing something worse. A Swamp Demon broods in the deep woods over some old relic, and the water rots around it. Kill it and take whatever it is guarding.",                                       Type = "Main", DefaultStatus = "NotStarted", MapName = "ElfForest",      RequiredLevel = 2,  TargetAmount = 1,  RewardExperience = 25,   RewardGold = 50m,   RewardGems = 5m, ObjectiveType = "Defeat",     ObjectiveTarget = "Swamp Demon",     ObjectiveLocation = "Deep Woods",       QuestGiverName = "Elder Rowan",          IsActive = true, BossMonsterId = 2 },
                new Quest { QuestId = 7,  Title = "[Chapter 1] Lyra and the Origin Tree",     Description = "Rowan cannot name the relic you took from the swamp. Carry it to Lyra at the Origin Tree — she is older than every elf alive, and she will know what you are holding.",                              Type = "Main", DefaultStatus = "NotStarted", MapName = "ElfForest",      RequiredLevel = 2,  TargetAmount = 1,  RewardExperience = 10,   RewardGold = 10m,   RewardGems = 5m, ObjectiveType = "Talk",       ObjectiveTarget = "Lyra",           ObjectiveLocation = "Origin Tree",      QuestGiverName = "Lyra",                 IsActive = true },
                new Quest { QuestId = 8,  Title = "[Chapter 1] Follow the Cloaked Figure",    Description = "A cloaked figure has been watching you since you woke, and now walks into a portal at the forest edge. Step through it before the way closes.",                                        Type = "Main", DefaultStatus = "NotStarted", MapName = "ElfForest",      RequiredLevel = 2,  TargetAmount = 1,  RewardExperience = 5,    RewardGold = 5m,    RewardGems = 5m, ObjectiveType = "Explore",    ObjectiveTarget = "Portal",         ObjectiveLocation = "Elf Forest",       QuestGiverName = "Mysterious Figure",    IsActive = true },
                // ── MAP 2: Autumn Pumpkin ────────────────────────────────────────────
                new Quest { QuestId = 9,  Title = "[Chapter 2] Ask Where You Are",            Description = "The portal spits you onto a cold beach under an autumn sky. Climb to the castle and find Drake, the one soul here willing to speak to a stranger, and ask what land this is.",                    Type = "Main", DefaultStatus = "NotStarted", MapName = "AutumnPumpkin",  RequiredLevel = 3,  TargetAmount = 1,  RewardExperience = 100,  RewardGold = 5m,    RewardGems = 5m, ObjectiveType = "Talk",       ObjectiveTarget = "Drake",          ObjectiveLocation = "Autumn Pumpkin",   QuestGiverName = "Drake",                IsActive = true },
                new Quest { QuestId = 10, Title = "[Chapter 2] Harvest for Your Supper",      Description = "You have no coin in this land and no one gives bread away. Farmer Fa will trade a meal for labour: pick 8 Enchanted Pumpkins from his field.",                                        Type = "Main", DefaultStatus = "NotStarted", MapName = "AutumnPumpkin",  RequiredLevel = 3,  TargetAmount = 8,  RewardExperience = 300,  RewardGold = 10m,   RewardGems = 5m, ObjectiveType = "Collect",    ObjectiveTarget = "Enchanted Pumpkin",ObjectiveLocation = "Pumpkin Town",   QuestGiverName = "Fa",                   IsActive = true },
                new Quest { QuestId = 11, Title = "[Chapter 2] Deliver the Harvest",          Description = "Fa is too old to make the road alone. Carry the harvest to the city gate and hand it to the guard Tristan.",                                                                          Type = "Main", DefaultStatus = "NotStarted", MapName = "AutumnPumpkin",  RequiredLevel = 3,  TargetAmount = 1,  RewardExperience = 200,  RewardGold = 5m,    RewardGems = 5m, ObjectiveType = "Talk",       ObjectiveTarget = "Tristan",        ObjectiveLocation = "City Gate",        QuestGiverName = "Fa",                   IsActive = true },
                new Quest { QuestId = 12, Title = "[Chapter 2] Examine the Fallen",           Description = "Beyond the gate the city is silent and the streets are full of the dead. Examine 5 of the bodies and learn what killed them.",                                                        Type = "Main", DefaultStatus = "NotStarted", MapName = "AutumnPumpkin",  RequiredLevel = 3,  TargetAmount = 5,  RewardExperience = 250,  RewardGold = 5m,    RewardGems = 5m, ObjectiveType = "Interact",   ObjectiveTarget = "Corpse",         ObjectiveLocation = "Ruined City",      QuestGiverName = "Tristan",              IsActive = true },
                new Quest { QuestId = 13, Title = "[Chapter 2] Seek the Silver Knight",       Description = "Tristan pales at your report: only one man ever held these ruins. Search the city for the silver knight Arthur and ask for his help.",                                               Type = "Main", DefaultStatus = "NotStarted", MapName = "AutumnPumpkin",  RequiredLevel = 3,  TargetAmount = 1,  RewardExperience = 250,  RewardGold = 5m,    RewardGems = 5m, ObjectiveType = "Talk",       ObjectiveTarget = "Arthur",         ObjectiveLocation = "Ruined City",      QuestGiverName = "Tristan",              IsActive = true },
                new Quest { QuestId = 14, Title = "[Chapter 2] Train in the Old Dungeon",     Description = "Arthur's wounds run deeper than his armour and his power is sealed away; he cannot fight for the city. He can, however, make you strong enough to. Clear his training dungeon.",       Type = "Main", DefaultStatus = "NotStarted", MapName = "AutumnPumpkin",  RequiredLevel = 4,  TargetAmount = 1,  RewardExperience = 250,  RewardGold = 15m,   RewardGems = 5m, ObjectiveType = "Explore",    ObjectiveTarget = "Dungeon_2",      ObjectiveLocation = "Dungeon",          QuestGiverName = "Arthur",               IsActive = true, RewardSkillId = 9, RewardItemId = 18 },
                new Quest { QuestId = 15, Title = "[Chapter 2] Trial I: The Robber Camp",      Description = "Arthur will not send you at a dragon on faith. He sets four trials, and the first is the robbers holding the eastern camp. Cut down 6 of them.",                       Type = "Main", DefaultStatus = "NotStarted", MapName = "AutumnPumpkin",  RequiredLevel = 4,  TargetAmount = 6, RewardExperience = 250,  RewardGold = 25m,   RewardGems = 5m, ObjectiveType = "Defeat",     ObjectiveTarget = "Robber", ObjectiveLocation = "Robber Camp", QuestGiverName = "Arthur", IsActive = true },
                new Quest { QuestId = 16, Title = "[Chapter 2] Trial II: The Haunted Quarter", Description = "One trial stands to your name. The second is the haunted quarter - ghosts, necromancers, and the red guard who died at their posts. Put down 10.",                     Type = "Main", DefaultStatus = "NotStarted", MapName = "AutumnPumpkin",  RequiredLevel = 4,  TargetAmount = 10, RewardExperience = 300,  RewardGold = 30m,   RewardGems = 5m, ObjectiveType = "Defeat",     ObjectiveTarget = "Ghost", ObjectiveLocation = "Haunted Quarter", QuestGiverName = "Arthur", IsActive = true },
                new Quest { QuestId = 17, Title = "[Chapter 2] Trial III: The Goblin Grounds", Description = "Two trials done. The third lies south of the ruins, where goblin spear and axe bands have dug in. Break 3 of them.",                                                   Type = "Main", DefaultStatus = "NotStarted", MapName = "AutumnPumpkin",  RequiredLevel = 4,  TargetAmount = 3, RewardExperience = 250,  RewardGold = 25m,   RewardGems = 5m, ObjectiveType = "Defeat",     ObjectiveTarget = "Goblin", ObjectiveLocation = "Goblin Grounds", QuestGiverName = "Arthur", IsActive = true },
                new Quest { QuestId = 18, Title = "[Chapter 2] Trial IV: The Goblin Warlord",  Description = "The goblins you broke were only a warband, and every warband answers to someone. Their warlord still holds the Goblin Grounds. Kill him and the last trial is yours.", Type = "Main", DefaultStatus = "NotStarted", MapName = "AutumnPumpkin",  RequiredLevel = 4,  TargetAmount = 1, RewardExperience = 350,  RewardGold = 40m,   RewardGems = 5m, ObjectiveType = "Defeat",     ObjectiveTarget = "Goblin Warlord", ObjectiveLocation = "Goblin Grounds", QuestGiverName = "Arthur", IsActive = true, BossMonsterId = 22 },
                new Quest { QuestId = 19, Title = "[Chapter 2] Slay the Dragon",              Description = "Arthur admits you now fight as well as he once did — and tells you what truly broke the city. A dragon nests in the ruins. End it.",                                                 Type = "Main", DefaultStatus = "NotStarted", MapName = "AutumnPumpkin",  RequiredLevel = 5,  TargetAmount = 1,  RewardExperience = 350,  RewardGold = 100m,  RewardGems = 5m, ObjectiveType = "Defeat",     ObjectiveTarget = "Red Dragon", ObjectiveLocation = "Ruined City",      QuestGiverName = "Arthur",               IsActive = true, BossMonsterId = 7 },
                new Quest { QuestId = 20, Title = "[Chapter 2] Arthur's Parting Words",       Description = "Return to Arthur for the knight's thanks and ask where the cursed codex came from. He points north, to a kingdom the codex froze solid.",                                            Type = "Main", DefaultStatus = "NotStarted", MapName = "AutumnPumpkin",  RequiredLevel = 5,  TargetAmount = 1,  RewardExperience = 150,  RewardGold = 10m,   RewardGems = 5m, ObjectiveType = "Talk",       ObjectiveTarget = "Arthur",         ObjectiveLocation = "Ruined City",      QuestGiverName = "Arthur",               IsActive = true },
                // ── MAP 3: Frozen Mountain ───────────────────────────────────────────
                new Quest { QuestId = 21, Title = "[Chapter 3] Slimes of the Snow Fields",    Description = "Queen Roselyn Aurora receives you in a hall of ice. Her fields are overrun before the winter stores are in — defeat 8 ice slimes for her.",                                        Type = "Main", DefaultStatus = "NotStarted", MapName = "FrozenMountain", RequiredLevel = 6,  TargetAmount = 8,  RewardExperience = 200,  RewardGold = 30m,   RewardGems = 5m, ObjectiveType = "Defeat",     ObjectiveTarget = "slime_ice",      ObjectiveLocation = "Snow Fields",      QuestGiverName = "Roselyn Aurora Queen", IsActive = true, RewardItemId = 31 },
                new Quest { QuestId = 22, Title = "[Chapter 3] Deliver the Magic Flour",      Description = "The Queen entrusts you with Magic Flour for the mountain shrine. Carry it up to the priest Zephyr before the pass closes.",                                                          Type = "Main", DefaultStatus = "NotStarted", MapName = "FrozenMountain", RequiredLevel = 6,  TargetAmount = 1,  RewardExperience = 150,  RewardGold = 15m,   RewardGems = 5m, ObjectiveType = "Talk",       ObjectiveTarget = "Zephyr",         ObjectiveLocation = "Frozen Mountain",  QuestGiverName = "Roselyn Aurora Queen", IsActive = true },
                new Quest { QuestId = 23, Title = "[Chapter 3] Dragons of the Frozen Peak",   Description = "Zephyr cannot hold his rites while ice dragons circle the shrine. Climb the peak and bring down 5 of them.",                                                                        Type = "Main", DefaultStatus = "NotStarted", MapName = "FrozenMountain", RequiredLevel = 7,  TargetAmount = 5,  RewardExperience = 250,  RewardGold = 40m,   RewardGems = 5m, ObjectiveType = "Defeat",     ObjectiveTarget = "Ice_Dragon",     ObjectiveLocation = "Frozen Mountain",  QuestGiverName = "Zephyr",               IsActive = true },
                new Quest { QuestId = 24, Title = "[Chapter 3] The Warden of the Ban",        Description = "Zephyr says the codex's mark lies inside the forbidden zone, and only its warden may open the way. Find Roland at the boundary stones and ask for passage.",                          Type = "Main", DefaultStatus = "NotStarted", MapName = "FrozenMountain", RequiredLevel = 7,  TargetAmount = 1,  RewardExperience = 150,  RewardGold = 15m,   RewardGems = 5m, ObjectiveType = "Talk",       ObjectiveTarget = "Roland",         ObjectiveLocation = "Forbidden Zone",   QuestGiverName = "Roland",               IsActive = true },
                new Quest { QuestId = 25, Title = "[Chapter 3] Break the Stone Guardian",     Description = "Roland tells you what the kingdom buried here: the codex itself, and the golem forged to guard it. Destroy the golem and take the second Seal Book.",                                 Type = "Main", DefaultStatus = "NotStarted", MapName = "FrozenMountain", RequiredLevel = 8,  TargetAmount = 1,  RewardExperience = 400,  RewardGold = 150m,  RewardGems = 5m, ObjectiveType = "Defeat",     ObjectiveTarget = "GolemBoss",      ObjectiveLocation = "Forbidden Zone",   QuestGiverName = "Roland",               IsActive = true, BossMonsterId = 10 },
                // ── MAP 4: Abandoned Castle ──────────────────────────────────────────
                new Quest { QuestId = 26, Title = "[Chapter 4] Break the Skeleton Army",      Description = "The trail of the seals ends at a ruined castle where the dead still keep watch. The Valiant Warrior holds the valley alone — help him put down 12 skeletons.",                       Type = "Main", DefaultStatus = "NotStarted", MapName = "AbandonedCastle",RequiredLevel = 9,  TargetAmount = 12, RewardExperience = 300,  RewardGold = 50m,   RewardGems = 5m, ObjectiveType = "Defeat",     ObjectiveTarget = "Skeleton",       ObjectiveLocation = "Valley",           QuestGiverName = "Valiant Warrior",      IsActive = true },
                new Quest { QuestId = 27, Title = "[Chapter 4] The Skull by the Well",        Description = "In the drowned village of Tide-Knell a girl named Natalie asks a strange favour: dig beside the old well and lift out the skull buried there.",                                     Type = "Main", DefaultStatus = "NotStarted", MapName = "AbandonedCastle",RequiredLevel = 9,  TargetAmount = 1,  RewardExperience = 200,  RewardGold = 30m,   RewardGems = 5m, ObjectiveType = "Interact",   ObjectiveTarget = "Skull",          ObjectiveLocation = "Tide-Knell",       QuestGiverName = "Natalie",              IsActive = true, RewardItemId = 32 },
                new Quest { QuestId = 28, Title = "[Chapter 4] Lay Natalie to Rest",          Description = "The skull is hers. Read the letter she left behind, bury her remains beneath the ivy tree, and she will give you the key she died holding.",                                        Type = "Main", DefaultStatus = "NotStarted", MapName = "AbandonedCastle",RequiredLevel = 10, TargetAmount = 1,  RewardExperience = 200,  RewardGold = 40m,   RewardGems = 5m, ObjectiveType = "Interact",   ObjectiveTarget = "Ivy Tree",       ObjectiveLocation = "Tide-Knell",       QuestGiverName = "Natalie",              IsActive = true, RewardItemId = 33 },
                new Quest { QuestId = 29, Title = "[Chapter 4] Ancient Leaves of the Isle",   Description = "Natalie's key opens the way to a deserted island where one elf guard still stands his post. He needs 5 Ancient Leaves from the plateau to break the seal below.",                   Type = "Main", DefaultStatus = "NotStarted", MapName = "AbandonedCastle",RequiredLevel = 10, TargetAmount = 5,  RewardExperience = 250,  RewardGold = 45m,   RewardGems = 5m, ObjectiveType = "Collect",    ObjectiveTarget = "Ancient Leaves", ObjectiveLocation = "Northern Plateau", QuestGiverName = "Elf Guard",            IsActive = true },
                new Quest { QuestId = 30, Title = "[Chapter 4] Defeat the UnderKing",         Description = "The leaves burn away the ward and the crypt opens. The UnderKing holds the last two Seal Books — take them from him.",                                                             Type = "Main", DefaultStatus = "NotStarted", MapName = "AbandonedCastle",RequiredLevel = 11, TargetAmount = 1,  RewardExperience = 500,  RewardGold = 300m,  RewardGems = 5m, ObjectiveType = "Defeat",     ObjectiveTarget = "UnderKing",      ObjectiveLocation = "Deserted Island",  QuestGiverName = "Elf Guard",            IsActive = true, BossMonsterId = 15 },
                new Quest { QuestId = 31, Title = "[Chapter 4] Ask for the Way Home",         Description = "All four seals are in your pack. Speak to the Elf Guard — he can open a portal back to the Elf Forest.",                                                                            Type = "Main", DefaultStatus = "NotStarted", MapName = "AbandonedCastle",RequiredLevel = 12, TargetAmount = 1,  RewardExperience = 150,  RewardGold = 10m,   RewardGems = 5m, ObjectiveType = "Talk",       ObjectiveTarget = "Elf Guard",      ObjectiveLocation = "Deserted Island",  QuestGiverName = "Elf Guard",            IsActive = true },
                // ── FINALE: back to the Elf Forest ───────────────────────────────────
                new Quest { QuestId = 32, Title = "[Chapter 5] Return with the Seals",        Description = "You are home, and the Origin Tree is worse than you left it. Bring all four Seal Books to Lyra.",                                                                                  Type = "Main", DefaultStatus = "NotStarted", MapName = "ElfForest",      RequiredLevel = 12, TargetAmount = 1,  RewardExperience = 250,  RewardGold = 50m,   RewardGems = 5m, ObjectiveType = "Talk",       ObjectiveTarget = "Lyra",           ObjectiveLocation = "Origin Tree",      QuestGiverName = "Lyra",                 IsActive = true },
                new Quest { QuestId = 33, Title = "[Chapter 5] Heal the Origin Tree",         Description = "Lyra opens the rite and steps back — the seals must be set by the one who won them. Place the four Seal Books on the Origin Tree and break the curse.",                             Type = "Main", DefaultStatus = "NotStarted", MapName = "ElfForest",      RequiredLevel = 12, TargetAmount = 1,  RewardExperience = 400,  RewardGold = 250m,  RewardGems = 5m, ObjectiveType = "Interact",   ObjectiveTarget = "Origin Tree",    ObjectiveLocation = "Elf Forest",       QuestGiverName = "Lyra",                 IsActive = true },
                new Quest { QuestId = 34, Title = "[Chapter 5] A New Dawn",                   Description = "The Origin Tree is green again and the forest wakes around it. Speak with Lyra one last time — the codex had a master, and that story is not finished.",                            Type = "Main", DefaultStatus = "NotStarted", MapName = "ElfForest",      RequiredLevel = 12, TargetAmount = 1,  RewardExperience = 300,  RewardGold = 200m,  RewardGems = 5m, ObjectiveType = "Talk",       ObjectiveTarget = "Lyra",           ObjectiveLocation = "Origin Tree",      QuestGiverName = "Lyra",                 IsActive = true }
            );

            // ─────────────────────────────────────────────────────────────────────────
            // NPC DIALOGUES – Fixed IDs with NPCId and LinkedQuestId referencing
            // the fixed NPC and Quest IDs above.
            // ─────────────────────────────────────────────────────────────────────────
            modelBuilder.Entity<NPCDialogue>().HasData(
                // ── Q1: A Word with Elder Rowan (QuestId=1, NPCId=1) ──
                // Rowan mở đầu bằng LÝ DO người chơi là người được chọn (rừng tự đánh thức,
                // chỉ mình họ nghe được tiếng gọi của Origin Tree) — nếu thiếu, người chơi
                // không có động cơ nào để trở thành nhân vật chính.
                new NPCDialogue { NPCDialogueId = 1, NPCId = 1, LinkedQuestId = 1, ResponseType = "None", Content = "Ah... a new face, and not one born of these woods. Welcome to the Elf Forest, traveler.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 127, NPCId = 1, LinkedQuestId = 1, ResponseType = "None", Content = "You did not wander in here. The forest awakened you at its edge, and the forest does not wake strangers.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 2, NPCId = 1, LinkedQuestId = 1, ResponseType = "None", Content = "For a thousand years this forest kept itself in peace. Now something gathers in the dark beneath the roots.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 94, NPCId = 1, LinkedQuestId = 1, ResponseType = "None", Content = "The Origin Tree at our heart is sickening. Its leaves fall in high summer, and the animals no longer sleep here.", DisplayOrder = 4, IsActive = true },
                new NPCDialogue { NPCDialogueId = 128, NPCId = 1, LinkedQuestId = 1, ResponseType = "None", Content = "A hundred of my people have walked past those roots and heard nothing. You heard it call before you opened your eyes. No one else could.", DisplayOrder = 5, IsActive = true },
                new NPCDialogue { NPCDialogueId = 3, NPCId = 1, LinkedQuestId = 1, ResponseType = "Quest", Content = "So the Origin Tree chose you, and I must trust its choosing. I am Elder Rowan. Speak with me when you are ready to begin.", DisplayOrder = 6, IsActive = true },
                // ── Q2: Gather White Flowers (QuestId=2, NPCId=1) ──
                // Động cơ cụ thể cho quest hái hoa: hoa là NGUYÊN LIỆU CHỮA THƯƠNG và Rowan
                // cần chúng để cứu dân làng đang hấp hối — không còn là việc sai vặt vô cớ.
                new NPCDialogue { NPCDialogueId = 4, NPCId = 1, LinkedQuestId = 2, ResponseType = "None", Content = "Before anything else, I must beg medicine of you. Eleven of my village lie in the healing hall, and my stores are almost gone.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 129, NPCId = 1, LinkedQuestId = 2, ResponseType = "None", Content = "The rot came up through the well water. The children fell first, then whoever carried them. Salve buys them days, no more than that.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 5, NPCId = 1, LinkedQuestId = 2, ResponseType = "None", Content = "By the old willow clearing grows a white flower that only opens where the air is still clean. Crushed with spring water, it is the one salve that answers this sickness.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 95, NPCId = 1, LinkedQuestId = 2, ResponseType = "None", Content = "Where those flowers still bloom, the curse has not yet reached. They are medicine and warning both.", DisplayOrder = 4, IsActive = true },
                new NPCDialogue { NPCDialogueId = 130, NPCId = 1, LinkedQuestId = 2, ResponseType = "None", Content = "My knees will not carry me that far anymore. Bring me the flowers and my people live through the week. That is the whole of it.", DisplayOrder = 5, IsActive = true },
                new NPCDialogue { NPCDialogueId = 6, NPCId = 1, LinkedQuestId = 2, ResponseType = "Quest", Content = "Go to the clearing and gather 3 White Flowers for me. Take care, even slimes wander there now.", DisplayOrder = 6, IsActive = true },
                // ── Q3: Deliver the White Flowers (QuestId=3, NPCId=1) ──
                // Kỹ năng (RewardSkillId=10) giờ là PHẦN THƯỞNG CHO VIỆC ĐÃ CHỨNG TỎ BẢN THÂN:
                // Rowan phá lệ truyền dạy vì người chơi đã cứu dân làng, thay vì "cầm lấy đi".
                new NPCDialogue { NPCDialogueId = 7, NPCId = 1, LinkedQuestId = 3, ResponseType = "None", Content = "Back already? Let me see your hands... ah, you found them.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 8, NPCId = 1, LinkedQuestId = 3, ResponseType = "None", Content = "Not a petal bruised. Three flowers, three doses. The healing hall will have them before nightfall, and the children will sleep.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 131, NPCId = 1, LinkedQuestId = 3, ResponseType = "None", Content = "I sent a stranger into cursed woods for people you had never met, and you went without asking payment. You have proven yourself.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 96, NPCId = 1, LinkedQuestId = 3, ResponseType = "None", Content = "We do not teach our craft outside the bloodline. For you I will break that rule. Let me teach you the First Elven Technique.", DisplayOrder = 4, IsActive = true },
                new NPCDialogue { NPCDialogueId = 9, NPCId = 1, LinkedQuestId = 3, ResponseType = "Reward", Content = "Hold still. Breathe with the roots, as we do... there. It is yours now, with an old elf's thanks.", DisplayOrder = 5, IsActive = true },
                // ── Q4: Equip Your First Skill (QuestId=4, NPCId=1) ──
                // Nối liền với Q3: kỹ năng vừa được DẠY, giờ phải đặt vào tay mới dùng được.
                new NPCDialogue { NPCDialogueId = 10, NPCId = 1, LinkedQuestId = 4, ResponseType = "None", Content = "The technique is in you now, but a technique you have not called upon is no better than one you never learned.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 11, NPCId = 1, LinkedQuestId = 4, ResponseType = "None", Content = "Every warrior in this world channels power through learned technique. Bare fists will not answer what waits out there.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 97, NPCId = 1, LinkedQuestId = 4, ResponseType = "None", Content = "Set it where your hand can reach it without thinking. In a fight you will not have time to remember.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 12, NPCId = 1, LinkedQuestId = 4, ResponseType = "Quest", Content = "Open your Skill Panel and equip the First Elven Technique. Do not step past the treeline without it.", DisplayOrder = 4, IsActive = true },
                // ── Q5: Cull the Little Slimes (QuestId=5, NPCId=1) ──
                new NPCDialogue { NPCDialogueId = 13, NPCId = 1, LinkedQuestId = 5, ResponseType = "None", Content = "Good. I can feel the power settled in you now. It must be tested before it is trusted.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 14, NPCId = 1, LinkedQuestId = 5, ResponseType = "None", Content = "The outskirts crawl with little slimes. They were harmless once, now they hunt in packs.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 98, NPCId = 1, LinkedQuestId = 5, ResponseType = "None", Content = "They are the curse's smallest children. Where they spread, the soil dies behind them.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 15, NPCId = 1, LinkedQuestId = 5, ResponseType = "Quest", Content = "Go out and defeat 3 little slimes, then return and tell me what you felt out there.", DisplayOrder = 4, IsActive = true },
                // ── Q6: Slay the Swamp Demon (QuestId=6, NPCId=1) ──
                // Rowan CHỈ biết hai điều: Swamp Demon đang gây ô nhiễm, và trong đầm lầy có
                // một cổ vật/phong ấn. Ông KHÔNG biết nó tên "Seal Book", KHÔNG biết có 4 quyển,
                // KHÔNG biết chúng là chìa khóa cứu Origin Tree — đó là phần Lyra tiết lộ ở Q7.
                new NPCDialogue { NPCDialogueId = 16, NPCId = 1, LinkedQuestId = 6, ResponseType = "None", Content = "You handled them cleanly. But the slimes are only spillage from something far worse.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 17, NPCId = 1, LinkedQuestId = 6, ResponseType = "None", Content = "Deep in the swamp lies a Demon. The water rots around it, and the corruption creeps closer each night.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 99, NPCId = 1, LinkedQuestId = 6, ResponseType = "None", Content = "Our scouts say the beast broods over some old relic down there. A seal of some kind, they think. I am no scholar, and none of them got close enough to be sure.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 132, NPCId = 1, LinkedQuestId = 6, ResponseType = "None", Content = "What it is, and whether it matters, I cannot tell you. Lyra at the Origin Tree is older than every elf alive. Bring it to her and she will know.", DisplayOrder = 4, IsActive = true },
                new NPCDialogue { NPCDialogueId = 18, NPCId = 1, LinkedQuestId = 6, ResponseType = "Quest", Content = "Destroy the Swamp Demon and take whatever it is guarding. Stopping the rot at its source is what matters.", DisplayOrder = 5, IsActive = true },
                // ── Q7: Lyra and the Origin Tree (QuestId=7, NPCId=2=Lyra) ──
                // Lyra là người TIẾT LỘ: đây là Seal Book đầu tiên, có 4 quyển, và chúng là
                // chìa khóa cứu Origin Tree. Toàn bộ lore này đã bị lấy khỏi lời Rowan ở Q6.
                new NPCDialogue { NPCDialogueId = 19, NPCId = 2, LinkedQuestId = 7, ResponseType = "None", Content = "Come closer, brave one. I am Lyra, not elf and not ghost. I am the spirit of the Origin Tree itself.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 20, NPCId = 2, LinkedQuestId = 7, ResponseType = "None", Content = "Look at my bark. The curse has reached my heartwood, and I am dying slowly, from the inside outward.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 133, NPCId = 2, LinkedQuestId = 7, ResponseType = "None", Content = "Now show me what you took from the swamp... ah. Rowan sent you here not knowing, did he. Hold it up, child.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 134, NPCId = 2, LinkedQuestId = 7, ResponseType = "None", Content = "This is a Seal Book. The first of them to see daylight in an age, and no living elf has ever held one.", DisplayOrder = 4, IsActive = true },
                new NPCDialogue { NPCDialogueId = 100, NPCId = 2, LinkedQuestId = 7, ResponseType = "None", Content = "Long ago the elders bound an ancient power into four such books. That binding has broken, and the leak is what poisons me.", DisplayOrder = 5, IsActive = true },
                new NPCDialogue { NPCDialogueId = 135, NPCId = 2, LinkedQuestId = 7, ResponseType = "None", Content = "Four books, scattered and guarded. They are not treasure, they are the lock on my heartwood. Nothing else will save me.", DisplayOrder = 6, IsActive = true },
                new NPCDialogue { NPCDialogueId = 21, NPCId = 2, LinkedQuestId = 7, ResponseType = "Quest", Content = "Only the 4 Seal Books can cleanse me. You hold the first already, find the remaining three, and hurry!", DisplayOrder = 7, IsActive = true },
                // ── Q8: Follow the Cloaked Figure (QuestId=8, NPCId=3) ──
                new NPCDialogue { NPCDialogueId = 22, NPCId = 3, LinkedQuestId = 8, ResponseType = "None", Content = "Heh... so you are the little errand-runner gathering up the Seal Books.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 23, NPCId = 3, LinkedQuestId = 8, ResponseType = "None", Content = "You carry them and do not even know what they are, or whose hand cursed that tree.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 101, NPCId = 3, LinkedQuestId = 8, ResponseType = "None", Content = "The elves told you a story with the ugly parts cut out. I can show you what they buried.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 24, NPCId = 3, LinkedQuestId = 8, ResponseType = "Quest", Content = "The truth waits through this portal. Follow me, or stay and keep watering a dying tree.", DisplayOrder = 4, IsActive = true },
                // ── Q9: Ask Where You Are (QuestId=9, NPCId=4=Drake) ──
                new NPCDialogue { NPCDialogueId = 25, NPCId = 4, LinkedQuestId = 9, ResponseType = "None", Content = "Steady, traveler. That portal spat us both out here on the beach, and the cloaked one is long gone.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 26, NPCId = 4, LinkedQuestId = 9, ResponseType = "None", Content = "We are far from the forest now, with no coin between us and no way back that I can see.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 102, NPCId = 4, LinkedQuestId = 9, ResponseType = "None", Content = "This is farming country. Folk here trade a day of work for supper, and honest work is easy to find.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 27, NPCId = 4, LinkedQuestId = 9, ResponseType = "Quest", Content = "Go and speak with Fa, the farmer just up the path. He always needs hands.", DisplayOrder = 4, IsActive = true },
                // ── Q10: Harvest for Your Supper (QuestId=10, NPCId=7=Fa) ──
                new NPCDialogue { NPCDialogueId = 28, NPCId = 7, LinkedQuestId = 10, ResponseType = "None", Content = "Drake sent you? Good timing, stranger. My back is not what it was.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 29, NPCId = 7, LinkedQuestId = 10, ResponseType = "None", Content = "The whole field came ripe at once, and the harvest cart leaves for the city at dusk.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 103, NPCId = 7, LinkedQuestId = 10, ResponseType = "None", Content = "Mind the ones that glow faintly. An enchanted pumpkin keeps a lantern lit all winter, that is why the city pays.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 30, NPCId = 7, LinkedQuestId = 10, ResponseType = "Quest", Content = "Collect 8 Enchanted Pumpkins for me and I will see you fed tonight.", DisplayOrder = 4, IsActive = true },
                // ── Q11: Deliver the Harvest (QuestId=11, NPCId=7=Fa) ──
                new NPCDialogue { NPCDialogueId = 31, NPCId = 7, LinkedQuestId = 11, ResponseType = "None", Content = "Eight, and not one bruised. You work like a farmhand born, not a wanderer.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 32, NPCId = 7, LinkedQuestId = 11, ResponseType = "None", Content = "Now the hard half of the job. These are owed at the city gate before nightfall.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 104, NPCId = 7, LinkedQuestId = 11, ResponseType = "None", Content = "I would carry them myself, but no one from this farm has come back from that road in a week.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 33, NPCId = 7, LinkedQuestId = 11, ResponseType = "Quest", Content = "Take them to the guard Tristan at the ruined city, and tell him Fa sent you.", DisplayOrder = 4, IsActive = true },
                // ── Q12: Examine the Fallen (QuestId=12, NPCId=5=Tristan) ──
                new NPCDialogue { NPCDialogueId = 34, NPCId = 5, LinkedQuestId = 12, ResponseType = "None", Content = "Halt! Who goes... ah, pumpkins from Fa. Set them down, you may be the last delivery this gate sees.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 35, NPCId = 5, LinkedQuestId = 12, ResponseType = "None", Content = "Something is wrong inside. No bells, no market noise, no smoke from a single chimney since dawn.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 105, NPCId = 5, LinkedQuestId = 12, ResponseType = "None", Content = "I am Tristan, and my orders bind me to this gate. I cannot take one step past it, even now.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 36, NPCId = 5, LinkedQuestId = 12, ResponseType = "Quest", Content = "Go in and look at the fallen with your own eyes. Then come back and tell me the truth of it.", DisplayOrder = 4, IsActive = true },
                // ── Q13: Seek the Silver Knight (QuestId=13, NPCId=5=Tristan) ──
                new NPCDialogue { NPCDialogueId = 37, NPCId = 5, LinkedQuestId = 13, ResponseType = "None", Content = "All of them? Every soul in the city, cut down where they stood? Gods, I stood here and heard nothing.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 38, NPCId = 5, LinkedQuestId = 13, ResponseType = "None", Content = "No bandit crew does this in one night. Whatever walked in there was not a man with a sword.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 106, NPCId = 5, LinkedQuestId = 13, ResponseType = "None", Content = "There is one person left who might stand against it. Arthur, the silver knight, camped in the old ruins.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 39, NPCId = 5, LinkedQuestId = 13, ResponseType = "Quest", Content = "Find Arthur and report what you saw. Go, before whatever did this moves on to the next town.", DisplayOrder = 4, IsActive = true },
                // ── Q14: Train in the Old Dungeon (QuestId=14, NPCId=6=Arthur) ──
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
                // ── Q19: Slay the Dragon (QuestId=19, NPCId=6=Arthur) ──
                new NPCDialogue { NPCDialogueId = 46, NPCId = 6, LinkedQuestId = 19, ResponseType = "None", Content = "You came back quieter than you left. That is how I know the fighting took hold in you.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 47, NPCId = 6, LinkedQuestId = 19, ResponseType = "None", Content = "Then hear the rest of it. The monsters were never the cause. Something older nests above the ruins.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 109, NPCId = 6, LinkedQuestId = 19, ResponseType = "None", Content = "A dragon. It is the thing that broke this city, and the thing that broke me. I have carried that shame for years.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 48, NPCId = 6, LinkedQuestId = 19, ResponseType = "Quest", Content = "Finish what I could not. Climb to its nest and slay the dragon!", DisplayOrder = 4, IsActive = true },
                // ── Q20: Arthur's Parting Words (QuestId=20, NPCId=6=Arthur) ──
                new NPCDialogue { NPCDialogueId = 49, NPCId = 6, LinkedQuestId = 20, ResponseType = "None", Content = "The dragon is dead. I felt it go — the whole city breathed out at once. Thank you.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 50, NPCId = 6, LinkedQuestId = 20, ResponseType = "None", Content = "You want to know about the cloaked one. Yes. He passed through here before the dragon ever came.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 110, NPCId = 6, LinkedQuestId = 20, ResponseType = "None", Content = "He carries something that should have stayed sealed. Wherever he walks, the land sickens behind him.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 51, NPCId = 6, LinkedQuestId = 20, ResponseType = "Quest", Content = "He went north, into the frozen lands. Follow him to the Frozen Mountains. I will hold this city.", DisplayOrder = 4, IsActive = true },
                // ── Q21: Slimes of the Snow Fields (QuestId=21, NPCId=8=Roselyn Aurora Queen) ──
                new NPCDialogue { NPCDialogueId = 52, NPCId = 8, LinkedQuestId = 21, ResponseType = "None", Content = "A living stranger, walking in out of the snow. I am Roselyn Aurora, and what is left of this kingdom is mine to hold.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 53, NPCId = 8, LinkedQuestId = 21, ResponseType = "None", Content = "The codex passed over these fields and the cold turned wrong. My soldiers are gone. Only volunteers stand the walls now.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 111, NPCId = 8, LinkedQuestId = 21, ResponseType = "None", Content = "The slimes come closer each night. They freeze whatever they touch, and my people cannot reach the grain stores.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 54, NPCId = 8, LinkedQuestId = 21, ResponseType = "Quest", Content = "Clear 8 ice slimes from the Snow Fields. I would ask a knight, but I have none left to ask.", DisplayOrder = 4, IsActive = true },
                // ── Q22: Deliver the Magic Flour (QuestId=22, NPCId=8=Queen + NPCId=9=Zephyr) ──
                new NPCDialogue { NPCDialogueId = 55, NPCId = 8, LinkedQuestId = 22, ResponseType = "None", Content = "The fields are quiet tonight. My people walked to the grain stores without an escort for the first time in a month.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 56, NPCId = 8, LinkedQuestId = 22, ResponseType = "None", Content = "Now the harder trouble. Priest Zephyr keeps a rite burning at the peak — it is the only thing holding the cold back.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 112, NPCId = 8, LinkedQuestId = 22, ResponseType = "None", Content = "His supplies ran out days ago and no courier of mine has come back down that road alive.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 57, NPCId = 8, LinkedQuestId = 22, ResponseType = "Quest", Content = "Take this Magic Flour to him at the mountain peak. If the rite goes out, we all freeze with it.", DisplayOrder = 4, IsActive = true },
                new NPCDialogue { NPCDialogueId = 58, NPCId = 9, LinkedQuestId = 22, ResponseType = "Reward", Content = "The Queen's flour, and a courier still breathing. The rite can go on. You have bought this mountain another season.", DisplayOrder = 1, IsActive = true },
                // ── Q23: Dragons of the Frozen Peak (QuestId=23, NPCId=9=Zephyr) ──
                new NPCDialogue { NPCDialogueId = 59, NPCId = 9, LinkedQuestId = 23, ResponseType = "None", Content = "Stay a moment. The codex passed over this peak too, and what it touched did not simply die — it changed.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 89, NPCId = 9, LinkedQuestId = 23, ResponseType = "None", Content = "I cannot hold the rites while they circle the shrine — their shadows alone put out the candles.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 113, NPCId = 9, LinkedQuestId = 23, ResponseType = "None", Content = "Ice dragons, five of them. Young, but the codex made them hungry in a way no beast should be.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 60, NPCId = 9, LinkedQuestId = 23, ResponseType = "Quest", Content = "Climb the peak and slay all 5. Do it, and the Queen's borders hold one more winter.", DisplayOrder = 4, IsActive = true },
                // ── Q24: The Warden of the Ban (QuestId=24, NPCId=10=Roland) ──
                new NPCDialogue { NPCDialogueId = 61, NPCId = 10, LinkedQuestId = 24, ResponseType = "None", Content = "Halt. Beyond this line is under ban, and I am Roland, the warden who keeps it.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 62, NPCId = 10, LinkedQuestId = 24, ResponseType = "None", Content = "Wait. That cold on you — dragon frost. You came down off the peak, not up from the road.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 114, NPCId = 10, LinkedQuestId = 24, ResponseType = "None", Content = "I have watched this ban for eleven years and never once set foot inside. Now something in there has begun to stir.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 63, NPCId = 10, LinkedQuestId = 24, ResponseType = "Quest", Content = "Walk it with me and map what waits inside. I will not send you where I do not go myself.", DisplayOrder = 4, IsActive = true },
                // ── Q25: Break the Stone Guardian (QuestId=25, NPCId=10=Roland) ──
                new NPCDialogue { NPCDialogueId = 64, NPCId = 10, LinkedQuestId = 25, ResponseType = "None", Content = "Now I know why my order was told to guard this place and never enter it. The codex did not begin in the world. It began here.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 65, NPCId = 10, LinkedQuestId = 25, ResponseType = "None", Content = "And it is not finished. One of the old Seal Books lies at the heart of the ban, still holding.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 115, NPCId = 10, LinkedQuestId = 25, ResponseType = "None", Content = "A stone golem stands over it. The elders left it there to keep hands off the book — mine included.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 66, NPCId = 10, LinkedQuestId = 25, ResponseType = "Quest", Content = "Break the stone guardian and take the Golem Seal Book. It is worth more in your hands than under my ban.", DisplayOrder = 4, IsActive = true },
                // ── Q26: Break the Skeleton Army (QuestId=26, NPCId=11=Valiant Warrior) ──
                new NPCDialogue { NPCDialogueId = 67, NPCId = 11, LinkedQuestId = 26, ResponseType = "None", Content = "Back, stranger, keep your back to the rock! They come up out of the valley floor faster than I can cut them down.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 68, NPCId = 11, LinkedQuestId = 26, ResponseType = "None", Content = "This is no ordinary haunting. An ancient power is leaking somewhere near, and the dead rise faster than they fall.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 116, NPCId = 11, LinkedQuestId = 26, ResponseType = "None", Content = "There is a Seal Book buried under all this bone. I have felt it since the day the leak began.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 69, NPCId = 11, LinkedQuestId = 26, ResponseType = "Quest", Content = "Cut down 12 of them in the valley with me. Two blades may be enough where one was not.", DisplayOrder = 4, IsActive = true },
                new NPCDialogue { NPCDialogueId = 70, NPCId = 11, LinkedQuestId = 26, ResponseType = "Reward", Content = "The animals are fleeing from the abandoned village of Tide-Knell. Look into it, and find the girl Natalie.", DisplayOrder = 5, IsActive = true },
                // ── Q27: The Skull by the Well (QuestId=27, NPCId=12=Natalie) ──
                new NPCDialogue { NPCDialogueId = 71, NPCId = 12, LinkedQuestId = 27, ResponseType = "None", Content = "You can see me. Nobody has seen me in a very long time. My name is Natalie, and this village is Tide-Knell.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 117, NPCId = 12, LinkedQuestId = 27, ResponseType = "None", Content = "I cannot leave the well. I have tried. Something of me is still down in that ground, and it holds me here.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 118, NPCId = 12, LinkedQuestId = 27, ResponseType = "None", Content = "The animals knew before you did. That is why they ran. They will not drink from a well with a girl in it.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 72, NPCId = 12, LinkedQuestId = 27, ResponseType = "Quest", Content = "Please. Dig beside the old well and lift out the skull you find there. I am ready to be found.", DisplayOrder = 4, IsActive = true },
                // ── Q28: Lay Natalie to Rest (QuestId=28, NPCId=12=Natalie) ──
                new NPCDialogue { NPCDialogueId = 73, NPCId = 12, LinkedQuestId = 28, ResponseType = "None", Content = "(A weathered letter lies where Natalie once stood. It is her own hand, and it is a farewell.)", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 119, NPCId = 12, LinkedQuestId = 28, ResponseType = "None", Content = "(She writes of a book she opened as a child, of a seal she did not understand, and of the day the valley began to fill with bone.)", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 74, NPCId = 12, LinkedQuestId = 28, ResponseType = "None", Content = "Thank you for bringing my remains home. Please bury me under the ivy tree in my courtyard, where I used to sit.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 75, NPCId = 12, LinkedQuestId = 28, ResponseType = "Quest", Content = "The ancient power leak was my doing, and I have paid for it here. Take this Mystic Key — it opens the castle gates on the deserted island.", DisplayOrder = 4, IsActive = true },
                // ── Q29: Ancient Leaves of the Isle (QuestId=29, NPCId=13=Elf Guard) ──
                new NPCDialogue { NPCDialogueId = 76, NPCId = 13, LinkedQuestId = 29, ResponseType = "None", Content = "An outsider, with a Mystic Key, standing on my island. The sea should have kept you. Yet here you are.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 77, NPCId = 13, LinkedQuestId = 29, ResponseType = "None", Content = "I am the last guard of this place. I know what you carry, and I know the forest you are trying to reach.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 120, NPCId = 13, LinkedQuestId = 29, ResponseType = "None", Content = "A portal home cannot be forced. It must be grown, and for that the rite needs leaves older than the curse itself.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 78, NPCId = 13, LinkedQuestId = 29, ResponseType = "Quest", Content = "Collect 5 Ancient Leaves from the Northern Plateau. Bring them, and I will begin the rite of return.", DisplayOrder = 4, IsActive = true },
                // ── Q30: Defeat the UnderKing (QuestId=30, NPCId=13=Elf Guard) ──
                new NPCDialogue { NPCDialogueId = 79, NPCId = 13, LinkedQuestId = 30, ResponseType = "None", Content = "The leaves are enough. The rite is ready. And yet I cannot light it — something below the castle is smothering it.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 80, NPCId = 13, LinkedQuestId = 30, ResponseType = "None", Content = "The UnderKing has woken. He held the last Seal Book in his hands long before you were born.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 121, NPCId = 13, LinkedQuestId = 30, ResponseType = "None", Content = "Three seals you have already. Without his, the Origin Tree cannot be cleansed and the forest ends with the tree.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 81, NPCId = 13, LinkedQuestId = 30, ResponseType = "Quest", Content = "End his reign. Defeat the UnderKing and take the fourth book from him.", DisplayOrder = 4, IsActive = true },
                // ── Q31: Ask for the Way Home (QuestId=31, NPCId=13=Elf Guard) ──
                new NPCDialogue { NPCDialogueId = 82, NPCId = 13, LinkedQuestId = 31, ResponseType = "None", Content = "It is done. The UnderKing has fallen, and all four Seal Books are in one pair of hands for the first time in an age.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 83, NPCId = 13, LinkedQuestId = 31, ResponseType = "None", Content = "You want the way home. I will give it, but understand what waits: the Origin Tree is nearly gone.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 122, NPCId = 13, LinkedQuestId = 31, ResponseType = "None", Content = "The rite will open once and close behind you. Whatever you leave undone on this side stays undone.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 84, NPCId = 13, LinkedQuestId = 31, ResponseType = "Reward", Content = "Then go. The portal to the Elf Forest is open. Save the tree, outsider.", DisplayOrder = 4, IsActive = true },
                // ── Q32: Return with the Seals (QuestId=32, NPCId=2=Lyra) ──
                new NPCDialogue { NPCDialogueId = 85, NPCId = 2, LinkedQuestId = 32, ResponseType = "None", Content = "You came back. Through the ruins, the snow, the ban, the sea — and you are carrying all four seals.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 90, NPCId = 2, LinkedQuestId = 32, ResponseType = "None", Content = "The tree has almost no strength left. Every leaf it drops, the curse takes a little more of the forest.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 123, NPCId = 2, LinkedQuestId = 32, ResponseType = "None", Content = "Four books, four elders, four bindings broken. Set them together and the curse has nowhere left to hide.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 86, NPCId = 2, LinkedQuestId = 32, ResponseType = "Quest", Content = "Bring the four books to me here, at the roots. Hurry.", DisplayOrder = 4, IsActive = true },
                // ── Q33: Heal the Origin Tree (QuestId=33, NPCId=2=Lyra) ──
                new NPCDialogue { NPCDialogueId = 91, NPCId = 2, LinkedQuestId = 33, ResponseType = "None", Content = "The four seals are whole. I have opened the rite... but I cannot finish it.", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 92, NPCId = 2, LinkedQuestId = 33, ResponseType = "None", Content = "The seals answer only to the one who won them. It must be your hand, not mine.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 124, NPCId = 2, LinkedQuestId = 33, ResponseType = "None", Content = "I am the tree's spirit. If the curse takes the roots, it takes me with them — so do not hesitate at the last step.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 93, NPCId = 2, LinkedQuestId = 33, ResponseType = "Quest", Content = "Step to the Origin Tree and set the four Seal Books upon it. Break the curse.", DisplayOrder = 4, IsActive = true },
                // ── Q34: A New Dawn (QuestId=34, NPCId=2=Lyra) ──
                new NPCDialogue { NPCDialogueId = 87, NPCId = 2, LinkedQuestId = 34, ResponseType = "None", Content = "The curse is breaking... The Origin Tree is finally healing!", DisplayOrder = 1, IsActive = true },
                new NPCDialogue { NPCDialogueId = 125, NPCId = 2, LinkedQuestId = 34, ResponseType = "None", Content = "Look at the roots. Green, after all this time. The forest will remember the one who stood here today.", DisplayOrder = 2, IsActive = true },
                new NPCDialogue { NPCDialogueId = 126, NPCId = 2, LinkedQuestId = 34, ResponseType = "None", Content = "And yet the cloaked one was never found, and no one has said who broke the four bindings in the first place.", DisplayOrder = 3, IsActive = true },
                new NPCDialogue { NPCDialogueId = 88, NPCId = 2, LinkedQuestId = 34, ResponseType = "Reward", Content = "Thank you, truly. The Origin Tree is saved. But this is not the end... To be continued.", DisplayOrder = 4, IsActive = true }
            );
        }
    }
}
