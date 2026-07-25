using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateQuest29_EndFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 29,
                columns: new[] { "Description", "RewardExperience", "RewardGold", "Title" },
                values: new object[] { "Talk to Lyra about the 4 Seal Books.", 50, 50m, "[Chapter 1] Return with the Seals" });

            migrationBuilder.InsertData(
                table: "Quests",
                columns: new[] { "QuestId", "BossMonsterId", "DefaultStatus", "Description", "IsActive", "MapName", "ObjectiveLocation", "ObjectiveTarget", "ObjectiveType", "QuestGiverName", "RegionName", "RequiredLevel", "RewardExperience", "RewardGems", "RewardGold", "RewardItemId", "RewardSkillId", "TargetAmount", "Title", "Type" },
                values: new object[,]
                {
                    { 30, null, "NotStarted", "Use the 4 Seal Books on the Origin Tree.", true, "ElfForest", "Elf Forest", "Origin Tree", "Interact", "Lyra", null, 12, 250, 5m, 250m, null, null, 1, "[Chapter 1] Heal the Origin Tree", "Main" },
                    { 31, null, "NotStarted", "Talk to Lyra. The Origin Tree is saved. To be continued...", true, "ElfForest", "Origin Tree", "Lyra", "Talk", "Lyra", null, 12, 200, 5m, 200m, null, null, 1, "[Chapter 1] A New Dawn", "Main" }
                });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 86,
                columns: new[] { "Content", "ResponseType" },
                values: new object[] { "Please, hurry! Use the books on the Origin Tree to cleanse the corruption.", "Quest" });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 87,
                columns: new[] { "Content", "DisplayOrder", "LinkedQuestId", "ResponseType" },
                values: new object[] { "The curse is breaking... The Origin Tree is finally healing!", 1, 31, "None" });

            migrationBuilder.InsertData(
                table: "NPCDialogues",
                columns: new[] { "NPCDialogueId", "Content", "DisplayOrder", "IsActive", "LinkedQuestId", "LinkedShopItemId", "NPCId", "ResponseType" },
                values: new object[] { 88, "Thank you! The Origin Tree is saved. But this is not the end... To be continued.", 2, true, 31, null, 2, "Reward" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 88);

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 29,
                columns: new[] { "Description", "RewardExperience", "RewardGold", "Title" },
                values: new object[] { "Talk to Lyra and use the 4 Seal Books to cleanse the tree. \"To be continued\".", 500, 500m, "[Chapter 1] Save the Origin Tree" });

            migrationBuilder.DeleteData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 31);

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 86,
                columns: new[] { "Content", "ResponseType" },
                values: new object[] { "The curse is breaking... The Origin Tree is finally healing!", "None" });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 87,
                columns: new[] { "Content", "DisplayOrder", "LinkedQuestId", "ResponseType" },
                values: new object[] { "Thank you! The Origin Tree is saved. But this is not the end... To be continued.", 3, 29, "Reward" });
        }
    }
}
