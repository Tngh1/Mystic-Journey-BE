using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class UnlockChapter1SwampDemonQuest_v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 6,
                column: "RequiredLevel",
                value: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 6,
                column: "RequiredLevel",
                value: 2);
        }
    }
}
