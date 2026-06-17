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
        public DbSet<Item> Items => Set<Item>();
        public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
        public DbSet<EquipmentStats> EquipmentStats => Set<EquipmentStats>();
        public DbSet<Skill> Skills => Set<Skill>();
        public DbSet<PlayerSkill> PlayerSkills => Set<PlayerSkill>();
        public DbSet<Quest> Quests => Set<Quest>();
        public DbSet<PlayerQuest> PlayerQuests => Set<PlayerQuest>();
        public DbSet<NPC> NPCs => Set<NPC>();
        public DbSet<NPCDialogue> NPCDialogues => Set<NPCDialogue>();
        public DbSet<Monster> Monsters => Set<Monster>();
        public DbSet<PurchaseHistory> PurchaseHistories => Set<PurchaseHistory>();
        public DbSet<PlayerCurrencyLog> PlayerCurrencyLogs => Set<PlayerCurrencyLog>();
        public DbSet<ShopItem> ShopItems => Set<ShopItem>();
        public DbSet<Mail> Mails => Set<Mail>();
        public DbSet<Friend> Friends => Set<Friend>();
        public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
        public DbSet<MonsterDrop> MonsterDrops => Set<MonsterDrop>();
        public DbSet<GachaBanner> GachaBanners => Set<GachaBanner>();
        public DbSet<GachaBannerItem> GachaBannerItems => Set<GachaBannerItem>();
        public DbSet<GachaPullHistory> GachaPullHistories => Set<GachaPullHistory>();
        public DbSet<DungeonConfig> DungeonConfigs => Set<DungeonConfig>();
        public DbSet<Achievement> Achievements => Set<Achievement>();
        public DbSet<PlayerAchievement> PlayerAchievements => Set<PlayerAchievement>();
        public DbSet<Guild> Guilds => Set<Guild>();
        public DbSet<GuildMember> GuildMembers => Set<GuildMember>();
        public DbSet<GuildInvitation> GuildInvitations => Set<GuildInvitation>();
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
        public DbSet<Content> Contents => Set<Content>();
        public DbSet<BlockContent> BlockContents => Set<BlockContent>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        modelBuilder.Entity<Role>().HasData(
        new Role { RoleId = 1, Name = "Player" },
        new Role { RoleId = 2, Name = "Admin" },
        new Role { RoleId = 3, Name = "SuperAdmin" }
    );
        modelBuilder.Entity<PlayerStatsSnapshot>()
            .HasIndex(s => s.PlayerProfileId)
            .IsUnique();
        modelBuilder.Entity<Quest>()
            .HasIndex(q => q.MapName);
        modelBuilder.Entity<NPC>()
            .HasIndex(n => n.MapName);
        }
    }
}
