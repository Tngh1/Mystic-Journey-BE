using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateQuest25_ResponseTypeQuest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 75,
                column: "ResponseType",
                value: "Quest");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 75,
                column: "ResponseType",
                value: "None");
        }
    }
}
