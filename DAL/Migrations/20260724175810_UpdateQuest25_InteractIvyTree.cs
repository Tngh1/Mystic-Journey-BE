using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateQuest25_InteractIvyTree : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 73,
                column: "Content",
                value: "(A weathered suicide letter lies where Natalie once stood...)");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 74,
                column: "Content",
                value: "Thank you for bringing my remains back to my homeland. Please bury me under the ivy tree in my courtyard.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 75,
                columns: new[] { "Content", "ResponseType" },
                values: new object[] { "The ancient power leak was my doing. I am deeply sorry. Take this Mystic Key. It will unlock the gates to the castle on the deserted island.", "None" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 25,
                columns: new[] { "ObjectiveTarget", "ObjectiveType" },
                values: new object[] { "Ivy Tree", "Interact" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 73,
                column: "Content",
                value: "Thank you for finding my remains. Now I can finally rest in peace.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 74,
                column: "Content",
                value: "The ancient power leak was my doing. I am so sorry for the chaos.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 75,
                columns: new[] { "Content", "ResponseType" },
                values: new object[] { "Take this key. It will unlock the doors to the island castle. Farewell.", "Reward" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 25,
                columns: new[] { "ObjectiveTarget", "ObjectiveType" },
                values: new object[] { "Natalie", "Talk" });
        }
    }
}
