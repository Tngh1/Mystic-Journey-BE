using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateQuest24_QuestGiver : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 70,
                columns: new[] { "DisplayOrder", "LinkedQuestId", "ResponseType" },
                values: new object[] { 4, 23, "Reward" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 24,
                columns: new[] { "Description", "QuestGiverName", "RewardItemId" },
                values: new object[] { "Dig up the skull near the old well.", "Natalie", 32 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 70,
                columns: new[] { "DisplayOrder", "LinkedQuestId", "ResponseType" },
                values: new object[] { 1, 24, "Quest" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 24,
                columns: new[] { "Description", "QuestGiverName", "RewardItemId" },
                values: new object[] { "Go to Tide-Knell village, meet Natalie, and dig up the skull near the old well.", "Valiant Warrior", null });
        }
    }
}
