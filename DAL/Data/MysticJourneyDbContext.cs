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
        public DbSet<PlayerProfile> PlayerProfiles => Set<PlayerProfile>();
        public DbSet<PlayerStat> PlayerStats => Set<PlayerStat>();
        public DbSet<Item> Items => Set<Item>();
        public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
        public DbSet<EquipmentStats> EquipmentStats => Set<EquipmentStats>();
        public DbSet<Skill> Skills => Set<Skill>();
        public DbSet<PlayerSkill> PlayerSkills => Set<PlayerSkill>();
        public DbSet<Quest> Quests => Set<Quest>();
        public DbSet<PlayerQuest> PlayerQuests => Set<PlayerQuest>();
        public DbSet<Monster> Monsters => Set<Monster>();
        public DbSet<PurchaseHistory> PurchaseHistories => Set<PurchaseHistory>();
        public DbSet<PlayerCurrencyLog> PlayerCurrencyLogs => Set<PlayerCurrencyLog>();
        public DbSet<ShopItem> ShopItems => Set<ShopItem>();
        public DbSet<Mail> Mails => Set<Mail>();
        public DbSet<Friend> Friends => Set<Friend>();
        public DbSet<Boss> Bosses => Set<Boss>();
        public DbSet<GachaBanner> GachaBanners => Set<GachaBanner>();
        public DbSet<GachaBannerItem> GachaBannerItems => Set<GachaBannerItem>();
        public DbSet<GachaPullHistory> GachaPullHistories => Set<GachaPullHistory>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>()
                .HasOne(a => a.PlayerProfile)
                .WithOne(p => p.Account)
                .HasForeignKey<PlayerProfile>(p => p.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlayerProfile>()
                .HasIndex(p => p.AccountId)
                .IsUnique();

            modelBuilder.Entity<PlayerProfile>()
                .HasOne<PlayerStat>(p => p.PlayerStats)
                .WithOne(s => s.PlayerProfile)
                .HasForeignKey<PlayerStat>(s => s.PlayerProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlayerStat>()
                .HasIndex(s => s.PlayerProfileId)
                .IsUnique();

            modelBuilder.Entity<InventoryItem>()
                .HasOne(i => i.PlayerProfile)
                .WithMany(p => p.InventoryItems)
                .HasForeignKey(i => i.PlayerProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<InventoryItem>()
                .HasOne(i => i.Item)
                .WithMany(item => item.InventoryItems)
                .HasForeignKey(i => i.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EquipmentStats>()
                .HasOne(e => e.Item)
                .WithOne(item => item.EquipmentStats)
                .HasForeignKey<EquipmentStats>(e => e.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EquipmentStats>()
                .HasIndex(e => e.ItemId)
                .IsUnique();

            modelBuilder.Entity<PlayerSkill>()
                .HasOne(ps => ps.PlayerProfile)
                .WithMany(p => p.PlayerSkills)
                .HasForeignKey(ps => ps.PlayerProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlayerSkill>()
                .HasOne(ps => ps.Skill)
                .WithMany(s => s.PlayerSkills)
                .HasForeignKey(ps => ps.SkillId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PlayerQuest>()
                .HasOne(pq => pq.PlayerProfile)
                .WithMany(p => p.PlayerQuests)
                .HasForeignKey(pq => pq.PlayerProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlayerQuest>()
                .HasOne(pq => pq.Quest)
                .WithMany(q => q.PlayerQuests)
                .HasForeignKey(pq => pq.QuestId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Quest>()
                .HasOne(q => q.RewardItem)
                .WithMany()
                .HasForeignKey(q => q.RewardItemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PlayerCurrencyLog>()
                .HasOne(l => l.PlayerProfile)
                .WithMany()
                .HasForeignKey(l => l.PlayerProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Mail>()
                .HasOne(m => m.PlayerProfile)
                .WithMany(p => p.Mails)
                .HasForeignKey(m => m.PlayerProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Mail>()
                .HasOne(m => m.AttachedItem)
                .WithMany()
                .HasForeignKey(m => m.AttachedItemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseHistory>()
                .HasOne(ph => ph.PlayerProfile)
                .WithMany()
                .HasForeignKey(ph => ph.PlayerProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PurchaseHistory>()
                .HasOne(ph => ph.ShopItem)
                .WithMany(si => si.PurchaseHistories)
                .HasForeignKey(ph => ph.ShopItemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ShopItem>()
                .HasOne(si => si.Item)
                .WithMany(item => item.ShopItems)
                .HasForeignKey(si => si.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GachaBannerItem>()
                .HasOne(gbi => gbi.GachaBanner)
                .WithMany(gb => gb.BannerItems)
                .HasForeignKey(gbi => gbi.GachaBannerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GachaBannerItem>()
                .HasOne(gbi => gbi.Item)
                .WithMany()
                .HasForeignKey(gbi => gbi.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GachaPullHistory>()
                .HasOne(gph => gph.PlayerProfile)
                .WithMany()
                .HasForeignKey(gph => gph.PlayerProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GachaPullHistory>()
                .HasOne(gph => gph.GachaBanner)
                .WithMany(gb => gb.PullHistories)
                .HasForeignKey(gph => gph.GachaBannerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GachaPullHistory>()
                .HasOne(gph => gph.RewardItem)
                .WithMany()
                .HasForeignKey(gph => gph.RewardItemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Friend>()
                .HasOne(f => f.Requester)
                .WithMany()
                .HasForeignKey(f => f.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Friend>()
                .HasOne(f => f.Addressee)
                .WithMany()
                .HasForeignKey(f => f.AddresseeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
