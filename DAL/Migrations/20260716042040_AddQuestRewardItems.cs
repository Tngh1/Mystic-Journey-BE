using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestRewardItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_QuestRewardItems_ItemId",
                table: "QuestRewardItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestRewardItems_QuestId_ItemId",
                table: "QuestRewardItems",
                columns: new[] { "QuestId", "ItemId" },
                unique: true);
            migrationBuilder.Sql(@"INSERT INTO ""QuestRewardItems"" (""QuestId"", ""ItemId"", ""Quantity"")
SELECT ""QuestId"", ""RewardItemId"", 1
FROM ""Quests""
WHERE ""RewardItemId"" IS NOT NULL
ON CONFLICT (""QuestId"", ""ItemId"") DO NOTHING;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuestRewardItems");
        }
    }
}
