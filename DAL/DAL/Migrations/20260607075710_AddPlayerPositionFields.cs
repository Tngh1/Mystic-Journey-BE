using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerPositionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastMapName",
                table: "PlayerProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "PositionX",
                table: "PlayerProfiles",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PositionY",
                table: "PlayerProfiles",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastMapName",
                table: "PlayerProfiles");

            migrationBuilder.DropColumn(
                name: "PositionX",
                table: "PlayerProfiles");

            migrationBuilder.DropColumn(
                name: "PositionY",
                table: "PlayerProfiles");
        }
    }
}
