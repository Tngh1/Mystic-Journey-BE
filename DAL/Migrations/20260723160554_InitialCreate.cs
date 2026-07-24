using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoryContents",
                columns: table => new
                {
                    CategoryContentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IconUrl = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryContents", x => x.CategoryContentId);
                });

            migrationBuilder.CreateTable(
                name: "Chests",
                columns: table => new
                {
                    ChestId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: false),
                    GoldMinReward = table.Column<int>(type: "integer", nullable: false),
                    GoldMaxReward = table.Column<int>(type: "integer", nullable: false),
                    ExperienceReward = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chests", x => x.ChestId);
                });

            migrationBuilder.CreateTable(
                name: "ClassConfigs",
                columns: table => new
                {
                    ClassConfigId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClassName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MaxHp = table.Column<int>(type: "integer", nullable: false),
                    Atk = table.Column<int>(type: "integer", nullable: false),
                    Def = table.Column<int>(type: "integer", nullable: false),
                    MoveSpeed = table.Column<int>(type: "integer", nullable: false),
                    AttackSpeed = table.Column<int>(type: "integer", nullable: false),
                    CritRate = table.Column<int>(type: "integer", nullable: false),
                    CritDamage = table.Column<int>(type: "integer", nullable: false),
                    DamageBonus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassConfigs", x => x.ClassConfigId);
                });

            migrationBuilder.CreateTable(
                name: "Dungeons",
                columns: table => new
                {
                    DungeonId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsRepeatable = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dungeons", x => x.DungeonId);
                });

            migrationBuilder.CreateTable(
                name: "GachaBanners",
                columns: table => new
                {
                    GachaBannerId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    PullCost = table.Column<int>(type: "integer", nullable: false),
                    CostItemId = table.Column<int>(type: "integer", nullable: true),
                    PityLimit = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    StartAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GachaBanners", x => x.GachaBannerId);
                });

            migrationBuilder.CreateTable(
                name: "GameAnnouncements",
                columns: table => new
                {
                    GameAnnouncementId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    StartsAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndsAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameAnnouncements", x => x.GameAnnouncementId);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Rarity = table.Column<string>(type: "text", nullable: false),
                    Slot = table.Column<string>(type: "text", nullable: false),
                    BaseValue = table.Column<decimal>(type: "numeric", nullable: false),
                    CorruptionReduction = table.Column<float>(type: "real", nullable: false),
                    MaxStack = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IconUrl = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.ItemId);
                });

            migrationBuilder.CreateTable(
                name: "Monsters",
                columns: table => new
                {
                    MonsterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    MaxHp = table.Column<int>(type: "integer", nullable: false),
                    Atk = table.Column<int>(type: "integer", nullable: false),
                    Def = table.Column<int>(type: "integer", nullable: false),
                    MoveSpeed = table.Column<int>(type: "integer", nullable: false),
                    AttackSpeed = table.Column<int>(type: "integer", nullable: false),
                    CritRate = table.Column<int>(type: "integer", nullable: false),
                    CritDamage = table.Column<int>(type: "integer", nullable: false),
                    ExperienceReward = table.Column<int>(type: "integer", nullable: false),
                    GoldReward = table.Column<decimal>(type: "numeric", nullable: false),
                    GemReward = table.Column<decimal>(type: "numeric", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monsters", x => x.MonsterId);
                });

            migrationBuilder.CreateTable(
                name: "NPCs",
                columns: table => new
                {
                    NPCId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: false),
                    MapName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PositionX = table.Column<double>(type: "double precision", nullable: false),
                    PositionY = table.Column<double>(type: "double precision", nullable: false),
                    InteractionRadius = table.Column<float>(type: "real", nullable: false),
                    IconUrl = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NPCs", x => x.NPCId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    SkillId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: false),
                    DamageType = table.Column<string>(type: "text", nullable: false),
                    TargetType = table.Column<string>(type: "text", nullable: false),
                    ClassRequirement = table.Column<string>(type: "text", nullable: false),
                    CooldownSeconds = table.Column<int>(type: "integer", nullable: false),
                    BaseDamage = table.Column<double>(type: "double precision", nullable: false),
                    DamagePerLevel = table.Column<double>(type: "double precision", nullable: false),
                    DamageGrowthPercent = table.Column<double>(type: "double precision", nullable: false),
                    UnlockLevel = table.Column<int>(type: "integer", nullable: false),
                    CorruptionCost = table.Column<float>(type: "real", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.SkillId);
                });

            migrationBuilder.CreateTable(
                name: "Skins",
                columns: table => new
                {
                    SkinId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Rarity = table.Column<string>(type: "text", nullable: false),
                    IconUrl = table.Column<string>(type: "text", nullable: true),
                    PreviewUrl = table.Column<string>(type: "text", nullable: true),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    IsForSale = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skins", x => x.SkinId);
                });

            migrationBuilder.CreateTable(
                name: "SubCategoryContents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Slug = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IconUrl = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CategoryContentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubCategoryContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubCategoryContents_CategoryContents_CategoryContentId",
                        column: x => x.CategoryContentId,
                        principalTable: "CategoryContents",
                        principalColumn: "CategoryContentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DungeonConfigs",
                columns: table => new
                {
                    DungeonConfigId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: false),
                    LevelRequirement = table.Column<int>(type: "integer", nullable: false),
                    MaxMembers = table.Column<int>(type: "integer", nullable: false),
                    Difficulty = table.Column<int>(type: "integer", nullable: false),
                    EnergyCost = table.Column<int>(type: "integer", nullable: false),
                    RecommendedPower = table.Column<int>(type: "integer", nullable: false),
                    ChestId = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DungeonConfigs", x => x.DungeonConfigId);
                    table.ForeignKey(
                        name: "FK_DungeonConfigs_Chests_ChestId",
                        column: x => x.ChestId,
                        principalTable: "Chests",
                        principalColumn: "ChestId");
                });

            migrationBuilder.CreateTable(
                name: "Achievements",
                columns: table => new
                {
                    AchievementId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: false),
                    IconUrl = table.Column<string>(type: "text", nullable: true),
                    BuffDescription = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    RequiredValue = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RewardItemId = table.Column<int>(type: "integer", nullable: true),
                    RewardQuantity = table.Column<int>(type: "integer", nullable: false),
                    RewardGold = table.Column<decimal>(type: "numeric", nullable: false),
                    RewardGem = table.Column<int>(type: "integer", nullable: false),
                    Point = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Achievements", x => x.AchievementId);
                    table.ForeignKey(
                        name: "FK_Achievements_Items_RewardItemId",
                        column: x => x.RewardItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId");
                });

            migrationBuilder.CreateTable(
                name: "ChestItems",
                columns: table => new
                {
                    ChestItemId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ChestId = table.Column<int>(type: "integer", nullable: false),
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    QuantityMin = table.Column<int>(type: "integer", nullable: false),
                    QuantityMax = table.Column<int>(type: "integer", nullable: false),
                    DropRate = table.Column<decimal>(type: "numeric", nullable: false),
                    IsGuaranteed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChestItems", x => x.ChestItemId);
                    table.ForeignKey(
                        name: "FK_ChestItems_Chests_ChestId",
                        column: x => x.ChestId,
                        principalTable: "Chests",
                        principalColumn: "ChestId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChestItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DailyLoginRewards",
                columns: table => new
                {
                    DailyLoginRewardId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DayNumber = table.Column<int>(type: "integer", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: true),
                    RewardType = table.Column<string>(type: "text", nullable: false),
                    RewardValue = table.Column<decimal>(type: "numeric", nullable: false),
                    RewardItemId = table.Column<int>(type: "integer", nullable: true),
                    RewardItemQuantity = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyLoginRewards", x => x.DailyLoginRewardId);
                    table.ForeignKey(
                        name: "FK_DailyLoginRewards_Items_RewardItemId",
                        column: x => x.RewardItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId");
                });

            migrationBuilder.CreateTable(
                name: "EquipmentStats",
                columns: table => new
                {
                    EquipmentStatsId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    BaseHp = table.Column<int>(type: "integer", nullable: false),
                    BaseAtk = table.Column<int>(type: "integer", nullable: false),
                    BaseDef = table.Column<int>(type: "integer", nullable: false),
                    BonusHp = table.Column<int>(type: "integer", nullable: false),
                    BonusAtk = table.Column<int>(type: "integer", nullable: false),
                    BonusDef = table.Column<int>(type: "integer", nullable: false),
                    BonusMoveSpeed = table.Column<int>(type: "integer", nullable: false),
                    BonusAttackSpeed = table.Column<int>(type: "integer", nullable: false),
                    BonusCritRate = table.Column<int>(type: "integer", nullable: false),
                    BonusCritDamage = table.Column<int>(type: "integer", nullable: false),
                    BonusDamageBonus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentStats", x => x.EquipmentStatsId);
                    table.ForeignKey(
                        name: "FK_EquipmentStats_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GachaBannerItems",
                columns: table => new
                {
                    GachaBannerItemId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GachaBannerId = table.Column<int>(type: "integer", nullable: false),
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    DropRate = table.Column<decimal>(type: "numeric", nullable: false),
                    IsFeatured = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GachaBannerItems", x => x.GachaBannerItemId);
                    table.ForeignKey(
                        name: "FK_GachaBannerItems_GachaBanners_GachaBannerId",
                        column: x => x.GachaBannerId,
                        principalTable: "GachaBanners",
                        principalColumn: "GachaBannerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GachaBannerItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShopItems",
                columns: table => new
                {
                    ShopItemId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    ShopSection = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValue: "Fixed"),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Stock = table.Column<int>(type: "integer", nullable: false),
                    DailyPurchaseLimit = table.Column<int>(type: "integer", nullable: false),
                    WeeklyPurchaseLimit = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    AvailableFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AvailableTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopItems", x => x.ShopItemId);
                    table.ForeignKey(
                        name: "FK_ShopItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MonsterDrops",
                columns: table => new
                {
                    MonsterDropId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MonsterId = table.Column<int>(type: "integer", nullable: false),
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    DropRate = table.Column<double>(type: "double precision", nullable: false),
                    MinQuantity = table.Column<int>(type: "integer", nullable: false),
                    MaxQuantity = table.Column<int>(type: "integer", nullable: false),
                    IsGuaranteed = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonsterDrops", x => x.MonsterDropId);
                    table.ForeignKey(
                        name: "FK_MonsterDrops_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MonsterDrops_Monsters_MonsterId",
                        column: x => x.MonsterId,
                        principalTable: "Monsters",
                        principalColumn: "MonsterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MonsterSpawns",
                columns: table => new
                {
                    MonsterSpawnId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MonsterId = table.Column<int>(type: "integer", nullable: false),
                    MapName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RegionName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Location = table.Column<string>(type: "text", nullable: true),
                    SpawnCount = table.Column<int>(type: "integer", nullable: false),
                    RespawnSeconds = table.Column<int>(type: "integer", nullable: false),
                    DungeonId = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonsterSpawns", x => x.MonsterSpawnId);
                    table.ForeignKey(
                        name: "FK_MonsterSpawns_Dungeons_DungeonId",
                        column: x => x.DungeonId,
                        principalTable: "Dungeons",
                        principalColumn: "DungeonId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MonsterSpawns_Monsters_MonsterId",
                        column: x => x.MonsterId,
                        principalTable: "Monsters",
                        principalColumn: "MonsterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    AccountId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    HashPassword = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: true),
                    RefreshTokenExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastLogin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastSeen = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.AccountId);
                    table.ForeignKey(
                        name: "FK_Accounts_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Quests",
                columns: table => new
                {
                    QuestId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: false),
                    DefaultStatus = table.Column<string>(type: "text", nullable: false),
                    MapName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RegionName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ObjectiveType = table.Column<string>(type: "text", nullable: false),
                    ObjectiveTarget = table.Column<string>(type: "text", nullable: true),
                    ObjectiveLocation = table.Column<string>(type: "text", nullable: true),
                    QuestGiverName = table.Column<string>(type: "text", nullable: true),
                    RequiredLevel = table.Column<int>(type: "integer", nullable: false),
                    TargetAmount = table.Column<int>(type: "integer", nullable: false),
                    RewardExperience = table.Column<int>(type: "integer", nullable: false),
                    RewardGold = table.Column<decimal>(type: "numeric", nullable: false),
                    RewardGems = table.Column<decimal>(type: "numeric", nullable: false),
                    RewardItemId = table.Column<int>(type: "integer", nullable: true),
                    RewardSkillId = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    BossMonsterId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quests", x => x.QuestId);
                    table.ForeignKey(
                        name: "FK_Quests_Items_RewardItemId",
                        column: x => x.RewardItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId");
                    table.ForeignKey(
                        name: "FK_Quests_Monsters_BossMonsterId",
                        column: x => x.BossMonsterId,
                        principalTable: "Monsters",
                        principalColumn: "MonsterId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Quests_Skills_RewardSkillId",
                        column: x => x.RewardSkillId,
                        principalTable: "Skills",
                        principalColumn: "SkillId");
                });

            migrationBuilder.CreateTable(
                name: "Contents",
                columns: table => new
                {
                    ContentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Slug = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Summary = table.Column<string>(type: "text", nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "text", nullable: true),
                    CategoryContentId = table.Column<int>(type: "integer", nullable: true),
                    SubCategoryContentId = table.Column<int>(type: "integer", nullable: true),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PublishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByAccountAccountId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contents", x => x.ContentId);
                    table.ForeignKey(
                        name: "FK_Contents_Accounts_CreatedByAccountAccountId",
                        column: x => x.CreatedByAccountAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId");
                    table.ForeignKey(
                        name: "FK_Contents_CategoryContents_CategoryContentId",
                        column: x => x.CategoryContentId,
                        principalTable: "CategoryContents",
                        principalColumn: "CategoryContentId");
                    table.ForeignKey(
                        name: "FK_Contents_SubCategoryContents_SubCategoryContentId",
                        column: x => x.SubCategoryContentId,
                        principalTable: "SubCategoryContents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GameSettings",
                columns: table => new
                {
                    GameSettingId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByAccountId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByAccountAccountId = table.Column<int>(type: "integer", nullable: true),
                    UpdatedByAccountId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedByAccountAccountId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSettings", x => x.GameSettingId);
                    table.ForeignKey(
                        name: "FK_GameSettings_Accounts_CreatedByAccountAccountId",
                        column: x => x.CreatedByAccountAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId");
                    table.ForeignKey(
                        name: "FK_GameSettings_Accounts_UpdatedByAccountAccountId",
                        column: x => x.UpdatedByAccountAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId");
                });

            migrationBuilder.CreateTable(
                name: "PlayerProfiles",
                columns: table => new
                {
                    PlayerProfileId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AccountId = table.Column<int>(type: "integer", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AvatarUrl = table.Column<string>(type: "text", nullable: false),
                    HasChangedName = table.Column<bool>(type: "boolean", nullable: false),
                    Class = table.Column<string>(type: "text", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    ExperiencePoints = table.Column<int>(type: "integer", nullable: false),
                    AvailableStatPoints = table.Column<int>(type: "integer", nullable: false),
                    CachedStatRolls = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Gold = table.Column<decimal>(type: "numeric", nullable: false),
                    Gems = table.Column<decimal>(type: "numeric", nullable: false),
                    CurrentEnergy = table.Column<int>(type: "integer", nullable: false),
                    MaxEnergy = table.Column<int>(type: "integer", nullable: false),
                    LastEnergyUpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastFreeGachaTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastActiveTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastLeaveAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TotalDungeonClears = table.Column<int>(type: "integer", nullable: false),
                    LastMapName = table.Column<string>(type: "text", nullable: false),
                    PositionX = table.Column<double>(type: "double precision", nullable: false),
                    PositionY = table.Column<double>(type: "double precision", nullable: false),
                    CorruptionLevel = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerProfiles", x => x.PlayerProfileId);
                    table.ForeignKey(
                        name: "FK_PlayerProfiles_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NPCDialogues",
                columns: table => new
                {
                    NPCDialogueId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NPCId = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    ResponseType = table.Column<string>(type: "text", nullable: false),
                    LinkedQuestId = table.Column<int>(type: "integer", nullable: true),
                    LinkedShopItemId = table.Column<int>(type: "integer", nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NPCDialogues", x => x.NPCDialogueId);
                    table.ForeignKey(
                        name: "FK_NPCDialogues_NPCs_NPCId",
                        column: x => x.NPCId,
                        principalTable: "NPCs",
                        principalColumn: "NPCId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NPCDialogues_Quests_LinkedQuestId",
                        column: x => x.LinkedQuestId,
                        principalTable: "Quests",
                        principalColumn: "QuestId");
                    table.ForeignKey(
                        name: "FK_NPCDialogues_ShopItems_LinkedShopItemId",
                        column: x => x.LinkedShopItemId,
                        principalTable: "ShopItems",
                        principalColumn: "ShopItemId");
                });

            migrationBuilder.CreateTable(
                name: "QuestRewardItems",
                columns: table => new
                {
                    QuestRewardItemId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QuestId = table.Column<int>(type: "integer", nullable: false),
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestRewardItems", x => x.QuestRewardItemId);
                    table.ForeignKey(
                        name: "FK_QuestRewardItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestRewardItems_Quests_QuestId",
                        column: x => x.QuestId,
                        principalTable: "Quests",
                        principalColumn: "QuestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestRewardSkills",
                columns: table => new
                {
                    QuestRewardSkillId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QuestId = table.Column<int>(type: "integer", nullable: false),
                    SkillId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestRewardSkills", x => x.QuestRewardSkillId);
                    table.ForeignKey(
                        name: "FK_QuestRewardSkills_Quests_QuestId",
                        column: x => x.QuestId,
                        principalTable: "Quests",
                        principalColumn: "QuestId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestRewardSkills_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "SkillId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BlockContents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ContentId = table.Column<int>(type: "integer", nullable: false),
                    BlockType = table.Column<string>(type: "text", nullable: false),
                    ContentData = table.Column<string>(type: "text", nullable: true),
                    MediaUrl = table.Column<string>(type: "text", nullable: true),
                    Caption = table.Column<string>(type: "text", nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlockContents_Contents_ContentId",
                        column: x => x.ContentId,
                        principalTable: "Contents",
                        principalColumn: "ContentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    ChatMessageId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SenderId = table.Column<int>(type: "integer", nullable: false),
                    RecipientId = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsReported = table.Column<bool>(type: "boolean", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false),
                    ReportedById = table.Column<int>(type: "integer", nullable: true),
                    ReportReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ReportedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.ChatMessageId);
                    table.ForeignKey(
                        name: "FK_ChatMessages_PlayerProfiles_RecipientId",
                        column: x => x.RecipientId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatMessages_PlayerProfiles_ReportedById",
                        column: x => x.ReportedById,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ChatMessages_PlayerProfiles_SenderId",
                        column: x => x.SenderId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DungeonSessions",
                columns: table => new
                {
                    DungeonSessionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerProfileId = table.Column<int>(type: "integer", nullable: false),
                    DungeonConfigId = table.Column<int>(type: "integer", nullable: false),
                    EnterTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ClaimedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    IsRewardClaimed = table.Column<bool>(type: "boolean", nullable: false),
                    PartyMembers = table.Column<string>(type: "text", nullable: true),
                    ClaimedByMembers = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DungeonSessions", x => x.DungeonSessionId);
                    table.ForeignKey(
                        name: "FK_DungeonSessions_DungeonConfigs_DungeonConfigId",
                        column: x => x.DungeonConfigId,
                        principalTable: "DungeonConfigs",
                        principalColumn: "DungeonConfigId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DungeonSessions_PlayerProfiles_PlayerProfileId",
                        column: x => x.PlayerProfileId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FriendBlocks",
                columns: table => new
                {
                    FriendBlockId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BlockerId = table.Column<int>(type: "integer", nullable: false),
                    BlockedId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendBlocks", x => x.FriendBlockId);
                    table.ForeignKey(
                        name: "FK_FriendBlocks_PlayerProfiles_BlockedId",
                        column: x => x.BlockedId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FriendBlocks_PlayerProfiles_BlockerId",
                        column: x => x.BlockerId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Friends",
                columns: table => new
                {
                    FriendId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RequesterId = table.Column<int>(type: "integer", nullable: false),
                    AddresseeId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RespondedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friends", x => x.FriendId);
                    table.ForeignKey(
                        name: "FK_Friends_PlayerProfiles_AddresseeId",
                        column: x => x.AddresseeId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Friends_PlayerProfiles_RequesterId",
                        column: x => x.RequesterId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GachaPullHistories",
                columns: table => new
                {
                    GachaPullHistoryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerProfileId = table.Column<int>(type: "integer", nullable: false),
                    GachaBannerId = table.Column<int>(type: "integer", nullable: false),
                    RewardItemId = table.Column<int>(type: "integer", nullable: false),
                    PullCount = table.Column<int>(type: "integer", nullable: false),
                    CostSpent = table.Column<decimal>(type: "numeric", nullable: false),
                    PulledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GachaPullHistories", x => x.GachaPullHistoryId);
                    table.ForeignKey(
                        name: "FK_GachaPullHistories_GachaBanners_GachaBannerId",
                        column: x => x.GachaBannerId,
                        principalTable: "GachaBanners",
                        principalColumn: "GachaBannerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GachaPullHistories_Items_RewardItemId",
                        column: x => x.RewardItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GachaPullHistories_PlayerProfiles_PlayerProfileId",
                        column: x => x.PlayerProfileId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Guilds",
                columns: table => new
                {
                    GuildId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Notice = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IconId = table.Column<int>(type: "integer", nullable: false),
                    BannerId = table.Column<int>(type: "integer", nullable: false),
                    LeaderId = table.Column<int>(type: "integer", nullable: false),
                    CreatedByProfileId = table.Column<int>(type: "integer", nullable: false),
                    RequiredLevel = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    GuildExp = table.Column<int>(type: "integer", nullable: false),
                    TotalMedals = table.Column<int>(type: "integer", nullable: false),
                    TotalFeats = table.Column<int>(type: "integer", nullable: false),
                    JoinPolicy = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guilds", x => x.GuildId);
                    table.ForeignKey(
                        name: "FK_Guilds_PlayerProfiles_CreatedByProfileId",
                        column: x => x.CreatedByProfileId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Guilds_PlayerProfiles_LeaderId",
                        column: x => x.LeaderId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventoryItems",
                columns: table => new
                {
                    InventoryItemId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerProfileId = table.Column<int>(type: "integer", nullable: false),
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    IsEquipped = table.Column<bool>(type: "boolean", nullable: false),
                    IsSkin = table.Column<bool>(type: "boolean", nullable: false),
                    EquippedSlot = table.Column<string>(type: "text", nullable: true),
                    EnhancementLevel = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItems", x => x.InventoryItemId);
                    table.ForeignKey(
                        name: "FK_InventoryItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryItems_PlayerProfiles_PlayerProfileId",
                        column: x => x.PlayerProfileId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Mails",
                columns: table => new
                {
                    MailId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerProfileId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    AttachedGold = table.Column<decimal>(type: "numeric", nullable: false),
                    AttachedGems = table.Column<decimal>(type: "numeric", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    IsClaimed = table.Column<bool>(type: "boolean", nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mails", x => x.MailId);
                    table.ForeignKey(
                        name: "FK_Mails_PlayerProfiles_PlayerProfileId",
                        column: x => x.PlayerProfileId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerAchievements",
                columns: table => new
                {
                    PlayerAchievementId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerProfileId = table.Column<int>(type: "integer", nullable: false),
                    AchievementId = table.Column<int>(type: "integer", nullable: false),
                    Progress = table.Column<int>(type: "integer", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UnlockedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerAchievements", x => x.PlayerAchievementId);
                    table.ForeignKey(
                        name: "FK_PlayerAchievements_Achievements_AchievementId",
                        column: x => x.AchievementId,
                        principalTable: "Achievements",
                        principalColumn: "AchievementId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerAchievements_PlayerProfiles_PlayerProfileId",
                        column: x => x.PlayerProfileId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerAnnouncements",
                columns: table => new
                {
                    PlayerAnnouncementId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerProfileId = table.Column<int>(type: "integer", nullable: false),
                    GameAnnouncementId = table.Column<int>(type: "integer", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerAnnouncements", x => x.PlayerAnnouncementId);
                    table.ForeignKey(
                        name: "FK_PlayerAnnouncements_GameAnnouncements_GameAnnouncementId",
                        column: x => x.GameAnnouncementId,
                        principalTable: "GameAnnouncements",
                        principalColumn: "GameAnnouncementId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerAnnouncements_PlayerProfiles_PlayerProfileId",
                        column: x => x.PlayerProfileId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerBuffs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerProfileId = table.Column<int>(type: "integer", nullable: false),
                    BuffName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IconName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DurationRemaining = table.Column<float>(type: "real", nullable: false),
                    IsDebuff = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerBuffs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerBuffs_PlayerProfiles_PlayerProfileId",
                        column: x => x.PlayerProfileId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerChests",
                columns: table => new
                {
                    PlayerChestId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerProfileId = table.Column<int>(type: "integer", nullable: false),
                    ChestId = table.Column<int>(type: "integer", nullable: false),
                    IsOpened = table.Column<bool>(type: "boolean", nullable: false),
                    ReceivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OpenedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerChests", x => x.PlayerChestId);
                    table.ForeignKey(
                        name: "FK_PlayerChests_Chests_ChestId",
                        column: x => x.ChestId,
                        principalTable: "Chests",
                        principalColumn: "ChestId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerChests_PlayerProfiles_PlayerProfileId",
                        column: x => x.PlayerProfileId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerCurrencyLogs",
                columns: table => new
                {
                    PlayerCurrencyLogId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerProfileId = table.Column<int>(type: "integer", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    BalanceAfter = table.Column<decimal>(type: "numeric", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerCurrencyLogs", x => x.PlayerCurrencyLogId);
                    table.ForeignKey(
                        name: "FK_PlayerCurrencyLogs_PlayerProfiles_PlayerProfileId",
                        column: x => x.PlayerProfileId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerDailyLogins",
                columns: table => new
                {
                    PlayerDailyLoginId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerProfileId = table.Column<int>(type: "integer", nullable: false),
                    CurrentStreak = table.Column<int>(type: "integer", nullable: false),
                    TotalDaysClaimed = table.Column<int>(type: "integer", nullable: false),
                    LastClaimedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsClaimedToday = table.Column<bool>(type: "boolean", nullable: false),
                    CurrentYear = table.Column<int>(type: "integer", nullable: false),
                    CurrentMonth = table.Column<int>(type: "integer", nullable: false),
                    RetroClaimCount = table.Column<int>(type: "integer", nullable: false),
                    ClaimedDaysStr = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerDailyLogins", x => x.PlayerDailyLoginId);
                    table.ForeignKey(
                        name: "FK_PlayerDailyLogins_PlayerProfiles_PlayerProfileId",
                        column: x => x.PlayerProfileId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerMonsterDiscoveries",
                columns: table => new
                {
                    PlayerMonsterDiscoveryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerProfileId = table.Column<int>(type: "integer", nullable: false),
                    MonsterId = table.Column<int>(type: "integer", nullable: false),
                    IsDiscovered = table.Column<bool>(type: "boolean", nullable: false),
                    DiscoveredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TimesDefeated = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerMonsterDiscoveries", x => x.PlayerMonsterDiscoveryId);
                    table.ForeignKey(
                        name: "FK_PlayerMonsterDiscoveries_Monsters_MonsterId",
                        column: x => x.MonsterId,
                        principalTable: "Monsters",
                        principalColumn: "MonsterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerMonsterDiscoveries_PlayerProfiles_PlayerProfileId",
                        column: x => x.PlayerProfileId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerQuests",
                columns: table => new
                {
                    PlayerQuestId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerProfileId = table.Column<int>(type: "integer", nullable: false),
                    QuestId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Progress = table.Column<int>(type: "integer", nullable: false),
                    TargetValue = table.Column<int>(type: "integer", nullable: false),
                    AcceptedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ClaimedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerQuests", x => x.PlayerQuestId);
                    table.ForeignKey(
                        name: "FK_PlayerQuests_PlayerProfiles_PlayerProfileId",
                        column: x => x.PlayerProfileId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerQuests_Quests_QuestId",
                        column: x => x.QuestId,
                        principalTable: "Quests",
                        principalColumn: "QuestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerShopRefreshStates",
                columns: table => new
                {
                    PlayerShopRefreshStateId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerProfileId = table.Column<int>(type: "integer", nullable: false),
                    ShopDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RefreshCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastRefreshAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerShopRefreshStates", x => x.PlayerShopRefreshStateId);
                    table.ForeignKey(
                        name: "FK_PlayerShopRefreshStates_PlayerProfiles_PlayerProfileId",
                        column: x => x.PlayerProfileId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerSkills",
                columns: table => new
                {
                    PlayerSkillId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerProfileId = table.Column<int>(type: "integer", nullable: false),
                    SkillId = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Experience = table.Column<int>(type: "integer", nullable: false),
                    EquippedSlot = table.Column<int>(type: "integer", nullable: true),
                    UnlockedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NextAvailableTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerSkills", x => x.PlayerSkillId);
                    table.ForeignKey(
                        name: "FK_PlayerSkills_PlayerProfiles_PlayerProfileId",
                        column: x => x.PlayerProfileId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerSkills_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "SkillId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerSkins",
                columns: table => new
                {
                    PlayerSkinId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerProfileId = table.Column<int>(type: "integer", nullable: false),
                    SkinId = table.Column<int>(type: "integer", nullable: false),
                    IsEquipped = table.Column<bool>(type: "boolean", nullable: false),
                    UnlockedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerSkins", x => x.PlayerSkinId);
                    table.ForeignKey(
                        name: "FK_PlayerSkins_PlayerProfiles_PlayerProfileId",
                        column: x => x.PlayerProfileId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerSkins_Skins_SkinId",
                        column: x => x.SkinId,
                        principalTable: "Skins",
                        principalColumn: "SkinId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerStats",
                columns: table => new
                {
                    PlayerStatId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerProfileId = table.Column<int>(type: "integer", nullable: false),
                    CurrentHp = table.Column<int>(type: "integer", nullable: false),
                    MaxHp = table.Column<int>(type: "integer", nullable: false),
                    Atk = table.Column<int>(type: "integer", nullable: false),
                    Def = table.Column<int>(type: "integer", nullable: false),
                    MoveSpeed = table.Column<int>(type: "integer", nullable: false),
                    AttackSpeed = table.Column<int>(type: "integer", nullable: false),
                    CritRate = table.Column<int>(type: "integer", nullable: false),
                    CritDamage = table.Column<int>(type: "integer", nullable: false),
                    DamageBonus = table.Column<int>(type: "integer", nullable: false),
                    SkillPoints = table.Column<int>(type: "integer", nullable: false),
                    TotalWins = table.Column<int>(type: "integer", nullable: false),
                    TotalLosses = table.Column<int>(type: "integer", nullable: false),
                    TotalKills = table.Column<int>(type: "integer", nullable: false),
                    TotalDeaths = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerStats", x => x.PlayerStatId);
                    table.ForeignKey(
                        name: "FK_PlayerStats_PlayerProfiles_PlayerProfileId",
                        column: x => x.PlayerProfileId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerStatsSnapshots",
                columns: table => new
                {
                    PlayerStatsSnapshotId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerProfileId = table.Column<int>(type: "integer", nullable: false),
                    MaxHp = table.Column<int>(type: "integer", nullable: false),
                    Atk = table.Column<int>(type: "integer", nullable: false),
                    Def = table.Column<int>(type: "integer", nullable: false),
                    MoveSpeed = table.Column<int>(type: "integer", nullable: false),
                    AttackSpeed = table.Column<int>(type: "integer", nullable: false),
                    CritRate = table.Column<int>(type: "integer", nullable: false),
                    CritDamage = table.Column<int>(type: "integer", nullable: false),
                    DamageBonus = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerStatsSnapshots", x => x.PlayerStatsSnapshotId);
                    table.ForeignKey(
                        name: "FK_PlayerStatsSnapshots_PlayerProfiles_PlayerProfileId",
                        column: x => x.PlayerProfileId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseHistories",
                columns: table => new
                {
                    PurchaseHistoryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerProfileId = table.Column<int>(type: "integer", nullable: false),
                    ShopItemId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    PurchasedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseHistories", x => x.PurchaseHistoryId);
                    table.ForeignKey(
                        name: "FK_PurchaseHistories_PlayerProfiles_PlayerProfileId",
                        column: x => x.PlayerProfileId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseHistories_ShopItems_ShopItemId",
                        column: x => x.ShopItemId,
                        principalTable: "ShopItems",
                        principalColumn: "ShopItemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorldChatMessages",
                columns: table => new
                {
                    WorldChatMessageId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SenderId = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsReported = table.Column<bool>(type: "boolean", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false),
                    ReportedById = table.Column<int>(type: "integer", nullable: true),
                    ReportReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ReportedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorldChatMessages", x => x.WorldChatMessageId);
                    table.ForeignKey(
                        name: "FK_WorldChatMessages_PlayerProfiles_ReportedById",
                        column: x => x.ReportedById,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_WorldChatMessages_PlayerProfiles_SenderId",
                        column: x => x.SenderId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DungeonProgresses",
                columns: table => new
                {
                    DungeonProgressId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DungeonSessionId = table.Column<int>(type: "integer", nullable: false),
                    MonstersKilled = table.Column<int>(type: "integer", nullable: false),
                    BossSpawned = table.Column<bool>(type: "boolean", nullable: false),
                    BossKilled = table.Column<bool>(type: "boolean", nullable: false),
                    ElapsedTime = table.Column<int>(type: "integer", nullable: false),
                    CompletionPercentage = table.Column<int>(type: "integer", nullable: false),
                    ExtraData = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DungeonProgresses", x => x.DungeonProgressId);
                    table.ForeignKey(
                        name: "FK_DungeonProgresses_DungeonSessions_DungeonSessionId",
                        column: x => x.DungeonSessionId,
                        principalTable: "DungeonSessions",
                        principalColumn: "DungeonSessionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GuildApplications",
                columns: table => new
                {
                    GuildApplicationId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GuildId = table.Column<int>(type: "integer", nullable: false),
                    PlayerProfileId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildApplications", x => x.GuildApplicationId);
                    table.ForeignKey(
                        name: "FK_GuildApplications_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "GuildId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuildApplications_PlayerProfiles_PlayerProfileId",
                        column: x => x.PlayerProfileId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GuildChatMessages",
                columns: table => new
                {
                    GuildChatMessageId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GuildId = table.Column<int>(type: "integer", nullable: false),
                    SenderId = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    MessageType = table.Column<int>(type: "integer", nullable: false),
                    SenderRole = table.Column<int>(type: "integer", nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildChatMessages", x => x.GuildChatMessageId);
                    table.ForeignKey(
                        name: "FK_GuildChatMessages_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "GuildId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuildChatMessages_PlayerProfiles_SenderId",
                        column: x => x.SenderId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GuildInvitations",
                columns: table => new
                {
                    GuildInvitationId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GuildId = table.Column<int>(type: "integer", nullable: false),
                    InviterId = table.Column<int>(type: "integer", nullable: false),
                    InviteeId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RespondedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildInvitations", x => x.GuildInvitationId);
                    table.ForeignKey(
                        name: "FK_GuildInvitations_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "GuildId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuildInvitations_PlayerProfiles_InviteeId",
                        column: x => x.InviteeId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuildInvitations_PlayerProfiles_InviterId",
                        column: x => x.InviterId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GuildLogs",
                columns: table => new
                {
                    GuildLogId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GuildId = table.Column<int>(type: "integer", nullable: false),
                    ActorProfileId = table.Column<int>(type: "integer", nullable: true),
                    TargetProfileId = table.Column<int>(type: "integer", nullable: true),
                    ActorName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    TargetName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Action = table.Column<int>(type: "integer", nullable: false),
                    Detail = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildLogs", x => x.GuildLogId);
                    table.ForeignKey(
                        name: "FK_GuildLogs_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "GuildId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuildLogs_PlayerProfiles_ActorProfileId",
                        column: x => x.ActorProfileId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_GuildLogs_PlayerProfiles_TargetProfileId",
                        column: x => x.TargetProfileId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "GuildMembers",
                columns: table => new
                {
                    GuildMemberId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GuildId = table.Column<int>(type: "integer", nullable: false),
                    PlayerProfileId = table.Column<int>(type: "integer", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    DailyContribution = table.Column<int>(type: "integer", nullable: false),
                    WeeklyContribution = table.Column<int>(type: "integer", nullable: false),
                    TotalContribution = table.Column<int>(type: "integer", nullable: false),
                    Contribution = table.Column<int>(type: "integer", nullable: false),
                    Medals = table.Column<int>(type: "integer", nullable: false),
                    Feats = table.Column<int>(type: "integer", nullable: false),
                    LastDonateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastChatAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    JoinedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LeftAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildMembers", x => x.GuildMemberId);
                    table.ForeignKey(
                        name: "FK_GuildMembers_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "GuildId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuildMembers_PlayerProfiles_PlayerProfileId",
                        column: x => x.PlayerProfileId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MailRewardItems",
                columns: table => new
                {
                    MailRewardItemId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MailId = table.Column<int>(type: "integer", nullable: false),
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailRewardItems", x => x.MailRewardItemId);
                    table.ForeignKey(
                        name: "FK_MailRewardItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MailRewardItems_Mails_MailId",
                        column: x => x.MailId,
                        principalTable: "Mails",
                        principalColumn: "MailId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatModerationPenalties",
                columns: table => new
                {
                    ChatModerationPenaltyId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerProfileId = table.Column<int>(type: "integer", nullable: false),
                    ReporterId = table.Column<int>(type: "integer", nullable: true),
                    ChatMessageId = table.Column<int>(type: "integer", nullable: true),
                    WorldChatMessageId = table.Column<int>(type: "integer", nullable: true),
                    Channel = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ContentSnapshot = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ReportReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    MatchedTerms = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ViolationCount = table.Column<int>(type: "integer", nullable: false),
                    LockLevel = table.Column<int>(type: "integer", nullable: false),
                    LockedUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatModerationPenalties", x => x.ChatModerationPenaltyId);
                    table.ForeignKey(
                        name: "FK_ChatModerationPenalties_ChatMessages_ChatMessageId",
                        column: x => x.ChatMessageId,
                        principalTable: "ChatMessages",
                        principalColumn: "ChatMessageId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ChatModerationPenalties_PlayerProfiles_PlayerProfileId",
                        column: x => x.PlayerProfileId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatModerationPenalties_PlayerProfiles_ReporterId",
                        column: x => x.ReporterId,
                        principalTable: "PlayerProfiles",
                        principalColumn: "PlayerProfileId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ChatModerationPenalties_WorldChatMessages_WorldChatMessageId",
                        column: x => x.WorldChatMessageId,
                        principalTable: "WorldChatMessages",
                        principalColumn: "WorldChatMessageId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "ClassConfigs",
                columns: new[] { "ClassConfigId", "Atk", "AttackSpeed", "ClassName", "CritDamage", "CritRate", "DamageBonus", "Def", "MaxHp", "MoveSpeed" },
                values: new object[,]
                {
                    { 1, 50, 100, "Knight", 150, 5, 0, 40, 500, 100 },
                    { 2, 70, 100, "Archer", 150, 5, 0, 20, 350, 100 },
                    { 3, 90, 100, "Mage", 150, 5, 0, 15, 300, 100 }
                });

            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "ItemId", "BaseValue", "CorruptionReduction", "CreatedAt", "Description", "IconUrl", "IsActive", "MaxStack", "Name", "Rarity", "Slot", "Type" },
                values: new object[,]
                {
                    { 1, 1m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "In-game gold currency.", null, true, 2147483647, "Gold", "Common", "None", "Currency" },
                    { 2, 1m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Experience points for leveling up.", null, true, 2147483647, "Exp", "Common", "None", "Currency" },
                    { 3, 5m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Premium gem used to purchase high-tier items.", null, true, 2147483647, "Gem", "Rare", "None", "Currency" },
                    { 4, 1m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Lucky ticket used to spin the gacha banner.", null, true, 99, "Lucky Ticket", "Rare", "None", "Consumable" },
                    { 5, 150m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Basic iron sword for beginner warriors.", null, true, 1, "Iron Sword", "Common", "Weapon", "Weapon" },
                    { 6, 150m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A forest hunter bow, light and accurate.", null, true, 1, "Hunter Bow", "Common", "Weapon", "Weapon" },
                    { 7, 150m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A novice magic staff for casting light spells.", null, true, 1, "Apprentice Staff", "Common", "Weapon", "Weapon" },
                    { 8, 800m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A glowing elven blade, forged deep in the ancient forest.", null, true, 1, "Elven Blade", "Epic", "Weapon", "Weapon" },
                    { 9, 120m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Light leather armor that provides basic defense.", null, true, 1, "Leather Armor", "Common", "Armor", "Armor" },
                    { 10, 100m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sturdy iron helmet that protects the head from damage.", null, true, 1, "Iron Helmet", "Common", "Helmet", "Armor" },
                    { 11, 200m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Wind-infused boots that increase movement speed.", null, true, 1, "Wind Boots", "Uncommon", "Boots", "Armor" },
                    { 12, 2000m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Legendary dragon scale armor offering supreme defense.", null, true, 1, "Dragon Scale Armor", "Legendary", "Armor", "Armor" },
                    { 13, 900m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Shadow cloak that boosts speed and evasion.", null, true, 1, "Phantom Cloak", "Epic", "Armor", "Armor" },
                    { 14, 500m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Dark hood that increases critical strike damage.", null, true, 1, "Shadow Hood", "Rare", "Helmet", "Armor" },
                    { 15, 120m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Iron gauntlets that increase physical damage.", null, true, 1, "Iron Gauntlets", "Common", "Gloves", "Armor" },
                    { 16, 100m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Soft leather gauntlets that allow flexible combat.", null, true, 1, "Leather Gauntlets", "Common", "Gloves", "Armor" },
                    { 17, 80m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Basic copper ring that slightly boosts stats.", null, true, 1, "Copper Ring", "Common", "Ring", "Armor" },
                    { 18, 200m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Silver necklace that increases maximum energy.", null, true, 1, "Silver Necklace", "Uncommon", "Necklace", "Armor" },
                    { 19, 30m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Small health potion that restores 80 HP.", null, true, 99, "Small Health Potion", "Common", "None", "Consumable" },
                    { 20, 80m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Large health potion that restores 200 HP.", null, true, 99, "Large Health Potion", "Uncommon", "None", "Consumable" },
                    { 21, 60m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Energy elixir that restores 60 Energy.", null, true, 99, "Energy Elixir", "Uncommon", "None", "Consumable" },
                    { 22, 50m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Magic stone used to upgrade player skills.", null, true, 999, "Skill Upgrade Stone", "Rare", "None", "Material" },
                    { 23, 0m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "White flower collected in the fairy forest.", null, true, 99, "White Flower", "Common", "None", "QuestItem" },
                    { 24, 0m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Logs collected from the ancient forest.", null, true, 99, "Wood Logs", "Common", "None", "QuestItem" },
                    { 25, 0m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ancient tree leaves collected from the fairy forest.", null, true, 99, "Ancient Leaves", "Common", "None", "QuestItem" },
                    { 26, 0m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Dragon Seal Book. Dropped by DragonBossIdle. Collect all 4 seal books to save the World Tree.", null, true, 1, "Dragon Seal Book", "Epic", "None", "QuestItem" },
                    { 27, 0m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Golem Seal Book. Dropped by GolemBoss.", null, true, 1, "Golem Seal Book", "Epic", "None", "QuestItem" },
                    { 28, 0m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "UnderKing Seal Book. Dropped by the UnderKing boss.", null, true, 1, "UnderKing Seal Book", "Epic", "None", "QuestItem" },
                    { 29, 0m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Swamp Demon Seal Book. Dropped by SwampDemon boss.", null, true, 1, "Swamp Seal Book", "Epic", "None", "QuestItem" },
                    { 30, 0m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A magical pumpkin glowing with autumn energy.", null, true, 99, "Enchanted Pumpkin", "Common", "None", "QuestItem" },
                    { 31, 0m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Mystical flour used for special spells.", null, true, 99, "Magic Flour", "Common", "None", "QuestItem" },
                    { 32, 0m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A skull radiating with ghostly presence.", null, true, 99, "Spirit Skull", "Common", "None", "QuestItem" },
                    { 33, 0m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A key that opens the castle on the deserted island.", null, true, 1, "Mystic Key", "Epic", "None", "QuestItem" },
                    { 901, 100m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A sharp sword dropped by the SwampDemon.", null, true, 1, "Swamp Sword", "Rare", "Weapon", "Weapon" },
                    { 902, 150m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A sturdy armor dropped by the SwampDemon.", null, true, 1, "Swamp Armor", "Rare", "Armor", "Armor" },
                    { 903, 500m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A legendary sword dropped by DragonBossIdle.", null, true, 1, "Dragon Boss Sword", "Legendary", "Weapon", "Weapon" },
                    { 904, 600m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A legendary armor dropped by DragonBossIdle.", null, true, 1, "Dragon Boss Armor", "Legendary", "Armor", "Armor" },
                    { 905, 800m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Heavy stone gloves dropped by GolemBoss.", null, true, 1, "Golem Boss Gloves", "Legendary", "Gloves", "Armor" },
                    { 906, 1000m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A massive stone armor dropped by GolemBoss.", null, true, 1, "Golem Boss Armor", "Legendary", "Armor", "Armor" },
                    { 907, 1500m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A royal cursed sword dropped by UnderKing.", null, true, 1, "UnderKing Sword", "Legendary", "Weapon", "Weapon" },
                    { 908, 2000m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "The crown of the UnderKing.", null, true, 1, "UnderKing Crown", "Legendary", "Helmet", "Armor" },
                    { 909, 0m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A magic book containing the power to seal the Origin Tree, guarded by SwampDemon.", null, true, 1, "Swamp Seal Book", "Legendary", "None", "QuestItem" },
                    { 910, 0m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A magic book containing the power to seal the Origin Tree, guarded by DragonBossIdle.", null, true, 1, "Dragon Seal Book", "Legendary", "None", "QuestItem" },
                    { 911, 0m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A magic book containing the power to seal the Origin Tree, guarded by GolemBoss.", null, true, 1, "Golem Seal Book", "Legendary", "None", "QuestItem" },
                    { 912, 0m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "The final magic book to seal the Origin Tree, guarded by UnderKing.", null, true, 1, "UnderKing Seal Book", "Legendary", "None", "QuestItem" }
                });

            migrationBuilder.InsertData(
                table: "Monsters",
                columns: new[] { "MonsterId", "Atk", "AttackSpeed", "CreatedAt", "CritDamage", "CritRate", "Def", "Description", "ExperienceReward", "GemReward", "GoldReward", "ImageUrl", "IsActive", "Level", "MaxHp", "MoveSpeed", "Name", "Type" },
                values: new object[,]
                {
                    { 1, 5, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 187, 10, 2, "A basic slime monster.", 5, 0m, 15m, null, true, 1, 50, 1, "SlimeLittle", "Normal" },
                    { 2, 20, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 130, 20, 10, "A dangerous swamp demon.", 100, 0m, 200m, null, true, 10, 500, 1, "SwampDemon", "Boss" },
                    { 3, 15, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 183, 10, 5, "A water elemental monster.", 10, 0m, 30m, null, true, 5, 80, 1, "WaterElemental", "Normal" },
                    { 4, 30, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 180, 20, 15, "A fierce dragon.", 20, 0m, 50m, null, true, 5, 200, 1, "Dragon", "Normal" },
                    { 5, 35, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 156, 20, 20, "A frosty blue dragon.", 22, 0m, 55m, null, true, 6, 250, 5, "BlueDragonFrost", "Normal" },
                    { 6, 37, 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 160, 12, 25, "A forest green dragon.", 25, 0m, 62m, null, true, 7, 270, 6, "GreenDragonForest", "Normal" },
                    { 7, 50, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 250, 30, 35, "A terrifying boss dragon.", 300, 0m, 1000m, null, true, 20, 1000, 0, "DragonBossIdle", "Boss" },
                    { 8, 25, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 160, 12, 50, "An icy slime.", 30, 0m, 70m, null, true, 8, 300, 1, "Slime_ice", "Normal" },
                    { 9, 50, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 170, 25, 70, "An icy dragon.", 32, 0m, 100m, null, true, 9, 350, 2, "Ice_Dragon", "Normal" },
                    { 10, 150, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 150, 30, 70, "A giant stone golem boss.", 1500, 10m, 2000m, null, true, 15, 3000, 3, "GolemBoss", "Boss" },
                    { 11, 50, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 166, 25, 100, "An undead orc skeleton.", 40, 0m, 70m, null, true, 5, 400, 2, "OrcSkeleton", "Normal" },
                    { 12, 70, 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 164, 15, 70, "A melee skeleton warrior.", 42, 0m, 74m, null, true, 6, 350, 3, "SkeletonMelee", "Normal" },
                    { 13, 100, 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 163, 25, 30, "A ranged skeleton archer.", 38, 0m, 78m, null, true, 6, 250, 3, "SkeletonArcher", "Normal" },
                    { 14, 90, 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 185, 30, 150, "A floating ghost.", 45, 0m, 85m, null, true, 5, 300, 4, "Ghost", "Normal" },
                    { 15, 200, 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 20, 40, 300, "The supreme skeleton king.", 900, 500m, 2500m, null, true, 20, 10000, 4, "UnderKing", "Boss" },
                    { 16, 70, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 150, 40, 30, "A terrifying demon.", 50, 0m, 100m, null, true, 8, 500, 2, "Demon", "Normal" },
                    { 17, 70, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 150, 20, 80, "A strong goblin warrior.", 50, 0m, 100m, null, true, 6, 450, 3, "GoblinWarrior", "Normal" },
                    { 18, 50, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 150, 20, 40, "A goblin spearman.", 50, 0m, 100m, null, true, 6, 450, 3, "GoblinSpear", "Normal" },
                    { 19, 50, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 150, 20, 40, "A fierce ogre.", 50, 0m, 100m, null, true, 6, 450, 3, "Ogre", "Normal" },
                    { 20, 100, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 175, 28, 80, "A formidable orc warlord.", 55, 0m, 100m, null, true, 7, 600, 3, "OrcWarlord", "Normal" }
                });

            migrationBuilder.InsertData(
                table: "NPCs",
                columns: new[] { "NPCId", "Description", "IconUrl", "InteractionRadius", "IsActive", "MapName", "Name", "PositionX", "PositionY", "Type" },
                values: new object[,]
                {
                    { 1, "The wise guide of the Elf Forest.", null, 2.5f, true, "ElfForest", "Elder Rowan", 12.4932, 18.61223, "QuestGiver" },
                    { 2, "A spirit of the forest.", null, 2.5f, true, "ElfForest", "Lyra", 41.945869999999999, -27.180520000000001, "QuestGiver" },
                    { 3, "A mysterious figure in a cloak.", null, 2.5f, true, "ElfForest", "Mysterious Figure", 10.111940000000001, -45.863010000000003, "QuestGiver" },
                    { 4, "The wise guide, now in the pumpkin town.", null, 2.5f, true, "AutumnPumpkin", "Elder Rowan (Pumpkin)", 1.8735120000000001, -92.815799999999996, "QuestGiver" },
                    { 5, "The city gate guard.", null, 2.5f, true, "AutumnPumpkin", "Tristan", 11.62283, -113.61579999999999, "QuestGiver" },
                    { 6, "The silver knight.", null, 2.5f, true, "AutumnPumpkin", "Arthur", 77.544120000000007, -77.443010000000001, "QuestGiver" },
                    { 7, "A farmer collecting enchanted pumpkins.", null, 2.5f, true, "AutumnPumpkin", "Fa", 6.0800000000000001, -161.90000000000001, "QuestGiver" },
                    { 8, "Queen of the frozen lands.", null, 2.5f, true, "FrozenMountain", "Roselyn Aurora Queen", 160.8554, -35.648600000000002, "QuestGiver" },
                    { 9, "The witch and disguised priest.", null, 2.5f, true, "FrozenMountain", "Zephyr", 6.9968139999999996, -0.20945549999999999, "QuestGiver" },
                    { 10, "The forbidden zone guard.", null, 2.5f, true, "FrozenMountain", "Roland", 70.456860000000006, 18.803540000000002, "QuestGiver" },
                    { 11, "A brave warrior fighting skeletons.", null, 2.5f, true, "AbandonedCastle", "Valiant Warrior", -10.66112, 54.928840000000001, "QuestGiver" },
                    { 12, "The ghost of a young girl.", null, 2.5f, true, "AbandonedCastle", "Natalie", -48.921259999999997, -21.120059999999999, "QuestGiver" },
                    { 13, "The lone guard of the deserted island.", null, 2.5f, true, "AbandonedCastle", "Elf Guard", -6.2377580000000004, -13.13438, "QuestGiver" }
                });

            migrationBuilder.InsertData(
                table: "Quests",
                columns: new[] { "QuestId", "BossMonsterId", "DefaultStatus", "Description", "IsActive", "MapName", "ObjectiveLocation", "ObjectiveTarget", "ObjectiveType", "QuestGiverName", "RegionName", "RequiredLevel", "RewardExperience", "RewardGems", "RewardGold", "RewardItemId", "RewardSkillId", "TargetAmount", "Title", "Type" },
                values: new object[,]
                {
                    { 1, null, "NotStarted", "Talk to Elder Rowan in the Elf Forest.", true, "ElfForest", "Elf Forest", "Elder Rowan", "Talk", "Elder Rowan", null, 1, 5, 5m, 10m, null, null, 1, "[Chapter 1] Speak with Elder Rowan", "Main" },
                    { 2, null, "NotStarted", "Collect 3 White Flowers from the forest.", true, "ElfForest", "Elf Forest", "White Flower", "Collect", "Elder Rowan", null, 1, 10, 5m, 8m, null, null, 3, "[Chapter 1] Gather White Flowers", "Main" },
                    { 4, null, "NotStarted", "Equip your first combat skill.", true, "ElfForest", "Elf Forest", "Skill Panel", "EquipSkill", "Elder Rowan", null, 1, 10, 5m, 10m, null, null, 1, "[Chapter 1] Equip Your Skill", "Main" },
                    { 5, null, "NotStarted", "Kill 3 SlimeLittle monsters in the forest.", true, "ElfForest", "Elf Forest", "SlimeLittle", "Defeat", "Elder Rowan", null, 1, 15, 5m, 15m, null, null, 3, "[Chapter 1] Defeat Slimes", "Main" },
                    { 7, null, "NotStarted", "Talk to Lyra about the cursed Origin Tree and the 4 Seal Books.", true, "ElfForest", "Origin Tree", "Lyra", "Talk", "Lyra", null, 2, 10, 5m, 10m, null, null, 1, "[Chapter 1] The Origin Tree", "Main" },
                    { 8, null, "NotStarted", "Follow the cloaked figure through the portal to Autumn Pumpkin.", true, "ElfForest", "Elf Forest", "Portal", "Explore", "Mysterious Figure", null, 2, 5, 5m, 5m, null, null, 1, "[Chapter 1] The Mysterious Figure", "Main" },
                    { 9, null, "NotStarted", "Teleported onto the beach, proceed to the castle and ask Elder Rowan where this is. After introductions, realize you have no money and ask if there is work to earn food.", true, "AutumnPumpkin", "Autumn Pumpkin", "Elder Rowan", "Talk", "Elder Rowan", null, 3, 100, 5m, 5m, null, null, 1, "[Chapter 2] Where Are We?", "Main" },
                    { 10, null, "NotStarted", "Collect 8 Enchanted Pumpkins from the field and hand them over to farmer Fa.", true, "AutumnPumpkin", "Pumpkin Town", "Enchanted Pumpkin", "Collect", "Fa", null, 3, 300, 5m, 10m, null, null, 8, "[Chapter 2] Work for Food", "Main" },
                    { 11, null, "NotStarted", "Help Fa deliver the harvested pumpkins to guard Tristan at the ruined city gate.", true, "AutumnPumpkin", "City Gate", "Tristan", "Talk", "Fa", null, 3, 200, 5m, 5m, null, null, 1, "[Chapter 2] Delivery to the City", "Main" },
                    { 12, null, "NotStarted", "Enter the city and investigate the dead bodies, then report back to guard Tristan.", true, "AutumnPumpkin", "Ruined City", "Corpse", "Interact", "Tristan", null, 3, 250, 5m, 5m, null, null, 5, "[Chapter 2] The Ruined City", "Main" },
                    { 13, null, "NotStarted", "Report the massacre to Tristan. He asks you to find the silver knight Arthur for help.", true, "AutumnPumpkin", "Ruined City", "Arthur", "Talk", "Tristan", null, 3, 250, 5m, 5m, null, null, 1, "[Chapter 2] Seek the Silver Knight", "Main" },
                    { 15, null, "NotStarted", "Receive the DarkExplosion skill and Silver Necklace from Arthur. Take his place to defeat 10 evil monsters in the Ruined City.", true, "AutumnPumpkin", "Ruined City", "Ghost/RobberAssassin/RedGuard/GoblinSpear/GoblinWarrior/RobberArcher/NecromancerCast", "Defeat", "Arthur", null, 12, 20, 5m, 20m, null, null, 10, "[Chapter 2] Defeat the Evil Monsters", "Main" },
                    { 17, null, "NotStarted", "Talk to Arthur and receive the knight's thanks, ask about the whereabouts of the ??? and he directs you to the frozen land devastated by the codex, go to Frozen Mountains.", true, "AutumnPumpkin", "Ruined City", "Arthur", "Talk", "Arthur", null, 5, 10, 5m, 10m, null, null, 1, "[Chapter 2] The Frozen Threat", "Main" },
                    { 19, null, "NotStarted", "Deliver Magic Flour (obtained from the Queen) to the Priest (Zephyr).", true, "FrozenMountain", "Frozen Mountain", "Zephyr", "Talk", "Roselyn Aurora Queen", null, 6, 15, 5m, 15m, null, null, 1, "[Chapter 3] Magic Flour for the Priest", "Main" },
                    { 20, null, "NotStarted", "Meet Zephyr and slay 5 Ice Dragons on the mountain.", true, "FrozenMountain", "Frozen Mountain", "Ice_Dragon", "Defeat", "Zephyr", null, 7, 40, 5m, 40m, null, null, 5, "[Chapter 3] Dragons of Snow", "Main" },
                    { 21, null, "NotStarted", "Head to the forbidden zone and speak with Roland to explore it.", true, "FrozenMountain", "Forbidden Zone", "Roland", "Explore", "Roland", null, 7, 15, 5m, 15m, null, null, 1, "[Chapter 3] The Forbidden Zone", "Main" },
                    { 23, null, "NotStarted", "Defeat 12 skeletons in the valley for Valiant Warrior.", true, "AbandonedCastle", "Valley", "Skeleton", "Defeat", "Valiant Warrior", null, 9, 50, 5m, 50m, null, null, 12, "[Chapter 4] Skeleton Army", "Main" },
                    { 24, null, "NotStarted", "Go to Tide-Knell village, meet Natalie, and dig up the skull near the old well.", true, "AbandonedCastle", "Tide-Knell", "Skull", "Interact", "Valiant Warrior", null, 9, 30, 5m, 30m, null, null, 1, "[Chapter 4] The Abandoned Village", "Main" },
                    { 26, null, "NotStarted", "Talk to Elf Guard on the deserted island and collect 5 Ancient Leaves.", true, "AbandonedCastle", "Northern Plateau", "Ancient Leaves", "Collect", "Elf Guard", null, 10, 45, 5m, 45m, null, null, 5, "[Chapter 4] Deserted Island", "Main" },
                    { 28, null, "NotStarted", "Talk to Elf Guard. He will open a portal back to the Elf Forest.", true, "AbandonedCastle", "Deserted Island", "Elf Guard", "Talk", "Elf Guard", null, 12, 10, 5m, 10m, null, null, 1, "[Chapter 4] Return to Elf Forest", "Main" },
                    { 29, null, "NotStarted", "Talk to Lyra and use the 4 Seal Books to cleanse the tree. \"To be continued\".", true, "ElfForest", "Origin Tree", "Lyra", "Talk", "Lyra", null, 12, 500, 5m, 500m, null, null, 1, "[Chapter 1] Save the Origin Tree", "Main" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "Name" },
                values: new object[,]
                {
                    { 1, "Player" },
                    { 2, "Admin" },
                    { 3, "SuperAdmin" }
                });

            migrationBuilder.InsertData(
                table: "Skills",
                columns: new[] { "SkillId", "BaseDamage", "ClassRequirement", "CooldownSeconds", "CorruptionCost", "DamageGrowthPercent", "DamagePerLevel", "DamageType", "Description", "IsActive", "Name", "TargetType", "Type", "UnlockLevel" },
                values: new object[,]
                {
                    { 1, 0.0, "Archer", 15, 0f, 0.0, 0.0, "Physical", "Automatically fires in the direction the archer is facing.", true, "Accelerationarrow", "SingleTarget", "Active", 1 },
                    { 2, 0.0, "Archer", 20, 0f, 0.0, 0.0, "Physical", "Automatically fires in the direction the archer is facing.", true, "ArrowofLight", "SingleTarget", "Active", 1 },
                    { 3, 0.0, "Mage", 10, 0f, 0.0, 0.0, "Magical", "Heals allies within range.", true, "Holymagic", "Ally", "Buff", 1 },
                    { 4, 0.0, "Mage", 10, 0f, 0.0, 0.0, "Magical", "Casts a spell in the direction the character is facing.", true, "Purification", "SingleTarget", "Active", 1 },
                    { 5, 0.0, "Mage", 10, 0f, 0.0, 0.0, "Magical", "Selects and attacks a random monster within range.", true, "Stardust", "SingleTarget", "Active", 1 },
                    { 6, 0.0, "Knight", 20, 0f, 0.0, 0.0, "Physical", "Selects a target with the monster tag to attack.", true, "Lightsabers", "SingleTarget", "Active", 1 },
                    { 7, 0.0, "Knight", 15, 0f, 0.0, 0.0, "Physical", "Casts a spell in the direction the character is facing.", true, "LightWaves", "Area", "Active", 1 },
                    { 8, 0.0, "Knight", 20, 0f, 0.0, 0.0, "Magical", "Protects all allies within range.", true, "ProtectiveShield", "Ally", "Buff", 1 },
                    { 9, 0.0, "All", 90, 15f, 0.0, 0.0, "Magical", "Shared among all classes. Deals damage equal to 3x base damage. Increases corruption points by 15.", true, "DarkExplosion", "Area", "Active", 1 },
                    { 10, 0.0, "All", 60, 10f, 0.0, 0.0, "Magical", "Shared among all classes. Deals damage equal to 2x base damage. Increases corruption points by 10.", true, "DarkPoisonZone", "Area", "Active", 1 },
                    { 11, 0.0, "Archer", 20, 0f, 0.0, 0.0, "Physical", "Automatically fires in the direction the archer is facing.", true, "DeadlyCurse", "SingleTarget", "Active", 1 },
                    { 12, 0.0, "Mage", 5, 0f, 0.0, 0.0, "Magical", "Selects an area within range to attack.", true, "NightMagic", "Area", "Active", 1 },
                    { 13, 200.0, "All", 30, 8f, 0.0, 0.0, "Magical", "Shared among all classes. Deals damage equal to 3x base damage. Increases corruption points by 8.", true, "DeadlyExplosion", "SingleTarget", "Active", 1 },
                    { 14, 0.0, "Knight", 6, 0f, 0.0, 0.0, "Physical", "A short-range slash in the direction the knight is facing.", true, "BloodySlash", "SingleTarget", "Active", 1 },
                    { 15, 38.0, "Fighter", 8, 0f, 4.0, 11.0, "Physical", "A short-range slash in the direction the character is facing.", true, "FrozenSash", "SingleTarget", "Active", 1 }
                });

            migrationBuilder.InsertData(
                table: "EquipmentStats",
                columns: new[] { "EquipmentStatsId", "BaseAtk", "BaseDef", "BaseHp", "BonusAtk", "BonusAttackSpeed", "BonusCritDamage", "BonusCritRate", "BonusDamageBonus", "BonusDef", "BonusHp", "BonusMoveSpeed", "ItemId" },
                values: new object[,]
                {
                    { 5, 35, 0, 0, 8, 0, 50, 30, 0, 0, 0, 0, 5 },
                    { 6, 30, 0, 0, 6, 10, 30, 40, 0, 0, 0, 0, 6 },
                    { 7, 28, 0, 0, 5, 0, 80, 20, 10, 0, 0, 0, 7 },
                    { 8, 80, 0, 0, 20, 5, 100, 60, 15, 0, 0, 0, 8 },
                    { 9, 0, 12, 50, 0, 0, 0, 0, 0, 3, 10, 0, 9 },
                    { 10, 0, 30, 100, 0, 0, 0, 0, 0, 8, 20, 0, 10 },
                    { 11, 0, 5, 0, 0, 0, 0, 0, 0, 2, 0, 20, 11 },
                    { 12, 0, 120, 500, 0, 0, 0, 0, 0, 30, 100, 0, 12 },
                    { 13, 0, 60, 0, 0, 0, 0, 0, 0, 15, 0, 15, 13 },
                    { 14, 0, 20, 0, 0, 0, 120, 80, 0, 5, 0, 0, 14 },
                    { 15, 20, 5, 0, 5, 0, 0, 0, 5, 2, 0, 0, 15 },
                    { 16, 15, 3, 0, 3, 5, 0, 0, 0, 1, 0, 5, 16 },
                    { 17, 5, 3, 30, 2, 0, 10, 10, 0, 1, 5, 0, 17 },
                    { 18, 0, 5, 80, 0, 0, 0, 0, 0, 2, 20, 0, 18 },
                    { 901, 15, 0, 0, 0, 0, 10, 5, 0, 0, 0, 0, 901 },
                    { 902, 0, 20, 100, 0, 0, 0, 0, 0, 0, 0, 0, 902 },
                    { 903, 100, 0, 0, 0, 0, 20, 15, 0, 0, 0, 0, 903 },
                    { 904, 0, 100, 500, 0, 0, 0, 0, 0, 0, 0, 0, 904 },
                    { 905, 50, 50, 200, 0, 0, 5, 5, 0, 0, 0, 0, 905 },
                    { 906, 0, 200, 1000, 0, 0, 0, 0, 0, 0, 0, 0, 906 },
                    { 907, 200, 0, 0, 0, 0, 20, 20, 0, 0, 0, 0, 907 },
                    { 908, 50, 300, 1000, 0, 0, 10, 10, 0, 0, 0, 0, 908 }
                });

            migrationBuilder.InsertData(
                table: "MonsterDrops",
                columns: new[] { "MonsterDropId", "DropRate", "IsActive", "IsGuaranteed", "ItemId", "MaxQuantity", "MinQuantity", "MonsterId" },
                values: new object[,]
                {
                    { 901, 100.0, true, true, 901, 1, 1, 2 },
                    { 902, 100.0, true, true, 902, 1, 1, 2 },
                    { 903, 100.0, true, true, 903, 1, 1, 7 },
                    { 904, 100.0, true, true, 904, 1, 1, 7 },
                    { 905, 100.0, true, true, 905, 1, 1, 10 },
                    { 906, 100.0, true, true, 906, 1, 1, 10 },
                    { 907, 100.0, true, true, 907, 1, 1, 15 },
                    { 908, 100.0, true, true, 908, 1, 1, 15 },
                    { 909, 100.0, true, true, 909, 1, 1, 2 },
                    { 910, 100.0, true, true, 910, 1, 1, 7 },
                    { 911, 100.0, true, true, 911, 1, 1, 10 },
                    { 912, 100.0, true, true, 912, 1, 1, 15 }
                });

            migrationBuilder.InsertData(
                table: "NPCDialogues",
                columns: new[] { "NPCDialogueId", "Content", "DisplayOrder", "IsActive", "LinkedQuestId", "LinkedShopItemId", "NPCId", "ResponseType" },
                values: new object[,]
                {
                    { 1, "Ah, a new traveler. Welcome to the Elf Forest.", 1, true, 1, null, 1, "None" },
                    { 2, "This forest has been peaceful for centuries, but recently, dark forces have begun to gather.", 2, true, 1, null, 1, "None" },
                    { 3, "I need your help to protect this place. Come speak to me when you are ready to begin.", 3, true, 1, null, 1, "Quest" },
                    { 4, "Before we can confront the darkness, we need to prepare some basic remedies.", 1, true, 2, null, 1, "None" },
                    { 5, "The old willow clearing has some magical herbs we can use.", 2, true, 2, null, 1, "None" },
                    { 6, "Please head over there and gather 3 White Flowers for me.", 3, true, 2, null, 1, "Quest" },
                    { 10, "Now that you have your reward, it is time to learn how to defend yourself.", 1, true, 4, null, 1, "None" },
                    { 11, "In this world, skills are essential for survival. You cannot fight with bare hands alone.", 2, true, 4, null, 1, "None" },
                    { 12, "Open your Skill Panel and equip your first combat skill before you face real danger.", 3, true, 4, null, 1, "Quest" },
                    { 13, "Good, you are armed and ready. It is time to test your newfound abilities.", 1, true, 5, null, 1, "None" },
                    { 14, "The outskirts of our forest have been overrun by strange, aggressive slimes.", 2, true, 5, null, 1, "None" },
                    { 15, "Head out and defeat 3 SlimeLittle monsters to prove your worth to the village.", 3, true, 5, null, 1, "Quest" },
                    { 19, "Greetings, brave warrior. I am Lyra, the spirit of the Origin Tree.", 1, true, 7, null, 2, "None" },
                    { 20, "As you can see, the tree has been cursed and is slowly dying.", 2, true, 7, null, 2, "None" },
                    { 21, "Only the 4 Seal Books can cleanse it. You have one, but you must find the remaining three!", 3, true, 7, null, 2, "Quest" },
                    { 22, "Heh... So you are the one collecting the Seal Books?", 1, true, 8, null, 3, "None" },
                    { 23, "You know nothing of the true history of this world, or why the tree was cursed.", 2, true, 8, null, 3, "None" },
                    { 24, "If you want the truth, follow me through this portal. Don't keep me waiting.", 3, true, 8, null, 3, "Quest" },
                    { 25, "Welcome to the beach. We were teleported here by the portal.", 1, true, 9, null, 4, "None" },
                    { 26, "You seem to have no money for food. Why don't you look for some work?", 2, true, 9, null, 4, "None" },
                    { 27, "Go talk to Fa, he is nearby and might need some help.", 3, true, 9, null, 4, "Quest" },
                    { 28, "Ah, Elder Rowan sent you? Good timing.", 1, true, 10, null, 7, "None" },
                    { 29, "I need someone to help me harvest the fields.", 2, true, 10, null, 7, "None" },
                    { 30, "Please collect 8 Enchanted Pumpkins for me.", 3, true, 10, null, 7, "Quest" },
                    { 31, "Great job with the pumpkins! You are a hard worker.", 1, true, 11, null, 7, "None" },
                    { 32, "Now, I need these delivered to the city gate.", 2, true, 11, null, 7, "None" },
                    { 33, "Please take them to guard Tristan at the ruined city.", 3, true, 11, null, 7, "Quest" },
                    { 34, "Halt! Who goes there? Ah, you brought pumpkins from Fa?", 1, true, 12, null, 5, "None" },
                    { 35, "Something is wrong in the city... It is too quiet.", 2, true, 12, null, 5, "None" },
                    { 36, "Please go inside and investigate. Let me know if you find anything suspicious.", 3, true, 12, null, 5, "Quest" },
                    { 37, "What?! The people inside have all been massacred? Corpses everywhere?", 1, true, 13, null, 5, "None" },
                    { 38, "This is a disaster. We need someone strong to handle this.", 2, true, 13, null, 5, "None" },
                    { 39, "Please, go find the silver knight Arthur and report this!", 3, true, 13, null, 5, "Quest" },
                    { 43, "Splendid! You have trained well and cleared the dungeon.", 1, true, 15, null, 6, "None" },
                    { 44, "As promised, take this DarkExplosion skill and Silver Necklace.", 2, true, 15, null, 6, "None" },
                    { 45, "Now, use your power to defeat 10 evil monsters in the Ruined City!", 3, true, 15, null, 6, "Quest" },
                    { 49, "You did it! The dragon is slain. I cannot thank you enough.", 1, true, 17, null, 6, "None" },
                    { 50, "You ask about the mysterious figure? The one who did this?", 2, true, 17, null, 6, "None" },
                    { 51, "He went towards the frozen lands devastated by the codex. Head to the Frozen Mountains next.", 3, true, 17, null, 6, "Quest" },
                    { 55, "Your efforts have not gone unnoticed. The slimes are thinning out.", 1, true, 19, null, 8, "None" },
                    { 56, "However, our Priest Zephyr requires supplies for a ritual.", 2, true, 19, null, 8, "None" },
                    { 57, "Take this Magic Flour and deliver it to him at the mountain peak.", 3, true, 19, null, 8, "Quest" },
                    { 58, "Ah, the flour from the Queen! Thank you, traveler.", 1, true, 19, null, 9, "None" },
                    { 59, "The codex has warped the creatures here. The beasts have become feral and dangerous.", 1, true, 20, null, 9, "None" },
                    { 60, "To secure our borders, go slay 5 Ice Dragons on the mountain.", 2, true, 20, null, 9, "Quest" },
                    { 61, "Halt! This is the forbidden zone. None may enter.", 1, true, 21, null, 10, "None" },
                    { 62, "Wait... you have the aura of one who has fought the Ice Dragons.", 2, true, 21, null, 10, "None" },
                    { 63, "Since you made it this far, help me explore this dangerous area.", 3, true, 21, null, 10, "Quest" },
                    { 67, "Stay back! The undead are relentless today.", 1, true, 23, null, 11, "None" },
                    { 68, "An ancient power is leaking, causing skeletons to multiply out of control.", 2, true, 23, null, 11, "None" },
                    { 69, "I can't hold them off alone. Defeat 12 of them in the valley!", 3, true, 23, null, 11, "Quest" },
                    { 70, "The animals are fleeing from the abandoned village Tide-Knell. Investigate it and find Natalie.", 1, true, 24, null, 11, "Quest" },
                    { 71, "Are you here to help me? I cannot leave this place...", 2, true, 24, null, 12, "None" },
                    { 72, "Please... dig up what is buried under the small tree near the old well.", 3, true, 24, null, 12, "Quest" },
                    { 76, "You actually survived the waves and made it to this deserted island.", 1, true, 26, null, 13, "None" },
                    { 77, "I need your assistance to prepare a ritual of return.", 2, true, 26, null, 13, "None" },
                    { 78, "Help me collect 5 Ancient Leaves from the Northern Plateau.", 3, true, 26, null, 13, "Quest" },
                    { 82, "It is done. The UnderKing is defeated, and you have all 4 Seal Books.", 1, true, 28, null, 13, "None" },
                    { 83, "The fate of the Origin Tree now rests entirely in your hands.", 2, true, 28, null, 13, "None" },
                    { 84, "Farewell, hero. I will use my power to open a portal back to the Elf Forest. Save the tree!", 3, true, 28, null, 13, "Reward" },
                    { 85, "You have returned! And I can sense the power of the 4 Seal Books.", 1, true, 29, null, 2, "None" },
                    { 86, "The curse is breaking... The Origin Tree is finally healing!", 2, true, 29, null, 2, "None" },
                    { 87, "Thank you! The Origin Tree is saved. But this is not the end... To be continued.", 3, true, 29, null, 2, "Reward" }
                });

            migrationBuilder.InsertData(
                table: "Quests",
                columns: new[] { "QuestId", "BossMonsterId", "DefaultStatus", "Description", "IsActive", "MapName", "ObjectiveLocation", "ObjectiveTarget", "ObjectiveType", "QuestGiverName", "RegionName", "RequiredLevel", "RewardExperience", "RewardGems", "RewardGold", "RewardItemId", "RewardSkillId", "TargetAmount", "Title", "Type" },
                values: new object[,]
                {
                    { 3, null, "NotStarted", "Deliver the gathered flowers to Elder Rowan.", true, "ElfForest", "Elf Forest", "Elder Rowan", "Talk", "Elder Rowan", null, 1, 5, 5m, 5m, null, 10, 1, "[Chapter 1] Deliver White Flowers", "Main" },
                    { 6, 2, "NotStarted", "Slay the Swamp Demon and obtain its Seal Book.", true, "ElfForest", "Deep Woods", "SwampDemon", "Defeat", "Elder Rowan", null, 2, 25, 5m, 50m, null, null, 1, "[Chapter 1] The Swamp Demon", "Main" },
                    { 14, null, "NotStarted", "Speak with Arthur and learn about his internal injuries and sealed power. Enter Dungeon ID 2 to train and level up your strength.", true, "AutumnPumpkin", "Dungeon", "Dungeon_2", "Explore", "Arthur", null, 12, 15, 5m, 15m, 18, 9, 1, "[Chapter 2] The Silver Knight's Training", "Main" },
                    { 16, 7, "NotStarted", "Turn in the quest and get Arthur's recognition of your strength, receive quest to kill DragonBossIdle. Go kill dragon DragonBossIdle.", true, "AutumnPumpkin", "Ruined City", "DragonBossIdle", "Defeat", "Arthur", null, 5, 50, 5m, 100m, null, null, 1, "[Chapter 2] Slay the Dragon", "Main" },
                    { 18, null, "NotStarted", "Meet Queen Roselyn Aurora and defeat 8 Ice Slimes.", true, "FrozenMountain", "Snow Fields", "slime_ice", "Defeat", "Roselyn Aurora Queen", null, 6, 30, 5m, 30m, 31, null, 8, "[Chapter 3] The Ice Slimes", "Main" },
                    { 22, 10, "NotStarted", "Discover the truth of the codex and defeat GolemBoss to get the Golem Seal Book.", true, "FrozenMountain", "Forbidden Zone", "GolemBoss", "Defeat", "Roland", null, 8, 80, 5m, 150m, null, null, 1, "[Chapter 3] Truth of the Codex", "Main" },
                    { 25, null, "NotStarted", "Read Natalie's suicide letter and bury her remains under the ivy tree. Receive Mystic Key.", true, "AbandonedCastle", "Tide-Knell", "Natalie", "Talk", "Natalie", null, 10, 40, 5m, 40m, 33, null, 1, "[Chapter 4] Rest in Peace", "Main" },
                    { 27, 15, "NotStarted", "Defeat the UnderKing to claim the final UnderKing Seal Book.", true, "AbandonedCastle", "Deserted Island", "UnderKing", "Defeat", "Elf Guard", null, 11, 200, 5m, 300m, null, null, 1, "[Chapter 4] The UnderKing", "Main" }
                });

            migrationBuilder.InsertData(
                table: "NPCDialogues",
                columns: new[] { "NPCDialogueId", "Content", "DisplayOrder", "IsActive", "LinkedQuestId", "LinkedShopItemId", "NPCId", "ResponseType" },
                values: new object[,]
                {
                    { 7, "You have returned quickly. Did you find the flowers?", 1, true, 3, null, 1, "None" },
                    { 8, "Excellent, these are in perfect condition. They will make fine healing poultices.", 2, true, 3, null, 1, "None" },
                    { 9, "Thank you! Take this as a token of my appreciation.", 3, true, 3, null, 1, "Reward" },
                    { 16, "You handled those slimes well. But a much greater threat lurks in the deep woods.", 1, true, 6, null, 1, "None" },
                    { 17, "A terrible Swamp Demon has made its lair there, corrupting the land with its presence.", 2, true, 6, null, 1, "None" },
                    { 18, "You must destroy the Swamp Demon and claim the Swamp Seal Book it guards. We are counting on you!", 3, true, 6, null, 1, "Quest" },
                    { 40, "Greetings, warrior. I am Arthur, the silver knight.", 1, true, 14, null, 6, "None" },
                    { 41, "I suffered severe internal injuries and my power has been sealed away.", 2, true, 14, null, 6, "None" },
                    { 42, "You must train in Dungeon 2 to level up and unlock your true potential. Go!", 3, true, 14, null, 6, "Quest" },
                    { 46, "You have returned, and you are stronger than before.", 1, true, 16, null, 6, "None" },
                    { 47, "I recognize your true strength now. You are ready for the ultimate challenge.", 2, true, 16, null, 6, "None" },
                    { 48, "A terrible dragon threatens our existence. Go and slay the DragonBossIdle!", 3, true, 16, null, 6, "Quest" },
                    { 52, "Ah, a survivor from the ruins. I am Queen Roselyn Aurora.", 1, true, 18, null, 8, "None" },
                    { 53, "This land is devastated by the codex. Only volunteers remain to defend it.", 2, true, 18, null, 8, "None" },
                    { 54, "Please, clear out 8 ice slimes from the Snow Fields to help us.", 3, true, 18, null, 8, "Quest" },
                    { 64, "We have uncovered the origin of the codex... The truth is terrifying.", 1, true, 22, null, 10, "None" },
                    { 65, "A massive ancient golem guards the final piece of the puzzle.", 2, true, 22, null, 10, "None" },
                    { 66, "Defeat the giant GolemBoss to claim the Golem Seal Book! Do not fail us.", 3, true, 22, null, 10, "Quest" },
                    { 73, "Thank you for finding my remains. Now I can finally rest in peace.", 1, true, 25, null, 12, "None" },
                    { 74, "The ancient power leak was my doing. I am so sorry for the chaos.", 2, true, 25, null, 12, "None" },
                    { 75, "Take this key. It will unlock the doors to the island castle. Farewell.", 3, true, 25, null, 12, "Reward" },
                    { 79, "We have everything we need. But a dark presence blocks our path.", 1, true, 27, null, 13, "None" },
                    { 80, "The UnderKing himself has awakened, and he guards the final Seal Book.", 2, true, 27, null, 13, "None" },
                    { 81, "You must end his reign! Defeat the UnderKing and claim the book!", 3, true, 27, null, 13, "Quest" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_RoleId",
                table: "Accounts",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Achievements_RewardItemId",
                table: "Achievements",
                column: "RewardItemId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockContents_ContentId",
                table: "BlockContents",
                column: "ContentId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_RecipientId",
                table: "ChatMessages",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ReportedById",
                table: "ChatMessages",
                column: "ReportedById");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_SenderId",
                table: "ChatMessages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatModerationPenalties_ChatMessageId",
                table: "ChatModerationPenalties",
                column: "ChatMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatModerationPenalties_LockedUntil",
                table: "ChatModerationPenalties",
                column: "LockedUntil");

            migrationBuilder.CreateIndex(
                name: "IX_ChatModerationPenalties_PlayerProfileId",
                table: "ChatModerationPenalties",
                column: "PlayerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatModerationPenalties_ReporterId",
                table: "ChatModerationPenalties",
                column: "ReporterId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatModerationPenalties_WorldChatMessageId",
                table: "ChatModerationPenalties",
                column: "WorldChatMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_ChestItems_ChestId",
                table: "ChestItems",
                column: "ChestId");

            migrationBuilder.CreateIndex(
                name: "IX_ChestItems_ItemId",
                table: "ChestItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_CategoryContentId",
                table: "Contents",
                column: "CategoryContentId");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_CreatedByAccountAccountId",
                table: "Contents",
                column: "CreatedByAccountAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_SubCategoryContentId",
                table: "Contents",
                column: "SubCategoryContentId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyLoginRewards_RewardItemId",
                table: "DailyLoginRewards",
                column: "RewardItemId");

            migrationBuilder.CreateIndex(
                name: "IX_DungeonConfigs_ChestId",
                table: "DungeonConfigs",
                column: "ChestId");

            migrationBuilder.CreateIndex(
                name: "IX_DungeonProgresses_DungeonSessionId",
                table: "DungeonProgresses",
                column: "DungeonSessionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DungeonSessions_DungeonConfigId",
                table: "DungeonSessions",
                column: "DungeonConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_DungeonSessions_PlayerProfileId",
                table: "DungeonSessions",
                column: "PlayerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentStats_ItemId",
                table: "EquipmentStats",
                column: "ItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FriendBlocks_BlockedId",
                table: "FriendBlocks",
                column: "BlockedId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendBlocks_BlockerId",
                table: "FriendBlocks",
                column: "BlockerId");

            migrationBuilder.CreateIndex(
                name: "IX_Friends_AddresseeId",
                table: "Friends",
                column: "AddresseeId");

            migrationBuilder.CreateIndex(
                name: "IX_Friends_RequesterId",
                table: "Friends",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_GachaBannerItems_GachaBannerId",
                table: "GachaBannerItems",
                column: "GachaBannerId");

            migrationBuilder.CreateIndex(
                name: "IX_GachaBannerItems_ItemId",
                table: "GachaBannerItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_GachaPullHistories_GachaBannerId",
                table: "GachaPullHistories",
                column: "GachaBannerId");

            migrationBuilder.CreateIndex(
                name: "IX_GachaPullHistories_PlayerProfileId",
                table: "GachaPullHistories",
                column: "PlayerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_GachaPullHistories_RewardItemId",
                table: "GachaPullHistories",
                column: "RewardItemId");

            migrationBuilder.CreateIndex(
                name: "IX_GameSettings_CreatedByAccountAccountId",
                table: "GameSettings",
                column: "CreatedByAccountAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_GameSettings_UpdatedByAccountAccountId",
                table: "GameSettings",
                column: "UpdatedByAccountAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildApplications_GuildId",
                table: "GuildApplications",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildApplications_PlayerProfileId",
                table: "GuildApplications",
                column: "PlayerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildChatMessages_GuildId",
                table: "GuildChatMessages",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildChatMessages_SenderId",
                table: "GuildChatMessages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildInvitations_GuildId",
                table: "GuildInvitations",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildInvitations_InviteeId",
                table: "GuildInvitations",
                column: "InviteeId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildInvitations_InviterId",
                table: "GuildInvitations",
                column: "InviterId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildLogs_ActorProfileId",
                table: "GuildLogs",
                column: "ActorProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildLogs_GuildId",
                table: "GuildLogs",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildLogs_TargetProfileId",
                table: "GuildLogs",
                column: "TargetProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildMembers_GuildId",
                table: "GuildMembers",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildMembers_PlayerProfileId",
                table: "GuildMembers",
                column: "PlayerProfileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_CreatedByProfileId",
                table: "Guilds",
                column: "CreatedByProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_LeaderId",
                table: "Guilds",
                column: "LeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_ItemId",
                table: "InventoryItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_PlayerProfileId",
                table: "InventoryItems",
                column: "PlayerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_MailRewardItems_ItemId",
                table: "MailRewardItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_MailRewardItems_MailId",
                table: "MailRewardItems",
                column: "MailId");

            migrationBuilder.CreateIndex(
                name: "IX_Mails_PlayerProfileId",
                table: "Mails",
                column: "PlayerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_MonsterDrops_ItemId",
                table: "MonsterDrops",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_MonsterDrops_MonsterId",
                table: "MonsterDrops",
                column: "MonsterId");

            migrationBuilder.CreateIndex(
                name: "IX_MonsterSpawns_DungeonId",
                table: "MonsterSpawns",
                column: "DungeonId");

            migrationBuilder.CreateIndex(
                name: "IX_MonsterSpawns_MonsterId",
                table: "MonsterSpawns",
                column: "MonsterId");

            migrationBuilder.CreateIndex(
                name: "IX_NPCDialogues_LinkedQuestId",
                table: "NPCDialogues",
                column: "LinkedQuestId");

            migrationBuilder.CreateIndex(
                name: "IX_NPCDialogues_LinkedShopItemId",
                table: "NPCDialogues",
                column: "LinkedShopItemId");

            migrationBuilder.CreateIndex(
                name: "IX_NPCDialogues_NPCId",
                table: "NPCDialogues",
                column: "NPCId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerAchievements_AchievementId",
                table: "PlayerAchievements",
                column: "AchievementId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerAchievements_PlayerProfileId",
                table: "PlayerAchievements",
                column: "PlayerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerAnnouncements_GameAnnouncementId",
                table: "PlayerAnnouncements",
                column: "GameAnnouncementId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerAnnouncements_PlayerProfileId",
                table: "PlayerAnnouncements",
                column: "PlayerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerBuffs_PlayerProfileId",
                table: "PlayerBuffs",
                column: "PlayerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerChests_ChestId",
                table: "PlayerChests",
                column: "ChestId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerChests_PlayerProfileId",
                table: "PlayerChests",
                column: "PlayerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCurrencyLogs_PlayerProfileId",
                table: "PlayerCurrencyLogs",
                column: "PlayerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerDailyLogins_PlayerProfileId",
                table: "PlayerDailyLogins",
                column: "PlayerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerMonsterDiscoveries_MonsterId",
                table: "PlayerMonsterDiscoveries",
                column: "MonsterId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerMonsterDiscoveries_PlayerProfileId_MonsterId",
                table: "PlayerMonsterDiscoveries",
                columns: new[] { "PlayerProfileId", "MonsterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerProfiles_AccountId",
                table: "PlayerProfiles",
                column: "AccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerQuests_PlayerProfileId",
                table: "PlayerQuests",
                column: "PlayerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerQuests_QuestId",
                table: "PlayerQuests",
                column: "QuestId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerShopRefreshStates_PlayerProfileId_ShopDateUtc",
                table: "PlayerShopRefreshStates",
                columns: new[] { "PlayerProfileId", "ShopDateUtc" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerSkills_PlayerProfileId",
                table: "PlayerSkills",
                column: "PlayerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerSkills_SkillId",
                table: "PlayerSkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerSkins_PlayerProfileId",
                table: "PlayerSkins",
                column: "PlayerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerSkins_SkinId",
                table: "PlayerSkins",
                column: "SkinId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStats_PlayerProfileId",
                table: "PlayerStats",
                column: "PlayerProfileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStatsSnapshots_PlayerProfileId",
                table: "PlayerStatsSnapshots",
                column: "PlayerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseHistories_PlayerProfileId",
                table: "PurchaseHistories",
                column: "PlayerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseHistories_ShopItemId",
                table: "PurchaseHistories",
                column: "ShopItemId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestRewardItems_ItemId",
                table: "QuestRewardItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestRewardItems_QuestId_ItemId",
                table: "QuestRewardItems",
                columns: new[] { "QuestId", "ItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestRewardSkills_QuestId_SkillId",
                table: "QuestRewardSkills",
                columns: new[] { "QuestId", "SkillId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestRewardSkills_SkillId",
                table: "QuestRewardSkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_Quests_BossMonsterId",
                table: "Quests",
                column: "BossMonsterId");

            migrationBuilder.CreateIndex(
                name: "IX_Quests_RewardItemId",
                table: "Quests",
                column: "RewardItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Quests_RewardSkillId",
                table: "Quests",
                column: "RewardSkillId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopItems_ItemId",
                table: "ShopItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopItems_ShopSection",
                table: "ShopItems",
                column: "ShopSection");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategoryContents_CategoryContentId",
                table: "SubCategoryContents",
                column: "CategoryContentId");

            migrationBuilder.CreateIndex(
                name: "IX_WorldChatMessages_ReportedById",
                table: "WorldChatMessages",
                column: "ReportedById");

            migrationBuilder.CreateIndex(
                name: "IX_WorldChatMessages_SenderId",
                table: "WorldChatMessages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_WorldChatMessages_SentAt",
                table: "WorldChatMessages",
                column: "SentAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlockContents");

            migrationBuilder.DropTable(
                name: "ChatModerationPenalties");

            migrationBuilder.DropTable(
                name: "ChestItems");

            migrationBuilder.DropTable(
                name: "ClassConfigs");

            migrationBuilder.DropTable(
                name: "DailyLoginRewards");

            migrationBuilder.DropTable(
                name: "DungeonProgresses");

            migrationBuilder.DropTable(
                name: "EquipmentStats");

            migrationBuilder.DropTable(
                name: "FriendBlocks");

            migrationBuilder.DropTable(
                name: "Friends");

            migrationBuilder.DropTable(
                name: "GachaBannerItems");

            migrationBuilder.DropTable(
                name: "GachaPullHistories");

            migrationBuilder.DropTable(
                name: "GameSettings");

            migrationBuilder.DropTable(
                name: "GuildApplications");

            migrationBuilder.DropTable(
                name: "GuildChatMessages");

            migrationBuilder.DropTable(
                name: "GuildInvitations");

            migrationBuilder.DropTable(
                name: "GuildLogs");

            migrationBuilder.DropTable(
                name: "GuildMembers");

            migrationBuilder.DropTable(
                name: "InventoryItems");

            migrationBuilder.DropTable(
                name: "MailRewardItems");

            migrationBuilder.DropTable(
                name: "MonsterDrops");

            migrationBuilder.DropTable(
                name: "MonsterSpawns");

            migrationBuilder.DropTable(
                name: "NPCDialogues");

            migrationBuilder.DropTable(
                name: "PlayerAchievements");

            migrationBuilder.DropTable(
                name: "PlayerAnnouncements");

            migrationBuilder.DropTable(
                name: "PlayerBuffs");

            migrationBuilder.DropTable(
                name: "PlayerChests");

            migrationBuilder.DropTable(
                name: "PlayerCurrencyLogs");

            migrationBuilder.DropTable(
                name: "PlayerDailyLogins");

            migrationBuilder.DropTable(
                name: "PlayerMonsterDiscoveries");

            migrationBuilder.DropTable(
                name: "PlayerQuests");

            migrationBuilder.DropTable(
                name: "PlayerShopRefreshStates");

            migrationBuilder.DropTable(
                name: "PlayerSkills");

            migrationBuilder.DropTable(
                name: "PlayerSkins");

            migrationBuilder.DropTable(
                name: "PlayerStats");

            migrationBuilder.DropTable(
                name: "PlayerStatsSnapshots");

            migrationBuilder.DropTable(
                name: "PurchaseHistories");

            migrationBuilder.DropTable(
                name: "QuestRewardItems");

            migrationBuilder.DropTable(
                name: "QuestRewardSkills");

            migrationBuilder.DropTable(
                name: "Contents");

            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "WorldChatMessages");

            migrationBuilder.DropTable(
                name: "DungeonSessions");

            migrationBuilder.DropTable(
                name: "GachaBanners");

            migrationBuilder.DropTable(
                name: "Guilds");

            migrationBuilder.DropTable(
                name: "Mails");

            migrationBuilder.DropTable(
                name: "Dungeons");

            migrationBuilder.DropTable(
                name: "NPCs");

            migrationBuilder.DropTable(
                name: "Achievements");

            migrationBuilder.DropTable(
                name: "GameAnnouncements");

            migrationBuilder.DropTable(
                name: "Skins");

            migrationBuilder.DropTable(
                name: "ShopItems");

            migrationBuilder.DropTable(
                name: "Quests");

            migrationBuilder.DropTable(
                name: "SubCategoryContents");

            migrationBuilder.DropTable(
                name: "DungeonConfigs");

            migrationBuilder.DropTable(
                name: "PlayerProfiles");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Monsters");

            migrationBuilder.DropTable(
                name: "Skills");

            migrationBuilder.DropTable(
                name: "CategoryContents");

            migrationBuilder.DropTable(
                name: "Chests");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
