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
                new ClassConfig { ClassConfigId = 1, ClassName = "Knight", MaxHp = 500, Atk = 30, Def = 40, MoveSpeed = 100, AttackSpeed = 100, CritRate = 5, CritDamage = 150, DamageBonus = 0 },
                new ClassConfig { ClassConfigId = 2, ClassName = "Archer", MaxHp = 350, Atk = 40, Def = 20, MoveSpeed = 100, AttackSpeed = 100, CritRate = 5, CritDamage = 150, DamageBonus = 0 },
                new ClassConfig { ClassConfigId = 3, ClassName = "Mage", MaxHp = 300, Atk = 50, Def = 15, MoveSpeed = 100, AttackSpeed = 100, CritRate = 5, CritDamage = 150, DamageBonus = 0 }
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
        }
    }
}
