using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddEnergySystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Energy",
                table: "PlayerProfiles",
                newName: "MaxEnergy");

            migrationBuilder.AddColumn<int>(
                name: "CurrentEnergy",
                table: "PlayerProfiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastEnergyUpdateTime",
                table: "PlayerProfiles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentEnergy",
                table: "PlayerProfiles");

            migrationBuilder.DropColumn(
                name: "LastEnergyUpdateTime",
                table: "PlayerProfiles");

            migrationBuilder.RenameColumn(
                name: "MaxEnergy",
                table: "PlayerProfiles",
                newName: "Energy");
        }
    }
}
