using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateElfGuardPosition2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "NPCs",
                keyColumn: "NPCId",
                keyValue: 13,
                columns: new[] { "PositionX", "PositionY" },
                values: new object[] { 1.0160000324249268, -1.0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "NPCs",
                keyColumn: "NPCId",
                keyValue: 13,
                columns: new[] { "PositionX", "PositionY" },
                values: new object[] { 4.9145519999999996, 1.045023 });
        }
    }
}
