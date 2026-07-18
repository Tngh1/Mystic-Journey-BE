using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddMonthYearToDailyLoginReward : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Month",
                table: "DailyLoginRewards",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "DailyLoginRewards",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Month",
                table: "DailyLoginRewards");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "DailyLoginRewards");
        }
    }
}
