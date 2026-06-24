using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class CalendarDailyLogin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClaimedDaysStr",
                table: "PlayerDailyLogins",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CurrentMonth",
                table: "PlayerDailyLogins",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrentYear",
                table: "PlayerDailyLogins",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClaimedDaysStr",
                table: "PlayerDailyLogins");

            migrationBuilder.DropColumn(
                name: "CurrentMonth",
                table: "PlayerDailyLogins");

            migrationBuilder.DropColumn(
                name: "CurrentYear",
                table: "PlayerDailyLogins");
        }
    }
}
