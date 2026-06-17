using DAL.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DAL.Migrations
{
    [DbContext(typeof(MysticJourneyDbContext))]
    [Migration("20260616100000_AddWorldQuestNpcMetadata")]
    public partial class AddWorldQuestNpcMetadata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MapName",
                table: "Quests",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "ElfForest");

            migrationBuilder.AddColumn<string>(
                name: "ObjectiveLocation",
                table: "Quests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObjectiveTarget",
                table: "Quests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObjectiveType",
                table: "Quests",
                type: "text",
                nullable: false,
                defaultValue: "Explore");

            migrationBuilder.AddColumn<string>(
                name: "QuestGiverName",
                table: "Quests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegionName",
                table: "Quests",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

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
                        principalColumn: "QuestId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_NPCDialogues_ShopItems_LinkedShopItemId",
                        column: x => x.LinkedShopItemId,
                        principalTable: "ShopItems",
                        principalColumn: "ShopItemId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Quests_MapName",
                table: "Quests",
                column: "MapName");

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
                name: "IX_NPCs_MapName",
                table: "NPCs",
                column: "MapName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NPCDialogues");

            migrationBuilder.DropTable(
                name: "NPCs");

            migrationBuilder.DropIndex(
                name: "IX_Quests_MapName",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "MapName",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "ObjectiveLocation",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "ObjectiveTarget",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "ObjectiveType",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "QuestGiverName",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "RegionName",
                table: "Quests");
        }
    }
}
