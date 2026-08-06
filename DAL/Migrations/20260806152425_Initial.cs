using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
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
                table: "Achievements",
                columns: new[] { "AchievementId", "BuffDescription", "CreatedAt", "Description", "IconUrl", "IsActive", "Name", "Point", "RequiredValue", "RewardGem", "RewardGold", "RewardItemId", "RewardQuantity", "Type" },
                values: new object[,]
                {
                    { 1, "+2% Max HP", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Complete the first chapter", "pioneer", true, "Pioneer", 0, 1, 0, 0m, null, 1, "Progression" },
                    { 2, "+2% Attack", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Defeat 1,000 monsters", "monster_hunter", true, "Monster Hunter", 0, 1000, 0, 0m, null, 1, "Combat" },
                    { 3, "+2% Critical Rate", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Reach the required cumulative Critical Rate", "deadeye", true, "Deadeye", 0, 100, 0, 0m, null, 1, "Progression" },
                    { 4, "+3% Defense", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Die fewer than 10 times before Level 30", "unyielding", true, "The Unyielding", 0, 1, 0, 0m, null, 1, "Progression" },
                    { 5, "+3% Movement Speed", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Explore every region on the map", "swift_wanderer", true, "Swift Wanderer", 0, 1, 0, 0m, null, 1, "Exploration" },
                    { 6, "+5% Gold Gain", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Open 500 treasure chests", "treasure_seeker", true, "Treasure Seeker", 0, 500, 0, 0m, null, 1, "Collection" },
                    { 7, "+3% EXP Gain", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Complete 100 quests", "adventurer", true, "Adventurer", 0, 100, 0, 0m, null, 1, "Progression" },
                    { 8, "+2% Max HP, +2% Defense", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Complete 100 co-op dungeons", "faithful_companion", true, "Faithful Companion", 0, 100, 0, 0m, null, 1, "Social" },
                    { 9, "+3% Damage to Bosses", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Defeat every Boss at least once", "conqueror", true, "Conqueror", 0, 1, 0, 0m, null, 1, "Combat" },
                    { 10, "+2% to All Stats (HP, ATK, DEF)", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Reach the maximum level and complete the main storyline", "legend_elarion", true, "Legend of Elarion", 0, 1, 0, 0m, null, 1, "Progression" }
                });

            migrationBuilder.InsertData(
                table: "CategoryContents",
                columns: new[] { "CategoryContentId", "CreatedAt", "Description", "IconUrl", "IsActive", "Name", "Slug" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "An ancient woodland inhabited by the Elven race, once protected by the Origin Tree before the curse befell the land.", null, true, "Elf Forest", "elf-forest" },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A collection of four ancient elemental seal books containing mysterious powers needed to unlock the realm's secrets.", null, true, "Seal Books", "seal-books" },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A journal recording legends, lore, and key storyline events unfolding across the realms.", null, true, "The Chronicle", "the-chronicle" }
                });

            migrationBuilder.InsertData(
                table: "ClassConfigs",
                columns: new[] { "ClassConfigId", "Atk", "AttackSpeed", "ClassName", "CritDamage", "CritRate", "DamageBonus", "Def", "MaxHp", "MoveSpeed" },
                values: new object[,]
                {
                    { 1, 42, 100, "Knight", 150, 5, 0, 45, 620, 100 },
                    { 2, 52, 100, "Archer", 150, 5, 0, 26, 420, 100 },
                    { 3, 46, 100, "Mage", 150, 5, 0, 20, 360, 100 }
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
                    { 31, 50m, 0.5f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Mystical flour imbued with purifying magic. Reduces your corruption by 50% when consumed.", null, true, 99, "Magic Flour", "Uncommon", "None", "Consumable" },
                    { 32, 0m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A skull radiating with ghostly presence.", null, true, 99, "Spirit Skull", "Common", "None", "QuestItem" },
                    { 33, 0m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A key that opens the castle on the deserted island.", null, true, 1, "Mystic Key", "Epic", "None", "QuestItem" },
                    { 34, 0m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A remembrance token recovered from the dead of Tide-Knell.", null, true, 99, "Tide-Knell Remembrance", "Common", "None", "QuestItem" },
                    { 35, 0m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A keepsake carrying a fragment of Natalie's family memories.", null, true, 99, "Natalie's Memory", "Common", "None", "QuestItem" },
                    { 36, 0m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A relic left by the wardens who sealed King Aderyn beneath the island.", null, true, 99, "Warden Relic", "Common", "None", "QuestItem" },
                    { 901, 100m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A sharp sword dropped by the SwampDemon.", null, true, 1, "Swamp Sword", "Rare", "Weapon", "Weapon" },
                    { 902, 150m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A sturdy armor dropped by the SwampDemon.", null, true, 1, "Swamp Armor", "Rare", "Armor", "Armor" },
                    { 903, 500m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A legendary sword dropped by DragonBossIdle.", null, true, 1, "Dragon Boss Sword", "Legendary", "Weapon", "Weapon" },
                    { 904, 600m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A legendary armor dropped by DragonBossIdle.", null, true, 1, "Dragon Boss Armor", "Legendary", "Armor", "Armor" },
                    { 905, 800m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Heavy stone gloves dropped by GolemBoss.", null, true, 1, "Golem Boss Gloves", "Legendary", "Gloves", "Armor" },
                    { 906, 1000m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A massive stone armor dropped by GolemBoss.", null, true, 1, "Golem Boss Armor", "Legendary", "Armor", "Armor" },
                    { 907, 1500m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A royal cursed sword dropped by UnderKing.", null, true, 1, "UnderKing Sword", "Legendary", "Weapon", "Weapon" },
                    { 908, 2000m, 0f, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "The crown of the UnderKing.", null, true, 1, "UnderKing Crown", "Legendary", "Helmet", "Armor" }
                });

            migrationBuilder.InsertData(
                table: "Monsters",
                columns: new[] { "MonsterId", "Atk", "AttackSpeed", "CreatedAt", "CritDamage", "CritRate", "Def", "Description", "ExperienceReward", "GemReward", "GoldReward", "ImageUrl", "IsActive", "Level", "MaxHp", "MoveSpeed", "Name", "Type" },
                values: new object[,]
                {
                    { 1, 30, 85, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 130, 5, 2, "A basic slime monster. The first thing a new player ever fights.", 4, 0m, 8m, null, true, 1, 300, 70, "SlimeLittle", "Normal" },
                    { 2, 32, 100, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 150, 12, 10, "A dangerous swamp demon brooding over an old relic in the deep woods.", 22, 5m, 110m, null, true, 3, 1380, 90, "SwampDemon", "Boss" },
                    { 3, 39, 95, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 140, 8, 5, "A water elemental monster from the forest marshes.", 4, 0m, 8m, null, true, 3, 400, 80, "WaterElemental", "Normal" },
                    { 4, 47, 100, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 160, 15, 12, "A fierce dragon nesting in the ruined city.", 6, 0m, 13m, null, true, 6, 560, 110, "Dragon", "Normal" },
                    { 5, 48, 100, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 160, 15, 14, "A frosty blue dragon.", 6, 0m, 13m, null, true, 7, 580, 110, "BlueDragonFrost", "Normal" },
                    { 6, 49, 105, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 160, 15, 15, "A forest green dragon.", 6, 0m, 13m, null, true, 7, 590, 110, "GreenDragonForest", "Normal" },
                    { 7, 53, 100, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 175, 20, 22, "The dragon that broke the city. It never leaves its nest, so MoveSpeed is 0 by design.", 35, 10m, 176m, null, true, 7, 2930, 0, "DragonBossIdle", "Boss" },
                    { 8, 50, 90, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 150, 10, 15, "An icy slime that creeps onto the snow fields at night.", 10, 0m, 19m, null, true, 7, 620, 75, "SlimeIce", "Normal" },
                    { 9, 55, 105, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 165, 20, 18, "An icy dragon driven down the mountain against the people below.", 10, 0m, 19m, null, true, 9, 840, 115, "IceDragon", "Normal" },
                    { 10, 65, 90, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 170, 20, 28, "A giant stone golem sealed inside the Doomed Land of Snow.", 53, 15m, 264m, null, true, 9, 4300, 80, "GolemBoss", "Boss" },
                    { 11, 61, 100, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 160, 15, 20, "An undead orc skeleton risen in the valley of Tide-Knell.", 13, 0m, 26m, null, true, 9, 850, 95, "OrcSkeleton", "Normal" },
                    { 12, 71, 105, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 160, 15, 22, "A melee skeleton warrior.", 13, 0m, 26m, null, true, 11, 1050, 100, "SkeletonMelee", "Normal" },
                    { 13, 78, 115, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 165, 22, 16, "A ranged skeleton archer. Glass cannon: highest Atk of the skeletons, lowest Def.", 13, 0m, 26m, null, true, 12, 1160, 100, "SkeletonArcher", "Normal" },
                    { 14, 42, 100, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 160, 15, 10, "A floating ghost haunting the ruined quarter.", 6, 0m, 13m, null, true, 4, 480, 95, "Ghost", "Normal" },
                    { 15, 94, 100, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 180, 25, 35, "Once a great human king who accepted two Seal Books and imprisoned himself beneath the deserted island to spare the world their curse. Centuries of darkness eroded the hero into the UnderKing.", 70, 30m, 352m, null, true, 12, 6040, 95, "UnderKing", "Boss" },
                    { 16, 51, 100, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 165, 20, 18, "A terrifying demon.", 10, 0m, 19m, null, true, 8, 730, 95, "Demon", "Normal" },
                    { 17, 45, 100, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 150, 12, 13, "A strong goblin warrior.", 6, 0m, 13m, null, true, 5, 530, 95, "GoblinWarrior", "Normal" },
                    { 18, 44, 100, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 150, 10, 10, "A goblin spearman.", 6, 0m, 13m, null, true, 5, 510, 100, "GoblinSpear", "Normal" },
                    { 19, 46, 90, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 165, 15, 19, "The brutal ogre holding the Goblin barracks. Dungeon 5 boss.", 35, 10m, 176m, null, true, 7, 2560, 85, "Ogre", "Boss" },
                    { 20, 73, 100, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 175, 22, 30, "A formidable orc warlord guarding the gate to the underworld. Dungeon 6 boss.", 70, 30m, 352m, null, true, 12, 4490, 95, "OrcWarlord", "Boss" },
                    { 21, 54, 100, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 150, 12, 16, "The spirit that never leaves the golem's side. Fought together with GolemBoss.", 53, 15m, 264m, null, true, 9, 3230, 100, "IceFairy", "Boss" },
                    { 22, 41, 100, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 165, 18, 18, "A fierce goblin warlord holding the Goblin Grounds.", 35, 10m, 176m, null, true, 7, 2180, 95, "GoblinWarlord", "Boss" },
                    { 23, 43, 90, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 155, 10, 7, "A dark necromancer casting dark spells.", 6, 0m, 13m, null, true, 4, 500, 85, "NecromancerCast", "Normal" },
                    { 24, 40, 110, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 150, 12, 6, "A rogue robber archer wielding a crossbow.", 6, 0m, 13m, null, true, 3, 440, 100, "RobberArcher", "Normal" },
                    { 25, 41, 115, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 160, 18, 9, "A stealthy robber assassin wielding a sword and shield.", 6, 0m, 13m, null, true, 3, 460, 105, "RobberAssassin", "Normal" },
                    { 26, 46, 95, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 150, 10, 15, "A heavy red guard soldier carrying a mace and shield.", 6, 0m, 13m, null, true, 6, 540, 85, "RedGuard", "Normal" },
                    { 27, 65, 95, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 160, 15, 24, "An orc skeleton standing watch in the valley of Tide-Knell. Slower and tougher than its roaming kin.", 13, 0m, 26m, null, true, 10, 950, 90, "OrcSkeletonAfk", "Normal" }
                });

            migrationBuilder.InsertData(
                table: "NPCs",
                columns: new[] { "NPCId", "Description", "IconUrl", "InteractionRadius", "IsActive", "MapName", "Name", "PositionX", "PositionY", "Type" },
                values: new object[,]
                {
                    { 1, "The wise guide of the Elf Forest.", null, 2.5f, true, "ElfForest", "Elder Rowan", -0.69999999999999996, 18.5, "QuestGiver" },
                    { 2, "A spirit of the forest.", null, 2.5f, true, "ElfForest", "Lyra", 30.0, -6.0, "QuestGiver" },
                    { 3, "A mysterious figure in a cloak.", null, 2.5f, true, "ElfForest", "Mysterious Figure", 14.0, -47.5, "QuestGiver" },
                    { 4, "A weathered guide in the pumpkin town.", null, 2.5f, true, "AutumnPumpkin", "Drake", -105.5, 40.700000000000003, "QuestGiver" },
                    { 5, "The city gate guard.", null, 2.5f, true, "AutumnPumpkin", "Tristan", -97.299999999999997, 21.399999999999999, "QuestGiver" },
                    { 6, "The silver knight.", null, 2.5f, true, "AutumnPumpkin", "Arthur", -32.0, 58.0, "QuestGiver" },
                    { 7, "A farmer collecting enchanted pumpkins.", null, 2.5f, true, "AutumnPumpkin", "Fa", -101.0, -26.0, "QuestGiver" },
                    { 8, "Queen of the frozen lands.", null, 2.5f, true, "FrozenMountain", "Roselyn Aurora Queen", 146.8143, -11.63209, "QuestGiver" },
                    { 9, "The witch and disguised priest.", null, 2.5f, true, "FrozenMountain", "Zephyr", -4.8736499999999996, 25.72625, "QuestGiver" },
                    { 10, "The forbidden zone guard.", null, 2.5f, true, "FrozenMountain", "Roland", 130.43000000000001, 28.789999999999999, "QuestGiver" },
                    { 11, "A battle-worn soldier who returned to Tide-Knell too late — Natalie's father, still guarding the valley from the dead.", null, 2.5f, true, "AbandonedCastle", "Valiant Warrior", 29.82, 129.09999999999999, "QuestGiver" },
                    { 12, "The ghost of a lonely girl whose desperate wish for friends doomed Tide-Knell.", null, 2.5f, true, "AbandonedCastle", "Natalie", -10.65, 58.850000000000001, "QuestGiver" },
                    { 13, "The lone guard of the deserted island.", null, 2.5f, true, "AbandonedCastle", "Elf Guard", -101.83, 32.899999999999999, "QuestGiver" },
                    { 14, "Captain of the snow-field militia.", null, 2.5f, true, "FrozenMountain", "Cedric", 5.5300000000000002, -8.6199999999999992, "QuestGiver" },
                    { 15, "The last keeper of King Aderyn's history, living among the island ruins.", null, 2.5f, true, "AbandonedCastle", "Brother Cael", -119.29000000000001, -10.890000000000001, "QuestGiver" }
                });

            migrationBuilder.InsertData(
                table: "Quests",
                columns: new[] { "QuestId", "BossMonsterId", "DefaultStatus", "Description", "IsActive", "MapName", "ObjectiveLocation", "ObjectiveTarget", "ObjectiveType", "QuestGiverName", "RegionName", "RequiredLevel", "RewardExperience", "RewardGems", "RewardGold", "RewardItemId", "RewardSkillId", "TargetAmount", "Title", "Type" },
                values: new object[,]
                {
                    { 1, null, "NotStarted", "You wake at the edge of the Elf Forest with no memory of how you arrived. Elder Rowan is waiting by the great roots — go to him and hear why the forest called you here.", true, "ElfForest", "Elf Forest", "Elder Rowan", "Talk", "Elder Rowan", null, 1, 15, 3m, 20m, null, null, 1, "[Chapter 1] A Word with Elder Rowan", "Main" },
                    { 2, null, "NotStarted", "The elders brew their healing draught from white flowers that only bloom in the shade of the old woods. Search the clearings and gather 3 White Flowers for Elder Rowan.", true, "ElfForest", "Elf Forest", "White Flower", "Collect", "Elder Rowan", null, 1, 15, 3m, 20m, null, null, 3, "[Chapter 1] Gather White Flowers", "Main" },
                    { 4, null, "NotStarted", "A skill is useless until it sits in your hand. Open the Skill panel and equip the technique Elder Rowan just taught you.", true, "ElfForest", "Elf Forest", "Skill Panel", "EquipSkill", "Elder Rowan", null, 1, 15, 3m, 20m, null, null, 1, "[Chapter 1] Equip Your First Skill", "Main" },
                    { 5, null, "NotStarted", "Little slimes have crept out of the marsh and are eating the flower beds. Put your new skill to work and defeat 3 of them.", true, "ElfForest", "Elf Forest", "Slime Little", "Defeat", "Elder Rowan", null, 1, 15, 3m, 20m, null, null, 3, "[Chapter 1] Cull the Little Slimes", "Main" },
                    { 7, null, "NotStarted", "Rowan cannot name the relic you took from the swamp. Carry it to Lyra at the Origin Tree — she is older than every elf alive, and she will know what you are holding.", true, "ElfForest", "Origin Tree", "Lyra", "Talk", "Lyra", null, 2, 15, 3m, 20m, null, null, 1, "[Chapter 1] Lyra and the Origin Tree", "Main" },
                    { 8, null, "NotStarted", "A cloaked figure has been watching you since you woke, and now walks into a portal at the forest edge. Step through it before the way closes.", true, "ElfForest", "Elf Forest", "Portal", "Explore", "Mysterious Figure", null, 2, 15, 7m, 55m, null, null, 1, "[Chapter 1] Follow the Cloaked Figure", "Main" },
                    { 9, null, "NotStarted", "The portal spits you onto a cold beach under an autumn sky. Climb to the castle and find Drake, the one soul here willing to speak to a stranger, and ask what land this is.", true, "AutumnPumpkin", "Autumn Pumpkin", "Drake", "Talk", "Drake", null, 3, 10, 4m, 30m, null, null, 1, "[Chapter 2] Ask Where You Are", "Main" },
                    { 10, null, "NotStarted", "You have no coin in this land and no one gives bread away. Farmer Fa will trade a meal for labour: pick 8 Enchanted Pumpkins from his field.", true, "AutumnPumpkin", "Pumpkin Town", "Enchanted Pumpkin", "Collect", "Fa", null, 3, 10, 4m, 30m, null, null, 8, "[Chapter 2] Harvest for Your Supper", "Main" },
                    { 11, null, "NotStarted", "Fa is too old to make the road alone. Carry the harvest to the city gate and hand it to the guard Tristan.", true, "AutumnPumpkin", "City Gate", "Tristan", "Talk", "Fa", null, 3, 10, 4m, 30m, null, null, 1, "[Chapter 2] Deliver the Harvest", "Main" },
                    { 12, null, "NotStarted", "Beyond the gate the city is silent and the streets are full of the dead. Examine 5 of the bodies and learn what killed them.", true, "AutumnPumpkin", "Ruined City", "Corpse", "Interact", "Tristan", null, 3, 10, 4m, 30m, null, null, 5, "[Chapter 2] Examine the Fallen", "Main" },
                    { 13, null, "NotStarted", "Tristan pales at your report: only one man ever held these ruins. Search the city for the silver knight Arthur and ask for his help.", true, "AutumnPumpkin", "Ruined City", "Arthur", "Talk", "Tristan", null, 3, 10, 4m, 30m, null, null, 1, "[Chapter 2] Seek the Silver Knight", "Main" },
                    { 15, null, "NotStarted", "Arthur will not send you at a dragon on faith. He sets four trials, and the first is the robbers holding the eastern camp. Cut down 6 of them.", true, "AutumnPumpkin", "Robber Camp", "Robber", "Defeat", "Arthur", null, 4, 10, 4m, 30m, null, null, 6, "[Chapter 2] Trial I: The Robber Camp", "Main" },
                    { 16, null, "NotStarted", "One trial stands to your name. The second is the haunted quarter - ghosts, necromancers, and the red guard who died at their posts. Put down 10.", true, "AutumnPumpkin", "Haunted Quarter", "Ghost", "Defeat", "Arthur", null, 4, 10, 4m, 30m, null, null, 10, "[Chapter 2] Trial II: The Haunted Quarter", "Main" },
                    { 17, null, "NotStarted", "Two trials done. The third lies south of the ruins, where goblin spear and axe bands have dug in. Break 3 of them.", true, "AutumnPumpkin", "Goblin Grounds", "Goblin", "Defeat", "Arthur", null, 4, 10, 4m, 30m, null, null, 3, "[Chapter 2] Trial III: The Goblin Grounds", "Main" },
                    { 20, null, "NotStarted", "Return to Arthur for the knight's thanks and ask where the cursed codex came from. He points north, to a kingdom the codex froze solid.", true, "AutumnPumpkin", "Ruined City", "Arthur", "Talk", "Arthur", null, 5, 10, 10m, 80m, null, null, 1, "[Chapter 2] Arthur's Parting Words", "Main" },
                    { 21, null, "NotStarted", "Cedric holds the snow fields with farmers and borrowed spears, and he has no reason to trust a stranger off the ice road. The slimes are on his fields tonight. Defeat 8 of them and he will hear you out.", true, "FrozenMountain", "Snow Fields", "Slime Ice", "Defeat", "Cedric", null, 6, 15, 5m, 40m, null, null, 8, "[Chapter 3] The Ice Slimes", "Main" },
                    { 23, null, "NotStarted", "The Queen speaks of the ancient king whose statue this kingdom still honours, and of a priest who studies the old magics. Deliver her Magic Flour to Zephyr and ask him what she could not answer.", true, "FrozenMountain", "Frozen Mountain", "Zephyr", "Talk", "Roselyn Aurora Queen", null, 6, 15, 5m, 40m, null, null, 1, "[Chapter 3] Magic Flour for the Priest", "Main" },
                    { 24, null, "NotStarted", "Zephyr has studied the vanished seal books for thirty years. Something is driving the ice dragons against the people below. Bring down 5 of them on the mountain and report what you saw.", true, "FrozenMountain", "Frozen Mountain", "Ice Dragon", "Defeat", "Zephyr", null, 7, 15, 5m, 40m, null, null, 5, "[Chapter 3] Dragons of Snow", "Main" },
                    { 25, null, "NotStarted", "Zephyr shares what he suspects: the codex may have been corrupted, not born evil. The rest lies in the sealed north, The Doomed Land of Snow. Find the guard Roland and ask for passage.", true, "FrozenMountain", "Forbidden Zone", "Roland", "Talk", "Roland", null, 7, 15, 5m, 40m, null, null, 1, "[Chapter 3] The Forbidden Zone", "Main" },
                    { 27, null, "NotStarted", "Roland is waiting where you left him, and what you carry out of the ban is heavier than a book. Speak with him and put together what was really done to the guardians.", true, "FrozenMountain", "Forbidden Zone", "Roland", "Talk", "Roland", null, 8, 15, 13m, 105m, null, null, 1, "[Chapter 3] Truth of the Codex", "Main" },
                    { 28, null, "NotStarted", "The Valiant Warrior is Natalie's father, returned from war to find Tide-Knell dead. Help him put down 12 skeletons and hold the valley.", true, "AbandonedCastle", "Valley", "Skeleton", "Defeat", "Valiant Warrior", null, 9, 15, 6m, 50m, null, null, 12, "[Chapter 4] Break the Skeleton Army", "Main" },
                    { 29, null, "NotStarted", "Recover 5 remembrance tokens so the Valiant Warrior can name the people he is forced to fight.", true, "AbandonedCastle", "Tide-Knell", "Tide-Knell Remembrance", "Collect", "Valiant Warrior", null, 9, 15, 6m, 50m, null, null, 5, "[Chapter 4] Names Beneath the Bone", "Main" },
                    { 31, null, "NotStarted", "Find 3 traces of the old seal around the cursed well and force its promise into the open.", true, "AbandonedCastle", "Tide-Knell", "Cursed Well", "Interact", "Natalie", null, 10, 15, 6m, 50m, null, null, 3, "[Chapter 4] The Voice Beneath the Well", "Main" },
                    { 32, null, "NotStarted", "Find 3 memories left by Natalie's father and let his daughter hear the truth.", true, "AbandonedCastle", "Tide-Knell", "Natalie's Memory", "Collect", "Valiant Warrior", null, 10, 15, 6m, 50m, null, null, 3, "[Chapter 4] The Father's Last Letter", "Main" },
                    { 34, null, "NotStarted", "Use Natalie's Mystic Key at the bridge gate and open the road to the deserted island.", true, "AbandonedCastle", "Bridge", "Locked Bridge Gate", "Interact", "Valiant Warrior", null, 10, 15, 6m, 50m, null, null, 1, "[Chapter 4] The Key to the Island", "Main" },
                    { 35, null, "NotStarted", "Gather 5 Ancient Leaves to restore the old rite and open King Aderyn's prison.", true, "AbandonedCastle", "Northern Plateau", "Ancient Leaves", "Collect", "Elf Guard", null, 10, 15, 6m, 50m, null, null, 5, "[Chapter 4] Ancient Leaves of the Isle", "Main" },
                    { 36, null, "NotStarted", "Recover 4 relics from the old sealing party and confront the Elf Guard's guilt.", true, "AbandonedCastle", "Deserted Island", "Warden Relic", "Collect", "Elf Guard", null, 11, 15, 6m, 50m, null, null, 4, "[Chapter 4] The Warden's Oath", "Main" },
                    { 37, null, "NotStarted", "Cleanse 3 cursed roots in King Aderyn's abandoned garden.", true, "AbandonedCastle", "Northern Plateau", "Cursed Root", "Interact", "Brother Cael", null, 11, 15, 6m, 50m, null, null, 3, "[Chapter 4] The King's Garden", "Main" },
                    { 38, null, "NotStarted", "Read 3 memory fragments and learn why King Aderyn chose imprisonment before entering the crypt.", true, "AbandonedCastle", "Deserted Island", "Aderyn Memory", "Interact", "Brother Cael", null, 11, 15, 6m, 50m, null, null, 3, "[Chapter 4] The Man Beneath the Crown", "Main" },
                    { 40, null, "NotStarted", "Hear the Elf Guard's farewell to his old friend, then open the portal back to the Elf Forest.", true, "AbandonedCastle", "Deserted Island", "Elf Guard", "Talk", "Elf Guard", null, 12, 15, 16m, 130m, null, null, 1, "[Chapter 4] Ask for the Way Home", "Main" },
                    { 41, null, "NotStarted", "You are home, and the Origin Tree is worse than you left it. Bring all four Seal Books to Lyra.", true, "ElfForest", "Origin Tree", "Lyra", "Talk", "Lyra", null, 12, 15, 7m, 60m, null, null, 1, "[Chapter 5] Return with the Seals", "Main" },
                    { 42, null, "NotStarted", "Return to Elder Rowan. The forest still remembers the first healing flowers and the people they saved.", true, "ElfForest", "Elf Forest", "Elder Rowan", "Talk", "Lyra", null, 12, 15, 7m, 60m, null, null, 1, "[Chapter 5] The Forest Remembers", "Main" },
                    { 43, null, "NotStarted", "Gather 3 White Flowers from the old clearing so Elder Rowan can brew the last healing draught.", true, "ElfForest", "Elf Forest", "White Flower", "Collect", "Elder Rowan", null, 12, 15, 7m, 60m, null, null, 3, "[Chapter 5] Flowers Before Dawn", "Main" },
                    { 44, null, "NotStarted", "Bring the flowers to Elder Rowan, then return to Lyra with the finished draught.", true, "ElfForest", "Origin Tree", "Lyra", "Talk", "Elder Rowan", null, 12, 15, 7m, 60m, null, null, 1, "[Chapter 5] The Last Healing Draught", "Main" },
                    { 45, null, "NotStarted", "Set the four Seal Books and the last healing draught upon the Origin Tree and break the curse.", true, "ElfForest", "Origin Tree", "Origin Tree", "Interact", "Lyra", null, 12, 15, 40m, 300m, null, null, 1, "[Chapter 5] Heal the Origin Tree", "Main" },
                    { 46, null, "NotStarted", "Speak with Lyra one last time and learn what still waits beyond the healed forest.", true, "ElfForest", "Origin Tree", "Lyra", "Talk", "Lyra", null, 12, 15, 40m, 300m, null, null, 1, "[Chapter 5] A New Dawn", "Main" }
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
                    { 1, 55.0, "Archer", 2, 0f, 3.0, 8.0, "Physical", "Automatically fires in the direction the archer is facing.", true, "Accelerationarrow", "SingleTarget", "Active", 1 },
                    { 2, 115.0, "Archer", 5, 0f, 3.5, 14.0, "Physical", "Automatically fires in the direction the archer is facing.", true, "ArrowofLight", "SingleTarget", "Active", 1 },
                    { 3, 0.0, "Mage", 4, 0f, 0.0, 0.0, "Magical", "Heals allies within range.", true, "Holymagic", "Ally", "Buff", 1 },
                    { 4, 75.0, "Mage", 3, 0f, 3.0, 10.0, "Magical", "Casts a spell in the direction the character is facing.", true, "Purification", "SingleTarget", "Active", 1 },
                    { 5, 75.0, "Mage", 3, 0f, 3.0, 10.0, "Magical", "Selects and attacks a random monster within range.", true, "Stardust", "SingleTarget", "Active", 1 },
                    { 6, 115.0, "Knight", 5, 0f, 3.5, 14.0, "Physical", "Selects a target with the monster tag to attack.", true, "Lightsabers", "SingleTarget", "Active", 1 },
                    { 7, 95.0, "Knight", 4, 0f, 3.5, 12.0, "Physical", "Casts a spell in the direction the character is facing.", true, "LightWaves", "Area", "Active", 1 },
                    { 8, 0.0, "Knight", 8, 0f, 0.0, 0.0, "Magical", "Protects all allies within range.", true, "ProtectiveShield", "Ally", "Buff", 1 },
                    { 9, 180.0, "All", 8, 15f, 4.0, 22.0, "Magical", "Shared among all classes. Deals damage equal to 3x base damage. Increases corruption points by 15.", true, "DarkExplosion", "Area", "Active", 1 },
                    { 10, 140.0, "All", 6, 10f, 4.0, 18.0, "Magical", "Shared among all classes. Deals damage equal to 2x base damage. Increases corruption points by 10.", true, "DarkPoisonZone", "Area", "Active", 1 },
                    { 11, 115.0, "Archer", 5, 0f, 3.5, 14.0, "Physical", "Automatically fires in the direction the archer is facing.", true, "DeadlyCurse", "SingleTarget", "Active", 1 },
                    { 12, 55.0, "Mage", 2, 0f, 3.0, 8.0, "Magical", "Selects an area within range to attack.", true, "NightMagic", "Area", "Active", 1 },
                    { 13, 140.0, "All", 6, 8f, 4.0, 18.0, "Magical", "Shared among all classes. Deals damage equal to 3x base damage. Increases corruption points by 8.", true, "DeadlyExplosion", "SingleTarget", "Active", 1 },
                    { 14, 55.0, "Knight", 2, 0f, 3.0, 8.0, "Physical", "A short-range slash in the direction the knight is facing.", true, "BloodySlash", "SingleTarget", "Active", 1 },
                    { 15, 75.0, "Knight", 3, 0f, 3.0, 10.0, "Physical", "Selects an area within range to unleash an icy slash.", true, "FrozenSash", "Area", "Active", 1 },
                    { 16, 115.0, "Archer", 5, 0f, 3.5, 14.0, "Physical", "Summons a magical pumpkin trap that lasts 5 seconds. Explodes when touched by monsters or when duration expires, dealing AoE physical damage.", true, "PumpkinMagic", "Area", "Active", 1 },
                    { 17, 115.0, "Knight", 5, 0f, 3.5, 14.0, "Physical", "Throws an explosive pumpkin in a parabolic arc. Explodes on impact with any object, dealing AoE physical damage to monsters.", true, "PumpkinThrow", "Area", "Active", 1 },
                    { 18, 55.0, "Knight", 2, 0f, 3.0, 8.0, "Physical", "A short-range pumpkin slash in the direction the knight is facing.", true, "PumpkinSlash", "SingleTarget", "Active", 1 },
                    { 19, 55.0, "Mage", 2, 0f, 3.0, 8.0, "Magical", "Summons a magic pumpkin that explodes immediately at the target location, dealing light magical AoE damage with a short cooldown.", true, "BoomBoomPumpkin", "Area", "Active", 1 }
                });

            migrationBuilder.InsertData(
                table: "Contents",
                columns: new[] { "ContentId", "CategoryContentId", "CreatedAt", "CreatedByAccountAccountId", "CreatedByAccountId", "IsPublished", "PublishedAt", "Slug", "SubCategoryContentId", "Summary", "ThumbnailUrl", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("00000000-0000-0000-0000-000000000000"), true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "secrets-of-the-origin-tree-in-elf-forest", null, "Discover the source of life power for the Elven race and the rising threat of dark forces surrounding the ancient forest.", null, "Secrets of the Origin Tree in Elf Forest", null },
                    { 2, 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("00000000-0000-0000-0000-000000000000"), true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "legend-of-the-fire-elemental-seal-book", null, "Details on the location and decryption of the first Seal Book to unlock Fire Magic skills.", null, "Legend of the Fire Elemental Seal Book", null },
                    { 3, 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("00000000-0000-0000-0000-000000000000"), true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "chapter-1-awakening-in-the-deep-woods", null, "The beginning of the protagonist's journey — waking up with no memories and the 4 ancient books as the sole clue.", null, "Chapter 1: Awakening in the Deep Woods", null },
                    { 4, 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("00000000-0000-0000-0000-000000000000"), false, null, "guide-to-collecting-all-4-seal-books", null, "Overview of requirements, minimum levels, and boss encounters required to complete the ancient book collection.", null, "Guide to Collecting All 4 Seal Books", null },
                    { 5, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("00000000-0000-0000-0000-000000000000"), true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ecosystem-and-monsters-in-elf-forest", null, "A list of mystical creatures and monster stats that players will encounter throughout the Elf Forest region.", null, "Ecosystem and Monsters in Elf Forest", null }
                });

            migrationBuilder.InsertData(
                table: "EquipmentStats",
                columns: new[] { "EquipmentStatsId", "BaseAtk", "BaseDef", "BaseHp", "BonusAtk", "BonusAttackSpeed", "BonusCritDamage", "BonusCritRate", "BonusDamageBonus", "BonusDef", "BonusHp", "BonusMoveSpeed", "ItemId" },
                values: new object[,]
                {
                    { 5, 7, 0, 0, 3, 0, 15, 3, 0, 0, 0, 0, 5 },
                    { 6, 6, 0, 0, 2, 6, 10, 6, 0, 0, 0, 0, 6 },
                    { 7, 6, 0, 0, 3, 0, 20, 2, 2, 0, 0, 0, 7 },
                    { 8, 29, 0, 0, 13, 4, 30, 10, 4, 0, 0, 0, 8 },
                    { 9, 0, 6, 31, 0, 0, 0, 0, 0, 2, 14, 0, 9 },
                    { 10, 0, 4, 21, 0, 0, 0, 0, 0, 2, 9, 0, 10 },
                    { 11, 0, 4, 0, 0, 0, 0, 0, 0, 1, 0, 8, 11 },
                    { 12, 0, 32, 196, 0, 0, 0, 0, 0, 14, 84, 0, 12 },
                    { 13, 0, 14, 84, 0, 0, 0, 0, 0, 6, 36, 6, 13 },
                    { 14, 0, 7, 0, 0, 0, 25, 8, 0, 3, 0, 0, 14 },
                    { 15, 4, 3, 0, 2, 0, 0, 0, 2, 1, 0, 0, 15 },
                    { 16, 3, 2, 0, 1, 3, 0, 0, 0, 1, 0, 3, 16 },
                    { 17, 2, 0, 18, 1, 0, 6, 3, 0, 0, 7, 0, 17 },
                    { 18, 0, 4, 35, 0, 0, 0, 0, 0, 1, 15, 0, 18 },
                    { 901, 13, 0, 0, 5, 0, 18, 5, 0, 0, 0, 0, 901 },
                    { 902, 0, 11, 56, 0, 0, 0, 0, 0, 5, 24, 0, 902 },
                    { 903, 22, 0, 0, 10, 0, 25, 8, 0, 0, 0, 0, 903 },
                    { 904, 0, 21, 112, 0, 0, 0, 0, 0, 9, 48, 0, 904 },
                    { 905, 8, 11, 49, 4, 0, 10, 4, 0, 5, 21, 0, 905 },
                    { 906, 0, 28, 147, 0, 0, 0, 0, 0, 12, 63, 0, 906 },
                    { 907, 38, 0, 0, 17, 0, 35, 12, 0, 0, 0, 0, 907 },
                    { 908, 6, 24, 98, 2, 0, 15, 5, 0, 10, 42, 0, 908 }
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
                    { 909, 100.0, true, true, 29, 1, 1, 2 },
                    { 910, 100.0, true, true, 26, 1, 1, 7 },
                    { 911, 100.0, true, true, 27, 1, 1, 10 },
                    { 912, 100.0, true, true, 28, 1, 1, 15 },
                    { 951, 100.0, true, true, 22, 5, 1, 1 },
                    { 952, 100.0, true, true, 22, 5, 1, 2 },
                    { 953, 100.0, true, true, 22, 5, 1, 3 },
                    { 954, 100.0, true, true, 22, 5, 1, 4 },
                    { 955, 100.0, true, true, 22, 5, 1, 5 },
                    { 956, 100.0, true, true, 22, 5, 1, 6 },
                    { 957, 100.0, true, true, 22, 5, 1, 7 },
                    { 958, 100.0, true, true, 22, 5, 1, 8 },
                    { 959, 100.0, true, true, 22, 5, 1, 9 },
                    { 960, 100.0, true, true, 22, 5, 1, 10 },
                    { 961, 100.0, true, true, 22, 5, 1, 11 },
                    { 962, 100.0, true, true, 22, 5, 1, 12 },
                    { 963, 100.0, true, true, 22, 5, 1, 13 },
                    { 964, 100.0, true, true, 22, 5, 1, 14 },
                    { 965, 100.0, true, true, 22, 5, 1, 15 }
                });

            migrationBuilder.InsertData(
                table: "NPCDialogues",
                columns: new[] { "NPCDialogueId", "Content", "DisplayOrder", "IsActive", "LinkedQuestId", "LinkedShopItemId", "NPCId", "ResponseType" },
                values: new object[,]
                {
                    { 1, "Ah... a new face, and not one born of these woods. Welcome to the Elf Forest, traveler.", 1, true, 1, null, 1, "None" },
                    { 2, "For a thousand years this forest kept itself in peace. Now something gathers in the dark beneath the roots.", 3, true, 1, null, 1, "None" },
                    { 3, "So the Origin Tree chose you, and I must trust its choosing. I am Elder Rowan. Speak with me when you are ready to begin.", 6, true, 1, null, 1, "Quest" },
                    { 4, "Before anything else, I must beg medicine of you. Eleven of my village lie in the healing hall, and my stores are almost gone.", 1, true, 2, null, 1, "None" },
                    { 5, "By the old willow clearing grows a white flower that only opens where the air is still clean. Crushed with spring water, it is the one salve that answers this sickness.", 3, true, 2, null, 1, "None" },
                    { 6, "Go to the clearing and gather 3 White Flowers for me. Take care, even slimes wander there now.", 6, true, 2, null, 1, "Quest" },
                    { 10, "The technique is in you now, but a technique you have not called upon is no better than one you never learned.", 1, true, 4, null, 1, "None" },
                    { 11, "Every warrior in this world channels power through learned technique. Bare fists will not answer what waits out there.", 2, true, 4, null, 1, "None" },
                    { 12, "Open your Skill Panel and equip the First Elven Technique. Do not step past the treeline without it.", 4, true, 4, null, 1, "Quest" },
                    { 13, "Good. I can feel the power settled in you now. It must be tested before it is trusted.", 1, true, 5, null, 1, "None" },
                    { 14, "The outskirts crawl with little slimes. They were harmless once, now they hunt in packs.", 2, true, 5, null, 1, "None" },
                    { 15, "Go out and defeat 3 little slimes, then return and tell me what you felt out there.", 4, true, 5, null, 1, "Quest" },
                    { 19, "Come closer, brave one. I am Lyra, not elf and not ghost. I am the spirit of the Origin Tree itself.", 1, true, 7, null, 2, "None" },
                    { 20, "Look at my bark. The curse has reached my heartwood, and I am dying slowly, from the inside outward.", 2, true, 7, null, 2, "None" },
                    { 21, "Only the 4 Seal Books can cleanse me. You hold the first already, find the remaining three, and hurry!", 7, true, 7, null, 2, "Quest" },
                    { 22, "Heh... so you are the little errand-runner gathering up the Seal Books.", 1, true, 8, null, 3, "None" },
                    { 23, "You carry them and do not even know what they are, or whose hand cursed that tree.", 2, true, 8, null, 3, "None" },
                    { 24, "The truth waits through this portal. Follow me, or stay and keep watering a dying tree.", 4, true, 8, null, 3, "Quest" },
                    { 25, "Steady, traveler. That portal spat us both out here on the beach, and the cloaked one is long gone.", 1, true, 9, null, 4, "None" },
                    { 26, "We are far from the forest now, with no coin between us and no way back that I can see.", 2, true, 9, null, 4, "None" },
                    { 27, "Go and speak with Fa, the farmer just up the path. He always needs hands.", 4, true, 9, null, 4, "Quest" },
                    { 28, "Drake sent you? Good timing, stranger. My back is not what it was.", 1, true, 10, null, 7, "None" },
                    { 29, "The whole field came ripe at once, and the harvest cart leaves for the city at dusk.", 2, true, 10, null, 7, "None" },
                    { 30, "Collect 8 Enchanted Pumpkins for me and I will see you fed tonight.", 4, true, 10, null, 7, "Quest" },
                    { 31, "Eight, and not one bruised. You work like a farmhand born, not a wanderer.", 1, true, 11, null, 7, "None" },
                    { 32, "Now the hard half of the job. These are owed at the city gate before nightfall.", 2, true, 11, null, 7, "None" },
                    { 33, "Take them to the guard Tristan at the ruined city, and tell him Fa sent you.", 4, true, 11, null, 7, "Quest" },
                    { 34, "Halt! Who goes... ah, pumpkins from Fa. Set them down, you may be the last delivery this gate sees.", 1, true, 12, null, 5, "None" },
                    { 35, "Something is wrong inside. No bells, no market noise, no smoke from a single chimney since dawn.", 2, true, 12, null, 5, "None" },
                    { 36, "Go in and look at the fallen with your own eyes. Then come back and tell me the truth of it.", 4, true, 12, null, 5, "Quest" },
                    { 37, "All of them? Every soul in the city, cut down where they stood? Gods, I stood here and heard nothing.", 1, true, 13, null, 5, "None" },
                    { 38, "No bandit crew does this in one night. Whatever walked in there was not a man with a sword.", 2, true, 13, null, 5, "None" },
                    { 39, "Find Arthur and report what you saw. Go, before whatever did this moves on to the next town.", 4, true, 13, null, 5, "Quest" },
                    { 49, "The dragon is dead. I felt it go — the whole city breathed out at once. Thank you.", 1, true, 20, null, 6, "None" },
                    { 50, "You want to know about the cloaked one. Yes. He passed through here before the dragon ever came.", 2, true, 20, null, 6, "None" },
                    { 51, "He went north, into the frozen lands. Follow him to the Frozen Mountains. I will hold this city.", 4, true, 20, null, 6, "Quest" },
                    { 52, "So you are the one Cedric put a name to. I am Roselyn Aurora, and what is left of this kingdom is mine to hold.", 1, true, 23, null, 8, "None" },
                    { 53, "It was not always like this. Once these were the quiet lands — snow fell all winter and killed nothing, and no one here carried a sword.", 2, true, 23, null, 8, "None" },
                    { 54, "You want me to take you seriously, stranger? They are out on the fields right now. Kill 8 of them. Then we can talk about who you are.", 4, true, 21, null, 14, "Quest" },
                    { 55, "The fields are quiet. My people walked out to the grain stores without an escort for the first time in a month. Come here — I owe you a word.", 5, true, 21, null, 14, "Reward" },
                    { 56, "You passed the statue at my gate. King Aurelian — the ancient king this whole kingdom still honours, and the reason any of it is still standing.", 5, true, 23, null, 8, "None" },
                    { 57, "Carry the Magic Flour I gave you to him — and ask him everything you have been asking me.", 8, true, 23, null, 8, "Quest" },
                    { 58, "The Queen's flour, and a courier still breathing. Welcome, hero from far away. I have been on this mountain thirty years chasing one question: how four holy books vanished out of a sealed world.", 1, true, 23, null, 9, "Reward" },
                    { 59, "Thirty years, and the answer keeps moving. But there is a nearer trouble, and it will not wait for my research.", 1, true, 24, null, 9, "None" },
                    { 60, "Climb the peak and put down all 5. May I ask that of a stranger?", 4, true, 24, null, 9, "Quest" },
                    { 61, "Halt! This ground is under ban and no one goes in. Who are you?", 1, true, 25, null, 10, "None" },
                    { 62, "A knight sent to recover the stolen books... I see. Then you did not climb all this way for the view. You mean to go inside the ban.", 2, true, 25, null, 10, "None" },
                    { 63, "Go, then. The way is open, and I will keep the road behind you.", 5, true, 25, null, 10, "Quest" },
                    { 67, "Those bones were my neighbours once. I left Tide-Knell for the king's army, and returned to find every soul walking without flesh.", 1, true, 28, null, 11, "None" },
                    { 68, "I have guarded this valley for years, cutting down friends who rise again by moonrise. Help me put 12 of them down.", 2, true, 28, null, 11, "None" },
                    { 69, "Recover five keepsakes from Tide-Knell. Let me remember the people I am forced to fight.", 1, true, 29, null, 11, "Quest" },
                    { 70, "Find the memories and my last letter. Natalie deserves to know I came home too late, not that I abandoned her.", 1, true, 32, null, 11, "Quest" },
                    { 73, "The voice still whispers. Find three traces around the well and make it answer for the promise it made.", 1, true, 31, null, 12, "Quest" },
                    { 76, "Natalie's key opens the bridge. Use it at the gate, then let the Elf Guard finish what I could not.", 1, true, 34, null, 11, "Quest" },
                    { 77, "The prisoner was King Aderyn, my closest friend. He accepted two Seal Books so the forests would not bear the whole curse.", 1, true, 35, null, 13, "None" },
                    { 78, "Gather five Ancient Leaves. They may open the crypt without destroying what remains of him.", 2, true, 35, null, 13, "Quest" },
                    { 79, "Recover four relics from the old sealing party. I have called my guilt duty for centuries.", 1, true, 36, null, 13, "Quest" },
                    { 81, "For one breath, I heard Aderyn thank you. Go home and tell the forest that he is finally free.", 1, true, 40, null, 13, "Reward" },
                    { 82, "Cleanse three cursed roots in the king's garden. His last living seed is still below the island.", 1, true, 37, null, 15, "Quest" },
                    { 83, "Read three memory fragments. Aderyn chose imprisonment to protect the world; the records must survive him.", 1, true, 38, null, 15, "Quest" },
                    { 85, "You came back carrying all four seals. The forest is still breathing, but only just. Bring them to the roots and I will show you what remains.", 1, true, 41, null, 2, "Quest" },
                    { 89, "The ice dragons have stopped behaving like animals. Something is steering them — and they have begun coming down on the people below.", 2, true, 24, null, 9, "None" },
                    { 91, "The books cannot heal a memory they do not understand. Return to Elder Rowan; he remembers the first flowers and the lives they saved.", 1, true, 42, null, 2, "Quest" },
                    { 94, "The Origin Tree at our heart is sickening. Its leaves fall in high summer, and the animals no longer sleep here.", 4, true, 1, null, 1, "None" },
                    { 95, "Where those flowers still bloom, the curse has not yet reached. They are medicine and warning both.", 4, true, 2, null, 1, "None" },
                    { 97, "Set it where your hand can reach it without thinking. In a fight you will not have time to remember.", 3, true, 4, null, 1, "None" },
                    { 98, "They are the curse's smallest children. Where they spread, the soil dies behind them.", 3, true, 5, null, 1, "None" },
                    { 100, "Long ago the elders bound an ancient power into four such books. That binding has broken, and the leak is what poisons me.", 5, true, 7, null, 2, "None" },
                    { 101, "The elves told you a story with the ugly parts cut out. I can show you what they buried.", 3, true, 8, null, 3, "None" },
                    { 102, "This is farming country. Folk here trade a day of work for supper, and honest work is easy to find.", 3, true, 9, null, 4, "None" },
                    { 103, "Mind the ones that glow faintly. An enchanted pumpkin keeps a lantern lit all winter, that is why the city pays.", 3, true, 10, null, 7, "None" },
                    { 104, "I would carry them myself, but no one from this farm has come back from that road in a week.", 3, true, 11, null, 7, "None" },
                    { 105, "I am Tristan, and my orders bind me to this gate. I cannot take one step past it, even now.", 3, true, 12, null, 5, "None" },
                    { 106, "There is one person left who might stand against it. Arthur, the silver knight, camped in the old ruins.", 3, true, 13, null, 5, "None" },
                    { 110, "He carries something that should have stayed sealed. Wherever he walks, the land sickens behind him.", 3, true, 20, null, 6, "None" },
                    { 111, "Then the codex came. It took the four Seal Books and drank the strength of the Origin Tree, and everything it passed turned wrong. What you are standing in is only what that war left behind.", 3, true, 23, null, 8, "None" },
                    { 112, "He spent his life holding back the codex's leavings so that this little peace would outlive him. It did. Barely.", 6, true, 23, null, 8, "None" },
                    { 113, "Five of them circle the peak. Young, all of them. Whatever holds their reins made them hungry in a way no beast should be.", 3, true, 24, null, 9, "None" },
                    { 114, "Then I will not stand in your way. But hear this: two ancient things still live in there, and both of them are dangerous.", 3, true, 25, null, 10, "None" },
                    { 127, "You did not wander in here. The forest awakened you at its edge, and the forest does not wake strangers.", 2, true, 1, null, 1, "None" },
                    { 128, "A hundred of my people have walked past those roots and heard nothing. You heard it call before you opened your eyes. No one else could.", 5, true, 1, null, 1, "None" },
                    { 129, "The rot came up through the well water. The children fell first, then whoever carried them. Salve buys them days, no more than that.", 2, true, 2, null, 1, "None" },
                    { 130, "My knees will not carry me that far anymore. Bring me the flowers and my people live through the week. That is the whole of it.", 5, true, 2, null, 1, "None" },
                    { 133, "Now show me what you took from the swamp... ah. Rowan sent you here not knowing, did he. Hold it up, child.", 3, true, 7, null, 2, "None" },
                    { 134, "This is a Seal Book. The first of them to see daylight in an age, and no living elf has ever held one.", 4, true, 7, null, 2, "None" },
                    { 135, "Four books, scattered and guarded. They are not treasure, they are the lock on my heartwood. Nothing else will save me.", 6, true, 7, null, 2, "None" },
                    { 136, "The streets are quieter. But quiet is not the same as ready, and a dragon is not a ghoul in an alley.", 1, true, 15, null, 6, "None" },
                    { 137, "I fought one once believing I was ready. You have seen what is left of me. So you will earn it in four trials.", 2, true, 15, null, 6, "None" },
                    { 138, "The robbers took the eastern camp the night the city fell. They prey on whoever still crawls out of here alive.", 3, true, 15, null, 6, "None" },
                    { 139, "Clear 6 of them from the Robber Camp. That is the first trial. Go.", 4, true, 15, null, 6, "Quest" },
                    { 140, "One trial down, and you came back on your own feet again. Good.", 1, true, 16, null, 6, "None" },
                    { 141, "The second is the haunted quarter - ghosts, necromancers, and the red guard. My own men, still standing their posts, still dead.", 2, true, 16, null, 6, "None" },
                    { 142, "I could never walk that street again. A dragon will not care how brave you felt in daylight.", 3, true, 16, null, 6, "None" },
                    { 143, "Put down 10 in the Haunted Quarter. Second trial. Move.", 4, true, 16, null, 6, "Quest" },
                    { 144, "Two trials, two returns. I am beginning to believe the city might keep you.", 1, true, 17, null, 6, "None" },
                    { 145, "The third is the ground south of the ruins. Goblins hold it, spear and axe together, and they fight as a pack.", 2, true, 17, null, 6, "None" },
                    { 146, "A dragon will not come at you alone either. Learn to hold when more than one thing wants you dead.", 3, true, 17, null, 6, "None" },
                    { 147, "Break 3 of them in the Goblin Grounds. Third trial.", 4, true, 17, null, 6, "Quest" },
                    { 152, "Cedric's company is the whole of my army. Farmers holding spears, and a captain who has stopped asking me for reinforcements because he knows there are none.", 4, true, 23, null, 8, "None" },
                    { 153, "I cannot answer what you really came to ask. But there is a priest near here — Zephyr. He studies the old magics, and he has studied them longer than I have been queen.", 7, true, 23, null, 8, "None" },
                    { 154, "The peak is silent. Sit down, hero — I will tell you the part I have never told the Queen.", 5, true, 24, null, 9, "Reward" },
                    { 155, "The codex may not have been evil to begin with. I think it was something else once, and a power turned it.", 1, true, 25, null, 9, "None" },
                    { 156, "Dark magic. Strength that gives freely and takes greed as its price. Am I certain? No. I am not certain of any of it.", 2, true, 25, null, 9, "None" },
                    { 157, "But look at the beasts here. I studied them before the great war — gentle things, no harm in them. After it, not one of them was itself. Something settled into them, like a taint in the blood.", 3, true, 25, null, 9, "None" },
                    { 158, "You want somewhere else to look? There is one place. Dangerous, and cut off from the capital long ago — the forbidden land north of here, The Doomed Land of Snow. Find the guard Roland at the boundary stones. He is a friend of mine.", 4, true, 25, null, 9, "None" },
                    { 159, "What are they? I do not truly know. The legend says one is a giant made of stone. The other is a mystery — since the old hero sealed this place, no one has dared walk in far enough to find out.", 4, true, 25, null, 10, "None" },
                    { 161, "So that is what was sleeping under my ban. Say it again, slowly — I want to be sure I have it right before I write it down.", 1, true, 27, null, 10, "None" },
                    { 162, "The golem was gentle once. Not a weapon, not a guardian — he lived close to people and helped anyone who asked him.", 2, true, 27, null, 10, "None" },
                    { 163, "And the fairy — he pulled her out of the hands of spirit-traders, men who sold her kind by weight. She never forgot it. She stayed at his side from that day to repay him.", 3, true, 27, null, 10, "None" },
                    { 164, "Then the darkness came down on these lands, and he stood against it — for the villages, not for himself. It was stronger. It did not kill him; it put him into a sleep that went on for years.", 4, true, 27, null, 10, "None" },
                    { 165, "The fairy was terrified. She went to the hero of that age and begged him for help — that is the hero whose statue the Queen still keeps at her gate.", 5, true, 27, null, 10, "None" },
                    { 166, "Two months after the codex fled this place, the golem finally woke — and it was her doing. She had just learned a blessing, and she spent it on him.", 6, true, 27, null, 10, "None" },
                    { 167, "But what woke was not what fell asleep. He behaved wrongly. He struck at the people he used to carry water for.", 7, true, 27, null, 10, "None" },
                    { 168, "And yet he was clear-headed sometimes. Clear enough to understand what was creeping through him — and to decide what to do about it.", 8, true, 27, null, 10, "None" },
                    { 169, "He walked into the forbidden land himself. Shut himself in. Wiped out every road and marker so no one could follow him in. That is the ban I have been standing guard over for eleven years — a cage a man built for himself.", 9, true, 27, null, 10, "None" },
                    { 170, "And she went in with him. Knowing what he had become. She never left his side, right up until you.", 10, true, 27, null, 10, "None" },
                    { 171, "Then it is confirmed, and I will carry it to the Queen: no demon, no evil born evil. Someone was corrupted by a dark power — and everything since has followed from that.", 11, true, 27, null, 10, "Quest" },
                    { 172, "You said you would come back for those two and set them right. Hold to that, hero. I will keep the ban open for the day you do.", 12, true, 27, null, 10, "Reward" },
                    { 173, "Far enough. I am Cedric — captain, if you are being generous. These fields are mine to hold until the Queen can spare someone better.", 1, true, 21, null, 14, "None" },
                    { 174, "You came up the ice road, so you saw the state of it. Something crawled out of the snow after the war and it has been eating this valley one field at a time.", 2, true, 21, null, 14, "None" },
                    { 175, "The ice slimes come further in every night and freeze whatever they touch. Half my company are farmers holding spears they do not know how to use. I have buried four of them this month.", 3, true, 21, null, 14, "None" },
                    { 181, "You came back to the old roots. The villagers who survived still carry the scent of those first flowers in their homes. There is enough life left for one final draught.", 1, true, 42, null, 1, "Reward" },
                    { 182, "Gather 3 White Flowers from the old clearing. This time they will not only keep a few people alive; they will give the Origin Tree something clean to remember.", 1, true, 43, null, 1, "Quest" },
                    { 183, "Three flowers, as before. I have brewed the last draught. Take it to Lyra at the Origin Tree; the rest belongs to the one who called you here.", 1, true, 44, null, 1, "Reward" },
                    { 184, "I can feel Rowan's draught in your hands. Bring it to the roots, and set the four books where the wound first opened.", 1, true, 44, null, 2, "Quest" },
                    { 185, "Set the four Seal Books and the last healing draught upon the Origin Tree. If the forest accepts them, the curse will break.", 1, true, 45, null, 2, "Quest" },
                    { 186, "The forest remembers every hand that carried these seals: Rowan, the silent cities, the frozen guardians, Natalie, and Aderyn. You have given them all another dawn.", 1, true, 46, null, 2, "None" },
                    { 187, "But the codex was not masterless. Something taught it to drink from the Origin Tree, and that presence is still somewhere beyond these woods.", 2, true, 46, null, 2, "Reward" }
                });

            migrationBuilder.InsertData(
                table: "QuestRewardItems",
                columns: new[] { "QuestRewardItemId", "ItemId", "Quantity", "QuestId" },
                values: new object[,]
                {
                    { 2, 15, 1, 8 },
                    { 6, 13, 1, 27 },
                    { 8, 12, 1, 45 }
                });

            migrationBuilder.InsertData(
                table: "QuestRewardSkills",
                columns: new[] { "QuestRewardSkillId", "QuestId", "SkillId" },
                values: new object[,]
                {
                    { 1, 2, 1 },
                    { 2, 2, 5 },
                    { 3, 2, 7 },
                    { 9, 11, 16 },
                    { 10, 11, 19 },
                    { 11, 11, 17 },
                    { 12, 11, 18 },
                    { 17, 28, 14 },
                    { 18, 28, 12 },
                    { 19, 28, 11 }
                });

            migrationBuilder.InsertData(
                table: "Quests",
                columns: new[] { "QuestId", "BossMonsterId", "DefaultStatus", "Description", "IsActive", "MapName", "ObjectiveLocation", "ObjectiveTarget", "ObjectiveType", "QuestGiverName", "RegionName", "RequiredLevel", "RewardExperience", "RewardGems", "RewardGold", "RewardItemId", "RewardSkillId", "TargetAmount", "Title", "Type" },
                values: new object[,]
                {
                    { 3, null, "NotStarted", "Bring the gathered flowers back to Elder Rowan. In return he will teach you the first strike an elf ever learns.", true, "ElfForest", "Elf Forest", "Elder Rowan", "Talk", "Elder Rowan", null, 1, 15, 3m, 20m, null, 10, 1, "[Chapter 1] Deliver the White Flowers", "Main" },
                    { 6, 2, "NotStarted", "The slimes were only fleeing something worse. A Swamp Demon broods in the deep woods over some old relic, and the water rots around it. Kill it and take whatever it is guarding.", true, "ElfForest", "Deep Woods", "Swamp Demon", "Defeat", "Elder Rowan", null, 2, 65, 8m, 60m, null, null, 1, "[Chapter 1] Slay the Swamp Demon", "Main" },
                    { 14, null, "NotStarted", "Arthur's wounds run deeper than his armour and his power is sealed away; he cannot fight for the city. He can, however, make you strong enough to. Clear his training dungeon.", true, "AutumnPumpkin", "Dungeon", "Dungeon_2", "Explore", "Arthur", null, 4, 10, 4m, 30m, 18, 9, 1, "[Chapter 2] Train in the Old Dungeon", "Main" },
                    { 18, 22, "NotStarted", "The goblins you broke were only a warband, and every warband answers to someone. Their warlord still holds the Goblin Grounds. Kill him and the last trial is yours.", true, "AutumnPumpkin", "Goblin Grounds", "Goblin Warlord", "Defeat", "Arthur", null, 4, 35, 16m, 120m, null, null, 1, "[Chapter 2] Trial IV: The Goblin Warlord", "Main" },
                    { 19, 7, "NotStarted", "Arthur admits you now fight as well as he once did — and tells you what truly broke the city. A dragon nests in the ruins. End it.", true, "AutumnPumpkin", "Ruined City", "Red Dragon", "Defeat", "Arthur", null, 5, 10, 16m, 120m, null, null, 1, "[Chapter 2] Slay the Dragon", "Main" },
                    { 22, null, "NotStarted", "The fields are clear, and Cedric has stopped calling you stranger. He says the Queen has been searching for someone with the strength to stand against what is coming, and that he intends to give her your name. Speak with Roselyn Aurora at the citadel.", true, "FrozenMountain", "Snow Fields", "Roselyn Aurora Queen", "Talk", "Cedric", null, 6, 15, 5m, 40m, 31, null, 1, "[Chapter 3] A Word to the Queen", "Main" },
                    { 26, 10, "NotStarted", "Two ancient things wait inside the ban: a giant of stone, and the spirit that never leaves his side. Defeat them both and take the Golem Seal Book.", true, "FrozenMountain", "Forbidden Zone", "Golem Boss / Ice Fairy", "Defeat", "Roland", null, 8, 15, 24m, 180m, null, null, 2, "[Chapter 3] The Sealed Guardians", "Main" },
                    { 30, null, "NotStarted", "Natalie's ghost asks you to dig beside the old well and recover the skull buried there.", true, "AbandonedCastle", "Tide-Knell", "Skull", "Interact", "Natalie", null, 9, 15, 6m, 50m, 32, null, 1, "[Chapter 4] The Skull by the Well", "Main" },
                    { 33, null, "NotStarted", "Bury Natalie beneath the ivy tree and forgive the lonely child who opened the seal.", true, "AbandonedCastle", "Tide-Knell", "Ivy Tree", "Interact", "Natalie", null, 10, 15, 6m, 50m, 33, null, 1, "[Chapter 4] Lay Natalie to Rest", "Main" },
                    { 39, 15, "NotStarted", "Defeat the UnderKing and release the hero beneath the crown.", true, "AbandonedCastle", "Deserted Island", "UnderKing", "Defeat", "Elf Guard", null, 12, 15, 32m, 240m, null, null, 1, "[Chapter 4] Free the UnderKing", "Main" }
                });

            migrationBuilder.InsertData(
                table: "ShopItems",
                columns: new[] { "ShopItemId", "AvailableFrom", "AvailableTo", "Currency", "DailyPurchaseLimit", "IsActive", "ItemId", "Price", "ShopSection", "Stock", "WeeklyPurchaseLimit" },
                values: new object[,]
                {
                    { 1, null, null, "Gold", 0, true, 19, 25m, "Fixed", -1, 0 },
                    { 2, null, null, "Gold", 0, true, 20, 70m, "Fixed", -1, 0 },
                    { 3, null, null, "Gold", 0, true, 21, 50m, "Fixed", -1, 0 },
                    { 4, null, null, "Gold", 0, true, 22, 40m, "Fixed", -1, 0 },
                    { 5, null, null, "Gold", 0, true, 5, 120m, "Fixed", -1, 0 },
                    { 6, null, null, "Gold", 0, true, 6, 120m, "Fixed", -1, 0 },
                    { 7, null, null, "Gold", 0, true, 7, 120m, "Fixed", -1, 0 },
                    { 8, null, null, "Gold", 0, true, 9, 100m, "Fixed", -1, 0 },
                    { 9, null, null, "Gold", 0, true, 10, 85m, "Fixed", -1, 0 },
                    { 10, null, null, "Gold", 0, true, 16, 80m, "Fixed", -1, 0 },
                    { 11, null, null, "Gold", 0, true, 15, 110m, "Fixed", -1, 0 },
                    { 12, null, null, "Gold", 0, true, 17, 70m, "Fixed", -1, 0 },
                    { 13, null, null, "Gold", 0, true, 11, 160m, "Fixed", -1, 0 },
                    { 14, null, null, "Gold", 0, true, 18, 170m, "Fixed", -1, 0 },
                    { 15, null, null, "Gold", 0, true, 14, 450m, "Fixed", -1, 0 },
                    { 16, null, null, "Gold", 0, true, 13, 800m, "Fixed", -1, 0 },
                    { 17, null, null, "Gold", 0, true, 8, 700m, "Fixed", -1, 0 },
                    { 18, null, null, "Gold", 0, true, 12, 1800m, "Fixed", -1, 0 },
                    { 19, null, null, "Gems", 0, true, 4, 100m, "Fixed", -1, 0 }
                });

            migrationBuilder.InsertData(
                table: "BlockContents",
                columns: new[] { "Id", "BlockType", "Caption", "ContentData", "ContentId", "CreatedAt", "Description", "IsActive", "MediaUrl", "SortOrder", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "Text", null, "Located at the heart of the Elf Forest, the Origin Tree once provided magical energy to all living beings. However, an ancient curse is causing its leaves to wither away...", 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null, 1, "", null },
                    { 2, "Image", "origin_tree_pixel.png", null, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null, 2, "", null },
                    { 3, "Text", null, "The four Seal Books contain remnants of ancient power. The Fire elemental book is currently sealed deep within the abandoned fortress of Autumn Pumpkin...", 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null, 1, "", null },
                    { 4, "Text", null, "You awaken in a cursed forest with no memories. Four Seal Books, four realms, and a fading Origin Tree — this is the only path forward.", 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null, 1, "", null },
                    { 5, "Image", "awakening_scene.png", null, 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null, 2, "", null },
                    { 6, "Text", null, "Each Seal Book corresponds to a realm on the map: Elf Forest (Earth), Frozen Mountain (Ice), Autumn Pumpkin (Fire), and Abandoned Castle (Shadow)...", 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null, 1, "", null },
                    { 7, "Text", null, "Although a starter area, Elf Forest hides many dangers from corrupted forest spirits...", 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null, 1, "", null },
                    { 8, "Image", "monster_list_pixel.png", null, 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null, 2, "", null }
                });

            migrationBuilder.InsertData(
                table: "NPCDialogues",
                columns: new[] { "NPCDialogueId", "Content", "DisplayOrder", "IsActive", "LinkedQuestId", "LinkedShopItemId", "NPCId", "ResponseType" },
                values: new object[,]
                {
                    { 7, "Back already? Let me see your hands... ah, you found them.", 1, true, 3, null, 1, "None" },
                    { 8, "Not a petal bruised. Three flowers, three doses. The healing hall will have them before nightfall, and the children will sleep.", 2, true, 3, null, 1, "None" },
                    { 9, "Hold still. Breathe with the roots, as we do... there. It is yours now, with an old elf's thanks.", 5, true, 3, null, 1, "Reward" },
                    { 16, "You handled them cleanly. But the slimes are only spillage from something far worse.", 1, true, 6, null, 1, "None" },
                    { 17, "Deep in the swamp lies a Demon. The water rots around it, and the corruption creeps closer each night.", 2, true, 6, null, 1, "None" },
                    { 18, "Destroy the Swamp Demon and take whatever it is guarding. Stopping the rot at its source is what matters.", 5, true, 6, null, 1, "Quest" },
                    { 40, "Lower your guard, I am no enemy. I am Arthur, once called the silver knight of this city.", 1, true, 14, null, 6, "None" },
                    { 41, "I met the thing that emptied these streets. It broke something inside me and sealed my power away.", 2, true, 14, null, 6, "None" },
                    { 42, "Clear my old training dungeon. Survive it, and I will give you everything I have left. Go!", 4, true, 14, null, 6, "Quest" },
                    { 46, "You came back quieter than you left. That is how I know the fighting took hold in you.", 1, true, 19, null, 6, "None" },
                    { 47, "Then hear the rest of it. The monsters were never the cause. Something older nests above the ruins.", 2, true, 19, null, 6, "None" },
                    { 48, "Finish what I could not. Climb to its nest and slay the dragon!", 4, true, 19, null, 6, "Quest" },
                    { 64, "So it was here all along. Now I know why my order was told to guard this place and never once to enter it.", 1, true, 26, null, 10, "None" },
                    { 65, "And the seal still holds. One of the four old Seal Books lies at the heart of the ban — the Golem Seal Book.", 2, true, 26, null, 10, "None" },
                    { 66, "Put down both of them and take the Golem Seal Book. It is worth more in your hands than under my ban.", 4, true, 26, null, 10, "Quest" },
                    { 71, "My mother died, my father went to war, and Tide-Knell called an orphan bad luck. Then a voice beneath the well called me by name.", 1, true, 30, null, 12, "None" },
                    { 72, "It promised friends who would never abandon me. I believed it. Please dig beside the well and lift out my skull.", 2, true, 30, null, 12, "Quest" },
                    { 74, "I was lonely, but the choice was mine. If you can still pity me, bury me beneath the ivy tree.", 1, true, 33, null, 12, "Quest" },
                    { 75, "If the earth accepts me, Tide-Knell may sleep. Take my Mystic Key and go to the island.", 2, true, 33, null, 12, "Reward" },
                    { 80, "Aderyn was a hero before darkness ate his mind. Defeat the UnderKing and free the man beneath the crown.", 1, true, 39, null, 13, "Quest" },
                    { 96, "We do not teach our craft outside the bloodline. For you I will break that rule. Let me teach you the First Elven Technique.", 4, true, 3, null, 1, "None" },
                    { 99, "Our scouts say the beast broods over some old relic down there. A seal of some kind, they think. I am no scholar, and none of them got close enough to be sure.", 3, true, 6, null, 1, "None" },
                    { 107, "I cannot lift my blade again. But a blade is only steel, what matters is the hand that learns to swing it.", 3, true, 14, null, 6, "None" },
                    { 109, "A dragon. It is the thing that broke this city, and the thing that broke me. I have carried that shame for years.", 3, true, 19, null, 6, "None" },
                    { 115, "Two guardians stand over it: the stone giant, and the spirit that never leaves his side. The elders left them there to keep every hand off that book, mine included.", 3, true, 26, null, 10, "None" },
                    { 131, "I sent a stranger into cursed woods for people you had never met, and you went without asking payment. You have proven yourself.", 3, true, 3, null, 1, "None" },
                    { 132, "What it is, and whether it matters, I cannot tell you. Lyra at the Origin Tree is older than every elf alive. Bring it to her and she will know.", 4, true, 6, null, 1, "None" },
                    { 148, "Three trials done. You have fought packs, and ghosts, and men. One thing is left that you have not fought.", 1, true, 18, null, 6, "None" },
                    { 149, "The warband you broke answers to a warlord, and he is still down there holding what is left of them.", 2, true, 18, null, 6, "None" },
                    { 150, "One enemy, bigger than you, who does not retreat and does not tire. That is the shape of a dragon fight. Learn it here where I can still pull you out.", 3, true, 18, null, 6, "None" },
                    { 151, "Kill the Goblin Warlord. Finish the last trial and I will tell you everything.", 4, true, 18, null, 6, "Quest" },
                    { 160, "You have the book. But you did not come out of there looking like a man who won a fight — come here and tell me what you saw.", 5, true, 26, null, 10, "Reward" },
                    { 176, "I had you down as one more sellsword chasing coin up the ice road. It seems you are not. I have watched men with ten years of service do less than you did out there.", 1, true, 22, null, 14, "None" },
                    { 177, "So I will tell you what I would not have told you this morning. The Queen has been searching — quietly — for someone with the strength to stand against what is coming for this kingdom.", 2, true, 22, null, 14, "None" },
                    { 178, "She has asked every captain on this mountain, and every captain has sent back the same answer: no one. I am tired of writing that answer.", 3, true, 22, null, 14, "None" },
                    { 179, "Go up to the citadel and stand in front of Roselyn Aurora. I am sending a runner ahead of you — for once with a name in it.", 4, true, 22, null, 14, "Quest" },
                    { 180, "Cedric's runner reached me an hour before you did. That man does not praise people, so I read it twice. Stay a moment — there are things you should hear from me and not from a soldier.", 1, true, 22, null, 8, "Reward" }
                });

            migrationBuilder.InsertData(
                table: "QuestRewardItems",
                columns: new[] { "QuestRewardItemId", "ItemId", "Quantity", "QuestId" },
                values: new object[,]
                {
                    { 1, 10, 1, 6 },
                    { 3, 17, 1, 18 },
                    { 4, 11, 1, 19 },
                    { 5, 14, 1, 26 },
                    { 7, 8, 1, 39 }
                });

            migrationBuilder.InsertData(
                table: "QuestRewardSkills",
                columns: new[] { "QuestRewardSkillId", "QuestId", "SkillId" },
                values: new object[,]
                {
                    { 4, 6, 2 },
                    { 5, 6, 3 },
                    { 6, 6, 4 },
                    { 7, 6, 8 },
                    { 8, 6, 6 },
                    { 13, 19, 9 },
                    { 14, 19, 10 },
                    { 15, 19, 13 },
                    { 16, 22, 15 }
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
                name: "IX_PlayerQuests_PlayerProfileId_QuestId",
                table: "PlayerQuests",
                columns: new[] { "PlayerProfileId", "QuestId" },
                unique: true);

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
                name: "IX_WorldChatMessages_Feed",
                table: "WorldChatMessages",
                columns: new[] { "IsHidden", "SentAt", "WorldChatMessageId" },
                descending: new[] { false, true, true });

            migrationBuilder.CreateIndex(
                name: "IX_WorldChatMessages_ReportedById",
                table: "WorldChatMessages",
                column: "ReportedById");

            migrationBuilder.CreateIndex(
                name: "IX_WorldChatMessages_SenderId",
                table: "WorldChatMessages",
                column: "SenderId");
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
